using System.Data;
using System.Text.RegularExpressions;

namespace InventoryManager;

public partial class ProductsPanel : UserControl
{
    long? selectedId;
    int selectedQty;
    bool ready;

    public ProductsPanel()
    {
        InitializeComponent();   // visual layout only - see ProductsPanel.Designer.cs
    }

    // ---------- runtime initialisation (skipped inside the Visual Studio Designer) ----------

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Ui.InDesigner(this) || ready) return;

        cboFilter.SelectedIndex = 0;
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
        LoadSuppliers();
        LoadCategories();
        LoadData();
    }

    void LoadSuppliers()
    {
        var current = cboSupplier.SelectedValue;
        var dt = Db.Query("SELECT Id, Name FROM Suppliers ORDER BY Name");
        var none = dt.NewRow();
        none["Id"] = DBNull.Value;
        none["Name"] = "(none)";
        dt.Rows.InsertAt(none, 0);
        cboSupplier.DataSource = dt;
        cboSupplier.DisplayMember = "Name";
        cboSupplier.ValueMember = "Id";
        if (current is long) cboSupplier.SelectedValue = current;
    }

    void LoadCategories()
    {
        var text = cboCategory.Text;
        cboCategory.Items.Clear();
        foreach (DataRow r in Db.Query("SELECT DISTINCT Category FROM Products WHERE Category IS NOT NULL AND Category <> '' ORDER BY 1").Rows)
            cboCategory.Items.Add(r[0]);
        cboCategory.Text = text;
    }

    void LoadData()
    {
        try
        {
            string where = "(p.Name LIKE @q OR p.Sku LIKE @q OR COALESCE(p.Category,'') LIKE @q)";
            if (cboFilter.SelectedIndex == 1) where += " AND p.Quantity > 0 AND p.Quantity <= p.ReorderLevel";
            else if (cboFilter.SelectedIndex == 2) where += " AND p.Quantity = 0";

            selectedId = null;
            dgvProducts.DataSource = Db.Query($@"
                SELECT p.Id, p.Sku, p.Name, p.Category, p.SupplierId,
                       COALESCE(s.Name, '-') AS Supplier, p.UnitPrice, p.Quantity, p.ReorderLevel
                FROM Products p LEFT JOIN Suppliers s ON s.Id = p.SupplierId
                WHERE {where} ORDER BY p.Name",
                ("@q", "%" + txtSearch.Text.Trim() + "%"));

            dgvProducts.Columns["Id"]!.Visible = false;
            dgvProducts.Columns["SupplierId"]!.Visible = false;
            dgvProducts.Columns["UnitPrice"]!.DefaultCellStyle.Format = "N2";
            dgvProducts.Columns["UnitPrice"]!.HeaderText = "Unit price";
            dgvProducts.Columns["ReorderLevel"]!.HeaderText = "Reorder level";
        }
        catch (Exception ex) { Ui.Error(ex); }
    }

    void Colorize()
    {
        foreach (DataGridViewRow row in dgvProducts.Rows)
        {
            if (row.DataBoundItem is not DataRowView v) continue;
            long q = Convert.ToInt64(v["Quantity"]);
            long lvl = Convert.ToInt64(v["ReorderLevel"]);
            row.DefaultCellStyle.BackColor =
                q == 0 ? Color.FromArgb(255, 205, 205) :
                q <= lvl ? Color.FromArgb(255, 240, 190) : Color.White;
        }
    }

    // ---------- event handlers (wired in the Designer) ----------

    void txtSearch_TextChanged(object? sender, EventArgs e) { if (ready) LoadData(); }
    void cboFilter_SelectedIndexChanged(object? sender, EventArgs e) { if (ready) LoadData(); }

    void dgvProducts_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) FillFromSelection();
    }

    void dgvProducts_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End)
            FillFromSelection();
    }

    void dgvProducts_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        Colorize();
        dgvProducts.ClearSelection();
    }

    void btnAdd_Click(object? sender, EventArgs e) => Save(false);
    void btnUpdate_Click(object? sender, EventArgs e) => Save(true);
    void btnDelete_Click(object? sender, EventArgs e) => Delete();
    void btnClear_Click(object? sender, EventArgs e) => ClearForm();

    // ---------- business logic (unchanged) ----------

    void FillFromSelection()
    {
        if (dgvProducts.SelectedRows.Count == 0 || dgvProducts.SelectedRows[0].DataBoundItem is not DataRowView v) return;
        var r = v.Row;
        selectedId = Convert.ToInt64(r["Id"]);
        txtSku.Text = Convert.ToString(r["Sku"]);
        txtName.Text = Convert.ToString(r["Name"]);
        cboCategory.Text = r["Category"] is DBNull ? "" : Convert.ToString(r["Category"]);
        if (r["SupplierId"] is DBNull) cboSupplier.SelectedIndex = 0;
        else cboSupplier.SelectedValue = r["SupplierId"];
        numPrice.Value = Convert.ToDecimal(r["UnitPrice"]);
        selectedQty = Convert.ToInt32(r["Quantity"]);
        numQty.Value = selectedQty;
        numReorder.Value = Convert.ToDecimal(r["ReorderLevel"]);
    }

    void ClearForm()
    {
        selectedId = null;
        txtSku.Clear();
        txtName.Clear();
        cboCategory.Text = "";
        cboSupplier.SelectedIndex = 0;
        numPrice.Value = 0;
        numQty.Value = 0;
        numReorder.Value = 5;
        dgvProducts.ClearSelection();
        txtSku.Focus();
    }

    void Save(bool update)
    {
        try
        {
            if (update && selectedId == null) { Ui.Warn("Select a product from the list first."); return; }

            string sku = txtSku.Text.Trim().ToUpperInvariant();
            string name = txtName.Text.Trim();
            if (!Regex.IsMatch(sku, @"^[A-Z0-9][A-Z0-9\-_]{1,19}$"))
            { Ui.Warn("SKU must be 2-20 characters: letters, digits, '-' or '_'."); txtSku.Focus(); return; }
            if (name.Length < 2)
            { Ui.Warn("Please enter a product name (at least 2 characters)."); txtName.Focus(); return; }
            if (numPrice.Value <= 0)
            { Ui.Warn("Unit price must be greater than zero."); numPrice.Focus(); return; }

            string cat = cboCategory.Text.Trim();
            object? sup = cboSupplier.SelectedValue is long sid ? sid : null;
            int qty = (int)numQty.Value;

            if (!update)
            {
                Db.Execute(@"
                    INSERT INTO Products(Sku,Name,Category,SupplierId,UnitPrice,Quantity,ReorderLevel)
                    VALUES(@sku,@name,@cat,@sup,@price,@qty,@lvl);
                    INSERT INTO StockMovements(ProductId,Delta,Reason,CreatedAt)
                    SELECT last_insert_rowid(), @qty, 'Initial stock', @now WHERE @qty <> 0;",
                    ("@sku", sku), ("@name", name), ("@cat", cat.Length == 0 ? null : cat), ("@sup", sup),
                    ("@price", (double)numPrice.Value), ("@qty", qty), ("@lvl", (int)numReorder.Value), ("@now", Db.Now()));
            }
            else
            {
                Db.Execute(@"
                    UPDATE Products SET Sku=@sku, Name=@name, Category=@cat, SupplierId=@sup,
                           UnitPrice=@price, Quantity=@qty, ReorderLevel=@lvl WHERE Id=@id;
                    INSERT INTO StockMovements(ProductId,Delta,Reason,CreatedAt)
                    SELECT @id, @delta, 'Manual adjustment', @now WHERE @delta <> 0;",
                    ("@id", selectedId), ("@sku", sku), ("@name", name), ("@cat", cat.Length == 0 ? null : cat),
                    ("@sup", sup), ("@price", (double)numPrice.Value), ("@qty", qty), ("@lvl", (int)numReorder.Value),
                    ("@delta", qty - selectedQty), ("@now", Db.Now()));
            }
            ClearForm();
        }
        catch (Exception ex) { Ui.Error(ex); }
    }

    void Delete()
    {
        try
        {
            if (selectedId == null) { Ui.Warn("Select a product from the list first."); return; }
            if (!Ui.Confirm($"Delete '{txtName.Text}'? Its stock history will be deleted too.")) return;
            Db.Execute("DELETE FROM Products WHERE Id=@id", ("@id", selectedId));
            ClearForm();
        }
        catch (Exception ex) { Ui.Error(ex); }
    }
}
