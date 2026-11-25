using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace InventoryManager.Network;

/// <summary>
/// TCP server: accepts many clients at once (one async handler per connection), registers devices, relays chat/broadcast messages,
/// executes inventory requests through IInventoryGateway, detects unexpected disconnections and cleans everything up on Stop.
/// Events are raised on background threads - the UI layer (NetworkController) marshals them to the UI thread.
/// </summary>
public sealed class NetworkServer : IDisposable
{
    readonly object _gate = new();
    readonly List<ClientConnection> _connections = new();
    readonly ConcurrentDictionary<int, Task> _handlers = new();
    TcpListener? _listener;
    CancellationTokenSource? _cts;
    Task? _acceptLoop, _watchdog;
    int _handlerSeq;
    long _listSeq;
    volatile bool _stopping;

    // ---- settings (set before Start) ----
    public string ServerName { get; set; } = NetUtil.DefaultDeviceName();
    public string? AccessCode { get; set; }
    public int MaxClients { get; set; } = 50;
    public TimeSpan HandshakeTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromSeconds(35);     // clients send a heartbeat every 10 s
    public TimeSpan SendTimeout { get; set; } = TimeSpan.FromSeconds(5);
    public TimeSpan WatchdogInterval { get; set; } = TimeSpan.FromSeconds(1);
    /// <summary>Decides which remote addresses may connect. Default: private LAN ranges only (the server must not be reachable from the internet).</summary>
    public Func<IPAddress, bool> AddressFilter { get; set; } = NetUtil.IsPrivateLan;
    public IInventoryGateway? Inventory { get; set; }

    // ---- state ----
    public bool IsRunning { get; private set; }
    public IPEndPoint? LocalEndPoint { get; private set; }
    public DateTime? StartedAtUtc { get; private set; }

    // ---- events (background threads) ----
    public event Action<DeviceInfo>? DeviceConnected;
    public event Action<DeviceInfo, string>? DeviceDisconnected;
    /// <summary>A CHAT addressed to the server, or a BROADCAST sent by a client. (Message already carries the verified sender.)</summary>
    public event Action<NetMessage>? MessageReceived;
    public event Action<string>? Log;

    public IReadOnlyList<DeviceInfo> GetDevices()
    {
        lock (_gate) return _connections.Where(c => c.Device != null).Select(c => c.Device!).OrderBy(d => d.ConnectedAtUtc).ToList();
    }

    // ================================================================ start / stop ================================================================

    public void Start(IPAddress bindAddress, int port)
    {
        lock (_gate) { if (IsRunning) throw new InvalidOperationException("The server is already running."); }
        if (port < 0 || port > 65535) throw new NetworkException($"Port {port} is out of range. Use a number between 1 and 65535.");

        var listener = new TcpListener(bindAddress, port);
        if (Environment.OSVersion.Platform == PlatformID.Win32NT) listener.ExclusiveAddressUse = true;   // a second listener on the same port must fail on Windows
        try { listener.Start(64); }
        catch (SocketException ex)
        {
            try { listener.Stop(); } catch (Exception) { }
            throw new NetworkException(NetUtil.FriendlyListenError(port, ex), ex);
        }

        _stopping = false;
        _cts = new CancellationTokenSource();
        _listener = listener;
        LocalEndPoint = (IPEndPoint)listener.LocalEndpoint;
        StartedAtUtc = DateTime.UtcNow;
        IsRunning = true;
        var ct = _cts.Token;
        _acceptLoop = Task.Run(() => AcceptLoopAsync(listener, ct));
        _watchdog = Task.Run(() => WatchdogAsync(ct));
        RaiseLog($"Server started, listening on {LocalEndPoint}.");
    }

    public async Task StopAsync()
    {
        TcpListener? listener; CancellationTokenSource? cts; List<ClientConnection> all;
        lock (_gate)
        {
            if (!IsRunning) return;
            IsRunning = false; _stopping = true;
            listener = _listener; cts = _cts; _listener = null;
            all = _connections.ToList();
        }
        try { listener?.Stop(); } catch (Exception) { }     // releases the port and ends the pending Accept (no new clients)

        // devices that are in the middle of registering finish their welcome first (milliseconds), so everybody gets the goodbye
        var waitSw = System.Diagnostics.Stopwatch.StartNew();
        while (waitSw.ElapsedMilliseconds < 1000 && all.Any(c => c.Device != null && !c.Ready)) await Task.Delay(5).ConfigureAwait(false);

        // tell registered devices why the connection ends (best effort, short timeout)
        var bye = NetMessage.Create(MessageTypes.Disconnect, ServerName, "The server was stopped.");
        await Task.WhenAll(all.Where(c => c.Device != null && c.Ready).Select(c => TrySendAsync(c, bye))).ConfigureAwait(false);

        // Graceful shutdown: half-close (send FIN) and give the clients a moment to read the goodbye and close their side.
        // Closing a socket that still has unread incoming data would send a TCP RST, and the peer could lose the goodbye message.
        foreach (var c in all)
        {
            if (c.Device == null) { c.Close("The server was stopped."); continue; }
            if (c.CloseReason == null) c.CloseReason = "The server was stopped.";
            try { c.Tcp.Client.Shutdown(SocketShutdown.Send); } catch (Exception) { }
        }
        var graceful = new List<Task>(_handlers.Values);
        if (graceful.Count > 0) await Task.WhenAny(Task.WhenAll(graceful), Task.Delay(1500)).ConfigureAwait(false);
        try { cts?.Cancel(); } catch (Exception) { }                    // stops the watchdog
        foreach (var c in all) c.Close("The server was stopped.");     // anything still open is closed now

        var running = new List<Task>(_handlers.Values);
        if (_acceptLoop != null) running.Add(_acceptLoop);
        if (_watchdog != null) running.Add(_watchdog);
        await Task.WhenAny(Task.WhenAll(running), Task.Delay(3000)).ConfigureAwait(false);

        lock (_gate) { _connections.Clear(); }
        LocalEndPoint = null; StartedAtUtc = null;
        cts?.Dispose();
        RaiseLog("Server stopped.");
    }

    public void Dispose()
    {
        try { Task.Run(StopAsync).Wait(5000); } catch (Exception) { }
    }

    // ================================================================ accept / handle ================================================================

    async Task AcceptLoopAsync(TcpListener listener, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            TcpClient tcp;
            try { tcp = await listener.AcceptTcpClientAsync().ConfigureAwait(false); }
            catch (ObjectDisposedException) { break; }
            catch (InvalidOperationException) { break; }
            catch (SocketException) when (ct.IsCancellationRequested || _stopping) { break; }
            catch (SocketException ex)
            {
                RaiseLog("Accept error: " + ex.Message);
                try { await Task.Delay(100, ct).ConfigureAwait(false); } catch (OperationCanceledException) { break; }
                continue;
            }
            int id = Interlocked.Increment(ref _handlerSeq);
            _handlers[id] = Task.Run(async () =>
            {
                try { await HandleClientAsync(tcp).ConfigureAwait(false); }
                finally { _handlers.TryRemove(id, out _); }
            });
        }
    }

    async Task HandleClientAsync(TcpClient tcp)
    {
        ClientConnection? conn = null;
        string reason = "Connection closed";
        try
        {
            var remote = tcp.Client.RemoteEndPoint as IPEndPoint;
            if (remote == null) { tcp.Close(); return; }
            if (!AddressFilter(remote.Address))
            {
                RaiseLog($"Rejected a connection from {remote.Address}: it is not a private LAN address. (Tick 'Allow non-private addresses' if this is intended.)");
                tcp.Close(); return;
            }
            tcp.NoDelay = true;
            try { tcp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true); } catch (Exception) { }

            conn = new ClientConnection(tcp, remote);
            lock (_gate)
            {
                if (_stopping || _connections.Count >= Math.Max(MaxClients, 1) * 2) { conn.Close(); return; }   // flood protection
                _connections.Add(conn);
            }

            // ---- phase 1: handshake (PING probes are answered; the first CONNECT registers the device) ----
            NetMessage? m;
            while (true)
            {
                m = await FrameCodec.ReadAsync(conn.Stream, CancellationToken.None).ConfigureAwait(false);
                if (m == null) { reason = "Probe or aborted connection"; return; }
                conn.Touch();
                if (m.Type == MessageTypes.Ping) { await conn.SendAsync(MakePong(m)).ConfigureAwait(false); continue; }
                break;
            }
            if (m.Type != MessageTypes.Connect)
            {
                await conn.SendAsync(NetMessage.Response(m, false, "The first message must be CONNECT.", ServerName)).ConfigureAwait(false);
                reason = "Protocol error: no CONNECT"; return;
            }
            var (device, error) = TryRegister(conn, m);
            if (device == null)
            {
                RaiseLog($"Refused {remote.Address}: {error}");
                await conn.SendAsync(NetMessage.Response(m, false, error!, ServerName)).ConfigureAwait(false);
                reason = "Refused: " + error; return;
            }

            await conn.SendWelcomeAsync(NetMessage.Response(m, true, $"Connected to {ServerName}.", ServerName,
                new Dictionary<string, string> { ["serverName"] = ServerName, ["deviceId"] = device.Id })).ConfigureAwait(false);   // also marks the connection Ready
            RaiseDeviceConnected(device);
            _ = BroadcastDeviceListAsync();

            // ---- phase 2: normal operation ----
            reason = "Connection closed by the device";
            while (true)       // ends when the peer closes, the connection fails, or the server closes it (Stop / watchdog)
            {
                m = await FrameCodec.ReadAsync(conn.Stream, CancellationToken.None).ConfigureAwait(false);
                if (m == null) break;
                conn.Touch();
                if (m.Type == MessageTypes.Disconnect) { reason = "Device disconnected"; break; }
                await ProcessAsync(conn, m).ConfigureAwait(false);
            }
        }
        catch (InvalidDataException ex)
        {
            reason = "Protocol error: " + ex.Message;
            if (conn != null) { try { await conn.SendAsync(NetMessage.Response(null, false, "Invalid message: " + ex.Message, ServerName)).ConfigureAwait(false); } catch (Exception) { } }
        }
        catch (Exception ex) when (ex is IOException or SocketException or ObjectDisposedException or EndOfStreamException)
        {
            reason = conn?.CloseReason ?? "Connection lost";
        }
        catch (Exception ex)
        {
            reason = "Unexpected error: " + ex.Message;
            RaiseLog("Unexpected error in a client handler: " + ex);
        }
        finally
        {
            Cleanup(conn, conn?.CloseReason ?? reason);
        }
    }

    (DeviceInfo? device, string? error) TryRegister(ClientConnection conn, NetMessage m)
    {
        string name = m.Get("name").Trim();
        string? nameError = NetUtil.ValidateDeviceName(name);
        if (nameError != null) return (null, nameError);
        string? code = AccessCode;
        if (!string.IsNullOrEmpty(code) && !ConstantTimeEquals(m.Get("code"), code))
            return (null, "Incorrect access code.");

        lock (_gate)
        {
            if (_stopping) return (null, "The server is shutting down.");
            int registered = _connections.Count(c => c.Device != null);
            if (registered >= MaxClients) return (null, $"The server is full ({MaxClients} devices).");
            if (_connections.Any(c => c.Device != null && c.Device.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return (null, $"A device named '{name}' is already connected. Choose another device name.");   // duplicate registration
            var d = new DeviceInfo(Guid.NewGuid().ToString("N").Substring(0, 8), name, conn.Remote.Address.ToString(), conn.Remote.Port, DateTime.UtcNow);
            conn.Device = d;
            return (d, null);
        }
    }

    void Cleanup(ClientConnection? conn, string reason)
    {
        if (conn == null) return;
        DeviceInfo? device;
        lock (_gate) { _connections.Remove(conn); device = conn.Device; }
        conn.Close(reason);
        if (device != null)
        {
            RaiseDeviceDisconnected(device, reason);
            if (!_stopping) _ = BroadcastDeviceListAsync();
        }
    }

    // ================================================================ message processing ================================================================

    async Task ProcessAsync(ClientConnection c, NetMessage m)
    {
        var dev = c.Device!;
        switch (m.Type)
        {
            case MessageTypes.Ping:
                await c.SendAsync(MakePong(m)).ConfigureAwait(false);
                break;

            case MessageTypes.Chat:
                await HandleChatAsync(c, m).ConfigureAwait(false);
                break;

            case MessageTypes.Broadcast:
            {
                // Two independent copies carrying the VERIFIED sender: one for the server's own UI, one for the other devices
                // (the fan-out must not re-stamp it as the server, and the UI must not see later mutations).
                RaiseMessageReceived(m.Relay(dev.Name, dev.Ip));
                int delivered = await FanOutAsync(m.Relay(dev.Name, dev.Ip), dev.Id).ConfigureAwait(false);
                await c.SendAsync(NetMessage.Response(m, true, $"Delivered to {delivered} other device(s).", ServerName,
                    new Dictionary<string, string> { ["delivered"] = delivered.ToString() })).ConfigureAwait(false);
                break;
            }

            case MessageTypes.InventoryUpdate:
                await HandleInventoryAsync(c, m).ConfigureAwait(false);
                break;

            default:   // CONNECT again, SERVER_RESPONSE, LOW_STOCK_ALERT, DEVICE_LIST ... are not accepted from clients
                await c.SendAsync(NetMessage.Response(m, false, $"Message type {m.Type} is not accepted from clients.", ServerName)).ConfigureAwait(false);
                break;
        }
    }

    async Task HandleChatAsync(ClientConnection c, NetMessage m)
    {
        var dev = c.Device!;
        string target = (m.Target ?? "").Trim();
        var relayed = m.Relay(dev.Name, dev.Ip);
        if (target.Length == 0 || target.Equals("SERVER", StringComparison.OrdinalIgnoreCase))
        {
            relayed.Target = "SERVER";
            RaiseMessageReceived(relayed);
            await c.SendAsync(NetMessage.Response(m, true, "Delivered to the server.", ServerName)).ConfigureAwait(false);
            return;
        }
        ClientConnection? dest;
        lock (_gate) dest = _connections.FirstOrDefault(x => x.Device != null && x.Ready && x.Device.Name.Equals(target, StringComparison.OrdinalIgnoreCase));
        if (dest == null)
        {
            await c.SendAsync(NetMessage.Response(m, false, $"No connected device is named '{target}'.", ServerName)).ConfigureAwait(false);
            return;
        }
        relayed.Target = dest.Device!.Name;
        bool ok = await TrySendAsync(dest, relayed).ConfigureAwait(false);
        RaiseLog($"Relayed CHAT {dev.Name} -> {dest.Device.Name}" + (ok ? "" : " (delivery failed)"));
        await c.SendAsync(NetMessage.Response(m, ok, ok ? $"Delivered to {dest.Device.Name}." : $"{dest.Device.Name} could not be reached.", ServerName)).ConfigureAwait(false);
    }

    async Task HandleInventoryAsync(ClientConnection c, NetMessage m)
    {
        NetMessage Fail(string text) => NetMessage.Response(m, false, text, ServerName);
        var gateway = Inventory;
        if (gateway == null) { await c.SendAsync(Fail("The inventory service is not available on this server.")).ConfigureAwait(false); return; }

        string action = m.Get("action", "ADJUST").Trim().ToUpperInvariant();
        string sku = m.Get("sku").Trim();
        if (action != "GET" && action != "ADJUST") { await c.SendAsync(Fail("Unknown inventory action.")).ConfigureAwait(false); return; }
        if (sku.Length == 0 || sku.Length > 32) { await c.SendAsync(Fail("Enter a valid SKU (1-32 characters).")).ConfigureAwait(false); return; }
        int delta = 0;
        if (action == "ADJUST" && (!int.TryParse(m.Get("delta"), out delta) || delta == 0 || Math.Abs(delta) > 100000))
        { await c.SendAsync(Fail("The quantity change must be a whole number between -100000 and 100000, and not zero.")).ConfigureAwait(false); return; }
        string reason = m.Get("reason").Trim();
        if (reason.Length > 100) reason = reason.Substring(0, 100);

        InventoryResult result;
        try { result = await gateway.HandleAsync(new InventoryRequest(action, sku, delta, reason), c.Device!).ConfigureAwait(false); }
        catch (Exception ex)
        {
            RaiseLog("Inventory request failed: " + ex.Message);
            result = new InventoryResult(false, "The server could not process the request.");
        }
        await c.SendAsync(NetMessage.Response(m, result.Ok, result.Message, ServerName, new Dictionary<string, string>
        {
            ["sku"] = result.Sku, ["name"] = result.Name, ["quantity"] = result.Quantity.ToString(), ["reorder"] = result.ReorderLevel.ToString()
        })).ConfigureAwait(false);
    }

    NetMessage MakePong(NetMessage ping)
    {
        var pong = NetMessage.Create(MessageTypes.Pong, ServerName, "pong", data: new Dictionary<string, string>
        {
            ["serverName"] = ServerName, ["devices"] = GetDevices().Count.ToString()
        });
        pong.ReplyTo = ping.Id;
        return pong;
    }

    // ================================================================ sending from the server side ================================================================

    /// <summary>Sends a message to every registered device (except one). Returns how many devices received it. A slow or dead device never blocks the others.</summary>
    public async Task<int> BroadcastAsync(NetMessage m, string? exceptDeviceId = null)
    {
        StampAsServer(m);
        return await FanOutAsync(m, exceptDeviceId).ConfigureAwait(false);
    }

    /// <summary>Sends the message exactly as it is (sender fields untouched) to every ready device except one.</summary>
    async Task<int> FanOutAsync(NetMessage m, string? exceptDeviceId)
    {
        List<ClientConnection> targets;
        lock (_gate) targets = _connections.Where(c => c.Device != null && c.Ready && c.Device.Id != exceptDeviceId).ToList();
        var results = await Task.WhenAll(targets.Select(t => TrySendAsync(t, m))).ConfigureAwait(false);
        return results.Count(r => r);
    }

    /// <summary>Sends to one device by name. Returns false when no such device is connected or the delivery failed.</summary>
    public async Task<bool> SendToDeviceAsync(string deviceName, NetMessage m)
    {
        ClientConnection? dest;
        lock (_gate) dest = _connections.FirstOrDefault(c => c.Device != null && c.Ready && c.Device.Name.Equals(deviceName, StringComparison.OrdinalIgnoreCase));
        if (dest == null) return false;
        StampAsServer(m);
        m.Target = dest.Device!.Name;
        return await TrySendAsync(dest, m).ConfigureAwait(false);
    }

    void StampAsServer(NetMessage m)
    {
        m.Sender = ServerName;
        m.SenderIp = LocalEndPoint?.Address.Equals(IPAddress.Any) == false ? LocalEndPoint!.Address.ToString() : (NetUtil.GetLocalIPv4().FirstOrDefault()?.Ip ?? "");
    }

    async Task BroadcastDeviceListAsync()
    {
        try
        {
            List<DeviceInfo> list; long seq;
            lock (_gate)
            {
                seq = ++_listSeq;      // snapshot and sequence number are taken together; clients ignore lists older than the newest they have seen
                list = _connections.Where(c => c.Device != null).Select(c => c.Device!).OrderBy(d => d.ConnectedAtUtc).ToList();
            }
            var m = NetMessage.Create(MessageTypes.DeviceList, ServerName, DeviceListCodec.Encode(list),
                data: new Dictionary<string, string> { ["count"] = list.Count.ToString(), ["seq"] = seq.ToString() });
            await BroadcastAsync(m).ConfigureAwait(false);
        }
        catch (Exception ex) { RaiseLog("Could not publish the device list: " + ex.Message); }
    }

    async Task<bool> TrySendAsync(ClientConnection c, NetMessage m)
    {
        try
        {
            var send = c.SendAsync(m);
            var done = await Task.WhenAny(send, Task.Delay(SendTimeout)).ConfigureAwait(false);
            if (done != send)
            {
                c.Close("The device did not accept data in time (send timed out).");
                _ = send.ContinueWith(t => { _ = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);
                return false;
            }
            await send.ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            c.Close("Sending to the device failed.");
            return false;
        }
    }

    // ================================================================ watchdog ================================================================

    async Task WatchdogAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try { await Task.Delay(WatchdogInterval, ct).ConfigureAwait(false); }
            catch (OperationCanceledException) { break; }
            List<ClientConnection> snapshot;
            lock (_gate) snapshot = _connections.ToList();
            var now = DateTime.UtcNow;
            foreach (var c in snapshot)
            {
                if (c.Device == null && now - c.AcceptedUtc > HandshakeTimeout) c.Close("The connection did not register in time.");
                else if (c.Device != null && now - c.LastSeenUtc > IdleTimeout) c.Close("No heartbeat received - the device is no longer reachable.");
            }
        }
    }

    // ================================================================ helpers ================================================================

    static bool ConstantTimeEquals(string a, string b)
    {
        int diff = a.Length ^ b.Length;
        for (int i = 0; i < Math.Min(a.Length, b.Length); i++) diff |= a[i] ^ b[i];
        return diff == 0;
    }

    void RaiseLog(string text) { try { Log?.Invoke(text); } catch (Exception) { } }
    void RaiseDeviceConnected(DeviceInfo d) { try { DeviceConnected?.Invoke(d); } catch (Exception) { } }
    void RaiseDeviceDisconnected(DeviceInfo d, string reason) { try { DeviceDisconnected?.Invoke(d, reason); } catch (Exception) { } }
    void RaiseMessageReceived(NetMessage m) { try { MessageReceived?.Invoke(m); } catch (Exception) { } }
}
