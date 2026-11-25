using InventoryManager.Auth;
using InventoryManager.Network;

namespace InventoryManager;

/// <summary>
/// The "Network" tab: run this computer as a Server, or connect to a server as a Client, see connected devices, exchange messages,
/// receive notifications and send stock requests. Layout is in NetworkPanel.Designer.cs (editable in Visual Studio);
/// the networking itself lives in InventoryManager.Network (NetworkController). Nothing here runs in the Designer.
/// </summary>
public partial class NetworkPanel : UserControl
{
    sealed record BindItem(string Address, string Display)
    {
        public override string ToString() => Display;
    }

    const int MaxLogRows = 500;
    NetworkController? _net;
    IReadOnlyList<LocalAddress> _locals = Array.Empty<LocalAddress>();

    public NetworkPanel()
    {
        InitializeComponent();   // visual layout only - see NetworkPanel.Designer.cs
    }

    // ---------- runtime initialisation (skipped inside the Visual Studio Designer) ----------

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Ui.InDesigner(this) || _net != null) return;

        _net = new NetworkController(SynchronizationContext.Current ?? new SynchronizationContext())
        {
            CurrentUser = Session.Current?.Username,
            BroadcastInventory = chkBroadcastInventory.Checked
        };
        _net.LogAdded += AddLog;
        _net.RoleChanged += UpdateUi;
        _net.DevicesChanged += RefreshDevices;
        _net.NotificationRaised += ShowNotification;

        txtDeviceName.Text = NetUtil.DefaultDeviceName();
        FillLocalAddresses();
        Disposed -= OnPanelDisposed;
        Disposed += OnPanelDisposed;
        UpdateUi();
        RefreshDevices();
        tmrClock.Start();
    }

    /// <summary>Logging out / closing the window disposes this panel: the server stops and every connection is closed cleanly.</summary>
    void OnPanelDisposed(object? sender, EventArgs e)
    {
        tmrClock.Stop();
        var net = _net;
        _net = null;
        net?.Dispose();
    }

    // ---------- helpers ----------

    void FillLocalAddresses()
    {
        _locals = NetUtil.GetLocalIPv4();
        cboBind.Items.Clear();
        cboBind.Items.Add(new BindItem(NetUtil.AllInterfaces, "All network adapters (0.0.0.0)"));
        foreach (var l in _locals) cboBind.Items.Add(new BindItem(l.Ip, $"{l.Ip}  ({l.Adapter})"));
        cboBind.SelectedIndex = 0;
    }

    static string Duration(TimeSpan ts) => $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";

    string LocalIpText(int? port) => _locals.Count == 0
        ? "No active network adapter found - connect to Wi-Fi or a LAN."
        : string.Join(",  ", _locals.Select(l => port == null ? l.Ip : $"{l.Ip}:{port}"));

    void UpdateUi()
    {
        if (_net == null) return;
        var role = _net.Role;
        bool idle = role == NetworkRole.None, server = role == NetworkRole.Server, client = role == NetworkRole.Client;

        // A. server
        cboBind.Enabled = txtServerPort.Enabled = txtServerCode.Enabled = chkAllowPublic.Enabled = idle;
        btnStartServer.Enabled = idle;
        btnStopServer.Enabled = server;
        lblServerStatus.ForeColor = server ? Color.SeaGreen : Color.Firebrick;
        lblServerEndpoint.Text = server && _net.Server.LocalEndPoint != null ? _net.Server.LocalEndPoint.ToString() : "-";
        lblLocalIps.Text = server && _net.Server.LocalEndPoint != null
            ? "Clients connect to: " + LocalIpText(_net.Server.LocalEndPoint.Port)
            : LocalIpText(null);

        // B. client
        txtHost.Enabled = txtClientPort.Enabled = txtDeviceName.Enabled = txtClientCode.Enabled = idle;
        btnConnect.Enabled = btnTest.Enabled = idle;
        btnDisconnect.Enabled = client;
        lblClientStatus.ForeColor = client ? Color.SeaGreen : Color.Firebrick;
        lblClientStatus.Text = client ? $"Connected to {_net.Client.ServerName}" : "Disconnected";

        // D / E
        bool active = !idle;
        cboRecipient.Enabled = txtMessage.Enabled = btnSend.Enabled = active;
        txtSku.Enabled = numDelta.Enabled = txtReason.Enabled = btnCheckStock.Enabled = btnApplyStock.Enabled = client;
        RefreshRecipients();
        UpdateClock();
    }

    void UpdateClock()
    {
        if (_net == null) return;
        if (_net.Role == NetworkRole.Server && _net.Server.StartedAtUtc != null)
            lblServerStatus.Text = "Running  -  up " + Duration(DateTime.UtcNow - _net.Server.StartedAtUtc.Value);
        else lblServerStatus.Text = _net.Role == NetworkRole.Server ? "Running" : "Stopped";

        var since = _net.Role == NetworkRole.Client ? _net.Client.ConnectedAtUtc : null;
        lblDuration.Text = since == null ? "-" : Duration(DateTime.UtcNow - since.Value);

        foreach (DataGridViewRow row in dgvDevices.Rows)
            if (row.Tag is DateTime started)
                row.Cells["colDevOnline"].Value = Duration(DateTime.Now - started);
    }

    void RefreshDevices()
    {
        if (_net == null) return;
        var rows = _net.CurrentDevices();
        dgvDevices.Rows.Clear();
        foreach (var r in rows)
        {
            int i = dgvDevices.Rows.Add(r.Name, r.Ip, r.Port, r.Status, r.ConnectedAtLocal.ToString("yyyy-MM-dd HH:mm:ss"), Duration(DateTime.Now - r.ConnectedAtLocal));
            dgvDevices.Rows[i].Tag = r.ConnectedAtLocal;
        }
        dgvDevices.ClearSelection();
        lblDeviceCount.Text = _net.Role switch
        {
            NetworkRole.Server => rows.Count == 0 ? "No devices connected yet." : $"{rows.Count} device(s) connected to this server.",
            NetworkRole.Client => rows.Count <= 1 ? $"Connected to {_net.Client.ServerName}. No other devices yet." : $"Connected to {_net.Client.ServerName}; {rows.Count - 1} other device(s) on the network.",
            _ => "No devices connected."
        };
        RefreshRecipients();
    }

    void RefreshRecipients()
    {
        if (_net == null) return;
        string? keep = (cboRecipient.SelectedItem as Recipient)?.Value;
        cboRecipient.Items.Clear();
        foreach (var r in _net.GetRecipients()) cboRecipient.Items.Add(r);
        int index = 0;
        for (int i = 0; i < cboRecipient.Items.Count; i++)
            if (cboRecipient.Items[i] is Recipient r && r.Value == keep) index = i;
        if (cboRecipient.Items.Count > 0) cboRecipient.SelectedIndex = index;
    }

    void AddLog(LogEntry e)
    {
        int i = dgvLog.Rows.Add(e.Time.ToString("HH:mm:ss"), e.Direction, e.Type, e.Peer, e.Text);
        var row = dgvLog.Rows[i];
        row.DefaultCellStyle.BackColor =
            e.Type == "ERROR" ? Color.FromArgb(255, 222, 222) :
            e.Type == MessageTypes.LowStockAlert ? Color.FromArgb(255, 240, 190) :
            e.Direction == "IN" ? Color.FromArgb(232, 241, 255) :
            e.Direction == "OUT" ? Color.FromArgb(232, 247, 236) : Color.WhiteSmoke;
        if (dgvLog.Rows.Count > MaxLogRows) dgvLog.Rows.RemoveAt(0);
        try { dgvLog.FirstDisplayedScrollingRowIndex = dgvLog.Rows.Count - 1; }
        catch (InvalidOperationException) { /* grid not visible yet */ }
    }

    void ShowNotification(NotificationInfo n)
    {
        if (chkToasts.Checked && !IsDisposed) NotificationToast.Present(FindForm(), n.Title, n.Body, n.Kind);
    }

    void ReportError(string message)
    {
        AddLog(new LogEntry(DateTime.Now, "SYS", "ERROR", "", message));
        Ui.Warn(message);
    }

    // ---------- event handlers (wired in the Designer) ----------

    void tmrClock_Tick(object? sender, EventArgs e) => UpdateClock();

    void chkBroadcastInventory_CheckedChanged(object? sender, EventArgs e)
    {
        if (_net != null) _net.BroadcastInventory = chkBroadcastInventory.Checked;
    }

    void btnStartServer_Click(object? sender, EventArgs e)
    {
        if (_net == null) return;
        string bind = (cboBind.SelectedItem as BindItem)?.Address ?? NetUtil.AllInterfaces;
        _net.BroadcastInventory = chkBroadcastInventory.Checked;
        string? error = _net.StartServer(bind, txtServerPort.Text, txtServerCode.Text, chkAllowPublic.Checked);
        if (error != null) ReportError(error);
        UpdateUi();
    }

    async void btnStopServer_Click(object? sender, EventArgs e)
    {
        if (_net == null) return;
        btnStopServer.Enabled = false;
        try { await _net.StopServerAsync(); }
        catch (Exception ex) { Ui.Error(ex); }
        if (!IsDisposed) UpdateUi();
    }

    async void btnConnect_Click(object? sender, EventArgs e)
    {
        var net = _net;
        if (net == null) return;
        btnConnect.Enabled = btnTest.Enabled = false;
        lblClientStatus.ForeColor = Color.DimGray;
        lblClientStatus.Text = "Connecting...";
        try
        {
            string? error = await net.ConnectAsync(txtHost.Text, txtClientPort.Text, txtDeviceName.Text, txtClientCode.Text);
            if (error != null && !IsDisposed) ReportError(error);
        }
        catch (Exception ex) { if (!IsDisposed) Ui.Error(ex); }
        if (!IsDisposed) UpdateUi();
    }

    async void btnDisconnect_Click(object? sender, EventArgs e)
    {
        var net = _net;
        if (net == null) return;
        btnDisconnect.Enabled = false;
        try { await net.DisconnectAsync(); }
        catch (Exception ex) { if (!IsDisposed) Ui.Error(ex); }
        if (!IsDisposed) UpdateUi();
    }

    async void btnTest_Click(object? sender, EventArgs e)
    {
        var net = _net;
        if (net == null) return;
        btnTest.Enabled = false;
        lblTestResult.ForeColor = Color.DimGray;
        lblTestResult.Text = "Testing...";
        try
        {
            var r = await net.TestAsync(txtHost.Text, txtClientPort.Text);
            if (IsDisposed) return;
            lblTestResult.ForeColor = r.Ok ? Color.SeaGreen : Color.Firebrick;
            lblTestResult.Text = r.Message;
        }
        catch (Exception ex) { if (!IsDisposed) Ui.Error(ex); }
        if (!IsDisposed) btnTest.Enabled = _net != null && _net.Role == NetworkRole.None;
    }

    void txtMessage_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;           // no "ding"
        btnSend_Click(sender, EventArgs.Empty);
    }

    async void btnSend_Click(object? sender, EventArgs e)
    {
        var net = _net;
        if (net == null || !btnSend.Enabled) return;
        string recipient = (cboRecipient.SelectedItem as Recipient)?.Value ?? NetworkController.RecipientAll;
        btnSend.Enabled = false;
        try
        {
            string? error = await net.SendMessageAsync(recipient, txtMessage.Text);
            if (IsDisposed) return;
            if (error == null) txtMessage.Clear();
            else ReportError(error);
        }
        catch (Exception ex) { if (!IsDisposed) Ui.Error(ex); }
        if (!IsDisposed) { btnSend.Enabled = net.Role != NetworkRole.None; txtMessage.Focus(); }
    }

    void btnClearLog_Click(object? sender, EventArgs e) => dgvLog.Rows.Clear();

    async void btnCheckStock_Click(object? sender, EventArgs e) => await RunStockAsync("GET");

    async void btnApplyStock_Click(object? sender, EventArgs e)
    {
        if (numDelta.Value == 0) { Ui.Warn("Enter the quantity change (use a minus sign to remove stock, e.g. -3)."); return; }
        if (!Ui.Confirm($"Ask the server to change the stock of '{txtSku.Text.Trim()}' by {numDelta.Value:+#;-#;0}?")) return;
        await RunStockAsync("ADJUST");
    }

    async Task RunStockAsync(string action)
    {
        var net = _net;
        if (net == null) return;
        btnCheckStock.Enabled = btnApplyStock.Enabled = false;
        lblStockResult.ForeColor = Color.DimGray;
        lblStockResult.Text = "Waiting for the server...";
        try
        {
            var (ok, message) = await net.RemoteStockAsync(action, txtSku.Text, numDelta.Value.ToString(), txtReason.Text);
            if (IsDisposed) return;
            lblStockResult.ForeColor = ok ? Color.SeaGreen : Color.Firebrick;
            lblStockResult.Text = message;
        }
        catch (Exception ex) { if (!IsDisposed) Ui.Error(ex); }
        if (!IsDisposed) btnCheckStock.Enabled = btnApplyStock.Enabled = net.Role == NetworkRole.Client;
    }
}
