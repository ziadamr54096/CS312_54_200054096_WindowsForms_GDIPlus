#nullable disable
namespace InventoryManager
{
    partial class MainForm
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
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabDashboard = new System.Windows.Forms.TabPage();
            this.dashboardPanel = new InventoryManager.DashboardPanel();
            this.tabProducts = new System.Windows.Forms.TabPage();
            this.productsPanel = new InventoryManager.ProductsPanel();
            this.tabSuppliers = new System.Windows.Forms.TabPage();
            this.suppliersPanel = new InventoryManager.SuppliersPanel();
            this.tabMovements = new System.Windows.Forms.TabPage();
            this.movementsPanel = new InventoryManager.MovementsPanel();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.reportsPanel = new InventoryManager.ReportsPanel();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.settingsPanel = new InventoryManager.SettingsPanel();
            this.tabNetwork = new System.Windows.Forms.TabPage();
            this.networkPanel = new InventoryManager.NetworkPanel();
            this.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabDashboard.SuspendLayout();
            this.tabProducts.SuspendLayout();
            this.tabSuppliers.SuspendLayout();
            this.tabMovements.SuspendLayout();
            this.tabReports.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabNetwork.SuspendLayout();
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabDashboard);
            this.tabMain.Controls.Add(this.tabProducts);
            this.tabMain.Controls.Add(this.tabSuppliers);
            this.tabMain.Controls.Add(this.tabMovements);
            this.tabMain.Controls.Add(this.tabReports);
            this.tabMain.Controls.Add(this.tabSettings);
            this.tabMain.Controls.Add(this.tabNetwork);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Padding = new System.Drawing.Point(14, 6);
            this.tabMain.SelectedIndex = 0;
            this.tabMain.TabIndex = 0;
            this.tabMain.Name = "tabMain";
            // 
            // tabDashboard
            // 
            this.tabDashboard.Controls.Add(this.dashboardPanel);
            this.tabDashboard.BackColor = System.Drawing.Color.White;
            this.tabDashboard.Padding = new System.Windows.Forms.Padding(3);
            this.tabDashboard.Text = "Dashboard";
            this.tabDashboard.UseVisualStyleBackColor = false;
            this.tabDashboard.TabIndex = 0;
            this.tabDashboard.Name = "tabDashboard";
            // 
            // dashboardPanel
            // 
            this.dashboardPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dashboardPanel.TabIndex = 0;
            this.dashboardPanel.Name = "dashboardPanel";
            // 
            // tabProducts
            // 
            this.tabProducts.Controls.Add(this.productsPanel);
            this.tabProducts.BackColor = System.Drawing.Color.White;
            this.tabProducts.Padding = new System.Windows.Forms.Padding(3);
            this.tabProducts.Text = "Products";
            this.tabProducts.UseVisualStyleBackColor = false;
            this.tabProducts.TabIndex = 1;
            this.tabProducts.Name = "tabProducts";
            // 
            // productsPanel
            // 
            this.productsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productsPanel.TabIndex = 0;
            this.productsPanel.Name = "productsPanel";
            // 
            // tabSuppliers
            // 
            this.tabSuppliers.Controls.Add(this.suppliersPanel);
            this.tabSuppliers.BackColor = System.Drawing.Color.White;
            this.tabSuppliers.Padding = new System.Windows.Forms.Padding(3);
            this.tabSuppliers.Text = "Suppliers";
            this.tabSuppliers.UseVisualStyleBackColor = false;
            this.tabSuppliers.TabIndex = 2;
            this.tabSuppliers.Name = "tabSuppliers";
            // 
            // suppliersPanel
            // 
            this.suppliersPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.suppliersPanel.TabIndex = 0;
            this.suppliersPanel.Name = "suppliersPanel";
            // 
            // tabMovements
            // 
            this.tabMovements.Controls.Add(this.movementsPanel);
            this.tabMovements.BackColor = System.Drawing.Color.White;
            this.tabMovements.Padding = new System.Windows.Forms.Padding(3);
            this.tabMovements.Text = "Stock Movements";
            this.tabMovements.UseVisualStyleBackColor = false;
            this.tabMovements.TabIndex = 3;
            this.tabMovements.Name = "tabMovements";
            // 
            // movementsPanel
            // 
            this.movementsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.movementsPanel.TabIndex = 0;
            this.movementsPanel.Name = "movementsPanel";
            // 
            // tabReports
            // 
            this.tabReports.Controls.Add(this.reportsPanel);
            this.tabReports.BackColor = System.Drawing.Color.White;
            this.tabReports.Padding = new System.Windows.Forms.Padding(3);
            this.tabReports.Text = "Reports";
            this.tabReports.UseVisualStyleBackColor = false;
            this.tabReports.TabIndex = 4;
            this.tabReports.Name = "tabReports";
            // 
            // reportsPanel
            // 
            this.reportsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportsPanel.TabIndex = 0;
            this.reportsPanel.Name = "reportsPanel";
            // 
            // tabSettings
            // 
            this.tabSettings.Controls.Add(this.settingsPanel);
            this.tabSettings.BackColor = System.Drawing.Color.White;
            this.tabSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = false;
            this.tabSettings.TabIndex = 5;
            this.tabSettings.Name = "tabSettings";
            // 
            // settingsPanel
            // 
            this.settingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsPanel.TabIndex = 0;
            this.settingsPanel.Name = "settingsPanel";
            // 
            // tabNetwork
            // 
            this.tabNetwork.Controls.Add(this.networkPanel);
            this.tabNetwork.BackColor = System.Drawing.Color.White;
            this.tabNetwork.Padding = new System.Windows.Forms.Padding(3);
            this.tabNetwork.Text = "Network";
            this.tabNetwork.UseVisualStyleBackColor = false;
            this.tabNetwork.TabIndex = 6;
            this.tabNetwork.Name = "tabNetwork";
            // 
            // networkPanel
            // 
            this.networkPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.networkPanel.TabIndex = 0;
            this.networkPanel.Name = "networkPanel";
            // 
            // MainForm
            // 
            this.Controls.Add(this.tabMain);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1164, 701);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(980, 620);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Inventory Manager";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            this.tabProducts.ResumeLayout(false);
            this.tabSuppliers.ResumeLayout(false);
            this.tabMovements.ResumeLayout(false);
            this.tabReports.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.tabNetwork.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabDashboard;
        private InventoryManager.DashboardPanel dashboardPanel;
        private System.Windows.Forms.TabPage tabProducts;
        private InventoryManager.ProductsPanel productsPanel;
        private System.Windows.Forms.TabPage tabSuppliers;
        private InventoryManager.SuppliersPanel suppliersPanel;
        private System.Windows.Forms.TabPage tabMovements;
        private InventoryManager.MovementsPanel movementsPanel;
        private System.Windows.Forms.TabPage tabReports;
        private InventoryManager.ReportsPanel reportsPanel;
        private System.Windows.Forms.TabPage tabSettings;
        private InventoryManager.SettingsPanel settingsPanel;
        private System.Windows.Forms.TabPage tabNetwork;
        private InventoryManager.NetworkPanel networkPanel;
    }
}
