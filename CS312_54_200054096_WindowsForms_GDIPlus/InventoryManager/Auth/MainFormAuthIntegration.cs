using System.Runtime.InteropServices;

namespace InventoryManager.Auth;

/// <summary>
/// Adds the account commands to the MAIN WINDOW'S SYSTEM MENU (click the window icon, right-click the title bar, or press Alt+Space):
///   Signed in as ...  |  Change my password...  |  Manage users... (admins only)  |  Log out
/// This keeps MainForm's visual layout untouched: no controls are added to it and it needs no code changes.
/// </summary>
public sealed class MainFormAuthIntegration : NativeWindow
{
    const int WM_SYSCOMMAND = 0x0112;
    const uint MF_STRING = 0x0000, MF_GRAYED = 0x0001, MF_SEPARATOR = 0x0800;
    const int ID_INFO = 0x1000, ID_CHANGE_PASSWORD = 0x1010, ID_MANAGE_USERS = 0x1020, ID_LOGOUT = 0x1030;

    [DllImport("user32.dll")] static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "AppendMenuW")]
    static extern bool AppendMenu(IntPtr hMenu, uint uFlags, UIntPtr uIDNewItem, string? lpNewItem);

    readonly Form _form;
    readonly AuthUser _user;

    MainFormAuthIntegration(Form form, AuthUser user) { _form = form; _user = user; }

    /// <summary>Call once, before the form is shown.</summary>
    public static void Attach(Form form, AuthUser user)
    {
        var integration = new MainFormAuthIntegration(form, user);
        form.HandleCreated += (_, _) => integration.Install();
        form.HandleDestroyed += (_, _) => integration.ReleaseHandle();
        if (form.IsHandleCreated) integration.Install();
    }

    void Install()
    {
        try
        {
            if (Handle != _form.Handle) { ReleaseHandle(); AssignHandle(_form.Handle); }
            IntPtr menu = GetSystemMenu(_form.Handle, false);
            if (menu == IntPtr.Zero) return;
            AppendMenu(menu, MF_SEPARATOR, UIntPtr.Zero, null);
            AppendMenu(menu, MF_STRING | MF_GRAYED, (UIntPtr)ID_INFO, $"Signed in as {_user.Username} ({(_user.IsAdmin ? "Admin" : "User")})");
            AppendMenu(menu, MF_STRING, (UIntPtr)ID_CHANGE_PASSWORD, "Change my password...");
            if (_user.IsAdmin) AppendMenu(menu, MF_STRING, (UIntPtr)ID_MANAGE_USERS, "Manage users...");
            AppendMenu(menu, MF_STRING, (UIntPtr)ID_LOGOUT, "Log out");
        }
        catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
        {
            // Not running on Windows: the menu commands are simply unavailable.
        }
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_SYSCOMMAND)
        {
            switch ((int)(m.WParam.ToInt64() & 0xFFF0))
            {
                case ID_CHANGE_PASSWORD: _form.BeginInvoke(new Action(ChangePassword)); return;
                case ID_MANAGE_USERS when _user.IsAdmin: _form.BeginInvoke(new Action(ManageUsers)); return;
                case ID_LOGOUT: _form.BeginInvoke(new Action(() => { if (Ui.Confirm("Log out of Stockora?")) RequestLogout(_form); })); return;
            }
        }
        base.WndProc(ref m);
    }

    void ChangePassword()
    {
        using var f = new ChangePasswordForm(_user, forced: false);
        f.ShowDialog(_form);
    }

    void ManageUsers()
    {
        using var f = new UserManagementForm(_user);
        f.ShowDialog(_form);
    }

    /// <summary>Ends the session: StartupFlow sees the flag after MainForm closes and shows the LoginForm again.</summary>
    public static void RequestLogout(Form mainForm)
    {
        Session.LogoutRequested = true;
        mainForm.Close();
    }
}
