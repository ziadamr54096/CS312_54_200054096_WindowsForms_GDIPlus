#nullable disable
namespace InventoryManager.Auth
{
    partial class LoginForm
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
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.tlpPassword = new System.Windows.Forms.TableLayoutPanel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnTogglePassword = new System.Windows.Forms.Button();
            this.lblCaps = new System.Windows.Forms.Label();
            this.chkRemember = new System.Windows.Forms.CheckBox();
            this.lblError = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lblHint = new System.Windows.Forms.Label();
            this.SuspendLayout();
            this.tlpRoot.SuspendLayout();
            this.tlpBody.SuspendLayout();
            this.tlpPassword.SuspendLayout();
            // 
            // tlpRoot
            // 
            this.tlpRoot.Controls.Add(this.brandHeader, 0, 0);
            this.tlpRoot.Controls.Add(this.tlpBody, 0, 1);
            this.tlpRoot.ColumnCount = 1;
            this.tlpRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.RowCount = 2;
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
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
            this.tlpBody.Controls.Add(this.lblWelcome, 0, 0);
            this.tlpBody.Controls.Add(this.lblUsername, 0, 1);
            this.tlpBody.Controls.Add(this.txtUsername, 0, 2);
            this.tlpBody.Controls.Add(this.lblPassword, 0, 3);
            this.tlpBody.Controls.Add(this.tlpPassword, 0, 4);
            this.tlpBody.Controls.Add(this.lblCaps, 0, 5);
            this.tlpBody.Controls.Add(this.chkRemember, 0, 6);
            this.tlpBody.Controls.Add(this.lblError, 0, 7);
            this.tlpBody.Controls.Add(this.btnLogin, 0, 8);
            this.tlpBody.Controls.Add(this.lblHint, 0, 9);
            this.tlpBody.ColumnCount = 1;
            this.tlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBody.RowCount = 10;
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
            this.tlpBody.Padding = new System.Windows.Forms.Padding(36, 24, 36, 16);
            this.tlpBody.TabIndex = 1;
            this.tlpBody.Name = "tlpBody";
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.lblWelcome.Margin = new System.Windows.Forms.Padding(3, 0, 3, 16);
            this.lblWelcome.Text = "Sign in to your account";
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Name = "lblWelcome";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblUsername.Text = "Username";
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
            this.txtUsername.TextChanged += new System.EventHandler(this.Field_TextChanged);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblPassword.Text = "Password";
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Name = "lblPassword";
            // 
            // tlpPassword
            // 
            this.tlpPassword.Controls.Add(this.txtPassword, 0, 0);
            this.tlpPassword.Controls.Add(this.btnTogglePassword, 1, 0);
            this.tlpPassword.ColumnCount = 2;
            this.tlpPassword.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpPassword.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpPassword.RowCount = 1;
            this.tlpPassword.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpPassword.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.tlpPassword.TabIndex = 4;
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
            this.txtPassword.TextChanged += new System.EventHandler(this.Field_TextChanged);
            this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyUp);
            this.txtPassword.Enter += new System.EventHandler(this.txtPassword_Enter);
            // 
            // btnTogglePassword
            // 
            this.btnTogglePassword.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.btnTogglePassword.Size = new System.Drawing.Size(70, 29);
            this.btnTogglePassword.Text = "Show";
            this.btnTogglePassword.UseVisualStyleBackColor = true;
            this.btnTogglePassword.TabIndex = 1;
            this.btnTogglePassword.Name = "btnTogglePassword";
            this.btnTogglePassword.Click += new System.EventHandler(this.btnTogglePassword_Click);
            // 
            // lblCaps
            // 
            this.lblCaps.AutoSize = true;
            this.lblCaps.ForeColor = System.Drawing.Color.DarkOrange;
            this.lblCaps.Visible = false;
            this.lblCaps.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.lblCaps.Text = "Caps Lock is on";
            this.lblCaps.TabIndex = 5;
            this.lblCaps.Name = "lblCaps";
            // 
            // chkRemember
            // 
            this.chkRemember.AutoSize = true;
            this.chkRemember.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
            this.chkRemember.Text = "Remember me";
            this.chkRemember.UseVisualStyleBackColor = true;
            this.chkRemember.TabIndex = 6;
            this.chkRemember.Name = "chkRemember";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Firebrick;
            this.lblError.MaximumSize = new System.Drawing.Size(340, 0);
            this.lblError.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.lblError.TabIndex = 7;
            this.lblError.Name = "lblError";
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.btnLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.Margin = new System.Windows.Forms.Padding(3, 6, 3, 8);
            this.btnLogin.MinimumSize = new System.Drawing.Size(0, 42);
            this.btnLogin.Text = "Sign in";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.TabIndex = 8;
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.lblHint.ForeColor = System.Drawing.Color.DimGray;
            this.lblHint.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHint.MaximumSize = new System.Drawing.Size(340, 0);
            this.lblHint.Text = "Accounts are created by an administrator. If you cannot sign in, contact your administrator.";
            this.lblHint.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblHint.TabIndex = 9;
            this.lblHint.Name = "lblHint";
            // 
            // LoginForm
            // 
            this.Controls.Add(this.tlpRoot);
            this.AcceptButton = this.btnLogin;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(420, 600);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stockora - Sign in";
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tlpRoot.ResumeLayout(false);
            this.tlpRoot.PerformLayout();
            this.tlpBody.ResumeLayout(false);
            this.tlpBody.PerformLayout();
            this.tlpPassword.ResumeLayout(false);
            this.tlpPassword.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpRoot;
        private InventoryManager.Auth.BrandHeader brandHeader;
        private System.Windows.Forms.TableLayoutPanel tlpBody;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TableLayoutPanel tlpPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnTogglePassword;
        private System.Windows.Forms.Label lblCaps;
        private System.Windows.Forms.CheckBox chkRemember;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblHint;
    }
}
