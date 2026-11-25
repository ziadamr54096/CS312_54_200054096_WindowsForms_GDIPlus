#nullable disable
namespace InventoryManager.Auth
{
    partial class ChangePasswordForm
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
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.txtCurrent = new System.Windows.Forms.TextBox();
            this.lblNew = new System.Windows.Forms.Label();
            this.txtNew = new System.Windows.Forms.TextBox();
            this.lblConfirm = new System.Windows.Forms.Label();
            this.txtConfirm = new System.Windows.Forms.TextBox();
            this.chkShow = new System.Windows.Forms.CheckBox();
            this.lblPolicy = new System.Windows.Forms.Label();
            this.lblError = new System.Windows.Forms.Label();
            this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
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
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tlpRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRoot.TabIndex = 0;
            this.tlpRoot.Name = "tlpRoot";
            // 
            // brandHeader
            // 
            this.brandHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brandHeader.Tagline = "Change your password";
            this.brandHeader.TabIndex = 0;
            this.brandHeader.Name = "brandHeader";
            // 
            // tlpBody
            // 
            this.tlpBody.Controls.Add(this.lblInfo, 0, 0);
            this.tlpBody.Controls.Add(this.lblCurrent, 0, 1);
            this.tlpBody.Controls.Add(this.txtCurrent, 0, 2);
            this.tlpBody.Controls.Add(this.lblNew, 0, 3);
            this.tlpBody.Controls.Add(this.txtNew, 0, 4);
            this.tlpBody.Controls.Add(this.lblConfirm, 0, 5);
            this.tlpBody.Controls.Add(this.txtConfirm, 0, 6);
            this.tlpBody.Controls.Add(this.chkShow, 0, 7);
            this.tlpBody.Controls.Add(this.lblPolicy, 0, 8);
            this.tlpBody.Controls.Add(this.lblError, 0, 9);
            this.tlpBody.Controls.Add(this.flowButtons, 0, 10);
            this.tlpBody.ColumnCount = 1;
            this.tlpBody.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBody.RowCount = 11;
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
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.MaximumSize = new System.Drawing.Size(340, 0);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(3, 0, 3, 12);
            this.lblInfo.Text = "Enter your current password, then choose a new one.";
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Name = "lblInfo";
            // 
            // lblCurrent
            // 
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblCurrent.Text = "Current password";
            this.lblCurrent.TabIndex = 1;
            this.lblCurrent.Name = "lblCurrent";
            // 
            // txtCurrent
            // 
            this.txtCurrent.MaxLength = 128;
            this.txtCurrent.UseSystemPasswordChar = true;
            this.txtCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCurrent.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.txtCurrent.TabIndex = 2;
            this.txtCurrent.Name = "txtCurrent";
            // 
            // lblNew
            // 
            this.lblNew.AutoSize = true;
            this.lblNew.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblNew.Text = "New password";
            this.lblNew.TabIndex = 3;
            this.lblNew.Name = "lblNew";
            // 
            // txtNew
            // 
            this.txtNew.MaxLength = 128;
            this.txtNew.UseSystemPasswordChar = true;
            this.txtNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNew.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.txtNew.TabIndex = 4;
            this.txtNew.Name = "txtNew";
            // 
            // lblConfirm
            // 
            this.lblConfirm.AutoSize = true;
            this.lblConfirm.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblConfirm.Text = "Confirm new password";
            this.lblConfirm.TabIndex = 5;
            this.lblConfirm.Name = "lblConfirm";
            // 
            // txtConfirm
            // 
            this.txtConfirm.MaxLength = 128;
            this.txtConfirm.UseSystemPasswordChar = true;
            this.txtConfirm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConfirm.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            this.txtConfirm.TabIndex = 6;
            this.txtConfirm.Name = "txtConfirm";
            // 
            // chkShow
            // 
            this.chkShow.AutoSize = true;
            this.chkShow.Margin = new System.Windows.Forms.Padding(3, 0, 3, 8);
            this.chkShow.Text = "Show passwords";
            this.chkShow.UseVisualStyleBackColor = true;
            this.chkShow.TabIndex = 7;
            this.chkShow.Name = "chkShow";
            this.chkShow.CheckedChanged += new System.EventHandler(this.chkShow_CheckedChanged);
            // 
            // lblPolicy
            // 
            this.lblPolicy.AutoSize = true;
            this.lblPolicy.ForeColor = System.Drawing.Color.DimGray;
            this.lblPolicy.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.lblPolicy.Text = "At least 8 characters, including a letter and a digit.";
            this.lblPolicy.TabIndex = 8;
            this.lblPolicy.Name = "lblPolicy";
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = System.Drawing.Color.Firebrick;
            this.lblError.MaximumSize = new System.Drawing.Size(340, 0);
            this.lblError.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.lblError.TabIndex = 9;
            this.lblError.Name = "lblError";
            // 
            // flowButtons
            // 
            this.flowButtons.Controls.Add(this.btnOk);
            this.flowButtons.Controls.Add(this.btnCancel);
            this.flowButtons.AutoSize = true;
            this.flowButtons.Padding = new System.Windows.Forms.Padding(0);
            this.flowButtons.TabIndex = 10;
            this.flowButtons.Name = "flowButtons";
            // 
            // btnOk
            // 
            this.btnOk.AutoSize = true;
            this.btnOk.Margin = new System.Windows.Forms.Padding(3);
            this.btnOk.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnOk.Text = "Change password";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.TabIndex = 0;
            this.btnOk.Name = "btnOk";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3);
            this.btnCancel.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnCancel.AutoSize = true;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Name = "btnCancel";
            // 
            // ChangePasswordForm
            // 
            this.Controls.Add(this.tlpRoot);
            this.AcceptButton = this.btnOk;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(420, 620);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangePasswordForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Stockora - Change password";
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
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.TextBox txtCurrent;
        private System.Windows.Forms.Label lblNew;
        private System.Windows.Forms.TextBox txtNew;
        private System.Windows.Forms.Label lblConfirm;
        private System.Windows.Forms.TextBox txtConfirm;
        private System.Windows.Forms.CheckBox chkShow;
        private System.Windows.Forms.Label lblPolicy;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.FlowLayoutPanel flowButtons;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
