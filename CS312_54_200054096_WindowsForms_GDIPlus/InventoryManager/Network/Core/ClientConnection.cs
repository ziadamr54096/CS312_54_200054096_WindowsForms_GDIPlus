using System.Net;
using System.Net.Sockets;

namespace InventoryManager.Network;

/// <summary>Server-side state of one TCP connection (registered device or not-yet-registered).</summary>
internal sealed class ClientConnection
{
    readonly SemaphoreSlim _sendLock = new(1, 1);
    int _closed;
    internal long LastSeenTicks;

    public ClientConnection(TcpClient tcp, IPEndPoint remote)
    {
        Tcp = tcp; Stream = tcp.GetStream(); Remote = remote;
        Touch();
    }

    public TcpClient Tcp { get; }
    public NetworkStream Stream { get; }
    public IPEndPoint Remote { get; }
    public DateTime AcceptedUtc { get; } = DateTime.UtcNow;
    public DeviceInfo? Device { get; set; }          // null until CONNECT succeeded
    /// <summary>True once the welcome reply was sent. Only ready connections receive server pushes (DEVICE_LIST, broadcasts) - otherwise a push could overtake the welcome.</summary>
    public volatile bool Ready;
    public string? CloseReason { get; set; }

    public DateTime LastSeenUtc => new(Interlocked.Read(ref LastSeenTicks), DateTimeKind.Utc);
    public void Touch() => Interlocked.Exchange(ref LastSeenTicks, DateTime.UtcNow.Ticks);

    /// <summary>All sends on one connection are serialised so frames are never interleaved.</summary>
    public async Task SendAsync(NetMessage m)
    {
        await _sendLock.WaitAsync().ConfigureAwait(false);
        try { await FrameCodec.WriteAsync(Stream, m, CancellationToken.None).ConfigureAwait(false); }
        finally { _sendLock.Release(); }
    }

    /// <summary>Sends the welcome reply and marks the connection Ready in ONE step under the send lock, so no push can ever overtake the welcome.</summary>
    public async Task SendWelcomeAsync(NetMessage welcome)
    {
        await _sendLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await FrameCodec.WriteAsync(Stream, welcome, CancellationToken.None).ConfigureAwait(false);
            Ready = true;
        }
        finally { _sendLock.Release(); }
    }

    public void Close(string? reason = null)
    {
        if (reason != null && CloseReason == null) CloseReason = reason;
        if (Interlocked.Exchange(ref _closed, 1) == 0)
        {
            try { Tcp.Close(); } catch (Exception) { }
        }
    }
}
