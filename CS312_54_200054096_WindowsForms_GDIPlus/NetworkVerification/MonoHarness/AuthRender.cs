using InventoryManager;
using InventoryManager.Auth;
public static class AuthRender
{
    [STAThread] public static int Main()
    {
        Environment.SetEnvironmentVariable("XDG_DATA_HOME", "/tmp/vauth/data_render_" + Guid.NewGuid().ToString("N").Substring(0, 6));
        Application.EnableVisualStyles();
        Db.Init(); AuthSchema.Ensure();
        AuthService.ProvisionFirstAdmin("owner", "Store Owner", "Str0ng!Pass1");
        var admin = AuthService.GetById(Db.Scalar<long>("SELECT Id FROM Users WHERE Username='owner'"))!;
        UserService.Create(admin, "alice", "Alice Hassan", UserRole.User, "Al1ce!Pass99", false);
        UserService.Create(admin, "bob", "Bob Samir", UserRole.User, "B0b!Secret1", true);
        UserService.SetActive(admin, Db.Scalar<long>("SELECT Id FROM Users WHERE Username='bob'"), false);
        var forms = new (string, Func<Form>)[] {
            ("login", () => { var f = new LoginForm(); return f; }),
            ("setup", () => new FirstAdminSetupForm()),
            ("change", () => new ChangePasswordForm(admin, true)),
            ("users", () => new UserManagementForm(admin)) };
        foreach (var (name, make) in forms)
        {
            using var f = make();
            f.StartPosition = FormStartPosition.Manual; f.Location = new Point(0, 0); f.Show();
            if (name == "login") { ((TextBox)f.Controls.Find("txtUsername", true)[0]).Text = "alice"; ((TextBox)f.Controls.Find("txtPassword", true)[0]).Text = "x"; ((Button)f.Controls.Find("btnLogin", true)[0]).PerformClick(); }
            for (int i = 0; i < 8; i++) { f.Refresh(); Application.DoEvents(); Thread.Sleep(120); }
            System.Diagnostics.Process.Start("scrot", $"-o /tmp/vauth/shot_{name}.png")!.WaitForExit();
            f.Close();
        }
        return 0;
    }
}
