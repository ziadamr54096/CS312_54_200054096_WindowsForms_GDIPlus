using System.ComponentModel;
using System.Reflection;
using System.Drawing.Imaging;
using InventoryManager;

class RegDesignCtx : LicenseContext { public override LicenseUsageMode UsageMode => LicenseUsageMode.Designtime; }
class RegRuntimeCtx : LicenseContext { public override LicenseUsageMode UsageMode => LicenseUsageMode.Runtime; }
class FakeSite : ISite
{
    public FakeSite(IComponent c) { Component = c; }
    public IComponent Component { get; }
    public IContainer? Container => null;
    public bool DesignMode => true;
    public string? Name { get; set; } = "x";
    public object? GetService(Type t) => null;
}

public static class RegressionTests
{
    static int fails, passes;
    static void Check(bool ok, string msg) { Console.WriteLine((ok ? "  PASS  " : "  FAIL  ") + msg); if (ok) passes++; else fails++; }
    const BindingFlags Inst = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly;

    static IEnumerable<Control> All(Control c) { foreach (Control k in c.Controls) { yield return k; foreach (var d in All(k)) yield return d; } }

    static int DataChangedCount()
    {
        var f = typeof(Db).GetField("DataChanged", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)!;
        var d = (Delegate?)f.GetValue(null);
        return d == null ? 0 : d.GetInvocationList().Length;
    }

    static int Blue(Control c)
    {
        using var bmp = new Bitmap(420, 260); using var g = Graphics.FromImage(bmp);
        g.Clear(Color.White);
        c.Size = new Size(420, 260);
        typeof(Control).GetMethod("OnPaint", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(c, new object[] { new PaintEventArgs(g, new Rectangle(0, 0, 420, 260)) });
        int n = 0;
        for (int x = 0; x < bmp.Width; x += 2) for (int y = 0; y < bmp.Height; y += 2) { var p = bmp.GetPixel(x, y); if (Math.Max(p.R, Math.Max(p.G, p.B)) - Math.Min(p.R, Math.Min(p.G, p.B)) > 70) n++; }
        return n;
    }

    [STAThread]
    public static int Main()
    {
        Environment.SetEnvironmentVariable("XDG_DATA_HOME", "/tmp/vauth/data_regr_" + Guid.NewGuid().ToString("N").Substring(0, 6));
        Application.EnableVisualStyles();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => Harness.Messages.Add("STUB-DB artefact: " + e.Exception.Message);
        var panels = new[] { typeof(DashboardPanel), typeof(ProductsPanel), typeof(SuppliersPanel), typeof(MovementsPanel), typeof(ReportsPanel), typeof(SettingsPanel) };

        Console.WriteLine("== 1. DESIGN-TIME simulation (LicenseUsageMode.Designtime, any database access throws) ==");
        LicenseManager.CurrentContext = new RegDesignCtx();
        Harness.ForbidDb = true;
        foreach (var t in panels.Concat(new[] { typeof(MainForm) }))
        {
            try
            {
                var c = (Control)Activator.CreateInstance(t)!;
                var fields = t.GetFields(Inst).Where(f => typeof(Control).IsAssignableFrom(f.FieldType)).ToList();
                int nullCount = fields.Count(f => f.GetValue(c) == null);
                int orphan = fields.Count(f => f.GetValue(c) is Control k && k.Parent == null);
                Check(fields.Count > 0 && nullCount == 0, $"{t.Name}: InitializeComponent created all {fields.Count} designer fields (null: {nullCount})");
                Check(orphan == 0, $"{t.Name}: every designer control is placed in the control tree (orphans: {orphan})");
                c.CreateControl();                                   // runs OnLoad
                Check(!Harness.DbTouched, $"{t.Name}: OnLoad at design time did not touch the database");
                Check(Ui.InDesigner(c), $"{t.Name}: Ui.InDesigner() is true");
                c.Dispose();
            }
            catch (Exception ex) { Check(false, $"{t.Name}: exception {ex.GetType().Name}: {ex.Message}"); }
        }

        Console.WriteLine("== 2. Custom controls drawn at design time ==");
        foreach (var t in new[] { typeof(BarChart), typeof(DonutChart) })
        {
            var plain = (Control)Activator.CreateInstance(t)!;
            int blank = Blue(plain);
            var sited = (Control)Activator.CreateInstance(t)!;
            sited.Site = new FakeSite(sited);
            int sample = Blue(sited);
            Check(blank < 20 && sample > 200, $"{t.Name}: empty at runtime w/o data (coloured px {blank}); sample data drawn when sited in designer (coloured px {sample})");
        }
        var kpi = new KpiCard { Caption = "Products", Value = "12", Accent = Color.SteelBlue };
        Check(Blue(kpi) > 0, "KpiCard: paints with Caption/Value/Accent properties");

        Console.WriteLine("== 3. RUNTIME path (stub database): load once, no duplicate subscriptions, unsubscribe on dispose ==");
        LicenseManager.CurrentContext = new RegRuntimeCtx();
        Harness.ForbidDb = false; Harness.DbTouched = false; Db.Init();
        int baseline = DataChanged_Baseline();
        foreach (var t in new[] { typeof(DashboardPanel), typeof(ProductsPanel), typeof(SuppliersPanel), typeof(MovementsPanel), typeof(ReportsPanel) })
        {
            var c = (Control)Activator.CreateInstance(t)!;
            c.CreateControl();
            var onLoad = t.GetMethod("OnLoad", BindingFlags.Instance | BindingFlags.NonPublic)!;
            onLoad.Invoke(c, new object[] { EventArgs.Empty });     // simulate a second Load
            onLoad.Invoke(c, new object[] { EventArgs.Empty });
            Thread.Sleep(400); Application.DoEvents();           // let the Dashboard's Task.Run worker reach the database
            Check(DataChangedCount() == baseline + 1, $"{t.Name}: exactly one Db.DataChanged subscription after 3 OnLoad calls ({DataChangedCount() - baseline})");
            Check(Harness.DbTouched, $"{t.Name}: runtime OnLoad does access the database");
            c.Dispose();
            Check(DataChangedCount() == baseline, $"{t.Name}: subscription removed on Dispose");
        }
        var sp = new SettingsPanel(); sp.CreateControl();
        var txtHost = (TextBox)typeof(SettingsPanel).GetField("txtHost", Inst)!.GetValue(sp)!;
        var numPort = (NumericUpDown)typeof(SettingsPanel).GetField("numPort", Inst)!.GetValue(sp)!;
        Check(txtHost.Text == "smtp.gmail.com" && numPort.Value == 587, $"SettingsPanel: runtime load fills defaults (host '{txtHost.Text}', port {numPort.Value})");

        var mf = new MainForm(); mf.CreateControl();
        var tabs = (TabControl)typeof(MainForm).GetField("tabMain", Inst)!.GetValue(mf)!;
        Check(string.Join("|", tabs.TabPages.Cast<TabPage>().Select(p => p.Text)) == "Dashboard|Products|Suppliers|Stock Movements|Reports|Settings|Network", "MainForm: tab names/order preserved");
        Check(tabs.TabPages.Cast<TabPage>().All(p => p.Controls.Count == 1 && p.Controls[0].Dock == DockStyle.Fill), "MainForm: every tab hosts exactly one docked screen");

        Console.WriteLine($"\nRESULT: {passes} passed, {fails} failed");
        return fails == 0 ? 0 : 1;
    }
    static int DataChanged_Baseline() => DataChangedCount();
}
