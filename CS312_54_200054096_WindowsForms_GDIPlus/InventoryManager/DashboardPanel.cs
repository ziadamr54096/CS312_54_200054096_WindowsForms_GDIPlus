using System.Data;

namespace InventoryManager;

/// <summary>KPIs, GDI+ charts and the alert list. Data is loaded on a worker thread (Task.Run) so the UI stays responsive.</summary>
public partial class DashboardPanel : UserControl
{
    bool ready;

    public DashboardPanel()
    {
        InitializeComponent();   // visual layout only - see DashboardPanel.Designer.cs
    }

    // ---------- runtime initialisation (skipped inside the Visual Studio Designer) ----------

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (Ui.InDesigner(this) || ready) return;

        ready = true;
        Db.DataChanged -= RefreshData;
        Db.DataChanged += RefreshData;
        Disposed -= OnPanelDisposed;
        Disposed += OnPanelDisposed;
        RefreshData();
    }

    void OnPanelDisposed(object? sender, EventArgs e) => Db.DataChanged -= RefreshData;

    public async void RefreshData()
    {
        try
        {
            var data = await Task.Run(() => (
                kpi: Db.Query(@"SELECT COUNT(*) AS Products,
                                       COALESCE(SUM(Quantity),0) AS Units,
                                       COALESCE(SUM(Quantity*UnitPrice),0) AS StockValue,
                                       COALESCE(SUM(CASE WHEN Quantity=0 THEN 1 ELSE 0 END),0) AS OutOf,
                                       COALESCE(SUM(CASE WHEN Quantity>0 AND Quantity<=ReorderLevel THEN 1 ELSE 0 END),0) AS Low
                                FROM Products"),
                cats: Db.Query(@"SELECT COALESCE(NULLIF(Category,''),'Uncategorized') AS Category,
                                        ROUND(SUM(Quantity*UnitPrice),2) AS StockValue
                                 FROM Products GROUP BY 1 ORDER BY StockValue DESC LIMIT 8"),
                alerts: Db.Query(@"SELECT p.Sku, p.Name AS Product, p.Quantity, p.ReorderLevel AS Reorder,
                                          COALESCE(s.Name,'-') AS Supplier,
                                          CASE WHEN p.Quantity=0 THEN 'OUT OF STOCK' ELSE 'LOW' END AS Status
                                   FROM Products p LEFT JOIN Suppliers s ON s.Id = p.SupplierId
                                   WHERE p.Quantity <= p.ReorderLevel ORDER BY p.Quantity")));

            if (IsDisposed) return;
            var k = data.kpi.Rows[0];
            int products = Convert.ToInt32(k["Products"]);
            int low = Convert.ToInt32(k["Low"]);
            int outOf = Convert.ToInt32(k["OutOf"]);

            kpiProducts.Value = products.ToString("N0");
            kpiUnits.Value = Convert.ToInt64(k["Units"]).ToString("N0");
            kpiValue.Value = Convert.ToDouble(k["StockValue"]).ToString("N2");
            kpiAlerts.Value = (low + outOf).ToString("N0");

            barChart.SetData(data.cats.AsEnumerable().Select(r =>
                (Convert.ToString(r["Category"]) ?? "", Convert.ToDouble(r["StockValue"]))));
            donutChart.SetData(new (string, double, Color)[]
            {
                ("In stock", products - low - outOf, Color.SeaGreen),
                ("Low stock", low, Color.Orange),
                ("Out of stock", outOf, Color.Firebrick)
            });
            dgvAlerts.DataSource = data.alerts;
        }
        catch (Exception ex) { Ui.Error(ex); }
    }

    // ---------- event handlers (wired in the Designer) ----------

    void dgvAlerts_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        foreach (DataGridViewRow row in dgvAlerts.Rows)
            if (row.DataBoundItem is DataRowView v)
                row.DefaultCellStyle.BackColor = Convert.ToString(v["Status"]) == "OUT OF STOCK"
                    ? Color.FromArgb(255, 205, 205) : Color.FromArgb(255, 240, 190);
        dgvAlerts.ClearSelection();
    }

    async void btnMail_Click(object? sender, EventArgs e)
    {
        btnMail.Enabled = false;
        lblStatus.ForeColor = Color.DimGray;
        lblStatus.Text = "Sending...";
        try
        {
            lblStatus.Text = await AlertService.NotifyLowStockAsync();
        }
        catch (Exception ex)
        {
            lblStatus.ForeColor = Color.Firebrick;
            lblStatus.Text = "Could not send: " + ex.Message;
        }
        finally { btnMail.Enabled = true; }
    }
}
