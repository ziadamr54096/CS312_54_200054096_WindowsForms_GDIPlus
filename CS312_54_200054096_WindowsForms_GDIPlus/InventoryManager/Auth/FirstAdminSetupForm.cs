namespace InventoryManager.Auth;

/// <summary>Shown only when no active administrator exists. The person running setup chooses the password; nothing is hardcoded.</summary>
public partial class FirstAdminSetupForm : Form
{
    public FirstAdminSetupForm()
    {
        InitializeComponent();   // visual layout only
    }

    void ShowError(string message) => lblError.Text = message;

    void chkShow_CheckedChanged(object? sender, EventArgs e)
    {
        bool mask = !chkShow.Checked;
        txtPassword.UseSystemPasswordChar = mask;
        txtConfirm.UseSystemPasswordChar = mask;
    }

    void btnCreate_Click(object? sender, EventArgs e)
    {
        ShowError("");
        string username = txtUsername.Text.Trim();

        string? err = PasswordPolicy.ValidateUsername(username);
        if (err != null) { ShowError(err); txtUsername.Focus(); return; }
        err = PasswordPolicy.Validate(txtPassword.Text, username);
        if (err != null) { ShowError(err); txtPassword.Focus(); return; }
        if (txtPassword.Text != txtConfirm.Text) { ShowError("The two passwords do not match."); txtConfirm.Focus(); return; }

        try
        {
            Cursor = Cursors.WaitCursor;
            bool created = AuthService.ProvisionFirstAdmin(username, txtFullName.Text, txtPassword.Text);
            Ui.Info(created
                ? $"Administrator '{username}' was created. Please sign in with this account."
                : "An administrator already exists. Please sign in.");
            DialogResult = DialogResult.OK;
        }
        catch (InvalidOperationException ex) { ShowError(ex.Message); }
        catch (Exception ex) { Ui.Error(ex); }
        finally { Cursor = Cursors.Default; }
    }
}
