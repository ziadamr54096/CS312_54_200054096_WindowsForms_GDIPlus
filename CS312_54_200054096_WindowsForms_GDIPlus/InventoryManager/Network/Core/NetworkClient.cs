using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace InventoryManager.Network;

/// <summary>Result of "Test connection": a one-shot probe that does NOT register a device on the server.</summary>
public sealed record ProbeResult(bool Ok, string Message, int ConnectMs, int RoundTripMs, string ServerName, int DeviceCount);

/// <summary>
/// TCP client: connects to a server by IP + port, registers with CONNECT, then runs a receive loop and a heartbeat in the background.
/// Events are raised on background threads (the UI layer marshals them). Every failure is reported with a friendly message.
/// </summary>
public sealed class NetworkClient : IDisposable
{
    readonly SemaphoreSlim _sendLock = new(1, 1);
    readonly ConcurrentDictionary<string, TaskCompletionSource<NetMessage>> _pending = new();
    TcpClient? _tcp;
    NetworkStream? _stream;
    CancellationTokenSource? _cts;
    Task? _receive, _heartbeat;
    int _state;                 // 0 = disconnected, 1 = connecting, 2 = connected
    int _finished = 1;          // 1 = no active session to finish
    long _lastRxTicks;
    long _lastListSeq;
    string? _closeReason;
    volatile bool _requested;
    volatile IReadOnlyList<DeviceInfo> _devices = Array.Empty<DeviceInfo>();

    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromSeconds(35);
    public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(8);

    public bool IsConnected => Volatile.Read(ref _state) == 2;
    public string DeviceName { get; private set; } = "";
    public string DeviceId { get; private set; } = "";
    public string ServerName { get; private set; } = "";
    public IPEndPoint? ServerEndPoint { get; private set; }
    public DateTime? ConnectedAtUtc { get; private set; }
    /// <summary>Other devices connected to the same server (from the server's DEVICE_LIST).</summary>
    public IReadOnlyList<DeviceInfo> Devices => _devices;

    public event Action<NetMessage>? MessageReceived;                 // CHAT, BROADCAST, INVENTORY_UPDATE, LOW_STOCK_ALERT from the server
    public event Action<IReadOnlyList<DeviceInfo>>? DevicesChanged;
    public event Action<string, bool>? Disconnected;                  // (reason, requestedByUser)
    public event Action<string>? Log;

    // ================================================================ connect ================================================================

    public async Task ConnectAsync(IPAddress ip, int port, string deviceName, string? accessCode, string? user = null)
    {
        if (Interlocked.CompareExchange(ref _state, 1, 0) != 0)
            throw new NetworkException("This device is already connected (or connecting). Disconnect first.");
        TcpClient? tcp = null;
        try
        {
            string? nameError = NetUtil.ValidateDeviceName(deviceName);
            if (nameError != null) throw new NetworkException(nameError);
            if (port < 1 || port > 65535) throw new NetworkException($"Port {port} is out of range. Use a number between 1 and 65535.");

            tcp = new TcpClient(AddressFamily.InterNetwork) { NoDelay = true };
            var connect = tcp.ConnectAsync(ip, port);
            var winner = await Task.WhenAny(connect, Task.Delay(ConnectTimeout)).ConfigureAwait(false);
            if (winner != connect)
            {
                tcp.Close();
                _ = connect.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                throw new NetworkException(NetUtil.TimeoutMessage(ip, port));
            }
            await connect.ConfigureAwait(false);

            var stream = tcp.GetStream();
            var hello = NetMessage.Create(MessageTypes.Connect, deviceName, $"{deviceName} wants to connect",
                data: new Dictionary<string, string>
                {
                    ["name"] = deviceName.Trim(), ["user"] = user ?? "", ["code"] = accessCode ?? "", ["app"] = "InventoryManager/1.0"
                });
            await FrameCodec.WriteAsync(stream, hello, CancellationToken.None).ConfigureAwait(false);
            var reply = await ReadReplyAsync(stream, tcp, hello.Id).ConfigureAwait(false);
            if (reply == null) throw new NetworkException("The server closed the connection during registration.");
            if (reply.Type != MessageTypes.ServerResponse) throw new NetworkException("The other computer is not a Stockora server (unexpected reply).");
            if (!reply.Succeeded()) throw new NetworkException(string.IsNullOrWhiteSpace(reply.Text) ? "The server refused the connection." : reply.Text);

            _tcp = tcp; _stream = stream;
            DeviceName = deviceName.Trim(); DeviceId = reply.Get("deviceId"); ServerName = reply.Get("serverName", ip.ToString());
            ServerEndPoint = new IPEndPoint(ip, port);
            ConnectedAtUtc = DateTime.UtcNow;
            _devices = Array.Empty<DeviceInfo>();
            Interlocked.Exchange(ref _lastListSeq, 0);
            _requested = false; _closeReason = null;
            Interlocked.Exchange(ref _lastRxTicks, DateTime.UtcNow.Ticks);
            _cts = new CancellationTokenSource();
            Interlocked.Exchange(ref _finished, 0);
            Volatile.Write(ref _state, 2);
            var ct = _cts.Token;
            _receive = Task.Run(() => ReceiveLoopAsync(stream));
            _heartbeat = Task.Run(() => HeartbeatLoopAsync(ct));
            RaiseLog($"Connected to {ServerName} at {ip}:{port}.");
        }
        catch (NetworkException) { tcp?.Close(); Volatile.Write(ref _state, 0); throw; }
        catch (SocketException ex) { tcp?.Close(); Volatile.Write(ref _state, 0); throw new NetworkException(NetUtil.FriendlyConnectError(ex, ip, port), ex); }
        catch (Exception ex) when (ex is IOException or InvalidDataException or ObjectDisposedException or EndOfStreamException)
        { tcp?.Close(); Volatile.Write(ref _state, 0); throw new NetworkException($"The connection to {ip}:{port} failed: {ex.Message}", ex); }
        catch (Exception) { tcp?.Close(); Volatile.Write(ref _state, 0); throw; }
    }

    /// <summary>Waits (within ConnectTimeout overall) for the SERVER_RESPONSE to our CONNECT, skipping any unsolicited push that arrives first.</summary>
    async Task<NetMessage?> ReadReplyAsync(NetworkStream stream, TcpClient tcp, string requestId)
    {
        var deadline = DateTime.UtcNow + ConnectTimeout;
        while (true)
        {
            var left = deadline - DateTime.UtcNow;
            if (left <= TimeSpan.Zero) { tcp.Close(); throw new NetworkException("The server did not answer the connection request in time."); }
            var m = await ReadWithTimeoutAsync(stream, tcp, left, "The server did not answer the connection request in time.").ConfigureAwait(false);
            if (m == null) return null;
            if (m.Type == MessageTypes.ServerResponse && (m.ReplyTo == requestId || m.ReplyTo == null)) return m;
            if (m.Type is MessageTypes.DeviceList or MessageTypes.Pong) continue;
            return m;       // anything else is not a Stockora handshake
        }
    }

    static async Task<NetMessage?> ReadWithTimeoutAsync(NetworkStream stream, TcpClient tcp, TimeSpan timeout, string timeoutMessage)
    {
        var read = FrameCodec.ReadAsync(stream, CancellationToken.None);
        var winner = await Task.WhenAny(read, Task.Delay(timeout)).ConfigureAwait(false);
        if (winner != read)
        {
            tcp.Close();
            _ = read.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
            throw new NetworkException(timeoutMessage);
        }
        return await read.ConfigureAwait(false);
    }

    // ================================================================ background loops ================================================================

    async Task ReceiveLoopAsync(NetworkStream stream)
    {
        string reason = "The server closed the connection.";
        try
        {
            while (true)       // ends on EOF, failure, or when the connection is closed locally
            {
                var m = await FrameCodec.ReadAsync(stream, CancellationToken.None).ConfigureAwait(false);
                if (m == null) break;
                Interlocked.Exchange(ref _lastRxTicks, DateTime.UtcNow.Ticks);
                if (m.Type == MessageTypes.Disconnect)
                {
                    reason = string.IsNullOrWhiteSpace(m.Text) ? "The server disconnected this device." : m.Text;
                    _closeReason = reason;      // the server's own explanation wins over any later local error (e.g. a heartbeat write racing the close)
                    break;
                }
                Dispatch(m);
            }
        }
        catch (InvalidDataException ex) { reason = "Protocol error: " + ex.Message; }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException or EndOfStreamException)
        { reason = _closeReason ?? "The connection to the server was lost."; }
        catch (Exception ex) { reason = "Unexpected error: " + ex.Message; }
        finally { Finish(_closeReason ?? reason); }
    }

    void Dispatch(NetMessage m)
    {
        switch (m.Type)
        {
            case MessageTypes.ServerResponse when m.ReplyTo != null && _pending.TryRemove(m.ReplyTo, out var tcs):
                tcs.TrySetResult(m);
                break;
            case MessageTypes.Pong:
                break;
            case MessageTypes.DeviceList:
                if (long.TryParse(m.Get("seq"), out long seq))
                {
                    if (seq <= Interlocked.Read(ref _lastListSeq)) break;          // an older list that arrived late: ignore it
                    Interlocked.Exchange(ref _lastListSeq, seq);
                }
                var all = DeviceListCodec.Decode(m.Text);
                _devices = all.Where(d => !d.Name.Equals(DeviceName, StringComparison.OrdinalIgnoreCase)).ToList();
                try { DevicesChanged?.Invoke(_devices); } catch (Exception) { }
                break;
            default:
                try { MessageReceived?.Invoke(m); } catch (Exception) { }
                break;
        }
    }

    async Task HeartbeatLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try { await Task.Delay(HeartbeatInterval, ct).ConfigureAwait(false); }
            catch (OperationCanceledException) { break; }
            if (DateTime.UtcNow - new DateTime(Interlocked.Read(ref _lastRxTicks), DateTimeKind.Utc) > IdleTimeout)
            { Close("The server stopped answering (connection timed out)."); break; }
            try { await SendAsync(NetMessage.Create(MessageTypes.Ping, DeviceName)).ConfigureAwait(false); }
            catch (Exception) { break; }       // SendAsync already closed the connection
        }
    }

    // ================================================================ sending ================================================================

    public async Task SendAsync(NetMessage m)
    {
        var stream = _stream;
        if (!IsConnected || stream == null) throw new NetworkException("Not connected to a server.");
        m.Sender = DeviceName;
        await _sendLock.WaitAsync().ConfigureAwait(false);
        try { await FrameCodec.WriteAsync(stream, m, CancellationToken.None).ConfigureAwait(false); }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException)
        {
            Close("Sending failed - the connection was lost.");
            throw new NetworkException("The connection to the server was lost.", ex);
        }
        finally { _sendLock.Release(); }
    }

    /// <summary>Sends a message and waits for the server's SERVER_RESPONSE to it.</summary>
    public async Task<NetMessage> RequestAsync(NetMessage request)
    {
        var tcs = new TaskCompletionSource<NetMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pending[request.Id] = tcs;
        try
        {
            await SendAsync(request).ConfigureAwait(false);
            var winner = await Task.WhenAny(tcs.Task, Task.Delay(RequestTimeout)).ConfigureAwait(false);
            if (winner != tcs.Task) throw new NetworkException("The server did not answer in time.");
            return await tcs.Task.ConfigureAwait(false);
        }
        finally { _pending.TryRemove(request.Id, out _); }
    }

    // ================================================================ disconnect ================================================================

    public async Task DisconnectAsync()
    {
        if (!IsConnected) return;
        _requested = true;
        try
        {
            var bye = SendAsync(NetMessage.Create(MessageTypes.Disconnect, DeviceName, "Goodbye"));
            await Task.WhenAny(bye, Task.Delay(1000)).ConfigureAwait(false);
            _ = bye.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
            try { _tcp?.Client.Shutdown(SocketShutdown.Send); } catch (Exception) { }   // graceful: FIN after the goodbye (no RST)
        }
        catch (Exception) { }
        var recv = _receive;                       // the server closes its side after reading DISCONNECT -> our receive loop ends
        if (recv != null) await Task.WhenAny(recv, Task.Delay(1500)).ConfigureAwait(false);
        Close("Disconnected.");                    // forced only if the server did not answer
        if (recv != null) await Task.WhenAny(recv, Task.Delay(1000)).ConfigureAwait(false);
    }

    void Close(string reason)
    {
        if (_closeReason == null) _closeReason = reason;
        try { _tcp?.Close(); } catch (Exception) { }
    }

    /// <summary>Runs exactly once per session: resets state, fails waiting requests, raises Disconnected.</summary>
    void Finish(string reason)
    {
        if (Interlocked.Exchange(ref _finished, 1) != 0) return;
        Volatile.Write(ref _state, 0);
        try { _cts?.Cancel(); } catch (Exception) { }
        try { _tcp?.Close(); } catch (Exception) { }
        foreach (var kv in _pending)
            if (_pending.TryRemove(kv.Key, out var tcs)) tcs.TrySetException(new NetworkException("The connection to the server was lost."));
        _devices = Array.Empty<DeviceInfo>();
        ConnectedAtUtc = null;
        bool requested = _requested;
        RaiseLog(requested ? "Disconnected from the server." : "Connection lost: " + reason);
        try { Disconnected?.Invoke(reason, requested); } catch (Exception) { }
    }

    public void Dispose()
    {
        try { if (IsConnected) Task.Run(DisconnectAsync).Wait(4000); } catch (Exception) { }
        Close("Disposed.");
        _sendLock.Dispose();
    }

    // ================================================================ "Test connection" ================================================================

    /// <summary>Connects, sends PING, waits for PONG, disconnects. Never throws; the result carries a friendly message.</summary>
    public static async Task<ProbeResult> ProbeAsync(IPAddress ip, int port, TimeSpan timeout)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        using var tcp = new TcpClient(AddressFamily.InterNetwork) { NoDelay = true };
        try
        {
            var connect = tcp.ConnectAsync(ip, port);
            if (await Task.WhenAny(connect, Task.Delay(timeout)).ConfigureAwait(false) != connect)
            {
                tcp.Close();
                _ = connect.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                return new ProbeResult(false, NetUtil.TimeoutMessage(ip, port), 0, 0, "", 0);
            }
            await connect.ConfigureAwait(false);
            int connectMs = (int)sw.ElapsedMilliseconds;

            var stream = tcp.GetStream();
            var rtt = System.Diagnostics.Stopwatch.StartNew();
            await FrameCodec.WriteAsync(stream, NetMessage.Create(MessageTypes.Ping, "probe"), CancellationToken.None).ConfigureAwait(false);
            var pong = await ReadWithTimeoutAsync(stream, tcp, timeout, "The server accepted the connection but did not answer the test.").ConfigureAwait(false);
            rtt.Stop();
            if (pong == null || pong.Type != MessageTypes.Pong)
                return new ProbeResult(false, "Something is listening on that port, but it is not a Stockora server.", connectMs, 0, "", 0);
            int.TryParse(pong.Get("devices"), out int devices);
            string name = pong.Get("serverName", ip.ToString());
            return new ProbeResult(true, $"Server '{name}' answered in {Math.Max(1, (int)rtt.ElapsedMilliseconds)} ms ({devices} device(s) connected).",
                                   connectMs, Math.Max(1, (int)rtt.ElapsedMilliseconds), name, devices);
        }
        catch (SocketException ex) { return new ProbeResult(false, NetUtil.FriendlyConnectError(ex, ip, port), 0, 0, "", 0); }
        catch (NetworkException ex) { return new ProbeResult(false, ex.Message, 0, 0, "", 0); }
        catch (Exception ex) when (ex is IOException or InvalidDataException or ObjectDisposedException or EndOfStreamException)
        { return new ProbeResult(false, $"The test failed: {ex.Message}", 0, 0, "", 0); }
    }

    void RaiseLog(string text) { try { Log?.Invoke(text); } catch (Exception) { } }
}
