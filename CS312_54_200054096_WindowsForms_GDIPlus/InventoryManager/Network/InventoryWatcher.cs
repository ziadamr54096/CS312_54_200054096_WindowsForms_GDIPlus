using System.Data;

namespace InventoryManager.Network;

/// <summary>
/// Server side: notices stock changes made by ANY part of the existing application (Stock Movements tab, Products tab, remote requests)
/// by comparing the Products table before and after every Db.DataChanged event. No existing screen had to be modified.
/// Raises Changes with the INVENTORY_UPDATE / LOW_STOCK_ALERT payloads to broadcast.
/// </summary>
public sealed class InventoryWatcher : IDisposable
{
    sealed record State(long Id, string Sku, string Name, int Quantity, int ReorderLevel, string Supplier);

    readonly object _lock = new();
    readonly Dictionary<long, (string By, string Reason)> _attribution = new();
    Dictionary<long, State> _snapshot = new();
    bool _started;

    /// <summary>Raised on the thread that wrote to the database (the UI thread).</summary>
    public event Action<IReadOnlyList<InventoryChange>, IReadOnlyList<LowStockInfo>>? Changes;

    public void Start()
    {
        lock (_lock)
        {
            if (_started) return;
            _snapshot = Read();
            _started = true;
        }
        Db.DataChanged += OnDbChanged;
    }

    public void Stop()
    {
        Db.DataChanged -= OnDbChanged;
        lock (_lock) { _started = false; _attribution.Clear(); }
    }

    public void Dispose() => Stop();

    /// <summary>Called by DbInventoryGateway just before it writes, so the broadcast can say WHO changed the stock and why.</summary>
    public void Attribute(long productId, string by, string reason)
    {
        lock (_lock) _attribution[productId] = (by, reason);
    }

    void OnDbChanged()
    {
        try { Check(); } catch (Exception) { /* a notification problem must never break a database write */ }
    }

    void Check()
    {
        var now = Read();
        var updates = new List<InventoryChange>();
        var lows = new List<LowStockInfo>();
        lock (_lock)
        {
            if (!_started) return;
            foreach (var cur in now.Values)
            {
                if (!_snapshot.TryGetValue(cur.Id, out var old))
                {
                    if (cur.Quantity != 0) updates.Add(new InventoryChange(cur.Sku, cur.Name, 0, cur.Quantity, "Product added", ""));
                    if (Status(cur) > 0) lows.Add(ToLow(cur));
                    continue;
                }
                if (cur.Quantity != old.Quantity)
                {
                    int delta = cur.Quantity - old.Quantity;
                    string by = "", reason;
                    if (_attribution.TryGetValue(cur.Id, out var a)) { by = a.By; reason = a.Reason; }
                    else reason = LastMovementReason(cur.Id, delta);
                    updates.Add(new InventoryChange(cur.Sku, cur.Name, old.Quantity, cur.Quantity, reason, by));
                }
                if (Status(cur) > Status(old)) lows.Add(ToLow(cur));     // OK -> LOW, LOW -> OUT, or the reorder level was raised
            }
            foreach (var old in _snapshot.Values)
                if (!now.ContainsKey(old.Id) && old.Quantity != 0)
                    updates.Add(new InventoryChange(old.Sku, old.Name, old.Quantity, 0, "Product removed", ""));
            _snapshot = now;
            _attribution.Clear();
        }
        if (updates.Count > 0 || lows.Count > 0)
            Changes?.Invoke(updates, lows);
    }

    static int Status(State s) => s.Quantity == 0 ? 2 : s.Quantity <= s.ReorderLevel ? 1 : 0;

    static LowStockInfo ToLow(State s) => new(s.Sku, s.Name, s.Quantity, s.ReorderLevel, Status(s) == 2 ? "OUT" : "LOW", s.Supplier);

    static string LastMovementReason(long productId, int delta)
    {
        try
        {
            var t = Db.Query("SELECT Delta, Reason FROM StockMovements WHERE ProductId = @id ORDER BY Id DESC LIMIT 1", ("@id", productId));
            if (t.Rows.Count == 1 && Convert.ToInt64(t.Rows[0]["Delta"]) == delta && t.Rows[0]["Reason"] is not DBNull)
                return Convert.ToString(t.Rows[0]["Reason"]) ?? "Stock changed";
        }
        catch (Exception) { }
        return "Stock changed";
    }

    static Dictionary<long, State> Read()
    {
        var map = new Dictionary<long, State>();
        DataTable dt = Db.Query(@"SELECT p.Id, p.Sku, p.Name, p.Quantity, p.ReorderLevel, COALESCE(s.Name, '-') AS Supplier
                                  FROM Products p LEFT JOIN Suppliers s ON s.Id = p.SupplierId");
        foreach (DataRow r in dt.Rows)
        {
            long id = Convert.ToInt64(r["Id"]);
            map[id] = new State(id, Convert.ToString(r["Sku"]) ?? "", Convert.ToString(r["Name"]) ?? "",
                                Convert.ToInt32(r["Quantity"]), Convert.ToInt32(r["ReorderLevel"]), Convert.ToString(r["Supplier"]) ?? "-");
        }
        return map;
    }
}
