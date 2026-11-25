#nullable disable
namespace InventoryManager
{
    partial class ProductsPanel
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
            this.lblSku = new System.Windows.Forms.Label();
            this.txtSku = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cboCategory = new System.Windows.Forms.ComboBox();
            this.lblSupplier = new System.Windows.Forms.Label();
            this.cboSupplier = new System.Windows.Forms.ComboBox();
            this.lblPrice = new System.Windows.Forms.Label();
            this.numPrice = new System.Windows.Forms.NumericUpDown();
            this.lblQty = new System.Windows.Forms.Label();
            this.numQty = new System.Windows.Forms.NumericUpDown();
            this.lblReorder = new System.Windows.Forms.Label();
            this.numReorder = new System.Windows.Forms.NumericUpDown();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.tlpRight = new System.Windows.Forms.TableLayoutPanel();
            this.flowSearch = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblShow = new System.Windows.Forms.Label();
            this.cboFilter = new System.Windows.Forms.ComboBox();
            this.dgvProducts = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).BeginInit();
            this.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.tlpLeft.SuspendLayout();
            this.tlpForm.SuspendLayout();
            this.flowButtons.SuspendLayout();
            this.tlpRight.SuspendLayout();
            this.flowSearch.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.Controls.Add(this.tlpLeft, 0, 0);
            this.tlpMain.Controls.Add(this.tlpRight, 1, 0);
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 340F));
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
            this.tlpLeft.Controls.Add(this.flowButtons, 0, 1);
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
            this.tlpForm.Controls.Add(this.lblSku, 0, 0);
            this.tlpForm.Controls.Add(this.txtSku, 1, 0);
            this.tlpForm.Controls.Add(this.lblName, 0, 1);
            this.tlpForm.Controls.Add(this.txtName, 1, 1);
            this.tlpForm.Controls.Add(this.lblCategory, 0, 2);
            this.tlpForm.Controls.Add(this.cboCategory, 1, 2);
            this.tlpForm.Controls.Add(this.lblSupplier, 0, 3);
            this.tlpForm.Controls.Add(this.cboSupplier, 1, 3);
            this.tlpForm.Controls.Add(this.lblPrice, 0, 4);
            this.tlpForm.Controls.Add(this.numPrice, 1, 4);
            this.tlpForm.Controls.Add(this.lblQty, 0, 5);
            this.tlpForm.Controls.Add(this.numQty, 1, 5);
            this.tlpForm.Controls.Add(this.lblReorder, 0, 6);
            this.tlpForm.Controls.Add(this.numReorder, 1, 6);
            this.tlpForm.ColumnCount = 2;
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpForm.RowCount = 7;
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpForm.RowStyles.Add(new System.Windows.Forms.RowStyle());
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
            // lblSku
            // 
            this.lblSku.AutoSize = true;
            this.lblSku.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblSku.Text = "SKU";
            this.lblSku.TabIndex = 0;
            this.lblSku.Name = "lblSku";
            // 
            // txtSku
            // 
            this.txtSku.MaxLength = 20;
            this.txtSku.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSku.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtSku.TabIndex = 1;
            this.txtSku.Name = "txtSku";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblName.Text = "Name";
            this.lblName.TabIndex = 2;
            this.lblName.Name = "lblName";
            // 
            // txtName
            // 
            this.txtName.MaxLength = 80;
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtName.TabIndex = 3;
            this.txtName.Name = "txtName";
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblCategory.Text = "Category";
            this.lblCategory.TabIndex = 4;
            this.lblCategory.Name = "lblCategory";
            // 
            // cboCategory
            // 
            this.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cboCategory.FormattingEnabled = true;
            this.cboCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboCategory.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cboCategory.TabIndex = 5;
            this.cboCategory.Name = "cboCategory";
            // 
            // lblSupplier
            // 
            this.lblSupplier.AutoSize = true;
            this.lblSupplier.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblSupplier.Text = "Supplier";
            this.lblSupplier.TabIndex = 6;
            this.lblSupplier.Name = "lblSupplier";
            // 
            // cboSupplier
            // 
            this.cboSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSupplier.FormattingEnabled = true;
            this.cboSupplier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboSupplier.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cboSupplier.TabIndex = 7;
            this.cboSupplier.Name = "cboSupplier";
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblPrice.Text = "Unit price";
            this.lblPrice.TabIndex = 8;
            this.lblPrice.Name = "lblPrice";
            // 
            // numPrice
            // 
            this.numPrice.DecimalPlaces = 2;
            this.numPrice.Increment = new decimal(new int[] {5, 0, 0, 65536});
            this.numPrice.Maximum = new decimal(new int[] {1000000, 0, 0, 0});
            this.numPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numPrice.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.numPrice.TabIndex = 9;
            this.numPrice.Name = "numPrice";
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblQty.Text = "Quantity";
            this.lblQty.TabIndex = 10;
            this.lblQty.Name = "lblQty";
            // 
            // numQty
            // 
            this.numQty.Maximum = new decimal(new int[] {1000000, 0, 0, 0});
            this.numQty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numQty.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.numQty.TabIndex = 11;
            this.numQty.Name = "numQty";
            // 
            // lblReorder
            // 
            this.lblReorder.AutoSize = true;
            this.lblReorder.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblReorder.Text = "Reorder level";
            this.lblReorder.TabIndex = 12;
            this.lblReorder.Name = "lblReorder";
            // 
            // numReorder
            // 
            this.numReorder.Maximum = new decimal(new int[] {1000000, 0, 0, 0});
            this.numReorder.Value = new decimal(new int[] {5, 0, 0, 0});
            this.numReorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numReorder.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.numReorder.TabIndex = 13;
            this.numReorder.Name = "numReorder";
            // 
            // flowButtons
            // 
            this.flowButtons.Controls.Add(this.btnAdd);
            this.flowButtons.Controls.Add(this.btnUpdate);
            this.flowButtons.Controls.Add(this.btnDelete);
            this.flowButtons.Controls.Add(this.btnClear);
            this.flowButtons.AutoSize = true;
            this.flowButtons.Padding = new System.Windows.Forms.Padding(4);
            this.flowButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowButtons.TabIndex = 1;
            this.flowButtons.Name = "flowButtons";
            // 
            // btnAdd
            // 
            this.btnAdd.AutoSize = true;
            this.btnAdd.Margin = new System.Windows.Forms.Padding(3);
            this.btnAdd.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.AutoSize = true;
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(3);
            this.btnUpdate.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.AutoSize = true;
            this.btnDelete.Margin = new System.Windows.Forms.Padding(3);
            this.btnDelete.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.Margin = new System.Windows.Forms.Padding(3);
            this.btnClear.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.TabIndex = 3;
            this.btnClear.Name = "btnClear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tlpRight
            // 
            this.tlpRight.Controls.Add(this.flowSearch, 0, 0);
            this.tlpRight.Controls.Add(this.dgvProducts, 0, 1);
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
            this.flowSearch.Controls.Add(this.lblShow);
            this.flowSearch.Controls.Add(this.cboFilter);
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
            this.lblSearch.Text = "Search:";
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Name = "lblSearch";
            // 
            // txtSearch
            // 
            this.txtSearch.PlaceholderText = "Search name, SKU or category...";
            this.txtSearch.Size = new System.Drawing.Size(260, 27);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // lblShow
            // 
            this.lblShow.AutoSize = true;
            this.lblShow.Margin = new System.Windows.Forms.Padding(6, 9, 3, 3);
            this.lblShow.Text = "Show:";
            this.lblShow.TabIndex = 2;
            this.lblShow.Name = "lblShow";
            // 
            // cboFilter
            // 
            this.cboFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilter.FormattingEnabled = true;
            this.cboFilter.Items.AddRange(new object[] {
            "All items",
            "Low stock",
            "Out of stock"});
            this.cboFilter.Size = new System.Drawing.Size(130, 28);
            this.cboFilter.TabIndex = 3;
            this.cboFilter.Name = "cboFilter";
            this.cboFilter.SelectedIndexChanged += new System.EventHandler(this.cboFilter_SelectedIndexChanged);
            // 
            // dgvProducts
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.dgvProducts.AllowUserToAddRows = false;
            this.dgvProducts.AllowUserToDeleteRows = false;
            this.dgvProducts.AllowUserToResizeRows = false;
            this.dgvProducts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProducts.BackgroundColor = System.Drawing.Color.White;
            this.dgvProducts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvProducts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvProducts.ColumnHeadersHeight = 32;
            this.dgvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProducts.EnableHeadersVisualStyles = false;
            this.dgvProducts.MultiSelect = false;
            this.dgvProducts.ReadOnly = true;
            this.dgvProducts.RowHeadersVisible = false;
            this.dgvProducts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProducts.TabIndex = 1;
            this.dgvProducts.Name = "dgvProducts";
            this.dgvProducts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProducts_CellClick);
            this.dgvProducts.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvProducts_KeyUp);
            this.dgvProducts.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvProducts_DataBindingComplete);
            // 
            // ProductsPanel
            // 
            this.Controls.Add(this.tlpMain);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ProductsPanel";
            this.Size = new System.Drawing.Size(1100, 640);
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReorder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProducts)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.tlpLeft.ResumeLayout(false);
            this.tlpLeft.PerformLayout();
            this.tlpForm.ResumeLayout(false);
            this.tlpForm.PerformLayout();
            this.flowButtons.ResumeLayout(false);
            this.flowButtons.PerformLayout();
            this.tlpRight.ResumeLayout(false);
            this.tlpRight.PerformLayout();
            this.flowSearch.ResumeLayout(false);
            this.flowSearch.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpLeft;
        private System.Windows.Forms.TableLayoutPanel tlpForm;
        private System.Windows.Forms.Label lblSku;
        private System.Windows.Forms.TextBox txtSku;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cboCategory;
        private System.Windows.Forms.Label lblSupplier;
        private System.Windows.Forms.ComboBox cboSupplier;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.NumericUpDown numPrice;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.NumericUpDown numQty;
        private System.Windows.Forms.Label lblReorder;
        private System.Windows.Forms.NumericUpDown numReorder;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TableLayoutPanel tlpRight;
        private System.Windows.Forms.FlowLayoutPanel flowSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblShow;
        private System.Windows.Forms.ComboBox cboFilter;
        private System.Windows.Forms.DataGridView dgvProducts;
    }
}
