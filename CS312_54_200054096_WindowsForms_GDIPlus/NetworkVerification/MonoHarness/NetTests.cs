using System.Collections.Concurrent;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using InventoryManager;
using InventoryManager.Network;

/// <summary>A stand-in for the WinForms UI thread: Post() queues work; Pump() (called by the test on the MAIN thread) runs it.</summary>
sealed class PumpContext : SynchronizationContext
{
    readonly ConcurrentQueue<(SendOrPostCallback, object?)> _q = new();
    public readonly int ThreadId = Thread.CurrentThread.ManagedThreadId;
    public override void Post(SendOrPostCallback d, object? s) => _q.Enqueue((d, s));
    public override void Send(SendOrPostCallback d, object? s) { if (Thread.CurrentThread.ManagedThreadId == ThreadId) d(s); else Post(d, s); }
    public int Pump() { int n = 0; while (_q.TryDequeue(out var item)) { item.Item1(item.Item2); n++; } return n; }
}

public static class NetTests
{
    static int pass, fail;
    static PumpContext ctx = null!;
    static void Check(bool ok, string msg) { Console.WriteLine((ok ? "  PASS  " : "  FAIL  ") + msg); if (ok) pass++; else fail++; }
    static T Block<T>(Task<T> t, int ms = 15000) { var sw = System.Diagnostics.Stopwatch.StartNew(); while (!t.IsCompleted) { ctx.Pump(); Thread.Sleep(3); if (sw.ElapsedMilliseconds > ms) throw new TimeoutException("task did not complete"); } ctx.Pump(); return t.GetAwaiter().GetResult(); }
    static void Block(Task t, int ms = 15000) { var sw = System.Diagnostics.Stopwatch.StartNew(); while (!t.IsCompleted) { ctx.Pump(); Thread.Sleep(3); if (sw.ElapsedMilliseconds > ms) throw new TimeoutException("task did not complete"); } ctx.Pump(); t.GetAwaiter().GetResult(); }
    static bool Until(Func<bool> cond, int ms = 6000) { var sw = System.Diagnostics.Stopwatch.StartNew(); while (sw.ElapsedMilliseconds < ms) { ctx.Pump(); if (cond()) return true; Thread.Sleep(5); } ctx.Pump(); return cond(); }
    static int FreePort() { var l = new TcpListener(IPAddress.Loopback, 0); l.Start(); int p = ((IPEndPoint)l.LocalEndpoint).Port; l.Stop(); return p; }
    static long Qty(string sku) => Db.Scalar<long>("SELECT Quantity FROM Products WHERE Sku=@s", ("@s", sku));
    static long Id(string sku) => Db.Scalar<long>("SELECT Id FROM Products WHERE Sku=@s", ("@s", sku));
    static void Move(string sku, int delta, string reason)   // what the existing Stock Movements tab does
        => Db.Execute("UPDATE Products SET Quantity = Quantity + @d WHERE Id=@id; INSERT INTO StockMovements(ProductId,Delta,Reason,CreatedAt) VALUES(@id,@d,@r,@now);", ("@id", Id(sku)), ("@d", delta), ("@r", reason), ("@now", Db.Now()));
    static string Snap(string table) { var dt = Db.Query($"SELECT * FROM {table} ORDER BY 1"); return string.Join("\n", dt.Rows.Cast<DataRow>().Select(r => string.Join("|", r.ItemArray))); }

    sealed class Probe   // collects everything a controller raises, with the thread it was raised on
    {
        public List<LogEntry> Logs = new(); public List<NotificationInfo> Notes = new(); public int RoleChanges, DeviceChanges; public HashSet<int> Threads = new();
        public Probe(NetworkController c)
        {
            c.LogAdded += e => { Logs.Add(e); Threads.Add(Thread.CurrentThread.ManagedThreadId); };
            c.NotificationRaised += n => { Notes.Add(n); Threads.Add(Thread.CurrentThread.ManagedThreadId); };
            c.RoleChanged += () => { RoleChanges++; Threads.Add(Thread.CurrentThread.ManagedThreadId); };
            c.DevicesChanged += () => { DeviceChanges++; Threads.Add(Thread.CurrentThread.ManagedThreadId); };
        }
        public bool Got(string title) => Notes.Any(n => n.Title == title);
        public int Count(string title) => Notes.Count(n => n.Title == title);
    }

    public static int Main()
    {
        Environment.SetEnvironmentVariable("XDG_DATA_HOME", "/tmp/vauth/data_net_" + Guid.NewGuid().ToString("N").Substring(0, 6));
        ctx = new PumpContext(); SynchronizationContext.SetSynchronizationContext(ctx);
        Db.Init();
        var devA = new DeviceInfo("a1", "PC-2", "192.168.1.20", 50000, DateTime.UtcNow);

        Console.WriteLine("== W. InventoryWatcher: detects changes made by the EXISTING screens, no existing code touched ==");
        var w = new InventoryWatcher(); var changes = new List<(IReadOnlyList<InventoryChange> u, IReadOnlyList<LowStockInfo> l)>();
        w.Changes += (u, l) => changes.Add((u, l)); w.Start();
        Move("EL-001", -5, "Sold 5");
        Check(changes.Count == 1 && changes[0].u.Count == 1 && changes[0].u[0] is { Sku: "EL-001", OldQuantity: 120, NewQuantity: 115, Reason: "Sold 5", By: "" } && changes[0].l.Count == 0, "Stock Movements-style write -> INVENTORY_UPDATE 120 -> 115 with the movement's reason, no alert");
        changes.Clear(); Move("EL-004", -31, "bulk sale");
        Check(changes.Count == 1 && changes[0].l.Count == 1 && changes[0].l[0] is { Sku: "EL-004", Status: "LOW", Quantity: 14, ReorderLevel: 15, Supplier: "Delta Electronics" }, "45 -> 14 crosses the reorder level (15) -> LOW_STOCK_ALERT (status LOW, supplier included)");
        changes.Clear(); Move("EL-004", -14, "last ones");
        Check(changes.Count == 1 && changes[0].l.Count == 1 && changes[0].l[0].Status == "OUT", "14 -> 0 -> alert escalates to OUT");
        changes.Clear(); Move("EL-004", 50, "restock");
        Check(changes.Count == 1 && changes[0].u[0].NewQuantity == 50 && changes[0].l.Count == 0, "restocking above the level raises an update but NO alert");
        changes.Clear(); Move("EL-002", -1, "one more");   // EL-002 is already LOW (8/10 -> 7)
        Check(changes.Count == 1 && changes[0].l.Count == 0, "a product that is already LOW does not re-alert on every sale");
        changes.Clear(); Db.Execute("UPDATE Products SET ReorderLevel = 200 WHERE Sku='EL-001'");
        Check(changes.Count == 1 && changes[0].u.Count == 0 && changes[0].l.Count == 1 && changes[0].l[0].Sku == "EL-001", "raising a reorder level above the stock raises an alert (no quantity update)");
        changes.Clear(); Db.Execute("UPDATE Suppliers SET Phone='+20 999' WHERE Id=1");
        Check(changes.Count == 0, "unrelated writes (suppliers) produce no network events");
        changes.Clear(); Db.Execute("INSERT INTO Products(Sku,Name,Category,SupplierId,UnitPrice,Quantity,ReorderLevel) VALUES('NEW-1','New thing','X',1,2.0,2,5)");
        Check(changes.Count == 1 && changes[0].u[0].Reason == "Product added" && changes[0].l.Count == 1, "a new product (qty 2, level 5) -> 'Product added' update + LOW alert");
        changes.Clear(); Db.Execute("DELETE FROM Products WHERE Sku='NEW-1'");
        Check(changes.Count == 1 && changes[0].u[0] is { NewQuantity: 0, Reason: "Product removed" }, "deleting a product -> 'Product removed' update");
        changes.Clear(); w.Attribute(Id("OF-001"), "PC-9", "Network test"); Move("OF-001", -1, "ignored because attributed");
        Check(changes.Count == 1 && changes[0].u[0] is { By: "PC-9", Reason: "Network test" }, "attribution: broadcast says WHO changed the stock and why");
        w.Stop(); changes.Clear(); Move("OF-001", -1, "after stop");
        Check(changes.Count == 0, "after Stop() no events are raised");
        w.Start(); Move("OF-001", -1, "after restart"); Check(changes.Count == 1, "Start() works again after Stop()"); w.Dispose();

        Console.WriteLine("== G. DbInventoryGateway: executes on the UI thread, protects the data ==");
        var watcher = new InventoryWatcher(); var gwChanges = new List<IReadOnlyList<InventoryChange>>(); watcher.Changes += (u, l) => gwChanges.Add(u); watcher.Start();
        var gw = new DbInventoryGateway(ctx, watcher);
        var get = Block(gw.HandleAsync(new InventoryRequest("GET", "el-001", 0, ""), devA));
        Check(get.Ok && get.Quantity == Qty("EL-001") && get.Name == "USB-C Cable" && get.Sku == "EL-001", "GET works (case-insensitive SKU): " + get.Message);
        Check(!Block(gw.HandleAsync(new InventoryRequest("GET", "NOPE-1", 0, ""), devA)).Ok, "unknown SKU -> clear failure");
        Check(!Block(gw.HandleAsync(new InventoryRequest("GET", "' OR '1'='1", 0, ""), devA)).Ok, "SQL-injection text in the SKU finds nothing (parameterised)");
        long before = Qty("OF-002"); long movBefore = Db.Scalar<long>("SELECT COUNT(*) FROM StockMovements");
        var adj = Block(gw.HandleAsync(new InventoryRequest("ADJUST", "OF-002", -3, "sold at front desk"), devA));
        Check(adj.Ok && Qty("OF-002") == before - 3 && adj.Quantity == before - 3, $"ADJUST -3 applied on the server's database ({before} -> {Qty("OF-002")})");
        var mv = Db.Query("SELECT Delta, Reason FROM StockMovements ORDER BY Id DESC LIMIT 1").Rows[0];
        Check(Db.Scalar<long>("SELECT COUNT(*) FROM StockMovements") == movBefore + 1 && Convert.ToInt32(mv["Delta"]) == -3 && Convert.ToString(mv["Reason"]) == "Network (PC-2): sold at front desk", "a StockMovements row was written: \"" + mv["Reason"] + "\"");
        Check(gwChanges.Count == 1 && gwChanges[0][0] is { By: "PC-2", Reason: "sold at front desk" }, "the watcher attributes the change to PC-2");
        long q = Qty("OF-002"); movBefore = Db.Scalar<long>("SELECT COUNT(*) FROM StockMovements");
        var tooMany = Block(gw.HandleAsync(new InventoryRequest("ADJUST", "OF-002", -99999, ""), devA));
        Check(!tooMany.Ok && tooMany.Message.Contains("Not enough stock") && Qty("OF-002") == q && Db.Scalar<long>("SELECT COUNT(*) FROM StockMovements") == movBefore, "removing more than is in stock is refused; quantity and history unchanged");
        Block(gw.HandleAsync(new InventoryRequest("ADJUST", "OF-002", 10, ""), devA));
        check2();
        void check2() { var last = Db.Query("SELECT Reason FROM StockMovements ORDER BY Id DESC LIMIT 1").Rows[0][0]; Check(Qty("OF-002") == q + 10 && Convert.ToString(last) == "Network (PC-2): Network adjustment", "empty reason -> default text; positive adjustment works"); }
        Block(gw.HandleAsync(new InventoryRequest("ADJUST", "OF-002", 1, new string('r', 300)), devA));
        Check(Convert.ToString(Db.Query("SELECT Reason FROM StockMovements ORDER BY Id DESC LIMIT 1").Rows[0][0])!.Length <= 100, "a very long reason is truncated to the column limit");
        var threads = new ConcurrentBag<int>(); Action onChange = () => threads.Add(Thread.CurrentThread.ManagedThreadId); Db.DataChanged += onChange;
        var viaPool = Task.Run(() => gw.HandleAsync(new InventoryRequest("ADJUST", "OF-003", -1, "from a worker thread"), devA));   // like a network handler thread
        Block(viaPool); Db.DataChanged -= onChange;
        Check(threads.Count >= 1 && threads.All(t => t == ctx.ThreadId), $"a request arriving on a WORKER thread still writes to the database on the UI thread (DataChanged raised on thread {string.Join(",", threads.Distinct())}, UI thread = {ctx.ThreadId})");
        watcher.Dispose();

        Console.WriteLine("== C. NetworkController end-to-end (1 server + 2 clients, real TCP over loopback, real SQLite) ==");
        string suppliersBefore = Snap("Suppliers"), settingsBefore = Snap("Settings");
        int port = FreePort();
        using var srv = new NetworkController(ctx); using var ca = new NetworkController(ctx); using var cb = new NetworkController(ctx);
        var ps = new Probe(srv); var pa = new Probe(ca); var pb = new Probe(cb);
        Check(srv.StartServer("127.0.0.1", "abc", null, false)!.Contains("not a valid port") && srv.StartServer("999.1.1.1", "5000", null, false)!.Contains("not a valid IPv4") && srv.StartServer("0.0.0.0", "70000", null, false)!.Contains("out of range"), "invalid bind address / port texts are rejected with clear messages");
        Check(srv.StartServer("127.0.0.1", port.ToString(), null, false) == null && srv.Role == NetworkRole.Server && srv.Server.LocalEndPoint!.Port == port, $"server started on the requested port {port} (actual listening port reported)");
        Check(srv.StartServer("127.0.0.1", port.ToString(), null, false)!.Contains("already running"), "starting twice -> 'already running'");
        using (var other = new NetworkController(ctx)) Check(other.StartServer("127.0.0.1", port.ToString(), null, false)!.Contains("already in use"), "a second server on the same port -> 'already in use'");
        Check(Block(srv.ConnectAsync("127.0.0.1", port.ToString(), "X", null))!.Contains("Stop the server first"), "a server cannot also connect as a client");
        Check(ca.StartServer("127.0.0.1", "5999", null, false) == null, "(setup) another controller can run its own server on a different port"); Block(ca.StopServerAsync());
        string bad = Block(ca.ConnectAsync("192.168.1", port.ToString(), "PC-A", null)); Check(bad!.Contains("not a valid IPv4"), "client: invalid IP -> \"" + bad + "\"");
        Check(Block(ca.ConnectAsync("127.0.0.1", "", "PC-A", null))!.Contains("Enter a port"), "client: empty port -> clear message");
        Check(Block(ca.ConnectAsync("127.0.0.1", port.ToString(), "bad|name", null))!.Contains("Device name"), "client: invalid device name -> clear message");
        int dead = FreePort(); string refused = Block(ca.ConnectAsync("127.0.0.1", dead.ToString(), "PC-A", null))!; Check(refused.Contains("No server is listening"), "client: nothing listening -> \"" + refused + "\"");

        Check(Block(ca.ConnectAsync("127.0.0.1", port.ToString(), "PC-A", null)) == null && ca.Role == NetworkRole.Client, "client A connected with IP + port");
        Check(Block(cb.ConnectAsync("localhost", port.ToString(), "PC-B", null)) == null && cb.Role == NetworkRole.Client, "client B connected ('localhost' accepted as the IP)");
        string dup = Block(new NetworkController(ctx).ConnectAsync("127.0.0.1", port.ToString(), "pc-a", null))!; Check(dup.Contains("already connected"), "duplicate device name refused: \"" + dup + "\"");
        Check(Until(() => srv.CurrentDevices().Count == 2 && ca.CurrentDevices().Count == 2 && cb.CurrentDevices().Count == 2), "all three see 2 devices (server list; each client sees server + the other device)");
        var rowsS = srv.CurrentDevices(); Check(rowsS.All(r => r.Ip == "127.0.0.1" && r.Port > 0 && r.Status == "Connected") && rowsS.Select(r => r.Name).OrderBy(x => x).SequenceEqual(new[] { "PC-A", "PC-B" }), "server's Connected Devices: name, IP, port, status, time -> " + string.Join(", ", rowsS.Select(r => $"{r.Name}@{r.Ip}:{r.Port}")));
        Check(ca.CurrentDevices().First().IsServer && ca.CurrentDevices().Skip(1).Single().Name == "PC-B", "client A's list: the server first, then PC-B");
        Check(ps.Logs.Count(l => l.Type == "CONNECT") == 2 && ps.Got("Device connected") && ps.DeviceChanges >= 2, "server log + notification for each device that joined");
        Check(ca.GetRecipients().Select(r => r.Value).SequenceEqual(new[] { "ALL", "SERVER", "PC-B" }) && srv.GetRecipients().Select(r => r.Value).OrderBy(x => x).SequenceEqual(new[] { "ALL", "PC-A", "PC-B" }), "recipient lists: client = All/Server/other device; server = All/each device");

        // ---- messaging + notifications ----
        Check(Block(ca.SendMessageAsync("ALL", "hello everyone")) == null, "PC-A broadcasts a message");
        Check(Until(() => ps.Got("Broadcast from PC-A") && pb.Got("Broadcast from PC-A")), "server AND PC-B received it and raised a notification immediately");
        Check(!pa.Got("Broadcast from PC-A") && ps.Logs.Any(l => l.Direction == "IN" && l.Type == "BROADCAST" && l.Peer == "PC-A (127.0.0.1)" && l.Text == "hello everyone") && pb.Logs.Any(l => l.Direction == "IN" && l.Peer.StartsWith("PC-A")), "the sender gets no copy; logs show direction, type, sender name + IP, text");
        Check(Block(srv.SendMessageAsync("ALL", "server announcement")) == null && Until(() => pa.Got("Broadcast from " + srv.Server.ServerName) && pb.Got("Broadcast from " + srv.Server.ServerName)), "server -> all clients");
        Check(Block(ca.SendMessageAsync("PC-B", "private to B")) == null && Until(() => pb.Got("Message from PC-A")) && !ps.Got("Message from PC-A"), "PC-A -> PC-B private message relayed by the server (server does not display it as its own)");
        Check(Block(ca.SendMessageAsync("SERVER", "only the server")) == null && Until(() => ps.Got("Message from PC-A")) && pb.Count("Message from PC-A") == 1, "PC-A -> server only");
        Check(Block(srv.SendMessageAsync("PC-A", "server to A")) == null && Until(() => pa.Got("Message from " + srv.Server.ServerName)), "server -> one device");
        Check(Block(ca.SendMessageAsync("GHOST", "x"))!.Contains("No connected device"), "message to an unknown device -> clear error");
        Check(Block(ca.SendMessageAsync("ALL", "   "))!.Contains("Type a message") && Block(ca.SendMessageAsync("ALL", new string('x', 1001)))!.Contains("1000"), "empty / too long messages rejected before sending");
        using (var idle = new NetworkController(ctx)) Check(Block(idle.SendMessageAsync("ALL", "x"))!.Contains("Start the server or connect"), "messaging while neither server nor client -> guidance");

        // ---- inventory over the network ----
        long qEl1 = Qty("EL-001"); pa.Notes.Clear(); pb.Notes.Clear(); ps.Logs.Clear();
        var get2 = Block(ca.RemoteStockAsync("GET", "EL-001", "0", "")); Check(get2.Ok && get2.Message.Contains("USB-C Cable") && Qty("EL-001") == qEl1, "client asks the server for stock: \"" + get2.Message + "\" (nothing changed)");
        var ok1 = Block(ca.RemoteStockAsync("ADJUST", "EL-001", "-2", "sold")); Check(ok1.Ok && Qty("EL-001") == qEl1 - 2, "client changes stock via the server: " + ok1.Message);
        Check(Until(() => pa.Got("Stock update") && pb.Got("Stock update") && ps.Logs.Any(l => l.Direction == "OUT" && l.Type == "INVENTORY_UPDATE")), "INVENTORY_UPDATE broadcast to every client (requester too) and logged on the server");
        Check(pb.Notes.First(n => n.Title == "Stock update").Body.Contains("by PC-A") && pb.Notes.First(n => n.Title == "Stock update").Kind == "inventory", "notification says who changed it: \"" + pb.Notes.First(n => n.Title == "Stock update").Body + "\"");
        pa.Notes.Clear(); pb.Notes.Clear(); long q4 = Qty("EL-004");
        Block(ca.RemoteStockAsync("ADJUST", "EL-004", (-(q4 - 10)).ToString(), "big order"));   // leaves 10 (<= 15)
        Check(Until(() => pb.Got("Low stock alert")) && pb.Notes.First(n => n.Title == "Low stock alert").Kind == "lowstock" && pb.Notes.First(n => n.Title == "Low stock alert").Body.Contains("EL-004"), "LOW_STOCK_ALERT reaches the other clients: \"" + pb.Notes.FirstOrDefault(n => n.Title == "Low stock alert")?.Body + "\"");
        Block(ca.RemoteStockAsync("ADJUST", "EL-004", "-10", "all gone")); Check(Until(() => pb.Got("Out of stock")) && pb.Notes.First(n => n.Title == "Out of stock").Kind == "outofstock", "stock reaching 0 -> 'Out of stock' alert");
        var no = Block(ca.RemoteStockAsync("ADJUST", "EL-004", "-5", "")); Check(!no.Ok && no.Message.Contains("Not enough stock") && Qty("EL-004") == 0, "cannot go below zero: \"" + no.Message + "\"");
        Check(!Block(ca.RemoteStockAsync("ADJUST", "", "1", "")).Ok && !Block(ca.RemoteStockAsync("ADJUST", "EL-001", "abc", "")).Ok && !Block(ca.RemoteStockAsync("ADJUST", "EL-001", "0", "")).Ok && !Block(ca.RemoteStockAsync("ADJUST", "NOPE", "1", "")).Ok, "empty SKU / non-numeric / zero / unknown SKU -> clear failures");
        Check(!Block(srv.RemoteStockAsync("GET", "EL-001", "0", "")).Ok, "the server itself cannot send remote stock requests");
        pa.Notes.Clear(); pb.Notes.Clear(); Move("EL-003", 5, "restock at the server"); Move("OF-004", -4, "sold at the server");   // OF-004: 4 -> 0 (OUT)
        Check(Until(() => pa.Count("Stock update") >= 2 && pa.Got("Out of stock") && pb.Got("Out of stock")), "changes made by the SERVER's own screens (Stock Movements tab) are broadcast too, incl. alerts");
        srv.BroadcastInventory = false; pa.Notes.Clear(); Move("EL-002", -1, "silent"); Thread.Sleep(300); ctx.Pump();
        Check(pa.Notes.Count == 0, "with 'Broadcast stock changes' switched off nothing is sent"); srv.BroadcastInventory = true;

        // ---- test connection ----
        using (var tester = new NetworkController(ctx))
        {
            var good = Block(tester.TestAsync("127.0.0.1", port.ToString())); Check(good.Ok && good.Message.Contains(srv.Server.ServerName), "Test connection OK: " + good.Message);
            Check(!Block(tester.TestAsync("not-an-ip", "5000")).Ok && !Block(tester.TestAsync("127.0.0.1", "0")).Ok && !Block(tester.TestAsync("127.0.0.1", dead.ToString())).Ok, "Test connection: bad IP / bad port / closed port -> failure messages");
            Check(srv.CurrentDevices().Count == 2, "testing does not add a device");
        }

        // ---- unexpected disconnection ----
        pa.Notes.Clear(); ps.Notes.Clear(); pb.Notes.Clear();
        var tcp = (TcpClient)typeof(NetworkClient).GetField("_tcp", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(ca.Client)!;
        tcp.Client.LingerState = new LingerOption(true, 0); tcp.Close();      // like pulling the cable / killing the process
        Check(Until(() => srv.CurrentDevices().Count == 1 && ps.Logs.Any(l => l.Type == "DISCONNECT" && l.Peer.StartsWith("PC-A"))), "server notices the crashed client and removes it from Connected Devices");
        Check(Until(() => ps.Got("Device disconnected") && cb.CurrentDevices().Count == 1), "server shows a notification; the other client's list updates");
        Check(Until(() => ca.Role == NetworkRole.None && pa.Got("Disconnected from the server")), "the crashed client's own UI goes back to 'disconnected' and tells the user");
        Check(Block(ca.ConnectAsync("127.0.0.1", port.ToString(), "PC-A", null)) == null && Until(() => srv.CurrentDevices().Count == 2), "the client can reconnect with the same name afterwards");

        // ---- clean disconnect + server stop ----
        Block(ca.DisconnectAsync()); Check(ca.Role == NetworkRole.None && Until(() => srv.CurrentDevices().Count == 1), "Disconnect button: clean leave");
        Check(Block(ca.ConnectAsync("127.0.0.1", port.ToString(), "PC-A", null)) == null, "(reconnect)");
        pb.Notes.Clear(); Block(srv.StopServerAsync());
        Check(srv.Role == NetworkRole.None && Until(() => cb.Role == NetworkRole.None && ca.Role == NetworkRole.None && pb.Notes.Any(n => n.Body.Contains("stopped"))), "stopping the server tells every client why: \"" + (pb.Notes.FirstOrDefault(n => n.Body.Contains("stopped"))?.Body ?? "?") + "\"");
        Check(srv.CurrentDevices().Count == 0, "server's device list is empty after Stop");
        var l2 = new TcpListener(IPAddress.Loopback, port); l2.Start(); l2.Stop(); Check(true, "the port was released");
        Check(srv.StartServer("127.0.0.1", port.ToString(), null, false) == null && Block(cb.ConnectAsync("127.0.0.1", port.ToString(), "PC-B", null)) == null, "server restart + client reconnect work");

        Console.WriteLine("== H. Hygiene ==");
        Check(new[] { ps, pa, pb }.All(p => p.Threads.Count == 1 && p.Threads.Contains(ctx.ThreadId)), $"EVERY event the UI receives (log, notification, role, devices) arrives on the UI thread ({ps.Logs.Count + pa.Logs.Count + pb.Logs.Count} log entries, {ps.Notes.Count + pa.Notes.Count + pb.Notes.Count} notifications checked)");
        Check(Snap("Suppliers") == suppliersBefore, "Suppliers table untouched by networking");
        Check(Snap("Settings") == settingsBefore, "Settings table untouched by networking");
        srv.Dispose(); ca.Dispose(); cb.Dispose();
        var l3 = new TcpListener(IPAddress.Loopback, port); l3.Start(); l3.Stop(); Check(true, "disposing the controllers releases every socket");
        Console.WriteLine($"\nRESULT: {pass} passed, {fail} failed");
        return fail == 0 ? 0 : 1;
    }
}
