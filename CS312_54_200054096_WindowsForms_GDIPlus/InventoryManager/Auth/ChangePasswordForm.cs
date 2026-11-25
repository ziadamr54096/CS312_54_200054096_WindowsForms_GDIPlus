namespace InventoryManager.Auth;

/// <summary>Lets a signed-in user change their own password. In forced mode (temporary password) it must be completed to continue.</summary>
public partial class ChangePasswordForm : Form
{
    AuthUser? _user;

    /// <summary>The refreshed account after a successful change.</summary>
    public AuthUser? UpdatedUser { get; private set; }

    public ChangePasswordForm()   // parameterless constructor = required by the Visual Studio Designer
    {
        InitializeComponent();
    }

    public ChangePasswordForm(AuthUser user, bool forced) : this()
    {
        _user = user;
        if (forced)
        {
            lblInfo.Text = "An administrator gave you a temporary password. Choose your own password to continue.";
            ControlBox = false;
        }
    }

    void ShowError(string message) => lblError.Text = message;

    void chkShow_CheckedChanged(object? sender, EventArgs e)
    {
        bool mask = !chkShow.Checked;
        txtCurrent.UseSystemPasswordChar = mask;
        txtNew.UseSystemPasswordChar = mask;
        txtConfirm.UseSystemPasswordChar = mask;
    }

    void btnOk_Click(object? sender, EventArgs e)
    {
        ShowError("");
        if (_user == null) return;
        if (txtCurrent.Text.Length == 0) { ShowError("Enter your current password."); txtCurrent.Focus(); return; }
        string? err = PasswordPolicy.Validate(txtNew.Text, _user.Username);
        if (err != null) { ShowError(err); txtNew.Focus(); return; }
        if (txtNew.Text != txtConfirm.Text) { ShowError("The two new passwords do not match."); txtConfirm.Focus(); return; }

        try
        {
            Cursor = Cursors.WaitCursor;
            UpdatedUser = UserService.ChangeOwnPassword(_user, txtCurrent.Text, txtNew.Text);
            Ui.Info("Your password was changed.");
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex) when (ex is InvalidOperationException or UnauthorizedAccessException) { ShowError(ex.Message); }
        catch (Exception ex) { Ui.Error(ex); }
        finally { Cursor = Cursors.Default; }
    }
}
