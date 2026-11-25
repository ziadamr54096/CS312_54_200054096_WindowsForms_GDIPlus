namespace InventoryManager.Network;

/// <summary>A connected device as seen by the server (Ip/Port are the REAL remote endpoint of its TCP connection).</summary>
public sealed record DeviceInfo(string Id, string Name, string Ip, int Port, DateTime ConnectedAtUtc);

/// <summary>Text format used inside DEVICE_LIST messages: one device per line "id|name|ip|port|ticks" (names cannot contain '|').</summary>
public static class DeviceListCodec
{
    public static string Encode(IEnumerable<DeviceInfo> devices) =>
        string.Join("\n", devices.Select(d => $"{d.Id}|{d.Name}|{d.Ip}|{d.Port}|{d.ConnectedAtUtc.Ticks}"));

    public static List<DeviceInfo> Decode(string? text)
    {
        var list = new List<DeviceInfo>();
        if (string.IsNullOrWhiteSpace(text)) return list;
        foreach (var line in text.Split('\n'))
        {
            var p = line.Split('|');
            if (p.Length == 5 && int.TryParse(p[3], out int port) && long.TryParse(p[4], out long ticks)
                && ticks > 0 && ticks <= DateTime.MaxValue.Ticks)
                list.Add(new DeviceInfo(p[0], p[1], p[2], port, new DateTime(ticks, DateTimeKind.Utc)));
        }
        return list;
    }
}
