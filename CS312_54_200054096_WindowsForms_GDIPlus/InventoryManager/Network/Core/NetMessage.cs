namespace InventoryManager.Network;

/// <summary>Message types of the Stockora LAN protocol (the value is the "Type" field of the JSON message).</summary>
public static class MessageTypes
{
    public const string Connect = "CONNECT";                // client -> server : register this device
    public const string Disconnect = "DISCONNECT";          // either way       : clean goodbye
    public const string Chat = "CHAT";                      // direct message (to the server or to one device)
    public const string Broadcast = "BROADCAST";            // message to every connected device
    public const string InventoryUpdate = "INVENTORY_UPDATE"; // client->server: stock request;  server->clients: stock changed
    public const string LowStockAlert = "LOW_STOCK_ALERT";  // server -> clients
    public const string ServerResponse = "SERVER_RESPONSE"; // server -> client: answer to a request (ReplyTo = request Id)
    public const string Ping = "PING";                      // heartbeat / "Test connection"
    public const string Pong = "PONG";
    public const string DeviceList = "DEVICE_LIST";         // server -> clients: who is connected

    public static readonly HashSet<string> All = new()
    {
        Connect, Disconnect, Chat, Broadcast, InventoryUpdate, LowStockAlert, ServerResponse, Ping, Pong, DeviceList
    };
}

/// <summary>
/// One protocol message. On the wire it is a UTF-8 JSON object preceded by a 4-byte big-endian length (see FrameCodec).
/// All fields are plain strings/ints so the format is easy to read, debug (e.g. in Wireshark) and extend.
/// </summary>
public sealed class NetMessage
{
    public const int ProtocolVersion = 1;
    public const int MaxTextLength = 4000;
    public const int MaxDataEntries = 24;
    public const int MaxDataValueLength = 1000;

    public int V { get; set; } = ProtocolVersion;
    public string Type { get; set; } = "";
    public string Id { get; set; } = "";
    public string? ReplyTo { get; set; }
    public string Sender { get; set; } = "";       // device name (the server overwrites it with the verified name)
    public string SenderIp { get; set; } = "";     // IP address (the server overwrites it with the real remote address)
    public string? Target { get; set; }            // CHAT only: "SERVER" / empty = the server, otherwise a device name
    public string Text { get; set; } = "";
    public string Timestamp { get; set; } = "";    // ISO-8601, UTC
    public Dictionary<string, string>? Data { get; set; }

    public static NetMessage Create(string type, string sender, string text = "", string? target = null,
                                    Dictionary<string, string>? data = null) => new()
    {
        Type = type,
        Id = Guid.NewGuid().ToString("N"),
        Sender = sender,
        Text = text,
        Target = target,
        Data = data,
        Timestamp = DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture)
    };

    public static NetMessage Response(NetMessage? request, bool ok, string text, string sender = "",
                                      Dictionary<string, string>? extra = null)
    {
        var data = new Dictionary<string, string> { ["ok"] = ok ? "true" : "false" };
        if (extra != null) foreach (var kv in extra) data[kv.Key] = kv.Value;
        var m = Create(MessageTypes.ServerResponse, sender, text, null, data);
        m.ReplyTo = request?.Id;
        return m;
    }

    public string Get(string key, string fallback = "") =>
        Data != null && Data.TryGetValue(key, out var v) ? v : fallback;

    /// <summary>For SERVER_RESPONSE: did the server accept the request? (a method, so it is not serialised onto the wire)</summary>
    public bool Succeeded() => Get("ok") == "true";

    public DateTime LocalTime() =>
        DateTime.TryParse(Timestamp, System.Globalization.CultureInfo.InvariantCulture,
                          System.Globalization.DateTimeStyles.RoundtripKind, out var t)
            ? t.ToLocalTime() : DateTime.Now;

    /// <summary>Copy used when the server relays a message: sender fields are set by the SERVER, never trusted from the client.</summary>
    public NetMessage Relay(string verifiedSender, string verifiedIp) => new()
    {
        V = V, Type = Type, Id = Id, ReplyTo = ReplyTo, Sender = verifiedSender, SenderIp = verifiedIp,
        Target = Target, Text = Text, Timestamp = Timestamp,
        Data = Data == null ? null : new Dictionary<string, string>(Data)
    };
}
