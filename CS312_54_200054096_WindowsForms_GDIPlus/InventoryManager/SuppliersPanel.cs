using System.Data;
using System.Text.RegularExpressions;

namespace InventoryManager;

public partial class SuppliersPanel : UserControl
{
    long? selectedId;
    bool ready;

    public SuppliersPanel()
    {
        InitializeComponent();   // visual layout only - see SuppliersPanel.Designer.cs
    }

    // ---------- runtime initialisation (skipped inside the Visual Studio Designer) ----------

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Ui.InDesigner(this) || ready) return;

        ready = true;
        Db.DataChanged -= LoadData;
        Db.DataChanged += LoadData;
        Disposed -= OnPanelDisposed;
        Disposed += OnPanelDisposed;
        LoadData();
    }

    void OnPanelDisposed(object? sender, EventArgs e) => Db.DataChanged -= LoadData;

    void LoadData()
    {
        try
        {
            selectedId = null;
            dgvSuppliers.DataSource = Db.Query(@"
                SELECT s.Id, s.Name, s.Phone, s.Email, s.Address,
                       (SELECT COUNT(*) FROM Products p WHERE p.SupplierId = s.Id) AS Products
                FROM Suppliers s
                WHERE s.Name LIKE @q OR COALESCE(s.Email,'') LIKE @q OR COALESCE(s.Address,'') LIKE @q
                ORDER BY s.Name",
                ("@q", "%" + txtSearch.Text.Trim() + "%"));
            dgvSuppliers.Columns["Id"]!.Visible = false;
        }
        catch (Exception ex) { Ui.Error(ex); }
    }

    // ---------- event handlers (wired in the Designer) ----------

    void txtSearch_TextChanged(object? sender, EventArgs e) { if (ready) LoadData(); }

    void dgvSuppliers_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0) FillFromSelection();
    }

    void dgvSuppliers_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is Keys.Up or Keys.Down or Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End)
            FillFromSelection();
    }

    void dgvSuppliers_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e) =>
        dgvSuppliers.ClearSelection();

    void btnAdd_Click(object? sender, EventArgs e) => Save(false);
    void btnUpdate_Click(object? sender, EventArgs e) => Save(true);
    void btnDelete_Click(object? sender, EventArgs e) => Delete();
    void btnClear_Click(object? sender, EventArgs e) => ClearForm();

    // ---------- business logic (unchanged) ----------

    void FillFromSelection()
    {
        if (dgvSuppliers.SelectedRows.Count == 0 || dgvSuppliers.SelectedRows[0].DataBoundItem is not DataRowView v) return;
        var r = v.Row;
        selectedId = Convert.ToInt64(r["Id"]);
        txtName.Text = Convert.ToString(r["Name"]);
        txtPhone.Text = r["Phone"] is DBNull ? "" : Convert.ToString(r["Phone"]);
        txtEmail.Text = r["Email"] is DBNull ? "" : Convert.ToString(r["Email"]);
        txtAddress.Text = r["Address"] is DBNull ? "" : Convert.ToString(r["Address"]);
    }

    void ClearForm()
    {
        selectedId = null;
        txtName.Clear(); txtPhone.Clear(); txtEmail.Clear(); txtAddress.Clear();
        dgvSuppliers.ClearSelection();
        txtName.Focus();
    }

    void Save(bool update)
    {
        try
        {
            if (update && selectedId == null) { Ui.Warn("Select a supplier from the list first."); return; }
            string name = txtName.Text.Trim(), phone = txtPhone.Text.Trim(), email = txtEmail.Text.Trim();
            if (name.Length < 2) { Ui.Warn("Please enter the supplier name."); txtName.Focus(); return; }
            if (phone.Length > 0 && !Regex.IsMatch(phone, @"^[0-9+\-\s()]{6,20}$"))
            { Ui.Warn("Phone may contain digits, spaces, + - ( ) only (6-20 characters)."); txtPhone.Focus(); return; }
            if (email.Length > 0 && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { Ui.Warn("That e-mail address is not valid."); txtEmail.Focus(); return; }

            object? ph = phone.Length == 0 ? null : phone;
            object? em = email.Length == 0 ? null : email;
            object? ad = txtAddress.Text.Trim().Length == 0 ? null : txtAddress.Text.Trim();

            if (!update)
                Db.Execute("INSERT INTO Suppliers(Name,Phone,Email,Address) VALUES(@n,@p,@e,@a)",
                    ("@n", name), ("@p", ph), ("@e", em), ("@a", ad));
            else
                Db.Execute("UPDATE Suppliers SET Name=@n, Phone=@p, Email=@e, Address=@a WHERE Id=@id",
                    ("@id", selectedId), ("@n", name), ("@p", ph), ("@e", em), ("@a", ad));
            ClearForm();
        }
        catch (Exception ex) { Ui.Error(ex); }
    }

    void Delete()
    {
        try
        {
            if (selectedId == null) { Ui.Warn("Select a supplier from the list first."); return; }
            if (!Ui.Confirm($"Delete supplier '{txtName.Text}'? Its products will be kept but left without a supplier.")) return;
            Db.Execute("DELETE FROM Suppliers WHERE Id=@id", ("@id", selectedId));
            ClearForm();
        }
        catch (Exception ex) { Ui.Error(ex); }
    }
}
