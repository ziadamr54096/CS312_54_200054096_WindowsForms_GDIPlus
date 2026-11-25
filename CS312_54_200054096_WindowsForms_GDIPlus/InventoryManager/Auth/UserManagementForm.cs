using System.Data;

namespace InventoryManager.Auth;

/// <summary>Administrator-only account management. Every operation is re-authorised by UserService against the database.</summary>
public partial class UserManagementForm : Form
{
    AuthUser? _actor;

    public UserManagementForm()   // parameterless constructor = required by the Visual Studio Designer
    {
        InitializeComponent();
    }

    public UserManagementForm(AuthUser actor) : this()
    {
        _actor = actor;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Ui.InDesigner(this) || _actor == null) return;
        cboRole.SelectedIndex = 0;
        LoadUsers();
    }

    // ---------- helpers ----------

    void ShowFailure(Exception ex)
    {
        if (ex is InvalidOperationException or UnauthorizedAccessException) Ui.Warn(ex.Message);
        else Ui.Error(ex);
    }

    void LoadUsers()
    {
        try
        {
            dgvUsers.DataSource = UserService.List(_actor!);
            dgvUsers.Columns["Id"]!.Visible = false;
            dgvUsers.Columns["FullName"]!.HeaderText = "Full name";
            dgvUsers.Columns["IsActive"]!.HeaderText = "Active";
            dgvUsers.Columns["MustChangePassword"]!.HeaderText = "Must change password";
            dgvUsers.Columns["LastLoginAt"]!.HeaderText = "Last sign-in";
            dgvUsers.Columns["CreatedAt"]!.HeaderText = "Created";
            dgvUsers.Columns["CreatedBy"]!.HeaderText = "Created by";
        }
        catch (Exception ex) { ShowFailure(ex); }
    }

    bool TrySelected(out long id, out bool active, out string username)
    {
        id = 0; active = false; username = "";
        if (dgvUsers.SelectedRows.Count != 1 || dgvUsers.SelectedRows[0].DataBoundItem is not DataRowView v)
        {
            Ui.Warn("Select a user in the list first.");
            return false;
        }
        id = Convert.ToInt64(v["Id"]);
        active = Convert.ToInt64(v["IsActive"]) == 1;
        username = Convert.ToString(v["Username"]) ?? "";
        return true;
    }

    UserRole ChosenRole => cboRole.SelectedIndex == 1 ? UserRole.Admin : UserRole.User;

    // ---------- event handlers (wired in the Designer) ----------

    void chkShow_CheckedChanged(object? sender, EventArgs e) => txtPassword.UseSystemPasswordChar = !chkShow.Checked;

    void btnGenerate_Click(object? sender, EventArgs e)
    {
        txtPassword.Text = PasswordPolicy.Generate();
        chkShow.Checked = true;      // so the administrator can read it out / hand it over
    }

    void dgvUsers_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.ColumnIndex < 0 || e.Value is not long n) return;
        string col = dgvUsers.Columns[e.ColumnIndex].Name;
        if (col is "IsActive" or "MustChangePassword") { e.Value = n == 1 ? "Yes" : "No"; e.FormattingApplied = true; }
    }

    void dgvUsers_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        foreach (DataGridViewRow row in dgvUsers.Rows)
            if (row.DataBoundItem is DataRowView v && Convert.ToInt64(v["IsActive"]) != 1)
                row.DefaultCellStyle.ForeColor = Color.Gray;
        dgvUsers.ClearSelection();
    }

    void dgvUsers_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvUsers.SelectedRows.Count == 1 && dgvUsers.SelectedRows[0].DataBoundItem is DataRowView v)
            btnToggleActive.Text = Convert.ToInt64(v["IsActive"]) == 1 ? "Deactivate" : "Activate";
    }

    void btnCreate_Click(object? sender, EventArgs e)
    {
        try
        {
            string username = txtUsername.Text.Trim();
            UserService.Create(_actor!, username, txtFullName.Text, ChosenRole, txtPassword.Text, chkMustChange.Checked);
            Ui.Info(chkMustChange.Checked
                ? $"User '{username}' was created. Give them the temporary password; they must change it at first sign-in."
                : $"User '{username}' was created.");
            txtUsername.Clear(); txtFullName.Clear(); txtPassword.Clear();
            chkShow.Checked = false;
            LoadUsers();
        }
        catch (Exception ex) { ShowFailure(ex); }
    }

    void btnReset_Click(object? sender, EventArgs e)
    {
        if (!TrySelected(out long id, out _, out string username)) return;
        try
        {
            UserService.ResetPassword(_actor!, id, txtPassword.Text, chkMustChange.Checked);
            Ui.Info($"The password for '{username}' was reset.");
            txtPassword.Clear();
            chkShow.Checked = false;
            LoadUsers();
        }
        catch (Exception ex) { ShowFailure(ex); }
    }

    void btnToggleActive_Click(object? sender, EventArgs e)
    {
        if (!TrySelected(out long id, out bool active, out string username)) return;
        if (active && !Ui.Confirm($"Deactivate '{username}'? They will no longer be able to sign in.")) return;
        try
        {
            UserService.SetActive(_actor!, id, !active);
            LoadUsers();
        }
        catch (Exception ex) { ShowFailure(ex); }
    }

    void btnSetRole_Click(object? sender, EventArgs e)
    {
        if (!TrySelected(out long id, out _, out string username)) return;
        try
        {
            UserService.SetRole(_actor!, id, ChosenRole);
            Ui.Info($"'{username}' is now {ChosenRole}.");
            LoadUsers();
        }
        catch (Exception ex) { ShowFailure(ex); }
    }
}
