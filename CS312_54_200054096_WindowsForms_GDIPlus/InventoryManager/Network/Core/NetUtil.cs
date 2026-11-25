using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace InventoryManager.Network;

public sealed record LocalAddress(string Ip, string Adapter);

/// <summary>Validation of user input, LAN detection and friendly error texts. No UI code.</summary>
public static class NetUtil
{
    public const int DefaultPort = 5000;
    public const string AllInterfaces = "0.0.0.0";

    static readonly Regex Ipv4Pattern = new(
        @"^(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}$", RegexOptions.Compiled);
    static readonly Regex DeviceNamePattern = new(@"^[A-Za-z0-9][A-Za-z0-9 ._-]{0,31}$", RegexOptions.Compiled);

    /// <summary>Accepts a dotted IPv4 address (e.g. 192.168.1.10) or "localhost". (IPAddress.TryParse alone would accept things like "1" or "10.1".)</summary>
    public static bool TryParseServerAddress(string? text, out IPAddress address, out string error)
    {
        address = IPAddress.None; error = "";
        string t = (text ?? "").Trim();
        if (t.Length == 0) { error = "Enter the server's IP address (for example 192.168.1.10)."; return false; }
        if (t.Equals("localhost", StringComparison.OrdinalIgnoreCase)) { address = IPAddress.Loopback; return true; }
        if (!Ipv4Pattern.IsMatch(t) || !IPAddress.TryParse(t, out var ip))
        { error = $"'{t}' is not a valid IPv4 address. Use four numbers from 0 to 255, for example 192.168.1.10."; return false; }
        if (ip.Equals(IPAddress.Any)) { error = "0.0.0.0 is not a server address. Enter the IP address of the computer that runs the server."; return false; }
        if (ip.Equals(IPAddress.Broadcast)) { error = "255.255.255.255 is a broadcast address, not a computer."; return false; }
        byte first = ip.GetAddressBytes()[0];
        if (first >= 224) { error = "That address is reserved (multicast/broadcast) and cannot be a server."; return false; }
        address = ip;
        return true;
    }

    /// <summary>Address the SERVER binds to: "0.0.0.0" (all adapters) or any IPv4 address of this computer.</summary>
    public static bool TryParseBindAddress(string? text, out IPAddress address, out string error)
    {
        address = IPAddress.Any; error = "";
        string t = (text ?? "").Trim();
        if (t.Length == 0 || t == AllInterfaces) return true;
        if (!Ipv4Pattern.IsMatch(t) || !IPAddress.TryParse(t, out var ip)) { error = $"'{t}' is not a valid IPv4 address."; return false; }
        address = ip;
        return true;
    }

    public static bool TryParsePort(string? text, out int port, out string error)
    {
        port = 0; error = "";
        string t = (text ?? "").Trim();
        if (t.Length == 0) { error = "Enter a port number between 1 and 65535 (default 5000)."; return false; }
        if (!int.TryParse(t, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out port))
        { error = $"'{t}' is not a valid port. Use a whole number between 1 and 65535."; return false; }
        if (port < 1 || port > 65535) { error = $"Port {port} is out of range. Use a number between 1 and 65535."; return false; }
        return true;
    }

    /// <summary>Returns an error message, or null when the device name is acceptable.</summary>
    public static string? ValidateDeviceName(string? name)
    {
        string n = (name ?? "").Trim();
        if (n.Length == 0) return "Enter a device name.";
        if (!DeviceNamePattern.IsMatch(n)) return "Device name: 1-32 characters - letters, digits, space, '.', '_' or '-' (must start with a letter or digit).";
        if (n.Equals("SERVER", StringComparison.OrdinalIgnoreCase) || n.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            return $"'{n}' is a reserved name. Choose another device name.";
        return null;
    }

    /// <summary>The computer name made safe for use as a device name.</summary>
    public static string DefaultDeviceName()
    {
        string raw = Environment.MachineName ?? "device";
        var chars = raw.Select(c => char.IsLetterOrDigit(c) || c is '.' or '_' or '-' ? c : '-').ToArray();
        string s = new string(chars).Trim('-', '.', '_');
        if (s.Length > 32) s = s.Substring(0, 32);
        return ValidateDeviceName(s) == null ? s : "device";
    }

    /// <summary>True for loopback, 10.0.0.0/8, 172.16.0.0/12, 192.168.0.0/16 and 169.254.0.0/16 (typical LAN / Wi-Fi / hotspot ranges).</summary>
    public static bool IsPrivateLan(IPAddress ip)
    {
        if (ip.IsIPv4MappedToIPv6) ip = ip.MapToIPv4();
        if (IPAddress.IsLoopback(ip)) return true;
        if (ip.AddressFamily != AddressFamily.InterNetwork) return false;
        var b = ip.GetAddressBytes();
        return b[0] == 10 || (b[0] == 172 && b[1] >= 16 && b[1] <= 31) || (b[0] == 192 && b[1] == 168) || (b[0] == 169 && b[1] == 254);
    }

    /// <summary>IPv4 addresses of this computer's active network adapters (loopback and 169.254.x.x excluded).</summary>
    public static IReadOnlyList<LocalAddress> GetLocalIPv4()
    {
        var result = new List<LocalAddress>();
        try
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus != OperationalStatus.Up) continue;
                if (nic.NetworkInterfaceType is NetworkInterfaceType.Loopback or NetworkInterfaceType.Tunnel) continue;
                IPInterfaceProperties props;
                try { props = nic.GetIPProperties(); } catch (Exception) { continue; }
                foreach (var ua in props.UnicastAddresses)
                {
                    if (ua.Address.AddressFamily != AddressFamily.InterNetwork) continue;
                    var b = ua.Address.GetAddressBytes();
                    if (b[0] == 169 && b[1] == 254) continue;
                    result.Add(new LocalAddress(ua.Address.ToString(), nic.Name));
                }
            }
        }
        catch (Exception) { /* some systems restrict adapter enumeration; the list is simply empty */ }
        return result;
    }

    // ---- friendly error texts (the UI shows them unchanged) ----

    public static string FriendlyListenError(int port, SocketException ex) => ex.SocketErrorCode switch
    {
        SocketError.AddressAlreadyInUse => $"Port {port} is already in use by another program (or another copy of this application). Choose a different port, or close the program that uses it.",
        SocketError.AccessDenied => $"Windows does not allow listening on port {port}. Choose a port between 1024 and 65535.",
        SocketError.AddressNotAvailable => "The selected IP address does not belong to this computer. Choose one from the list, or 'All network adapters'.",
        _ => $"The server could not start on port {port}: {ex.Message}"
    };

    public static string FriendlyConnectError(SocketException ex, IPAddress ip, int port) => ex.SocketErrorCode switch
    {
        SocketError.ConnectionRefused => $"No server is listening at {ip}:{port}. Check the IP address and port, make sure the server is started, and that the server's firewall allows this port.",
        SocketError.HostUnreachable or SocketError.NetworkUnreachable or SocketError.HostDown => $"The computer {ip} cannot be reached. Check that both computers are on the same Wi-Fi/LAN and the IP address is correct.",
        SocketError.TimedOut => TimeoutMessage(ip, port),
        SocketError.ConnectionReset or SocketError.ConnectionAborted => $"The server at {ip}:{port} closed the connection.",
        _ => $"Could not connect to {ip}:{port}: {ex.Message}"
    };

    public static string TimeoutMessage(IPAddress ip, int port) =>
        $"{ip}:{port} did not answer in time. Check that the IP address is correct, both computers are on the same network, and the server's firewall allows the port.";
}
