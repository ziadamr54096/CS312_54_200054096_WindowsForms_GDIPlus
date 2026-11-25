namespace InventoryManager.Auth;

/// <summary>Stockora sign-in screen. Layout lives in LoginForm.Designer.cs (editable in Visual Studio); this file holds behaviour only.</summary>
public partial class LoginForm : Form
{
    /// <summary>Set when DialogResult is OK.</summary>
    public AuthUser? SignedInUser { get; private set; }

    public LoginForm()
    {
        InitializeComponent();   // visual layout only
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Ui.InDesigner(this)) return;

        // "Remember me" restores only the username.
        string? saved = LoginPreferences.LoadUsername();
        if (!string.IsNullOrEmpty(saved))
        {
            txtUsername.Text = saved;
            chkRemember.Checked = true;
            ActiveControl = txtPassword;
        }
        else ActiveControl = txtUsername;
    }

    void ShowError(string message) => lblError.Text = message;

    void Field_TextChanged(object? sender, EventArgs e) => lblError.Text = "";

    void btnTogglePassword_Click(object? sender, EventArgs e)
    {
        txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
        btnTogglePassword.Text = txtPassword.UseSystemPasswordChar ? "Show" : "Hide";
        txtPassword.Focus();
    }

    void txtPassword_Enter(object? sender, EventArgs e) => UpdateCapsWarning();
    void txtPassword_KeyUp(object? sender, KeyEventArgs e) => UpdateCapsWarning();

    void UpdateCapsWarning()
    {
        try { lblCaps.Visible = IsKeyLocked(Keys.CapsLock); }
        catch (NotSupportedException) { lblCaps.Visible = false; }
    }

    void btnLogin_Click(object? sender, EventArgs e)
    {
        ShowError("");
        string username = txtUsername.Text.Trim();
        string password = txtPassword.Text;

        if (username.Length == 0) { ShowError("Please enter your username."); txtUsername.Focus(); return; }
        if (password.Length == 0) { ShowError("Please enter your password."); txtPassword.Focus(); return; }

        btnLogin.Enabled = false;
        Cursor = Cursors.WaitCursor;
        try
        {
            LoginResult result = AuthService.Authenticate(username, password);
            if (result.Status == LoginStatus.Success && result.User != null)
            {
                LoginPreferences.Save(chkRemember.Checked ? result.User.Username : null);
                SignedInUser = result.User;
                DialogResult = DialogResult.OK;      // closes the dialog; the password is not kept anywhere
                return;
            }

            txtPassword.Clear();                     // clear first: changing the text resets the error label (Field_TextChanged)
            txtPassword.Focus();
            ShowError(result.Message);               // generic for wrong user/password; specific for deactivated or locked accounts
        }
        catch (Exception ex)
        {
            ShowError("Sign-in is unavailable right now. Please try again.");
            Ui.Error(ex);
        }
        finally
        {
            Cursor = Cursors.Default;
            btnLogin.Enabled = true;
        }
    }
}
