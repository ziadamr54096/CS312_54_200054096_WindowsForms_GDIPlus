#nullable disable
namespace InventoryManager
{
    partial class MovementsPanel
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
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpLeft = new System.Windows.Forms.TableLayoutPanel();
            this.tlpForm = new System.Windows.Forms.TableLayoutPanel();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cboProduct = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.lblQty = new System.Windows.Forms.Label();
            this.numQty = new System.Windows.Forms.NumericUpDown();
            this.lblReason = new System.Windows.Forms.Label();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.flowActions = new System.Windows.Forms.FlowLayoutPanel();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.tlpRight = new System.Windows.Forms.TableLayoutPanel();
            this.flowSearch = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.numQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.tlpLeft.SuspendLayout();
            this.tlpForm.SuspendLayout();
            this.flowActions.SuspendLayout();
            this.tlpRight.SuspendLayout();
            this.flowSearch.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.Controls.Add(this.tlpLeft, 0, 0);
            this.tlpMain.Controls.Add(this.tlpRight, 1, 0);
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 360F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.TabIndex = 0;
            this.tlpMain.Name = "tlpMain";
            // 
            // tlpLeft
            // 
            this.tlpLeft.Controls.Add(this.tlpForm, 0, 0);
            this.tlpLeft.Controls.Add(this.flowActions, 0, 1);
            this.tlpLeft.ColumnCount = 1;
            this.tlpLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLeft.RowCount = 2;
            this.tlpLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpLeft.TabIndex = 0;
            this.tlpLeft.Name = "tlpLeft";
            // 
            // tlpForm
            // 
            this.tlpForm.Controls.Add(this.lblProduct, 0, 0);
            this.tlpForm.Controls.Add(this.cboProduct, 1, 0);
            this.tlpForm.Controls.Add(this.lblType, 0, 1);
            this.tlpForm.Controls.Add(this.cboType, 1, 1);
            this.tlpForm.Controls.Add(this.lblQty, 0, 2);
            this.tlpForm.Controls.Add(this.numQty, 1, 2);
            this.tlpForm.Controls.Add(this.lblReason, 0, 3);
            this.tlpForm.Controls.Add(this.txtReason, 1, 3);
            this.tlpForm.ColumnCount = 2;
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpForm.RowCount = 4;
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpForm.AutoSize = true;
            this.tlpForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpForm.Padding = new System.Windows.Forms.Padding(8);
            this.tlpForm.TabIndex = 0;
            this.tlpForm.Name = "tlpForm";
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblProduct.Text = "Product";
            this.lblProduct.TabIndex = 0;
            this.lblProduct.Name = "lblProduct";
            // 
            // cboProduct
            // 
            this.cboProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProduct.FormattingEnabled = true;
            this.cboProduct.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboProduct.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cboProduct.TabIndex = 1;
            this.cboProduct.Name = "cboProduct";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblType.Text = "Type";
            this.lblType.TabIndex = 2;
            this.lblType.Name = "lblType";
            // 
            // cboType
            // 
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.FormattingEnabled = true;
            this.cboType.Items.AddRange(new object[] {
            "Stock in (receive)",
            "Stock out (issue)"});
            this.cboType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboType.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cboType.TabIndex = 3;
            this.cboType.Name = "cboType";
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblQty.Text = "Quantity";
            this.lblQty.TabIndex = 4;
            this.lblQty.Name = "lblQty";
            // 
            // numQty
            // 
            this.numQty.Maximum = new decimal(new int[] {100000, 0, 0, 0});
            this.numQty.Minimum = new decimal(new int[] {1, 0, 0, 0});
            this.numQty.Value = new decimal(new int[] {1, 0, 0, 0});
            this.numQty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numQty.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.numQty.TabIndex = 5;
            this.numQty.Name = "numQty";
            // 
            // lblReason
            // 
            this.lblReason.AutoSize = true;
            this.lblReason.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblReason.Text = "Reason";
            this.lblReason.TabIndex = 6;
            this.lblReason.Name = "lblReason";
            // 
            // txtReason
            // 
            this.txtReason.MaxLength = 100;
            this.txtReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReason.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtReason.TabIndex = 7;
            this.txtReason.Name = "txtReason";
            // 
            // flowActions
            // 
            this.flowActions.Controls.Add(this.btnApply);
            this.flowActions.Controls.Add(this.lblStatus);
            this.flowActions.AutoSize = true;
            this.flowActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowActions.Padding = new System.Windows.Forms.Padding(4);
            this.flowActions.WrapContents = false;
            this.flowActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowActions.TabIndex = 1;
            this.flowActions.Name = "flowActions";
            // 
            // btnApply
            // 
            this.btnApply.AutoSize = true;
            this.btnApply.Margin = new System.Windows.Forms.Padding(3);
            this.btnApply.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnApply.Text = "Apply movement";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.TabIndex = 0;
            this.btnApply.Name = "btnApply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.MaximumSize = new System.Drawing.Size(310, 0);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Name = "lblStatus";
            // 
            // tlpRight
            // 
            this.tlpRight.Controls.Add(this.flowSearch, 0, 0);
            this.tlpRight.Controls.Add(this.dgvHistory, 0, 1);
            this.tlpRight.ColumnCount = 1;
            this.tlpRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRight.RowCount = 2;
            this.tlpRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRight.TabIndex = 1;
            this.tlpRight.Name = "tlpRight";
            // 
            // flowSearch
            // 
            this.flowSearch.Controls.Add(this.lblSearch);
            this.flowSearch.Controls.Add(this.txtSearch);
            this.flowSearch.AutoSize = true;
            this.flowSearch.Padding = new System.Windows.Forms.Padding(4);
            this.flowSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowSearch.TabIndex = 0;
            this.flowSearch.Name = "flowSearch";
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Margin = new System.Windows.Forms.Padding(6, 9, 3, 3);
            this.lblSearch.Text = "Search history:";
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Name = "lblSearch";
            // 
            // txtSearch
            // 
            this.txtSearch.PlaceholderText = "Search product, SKU or reason...";
            this.txtSearch.Size = new System.Drawing.Size(280, 27);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // dgvHistory
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.AllowUserToResizeRows = false;
            this.dgvHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistory.BackgroundColor = System.Drawing.Color.White;
            this.dgvHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvHistory.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvHistory.ColumnHeadersHeight = 32;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvHistory.EnableHeadersVisualStyles = false;
            this.dgvHistory.MultiSelect = false;
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.RowHeadersVisible = false;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.TabIndex = 1;
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvHistory_CellFormatting);
            // 
            // MovementsPanel
            // 
            this.Controls.Add(this.tlpMain);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "MovementsPanel";
            this.Size = new System.Drawing.Size(1100, 640);
            ((System.ComponentModel.ISupportInitialize)(this.numQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.tlpLeft.ResumeLayout(false);
            this.tlpLeft.PerformLayout();
            this.tlpForm.ResumeLayout(false);
            this.tlpForm.PerformLayout();
            this.flowActions.ResumeLayout(false);
            this.flowActions.PerformLayout();
            this.tlpRight.ResumeLayout(false);
            this.tlpRight.PerformLayout();
            this.flowSearch.ResumeLayout(false);
            this.flowSearch.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpLeft;
        private System.Windows.Forms.TableLayoutPanel tlpForm;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.ComboBox cboProduct;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.NumericUpDown numQty;
        private System.Windows.Forms.Label lblReason;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.FlowLayoutPanel flowActions;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TableLayoutPanel tlpRight;
        private System.Windows.Forms.FlowLayoutPanel flowSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DataGridView dgvHistory;
    }
}
