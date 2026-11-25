#nullable disable
namespace InventoryManager.Auth
{
    partial class FirstAdminSetupForm
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
            this.tlpRoot = new System.Windows.Forms.TableLayoutPanel();
            this.brandHeader = new InventoryManager.Auth.BrandHeader();
            this.tlpBody = new System.Windows.Forms.TableLayoutPanel();
            this.lblIntro = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblFullName = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblConfirm = new System.Windows.Forms.Label();
            this.txtConfirm = new System.Windows.Forms.TextBox();
            this.chkShow = new System.Windows.Forms.CheckBox();
            this.lblPolicy = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            this.tlpRoot.SuspendLayout();
            this.tlpBody.SuspendLayout();
            this.flowButtons.SuspendLayout();
            // 
            // tlpRoot
            // 
            this.tlpRoot.Controls.Add(this.brandHeader, 0, 0);
            this.tlpRoot.Controls.Add(this.tlpBody, 0, 1);
            this.tlpRoot.ColumnCount = 1;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.RowCount = 2;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.TabIndex = 0;
            this.tlpRoot.Name = "tlpRoot";
            // 
            // brandHeader
            // 
            this.brandHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brandHeader.TabIndex = 0;
            this.brandHeader.Name = "brandHeader";
            // 
            // tlpBody
            // 
            this.tlpBody.Controls.Add(this.lblIntro, 0, 0);
            this.tlpBody.Controls.Add(this.lblUsername, 0, 1);
            this.tlpBody.Controls.Add(this.txtUsername, 0, 2);
            this.tlpBody.Controls.Add(this.lblFullName, 0, 3);
            this.tlpBody.Controls.Add(this.txtFullName, 0, 4);
            this.tlpBody.Controls.Add(this.lblPassword, 0, 5);
            this.tlpBody.Controls.Add(this.txtPassword, 0, 6);
            this.tlpBody.Controls.Add(this.lblConfirm, 0, 7);
            this.tlpBody.Controls.Add(this.txtConfirm, 0, 8);
            this.tlpBody.Controls.Add(this.chkShow, 0, 9);
            this.tlpBody.Controls.Add(this.lblPolicy, 0, 10);
            this.tlpBody.Controls.Add(this.lblError, 0, 11);
            this.tlpBody.Controls.Add(this.flowButtons, 0, 12);
            this.tlpBody.ColumnCount = 1;
            this.tlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBody.RowCount = 13;
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpBody.Padding = new System.Windows.Forms.Padding(30, 18, 30, 12);
            this.tlpBody.TabIndex = 1;
            this.tlpBody.Name = "tlpBody";
            // 
            // lblIntro
            // 
            this.lblIntro.AutoSize = true;
            this.lblIntro.MaximumSize = new System.Drawing.Size(380, 0);
            this.lblIntro.Margin = new System.Windows.Forms.Padding(3, 0, 3, 14);
            this.lblIntro.Text = "Welcome to Stockora. No administrator account exists yet, so create the first one now. Choose your own password - it is stored only as a salted hash. Only an administrator can create other accounts afterwards.";
            this.lblIntro.TabIndex = 0;
            this.lblIntro.Name = "lblIntro";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblUsername.Text = "Administrator username";
            this.lblUsername.TabIndex = 1;
            this.lblUsername.Name = "lblUsername";
            // 
            // txtUsername
            // 
            this.txtUsername.MaxLength = 32;
            this.txtUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUsername.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.Name = "txtUsername";
            // 
            // lblFullName
            // 
            this.lblFullName.AutoSize = true;
            this.lblFullName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblFullName.Text = "Full name (optional)";
            this.lblFullName.TabIndex = 3;
            this.lblFullName.Name = "lblFullName";
            // 
            // txtFullName
            // 
            this.txtFullName.MaxLength = 80;
            this.txtFullName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFullName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.txtFullName.TabIndex = 4;
            this.txtFullName.Name = "txtFullName";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblPassword.Text = "Password";
            this.lblPassword.TabIndex = 5;
            this.lblPassword.Name = "lblPassword";
            // 
            // txtPassword
            // 
            this.txtPassword.MaxLength = 128;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPassword.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.txtPassword.TabIndex = 6;
            this.txtPassword.Name = "txtPassword";
            // 
            // lblConfirm
            // 
            this.lblConfirm.AutoSize = true;
            this.lblConfirm.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblConfirm.Text = "Confirm password";
            this.lblConfirm.TabIndex = 7;
            this.lblConfirm.Name = "lblConfirm";
            // 
            // txtConfirm
            // 
            this.txtConfirm.MaxLength = 128;
            this.txtConfirm.UseSystemPasswordChar = true;
            this.txtConfirm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConfirm.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.txtConfirm.TabIndex = 8;
            this.txtConfirm.Name = "txtConfirm";
            // 
            // chkShow
            // 
            this.chkShow.AutoSize = true;
            this.chkShow.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
            this.chkShow.Text = "Show passwords";
            this.chkShow.UseVisualStyleBackColor = true;
            this.chkShow.TabIndex = 9;
            this.chkShow.Name = "chkShow";
            this.chkShow.CheckedChanged += new System.EventHandler(this.chkShow_CheckedChanged);
            // 
            // lblPolicy
            // 
            this.lblPolicy.AutoSize = true;
            this.lblPolicy.ForeColor = System.Drawing.Color.DimGray;
            this.lblPolicy.MaximumSize = new System.Drawing.Size(380, 0);
            this.lblPolicy.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.lblPolicy.Text = "Username: 3-32 letters, digits, '.', '_' or '-'.\r\nPassword: at least 8 characters, with a letter and a digit.";
            this.lblPolicy.TabIndex = 10;
            this.lblPolicy.Name = "lblPolicy";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Firebrick;
            this.lblError.MaximumSize = new System.Drawing.Size(380, 0);
            this.lblError.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.lblError.TabIndex = 11;
            this.lblError.Name = "lblError";
            // 
            // flowButtons
            // 
            this.flowButtons.Controls.Add(this.btnCreate);
            this.flowButtons.Controls.Add(this.btnExit);
            this.flowButtons.AutoSize = true;
            this.flowButtons.Padding = new System.Windows.Forms.Padding(0);
            this.flowButtons.TabIndex = 12;
            this.flowButtons.Name = "flowButtons";
            // 
            // btnCreate
            // 
            this.btnCreate.AutoSize = true;
            this.btnCreate.Margin = new System.Windows.Forms.Padding(3);
            this.btnCreate.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnCreate.Text = "Create administrator";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.TabIndex = 0;
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Margin = new System.Windows.Forms.Padding(3);
            this.btnExit.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnExit.AutoSize = true;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.TabIndex = 1;
            this.btnExit.Name = "btnExit";
            // 
            // FirstAdminSetupForm
            // 
            this.Controls.Add(this.tlpRoot);
            this.AcceptButton = this.btnCreate;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(440, 700);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirstAdminSetupForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stockora - First-time setup";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tlpRoot.ResumeLayout(false);
            this.tlpRoot.PerformLayout();
            this.tlpBody.ResumeLayout(false);
            this.tlpBody.PerformLayout();
            this.flowButtons.ResumeLayout(false);
            this.flowButtons.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpRoot;
        private InventoryManager.Auth.BrandHeader brandHeader;
        private System.Windows.Forms.TableLayoutPanel tlpBody;
        private System.Windows.Forms.Label lblIntro;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblFullName;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblConfirm;
        private System.Windows.Forms.TextBox txtConfirm;
        private System.Windows.Forms.CheckBox chkShow;
        private System.Windows.Forms.Label lblPolicy;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnExit;
    }
}
