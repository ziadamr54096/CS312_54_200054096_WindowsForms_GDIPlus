using System.Data;
using System.Globalization;
using System.Text;

namespace InventoryManager;

/// <summary>Reports + statistics + chart, with an async CSV export that reports progress.</summary>
public partial class ReportsPanel : UserControl
{
    record ReportDef(string Name, string Sql, string ValueCol, bool Chart, bool UsesSince = false);

    static readonly ReportDef[] Defs =
    {
        new("Stock value by category",
            @"SELECT COALESCE(NULLIF(Category,''),'Uncategorized') AS Category, COUNT(*) AS Products,
                     SUM(Quantity) AS Units, ROUND(SUM(Quantity*UnitPrice),2) AS StockValue
              FROM Products GROUP BY 1 ORDER BY StockValue DESC", "StockValue", true),
        new("Reorder list (low and out of stock)",
            @"SELECT p.Sku, p.Name AS Product, p.Quantity, p.ReorderLevel,
                     MAX(p.ReorderLevel*2 - p.Quantity, 0) AS SuggestedOrder, COALESCE(s.Name,'-') AS Supplier
              FROM Products p LEFT JOIN Suppliers s ON s.Id = p.SupplierId
              WHERE p.Quantity <= p.ReorderLevel ORDER BY p.Quantity", "SuggestedOrder", false),
        new("Stock movements - last 30 days",
            @"SELECT m.CreatedAt AS Date, p.Sku, p.Name AS Product, m.Delta, m.Reason
              FROM StockMovements m JOIN Products p ON p.Id = m.ProductId
              WHERE m.CreatedAt >= @since ORDER BY m.Id DESC", "Delta", false, true),
        new("Top 10 products by stock value",
            @"SELECT p.Name AS Product, p.Quantity, p.UnitPrice, ROUND(p.Quantity*p.UnitPrice,2) AS StockValue
              FROM Products p ORDER BY StockValue DESC LIMIT 10", "StockValue", true),
        new("Supplier summary",
            @"SELECT s.Name AS Supplier, COUNT(p.Id) AS Products, COALESCE(SUM(p.Quantity),0) AS Units,
                     ROUND(COALESCE(SUM(p.Quantity*p.UnitPrice),0),2) AS StockValue
              FROM Suppliers s LEFT JOIN Products p ON p.SupplierId = s.Id
              GROUP BY s.Id ORDER BY StockValue DESC", "StockValue", true)
    };

    bool ready;

    public ReportsPanel()
    {
        InitializeComponent();   // visual layout only - see ReportsPanel.Designer.cs
    }

    // ---------- runtime initialisation (skipped inside the Visual Studio Designer) ----------

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Ui.InDesigner(this) || ready) return;

        // The report list is data-driven (Defs above), so the combo box is filled here rather than in the Designer.
        cboReport.Items.AddRange(Defs.Select(d => (object)d.Name).ToArray());
        cboReport.SelectedIndex = 0;
        ready = true;
        Db.DataChanged -= Run;
        Db.DataChanged += Run;
        Disposed -= OnPanelDisposed;
        Disposed += OnPanelDisposed;
        Run();
    }

    void OnPanelDisposed(object? sender, EventArgs e) => Db.DataChanged -= Run;

    // ---------- event handlers (wired in the Designer) ----------

    void cboReport_SelectedIndexChanged(object? sender, EventArgs e) { if (ready) Run(); }
    void btnRun_Click(object? sender, EventArgs e) => Run();

    async void btnExport_Click(object? sender, EventArgs e)
    {
        if (dgvReport.DataSource is not DataTable dt || dt.Rows.Count == 0) { Ui.Warn("Run a report that has data first."); return; }
        using var dlg = new SaveFileDialog { Filter = "CSV file (*.csv)|*.csv", FileName = $"report_{DateTime.Now:yyyyMMdd_HHmm}.csv" };
        if (dlg.ShowDialog() != DialogResult.OK) return;

        string path = dlg.FileName;
        btnExport.Enabled = false;
        progress.Value = 0;
        try
        {
            var prog = new Progress<int>(v => progress.Value = Math.Clamp(v, 0, 100));
            await Task.Run(() => WriteCsv(dt, path, prog));
            Ui.Info("Report exported to:\n" + path);
        }
        catch (Exception ex) { Ui.Error(ex); }
        finally { btnExport.Enabled = true; }
    }

    // ---------- business logic (unchanged) ----------

    void Run()
    {
        try
        {
            var def = Defs[Math.Max(cboReport.SelectedIndex, 0)];
            DataTable dt = def.UsesSince
                ? Db.Query(def.Sql, ("@since", DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)))
                : Db.Query(def.Sql);
            dgvReport.DataSource = dt;

            var vals = dt.AsEnumerable().Where(r => r[def.ValueCol] is not DBNull)
                         .Select(r => Convert.ToDouble(r[def.ValueCol])).ToList();
            lblStats.Text = vals.Count == 0
                ? "No data for this report."
                : $"Rows: {dt.Rows.Count}     Total: {vals.Sum():N2}     Average: {vals.Average():N2}     Max: {vals.Max():N2}     Min: {vals.Min():N2}";

            chart.Title = def.Chart ? def.Name : "";
            chart.SetData(def.Chart
                ? dt.AsEnumerable().Take(10).Select(r => (Convert.ToString(r[0]) ?? "", Convert.ToDouble(r[def.ValueCol])))
                : Enumerable.Empty<(string, double)>());
        }
        catch (Exception ex) { Ui.Error(ex); }
    }

    static void WriteCsv(DataTable dt, string path, IProgress<int> prog)
    {
        using var w = new StreamWriter(path, false, new UTF8Encoding(true));
        w.WriteLine(string.Join(",", dt.Columns.Cast<DataColumn>().Select(c => Esc(c.ColumnName))));
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            w.WriteLine(string.Join(",", dt.Rows[i].ItemArray.Select(v => Esc(Convert.ToString(v, CultureInfo.InvariantCulture)))));
            prog.Report((i + 1) * 100 / dt.Rows.Count);
        }
    }

    static string Esc(string? s)
    {
        s ??= "";
        return s.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0 ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
    }
}
