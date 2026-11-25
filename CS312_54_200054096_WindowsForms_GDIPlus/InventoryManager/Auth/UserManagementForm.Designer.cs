#nullable disable
namespace InventoryManager.Auth
{
    partial class UserManagementForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.tlpActions = new System.Windows.Forms.TableLayoutPanel();
            this.tlpForm = new System.Windows.Forms.TableLayoutPanel();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblRole = new System.Windows.Forms.Label();
            this.cboRole = new System.Windows.Forms.ComboBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.tlpPassword = new System.Windows.Forms.TableLayoutPanel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.chkMustChange = new System.Windows.Forms.CheckBox();
            this.chkShow = new System.Windows.Forms.CheckBox();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnToggleActive = new System.Windows.Forms.Button();
            this.btnSetRole = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.SuspendLayout();
            this.tlpRoot.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.tlpActions.SuspendLayout();
            this.tlpForm.SuspendLayout();
            this.tlpPassword.SuspendLayout();
            this.flowButtons.SuspendLayout();
            // 
            // tlpRoot
            // 
            this.tlpRoot.Controls.Add(this.lblTitle, 0, 0);
            this.tlpRoot.Controls.Add(this.dgvUsers, 0, 1);
            this.tlpRoot.Controls.Add(this.grpActions, 0, 2);
            this.tlpRoot.ColumnCount = 1;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.RowCount = 3;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 310F));
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.Padding = new System.Windows.Forms.Padding(12);
            this.tlpRoot.TabIndex = 0;
            this.tlpRoot.Name = "tlpRoot";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.lblTitle.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
            this.lblTitle.Text = "User accounts";
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Name = "lblTitle";
            // 
            // dgvUsers
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.dgvUsers.AllowUserToAddRows = false;
            this.dgvUsers.AllowUserToDeleteRows = false;
            this.dgvUsers.AllowUserToResizeRows = false;
            this.dgvUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsers.BackgroundColor = System.Drawing.Color.White;
            this.dgvUsers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvUsers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvUsers.ColumnHeadersHeight = 32;
            this.dgvUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUsers.EnableHeadersVisualStyles = false;
            this.dgvUsers.MultiSelect = false;
            this.dgvUsers.ReadOnly = true;
            this.dgvUsers.RowHeadersVisible = false;
            this.dgvUsers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsers.TabIndex = 1;
            this.dgvUsers.Name = "dgvUsers";
            this.dgvUsers.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvUsers_CellFormatting);
            this.dgvUsers.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvUsers_DataBindingComplete);
            this.dgvUsers.SelectionChanged += new System.EventHandler(this.dgvUsers_SelectionChanged);
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.tlpActions);
            this.grpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpActions.Padding = new System.Windows.Forms.Padding(10);
            this.grpActions.Text = "Create a user, or manage the selected user";
            this.grpActions.TabIndex = 2;
            this.grpActions.Name = "grpActions";
            // 
            // tlpActions
            // 
            this.tlpActions.Controls.Add(this.tlpForm, 0, 0);
            this.tlpActions.Controls.Add(this.flowButtons, 1, 0);
            this.tlpActions.ColumnCount = 2;
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpActions.RowCount = 1;
            this.tlpActions.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpActions.AutoSize = true;
            this.tlpActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpActions.TabIndex = 0;
            this.tlpActions.Name = "tlpActions";
            // 
            // tlpForm
            // 
            this.tlpForm.Controls.Add(this.lblUsername, 0, 0);
            this.tlpForm.Controls.Add(this.txtUsername, 1, 0);
            this.tlpForm.Controls.Add(this.lblFullName, 0, 1);
            this.tlpForm.Controls.Add(this.txtFullName, 1, 1);
            this.tlpForm.Controls.Add(this.lblRole, 0, 2);
            this.tlpForm.Controls.Add(this.cboRole, 1, 2);
            this.tlpForm.Controls.Add(this.lblPassword, 0, 3);
            this.tlpForm.Controls.Add(this.tlpPassword, 1, 3);
            this.tlpForm.Controls.Add(this.chkMustChange, 1, 4);
            this.tlpForm.Controls.Add(this.chkShow, 1, 5);
            this.tlpForm.ColumnCount = 2;
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpForm.RowCount = 6;
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
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblUsername.Text = "Username";
            this.lblUsername.TabIndex = 0;
            this.lblUsername.Name = "lblUsername";
            // 
            // txtUsername
            // 
            this.txtUsername.MaxLength = 32;
            this.txtUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUsername.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtUsername.TabIndex = 1;
            this.txtUsername.Name = "txtUsername";
            // 
            // lblFullName
            // 
            this.lblFullName.AutoSize = true;
            this.lblFullName.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblFullName.Text = "Full name";
            this.lblFullName.TabIndex = 2;
            this.lblFullName.Name = "lblFullName";
            // 
            // txtFullName
            // 
            this.txtFullName.MaxLength = 80;
            this.txtFullName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFullName.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtFullName.TabIndex = 3;
            this.txtFullName.Name = "txtFullName";
            // 
            // lblRole
            // 
            this.lblRole.AutoSize = true;
            this.lblRole.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblRole.Text = "Role";
            this.lblRole.TabIndex = 4;
            this.lblRole.Name = "lblRole";
            // 
            // cboRole
            // 
            this.cboRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRole.FormattingEnabled = true;
            this.cboRole.Items.AddRange(new object[] {
            "User",
            "Admin"});
            this.cboRole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboRole.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.cboRole.TabIndex = 5;
            this.cboRole.Name = "cboRole";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblPassword.Text = "Password";
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Name = "lblPassword";
            // 
            // tlpPassword
            // 
            this.tlpPassword.Controls.Add(this.txtPassword, 0, 0);
            this.tlpPassword.Controls.Add(this.btnGenerate, 1, 0);
            this.tlpPassword.ColumnCount = 2;
            this.tlpPassword.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPassword.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPassword.RowCount = 1;
            this.tlpPassword.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpPassword.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.tlpPassword.TabIndex = 7;
            this.tlpPassword.Name = "tlpPassword";
            // 
            // txtPassword
            // 
            this.txtPassword.MaxLength = 128;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPassword.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.txtPassword.TabIndex = 0;
            this.txtPassword.Name = "txtPassword";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(0);
            this.btnGenerate.Size = new System.Drawing.Size(86, 29);
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.TabIndex = 1;
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // chkMustChange
            // 
            this.chkMustChange.AutoSize = true;
            this.chkMustChange.Margin = new System.Windows.Forms.Padding(3, 0, 3, 2);
            this.chkMustChange.Checked = true;
            this.chkMustChange.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMustChange.Text = "Require a password change at next sign-in";
            this.chkMustChange.UseVisualStyleBackColor = true;
            this.chkMustChange.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkMustChange.TabIndex = 8;
            this.chkMustChange.Name = "chkMustChange";
            // 
            // chkShow
            // 
            this.chkShow.AutoSize = true;
            this.chkShow.Margin = new System.Windows.Forms.Padding(3, 0, 3, 2);
            this.chkShow.Text = "Show password";
            this.chkShow.UseVisualStyleBackColor = true;
            this.chkShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkShow.TabIndex = 9;
            this.chkShow.Name = "chkShow";
            this.chkShow.CheckedChanged += new System.EventHandler(this.chkShow_CheckedChanged);
            // 
            // flowButtons
            // 
            this.flowButtons.Controls.Add(this.btnCreate);
            this.flowButtons.Controls.Add(this.btnReset);
            this.flowButtons.Controls.Add(this.btnToggleActive);
            this.flowButtons.Controls.Add(this.btnSetRole);
            this.flowButtons.Controls.Add(this.btnClose);
            this.flowButtons.AutoSize = true;
            this.flowButtons.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowButtons.Padding = new System.Windows.Forms.Padding(4);
            this.flowButtons.WrapContents = false;
            this.flowButtons.TabIndex = 1;
            this.flowButtons.Name = "flowButtons";
            // 
            // btnCreate
            // 
            this.btnCreate.AutoSize = true;
            this.btnCreate.Margin = new System.Windows.Forms.Padding(3);
            this.btnCreate.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnCreate.Text = "Create user";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.TabIndex = 0;
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnReset
            // 
            this.btnReset.AutoSize = true;
            this.btnReset.Margin = new System.Windows.Forms.Padding(3);
            this.btnReset.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnReset.Text = "Reset password";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.TabIndex = 1;
            this.btnReset.Name = "btnReset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnToggleActive
            // 
            this.btnToggleActive.AutoSize = true;
            this.btnToggleActive.Margin = new System.Windows.Forms.Padding(3);
            this.btnToggleActive.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnToggleActive.Text = "Deactivate";
            this.btnToggleActive.UseVisualStyleBackColor = true;
            this.btnToggleActive.TabIndex = 2;
            this.btnToggleActive.Name = "btnToggleActive";
            this.btnToggleActive.Click += new System.EventHandler(this.btnToggleActive_Click);
            // 
            // btnSetRole
            // 
            this.btnSetRole.AutoSize = true;
            this.btnSetRole.Margin = new System.Windows.Forms.Padding(3);
            this.btnSetRole.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnSetRole.Text = "Change role";
            this.btnSetRole.UseVisualStyleBackColor = true;
            this.btnSetRole.TabIndex = 3;
            this.btnSetRole.Name = "btnSetRole";
            this.btnSetRole.Click += new System.EventHandler(this.btnSetRole_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Margin = new System.Windows.Forms.Padding(3);
            this.btnClose.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnClose.AutoSize = true;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.TabIndex = 4;
            this.btnClose.Name = "btnClose";
            // 
            // UserManagementForm
            // 
            this.Controls.Add(this.tlpRoot);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(980, 680);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "UserManagementForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Stockora - Manage users";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(860, 600);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tlpRoot.ResumeLayout(false);
            this.tlpRoot.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.tlpActions.ResumeLayout(false);
            this.tlpActions.PerformLayout();
            this.tlpForm.ResumeLayout(false);
            this.tlpForm.PerformLayout();
            this.tlpPassword.ResumeLayout(false);
            this.tlpPassword.PerformLayout();
            this.flowButtons.ResumeLayout(false);
            this.flowButtons.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpRoot;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridView dgvUsers;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.TableLayoutPanel tlpActions;
        private System.Windows.Forms.TableLayoutPanel tlpForm;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblRole;
        private System.Windows.Forms.ComboBox cboRole;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TableLayoutPanel tlpPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.CheckBox chkMustChange;
        private System.Windows.Forms.CheckBox chkShow;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnToggleActive;
        private System.Windows.Forms.Button btnSetRole;
        private System.Windows.Forms.Button btnClose;
    }
}
