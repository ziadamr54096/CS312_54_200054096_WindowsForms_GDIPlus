#nullable disable
namespace InventoryManager
{
    partial class DashboardPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tlpRoot = new System.Windows.Forms.TableLayoutPanel();
            this.tlpKpis = new System.Windows.Forms.TableLayoutPanel();
            this.kpiProducts = new InventoryManager.KpiCard();
            this.kpiUnits = new InventoryManager.KpiCard();
            this.kpiValue = new InventoryManager.KpiCard();
            this.kpiAlerts = new InventoryManager.KpiCard();
            this.barChart = new InventoryManager.BarChart();
            this.donutChart = new InventoryManager.DonutChart();
            this.tlpAlerts = new System.Windows.Forms.TableLayoutPanel();
            this.flowAlertBar = new System.Windows.Forms.FlowLayoutPanel();
            this.lblAlertsTitle = new System.Windows.Forms.Label();
            this.btnMail = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.dgvAlerts = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlerts)).BeginInit();
            this.SuspendLayout();
            this.tlpRoot.SuspendLayout();
            this.tlpKpis.SuspendLayout();
            this.tlpAlerts.SuspendLayout();
            this.flowAlertBar.SuspendLayout();
            // 
            // tlpRoot
            // 
            this.tlpRoot.Controls.Add(this.tlpKpis, 0, 0);
            this.tlpRoot.Controls.Add(this.barChart, 0, 1);
            this.tlpRoot.Controls.Add(this.donutChart, 1, 1);
            this.tlpRoot.Controls.Add(this.tlpAlerts, 0, 2);
            this.tlpRoot.SetColumnSpan(this.tlpKpis, 2);
            this.tlpRoot.SetColumnSpan(this.tlpAlerts, 2);
            this.tlpRoot.ColumnCount = 2;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62F));
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38F));
            this.tlpRoot.RowCount = 3;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 52F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.Padding = new System.Windows.Forms.Padding(8);
            this.tlpRoot.TabIndex = 0;
            this.tlpRoot.Name = "tlpRoot";
            // 
            // tlpKpis
            // 
            this.tlpKpis.Controls.Add(this.kpiProducts, 0, 0);
            this.tlpKpis.Controls.Add(this.kpiUnits, 1, 0);
            this.tlpKpis.Controls.Add(this.kpiValue, 2, 0);
            this.tlpKpis.Controls.Add(this.kpiAlerts, 3, 0);
            this.tlpKpis.ColumnCount = 4;
            this.tlpKpis.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKpis.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKpis.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKpis.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpKpis.RowCount = 1;
            this.tlpKpis.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpKpis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpKpis.TabIndex = 0;
            this.tlpKpis.Name = "tlpKpis";
            // 
            // kpiProducts
            // 
            this.kpiProducts.Accent = System.Drawing.Color.SteelBlue;
            this.kpiProducts.Caption = "Products";
            this.kpiProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kpiProducts.Margin = new System.Windows.Forms.Padding(6);
            this.kpiProducts.TabIndex = 0;
            this.kpiProducts.Name = "kpiProducts";
            // 
            // kpiUnits
            // 
            this.kpiUnits.Accent = System.Drawing.Color.SeaGreen;
            this.kpiUnits.Caption = "Units in stock";
            this.kpiUnits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kpiUnits.Margin = new System.Windows.Forms.Padding(6);
            this.kpiUnits.TabIndex = 1;
            this.kpiUnits.Name = "kpiUnits";
            // 
            // kpiValue
            // 
            this.kpiValue.Accent = System.Drawing.Color.DarkOrchid;
            this.kpiValue.Caption = "Inventory value";
            this.kpiValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kpiValue.Margin = new System.Windows.Forms.Padding(6);
            this.kpiValue.TabIndex = 2;
            this.kpiValue.Name = "kpiValue";
            // 
            // kpiAlerts
            // 
            this.kpiAlerts.Accent = System.Drawing.Color.Firebrick;
            this.kpiAlerts.Caption = "Active alerts";
            this.kpiAlerts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kpiAlerts.Margin = new System.Windows.Forms.Padding(6);
            this.kpiAlerts.TabIndex = 3;
            this.kpiAlerts.Name = "kpiAlerts";
            // 
            // barChart
            // 
            this.barChart.BackColor = System.Drawing.Color.White;
            this.barChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.barChart.Title = "Stock value by category";
            this.barChart.TabIndex = 1;
            this.barChart.Name = "barChart";
            // 
            // donutChart
            // 
            this.donutChart.BackColor = System.Drawing.Color.White;
            this.donutChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.donutChart.Title = "Stock status";
            this.donutChart.TabIndex = 2;
            this.donutChart.Name = "donutChart";
            // 
            // tlpAlerts
            // 
            this.tlpAlerts.Controls.Add(this.flowAlertBar, 0, 0);
            this.tlpAlerts.Controls.Add(this.dgvAlerts, 0, 1);
            this.tlpAlerts.ColumnCount = 1;
            this.tlpAlerts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAlerts.RowCount = 2;
            this.tlpAlerts.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAlerts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAlerts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAlerts.TabIndex = 3;
            this.tlpAlerts.Name = "tlpAlerts";
            // 
            // flowAlertBar
            // 
            this.flowAlertBar.Controls.Add(this.lblAlertsTitle);
            this.flowAlertBar.Controls.Add(this.btnMail);
            this.flowAlertBar.Controls.Add(this.lblStatus);
            this.flowAlertBar.AutoSize = true;
            this.flowAlertBar.Padding = new System.Windows.Forms.Padding(4);
            this.flowAlertBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowAlertBar.TabIndex = 0;
            this.flowAlertBar.Name = "flowAlertBar";
            // 
            // lblAlertsTitle
            // 
            this.lblAlertsTitle.AutoSize = true;
            this.lblAlertsTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlertsTitle.Margin = new System.Windows.Forms.Padding(3, 9, 12, 3);
            this.lblAlertsTitle.Text = "Alerts (low / out of stock)";
            this.lblAlertsTitle.TabIndex = 0;
            this.lblAlertsTitle.Name = "lblAlertsTitle";
            // 
            // btnMail
            // 
            this.btnMail.AutoSize = true;
            this.btnMail.Margin = new System.Windows.Forms.Padding(3);
            this.btnMail.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnMail.Text = "Send low-stock alert e-mail";
            this.btnMail.UseVisualStyleBackColor = true;
            this.btnMail.TabIndex = 1;
            this.btnMail.Name = "btnMail";
            this.btnMail.Click += new System.EventHandler(this.btnMail_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Margin = new System.Windows.Forms.Padding(8, 9, 3, 3);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Name = "lblStatus";
            // 
            // dgvAlerts
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.dgvAlerts.AllowUserToAddRows = false;
            this.dgvAlerts.AllowUserToDeleteRows = false;
            this.dgvAlerts.AllowUserToResizeRows = false;
            this.dgvAlerts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAlerts.BackgroundColor = System.Drawing.Color.White;
            this.dgvAlerts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvAlerts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvAlerts.ColumnHeadersHeight = 32;
            this.dgvAlerts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvAlerts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAlerts.EnableHeadersVisualStyles = false;
            this.dgvAlerts.MultiSelect = false;
            this.dgvAlerts.ReadOnly = true;
            this.dgvAlerts.RowHeadersVisible = false;
            this.dgvAlerts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAlerts.TabIndex = 1;
            this.dgvAlerts.Name = "dgvAlerts";
            this.dgvAlerts.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvAlerts_DataBindingComplete);
            // 
            // DashboardPanel
            // 
            this.Controls.Add(this.tlpRoot);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DashboardPanel";
            this.Size = new System.Drawing.Size(1100, 640);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAlerts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tlpRoot.ResumeLayout(false);
            this.tlpRoot.PerformLayout();
            this.tlpKpis.ResumeLayout(false);
            this.tlpKpis.PerformLayout();
            this.tlpAlerts.ResumeLayout(false);
            this.tlpAlerts.PerformLayout();
            this.flowAlertBar.ResumeLayout(false);
            this.flowAlertBar.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpRoot;
        private System.Windows.Forms.TableLayoutPanel tlpKpis;
        private InventoryManager.KpiCard kpiProducts;
        private InventoryManager.KpiCard kpiUnits;
        private InventoryManager.KpiCard kpiValue;
        private InventoryManager.KpiCard kpiAlerts;
        private InventoryManager.BarChart barChart;
        private InventoryManager.DonutChart donutChart;
        private System.Windows.Forms.TableLayoutPanel tlpAlerts;
        private System.Windows.Forms.FlowLayoutPanel flowAlertBar;
        private System.Windows.Forms.Label lblAlertsTitle;
        private System.Windows.Forms.Button btnMail;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.DataGridView dgvAlerts;
    }
}
