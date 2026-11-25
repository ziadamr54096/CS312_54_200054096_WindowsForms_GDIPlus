using System.ComponentModel;
using System.Data;
using System.Reflection;
using InventoryManager;
using InventoryManager.Auth;

class DesignCtx : LicenseContext { public override LicenseUsageMode UsageMode => LicenseUsageMode.Designtime; }
class RuntimeCtx : LicenseContext { public override LicenseUsageMode UsageMode => LicenseUsageMode.Runtime; }

public static class AuthUiTests
{
    static int pass, fail;
    static void Check(bool ok, string msg) { Console.WriteLine((ok ? "  PASS  " : "  FAIL  ") + msg); if (ok) pass++; else fail++; }
    const BindingFlags Inst = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    static T F<T>(object o, string name) => (T)o.GetType().GetField(name, Inst | BindingFlags.DeclaredOnly)!.GetValue(o)!;
    static void Type(object form, string field, string text) => F<TextBox>(form, field).Text = text;
    static void Click(object form, string field) => F<Button>(form, field).PerformClick();
    static void ShowIt(Control c)
    {
        if (c is Form f) { f.StartPosition = FormStartPosition.Manual; f.Location = new Point(10, 10); f.Show(); } else c.CreateControl();
        for (int i = 0; i < 3; i++) Application.DoEvents();
    }
    static string Err(object form) => F<Label>(form, "lblError").Text;
    static string DataHome(string tag) => "/tmp/vauth/data_ui_" + tag + "_" + Guid.NewGuid().ToString("N").Substring(0, 6);
    static IEnumerable<Control> All(Control c) { foreach (Control k in c.Controls) { yield return k; foreach (var d in All(k)) yield return d; } }
    static long Fixture(string user, string password, string role = "User", bool active = true)
    {
        Db.Execute(@"INSERT INTO Users(Username,FullName,PasswordHash,Role,IsActive,MustChangePassword,CreatedAt,CreatedBy) VALUES(@u,@u,@h,@r,@a,0,@now,'fixture')",
            ("@u", user), ("@h", PasswordHasher.Hash(password, 10000)), ("@r", role), ("@a", active ? 1 : 0), ("@now", Db.Now()));
        return Db.Scalar<long>("SELECT Id FROM Users WHERE Username=@u", ("@u", user));
    }

    [STAThread]
    public static int Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => Harness.Messages.Add("THREAD EXCEPTION: " + e.Exception);
        return args.Length > 0 && args[0] == "flow" ? Flow() : Forms();
    }

    // ============================================================ forms ============================================================
    static int Forms()
    {
        string home = DataHome("forms"); Environment.SetEnvironmentVariable("XDG_DATA_HOME", home);

        Console.WriteLine("== 1. DESIGN-TIME simulation of the new forms (any database access throws) ==");
        LicenseManager.CurrentContext = new DesignCtx(); Harness.ForbidDb = true;
        foreach (var t in new[] { typeof(LoginForm), typeof(FirstAdminSetupForm), typeof(ChangePasswordForm), typeof(UserManagementForm), typeof(BrandHeader) })
        {
            try
            {
                var c = (Control)Activator.CreateInstance(t)!;     // parameterless constructor, as the Designer does
                var fields = t.GetFields(Inst | BindingFlags.DeclaredOnly).Where(f => typeof(Control).IsAssignableFrom(f.FieldType)).ToList();
                int nulls = fields.Count(f => f.GetValue(c) == null), orphans = fields.Count(f => f.GetValue(c) is Control k && k.Parent == null);
                Check(nulls == 0 && orphans == 0 && (fields.Count > 0 || t == typeof(BrandHeader)), $"{t.Name}: InitializeComponent builds {fields.Count} controls, all placed in the tree");
                ShowIt(c);   // forms: Show() so that OnLoad really runs
                Check(!Harness.DbTouched && Ui.InDesigner(c), $"{t.Name}: loading in the designer does not touch the database");
                c.Dispose();
            }
            catch (Exception ex) { Check(false, $"{t.Name}: {ex.GetType().Name}: {ex.Message}"); }
        }

        Console.WriteLine("== 2. LoginForm behaviour (real forms, real SQLite) ==");
        LicenseManager.CurrentContext = new RuntimeCtx(); Harness.ForbidDb = false;
        Db.Init(); AuthSchema.Ensure();
        AuthService.ProvisionFirstAdmin("owner", "Store Owner", "Str0ng!Pass1");
        long ownerId = Db.Scalar<long>("SELECT Id FROM Users WHERE Username='owner'");
        Fixture("alice", "Al1ce!Pass99"); Fixture("bob", "B0b!Secret", active: false);

        var f1 = new LoginForm(); f1.Tag = null; ShowIt(f1);
        Click(f1, "btnLogin");
        Check(Err(f1) == "Please enter your username." && f1.DialogResult == DialogResult.None, "empty form -> 'Please enter your username.'");
        Type(f1, "txtUsername", "alice"); Click(f1, "btnLogin");
        Check(Err(f1) == "Please enter your password.", "username only -> 'Please enter your password.'");
        Type(f1, "txtPassword", "wrong-pass1"); Click(f1, "btnLogin");
        Check(Err(f1) == "Invalid username or password." && f1.DialogResult == DialogResult.None && f1.SignedInUser == null && F<TextBox>(f1, "txtPassword").Text == "",
              "wrong password -> friendly error, stays on the LoginForm, password box cleared");
        Type(f1, "txtUsername", "bob"); Type(f1, "txtPassword", "B0b!Secret"); Click(f1, "btnLogin");
        Check(Err(f1).Contains("deactivated") && f1.DialogResult == DialogResult.None && f1.SignedInUser == null, "INACTIVE account -> 'deactivated' message, cannot sign in");
        Type(f1, "txtUsername", "ghost"); Type(f1, "txtPassword", "whatever1"); Click(f1, "btnLogin");
        Check(Err(f1) == "Invalid username or password.", "unknown user -> same generic message");
        Type(f1, "txtPassword", "x"); Check(Err(f1) == "", "error message clears when the user types again");

        var pwd = F<TextBox>(f1, "txtPassword"); var tog = F<Button>(f1, "btnTogglePassword");
        bool masked0 = pwd.UseSystemPasswordChar; Click(f1, "btnTogglePassword"); bool masked1 = pwd.UseSystemPasswordChar; string t1 = tog.Text;
        Click(f1, "btnTogglePassword");
        Check(masked0 && !masked1 && t1 == "Hide" && pwd.UseSystemPasswordChar && tog.Text == "Show", "Show/Hide button toggles masking and its caption");

        Type(f1, "txtUsername", "alice"); Type(f1, "txtPassword", "Al1ce!Pass99"); F<CheckBox>(f1, "chkRemember").Checked = true; Click(f1, "btnLogin");
        Check(f1.DialogResult == DialogResult.OK && f1.SignedInUser?.Username == "alice" && !f1.SignedInUser.IsAdmin, "valid credentials -> DialogResult.OK with the signed-in user");
        string remFile = Path.Combine(Environment.GetEnvironmentVariable("XDG_DATA_HOME")!, "InventoryManager", "last-user.txt");
        Check(File.Exists(remFile) && File.ReadAllText(remFile) == "alice" && !File.ReadAllText(remFile).Contains("Al1ce"), "Remember me stores ONLY the username (never the password)");
        f1.Dispose();

        var f2 = new LoginForm(); f2.Tag = null; ShowIt(f2);
        Check(F<TextBox>(f2, "txtUsername").Text == "alice" && F<CheckBox>(f2, "chkRemember").Checked && F<TextBox>(f2, "txtPassword").Text == "", "next start: username pre-filled, password empty");
        F<CheckBox>(f2, "chkRemember").Checked = false; Type(f2, "txtPassword", "Al1ce!Pass99"); Click(f2, "btnLogin");
        Check(f2.DialogResult == DialogResult.OK && !File.Exists(remFile), "signing in with Remember me OFF forgets the username");
        f2.Dispose();

        Console.WriteLine("== 3. Admin-only user management form ==");
        var admin = AuthService.GetById(ownerId)!; var alice = AuthService.GetById(Db.Scalar<long>("SELECT Id FROM Users WHERE Username='alice'"))!;
        var um = new UserManagementForm(admin); um.Tag = null; ShowIt(um);
        var grid = F<DataGridView>(um, "dgvUsers");
        Check(grid.Rows.Count == 3 && !grid.Columns["Id"]!.Visible && grid.Columns["PasswordHash"] == null, $"admin sees {grid.Rows.Count} accounts; internal Id hidden; password hashes are never loaded into the grid");
        Type(um, "txtUsername", "carol"); Type(um, "txtPassword", "abc"); Harness.Messages.Clear(); Click(um, "btnCreate");
        Check(Db.Scalar<long>("SELECT COUNT(*) FROM Users") == 3 && Harness.Messages.Any(m => m.Contains("at least 8")), "weak password rejected with a friendly message");
        Click(um, "btnGenerate"); string gen = F<TextBox>(um, "txtPassword").Text;
        Click(um, "btnCreate");
        Check(Db.Scalar<long>("SELECT COUNT(*) FROM Users WHERE Username='carol'") == 1 && PasswordPolicy.Validate(gen, "carol") == null, "admin creates 'carol' with a generated temporary password");
        Check(AuthService.Authenticate("carol", gen).User?.MustChangePassword == true, "that account must change its password at first sign-in");
        grid.ClearSelection(); Harness.Messages.Clear(); Click(um, "btnToggleActive");
        Check(Harness.Messages.Any(m => m.Contains("Select a user")), "actions without a selection ask the admin to pick a user");
        var bobRow = grid.Rows.Cast<DataGridViewRow>().First(r => Convert.ToString(((DataRowView)r.DataBoundItem)["Username"]) == "bob");
        bobRow.Selected = true; Check(F<Button>(um, "btnToggleActive").Text == "Activate", "inactive user selected -> button reads 'Activate'");
        Click(um, "btnToggleActive");
        Check(Db.Scalar<long>("SELECT IsActive FROM Users WHERE Username='bob'") == 1, "admin re-activates bob from the UI");
        um.Dispose();

        Harness.Messages.Clear();
        var um2 = new UserManagementForm(alice); um2.Tag = null; ShowIt(um2);
        Check(Harness.Messages.Any(m => m.Contains("Only an active administrator")), "a NON-admin who opens the form is refused (and sees no accounts)");
        Check(F<DataGridView>(um2, "dgvUsers").Rows.Count == 0, "no account data is shown to a non-admin");
        um2.Dispose();

        Console.WriteLine("== 4. Change-password form ==");
        var cp = new ChangePasswordForm(alice, forced: true); cp.Tag = null; ShowIt(cp);
        Check(!cp.ControlBox && F<Label>(cp, "lblInfo").Text.Contains("temporary"), "forced mode: no close box, explains why");
        Type(cp, "txtCurrent", "WRONG-one1"); Type(cp, "txtNew", "N3w!Password"); Type(cp, "txtConfirm", "N3w!Password"); Click(cp, "btnOk");
        Check(Err(cp).Contains("current password is incorrect") && cp.DialogResult == DialogResult.None, "wrong current password -> error, dialog stays open");
        Type(cp, "txtCurrent", "Al1ce!Pass99"); Type(cp, "txtConfirm", "different1"); Click(cp, "btnOk");
        Check(Err(cp).Contains("do not match"), "mismatched confirmation -> error");
        Type(cp, "txtConfirm", "N3w!Password"); Click(cp, "btnOk");
        Check(cp.DialogResult == DialogResult.OK && cp.UpdatedUser != null && AuthService.Authenticate("alice", "N3w!Password").Status == LoginStatus.Success, "valid change -> OK, new password works");
        cp.Dispose();

        Console.WriteLine("== 5. First-run setup form ==");
        var d2 = DataHome("setup"); // separate process state is not possible; validate form logic on a DB that already has an admin
        var su = new FirstAdminSetupForm(); su.Tag = null; ShowIt(su);
        Type(su, "txtUsername", "x"); Click(su, "btnCreate"); Check(Err(su).StartsWith("Username must be"), "invalid username -> friendly message");
        Type(su, "txtUsername", "root1"); Type(su, "txtPassword", "weak"); Type(su, "txtConfirm", "weak"); Click(su, "btnCreate"); Check(Err(su).Contains("at least 8"), "weak password -> friendly message");
        Type(su, "txtPassword", "Str0ng!Pass1"); Type(su, "txtConfirm", "Str0ng!Pass2"); Click(su, "btnCreate"); Check(Err(su).Contains("do not match"), "mismatch -> friendly message");
        Harness.Messages.Clear(); Type(su, "txtConfirm", "Str0ng!Pass1"); Click(su, "btnCreate");
        Check(Db.Scalar<long>("SELECT COUNT(*) FROM Users WHERE Username='root1'") == 0 && Harness.Messages.Any(m => m.Contains("already exists")), "setup form cannot add an admin when one already exists");
        su.Dispose();

        Check(!Harness.Messages.Any(m => m.StartsWith("THREAD EXCEPTION")), "no unhandled exceptions were raised during the form tests");
        Console.WriteLine($"\nRESULT: {pass} passed, {fail} failed");
        return fail == 0 ? 0 : 1;
    }

    // ============================================================ full start-up flow ============================================================
    static int phase; static int wait; static string log = "";
    static T? Find<T>() where T : Form => Application.OpenForms.Cast<Form>().OfType<T>().FirstOrDefault(f => f.Visible);

    static int Flow()
    {
        string home = DataHome("flow"); Environment.SetEnvironmentVariable("XDG_DATA_HOME", home);
        LicenseManager.CurrentContext = new RuntimeCtx();
        Db.Init(); AuthSchema.Ensure();           // an existing database that has NO accounts yet
        Console.WriteLine("== StartupFlow end-to-end: first-run setup -> login -> MainForm -> logout -> login -> exit ==");
        var timer = new System.Windows.Forms.Timer { Interval = 200 };
        var watchdog = new System.Windows.Forms.Timer { Interval = 120000 };
        watchdog.Tick += (_, _) => { Console.WriteLine("  FAIL  watchdog: flow did not finish, stuck in phase " + phase); Environment.Exit(2); };
        watchdog.Start();
        timer.Tick += (_, _) => Drive();
        timer.Start();

        StartupFlow.Run();

        timer.Stop(); watchdog.Stop();
        Check(phase == 6, "flow ran through every phase and ended when MainForm was closed normally (phase " + phase + ")");
        Check(Session.Current == null && !Session.LogoutRequested, "session cleared after exit");
        Check(!Harness.Messages.Any(m => m.StartsWith("THREAD EXCEPTION") || m.StartsWith("Error")), "no errors/exceptions reported during the flow" + (Harness.Messages.Count > 0 ? " [" + string.Join(" || ", Harness.Messages) + "]" : ""));
        Console.WriteLine(log);
        Console.WriteLine($"\nRESULT: {pass} passed, {fail} failed");
        return fail == 0 ? 0 : 1;
    }

    static void Drive()
    {
        switch (phase)
        {
            case 0:   // first run: no active admin -> setup form must appear BEFORE any login
                if (Find<FirstAdminSetupForm>() is { } setup)
                {
                    Check(Find<LoginForm>() == null, "first run shows the setup form first (no login possible yet)");
                    Type(setup, "txtUsername", "owner"); Type(setup, "txtFullName", "Store Owner");
                    Type(setup, "txtPassword", "Str0ng!Pass1"); Type(setup, "txtConfirm", "Str0ng!Pass1");
                    Click(setup, "btnCreate"); phase = 1;
                    Check(Harness.Messages.Any(m => m.Contains("'owner' was created")), "administrator created from the setup form");
                }
                break;
            case 1:   // login: wrong first, then right
                if (Find<LoginForm>() is { } login)
                {
                    Type(login, "txtUsername", "owner"); Type(login, "txtPassword", "Wrong!Pass9"); Click(login, "btnLogin");
                    Check(login.Visible && login.DialogResult == DialogResult.None && Err(login) == "Invalid username or password.", "wrong password: remains on the LoginForm with an error");
                    Check(Find<MainForm>() == null, "MainForm is NOT opened after a failed login");
                    Type(login, "txtPassword", "Str0ng!Pass1"); Click(login, "btnLogin"); phase = 2; wait = 0;
                }
                break;
            case 2:   // MainForm opens after a successful login; create more users, then log out
                if (Find<MainForm>() is { } main)
                {
                    if (++wait < 8) break;                       // give the dashboard time to load its data (async)
                    var tabs = F<TabControl>(main, "tabMain");
                    Check(string.Join("|", tabs.TabPages.Cast<TabPage>().Select(p => p.Text)) == "Dashboard|Products|Suppliers|Stock Movements|Reports|Settings|Network", "existing MainForm opened with its six tabs unchanged + the new Network tab");
                    var dash = F<DashboardPanel>(main, "dashboardPanel");
                    Check(F<KpiCard>(dash, "kpiProducts").Value == "12", "existing Dashboard loaded real data (12 products) after login");
                    Check(Session.Current?.Username == "owner", "Session.Current is the signed-in admin");
                    long ownerId = Session.Current!.Id;
                    Fixture("alice", "Al1ce!Pass99"); Fixture("bob", "B0b!Secret", active: false);
                    MainFormAuthIntegration.RequestLogout(main); phase = 3;
                }
                break;
            case 3:   // after logout the LoginForm is shown again, clean
                if (Find<LoginForm>() is { } again)
                {
                    Check(F<TextBox>(again, "txtPassword").Text == "" && again.SignedInUser == null && Session.Current == null, "LOGOUT returned to a clean LoginForm (no session, empty password)");
                    Type(again, "txtUsername", "bob"); Type(again, "txtPassword", "B0b!Secret"); Click(again, "btnLogin");
                    Check(again.Visible && Err(again).Contains("deactivated") && Find<MainForm>() == null, "inactive user cannot get past the LoginForm");
                    Type(again, "txtUsername", "alice"); Type(again, "txtPassword", "Al1ce!Pass99"); Click(again, "btnLogin"); phase = 4; wait = 0;
                }
                break;
            case 4:   // regular user logs in -> MainForm, then closes the window normally
                if (Find<MainForm>() is { } main2)
                {
                    if (++wait < 3) break;
                    Check(Session.Current?.Username == "alice" && !Session.Current.IsAdmin, "a regular user can sign in and reaches the same MainForm");
                    phase = 6; main2.Close();                    // closing the window (not logging out) ends the application
                }
                break;
        }
    }
}
