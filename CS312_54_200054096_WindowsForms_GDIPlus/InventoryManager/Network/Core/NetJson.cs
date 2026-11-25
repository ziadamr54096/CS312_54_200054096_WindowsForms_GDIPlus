using System.Text.Json;

namespace InventoryManager.Network;

/// <summary>JSON (de)serialisation + validation. All System.Text.Json usage is kept in this one file.</summary>
public static class NetJson
{
    public static byte[] Serialize(NetMessage m) => JsonSerializer.SerializeToUtf8Bytes(m);

    /// <summary>Parses and validates one message; throws InvalidDataException for anything malformed or out of limits.</summary>
    public static NetMessage Deserialize(byte[] utf8)
    {
        NetMessage? m;
        try { m = JsonSerializer.Deserialize<NetMessage>(utf8); }
        catch (JsonException ex) { throw new InvalidDataException("Malformed JSON: " + ex.Message, ex); }
        if (m == null) throw new InvalidDataException("Empty message.");
        Validate(m);
        return m;
    }

    static void Validate(NetMessage m)
    {
        if (m.V != NetMessage.ProtocolVersion) throw new InvalidDataException($"Unsupported protocol version {m.V}.");
        if (m.Type == null || !MessageTypes.All.Contains(m.Type)) throw new InvalidDataException($"Unknown message type '{m.Type}'.");
        m.Id ??= ""; m.Sender ??= ""; m.SenderIp ??= ""; m.Text ??= ""; m.Timestamp ??= "";
        if (m.Id.Length > 64 || m.Sender.Length > 64 || m.SenderIp.Length > 64 || (m.Target?.Length ?? 0) > 64)
            throw new InvalidDataException("A header field is too long.");
        if (m.Text.Length > NetMessage.MaxTextLength) throw new InvalidDataException("Message text is too long.");
        if (m.Data != null)
        {
            if (m.Data.Count > NetMessage.MaxDataEntries) throw new InvalidDataException("Too many data entries.");
            foreach (var kv in m.Data)
                if (kv.Key == null || kv.Key.Length > 40 || (kv.Value?.Length ?? 0) > NetMessage.MaxDataValueLength)
                    throw new InvalidDataException("A data entry is too long.");
        }
    }
}
