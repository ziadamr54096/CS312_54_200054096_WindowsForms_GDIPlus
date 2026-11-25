namespace InventoryManager.Network;

/// <summary>
/// TCP is a byte STREAM: one Read may return half a message, or three messages at once. So every message is sent as a frame:
///     [4 bytes: length of the JSON, big-endian][JSON bytes (UTF-8)]
/// and the reader loops until it has received exactly the announced number of bytes. Frames larger than MaxFrameBytes are rejected.
/// </summary>
public static class FrameCodec
{
    public const int MaxFrameBytes = 64 * 1024;

    public static byte[] Encode(NetMessage m)
    {
        byte[] json = NetJson.Serialize(m);
        if (json.Length == 0 || json.Length > MaxFrameBytes) throw new InvalidDataException("Message is too large to send.");
        var frame = new byte[4 + json.Length];
        frame[0] = (byte)(json.Length >> 24); frame[1] = (byte)(json.Length >> 16);
        frame[2] = (byte)(json.Length >> 8);  frame[3] = (byte)json.Length;
        Buffer.BlockCopy(json, 0, frame, 4, json.Length);
        return frame;
    }

    public static async Task WriteAsync(Stream s, NetMessage m, CancellationToken ct)
    {
        byte[] frame = Encode(m);
        await s.WriteAsync(frame, 0, frame.Length, ct).ConfigureAwait(false);   // one write = header+body, never interleaved (callers serialise sends)
        await s.FlushAsync(ct).ConfigureAwait(false);
    }

    /// <summary>Returns the next message, or null when the peer closed the connection cleanly between two frames.</summary>
    public static async Task<NetMessage?> ReadAsync(Stream s, CancellationToken ct)
    {
        var header = new byte[4];
        if (!await ReadExactAsync(s, header, ct, allowCleanEof: true).ConfigureAwait(false)) return null;
        int length = (header[0] << 24) | (header[1] << 16) | (header[2] << 8) | header[3];
        if (length <= 0 || length > MaxFrameBytes) throw new InvalidDataException($"Invalid frame length {length}.");
        var body = new byte[length];
        await ReadExactAsync(s, body, ct, allowCleanEof: false).ConfigureAwait(false);
        return NetJson.Deserialize(body);
    }

    static async Task<bool> ReadExactAsync(Stream s, byte[] buffer, CancellationToken ct, bool allowCleanEof)
    {
        int read = 0;
        while (read < buffer.Length)
        {
            int n = await s.ReadAsync(buffer, read, buffer.Length - read, ct).ConfigureAwait(false);
            if (n == 0)
            {
                if (read == 0 && allowCleanEof) return false;
                throw new EndOfStreamException("The connection closed in the middle of a message.");
            }
            read += n;
        }
        return true;
    }
}
