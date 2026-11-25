using System.Net;

namespace InventoryManager.Network;

public enum NetworkRole { None, Server, Client }

/// <summary>One line of the communication log. Direction: "IN", "OUT" or "SYS".</summary>
public sealed record LogEntry(DateTime Time, string Direction, string Type, string Peer, string Text);

/// <summary>Kind: "message", "inventory", "lowstock", "outofstock" or "info".</summary>
public sealed record NotificationInfo(string Title, string Body, string Kind);

/// <summary>One row of the "Connected devices" list.</summary>
public sealed record DeviceRow(string Id, string Name, string Ip, int Port, string Status, DateTime ConnectedAtLocal, bool IsServer, bool IsSelf);

public sealed record Recipient(string Display, string Value)
{
    public override string ToString() => Display;
}

/// <summary>
/// Orchestrates NetworkServer / NetworkClient for the Network tab. No controls here: every event from a background thread is
/// posted to the UI SynchronizationContext, so subscribers (the panel) can touch WinForms controls directly.
/// One computer is either the Server or a Client at a time.
/// </summary>
public sealed class NetworkController : IDisposable
{
    readonly SynchronizationContext _ui;
    readonly InventoryWatcher _watcher = new();
    volatile bool _disposed;

    public NetworkController(SynchronizationContext ui)
    {
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        Server = new NetworkServer { ServerName = NetUtil.DefaultDeviceName() };
        Client = new NetworkClient();
        Server.Inventory = new DbInventoryGateway(_ui, _watcher);

        Server.DeviceConnected += d => Post(() =>
        {
            AddLog("SYS", "CONNECT", $"{d.Name} ({d.Ip})", $"Device connected from {d.Ip}:{d.Port}.");
            Notify(new NotificationInfo("Device connected", $"{d.Name} ({d.Ip}) joined the network.", "info"));
            DevicesChanged?.Invoke();
        });
        Server.DeviceDisconnected += (d, reason) => Post(() =>
        {
            AddLog("SYS", "DISCONNECT", $"{d.Name} ({d.Ip})", $"Device left: {reason}");
            Notify(new NotificationInfo("Device disconnected", $"{d.Name} ({d.Ip}): {reason}", "info"));
            DevicesChanged?.Invoke();
        });
        Server.MessageReceived += m => Post(() => OnServerReceived(m));
        Server.Log += t => Post(() => AddLog("SYS", "SERVER", "", t));

        Client.MessageReceived += m => Post(() => OnClientReceived(m));
        Client.DevicesChanged += _ => Post(() => DevicesChanged?.Invoke());
        Client.Disconnected += (reason, requested) => Post(() =>
        {
            if (Role == NetworkRole.Client) Role = NetworkRole.None;
            AddLog("SYS", "DISCONNECT", "", requested ? "Disconnected from the server." : "Disconnected: " + reason);
            if (!requested) Notify(new NotificationInfo("Disconnected from the server", reason, "info"));
            RoleChanged?.Invoke();
            DevicesChanged?.Invoke();
        });
        Client.Log += t => Post(() => AddLog("SYS", "CLIENT", "", t));

        _watcher.Changes += OnInventoryChanged;
    }

    public NetworkServer Server { get; }
    public NetworkClient Client { get; }
    public NetworkRole Role { get; private set; }
    public bool BroadcastInventory { get; set; } = true;
    /// <summary>Signed-in application user (informational: sent to the server in CONNECT and shown in its log).</summary>
    public string? CurrentUser { get; set; }

    // ---- events (always raised on the UI thread) ----
    public event Action<LogEntry>? LogAdded;
    public event Action? RoleChanged;
    public event Action? DevicesChanged;
    public event Action<NotificationInfo>? NotificationRaised;

    void Post(Action a) => _ui.Post(_ => { if (_disposed) return; try { a(); } catch (Exception) { } }, null);
    void AddLog(string dir, string type, string peer, string text) => LogAdded?.Invoke(new LogEntry(DateTime.Now, dir, type, peer, text));
    void Notify(NotificationInfo n) => NotificationRaised?.Invoke(n);

    // ================================================================ server ================================================================

    /// <summary>Starts the server. Returns null on success, otherwise a message for the user.</summary>
    public string? StartServer(string bindText, string portText, string? accessCode, bool allowNonPrivate)
    {
        if (Role == NetworkRole.Client) return "This computer is connected as a client. Disconnect first, then start the server.";
        if (Role == NetworkRole.Server) return "The server is already running.";
        if (!NetUtil.TryParseBindAddress(bindText, out var bind, out var err)) return err;
        if (!NetUtil.TryParsePort(portText, out int port, out err)) return err;
        try
        {
            Server.AccessCode = string.IsNullOrEmpty(accessCode) ? null : accessCode;
            Server.AddressFilter = allowNonPrivate ? (_ => true) : NetUtil.IsPrivateLan;
            _watcher.Start();                                  // may throw if the database is unavailable
            Server.Start(bind, port);
        }
        catch (NetworkException ex) { _watcher.Stop(); return ex.Message; }
        catch (Exception ex) { _watcher.Stop(); return "The server could not start: " + ex.Message; }

        Role = NetworkRole.Server;
        AddLog("SYS", "SERVER", "", $"Server started on {Server.LocalEndPoint} (name '{Server.ServerName}')" + (Server.AccessCode != null ? ", access code required." : "."));
        RoleChanged?.Invoke();
        DevicesChanged?.Invoke();
        return null;
    }

    public async Task StopServerAsync()
    {
        if (Role != NetworkRole.Server) return;
        _watcher.Stop();
        await Server.StopAsync();
        Role = NetworkRole.None;
        AddLog("SYS", "SERVER", "", "Server stopped.");
        RoleChanged?.Invoke();
        DevicesChanged?.Invoke();
    }

    void OnServerReceived(NetMessage m)
    {
        bool broadcast = m.Type == MessageTypes.Broadcast;
        AddLog("IN", m.Type, $"{m.Sender} ({m.SenderIp})", m.Text);
        Notify(new NotificationInfo(broadcast ? $"Broadcast from {m.Sender}" : $"Message from {m.Sender}", m.Text, "message"));
    }

    void OnInventoryChanged(IReadOnlyList<InventoryChange> updates, IReadOnlyList<LowStockInfo> lows)
    {
        if (Role != NetworkRole.Server || !BroadcastInventory || !Server.IsRunning) return;
        _ = BroadcastInventoryAsync(updates, lows);
    }

    async Task BroadcastInventoryAsync(IReadOnlyList<InventoryChange> updates, IReadOnlyList<LowStockInfo> lows)
    {
        try
        {
            foreach (var u in updates)
            {
                int n = await Server.BroadcastAsync(InventoryMessages.Update(u));
                Post(() => AddLog("OUT", MessageTypes.InventoryUpdate, $"{n} device(s)", $"{u.Name} ({u.Sku}): {u.OldQuantity} -> {u.NewQuantity}" + (u.By.Length > 0 ? $" by {u.By}" : "")));
            }
            foreach (var l in lows)
            {
                int n = await Server.BroadcastAsync(InventoryMessages.LowStock(l));
                Post(() => AddLog("OUT", MessageTypes.LowStockAlert, $"{n} device(s)", InventoryMessages.LowStock(l).Text));
            }
        }
        catch (Exception ex) { Post(() => AddLog("SYS", "ERROR", "", "Could not broadcast the stock change: " + ex.Message)); }
    }

    // ================================================================ client ================================================================

    /// <summary>Connects to a server. Returns null on success, otherwise a message for the user.</summary>
    public async Task<string?> ConnectAsync(string hostText, string portText, string deviceName, string? accessCode)
    {
        if (Role == NetworkRole.Server) return "This computer is running the server. Stop the server first, then connect as a client.";
        if (Role == NetworkRole.Client) return "Already connected. Disconnect first.";
        if (!NetUtil.TryParseServerAddress(hostText, out var ip, out var err)) return err;
        if (!NetUtil.TryParsePort(portText, out int port, out err)) return err;
        err = NetUtil.ValidateDeviceName(deviceName);
        if (err != null) return err;
        try
        {
            await Client.ConnectAsync(ip, port, deviceName.Trim(), string.IsNullOrEmpty(accessCode) ? null : accessCode, CurrentUser);
        }
        catch (NetworkException ex) { AddLog("SYS", "ERROR", $"{ip}:{port}", ex.Message); return ex.Message; }
        catch (Exception ex) { return "Could not connect: " + ex.Message; }

        Role = NetworkRole.Client;
        AddLog("SYS", "CONNECT", $"{Client.ServerName} ({ip}:{port})", $"Connected to {Client.ServerName} as '{Client.DeviceName}'.");
        RoleChanged?.Invoke();
        DevicesChanged?.Invoke();
        return null;
    }

    public async Task DisconnectAsync()
    {
        if (Role != NetworkRole.Client) return;
        await Client.DisconnectAsync();     // the Disconnected event updates Role + UI
    }

    public async Task<ProbeResult> TestAsync(string hostText, string portText)
    {
        if (!NetUtil.TryParseServerAddress(hostText, out var ip, out var err)) return new ProbeResult(false, err, 0, 0, "", 0);
        if (!NetUtil.TryParsePort(portText, out int port, out err)) return new ProbeResult(false, err, 0, 0, "", 0);
        var r = await NetworkClient.ProbeAsync(ip, port, TimeSpan.FromSeconds(4));
        AddLog("SYS", "TEST", $"{ip}:{port}", r.Ok ? "Test OK - " + r.Message : "Test failed - " + r.Message);
        return r;
    }

    void OnClientReceived(NetMessage m)
    {
        string from = string.IsNullOrEmpty(m.SenderIp) ? m.Sender : $"{m.Sender} ({m.SenderIp})";
        AddLog("IN", m.Type, from, m.Text);
        switch (m.Type)
        {
            case MessageTypes.Chat:
                Notify(new NotificationInfo($"Message from {m.Sender}", m.Text, "message")); break;
            case MessageTypes.Broadcast:
                Notify(new NotificationInfo($"Broadcast from {m.Sender}", m.Text, "message")); break;
            case MessageTypes.InventoryUpdate:
                Notify(new NotificationInfo("Stock update", m.Text, "inventory")); break;
            case MessageTypes.LowStockAlert:
                Notify(new NotificationInfo(m.Get("status") == "OUT" ? "Out of stock" : "Low stock alert", m.Text, m.Get("status") == "OUT" ? "outofstock" : "lowstock")); break;
        }
    }

    // ================================================================ messaging ================================================================

    public const string RecipientAll = "ALL";
    public const string RecipientServer = "SERVER";

    public IReadOnlyList<Recipient> GetRecipients()
    {
        var list = new List<Recipient> { new("All devices (broadcast)", RecipientAll) };
        if (Role == NetworkRole.Client) list.Add(new Recipient($"Server only ({Client.ServerName})", RecipientServer));
        foreach (var d in CurrentDevices().Where(d => !d.IsServer && !d.IsSelf)) list.Add(new Recipient(d.Name, d.Name));
        return list;
    }

    /// <summary>Sends a message. Returns null on success, otherwise a message for the user.</summary>
    public async Task<string?> SendMessageAsync(string recipient, string text)
    {
        text = (text ?? "").Trim();
        if (text.Length == 0) return "Type a message first.";
        if (text.Length > 1000) return "Messages are limited to 1000 characters.";
        try
        {
            if (Role == NetworkRole.Server)
            {
                if (recipient == RecipientAll)
                {
                    int n = await Server.BroadcastAsync(NetMessage.Create(MessageTypes.Broadcast, Server.ServerName, text));
                    AddLog("OUT", MessageTypes.Broadcast, $"{n} device(s)", text);
                    return n == 0 ? "No devices are connected - nobody received the message." : null;
                }
                bool ok = await Server.SendToDeviceAsync(recipient, NetMessage.Create(MessageTypes.Chat, Server.ServerName, text));
                AddLog("OUT", MessageTypes.Chat, recipient, text);
                return ok ? null : $"'{recipient}' is no longer connected.";
            }
            if (Role == NetworkRole.Client)
            {
                NetMessage req = recipient == RecipientAll
                    ? NetMessage.Create(MessageTypes.Broadcast, Client.DeviceName, text)
                    : NetMessage.Create(MessageTypes.Chat, Client.DeviceName, text, recipient);
                var resp = await Client.RequestAsync(req);
                AddLog("OUT", req.Type, recipient == RecipientAll ? "all devices" : recipient == RecipientServer ? Client.ServerName : recipient, text);
                if (!resp.Succeeded()) return resp.Text;
                AddLog("SYS", MessageTypes.ServerResponse, Client.ServerName, resp.Text);
                return null;
            }
            return "Start the server or connect to a server first.";
        }
        catch (NetworkException ex) { return ex.Message; }
    }

    // ================================================================ remote inventory (client -> server) ================================================================

    /// <summary>Asks the SERVER to read ("GET") or change ("ADJUST") the stock of a product. Returns (success, message).</summary>
    public async Task<(bool Ok, string Message)> RemoteStockAsync(string action, string sku, string deltaText, string reason)
    {
        if (Role != NetworkRole.Client) return (false, "Connect to a server first.");
        sku = (sku ?? "").Trim();
        if (sku.Length == 0) return (false, "Enter the SKU of the product (for example EL-001).");
        int delta = 0;
        if (action == "ADJUST" && (!int.TryParse((deltaText ?? "").Trim(), out delta) || delta == 0 || Math.Abs(delta) > 100000))
            return (false, "Enter the quantity change as a whole number, not zero (use a minus sign to remove stock, e.g. -3).");
        var req = NetMessage.Create(MessageTypes.InventoryUpdate, Client.DeviceName, data: new Dictionary<string, string>
        {
            ["action"] = action, ["sku"] = sku, ["delta"] = delta.ToString(), ["reason"] = reason ?? ""
        });
        try
        {
            var resp = await Client.RequestAsync(req);
            AddLog("OUT", MessageTypes.InventoryUpdate, Client.ServerName, action == "GET" ? $"Stock query for {sku}" : $"Adjust {sku} by {delta}" + (string.IsNullOrWhiteSpace(reason) ? "" : $" ({reason})"));
            AddLog("SYS", MessageTypes.ServerResponse, Client.ServerName, resp.Text);
            return (resp.Succeeded(), resp.Text);
        }
        catch (NetworkException ex) { return (false, ex.Message); }
    }

    // ================================================================ devices ================================================================

    public IReadOnlyList<DeviceRow> CurrentDevices()
    {
        var rows = new List<DeviceRow>();
        if (Role == NetworkRole.Server)
            foreach (var d in Server.GetDevices())
                rows.Add(new DeviceRow(d.Id, d.Name, d.Ip, d.Port, "Connected", d.ConnectedAtUtc.ToLocalTime(), false, false));
        else if (Role == NetworkRole.Client && Client.IsConnected && Client.ServerEndPoint != null)
        {
            rows.Add(new DeviceRow("server", Client.ServerName + " (server)", Client.ServerEndPoint.Address.ToString(), Client.ServerEndPoint.Port, "Connected",
                                   (Client.ConnectedAtUtc ?? DateTime.UtcNow).ToLocalTime(), true, false));
            foreach (var d in Client.Devices)
                rows.Add(new DeviceRow(d.Id, d.Name, d.Ip, d.Port, "Connected", d.ConnectedAtUtc.ToLocalTime(), false, false));
        }
        return rows;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _watcher.Dispose();
        try { Client.Dispose(); } catch (Exception) { }
        try { Server.Dispose(); } catch (Exception) { }
    }
}
