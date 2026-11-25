using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using InventoryManager.Network;

sealed class FakeGateway : IInventoryGateway
{
    public ConcurrentQueue<InventoryRequest> Requests = new(); public bool Throw;
    public Task<InventoryResult> HandleAsync(InventoryRequest r, DeviceInfo from)
    {
        Requests.Enqueue(r);
        if (Throw) throw new InvalidOperationException("boom (secret internals)");
        return Task.FromResult(r.Action == "GET" ? new InventoryResult(true, "Stock for X", r.Sku, "Widget", 42, 5)
                                                 : new InventoryResult(true, $"{from.Name} changed {r.Sku} by {r.Delta}", r.Sku, "Widget", 42 + r.Delta, 5));
    }
}

sealed class Raw : IDisposable
{
    public TcpClient Tcp = new(AddressFamily.InterNetwork) { NoDelay = true }; public NetworkStream S = null!;
    public async Task ConnectAsync(int port) { await Tcp.ConnectAsync(IPAddress.Loopback, port); S = Tcp.GetStream(); }
    public Task SendAsync(NetMessage m) => FrameCodec.WriteAsync(S, m, CancellationToken.None);
    public async Task<NetMessage?> ReadAsync(int ms = 3000)
    { var t = FrameCodec.ReadAsync(S, CancellationToken.None); if (await Task.WhenAny(t, Task.Delay(ms)) != t) throw new TimeoutException("no frame"); return await t; }
    public async Task<NetMessage?> ResponseAsync(int ms = 3000)
    { while (true) { var m = await ReadAsync(ms); if (m == null || m.Type == MessageTypes.ServerResponse) return m; } }
    public async Task<bool> ClosedWithinAsync(int ms = 2500)   // drains frames; true when the server closes the connection
    { try { var sw = Stopwatch.StartNew(); while (sw.ElapsedMilliseconds < ms) { var m = await ReadAsync(Math.Max(100, ms - (int)sw.ElapsedMilliseconds)); if (m == null) return true; } return false; } catch (IOException) { return true; } catch (TimeoutException) { return false; } }
    public async Task<NetMessage?> HandshakeAsync(string name, string? code = null)
    { await SendAsync(NetMessage.Create(MessageTypes.Connect, name, data: new() { ["name"] = name, ["code"] = code ?? "" })); return await ResponseAsync(); }
    public void Abort() { try { Tcp.Client.LingerState = new LingerOption(true, 0); } catch { } Tcp.Close(); }
    public void Dispose() { try { Tcp.Close(); } catch { } }
}

sealed class OneByteStream : Stream   // delivers data one byte per Read - the worst case for a naive reader
{
    readonly MemoryStream _m; public OneByteStream(byte[] b) { _m = new MemoryStream(b); }
    public override int Read(byte[] buffer, int offset, int count) => _m.Read(buffer, offset, Math.Min(1, count));
    public override bool CanRead => true; public override bool CanSeek => false; public override bool CanWrite => false; public override long Length => throw new NotSupportedException();
    public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    public override void Flush() { } public override long Seek(long o, SeekOrigin s) => throw new NotSupportedException();
    public override void SetLength(long v) => throw new NotSupportedException(); public override void Write(byte[] b, int o, int c) => throw new NotSupportedException();
}

public static class CoreTests
{
    static int pass, fail;
    static void Check(bool ok, string msg) { Console.WriteLine((ok ? "  PASS  " : "  FAIL  ") + msg); if (ok) pass++; else { fail++; } }
    static async Task<bool> WaitFor(Func<bool> cond, int ms = 4000) { var sw = Stopwatch.StartNew(); while (sw.ElapsedMilliseconds < ms) { if (cond()) return true; await Task.Delay(20); } return cond(); }
    static NetworkServer NewServer(Action<NetworkServer>? cfg = null) { var s = new NetworkServer { ServerName = "SRV", WatchdogInterval = TimeSpan.FromMilliseconds(100) }; cfg?.Invoke(s); s.Start(IPAddress.Loopback, 0); return s; }
    static int PortOf(NetworkServer s) => s.LocalEndPoint!.Port;
    static async Task<NetworkClient> Join(NetworkServer s, string name, string? code = null)
    { var c = new NetworkClient { HeartbeatInterval = TimeSpan.FromMilliseconds(300) }; await c.ConnectAsync(IPAddress.Loopback, PortOf(s), name, code); return c; }
    static async Task<string> ErrOf(Func<Task> a) { try { await a(); return "<no error>"; } catch (NetworkException ex) { return ex.Message; } catch (Exception ex) { return "WRONG " + ex.GetType().Name + ": " + ex.Message; } }

    static async Task Run(string name, Func<Task> body)
    {
        Console.WriteLine($"== {name} ==");
        try { var t = body(); if (await Task.WhenAny(t, Task.Delay(40000)) != t) { Check(false, "TEST TIMED OUT (40 s)"); return; } await t; }
        catch (Exception ex) { Check(false, $"unexpected exception: {ex.GetType().Name}: {ex.Message}"); }
    }

    public static async Task<int> Main()
    {
        await Run("1. Framing: round trip, one byte at a time, coalesced frames", async () =>
        {
            var m = NetMessage.Create(MessageTypes.Chat, "PC-1", "héllo wörld ✓ 日本語", "SERVER", new() { ["k"] = "v" });
            byte[] frame = FrameCodec.Encode(m);
            int len = (frame[0] << 24) | (frame[1] << 16) | (frame[2] << 8) | frame[3];
            Check(len == frame.Length - 4, "4-byte big-endian length prefix matches the JSON size");
            var back = await FrameCodec.ReadAsync(new MemoryStream(frame), CancellationToken.None);
            Check(back != null && back.Text == m.Text && back.Target == "SERVER" && back.Get("k") == "v" && back.Id == m.Id, "round trip keeps every field (incl. Unicode)");
            var slow = await FrameCodec.ReadAsync(new OneByteStream(frame), CancellationToken.None);
            Check(slow != null && slow.Text == m.Text, "a message delivered ONE BYTE PER READ is reassembled correctly");
            var two = new MemoryStream(); two.Write(frame); two.Write(FrameCodec.Encode(NetMessage.Create(MessageTypes.Ping, "x"))); two.Position = 0;
            var a = await FrameCodec.ReadAsync(two, CancellationToken.None); var b = await FrameCodec.ReadAsync(two, CancellationToken.None); var c = await FrameCodec.ReadAsync(two, CancellationToken.None);
            Check(a?.Type == "CHAT" && b?.Type == "PING" && c == null, "two frames in one buffer are split correctly, then clean end-of-stream = null");
            Check(!System.Text.Encoding.UTF8.GetString(FrameCodec.Encode(m)).Contains("IsOk"), "no computed properties leak onto the wire");
        });

        await Run("2. Framing: malformed input is rejected, never crashes", async () =>
        {
            async Task<string> Try(byte[] bytes) { try { await FrameCodec.ReadAsync(new MemoryStream(bytes), CancellationToken.None); return "<ok>"; } catch (Exception ex) { return ex.GetType().Name; } }
            Check(await Try(new byte[] { 0, 0, 0, 0 }) == nameof(InvalidDataException), "zero length rejected");
            Check(await Try(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }) == nameof(InvalidDataException), "negative length rejected");
            Check(await Try(new byte[] { 0, 1, 0, 1 }) == nameof(InvalidDataException), "length 65537 > 64 KB limit rejected");
            Check(await Try(new byte[] { 0, 0, 0, 10, 1, 2, 3 }) == nameof(EndOfStreamException), "truncated body -> EndOfStreamException");
            Check(await Try(new byte[] { 0, 0 }) == nameof(EndOfStreamException), "truncated header -> EndOfStreamException");
            byte[] Frame(string json) { var j = System.Text.Encoding.UTF8.GetBytes(json); var f = new byte[4 + j.Length]; f[3] = (byte)j.Length; f[2] = (byte)(j.Length >> 8); Array.Copy(j, 0, f, 4, j.Length); return f; }
            Check(await Try(Frame("not json at all")) == nameof(InvalidDataException), "invalid JSON rejected");
            Check(await Try(Frame("{\"V\":1,\"Type\":\"HACK\"}")) == nameof(InvalidDataException), "unknown message type rejected");
            Check(await Try(Frame("{\"V\":9,\"Type\":\"PING\"}")) == nameof(InvalidDataException), "unsupported protocol version rejected");
            Check(await Try(Frame("null")) == nameof(InvalidDataException), "JSON null rejected");
            Check(await Try(Frame("{\"V\":1,\"Type\":\"CHAT\",\"Text\":\"" + new string('x', 4001) + "\"}")) == nameof(InvalidDataException), "text over 4000 characters rejected");
            Check(await Try(FrameCodec.Encode(NetMessage.Create(MessageTypes.Chat, "a", new string('y', 4000)))) == "<ok>", "text of exactly 4000 characters accepted");
        });

        await Run("3. Input validation (IP address, port, device name, LAN detection)", () =>
        {
            foreach (var ok in new[] { "192.168.1.10", " 10.0.0.5 ", "172.16.5.4", "127.0.0.1", "localhost", "8.8.8.8" })
                Check(NetUtil.TryParseServerAddress(ok, out _, out _), $"valid address accepted: '{ok}'");
            foreach (var bad in new[] { "", "   ", "1", "10.1", "256.1.1.1", "1.2.3.4.5", "abc", "192.168.1.10:5000", "01.2.3.4", "0.0.0.0", "255.255.255.255", "224.0.0.1", "192.168.1", "192,168,1,10", "http://192.168.1.10" })
            { bool r = NetUtil.TryParseServerAddress(bad, out _, out string e); Check(!r && e.Length > 10, $"invalid address rejected with a message: '{bad}'"); }
            foreach (var ok in new[] { "5000", "1", "65535", " 8080 " }) Check(NetUtil.TryParsePort(ok, out _, out _), $"valid port: '{ok}'");
            foreach (var bad in new[] { "", "0", "65536", "-1", "abc", "50 00", "5000.5", "+5000", "99999999999" })
            { bool r = NetUtil.TryParsePort(bad, out _, out string e); Check(!r && e.Length > 10, $"invalid port rejected with a message: '{bad}'"); }
            Check(NetUtil.ValidateDeviceName("PC-2") == null && NetUtil.ValidateDeviceName("Lab PC 3") == null, "normal device names accepted");
            Check(new[] { "", " ", "a|b", "-x", "x\ny", new string('a', 33), "SERVER", "all", "a<b>" }.All(n => NetUtil.ValidateDeviceName(n) != null), "empty / '|' / newline / too long / reserved names rejected");
            Check(NetUtil.ValidateDeviceName(NetUtil.DefaultDeviceName()) == null, "default device name (machine name) is valid: " + NetUtil.DefaultDeviceName());
            Check(new[] { "192.168.1.1", "10.2.3.4", "172.16.0.1", "172.31.255.255", "169.254.1.1", "127.0.0.1", "::1" }.All(a => NetUtil.IsPrivateLan(IPAddress.Parse(a))), "private/loopback ranges recognised as LAN");
            Check(new[] { "8.8.8.8", "172.32.0.1", "172.15.0.1", "100.64.0.1", "203.0.113.5", "2001:db8::1" }.All(a => !NetUtil.IsPrivateLan(IPAddress.Parse(a))), "public addresses are NOT treated as LAN");
            Check(NetUtil.IsPrivateLan(IPAddress.Parse("::ffff:192.168.1.5")), "IPv4-mapped IPv6 address handled");
            Check(DeviceListCodec.Decode(DeviceListCodec.Encode(new[] { new DeviceInfo("a1", "PC-1", "10.0.0.2", 51000, DateTime.UtcNow), new DeviceInfo("b2", "PC 2", "10.0.0.3", 51001, DateTime.UtcNow) })).Count == 2
                  && DeviceListCodec.Decode("garbage\n||||\nx|y|z|notnum|1").Count == 0, "device list text codec round-trips and ignores garbage lines");
            return Task.CompletedTask;
        });

        await Run("4. Server lifecycle: start, stop, restart, port in use, invalid port", async () =>
        {
            var s = NewServer();
            int port = PortOf(s);
            Check(s.IsRunning && port > 0, $"server running on a real port ({s.LocalEndPoint})");
            Check((await ErrOf(async () => { s.Start(IPAddress.Loopback, 0); await Task.CompletedTask; })).StartsWith("WRONG InvalidOperationException"), "starting twice is refused");
            using (var probe = new TcpClient()) { await probe.ConnectAsync(IPAddress.Loopback, port); Check(probe.Connected, "a plain TCP connection to the port succeeds while running"); }
            await s.StopAsync();
            Check(!s.IsRunning && s.LocalEndPoint == null, "server stopped");
            bool refused = false; try { using var p2 = new TcpClient(); await p2.ConnectAsync(IPAddress.Loopback, port); } catch (SocketException) { refused = true; }
            Check(refused, "after Stop the port no longer accepts connections (socket released)");
            var s2 = new NetworkServer(); s2.Start(IPAddress.Loopback, port);
            Check(s2.IsRunning && PortOf(s2) == port, "the same port can be re-used immediately after Stop");
            var s3 = new NetworkServer();
            string e = await ErrOf(async () => { s3.Start(IPAddress.Loopback, port); await Task.CompletedTask; });
            Check(e.Contains("already in use") && e.Contains(port.ToString()), "port already in use -> friendly NetworkException: \"" + e + "\"");
            await s2.StopAsync();
            Check((await ErrOf(async () => { new NetworkServer().Start(IPAddress.Loopback, -5); await Task.CompletedTask; })).Contains("out of range"), "invalid port refused");
            string e2 = await ErrOf(async () => { new NetworkServer().Start(IPAddress.Parse("203.0.113.77"), 5000); await Task.CompletedTask; });
            Check(e2.Contains("does not belong to this computer"), "binding to an IP that is not this computer -> friendly message: \"" + e2 + "\"");
            await s.StopAsync();
            Check(true, "stopping an already stopped server is harmless");
        });

        await Run("5. Client connect / disconnect (real IP + port)", async () =>
        {
            var s = NewServer(); var connected = new ConcurrentQueue<DeviceInfo>(); var gone = new ConcurrentQueue<(DeviceInfo, string)>();
            s.DeviceConnected += d => connected.Enqueue(d); s.DeviceDisconnected += (d, r) => gone.Enqueue((d, r));
            var c = await Join(s, "PC-2");
            Check(c.IsConnected && c.ServerName == "SRV" && c.DeviceId.Length > 0 && c.ConnectedAtUtc != null, "client connected, knows the server name and its device id");
            Check(await WaitFor(() => connected.Count == 1), "server raised DeviceConnected");
            connected.TryPeek(out var d);
            Check(d!.Name == "PC-2" && d.Ip == "127.0.0.1" && d.Port > 0 && d.Port != PortOf(s), $"server recorded the REAL remote endpoint {d.Ip}:{d.Port}");
            Check(s.GetDevices().Count == 1, "device appears in the server's list");
            var events = new ConcurrentQueue<(string, bool)>(); c.Disconnected += (r, req) => events.Enqueue((r, req));
            await c.DisconnectAsync();
            Check(!c.IsConnected && await WaitFor(() => events.Count == 1) && events.TryPeek(out var ev) && ev.Item2, "client: Disconnected raised once, marked as requested by the user");
            Check(await WaitFor(() => gone.Count == 1 && s.GetDevices().Count == 0), "server removed the device and raised DeviceDisconnected");
            await c.ConnectAsync(IPAddress.Loopback, PortOf(s), "PC-2", null);
            Check(c.IsConnected && await WaitFor(() => s.GetDevices().Count == 1), "the same client object can reconnect");
            Check((await ErrOf(() => c.ConnectAsync(IPAddress.Loopback, PortOf(s), "PC-9", null))).Contains("already connected"), "connecting while connected is refused");
            c.Dispose(); await s.StopAsync();
        });

        await Run("6. Registration rules: names, duplicates, access code, capacity", async () =>
        {
            var s = NewServer(x => { x.AccessCode = "s3cret"; x.MaxClients = 3; });
            string e1 = await ErrOf(async () => { using var c = new NetworkClient(); await c.ConnectAsync(IPAddress.Loopback, PortOf(s), "PC-1", "wrong"); });
            Check(e1.Contains("Incorrect access code"), "wrong access code refused: \"" + e1 + "\"");
            string e1b = await ErrOf(async () => { using var c = new NetworkClient(); await c.ConnectAsync(IPAddress.Loopback, PortOf(s), "PC-1", null); });
            Check(e1b.Contains("Incorrect access code"), "missing access code refused");
            var a = await Join(s, "PC-1", "s3cret");
            Check(a.IsConnected, "correct access code accepted");
            string dup = await ErrOf(async () => { using var c = new NetworkClient(); await c.ConnectAsync(IPAddress.Loopback, PortOf(s), "pc-1", "s3cret"); });
            Check(dup.Contains("already connected") && s.GetDevices().Count == 1, "duplicate device name refused (case-insensitive): \"" + dup + "\"");
            Check((await ErrOf(async () => { using var c = new NetworkClient(); await c.ConnectAsync(IPAddress.Loopback, PortOf(s), "SERVER", "s3cret"); })).Contains("reserved"), "reserved name refused");
            Check((await ErrOf(async () => { using var c = new NetworkClient(); await c.ConnectAsync(IPAddress.Loopback, PortOf(s), "bad|name", "s3cret"); })).Contains("Device name"), "illegal characters in the name refused");
            var b = await Join(s, "PC-2", "s3cret"); var c3 = await Join(s, "PC-3", "s3cret");
            string full = await ErrOf(async () => { using var c = new NetworkClient(); await c.ConnectAsync(IPAddress.Loopback, PortOf(s), "PC-4", "s3cret"); });
            Check(full.Contains("full") && s.GetDevices().Count == 3, "server full -> refused: \"" + full + "\"");
            foreach (var x in new[] { a, b, c3 }) x.Dispose(); await s.StopAsync();
        });

        await Run("7. Race: 10 clients register the SAME name at the same moment", async () =>
        {
            var s = NewServer(); var results = new ConcurrentBag<bool>(); var keep = new ConcurrentBag<NetworkClient>();
            await Task.WhenAll(Enumerable.Range(0, 10).Select(async _ =>
            {
                var c = new NetworkClient();
                try { await c.ConnectAsync(IPAddress.Loopback, PortOf(s), "Twin", null); results.Add(true); keep.Add(c); }
                catch (NetworkException) { results.Add(false); c.Dispose(); }
            }));
            Check(results.Count(r => r) == 1 && s.GetDevices().Count == 1, $"exactly ONE of 10 simultaneous registrations succeeded (successes: {results.Count(r => r)})");
            foreach (var c in keep) c.Dispose(); await s.StopAsync();
        });

        await Run("8. Many simultaneous clients (25) + device list", async () =>
        {
            var s = NewServer(); var clients = new ConcurrentBag<NetworkClient>();
            var sw = Stopwatch.StartNew();
            await Task.WhenAll(Enumerable.Range(1, 25).Select(async i => clients.Add(await Join(s, $"PC-{i}"))));
            Check(clients.All(c => c.IsConnected) && s.GetDevices().Count == 25, $"25 clients connected simultaneously in {sw.ElapsedMilliseconds} ms; server lists {s.GetDevices().Count}");
            Check(s.GetDevices().Select(d => d.Name).Distinct().Count() == 25 && s.GetDevices().Select(d => d.Port).Distinct().Count() == 25, "25 unique names and 25 distinct remote ports");
            bool listOk = await WaitFor(() => clients.All(c => c.Devices.Count == 24), 6000);
            Check(listOk, "every client receives the device list (24 other devices)" + (listOk ? "" : "  [counts seen: " + string.Join(",", clients.Select(c => c.Devices.Count).GroupBy(x => x).Select(g => $"{g.Key}x{g.Count()}")) + "]"));
            var first = clients.First(); await first.DisconnectAsync();
            Check(await WaitFor(() => s.GetDevices().Count == 24 && clients.Where(c => c != first).All(c => c.Devices.Count == 23), 6000), "when one leaves, the others' lists update automatically");
            foreach (var c in clients) c.Dispose(); await s.StopAsync();
        });

        await Run("9. Messaging: chat to server, relay to a device, broadcast, spoofing", async () =>
        {
            var s = NewServer(); var rx = new ConcurrentQueue<NetMessage>(); s.MessageReceived += m => rx.Enqueue(m);
            var c1 = await Join(s, "PC-1"); var c2 = await Join(s, "PC-2"); var c3 = await Join(s, "PC-3");
            var got1 = new ConcurrentQueue<NetMessage>(); var got2 = new ConcurrentQueue<NetMessage>(); var got3 = new ConcurrentQueue<NetMessage>();
            c1.MessageReceived += got1.Enqueue; c2.MessageReceived += got2.Enqueue; c3.MessageReceived += got3.Enqueue;

            var r = await c2.RequestAsync(NetMessage.Create(MessageTypes.Chat, "PC-2", "hello server", "SERVER"));
            Check(r.Succeeded() && await WaitFor(() => rx.Count == 1) && rx.TryPeek(out var m) && m.Text == "hello server" && m.Sender == "PC-2" && m.SenderIp == "127.0.0.1", "CHAT to the server: delivered, with verified sender name + IP");
            while (rx.TryDequeue(out _)) { }

            r = await c2.RequestAsync(NetMessage.Create(MessageTypes.Chat, "PC-2", "psst", "pc-3"));
            Check(r.Succeeded() && await WaitFor(() => got3.Count == 1) && got3.TryPeek(out var d) && d.Text == "psst" && d.Sender == "PC-2" && d.Type == "CHAT" && got1.IsEmpty, "private CHAT is relayed to the named device only");
            r = await c2.RequestAsync(NetMessage.Create(MessageTypes.Chat, "PC-2", "x", "Nobody"));
            Check(!r.Succeeded() && r.Text.Contains("No connected device"), "chat to an unknown device -> clear error: \"" + r.Text + "\"");

            while (got3.TryDequeue(out _)) { }
            r = await c2.RequestAsync(NetMessage.Create(MessageTypes.Broadcast, "PC-2", "to everyone"));
            Check(r.Succeeded() && r.Get("delivered") == "2" && await WaitFor(() => got1.Count == 1 && got3.Count == 1), "BROADCAST from PC-2 reaches the two OTHER clients (delivered=2)");
            Check(got1.TryPeek(out var b1) && b1.Sender == "PC-2" && b1.SenderIp == "127.0.0.1" && got3.TryPeek(out var b3) && b3.Sender == "PC-2" && b3.Text == "to everyone", "...and they see the REAL sender (PC-2 / 127.0.0.1), not the server's name");
            Check(await WaitFor(() => rx.Count >= 1) && rx.Last().Type == "BROADCAST" && rx.Last().Sender == "PC-2" && rx.Last().SenderIp == "127.0.0.1", "...and so does the server's own copy of the broadcast");
            Check(got2.IsEmpty && await WaitFor(() => rx.Count == 1), "the sender does not get its own broadcast back; the server sees it");
            got1.Clear(); got3.Clear(); got2.Clear();
            int n = await s.BroadcastAsync(NetMessage.Create(MessageTypes.Broadcast, "", "from the server"));
            Check(n == 3 && await WaitFor(() => got1.Count == 1 && got2.Count == 1 && got3.Count == 1) && got1.TryPeek(out var sm) && sm.Sender == "SRV", "server BROADCAST reaches all 3 clients and is stamped with the server name");
            Check(await s.SendToDeviceAsync("PC-1", NetMessage.Create(MessageTypes.Chat, "", "direct")) && !await s.SendToDeviceAsync("ghost", NetMessage.Create(MessageTypes.Chat, "", "x")), "server -> one device works; unknown device returns false");

            using var raw = new Raw(); await raw.ConnectAsync(PortOf(s)); var hs = await raw.HandshakeAsync("Mallory");
            var forged = NetMessage.Create(MessageTypes.Chat, "ADMIN", "forged"); forged.SenderIp = "1.2.3.4"; forged.Target = "SERVER";
            rx.Clear(); await raw.SendAsync(forged);
            Check(hs!.Succeeded() && await WaitFor(() => rx.Count == 1) && rx.TryPeek(out var f) && f.Sender == "Mallory" && f.SenderIp == "127.0.0.1", "a client cannot forge the sender name or IP (the server overwrites them)");
            foreach (var c in new[] { c1, c2, c3 }) c.Dispose(); await s.StopAsync();
        });

        await Run("10. Inventory requests are executed by the SERVER's gateway", async () =>
        {
            var gw = new FakeGateway(); var s = NewServer(x => x.Inventory = gw); var c = await Join(s, "PC-2");
            NetMessage Req(string action, string sku, string delta = "", string reason = "r") => NetMessage.Create(MessageTypes.InventoryUpdate, "PC-2", data: new() { ["action"] = action, ["sku"] = sku, ["delta"] = delta, ["reason"] = reason });
            var r = await c.RequestAsync(Req("ADJUST", "EL-001", "-3", "sold"));
            Check(r.Succeeded() && r.Get("quantity") == "39" && r.Text.Contains("PC-2") && gw.Requests.TryPeek(out var q) && q.Delta == -3 && q.Sku == "EL-001", "ADJUST: server applied it via the gateway and answered with the new quantity");
            r = await c.RequestAsync(Req("GET", "EL-001"));
            Check(r.Succeeded() && r.Get("quantity") == "42" && r.Get("name") == "Widget", "GET returns the server's stock data");
            int before = gw.Requests.Count;
            foreach (var bad in new[] { Req("ADJUST", "EL-001", "abc"), Req("ADJUST", "EL-001", "0"), Req("ADJUST", "EL-001", "999999"), Req("ADJUST", "", "1"), Req("DROP", "EL-001", "1"), Req("ADJUST", new string('x', 40), "1") })
            { var br = await c.RequestAsync(bad); if (br.Succeeded()) Check(false, "invalid request accepted!"); }
            Check(gw.Requests.Count == before, "6 invalid requests were rejected BEFORE reaching the database layer");
            gw.Throw = true; r = await c.RequestAsync(Req("ADJUST", "EL-001", "1"));
            Check(!r.Succeeded() && !r.Text.Contains("secret") && c.IsConnected, "a failing gateway gives a generic error (no internals leaked) and the connection survives");
            s.Inventory = null; r = await c.RequestAsync(Req("GET", "EL-001"));
            Check(!r.Succeeded() && r.Text.Contains("not available"), "no gateway configured -> clear error");
            c.Dispose(); await s.StopAsync();
        });

        await Run("11. Unexpected disconnections", async () =>
        {
            var s = NewServer(); var gone = new ConcurrentQueue<(DeviceInfo, string)>(); s.DeviceDisconnected += (d, r) => gone.Enqueue((d, r));
            var observer = await Join(s, "Observer");
            using (var raw = new Raw()) { await raw.ConnectAsync(PortOf(s)); await raw.HandshakeAsync("Crash-1"); Check(await WaitFor(() => s.GetDevices().Count == 2), "raw device registered"); raw.Abort(); }
            Check(await WaitFor(() => s.GetDevices().Count == 1 && gone.Count == 1), "TCP reset (cable pulled / process killed) detected -> device removed");
            Check(await WaitFor(() => observer.Devices.Count == 0), "other clients were told the device left");
            using (var raw = new Raw()) { await raw.ConnectAsync(PortOf(s)); await raw.HandshakeAsync("Crash-2"); raw.Tcp.Close(); }
            Check(await WaitFor(() => gone.Count == 2 && s.GetDevices().Count == 1), "connection closed without DISCONNECT detected");
            using (var raw = new Raw()) { await raw.ConnectAsync(PortOf(s)); await raw.SendAsync(NetMessage.Create(MessageTypes.Connect, "x", data: new() { ["name"] = "Half" })); raw.Abort(); }
            Check(await WaitFor(() => s.GetDevices().Count == 1) && s.GetDevices().Count == 1, "client vanishing right after CONNECT leaves no ghost entry");
            observer.Dispose(); await s.StopAsync();
        });

        await Run("12. Heartbeat + timeouts", async () =>
        {
            var s = NewServer(x => { x.IdleTimeout = TimeSpan.FromMilliseconds(1200); x.HandshakeTimeout = TimeSpan.FromMilliseconds(800); });
            var gone = new ConcurrentQueue<(DeviceInfo, string)>(); s.DeviceDisconnected += (d, r) => gone.Enqueue((d, r));
            var alive = await Join(s, "Alive");     // heartbeat every 300 ms
            using var silent = new Raw(); await silent.ConnectAsync(PortOf(s)); await silent.HandshakeAsync("Silent");
            Check(await WaitFor(() => gone.Count == 1, 5000) && gone.TryPeek(out var g) && g.Item1.Name == "Silent" && g.Item2.Contains("heartbeat"), "silent device dropped after the idle timeout: \"" + (gone.TryPeek(out g) ? g.Item2 : "") + "\"");
            Check(alive.IsConnected && s.GetDevices().Count == 1, "a device that sends heartbeats stays connected well beyond the timeout");
            using var mute = new Raw(); await mute.ConnectAsync(PortOf(s));
            await Task.Delay(1800);
            bool closed = false; try { var t = mute.S.ReadAsync(new byte[1], 0, 1); closed = await Task.WhenAny(t, Task.Delay(1500)) == t && await t == 0; } catch (IOException) { closed = true; }
            Check(closed && s.GetDevices().Count == 1, "a connection that never sends CONNECT is closed after the handshake timeout");
            var fake = new TcpListener(IPAddress.Loopback, 0); fake.Start(); int fp = ((IPEndPoint)fake.LocalEndpoint).Port;
            var serve = Task.Run(async () => { using var t = await fake.AcceptTcpClientAsync(); var st = t.GetStream(); var hello = await FrameCodec.ReadAsync(st, default);
                await FrameCodec.WriteAsync(st, NetMessage.Response(hello, true, "ok", "FAKE", new() { ["serverName"] = "FAKE", ["deviceId"] = "z" }), default); await Task.Delay(6000); });
            var cl = new NetworkClient { HeartbeatInterval = TimeSpan.FromMilliseconds(200), IdleTimeout = TimeSpan.FromMilliseconds(1000) }; var ev = new ConcurrentQueue<(string, bool)>(); cl.Disconnected += (r, q) => ev.Enqueue((r, q));
            await cl.ConnectAsync(IPAddress.Loopback, fp, "Probe-1", null);
            Check(await WaitFor(() => ev.Count == 1, 5000) && ev.TryPeek(out var e) && !e.Item2 && e.Item1.Contains("timed out") && !cl.IsConnected, "client detects a server that stops answering: \"" + (ev.TryPeek(out e) ? e.Item1 : "") + "\"");
            cl.Dispose(); fake.Stop(); alive.Dispose(); await s.StopAsync();
        });

        await Run("13. Server stops while clients are connected", async () =>
        {
            var s = NewServer(); int port = PortOf(s); var cs = new List<NetworkClient>(); var ev = new ConcurrentQueue<(string, bool)>();
            for (int i = 0; i < 5; i++) { var c = await Join(s, $"PC-{i}"); c.Disconnected += (r, q) => ev.Enqueue((r, q)); cs.Add(c); }
            await s.StopAsync();
            Check(await WaitFor(() => ev.Count == 5), "all 5 clients were notified");
            Check(ev.All(e => !e.Item2 && e.Item1.Contains("stopped")) && cs.All(c => !c.IsConnected), "reason says the server was stopped; clients are disconnected  [reasons seen: " + string.Join(" | ", ev.Select(e => e.Item1 + (e.Item2 ? " (requested)" : "")).Distinct()) + "]");
            Check(s.GetDevices().Count == 0, "server registry is empty");
            var l = new TcpListener(IPAddress.Loopback, port); l.Start(); l.Stop(); Check(true, "the port is free again (all sockets were closed)");
            string e2 = await ErrOf(() => cs[0].ConnectAsync(IPAddress.Loopback, port, "PC-0", null));
            Check(e2.Contains("No server is listening"), "reconnecting to the stopped server -> friendly message: \"" + e2 + "\"");
            foreach (var c in cs) c.Dispose();
        });

        await Run("14. Hostile / broken peers cannot hurt the server", async () =>
        {
            var s = NewServer(); var good = await Join(s, "Good");
            using (var r1 = new Raw()) { await r1.ConnectAsync(PortOf(s)); await r1.S.WriteAsync(new byte[] { 0x00, 0x01, 0x86, 0xA0 }, 0, 4); /* announces 100000 bytes */
                Check(await r1.ClosedWithinAsync(), "oversized length header -> error reply, then the connection is dropped"); }
            using (var r2 = new Raw()) { await r2.ConnectAsync(PortOf(s)); var junk = new byte[500]; new Random(1).NextBytes(junk); try { await r2.S.WriteAsync(junk, 0, junk.Length); } catch { } await Task.Delay(300); Check(true, "random binary garbage sent"); }
            using (var r3 = new Raw()) { await r3.ConnectAsync(PortOf(s)); var j = System.Text.Encoding.UTF8.GetBytes("{this is not json}"); var f = new byte[4 + j.Length]; f[3] = (byte)j.Length; Array.Copy(j, 0, f, 4, j.Length); await r3.S.WriteAsync(f, 0, f.Length);
                var reply = await r3.ResponseAsync(); Check(reply != null && !reply.Succeeded() && reply.Text.Contains("Invalid message") && await r3.ClosedWithinAsync(), "invalid JSON -> error response, then closed"); }
            using (var r4 = new Raw()) { await r4.ConnectAsync(PortOf(s)); await r4.SendAsync(NetMessage.Create(MessageTypes.Chat, "x", "no connect first")); var reply = await r4.ResponseAsync(); Check(reply != null && !reply.Succeeded() && reply.Text.Contains("CONNECT"), "first message must be CONNECT"); }
            using (var r5 = new Raw()) { await r5.ConnectAsync(PortOf(s)); await r5.HandshakeAsync("Sneaky"); await r5.SendAsync(NetMessage.Create(MessageTypes.LowStockAlert, "x", "fake alert")); var reply = await r5.ResponseAsync(); Check(reply != null && !reply.Succeeded() && reply.Text.Contains("not accepted"), "clients cannot inject LOW_STOCK_ALERT / SERVER_RESPONSE etc."); }
            using (var r6 = new Raw()) { await r6.ConnectAsync(PortOf(s)); await r6.HandshakeAsync("Big"); var big = NetMessage.Create(MessageTypes.Chat, "Big", new string('z', 5000), "SERVER"); await r6.SendAsync(big);
                var reply = await r6.ResponseAsync(); Check(reply != null && !reply.Succeeded() && reply.Text.Contains("too long") && await r6.ClosedWithinAsync(), "message text over the limit -> error \"" + reply?.Text + "\" and the connection is closed"); }
            Check(good.IsConnected && await WaitFor(() => s.GetDevices().Count == 1, 3000), "the well-behaved client was unaffected and no ghost devices remain");
            var r7 = await good.RequestAsync(NetMessage.Create(MessageTypes.Chat, "Good", "still works", "SERVER")); Check(r7.Succeeded(), "the server still serves requests");
            good.Dispose(); await s.StopAsync();
        });

        await Run("15. TCP fragmentation on the SERVER side (partial writes, coalesced frames)", async () =>
        {
            var s = NewServer(); var rx = new ConcurrentQueue<NetMessage>(); s.MessageReceived += rx.Enqueue;
            using var raw = new Raw(); await raw.ConnectAsync(PortOf(s));
            var hello = FrameCodec.Encode(NetMessage.Create(MessageTypes.Connect, "Slow", data: new() { ["name"] = "Slow" }));
            await raw.S.WriteAsync(hello, 0, 2); await raw.S.FlushAsync(); await Task.Delay(150);          // half of the header
            await raw.S.WriteAsync(hello, 2, 10); await raw.S.FlushAsync(); await Task.Delay(150);         // header + start of the body
            await raw.S.WriteAsync(hello, 12, hello.Length - 12); await raw.S.FlushAsync();
            var resp = await raw.ResponseAsync(); Check(resp != null && resp.Succeeded(), "CONNECT delivered in 3 pieces is reassembled and accepted");
            var f1 = FrameCodec.Encode(NetMessage.Create(MessageTypes.Chat, "Slow", "one", "SERVER")); var f2 = FrameCodec.Encode(NetMessage.Create(MessageTypes.Chat, "Slow", "two", "SERVER")); var f3 = FrameCodec.Encode(NetMessage.Create(MessageTypes.Chat, "Slow", "three", "SERVER"));
            var all = f1.Concat(f2).Concat(f3).ToArray(); await raw.S.WriteAsync(all, 0, all.Length);
            Check(await WaitFor(() => rx.Count == 3) && string.Join(",", rx.Select(m => m.Text)) == "one,two,three", "three frames glued into ONE write are processed as three messages, in order");
            rx.Clear(); var f4 = FrameCodec.Encode(NetMessage.Create(MessageTypes.Chat, "Slow", "split", "SERVER")); var tail = FrameCodec.Encode(NetMessage.Create(MessageTypes.Chat, "Slow", "next", "SERVER"));
            await raw.S.WriteAsync(f4.Concat(tail.Take(3)).ToArray(), 0, f4.Length + 3); await Task.Delay(100); await raw.S.WriteAsync(tail, 3, tail.Length - 3);
            Check(await WaitFor(() => rx.Count == 2) && string.Join(",", rx.Select(m => m.Text)) == "split,next", "a frame cut in the middle of the NEXT frame's header is handled");
            await s.StopAsync();
        });

        await Run("16. Stress: 30 clients x 60 messages concurrently + broadcasts", async () =>
        {
            var s = NewServer(); var perClient = new ConcurrentDictionary<string, List<int>>(); long total = 0;
            s.MessageReceived += m => { if (m.Type == MessageTypes.Chat) { Interlocked.Increment(ref total); lock (perClient) perClient.GetOrAdd(m.Sender, _ => new()).Add(int.Parse(m.Text)); } };
            var clients = new List<NetworkClient>(); for (int i = 0; i < 30; i++) clients.Add(await Join(s, $"C{i:00}"));
            long bcastRx = 0; foreach (var c in clients) c.MessageReceived += m => { if (m.Type == MessageTypes.Broadcast) Interlocked.Increment(ref bcastRx); };
            var sw = Stopwatch.StartNew();
            await Task.WhenAll(clients.Select(async c => { for (int k = 0; k < 60; k++) await c.SendAsync(NetMessage.Create(MessageTypes.Chat, c.DeviceName, k.ToString(), "SERVER")); }));
            Check(await WaitFor(() => Interlocked.Read(ref total) == 1800, 15000), $"server received all 1800 messages ({sw.ElapsedMilliseconds} ms)");
            bool ordered = perClient.Count == 30 && perClient.Values.All(l => l.SequenceEqual(Enumerable.Range(0, 60)));
            Check(ordered, "every client's 60 messages arrived complete, uncorrupted and in order");
            for (int i = 0; i < 20; i++) await s.BroadcastAsync(NetMessage.Create(MessageTypes.Broadcast, "", "b" + i));
            Check(await WaitFor(() => Interlocked.Read(ref bcastRx) == 600, 10000), "20 broadcasts x 30 clients = 600 deliveries");
            Check(clients.All(c => c.IsConnected), "all clients still connected after the storm");
            foreach (var c in clients) c.Dispose(); await s.StopAsync();
        });

        await Run("17. 'Test connection' probe", async () =>
        {
            var s = NewServer(); int port = PortOf(s);
            var ok = await NetworkClient.ProbeAsync(IPAddress.Loopback, port, TimeSpan.FromSeconds(3));
            Check(ok.Ok && ok.ServerName == "SRV" && ok.RoundTripMs >= 1, "probe of a running server: " + ok.Message);
            Check(s.GetDevices().Count == 0, "the probe does NOT register a device");
            var c = await Join(s, "X"); var ok2 = await NetworkClient.ProbeAsync(IPAddress.Loopback, port, TimeSpan.FromSeconds(3)); Check(ok2.Ok && ok2.DeviceCount == 1, "probe reports the number of connected devices (1)"); c.Dispose();
            await s.StopAsync();
            var refused = await NetworkClient.ProbeAsync(IPAddress.Loopback, port, TimeSpan.FromSeconds(3)); Check(!refused.Ok && refused.Message.Contains("No server is listening"), "probe of a closed port: " + refused.Message);
            var dead = new TcpListener(IPAddress.Loopback, 0); dead.Start(); int dp = ((IPEndPoint)dead.LocalEndpoint).Port;
            var silent = await NetworkClient.ProbeAsync(IPAddress.Loopback, dp, TimeSpan.FromMilliseconds(800)); Check(!silent.Ok && silent.Message.Length > 10, "probe of something that accepts but never answers: " + silent.Message); dead.Stop();
            var garbage = new TcpListener(IPAddress.Loopback, 0); garbage.Start(); int gp = ((IPEndPoint)garbage.LocalEndpoint).Port;
            _ = Task.Run(async () => { using var t = await garbage.AcceptTcpClientAsync(); await t.GetStream().WriteAsync(System.Text.Encoding.ASCII.GetBytes("HTTP/1.1 400 Bad Request\r\n\r\n")); await Task.Delay(500); });
            var notUs = await NetworkClient.ProbeAsync(IPAddress.Loopback, gp, TimeSpan.FromSeconds(2)); Check(!notUs.Ok, "probe of a non-Stockora service (HTTP) is refused: " + notUs.Message); garbage.Stop();
        });

        await Run("18. LAN-only filter, connect errors", async () =>
        {
            var s = NewServer(x => x.AddressFilter = ip => false); var log = new ConcurrentQueue<string>(); s.Log += log.Enqueue;
            string e = await ErrOf(async () => { using var c = new NetworkClient(); await c.ConnectAsync(IPAddress.Loopback, PortOf(s), "PC-1", null); });
            Check(e != "<no error>" && !e.StartsWith("WRONG") && s.GetDevices().Count == 0 && log.Any(l => l.Contains("not a private LAN address")), "server rejects non-LAN peers and logs why; client gets: \"" + e + "\"");
            await s.StopAsync();
            Check((await ErrOf(async () => { using var c = new NetworkClient(); await c.ConnectAsync(IPAddress.Loopback, 0, "PC-1", null); })).Contains("out of range"), "port 0 refused by the client");
            Check((await ErrOf(async () => { using var c = new NetworkClient(); await c.ConnectAsync(IPAddress.Loopback, 70000, "PC-1", null); })).Contains("out of range"), "port 70000 refused by the client");
            Check((await ErrOf(async () => { using var c = new NetworkClient(); await c.ConnectAsync(IPAddress.Loopback, 5000, "bad name!", null); })).Contains("Device name"), "invalid device name refused by the client");
            var sw = Stopwatch.StartNew(); string t = await ErrOf(async () => { using var c = new NetworkClient { ConnectTimeout = TimeSpan.FromMilliseconds(1200) }; await c.ConnectAsync(IPAddress.Parse("10.255.255.1"), 5000, "PC-1", null); });
            Check(t.Contains("10.255.255.1") && sw.ElapsedMilliseconds < 4000, $"unreachable computer -> friendly error within the timeout ({sw.ElapsedMilliseconds} ms): \"{t}\"");
            var c0 = new NetworkClient(); Check((await ErrOf(() => c0.SendAsync(NetMessage.Create(MessageTypes.Chat, "x", "y")))).Contains("Not connected"), "sending while disconnected -> clear error"); c0.Dispose();
        });

        await Run("19. Resource cleanup: 25 start/stop cycles with clients", async () =>
        {
            int threadsBefore = Process.GetCurrentProcess().Threads.Count; long memBefore = GC.GetTotalMemory(true);
            for (int i = 0; i < 25; i++)
            {
                var s = NewServer(); var cs = new List<NetworkClient>(); for (int k = 0; k < 4; k++) cs.Add(await Join(s, $"P{k}"));
                if (i % 2 == 0) { foreach (var c in cs) c.Dispose(); await s.StopAsync(); } else { await s.StopAsync(); foreach (var c in cs) c.Dispose(); }
            }
            await Task.Delay(500); GC.Collect(); GC.WaitForPendingFinalizers(); int threadsAfter = Process.GetCurrentProcess().Threads.Count; long memAfter = GC.GetTotalMemory(true);
            Check(threadsAfter - threadsBefore < 12, $"no thread leak (threads {threadsBefore} -> {threadsAfter})");
            Check(memAfter - memBefore < 20_000_000, $"no memory leak (managed heap {memBefore / 1024} KB -> {memAfter / 1024} KB)");
            var last = NewServer(); using (var s2 = last) { Check(s2.IsRunning, "Dispose() on a running server stops it"); }
            Check(!last.IsRunning, "server stopped by Dispose()");
        });

        Console.WriteLine($"\nRESULT: {pass} passed, {fail} failed");
        return fail == 0 ? 0 : 1;
    }
}
