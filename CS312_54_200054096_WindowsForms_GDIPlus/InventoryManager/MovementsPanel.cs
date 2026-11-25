using System.Data;

namespace InventoryManager;

/// <summary>Stock in / stock out with history. A low-stock result can trigger an automatic e-mail alert (async).</summary>
public partial class MovementsPanel : UserControl
{
    bool ready;

    public MovementsPanel()
    {
        InitializeComponent();   // visual layout only - see MovementsPanel.Designer.cs
    }

    // ---------- runtime initialisation (skipped inside the Visual Studio Designer) ----------

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Ui.InDesigner(this) || ready) return;

        cboType.SelectedIndex = 0;
        ready = true;
        Db.DataChanged -= Reload;
        Db.DataChanged += Reload;
        Disposed -= OnPanelDisposed;
        Disposed += OnPanelDisposed;
        Reload();
    }

    void OnPanelDisposed(object? sender, EventArgs e) => Db.DataChanged -= Reload;

    void Reload()
    {
        try
        {
            var current = cboProduct.SelectedValue;
            cboProduct.DataSource = Db.Query(
                "SELECT Id, Sku || ' - ' || Name || '   (in stock: ' || Quantity || ')' AS Label FROM Products ORDER BY Name");
            cboProduct.DisplayMember = "Label";
            cboProduct.ValueMember = "Id";
            if (current is long) cboProduct.SelectedValue = current;
            LoadHistory();
        }
        catch (Exception ex) { Ui.Error(ex); }
    }

    void LoadHistory()
    {
        try
        {
            dgvHistory.DataSource = Db.Query(@"
                SELECT m.CreatedAt AS Date, p.Sku, p.Name AS Product, m.Delta, m.Reason
                FROM StockMovements m JOIN Products p ON p.Id = m.ProductId
                WHERE p.Name LIKE @q OR p.Sku LIKE @q OR COALESCE(m.Reason,'') LIKE @q
                ORDER BY m.Id DESC LIMIT 500",
                ("@q", "%" + txtSearch.Text.Trim() + "%"));
        }
        catch (Exception ex) { Ui.Error(ex); }
    }

    // ---------- event handlers (wired in the Designer) ----------

    void txtSearch_TextChanged(object? sender, EventArgs e) { if (ready) LoadHistory(); }

    void dgvHistory_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.ColumnIndex >= 0 && dgvHistory.Columns[e.ColumnIndex].Name == "Delta" && e.Value is long d)
            e.CellStyle!.ForeColor = d < 0 ? Color.Firebrick : Color.SeaGreen;
    }

    async void btnApply_Click(object? sender, EventArgs e)
    {
        bool lowNow = false;
        try
        {
            if (cboProduct.SelectedValue is not long pid) { Ui.Warn("Choose a product first."); return; }

            int qty = (int)numQty.Value;
            bool outgoing = cboType.SelectedIndex == 1;
            long current = Db.Scalar<long>("SELECT Quantity FROM Products WHERE Id=@id", ("@id", pid));
            if (outgoing && qty > current) { Ui.Warn($"Only {current} unit(s) are in stock."); return; }

            int delta = outgoing ? -qty : qty;
            string reason = txtReason.Text.Trim();
            if (reason.Length == 0) reason = outgoing ? "Stock out" : "Stock in";

            Db.Execute(@"
                UPDATE Products SET Quantity = Quantity + @d WHERE Id = @id;
                INSERT INTO StockMovements(ProductId, Delta, Reason, CreatedAt) VALUES(@id, @d, @r, @now);",
                ("@id", pid), ("@d", delta), ("@r", reason), ("@now", Db.Now()));

            txtReason.Clear();
            numQty.Value = 1;

            long level = Db.Scalar<long>("SELECT ReorderLevel FROM Products WHERE Id=@id", ("@id", pid));
            lowNow = current + delta <= level;
            lblStatus.ForeColor = lowNow ? Color.DarkOrange : Color.SeaGreen;
            lblStatus.Text = lowNow ? "Movement saved. Warning: this product is now low on stock." : "Movement saved.";
        }
        catch (Exception ex) { Ui.Error(ex); return; }

        if (lowNow && AlertService.Load().Auto)
        {
            try
            {
                lblStatus.Text += " Sending e-mail alert...";
                lblStatus.Text = await AlertService.NotifyLowStockAsync();
            }
            catch (Exception ex) { lblStatus.Text = "E-mail alert failed: " + ex.Message; }
        }
    }
}
