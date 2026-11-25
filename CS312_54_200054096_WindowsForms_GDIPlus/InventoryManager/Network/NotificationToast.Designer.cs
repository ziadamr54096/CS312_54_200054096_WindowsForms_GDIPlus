#nullable disable
namespace InventoryManager
{
    partial class NotificationToast
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
            this.components = new System.ComponentModel.Container();
            this.tlpToast = new System.Windows.Forms.TableLayoutPanel();
            this.pnlAccent = new System.Windows.Forms.Panel();
            this.tlpText = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblBody = new System.Windows.Forms.Label();
            this.tmrClose = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            this.tlpToast.SuspendLayout();
            this.tlpText.SuspendLayout();
            // 
            // tlpToast
            // 
            this.tlpToast.Controls.Add(this.pnlAccent, 0, 0);
            this.tlpToast.Controls.Add(this.tlpText, 1, 0);
            this.tlpToast.ColumnCount = 2;
            this.tlpToast.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tlpToast.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpToast.RowCount = 1;
            this.tlpToast.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpToast.BackColor = System.Drawing.Color.White;
            this.tlpToast.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpToast.TabIndex = 0;
            this.tlpToast.Name = "tlpToast";
            this.tlpToast.Click += new System.EventHandler(this.Toast_Click);
            // 
            // pnlAccent
            // 
            this.pnlAccent.BackColor = System.Drawing.Color.SteelBlue;
            this.pnlAccent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAccent.Margin = new System.Windows.Forms.Padding(0);
            this.pnlAccent.TabIndex = 0;
            this.pnlAccent.Name = "pnlAccent";
            this.pnlAccent.Click += new System.EventHandler(this.Toast_Click);
            // 
            // tlpText
            // 
            this.tlpText.Controls.Add(this.lblTitle, 0, 0);
            this.tlpText.Controls.Add(this.lblBody, 0, 1);
            this.tlpText.ColumnCount = 1;
            this.tlpText.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpText.RowCount = 2;
            this.tlpText.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpText.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpText.Padding = new System.Windows.Forms.Padding(10, 8, 10, 6);
            this.tlpText.TabIndex = 1;
            this.tlpText.Name = "tlpText";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.lblTitle.MaximumSize = new System.Drawing.Size(330, 0);
            this.lblTitle.Text = "Notification";
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Click += new System.EventHandler(this.Toast_Click);
            // 
            // lblBody
            // 
            this.lblBody.AutoEllipsis = true;
            this.lblBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBody.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBody.ForeColor = System.Drawing.Color.DimGray;
            this.lblBody.Margin = new System.Windows.Forms.Padding(0);
            this.lblBody.TabIndex = 1;
            this.lblBody.Name = "lblBody";
            this.lblBody.Click += new System.EventHandler(this.Toast_Click);
            // 
            // tmrClose
            // 
            this.tmrClose.Interval = 5500;
            this.tmrClose.Tick += new System.EventHandler(this.tmrClose_Tick);
            // 
            // NotificationToast
            // 
            this.Controls.Add(this.tlpToast);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(208)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(360, 92);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "NotificationToast";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Stockora notification";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tlpToast.ResumeLayout(false);
            this.tlpToast.PerformLayout();
            this.tlpText.ResumeLayout(false);
            this.tlpText.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpToast;
        private System.Windows.Forms.Panel pnlAccent;
        private System.Windows.Forms.TableLayoutPanel tlpText;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBody;
        private System.Windows.Forms.Timer tmrClose;
    }
}
