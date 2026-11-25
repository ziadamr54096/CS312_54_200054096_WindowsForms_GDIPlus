#nullable disable
namespace InventoryManager
{
    partial class ReportsPanel
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
            this.flowBar = new System.Windows.Forms.FlowLayoutPanel();
            this.lblReport = new System.Windows.Forms.Label();
            this.cboReport = new System.Windows.Forms.ComboBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.lblStats = new System.Windows.Forms.Label();
            this.dgvReport = new System.Windows.Forms.DataGridView();
            this.chart = new InventoryManager.BarChart();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).BeginInit();
            this.SuspendLayout();
            this.tlpRoot.SuspendLayout();
            this.flowBar.SuspendLayout();
            // 
            // tlpRoot
            // 
            this.tlpRoot.Controls.Add(this.flowBar, 0, 0);
            this.tlpRoot.Controls.Add(this.lblStats, 0, 1);
            this.tlpRoot.Controls.Add(this.dgvReport, 0, 2);
            this.tlpRoot.Controls.Add(this.chart, 0, 3);
            this.tlpRoot.ColumnCount = 1;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.RowCount = 4;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.TabIndex = 0;
            this.tlpRoot.Name = "tlpRoot";
            // 
            // flowBar
            // 
            this.flowBar.Controls.Add(this.lblReport);
            this.flowBar.Controls.Add(this.cboReport);
            this.flowBar.Controls.Add(this.btnRun);
            this.flowBar.Controls.Add(this.btnExport);
            this.flowBar.Controls.Add(this.progress);
            this.flowBar.AutoSize = true;
            this.flowBar.Padding = new System.Windows.Forms.Padding(4);
            this.flowBar.TabIndex = 0;
            this.flowBar.Name = "flowBar";
            // 
            // lblReport
            // 
            this.lblReport.AutoSize = true;
            this.lblReport.Margin = new System.Windows.Forms.Padding(6, 9, 3, 3);
            this.lblReport.Text = "Report:";
            this.lblReport.TabIndex = 0;
            this.lblReport.Name = "lblReport";
            // 
            // cboReport
            // 
            this.cboReport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReport.FormattingEnabled = true;
            this.cboReport.Size = new System.Drawing.Size(280, 28);
            this.cboReport.TabIndex = 1;
            this.cboReport.Name = "cboReport";
            this.cboReport.SelectedIndexChanged += new System.EventHandler(this.cboReport_SelectedIndexChanged);
            // 
            // btnRun
            // 
            this.btnRun.AutoSize = true;
            this.btnRun.Margin = new System.Windows.Forms.Padding(3);
            this.btnRun.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.TabIndex = 2;
            this.btnRun.Name = "btnRun";
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnExport
            // 
            this.btnExport.AutoSize = true;
            this.btnExport.Margin = new System.Windows.Forms.Padding(3);
            this.btnExport.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnExport.Text = "Export CSV";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.TabIndex = 3;
            this.btnExport.Name = "btnExport";
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // progress
            // 
            this.progress.Margin = new System.Windows.Forms.Padding(8, 9, 3, 3);
            this.progress.Size = new System.Drawing.Size(120, 18);
            this.progress.TabIndex = 4;
            this.progress.Name = "progress";
            // 
            // lblStats
            // 
            this.lblStats.AutoSize = true;
            this.lblStats.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStats.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.lblStats.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.lblStats.TabIndex = 1;
            this.lblStats.Name = "lblStats";
            // 
            // dgvReport
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.dgvReport.AllowUserToAddRows = false;
            this.dgvReport.AllowUserToDeleteRows = false;
            this.dgvReport.AllowUserToResizeRows = false;
            this.dgvReport.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReport.BackgroundColor = System.Drawing.Color.White;
            this.dgvReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvReport.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvReport.ColumnHeadersHeight = 32;
            this.dgvReport.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvReport.EnableHeadersVisualStyles = false;
            this.dgvReport.MultiSelect = false;
            this.dgvReport.ReadOnly = true;
            this.dgvReport.RowHeadersVisible = false;
            this.dgvReport.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvReport.TabIndex = 2;
            this.dgvReport.Name = "dgvReport";
            // 
            // chart
            // 
            this.chart.BackColor = System.Drawing.Color.White;
            this.chart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chart.TabIndex = 3;
            this.chart.Name = "chart";
            // 
            // ReportsPanel
            // 
            this.Controls.Add(this.tlpRoot);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ReportsPanel";
            this.Size = new System.Drawing.Size(1100, 640);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReport)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tlpRoot.ResumeLayout(false);
            this.tlpRoot.PerformLayout();
            this.flowBar.ResumeLayout(false);
            this.flowBar.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpRoot;
        private System.Windows.Forms.FlowLayoutPanel flowBar;
        private System.Windows.Forms.Label lblReport;
        private System.Windows.Forms.ComboBox cboReport;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.DataGridView dgvReport;
        private InventoryManager.BarChart chart;
    }
}
