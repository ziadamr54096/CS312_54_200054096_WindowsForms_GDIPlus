namespace InventoryManager.Network;

/// <summary>
/// Executes inventory requests that arrive over the network against THIS computer's database (the server's). Clients never see the
/// database file; they only send small requests. The write uses the same two statements as the Stock Movements tab.
///
/// IMPORTANT (threading): Db.DataChanged is raised on whatever thread performs the write and the existing screens update their
/// controls from that event. Network handlers run on thread-pool threads, so every database operation is posted to the UI thread.
/// </summary>
public sealed class DbInventoryGateway : IInventoryGateway
{
    readonly SynchronizationContext _ui;
    readonly InventoryWatcher _watcher;

    public DbInventoryGateway(SynchronizationContext ui, InventoryWatcher watcher)
    {
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        _watcher = watcher;
    }

    public Task<InventoryResult> HandleAsync(InventoryRequest request, DeviceInfo from)
    {
        var tcs = new TaskCompletionSource<InventoryResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        _ui.Post(_ =>
        {
            try { tcs.SetResult(Execute(request, from)); }
            catch (Exception ex) { tcs.SetException(ex); }
        }, null);
        return tcs.Task;
    }

    InventoryResult Execute(InventoryRequest req, DeviceInfo from)
    {
        var t = Db.Query("SELECT Id, Sku, Name, Quantity, ReorderLevel FROM Products WHERE UPPER(Sku) = UPPER(@s)", ("@s", req.Sku));
        if (t.Rows.Count == 0) return new InventoryResult(false, $"No product with SKU '{req.Sku}' exists on the server.");

        var r = t.Rows[0];
        long id = Convert.ToInt64(r["Id"]);
        string sku = Convert.ToString(r["Sku"]) ?? req.Sku, name = Convert.ToString(r["Name"]) ?? sku;
        int qty = Convert.ToInt32(r["Quantity"]), level = Convert.ToInt32(r["ReorderLevel"]);

        if (req.Action == "GET")
            return new InventoryResult(true, $"{name} ({sku}): {qty} in stock (reorder level {level}).", sku, name, qty, level);

        int newQty = qty + req.Delta;
        if (newQty < 0)
            return new InventoryResult(false, $"Not enough stock: only {qty} of {name} ({sku}) in stock.", sku, name, qty, level);

        string reason = string.IsNullOrWhiteSpace(req.Reason) ? "Network adjustment" : req.Reason.Trim();
        string movementReason = $"Network ({from.Name}): {reason}";
        if (movementReason.Length > 100) movementReason = movementReason.Substring(0, 100);

        _watcher.Attribute(id, from.Name, reason);        // lets the broadcast say who did it
        Db.Execute(@"
            UPDATE Products SET Quantity = Quantity + @d WHERE Id = @id;
            INSERT INTO StockMovements(ProductId, Delta, Reason, CreatedAt) VALUES(@id, @d, @r, @now);",
            ("@id", id), ("@d", req.Delta), ("@r", movementReason), ("@now", Db.Now()));

        return new InventoryResult(true, $"{name} ({sku}): {qty} -> {newQty}.", sku, name, newQty, level);
    }
}
