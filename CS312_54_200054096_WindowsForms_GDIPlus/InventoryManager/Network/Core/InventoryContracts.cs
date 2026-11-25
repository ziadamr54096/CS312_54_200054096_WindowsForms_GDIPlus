namespace InventoryManager.Network;

/// <summary>A stock request sent by a client. The SERVER executes it against ITS database (clients never touch the database file).</summary>
public sealed record InventoryRequest(string Action, string Sku, int Delta, string Reason);   // Action: "GET" or "ADJUST"

public sealed record InventoryResult(bool Ok, string Message, string Sku = "", string Name = "", int Quantity = 0, int ReorderLevel = 0);

/// <summary>Implemented by the application (DbInventoryGateway); the networking core stays free of database code.</summary>
public interface IInventoryGateway
{
    Task<InventoryResult> HandleAsync(InventoryRequest request, DeviceInfo from);
}

/// <summary>A stock quantity changed on the server (broadcast as INVENTORY_UPDATE).</summary>
public sealed record InventoryChange(string Sku, string Name, int OldQuantity, int NewQuantity, string Reason, string By)
{
    public int Delta => NewQuantity - OldQuantity;
}

/// <summary>A product reached its reorder level or ran out (broadcast as LOW_STOCK_ALERT). Status is "LOW" or "OUT".</summary>
public sealed record LowStockInfo(string Sku, string Name, int Quantity, int ReorderLevel, string Status, string Supplier);

public static class InventoryMessages
{
    public static NetMessage Update(InventoryChange c) => NetMessage.Create(MessageTypes.InventoryUpdate, "",
        $"{c.Name} ({c.Sku}): {c.OldQuantity} -> {c.NewQuantity}" + (string.IsNullOrEmpty(c.By) ? "" : $" by {c.By}"),
        data: new Dictionary<string, string>
        {
            ["sku"] = c.Sku, ["name"] = c.Name, ["old"] = c.OldQuantity.ToString(), ["new"] = c.NewQuantity.ToString(),
            ["delta"] = c.Delta.ToString(), ["reason"] = c.Reason, ["by"] = c.By
        });

    public static NetMessage LowStock(LowStockInfo i) => NetMessage.Create(MessageTypes.LowStockAlert, "",
        i.Status == "OUT" ? $"OUT OF STOCK: {i.Name} ({i.Sku})" : $"Low stock: {i.Name} ({i.Sku}) - {i.Quantity} left (reorder level {i.ReorderLevel})",
        data: new Dictionary<string, string>
        {
            ["sku"] = i.Sku, ["name"] = i.Name, ["quantity"] = i.Quantity.ToString(), ["reorder"] = i.ReorderLevel.ToString(),
            ["status"] = i.Status, ["supplier"] = i.Supplier
        });
}
