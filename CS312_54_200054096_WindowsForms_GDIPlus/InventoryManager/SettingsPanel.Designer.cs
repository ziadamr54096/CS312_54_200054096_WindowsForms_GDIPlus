#nullable disable
namespace InventoryManager
{
    partial class SettingsPanel
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
            this.flowRoot = new System.Windows.Forms.FlowLayoutPanel();
            this.lblInfo = new System.Windows.Forms.Label();
            this.tlpForm = new System.Windows.Forms.TableLayoutPanel();
            this.lblHost = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.chkSsl = new System.Windows.Forms.CheckBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.txtPass = new System.Windows.Forms.TextBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.txtTo = new System.Windows.Forms.TextBox();
            this.chkAuto = new System.Windows.Forms.CheckBox();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.SuspendLayout();
            this.flowRoot.SuspendLayout();
            this.tlpForm.SuspendLayout();
            this.flowButtons.SuspendLayout();
            // 
            // flowRoot
            // 
            this.flowRoot.Controls.Add(this.lblInfo);
            this.flowRoot.Controls.Add(this.tlpForm);
            this.flowRoot.Controls.Add(this.flowButtons);
            this.flowRoot.Controls.Add(this.lblStatus);
            this.flowRoot.AutoScroll = true;
            this.flowRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowRoot.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowRoot.Padding = new System.Windows.Forms.Padding(16);
            this.flowRoot.WrapContents = false;
            this.flowRoot.TabIndex = 0;
            this.flowRoot.Name = "flowRoot";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.MaximumSize = new System.Drawing.Size(560, 0);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 12);
            this.lblInfo.Text = "Configure SMTP to receive low-stock alerts by e-mail.\r\nGmail needs an App Password (Google Account > Security > 2-Step Verification > App passwords), server smtp.gmail.com, port 587, SSL on. Click \"Test connection\" to verify.";
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Name = "lblInfo";
            // 
            // tlpForm
            // 
            this.tlpForm.Controls.Add(this.lblHost, 0, 0);
            this.tlpForm.Controls.Add(this.txtHost, 1, 0);
            this.tlpForm.Controls.Add(this.lblPort, 0, 1);
            this.tlpForm.Controls.Add(this.numPort, 1, 1);
            this.tlpForm.Controls.Add(this.chkSsl, 1, 2);
            this.tlpForm.Controls.Add(this.lblUser, 0, 3);
            this.tlpForm.Controls.Add(this.txtUser, 1, 3);
            this.tlpForm.Controls.Add(this.lblPass, 0, 4);
            this.tlpForm.Controls.Add(this.txtPass, 1, 4);
            this.tlpForm.Controls.Add(this.lblTo, 0, 5);
            this.tlpForm.Controls.Add(this.txtTo, 1, 5);
            this.tlpForm.Controls.Add(this.chkAuto, 1, 6);
            this.tlpForm.ColumnCount = 2;
            this.tlpForm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
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
            this.tlpForm.Padding = new System.Windows.Forms.Padding(8);
            this.tlpForm.Size = new System.Drawing.Size(560, 100);
            this.tlpForm.TabIndex = 1;
            this.tlpForm.Name = "tlpForm";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblHost.Text = "SMTP server";
            this.lblHost.TabIndex = 0;
            this.lblHost.Name = "lblHost";
            // 
            // txtHost
            // 
            this.txtHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHost.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtHost.TabIndex = 1;
            this.txtHost.Name = "txtHost";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblPort.Text = "Port";
            this.lblPort.TabIndex = 2;
            this.lblPort.Name = "lblPort";
            // 
            // numPort
            // 
            this.numPort.Maximum = new decimal(new int[] {65535, 0, 0, 0});
            this.numPort.Minimum = new decimal(new int[] {1, 0, 0, 0});
            this.numPort.Value = new decimal(new int[] {587, 0, 0, 0});
            this.numPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numPort.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.numPort.TabIndex = 3;
            this.numPort.Name = "numPort";
            // 
            // chkSsl
            // 
            this.chkSsl.AutoSize = true;
            this.chkSsl.Checked = true;
            this.chkSsl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSsl.Text = "Use SSL / STARTTLS";
            this.chkSsl.UseVisualStyleBackColor = true;
            this.chkSsl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkSsl.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.chkSsl.TabIndex = 4;
            this.chkSsl.Name = "chkSsl";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblUser.Text = "From / user";
            this.lblUser.TabIndex = 5;
            this.lblUser.Name = "lblUser";
            // 
            // txtUser
            // 
            this.txtUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUser.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtUser.TabIndex = 6;
            this.txtUser.Name = "txtUser";
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblPass.Text = "Password";
            this.lblPass.TabIndex = 7;
            this.lblPass.Name = "lblPass";
            // 
            // txtPass
            // 
            this.txtPass.UseSystemPasswordChar = true;
            this.txtPass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPass.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtPass.TabIndex = 8;
            this.txtPass.Name = "txtPass";
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblTo.Text = "Alert e-mail to";
            this.lblTo.TabIndex = 9;
            this.lblTo.Name = "lblTo";
            // 
            // txtTo
            // 
            this.txtTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTo.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.txtTo.TabIndex = 10;
            this.txtTo.Name = "txtTo";
            // 
            // chkAuto
            // 
            this.chkAuto.AutoSize = true;
            this.chkAuto.Text = "Send an alert e-mail automatically when stock runs low";
            this.chkAuto.UseVisualStyleBackColor = true;
            this.chkAuto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkAuto.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.chkAuto.TabIndex = 11;
            this.chkAuto.Name = "chkAuto";
            // 
            // flowButtons
            // 
            this.flowButtons.Controls.Add(this.btnSave);
            this.flowButtons.Controls.Add(this.btnTest);
            this.flowButtons.AutoSize = true;
            this.flowButtons.Padding = new System.Windows.Forms.Padding(4);
            this.flowButtons.TabIndex = 2;
            this.flowButtons.Name = "flowButtons";
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.Margin = new System.Windows.Forms.Padding(3);
            this.btnSave.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnSave.Text = "Save settings";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.TabIndex = 0;
            this.btnSave.Name = "btnSave";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnTest
            // 
            this.btnTest.AutoSize = true;
            this.btnTest.Margin = new System.Windows.Forms.Padding(3);
            this.btnTest.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnTest.Text = "Test connection";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.TabIndex = 1;
            this.btnTest.Name = "btnTest";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.MaximumSize = new System.Drawing.Size(560, 0);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Name = "lblStatus";
            // 
            // SettingsPanel
            // 
            this.Controls.Add(this.flowRoot);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SettingsPanel";
            this.Size = new System.Drawing.Size(1100, 640);
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.flowRoot.ResumeLayout(false);
            this.flowRoot.PerformLayout();
            this.tlpForm.ResumeLayout(false);
            this.tlpForm.PerformLayout();
            this.flowButtons.ResumeLayout(false);
            this.flowButtons.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowRoot;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.TableLayoutPanel tlpForm;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.CheckBox chkSsl;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.TextBox txtPass;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.CheckBox chkAuto;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label lblStatus;
    }
}
