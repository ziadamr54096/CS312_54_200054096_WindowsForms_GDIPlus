using System.ComponentModel;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using InventoryManager;
using InventoryManager.Network;

class NDesignCtx : LicenseContext { public override LicenseUsageMode UsageMode => LicenseUsageMode.Designtime; }
class NRuntimeCtx : LicenseContext { public override LicenseUsageMode UsageMode => LicenseUsageMode.Runtime; }

public static class NetUiTests
{
    static int pass, fail;
    static void Check(bool ok, string msg) { Console.WriteLine((ok ? "  PASS  " : "  FAIL  ") + msg); if (ok) pass++; else fail++; }
    const BindingFlags Inst = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    static T F<T>(object o, string name) => (T)o.GetType().GetField(name, Inst | BindingFlags.DeclaredOnly)!.GetValue(o)!;
    static bool Until(Func<bool> cond, int ms = 8000) { var sw = System.Diagnostics.Stopwatch.StartNew(); while (sw.ElapsedMilliseconds < ms) { Application.DoEvents(); if (cond()) return true; Thread.Sleep(8); } Application.DoEvents(); return cond(); }
    static void Pause(int ms) { var sw = System.Diagnostics.Stopwatch.StartNew(); while (sw.ElapsedMilliseconds < ms) { Application.DoEvents(); Thread.Sleep(8); } }
    static bool Pause2(int ms) { Pause(ms); return false; }
    static int FreePort() { var l = new TcpListener(IPAddress.Loopback, 0); l.Start(); int p = ((IPEndPoint)l.LocalEndpoint).Port; l.Stop(); return p; }
    static Form Host(Control panel, int x, int y, string title)
    {
        var f = new Form { Text = title, StartPosition = FormStartPosition.Manual, Location = new Point(x, y), ClientSize = new Size(1140, 660) };
        panel.Dock = DockStyle.Fill; f.Controls.Add(panel); f.Show(); Application.DoEvents(); return f;
    }
    static void Type(object p, string field, string text) => F<TextBox>(p, field).Text = text;
    static void Click(object p, string field) => F<Button>(p, field).PerformClick();
    static string Lbl(object p, string field) => F<Label>(p, field).Text;
    static DataGridView G(object p, string name) => F<DataGridView>(p, name);
    static List<string[]> Rows(DataGridView g) => g.Rows.Cast<DataGridViewRow>().Select(r => r.Cells.Cast<DataGridViewCell>().Select(c => Convert.ToString(c.Value) ?? "").ToArray()).ToList();
    static bool LogHas(object p, string dir, string type, string textPart) => Rows(G(p, "dgvLog")).Any(r => r[1] == dir && r[2] == type && r[4].Contains(textPart));
    static List<NotificationToast> Toasts() => Application.OpenForms.Cast<Form>().OfType<NotificationToast>().ToList();
    static void CloseToasts() { foreach (var t in Toasts()) t.Close(); Application.DoEvents(); }
    static void Recipient(object p, string display) { var c = F<ComboBox>(p, "cboRecipient"); for (int i = 0; i < c.Items.Count; i++) if (c.Items[i]!.ToString()!.StartsWith(display)) c.SelectedIndex = i; }

    [STAThread]
    public static int Main(string[] args)
    {
        Environment.SetEnvironmentVariable("XDG_DATA_HOME", "/tmp/vauth/data_netui_" + Guid.NewGuid().ToString("N").Substring(0, 6));
        Application.EnableVisualStyles();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => Harness.Messages.Add("THREAD EXCEPTION: " + e.Exception);

        Console.WriteLine("== 1. DESIGN-TIME simulation of the new components (database or sockets would throw / be detected) ==");
        LicenseManager.CurrentContext = new NDesignCtx(); Harness.ForbidDb = true;
        foreach (var t in new[] { typeof(NetworkPanel), typeof(NotificationToast) })
        {
            var c = (Control)Activator.CreateInstance(t)!;
            var fields = t.GetFields(Inst | BindingFlags.DeclaredOnly).Where(f => typeof(Control).IsAssignableFrom(f.FieldType)).ToList();
            Check(fields.Count > 0 && fields.All(f => f.GetValue(c) != null) && fields.All(f => ((Control)f.GetValue(c)!).Parent != null), $"{t.Name}: InitializeComponent builds {fields.Count} controls, all placed in the tree");
            if (c is NetworkPanel np)
            {
                Check(G(np, "dgvDevices").Columns.Count == 6 && G(np, "dgvLog").Columns.Count == 5, "grids have their columns from the Designer (devices: 6, log: 5)");
                var h = Host(np, 10, 10, "design"); Pause(100);
                Check(!Harness.DbTouched && Ui.InDesigner(np) && F<System.Windows.Forms.Timer>(np, "tmrClock").Enabled == false, "loading the panel in the designer touches no database and starts no timer");
                Check(typeof(NetworkPanel).GetField("_net", Inst | BindingFlags.DeclaredOnly)!.GetValue(np) == null, "no network controller / sockets exist at design time");
                h.Close();
            }
            else { c.CreateControl(); c.Dispose(); }
        }

        LicenseManager.CurrentContext = new NRuntimeCtx(); Harness.ForbidDb = false;
        Db.Init();
        Console.WriteLine("== 2. Network tab at runtime: initial state ==");
        var sp = new NetworkPanel(); var sh = Host(sp, 0, 0, "SERVER");
        Check(Until(() => F<TextBox>(sp, "txtDeviceName").Text.Length > 0), "device name pre-filled with this computer's name: '" + F<TextBox>(sp, "txtDeviceName").Text + "'");
        var bind = F<ComboBox>(sp, "cboBind");
        Check(bind.Items.Count >= 1 && bind.Items[0]!.ToString()!.Contains("All network adapters") && bind.SelectedIndex == 0, "Server IP list: 'All network adapters' + this computer's IPv4 addresses: " + string.Join(" | ", bind.Items.Cast<object>().Select(o => o.ToString())));
        Check(Lbl(sp, "lblServerStatus") == "Stopped" && !F<Button>(sp, "btnStopServer").Enabled && Lbl(sp, "lblClientStatus") == "Disconnected" && !F<Button>(sp, "btnDisconnect").Enabled, "status: Stopped / Disconnected; Stop and Disconnect disabled");
        Check(!F<Button>(sp, "btnSend").Enabled && !F<Button>(sp, "btnApplyStock").Enabled && Lbl(sp, "lblLocalIps").Length > 3, "messaging + remote-stock disabled until connected; local IPv4 shown: " + Lbl(sp, "lblLocalIps"));

        Console.WriteLine("== 3. Server: errors, start, stop ==");
        Type(sp, "txtServerPort", "abc"); Harness.Messages.Clear(); Click(sp, "btnStartServer");
        Check(Harness.Messages.Any(m => m.Contains("not a valid port")) && Lbl(sp, "lblServerStatus") == "Stopped" && LogHas(sp, "SYS", "ERROR", "not a valid port"), "invalid port -> friendly message box + red log line; server stays stopped");
        int port = FreePort(); var blocker = new TcpListener(IPAddress.Any, port); blocker.Start();
        Type(sp, "txtServerPort", port.ToString()); Harness.Messages.Clear(); Click(sp, "btnStartServer");
        Check(Harness.Messages.Any(m => m.Contains("already in use")) && Lbl(sp, "lblServerStatus") == "Stopped", "port taken by another program -> 'already in use' message; server stays stopped"); blocker.Stop();
        Type(sp, "txtServerPort", port.ToString()); Click(sp, "btnStartServer");
        Check(Lbl(sp, "lblServerStatus").StartsWith("Running") && Lbl(sp, "lblServerEndpoint").EndsWith(":" + port) && F<Button>(sp, "btnStopServer").Enabled && !F<Button>(sp, "btnStartServer").Enabled && !F<TextBox>(sp, "txtServerPort").Enabled, "server running: status, actual endpoint '" + Lbl(sp, "lblServerEndpoint") + "', inputs locked");
        Check(Lbl(sp, "lblLocalIps").Contains(":" + port) || Lbl(sp, "lblLocalIps").Contains("No active network"), "the address clients must use is shown: " + Lbl(sp, "lblLocalIps"));
        Check(!F<Button>(sp, "btnConnect").Enabled && !F<TextBox>(sp, "txtHost").Enabled && F<Button>(sp, "btnSend").Enabled && !F<Button>(sp, "btnApplyStock").Enabled, "client controls disabled while serving; messaging enabled; remote stock stays disabled (clients only)");

        Console.WriteLine("== 4. Two client computers connect (real TCP) ==");
        var ap = new NetworkPanel(); var ah = Host(ap, 30, 30, "CLIENT A"); var bp = new NetworkPanel(); var bh = Host(bp, 60, 60, "CLIENT B");
        Type(ap, "txtHost", "127.0.0.1"); Type(ap, "txtClientPort", port.ToString()); Type(ap, "txtDeviceName", "PC-A");
        Click(ap, "btnTest");
        Check(Until(() => Lbl(ap, "lblTestResult").Contains("answered in")) && F<Label>(ap, "lblTestResult").ForeColor == Color.SeaGreen, "Test connection: '" + Lbl(ap, "lblTestResult") + "' (green)");
        Check(G(sp, "dgvDevices").Rows.Count == 0, "the test did not register a device");
        Type(ap, "txtHost", "300.1.1.1"); Harness.Messages.Clear(); Click(ap, "btnConnect");
        Check(Harness.Messages.Any(m => m.Contains("not a valid IPv4")) && Lbl(ap, "lblClientStatus") == "Disconnected", "invalid IP -> friendly message, still disconnected");
        Type(ap, "txtHost", "127.0.0.1"); Click(ap, "btnConnect");
        Check(Until(() => Lbl(ap, "lblClientStatus").StartsWith("Connected to")) && F<Button>(ap, "btnDisconnect").Enabled && !F<Button>(ap, "btnConnect").Enabled && F<Button>(ap, "btnApplyStock").Enabled, "client A: 'Connected to ...', Disconnect enabled, remote stock enabled");
        Type(bp, "txtHost", "127.0.0.1"); Type(bp, "txtClientPort", port.ToString()); Type(bp, "txtDeviceName", "PC-B"); Click(bp, "btnConnect");
        Check(Until(() => Lbl(bp, "lblClientStatus").StartsWith("Connected to")), "client B connected");
        Check(Until(() => G(sp, "dgvDevices").Rows.Count == 2 && G(ap, "dgvDevices").Rows.Count == 2 && G(bp, "dgvDevices").Rows.Count == 2), "Connected Devices updates automatically on all three screens");
        var rows = Rows(G(sp, "dgvDevices")).OrderBy(r => r[0]).ToList();
        Check(rows[0][0] == "PC-A" && rows[0][1] == "127.0.0.1" && int.Parse(rows[0][2]) > 0 && rows[0][3] == "Connected" && System.Text.RegularExpressions.Regex.IsMatch(rows[0][4], @"^\d{4}-\d\d-\d\d \d\d:\d\d:\d\d$") && rows[1][0] == "PC-B", "server list columns: " + string.Join(" | ", rows[0]));
        Check(Lbl(sp, "lblDeviceCount").StartsWith("2 device(s)") && Rows(G(ap, "dgvDevices"))[0][0].Contains("(server)") && Rows(G(ap, "dgvDevices"))[1][0] == "PC-B", "counts + client A sees the server row and PC-B");
        Check(LogHas(sp, "SYS", "CONNECT", "connected") && Toasts().Count > 0, "server log line + pop-up for new devices");
        typeof(NetworkPanel).GetMethod("tmrClock_Tick", Inst)!.Invoke(ap, new object?[] { null, EventArgs.Empty }); Pause(1200); typeof(NetworkPanel).GetMethod("tmrClock_Tick", Inst)!.Invoke(ap, new object?[] { null, EventArgs.Empty });
        Check(System.Text.RegularExpressions.Regex.IsMatch(Lbl(ap, "lblDuration"), @"^00:00:0[1-9]$") && Rows(G(sp, "dgvDevices")).Count == 2, "connection duration counts up: " + Lbl(ap, "lblDuration"));
        var rec = F<ComboBox>(ap, "cboRecipient").Items.Cast<object>().Select(o => o.ToString()!).ToList();
        Check(rec.Count == 3 && rec[0].StartsWith("All devices") && rec[1].StartsWith("Server only") && rec[2] == "PC-B", "client recipient list: " + string.Join(" | ", rec));

        Console.WriteLine("== 5. Messaging + notifications ==");
        CloseToasts(); Type(ap, "txtMessage", "hello from A"); Recipient(ap, "All devices"); Click(ap, "btnSend");
        Check(Until(() => F<TextBox>(ap, "txtMessage").Text == "") && Until(() => LogHas(sp, "IN", "BROADCAST", "hello from A") && LogHas(bp, "IN", "BROADCAST", "hello from A")), "A -> all: message box cleared; server and B log it as an incoming BROADCAST");
        var row = Rows(G(sp, "dgvLog")).Last(r => r[2] == "BROADCAST"); Check(row[3].Contains("PC-A") && row[3].Contains("127.0.0.1") && row[0].Length == 8 && row[1] == "IN", "log row shows time, direction, type, sender name + IP: " + string.Join(" | ", row));
        Check(LogHas(ap, "OUT", "BROADCAST", "hello from A") && !LogHas(ap, "IN", "BROADCAST", "hello from A"), "A logs it as outgoing and gets no copy back");
        Check(Until(() => Toasts().Any(t => F<Label>(t, "lblTitle").Text.StartsWith("Broadcast from PC-A"))), "pop-up notification appears on the receiving computer: '" + (Toasts().Count > 0 ? F<Label>(Toasts().Last(), "lblTitle").Text + " / " + F<Label>(Toasts().Last(), "lblBody").Text : "none") + "'");
        var toast = Toasts().First(); var wa = Screen.FromControl(sh).WorkingArea;
        Check(toast.Right <= wa.Right && toast.Bottom <= wa.Bottom && toast.Left > wa.Left, "the pop-up is placed inside the screen's bottom-right corner");
        CloseToasts(); Type(ap, "txtMessage", "just for B"); Recipient(ap, "PC-B"); Click(ap, "btnSend");
        Check(Until(() => LogHas(bp, "IN", "CHAT", "just for B")) && !LogHas(sp, "IN", "CHAT", "just for B"), "A -> PC-B private message arrives only at B (relayed by the server)");
        Type(ap, "txtMessage", "to the server"); Recipient(ap, "Server only"); Click(ap, "btnSend");
        Check(Until(() => LogHas(sp, "IN", "CHAT", "to the server")) && !LogHas(bp, "IN", "CHAT", "to the server"), "A -> server only");
        Type(sp, "txtMessage", "announcement from the server"); Recipient(sp, "All devices"); typeof(NetworkPanel).GetMethod("txtMessage_KeyDown", Inst)!.Invoke(sp, new object?[] { null, new KeyEventArgs(Keys.Enter) });
        Check(Until(() => LogHas(ap, "IN", "BROADCAST", "announcement from the server") && LogHas(bp, "IN", "BROADCAST", "announcement from the server")) && Until(() => F<TextBox>(sp, "txtMessage").Text == ""), "server broadcast via the ENTER key reaches both clients");
        foreach (var pn in new[] { sp, ap, bp }) F<CheckBox>(pn, "chkToasts").Checked = false;
        CloseToasts(); Type(sp, "txtMessage", "quiet one"); Recipient(sp, "All devices"); Click(sp, "btnSend");
        Check(Until(() => LogHas(ap, "IN", "BROADCAST", "quiet one")) && (Pause2(700) || Toasts().Count == 0), "with 'Show pop-up notifications' switched off the message is still logged but NO pop-up appears");
        foreach (var pn in new[] { sp, ap, bp }) F<CheckBox>(pn, "chkToasts").Checked = true;
        Harness.Messages.Clear(); Type(ap, "txtMessage", ""); Click(ap, "btnSend");
        Check(Until(() => Harness.Messages.Any(m => m.Contains("Type a message"))), "empty message -> friendly warning");

        Console.WriteLine("== 6. Inventory over the network ==");
        Type(ap, "txtSku", "EL-001"); Click(ap, "btnCheckStock");
        Check(Until(() => Lbl(ap, "lblStockResult").Contains("USB-C Cable") && Lbl(ap, "lblStockResult").Contains("in stock")), "Check stock: '" + Lbl(ap, "lblStockResult") + "'");
        long q0 = Db.Scalar<long>("SELECT Quantity FROM Products WHERE Sku='EL-001'"); CloseToasts();
        F<NumericUpDown>(ap, "numDelta").Value = -2; Type(ap, "txtReason", "sold at desk"); Click(ap, "btnApplyStock");
        Check(Until(() => Db.Scalar<long>("SELECT Quantity FROM Products WHERE Sku='EL-001'") == q0 - 2) && Until(() => Lbl(ap, "lblStockResult").Contains("->")) && F<Label>(ap, "lblStockResult").ForeColor == Color.SeaGreen, "Apply change: the SERVER's database changed (" + q0 + " -> " + (q0 - 2) + "): '" + Lbl(ap, "lblStockResult") + "'");
        Check(Until(() => LogHas(bp, "IN", "INVENTORY_UPDATE", "by PC-A") && LogHas(sp, "OUT", "INVENTORY_UPDATE", "EL-001")) && Until(() => Toasts().Any(t => F<Label>(t, "lblTitle").Text == "Stock update")), "other devices got the INVENTORY_UPDATE notification; server logged the broadcast");
        long q4 = Db.Scalar<long>("SELECT Quantity FROM Products WHERE Sku='EL-004'"); F<NumericUpDown>(ap, "numDelta").Value = -(q4 - 5); Type(ap, "txtSku", "EL-004"); Click(ap, "btnApplyStock");
        Check(Until(() => LogHas(bp, "IN", "LOW_STOCK_ALERT", "EL-004")) && Rows(G(bp, "dgvLog")).Any(r => r[2] == "LOW_STOCK_ALERT"), "stock falling to the reorder level -> LOW_STOCK_ALERT in every client's log");
        F<NumericUpDown>(ap, "numDelta").Value = -9999; Click(ap, "btnApplyStock");
        Check(Until(() => Lbl(ap, "lblStockResult").Contains("Not enough stock")) && F<Label>(ap, "lblStockResult").ForeColor == Color.Firebrick, "too much -> '" + Lbl(ap, "lblStockResult") + "' (red)");
        F<NumericUpDown>(ap, "numDelta").Value = 0; Harness.Messages.Clear(); Click(ap, "btnApplyStock");
        Check(Harness.Messages.Any(m => m.Contains("Enter the quantity change")), "zero change -> asks for a quantity");
        // a change made by the server's OWN screens (what MovementsPanel does)
        Db.Execute("UPDATE Products SET Quantity = Quantity - 1 WHERE Sku='OF-001'; INSERT INTO StockMovements(ProductId,Delta,Reason,CreatedAt) VALUES((SELECT Id FROM Products WHERE Sku='OF-001'),-1,'sold at the server',@n)", ("@n", Db.Now()));
        Check(Until(() => LogHas(ap, "IN", "INVENTORY_UPDATE", "OF-001") && LogHas(bp, "IN", "INVENTORY_UPDATE", "OF-001")), "a stock change made on the server computer itself is pushed to the clients");

        Console.WriteLine("== 7. Disconnect, crash, stop, log limits ==");
        Click(ap, "btnDisconnect");
        Check(Until(() => Lbl(ap, "lblClientStatus") == "Disconnected" && F<Button>(ap, "btnConnect").Enabled && Until(() => G(sp, "dgvDevices").Rows.Count == 1)), "Disconnect: client back to 'Disconnected'; server list drops to 1 device");
        var net = (NetworkController)typeof(NetworkPanel).GetField("_net", Inst | BindingFlags.DeclaredOnly)!.GetValue(bp)!;
        var tcp = (TcpClient)typeof(NetworkClient).GetField("_tcp", Inst)!.GetValue(net.Client)!; tcp.Client.LingerState = new LingerOption(true, 0); tcp.Close();
        Check(Until(() => Lbl(bp, "lblClientStatus") == "Disconnected" && G(sp, "dgvDevices").Rows.Count == 0 && LogHas(sp, "SYS", "DISCONNECT", "left")) && LogHas(bp, "SYS", "DISCONNECT", "Disconnected"), "crashed client B: server removes it and logs why; B's screen returns to 'Disconnected'");
        Click(ap, "btnConnect"); Until(() => Lbl(ap, "lblClientStatus").StartsWith("Connected to")); Click(bp, "btnConnect"); Check(Until(() => Lbl(bp, "lblClientStatus").StartsWith("Connected to") && G(sp, "dgvDevices").Rows.Count == 2), "both reconnect");
        Click(sp, "btnStopServer");
        Check(Until(() => Lbl(sp, "lblServerStatus") == "Stopped" && F<Button>(sp, "btnStartServer").Enabled && Lbl(ap, "lblClientStatus") == "Disconnected" && Lbl(bp, "lblClientStatus") == "Disconnected"), "Stop server: server 'Stopped'; both clients drop to 'Disconnected'");
        Check(Rows(G(ap, "dgvLog")).Any(r => r[4].Contains("stopped")), "clients' logs explain: the server was stopped");
        var l2 = new TcpListener(IPAddress.Loopback, port); l2.Start(); l2.Stop(); Check(true, "the port is free after Stop");
        var addLog = typeof(NetworkPanel).GetMethod("AddLog", Inst)!; for (int i = 0; i < 620; i++) addLog.Invoke(sp, new object[] { new LogEntry(DateTime.Now, "SYS", "TEST", "", "line " + i) });
        Check(G(sp, "dgvLog").Rows.Count <= 500, $"communication log is capped ({G(sp, "dgvLog").Rows.Count} rows after 620 entries)");
        Click(sp, "btnClearLog"); Check(G(sp, "dgvLog").Rows.Count == 0, "Clear log");

        Console.WriteLine("== 8. Screenshots + cleanup when the window closes (logout) ==");
        Type(sp, "txtServerPort", port.ToString()); Click(sp, "btnStartServer"); Click(ap, "btnConnect"); Click(bp, "btnConnect"); Until(() => G(sp, "dgvDevices").Rows.Count == 2);
        Type(ap, "txtMessage", "Low stock at the front desk?"); Recipient(ap, "All devices"); Click(ap, "btnSend"); Type(ap, "txtSku", "EL-002"); Click(ap, "btnCheckStock"); Pause(800);
        sh.Location = new Point(0, 0); sh.BringToFront(); sh.Refresh(); Pause(500);
        System.Diagnostics.Process.Start("scrot", "-o /tmp/vauth/net_server.png")!.WaitForExit();
        ah.Location = new Point(0, 0); ah.BringToFront(); ah.Refresh(); Pause(500);
        System.Diagnostics.Process.Start("scrot", "-o /tmp/vauth/net_client.png")!.WaitForExit();
        sh.Close(); Pause(300);
        Check(Until(() => Lbl(ap, "lblClientStatus") == "Disconnected"), "closing the SERVER window (as logout does) stops the server and disconnects its clients");
        var l3 = new TcpListener(IPAddress.Loopback, port); l3.Start(); l3.Stop(); Check(true, "...and releases the port");
        ah.Close(); bh.Close();

        var mf = new MainForm(); mf.StartPosition = FormStartPosition.Manual; mf.Location = new Point(0, 0); mf.Show(); Pause(500);
        var tabs = F<TabControl>(mf, "tabMain");
        Check(string.Join("|", tabs.TabPages.Cast<TabPage>().Select(p => p.Text)) == "Dashboard|Products|Suppliers|Stock Movements|Reports|Settings|Network", "MainForm: existing six tabs unchanged + new 'Network' tab (last)");
        tabs.SelectedIndex = 6; Pause(400); var np2 = F<NetworkPanel>(mf, "networkPanel"); Type(np2, "txtServerPort", port.ToString()); Click(np2, "btnStartServer");
        Check(Lbl(np2, "lblServerStatus").StartsWith("Running"), "server started from the real MainForm's Network tab");
        tabs.SelectedIndex = 1; Pause(200); tabs.SelectedIndex = 6; Pause(200);
        System.Diagnostics.Process.Start("scrot", "-o /tmp/vauth/net_mainform.png")!.WaitForExit();
        mf.Close(); Pause(500);
        var l4 = new TcpListener(IPAddress.Loopback, port); l4.Start(); l4.Stop(); Check(true, "closing MainForm (logout / exit) stops the server and frees the port");
        Check(!Harness.Messages.Any(m => m.StartsWith("THREAD EXCEPTION")), "no unhandled exceptions during the whole UI run" + (Harness.Messages.Any(m => m.StartsWith("THREAD EXCEPTION")) ? ": " + Harness.Messages.First(m => m.StartsWith("THREAD EXCEPTION")) : ""));
        Console.WriteLine($"\nRESULT: {pass} passed, {fail} failed");
        return fail == 0 ? 0 : 1;
    }
}
