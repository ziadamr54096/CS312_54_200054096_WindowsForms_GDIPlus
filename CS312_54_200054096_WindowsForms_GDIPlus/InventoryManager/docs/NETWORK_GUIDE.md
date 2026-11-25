# Stockora - LAN Network Communication (TCP/IP)

This guide explains the new **Network** tab: architecture, how to run it on two or more computers, how to find the IP address,
Windows Firewall settings, a demonstration script for your professor, and troubleshooting.

---------------------------------------------------------------------------------------------------

## 1. What it does

* One computer is the **Server** (click *Start server*). Other computers are **Clients** (type the server's IP + port, click *Connect*).
* Everything uses **real TCP sockets** (`TcpListener`, `TcpClient`, `NetworkStream`) with `async/await`.
* The server keeps a live list of connected devices (name, IP, port, status, connection time) - clients see it too.
* Devices exchange **messages** (to one device, to the server, or to everyone) and get **pop-up notifications**.
* The server broadcasts **stock updates** and **low-stock / out-of-stock alerts** to all clients. A client can also ask the
  server to read or change the stock of a product (the *server* executes it on *its* database).
* The SQLite database file is **never shared**. Only small JSON messages travel over the network.

## 2. Architecture

```
  COMPUTER 1 (Server)                                   COMPUTER 2 / 3 (Clients)
 +------------------------------------+               +-------------------------------+
 |  Network tab (NetworkPanel)        |               |  Network tab (NetworkPanel)   |
 |        |                           |               |        |                      |
 |  NetworkController  (UI thread)    |               |  NetworkController            |
 |    |            |                  |   TCP :5000   |    |                          |
 |  NetworkServer  InventoryWatcher   | <-----------> |  NetworkClient                |
 |    |            DbInventoryGateway |  JSON frames  |   (receive loop + heartbeat)  |
 |  Db.cs  ->  inventory.db (SQLite)  |               |  (its own local database is   |
 +------------------------------------+               |   NOT used by the network)    |
                                                      +-------------------------------+
```

| Layer | Files (all new, folder `Network/`) | Responsibility |
|-------|-----------------------------------|----------------|
| Protocol | `Core/NetMessage.cs`, `Core/NetJson.cs`, `Core/FrameCodec.cs` | message format, validation, **framing** |
| Server | `Core/NetworkServer.cs`, `Core/ClientConnection.cs` | listener, many clients at once, registry, relay, watchdog |
| Client | `Core/NetworkClient.cs` | connect, receive loop, heartbeat, requests, "Test connection" |
| Helpers | `Core/NetUtil.cs`, `Core/DeviceInfo.cs`, `Core/InventoryContracts.cs`, `Core/NetworkException.cs` | IP/port validation, LAN detection, friendly errors |
| Application glue | `DbInventoryGateway.cs`, `InventoryWatcher.cs`, `NetworkController.cs` | database access on the UI thread, stock-change detection, orchestration |
| UI | `NetworkPanel(.Designer).cs`, `NotificationToast(.Designer).cs` | the Network tab and the pop-up (both editable in the Designer) |

`Network/Core/` has **no WinForms and no database code**, so it is unit-testable on its own.

### Message framing (important)
TCP is a byte *stream*: one read can return half a message or three messages. Every message is therefore sent as

```
[ 4 bytes: length of the JSON, big-endian ][ JSON, UTF-8 ]
```

and the receiver loops until it has exactly that many bytes. Frames above 64 KB, zero/negative lengths, invalid JSON,
unknown message types and over-long fields are rejected and the connection is closed - the server stays up.

### Message types

| Type | Direction | Meaning |
|------|-----------|---------|
| `CONNECT` | client -> server | register this device (name, optional access code, signed-in user) |
| `DISCONNECT` | both | polite goodbye (the server also uses it to say "the server was stopped") |
| `CHAT` | any | private message: to the server (`Target = SERVER`) or to one device (`Target = <name>`) |
| `BROADCAST` | any | message to every device (a client's broadcast is relayed by the server) |
| `INVENTORY_UPDATE` | client -> server | *request*: `GET` or `ADJUST` the stock of a SKU. server -> clients: *event* "stock changed" |
| `LOW_STOCK_ALERT` | server -> clients | a product reached its reorder level (`LOW`) or ran out (`OUT`) |
| `SERVER_RESPONSE` | server -> client | answer to a request (`ReplyTo` = request id, `ok` = true/false) |
| `PING` / `PONG` | both | heartbeat every 10 s and the "Test connection" probe |
| `DEVICE_LIST` | server -> clients | the current list of devices (sequence-numbered so a late list never overwrites a newer one) |

Example (formatted):
`{"V":1,"Type":"CHAT","Id":"...","Sender":"PC-2","SenderIp":"192.168.1.20","Target":"SERVER","Text":"hello","Timestamp":"2026-..Z"}`

### Safety rules built in
* **LAN only:** the server rejects connections from non-private addresses (10.x, 172.16-31.x, 192.168.x, 169.254.x, loopback)
  unless you tick *Allow non-private addresses*. Never forward the port on your router.
* **Verified sender:** the server overwrites sender name and IP with the real values; a client cannot impersonate another device.
* **No duplicates:** a device name can be registered once (case-insensitive), even if 10 clients try at the same moment.
* **Optional access code** (server and client fields). *Note: traffic is plain TCP - the code is not encrypted. Use trusted networks.*
* **Limits:** 50 devices, 64 KB frames, 4000-character texts, 5 s handshake timeout, 35 s heartbeat timeout, 5 s send timeout
  (a slow device cannot block the others).
* **Unexpected disconnections** (cable pulled, crash, Wi-Fi lost) are detected by the heartbeat/TCP and the device disappears from the lists.
* **Threading:** all socket work is `async`; every event reaches the UI through the UI `SynchronizationContext`.
  Database writes caused by network requests are executed **on the UI thread** because the existing screens refresh themselves
  from `Db.DataChanged` and touch controls directly.
* **Clean-up:** stopping the server (or logging out / closing the window) sends a goodbye, closes every socket and frees the port.

## 3. Using the Network tab

### Server computer (e.g. 192.168.1.10)
1. Open the **Network** tab. Under **A. Server Configuration** choose the IP (leave *All network adapters*), keep port **5000**.
   (Optional: type an access code.)
2. Click **Start server**. Status turns green: *Running*, *Listening on 0.0.0.0:5000*, and
   *Clients connect to: 192.168.1.10:5000* - this is what you tell the other computers.
3. Connected clients appear in **C. Connected Devices** automatically.

### Client computers
1. Open the **Network** tab. Under **B. Client Connection** type the **Server IP address** (e.g. `192.168.1.10`) and the **port** (5000).
   Keep or change the **device name** (must be unique).
2. (Optional) **Test connection** - checks that the server answers and shows the round-trip time; it does not register the device.
3. Click **Connect**. Status: *Connected to <server>*, and the connection time counts up.

### Messaging and notifications (section D)
* Pick the recipient - *All devices (broadcast)*, *Server only*, or a specific device - type a message, press **Enter** or **Send**.
* Incoming messages appear in the log (time, direction, type, sender name + IP) **and** as a pop-up at the bottom-right of the screen.
  Untick *Show pop-up notifications* to silence pop-ups (the log keeps working).
* Stock events: when stock changes on the server (from the server's Stock Movements/Products tabs, or a client's request) every client gets an
  *INVENTORY_UPDATE* notification; reaching the reorder level gives a *LOW_STOCK_ALERT*; reaching 0 gives *Out of stock*.

### Inventory over the network (client -> server)
Enter a SKU (for example `EL-001`) then **Check stock on server** (read only) or set *Change (+/-)* and **Apply change on server**.
The server validates the request, writes the change and a `StockMovements` row ("Network (PC-2): reason") to **its** database, and tells everybody.

### Good to know
* A computer is **either** the server **or** a client at a time.
* The server's data is the "network inventory". A client's other tabs keep using that client's own local database as before.
* Logging out (or closing the app) **stops the server** and disconnects its clients.

## 4. Finding the server's IP address

* **In the app:** the Network tab shows *This computer* with the IPv4 address(es) (and *Clients connect to ...* once the server runs).
* **Windows 10/11:** `Win + R` -> `cmd` -> `ipconfig` -> look for **IPv4 Address** under *Wireless LAN adapter Wi-Fi* (or *Ethernet adapter*),
  e.g. `192.168.1.10`. Or: Settings -> Network & Internet -> Wi-Fi -> *Hardware properties*.
* **PowerShell:** `Get-NetIPAddress -AddressFamily IPv4 | Where-Object {$_.IPAddress -notlike '169.*' -and $_.IPAddress -ne '127.0.0.1'}`
* Both computers must be on the **same network** (same Wi-Fi, or same switch/router). The first three numbers usually match (192.168.1.x).

## 5. Windows Firewall

Windows blocks incoming connections by default - **only the server computer** needs a rule.

1. **First start:** when you click *Start server*, Windows may ask *"Allow Inventory Manager to communicate on these networks?"*
   -> tick **Private networks** (and Public only if your network is marked Public) -> **Allow access**.
   If you clicked Cancel, create the rule manually:
2. **Command (run Command Prompt as Administrator):**
   ```
   netsh advfirewall firewall add rule name="Stockora Server 5000" dir=in action=allow protocol=TCP localport=5000 profile=private
   ```
   (add `profile=private,public` if the Wi-Fi is classified as Public). Remove it later with
   `netsh advfirewall firewall delete rule name="Stockora Server 5000"`.
3. **GUI:** Windows Security -> Firewall & network protection -> Advanced settings -> Inbound Rules -> New Rule -> Port -> TCP, 5000 -> Allow -> Private.
4. Make sure the Wi-Fi network profile is **Private** (Settings -> Network & Internet -> Wi-Fi -> your network -> *Private*).
5. Test from the client's PowerShell: `Test-NetConnection 192.168.1.10 -Port 5000` -> `TcpTestSucceeded : True`.

Not working although the firewall is open? Some Wi-Fi networks (guest networks, university/public hotspots) enable **client isolation** so devices
cannot see each other. Use a phone hotspot or a home router for the demo.

## 6. Test plan for two computers

| # | Do this | Expect |
|---|---------|--------|
| 1 | PC1: Network tab -> **Start server** (port 5000) | Status *Running*; IP shown |
| 2 | PC2: enter PC1's IP + 5000 -> **Test connection** | green "Server '...' answered in N ms" |
| 3 | PC2: **Connect** | *Connected to PC1*; PC1's device list shows PC2 (name, IP, port, time) + pop-up on PC1 |
| 4 | PC3: **Connect** (different device name) | PC1 shows 2 devices; PC2 and PC3 see each other in their lists |
| 5 | PC2: recipient *All devices*, type "Hello" -> **Send** | PC1 log + pop-up; PC3 log + pop-up; PC2 gets no copy |
| 6 | PC1: recipient *All devices*, send "Meeting at 10" | PC2 and PC3 pop-ups |
| 7 | PC2: recipient *PC3*, send a private message | only PC3 receives it |
| 8 | PC2: SKU `EL-001`, change `-5` -> **Apply change on server** | PC1's stock drops by 5 (Stock Movements tab shows "Network (PC2): ..."); PC2 and PC3 pop-up *Stock update* |
| 9 | PC2: SKU `EL-004` (sample data: 45 in stock, reorder level 15) change `-31`, then `-14` | first a *Low stock alert*, then *Out of stock* pop-ups on PC2/PC3 (use one of your own products if your data differs) |
| 10 | On PC1 sell stock in the **Stock Movements** tab | clients get the stock notification too |
| 11 | Unplug PC3's Wi-Fi (or kill the app) | within ~35 s PC1 removes PC3; PC2's list updates |
| 12 | Try an invalid IP (`300.1.1.1`), a closed port, a port in use | friendly error messages, nothing crashes |
| 13 | PC1: **Stop server** | PC2/PC3 show *Disconnected: The server was stopped.* |

## 7. Demonstration script for the professor (about 8 minutes)

1. **Explain (1 min):** "TCP client-server over the LAN. One computer owns the database and acts as the server; the others send small JSON messages.
   Messages are length-prefixed because TCP is a byte stream. The database file is never shared."
2. **Server (1 min):** on PC1 open *Network*, point at the local IPv4 address, click **Start server**. Show *Running*, port 5000.
3. **Clients (1 min):** on PC2 type PC1's IP + 5000, click **Test connection** (round-trip time), then **Connect**. Repeat on PC3.
   Show PC1's **Connected Devices** list filling up live (name, IP, port, time) and the pop-ups.
4. **Messaging (2 min):** PC2 broadcasts "Hello" - PC1 and PC3 react immediately. PC1 broadcasts. PC2 sends a private message to PC3.
   Point at the communication log (time, direction, sender + IP).
5. **Inventory (2 min):** PC2 *Check stock* for `EL-001`, then *Apply change* -3. Switch to PC1's **Stock Movements** tab to show the new "Network (PC2)" row,
   while PC3 shows the *Stock update* pop-up. Reduce a product to its reorder level -> *Low stock alert*.
6. **Robustness (1 min):** disconnect PC3's Wi-Fi: PC1 removes it. Type a wrong IP: friendly error. Stop the server: clients are told why.
7. **Wrap-up:** mention LAN-only protection, verified sender, duplicate-name protection, async I/O, heartbeats, clean shutdown.

Tip: do a full dry run beforehand, and have the netsh firewall command and `ipconfig` ready.

## 8. Troubleshooting

| Message / symptom | Cause and fix |
|-------------------|---------------|
| *No server is listening at IP:port* | server not started, wrong port, or the firewall blocks it -> see section 5 |
| *IP:port did not answer in time* | wrong IP, different networks, client isolation, or firewall dropping packets |
| *Port 5000 is already in use* | another program (or a second copy of the app) uses it -> choose another port (e.g. 5001) on **both** computers |
| *The selected IP address does not belong to this computer* | choose *All network adapters* or an address from the list |
| *A device named 'X' is already connected* | change the **Device name** |
| *Incorrect access code* | type the same code the server uses (or clear it) |
| *Connect* is greyed out | this computer is running the server - stop it first (one role at a time) |
| Server list shows 127.0.0.1 | the client is on the same computer (fine for testing) |

## 9. What was verified (and what was not)

Verified automatically (no Windows machine was available while building this):
* **147 checks** on the networking core, compiled and run on the real **.NET 8** runtime with real sockets: framing (1-byte reads, glued frames, hostile input),
  validation, start/stop/restart, port-in-use, 25 simultaneous clients, duplicate-registration race, messaging/relay/broadcast/spoofing,
  inventory requests, abrupt disconnects, heartbeat timeouts, server stop, 30 clients x 60 messages stress, resource-leak cycles.
  Several real concurrency bugs were found and fixed through these tests (late device lists, lost 'server stopped' notice, push overtaking the welcome, relayed broadcasts re-stamped as the server). The final version passed 20 consecutive runs (4 of them pinned to a single CPU core).
* **78 checks** on the application layer against a real SQLite database (stock-change detection, low-stock logic, database writes always on the UI thread,
  one server + two clients end to end, events always on the UI thread).
* **56 checks** driving the real Network tab and pop-up in WinForms (three tabs talking over TCP), plus 49 + 62 + 39 + 13 regression checks of the existing screens and the login system.

Not verified here - please do these on your computers:
* A build with the real .NET 8 SDK on Windows and **opening the files in Visual Studio's Designer**.
* A run on **two or more real computers** (Windows Firewall behaviour and your Wi-Fi are outside what could be tested).
* Windows-specific look of the pop-up (it uses the no-activate window style).

### Opening the Designer
Build the project first, then double-click `Network/NetworkPanel.cs` (or right-click -> *View Designer*), `Network/NotificationToast.cs`, and `MainForm.cs`
(the new tab *Network* is the last one). Do not put code inside `InitializeComponent()`.
