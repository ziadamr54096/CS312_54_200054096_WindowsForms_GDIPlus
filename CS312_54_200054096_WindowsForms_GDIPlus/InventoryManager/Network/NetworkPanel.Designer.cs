#nullable disable
namespace InventoryManager
{
    partial class NetworkPanel
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.components = new System.ComponentModel.Container();
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpLeft = new System.Windows.Forms.TableLayoutPanel();
            this.grpServer = new System.Windows.Forms.GroupBox();
            this.tlpServer = new System.Windows.Forms.TableLayoutPanel();
            this.lblBind = new System.Windows.Forms.Label();
            this.cboBind = new System.Windows.Forms.ComboBox();
            this.lblServerPort = new System.Windows.Forms.Label();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.lblServerCode = new System.Windows.Forms.Label();
            this.txtServerCode = new System.Windows.Forms.TextBox();
            this.chkAllowPublic = new System.Windows.Forms.CheckBox();
            this.chkBroadcastInventory = new System.Windows.Forms.CheckBox();
            this.flowServerButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnStopServer = new System.Windows.Forms.Button();
            this.lblServerStatusCaption = new System.Windows.Forms.Label();
            this.lblServerStatus = new System.Windows.Forms.Label();
            this.lblEndpointCaption = new System.Windows.Forms.Label();
            this.lblServerEndpoint = new System.Windows.Forms.Label();
            this.lblLocalIpCaption = new System.Windows.Forms.Label();
            this.lblLocalIps = new System.Windows.Forms.Label();
            this.grpClient = new System.Windows.Forms.GroupBox();
            this.tlpClient = new System.Windows.Forms.TableLayoutPanel();
            this.lblHost = new System.Windows.Forms.Label();
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblClientPort = new System.Windows.Forms.Label();
            this.txtClientPort = new System.Windows.Forms.TextBox();
            this.lblDeviceName = new System.Windows.Forms.Label();
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.lblClientCode = new System.Windows.Forms.Label();
            this.txtClientCode = new System.Windows.Forms.TextBox();
            this.flowClientButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.lblClientStatusCaption = new System.Windows.Forms.Label();
            this.lblClientStatus = new System.Windows.Forms.Label();
            this.lblDurationCaption = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblTestCaption = new System.Windows.Forms.Label();
            this.lblTestResult = new System.Windows.Forms.Label();
            this.tlpRight = new System.Windows.Forms.TableLayoutPanel();
            this.grpDevices = new System.Windows.Forms.GroupBox();
            this.tlpDevices = new System.Windows.Forms.TableLayoutPanel();
            this.dgvDevices = new System.Windows.Forms.DataGridView();
            this.colDevName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDevIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDevPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDevStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDevSince = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDevOnline = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblDeviceCount = new System.Windows.Forms.Label();
            this.grpMessaging = new System.Windows.Forms.GroupBox();
            this.tlpMessaging = new System.Windows.Forms.TableLayoutPanel();
            this.tlpSend = new System.Windows.Forms.TableLayoutPanel();
            this.lblTo = new System.Windows.Forms.Label();
            this.cboRecipient = new System.Windows.Forms.ComboBox();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.dgvLog = new System.Windows.Forms.DataGridView();
            this.colLogTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogDir = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogPeer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLogText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.flowLogBar = new System.Windows.Forms.FlowLayoutPanel();
            this.chkToasts = new System.Windows.Forms.CheckBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.grpStock = new System.Windows.Forms.GroupBox();
            this.tlpStock = new System.Windows.Forms.TableLayoutPanel();
            this.lblSku = new System.Windows.Forms.Label();
            this.txtSku = new System.Windows.Forms.TextBox();
            this.lblDelta = new System.Windows.Forms.Label();
            this.numDelta = new System.Windows.Forms.NumericUpDown();
            this.lblReason = new System.Windows.Forms.Label();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.flowStockButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCheckStock = new System.Windows.Forms.Button();
            this.btnApplyStock = new System.Windows.Forms.Button();
            this.lblStockResult = new System.Windows.Forms.Label();
            this.tmrClock = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelta)).BeginInit();
            this.SuspendLayout();
            this.tlpMain.SuspendLayout();
            this.tlpLeft.SuspendLayout();
            this.grpServer.SuspendLayout();
            this.tlpServer.SuspendLayout();
            this.flowServerButtons.SuspendLayout();
            this.grpClient.SuspendLayout();
            this.tlpClient.SuspendLayout();
            this.flowClientButtons.SuspendLayout();
            this.tlpRight.SuspendLayout();
            this.grpDevices.SuspendLayout();
            this.tlpDevices.SuspendLayout();
            this.grpMessaging.SuspendLayout();
            this.tlpMessaging.SuspendLayout();
            this.tlpSend.SuspendLayout();
            this.flowLogBar.SuspendLayout();
            this.grpStock.SuspendLayout();
            this.tlpStock.SuspendLayout();
            this.flowStockButtons.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.Controls.Add(this.tlpLeft, 0, 0);
            this.tlpMain.Controls.Add(this.tlpRight, 1, 0);
            this.tlpMain.ColumnCount = 2;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 410F));
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.RowCount = 1;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Padding = new System.Windows.Forms.Padding(6);
            this.tlpMain.TabIndex = 0;
            this.tlpMain.Name = "tlpMain";
            // 
            // tlpLeft
            // 
            this.tlpLeft.Controls.Add(this.grpServer, 0, 0);
            this.tlpLeft.Controls.Add(this.grpClient, 0, 1);
            this.tlpLeft.ColumnCount = 1;
            this.tlpLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLeft.RowCount = 2;
            this.tlpLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53F));
            this.tlpLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.tlpLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpLeft.TabIndex = 0;
            this.tlpLeft.Name = "tlpLeft";
            // 
            // grpServer
            // 
            this.grpServer.Controls.Add(this.tlpServer);
            this.grpServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpServer.Padding = new System.Windows.Forms.Padding(8, 6, 8, 8);
            this.grpServer.Text = "A. Server Configuration";
            this.grpServer.TabIndex = 0;
            this.grpServer.Name = "grpServer";
            // 
            // tlpServer
            // 
            this.tlpServer.Controls.Add(this.lblBind, 0, 0);
            this.tlpServer.Controls.Add(this.cboBind, 1, 0);
            this.tlpServer.Controls.Add(this.lblServerPort, 0, 1);
            this.tlpServer.Controls.Add(this.txtServerPort, 1, 1);
            this.tlpServer.Controls.Add(this.lblServerCode, 0, 2);
            this.tlpServer.Controls.Add(this.txtServerCode, 1, 2);
            this.tlpServer.Controls.Add(this.chkAllowPublic, 1, 3);
            this.tlpServer.Controls.Add(this.chkBroadcastInventory, 1, 4);
            this.tlpServer.Controls.Add(this.flowServerButtons, 0, 5);
            this.tlpServer.Controls.Add(this.lblServerStatusCaption, 0, 6);
            this.tlpServer.Controls.Add(this.lblServerStatus, 1, 6);
            this.tlpServer.Controls.Add(this.lblEndpointCaption, 0, 7);
            this.tlpServer.Controls.Add(this.lblServerEndpoint, 1, 7);
            this.tlpServer.Controls.Add(this.lblLocalIpCaption, 0, 8);
            this.tlpServer.Controls.Add(this.lblLocalIps, 1, 8);
            this.tlpServer.SetColumnSpan(this.flowServerButtons, 2);
            this.tlpServer.ColumnCount = 2;
            this.tlpServer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tlpServer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpServer.RowCount = 9;
            this.tlpServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpServer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpServer.AutoSize = true;
            this.tlpServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpServer.Padding = new System.Windows.Forms.Padding(4);
            this.tlpServer.TabIndex = 0;
            this.tlpServer.Name = "tlpServer";
            // 
            // lblBind
            // 
            this.lblBind.AutoSize = true;
            this.lblBind.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.lblBind.Text = "Server IP address";
            this.lblBind.TabIndex = 0;
            this.lblBind.Name = "lblBind";
            // 
            // cboBind
            // 
            this.cboBind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBind.FormattingEnabled = true;
            this.cboBind.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboBind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboBind.TabIndex = 1;
            this.cboBind.Name = "cboBind";
            // 
            // lblServerPort
            // 
            this.lblServerPort.AutoSize = true;
            this.lblServerPort.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblServerPort.Text = "Listening port";
            this.lblServerPort.TabIndex = 2;
            this.lblServerPort.Name = "lblServerPort";
            // 
            // txtServerPort
            // 
            this.txtServerPort.MaxLength = 5;
            this.txtServerPort.Text = "5000";
            this.txtServerPort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtServerPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtServerPort.TabIndex = 3;
            this.txtServerPort.Name = "txtServerPort";
            // 
            // lblServerCode
            // 
            this.lblServerCode.AutoSize = true;
            this.lblServerCode.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblServerCode.Text = "Access code";
            this.lblServerCode.TabIndex = 4;
            this.lblServerCode.Name = "lblServerCode";
            // 
            // txtServerCode
            // 
            this.txtServerCode.MaxLength = 64;
            this.txtServerCode.PlaceholderText = "optional - clients must type it";
            this.txtServerCode.UseSystemPasswordChar = true;
            this.txtServerCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtServerCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtServerCode.TabIndex = 5;
            this.txtServerCode.Name = "txtServerCode";
            // 
            // chkAllowPublic
            // 
            this.chkAllowPublic.AutoSize = true;
            this.chkAllowPublic.Text = "Allow non-private addresses";
            this.chkAllowPublic.UseVisualStyleBackColor = true;
            this.chkAllowPublic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkAllowPublic.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.chkAllowPublic.TabIndex = 6;
            this.chkAllowPublic.Name = "chkAllowPublic";
            // 
            // chkBroadcastInventory
            // 
            this.chkBroadcastInventory.AutoSize = true;
            this.chkBroadcastInventory.Checked = true;
            this.chkBroadcastInventory.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBroadcastInventory.Text = "Broadcast stock changes and alerts";
            this.chkBroadcastInventory.UseVisualStyleBackColor = true;
            this.chkBroadcastInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkBroadcastInventory.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.chkBroadcastInventory.TabIndex = 7;
            this.chkBroadcastInventory.Name = "chkBroadcastInventory";
            this.chkBroadcastInventory.CheckedChanged += new System.EventHandler(this.chkBroadcastInventory_CheckedChanged);
            // 
            // flowServerButtons
            // 
            this.flowServerButtons.Controls.Add(this.btnStartServer);
            this.flowServerButtons.Controls.Add(this.btnStopServer);
            this.flowServerButtons.AutoSize = true;
            this.flowServerButtons.Padding = new System.Windows.Forms.Padding(0);
            this.flowServerButtons.TabIndex = 8;
            this.flowServerButtons.Name = "flowServerButtons";
            // 
            // btnStartServer
            // 
            this.btnStartServer.AutoSize = true;
            this.btnStartServer.Margin = new System.Windows.Forms.Padding(3);
            this.btnStartServer.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnStartServer.Text = "Start server";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.TabIndex = 0;
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);
            // 
            // btnStopServer
            // 
            this.btnStopServer.AutoSize = true;
            this.btnStopServer.Margin = new System.Windows.Forms.Padding(3);
            this.btnStopServer.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnStopServer.Text = "Stop server";
            this.btnStopServer.UseVisualStyleBackColor = true;
            this.btnStopServer.Enabled = false;
            this.btnStopServer.TabIndex = 1;
            this.btnStopServer.Name = "btnStopServer";
            this.btnStopServer.Click += new System.EventHandler(this.btnStopServer_Click);
            // 
            // lblServerStatusCaption
            // 
            this.lblServerStatusCaption.AutoSize = true;
            this.lblServerStatusCaption.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblServerStatusCaption.Text = "Status";
            this.lblServerStatusCaption.TabIndex = 9;
            this.lblServerStatusCaption.Name = "lblServerStatusCaption";
            // 
            // lblServerStatus
            // 
            this.lblServerStatus.AutoSize = true;
            this.lblServerStatus.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.lblServerStatus.ForeColor = System.Drawing.Color.Firebrick;
            this.lblServerStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServerStatus.Text = "Stopped";
            this.lblServerStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblServerStatus.TabIndex = 10;
            this.lblServerStatus.Name = "lblServerStatus";
            // 
            // lblEndpointCaption
            // 
            this.lblEndpointCaption.AutoSize = true;
            this.lblEndpointCaption.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblEndpointCaption.Text = "Listening on";
            this.lblEndpointCaption.TabIndex = 11;
            this.lblEndpointCaption.Name = "lblEndpointCaption";
            // 
            // lblServerEndpoint
            // 
            this.lblServerEndpoint.AutoSize = true;
            this.lblServerEndpoint.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.lblServerEndpoint.Text = "-";
            this.lblServerEndpoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblServerEndpoint.TabIndex = 12;
            this.lblServerEndpoint.Name = "lblServerEndpoint";
            // 
            // lblLocalIpCaption
            // 
            this.lblLocalIpCaption.AutoSize = true;
            this.lblLocalIpCaption.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblLocalIpCaption.Text = "This computer";
            this.lblLocalIpCaption.TabIndex = 13;
            this.lblLocalIpCaption.Name = "lblLocalIpCaption";
            // 
            // lblLocalIps
            // 
            this.lblLocalIps.AutoSize = true;
            this.lblLocalIps.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.lblLocalIps.MaximumSize = new System.Drawing.Size(270, 0);
            this.lblLocalIps.Text = "-";
            this.lblLocalIps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLocalIps.TabIndex = 14;
            this.lblLocalIps.Name = "lblLocalIps";
            // 
            // grpClient
            // 
            this.grpClient.Controls.Add(this.tlpClient);
            this.grpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpClient.Padding = new System.Windows.Forms.Padding(8, 6, 8, 8);
            this.grpClient.Text = "B. Client Connection";
            this.grpClient.TabIndex = 1;
            this.grpClient.Name = "grpClient";
            // 
            // tlpClient
            // 
            this.tlpClient.Controls.Add(this.lblHost, 0, 0);
            this.tlpClient.Controls.Add(this.txtHost, 1, 0);
            this.tlpClient.Controls.Add(this.lblClientPort, 0, 1);
            this.tlpClient.Controls.Add(this.txtClientPort, 1, 1);
            this.tlpClient.Controls.Add(this.lblDeviceName, 0, 2);
            this.tlpClient.Controls.Add(this.txtDeviceName, 1, 2);
            this.tlpClient.Controls.Add(this.lblClientCode, 0, 3);
            this.tlpClient.Controls.Add(this.txtClientCode, 1, 3);
            this.tlpClient.Controls.Add(this.flowClientButtons, 0, 4);
            this.tlpClient.Controls.Add(this.lblClientStatusCaption, 0, 5);
            this.tlpClient.Controls.Add(this.lblClientStatus, 1, 5);
            this.tlpClient.Controls.Add(this.lblDurationCaption, 0, 6);
            this.tlpClient.Controls.Add(this.lblDuration, 1, 6);
            this.tlpClient.Controls.Add(this.lblTestCaption, 0, 7);
            this.tlpClient.Controls.Add(this.lblTestResult, 1, 7);
            this.tlpClient.SetColumnSpan(this.flowClientButtons, 2);
            this.tlpClient.ColumnCount = 2;
            this.tlpClient.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tlpClient.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpClient.RowCount = 8;
            this.tlpClient.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpClient.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpClient.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpClient.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpClient.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpClient.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpClient.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpClient.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpClient.AutoSize = true;
            this.tlpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpClient.Padding = new System.Windows.Forms.Padding(4);
            this.tlpClient.TabIndex = 0;
            this.tlpClient.Name = "tlpClient";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblHost.Text = "Server IP address";
            this.lblHost.TabIndex = 0;
            this.lblHost.Name = "lblHost";
            // 
            // txtHost
            // 
            this.txtHost.MaxLength = 15;
            this.txtHost.PlaceholderText = "e.g. 192.168.1.10";
            this.txtHost.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHost.TabIndex = 1;
            this.txtHost.Name = "txtHost";
            // 
            // lblClientPort
            // 
            this.lblClientPort.AutoSize = true;
            this.lblClientPort.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblClientPort.Text = "Server port";
            this.lblClientPort.TabIndex = 2;
            this.lblClientPort.Name = "lblClientPort";
            // 
            // txtClientPort
            // 
            this.txtClientPort.MaxLength = 5;
            this.txtClientPort.Text = "5000";
            this.txtClientPort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtClientPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtClientPort.TabIndex = 3;
            this.txtClientPort.Name = "txtClientPort";
            // 
            // lblDeviceName
            // 
            this.lblDeviceName.AutoSize = true;
            this.lblDeviceName.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblDeviceName.Text = "Device name";
            this.lblDeviceName.TabIndex = 4;
            this.lblDeviceName.Name = "lblDeviceName";
            // 
            // txtDeviceName
            // 
            this.txtDeviceName.MaxLength = 32;
            this.txtDeviceName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtDeviceName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDeviceName.TabIndex = 5;
            this.txtDeviceName.Name = "txtDeviceName";
            // 
            // lblClientCode
            // 
            this.lblClientCode.AutoSize = true;
            this.lblClientCode.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblClientCode.Text = "Access code";
            this.lblClientCode.TabIndex = 6;
            this.lblClientCode.Name = "lblClientCode";
            // 
            // txtClientCode
            // 
            this.txtClientCode.MaxLength = 64;
            this.txtClientCode.PlaceholderText = "only if the server uses one";
            this.txtClientCode.UseSystemPasswordChar = true;
            this.txtClientCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtClientCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtClientCode.TabIndex = 7;
            this.txtClientCode.Name = "txtClientCode";
            // 
            // flowClientButtons
            // 
            this.flowClientButtons.Controls.Add(this.btnConnect);
            this.flowClientButtons.Controls.Add(this.btnDisconnect);
            this.flowClientButtons.Controls.Add(this.btnTest);
            this.flowClientButtons.AutoSize = true;
            this.flowClientButtons.Padding = new System.Windows.Forms.Padding(0);
            this.flowClientButtons.TabIndex = 8;
            this.flowClientButtons.Name = "flowClientButtons";
            // 
            // btnConnect
            // 
            this.btnConnect.AutoSize = true;
            this.btnConnect.Margin = new System.Windows.Forms.Padding(3);
            this.btnConnect.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.AutoSize = true;
            this.btnDisconnect.Margin = new System.Windows.Forms.Padding(3);
            this.btnDisconnect.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.TabIndex = 1;
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnTest
            // 
            this.btnTest.AutoSize = true;
            this.btnTest.Margin = new System.Windows.Forms.Padding(3);
            this.btnTest.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnTest.Text = "Test connection";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.TabIndex = 2;
            this.btnTest.Name = "btnTest";
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // lblClientStatusCaption
            // 
            this.lblClientStatusCaption.AutoSize = true;
            this.lblClientStatusCaption.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblClientStatusCaption.Text = "Status";
            this.lblClientStatusCaption.TabIndex = 9;
            this.lblClientStatusCaption.Name = "lblClientStatusCaption";
            // 
            // lblClientStatus
            // 
            this.lblClientStatus.AutoSize = true;
            this.lblClientStatus.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.lblClientStatus.ForeColor = System.Drawing.Color.Firebrick;
            this.lblClientStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientStatus.Text = "Disconnected";
            this.lblClientStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClientStatus.TabIndex = 10;
            this.lblClientStatus.Name = "lblClientStatus";
            // 
            // lblDurationCaption
            // 
            this.lblDurationCaption.AutoSize = true;
            this.lblDurationCaption.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblDurationCaption.Text = "Connected for";
            this.lblDurationCaption.TabIndex = 11;
            this.lblDurationCaption.Name = "lblDurationCaption";
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.lblDuration.Text = "-";
            this.lblDuration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDuration.TabIndex = 12;
            this.lblDuration.Name = "lblDuration";
            // 
            // lblTestCaption
            // 
            this.lblTestCaption.AutoSize = true;
            this.lblTestCaption.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.lblTestCaption.Text = "Last test";
            this.lblTestCaption.TabIndex = 13;
            this.lblTestCaption.Name = "lblTestCaption";
            // 
            // lblTestResult
            // 
            this.lblTestResult.AutoSize = true;
            this.lblTestResult.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.lblTestResult.MaximumSize = new System.Drawing.Size(270, 0);
            this.lblTestResult.Text = "-";
            this.lblTestResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTestResult.TabIndex = 14;
            this.lblTestResult.Name = "lblTestResult";
            // 
            // tlpRight
            // 
            this.tlpRight.Controls.Add(this.grpDevices, 0, 0);
            this.tlpRight.Controls.Add(this.grpMessaging, 0, 1);
            this.tlpRight.Controls.Add(this.grpStock, 0, 2);
            this.tlpRight.ColumnCount = 1;
            this.tlpRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRight.RowCount = 3;
            this.tlpRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.tlpRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 118F));
            this.tlpRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpRight.TabIndex = 1;
            this.tlpRight.Name = "tlpRight";
            // 
            // grpDevices
            // 
            this.grpDevices.Controls.Add(this.tlpDevices);
            this.grpDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDevices.Padding = new System.Windows.Forms.Padding(8, 6, 8, 8);
            this.grpDevices.Text = "C. Connected Devices";
            this.grpDevices.TabIndex = 0;
            this.grpDevices.Name = "grpDevices";
            // 
            // tlpDevices
            // 
            this.tlpDevices.Controls.Add(this.dgvDevices, 0, 0);
            this.tlpDevices.Controls.Add(this.lblDeviceCount, 0, 1);
            this.tlpDevices.ColumnCount = 1;
            this.tlpDevices.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDevices.RowCount = 2;
            this.tlpDevices.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDevices.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDevices.TabIndex = 0;
            this.tlpDevices.Name = "tlpDevices";
            // 
            // dgvDevices
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.dgvDevices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colDevName,
                this.colDevIp,
                this.colDevPort,
                this.colDevStatus,
                this.colDevSince,
                this.colDevOnline});
            this.dgvDevices.AllowUserToAddRows = false;
            this.dgvDevices.AllowUserToDeleteRows = false;
            this.dgvDevices.AllowUserToResizeRows = false;
            this.dgvDevices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDevices.BackgroundColor = System.Drawing.Color.White;
            this.dgvDevices.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDevices.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDevices.ColumnHeadersHeight = 32;
            this.dgvDevices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDevices.EnableHeadersVisualStyles = false;
            this.dgvDevices.MultiSelect = false;
            this.dgvDevices.ReadOnly = true;
            this.dgvDevices.RowHeadersVisible = false;
            this.dgvDevices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDevices.TabIndex = 0;
            this.dgvDevices.Name = "dgvDevices";
            // 
            // colDevName
            // 
            this.colDevName.FillWeight = 140F;
            this.colDevName.HeaderText = "Device name";
            this.colDevName.ReadOnly = true;
            this.colDevName.Name = "colDevName";
            // 
            // colDevIp
            // 
            this.colDevIp.FillWeight = 100F;
            this.colDevIp.HeaderText = "IP address";
            this.colDevIp.ReadOnly = true;
            this.colDevIp.Name = "colDevIp";
            // 
            // colDevPort
            // 
            this.colDevPort.FillWeight = 55F;
            this.colDevPort.HeaderText = "Port";
            this.colDevPort.ReadOnly = true;
            this.colDevPort.Name = "colDevPort";
            // 
            // colDevStatus
            // 
            this.colDevStatus.FillWeight = 80F;
            this.colDevStatus.HeaderText = "Status";
            this.colDevStatus.ReadOnly = true;
            this.colDevStatus.Name = "colDevStatus";
            // 
            // colDevSince
            // 
            this.colDevSince.FillWeight = 120F;
            this.colDevSince.HeaderText = "Connection time";
            this.colDevSince.ReadOnly = true;
            this.colDevSince.Name = "colDevSince";
            // 
            // colDevOnline
            // 
            this.colDevOnline.FillWeight = 75F;
            this.colDevOnline.HeaderText = "Online for";
            this.colDevOnline.ReadOnly = true;
            this.colDevOnline.Name = "colDevOnline";
            // 
            // lblDeviceCount
            // 
            this.lblDeviceCount.AutoSize = true;
            this.lblDeviceCount.Margin = new System.Windows.Forms.Padding(3, 6, 3, 0);
            this.lblDeviceCount.Text = "No devices connected.";
            this.lblDeviceCount.TabIndex = 1;
            this.lblDeviceCount.Name = "lblDeviceCount";
            // 
            // grpMessaging
            // 
            this.grpMessaging.Controls.Add(this.tlpMessaging);
            this.grpMessaging.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMessaging.Padding = new System.Windows.Forms.Padding(8, 6, 8, 8);
            this.grpMessaging.Text = "D. Network Messaging and Notifications";
            this.grpMessaging.TabIndex = 1;
            this.grpMessaging.Name = "grpMessaging";
            // 
            // tlpMessaging
            // 
            this.tlpMessaging.Controls.Add(this.tlpSend, 0, 0);
            this.tlpMessaging.Controls.Add(this.dgvLog, 0, 1);
            this.tlpMessaging.Controls.Add(this.flowLogBar, 0, 2);
            this.tlpMessaging.ColumnCount = 1;
            this.tlpMessaging.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMessaging.RowCount = 3;
            this.tlpMessaging.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMessaging.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMessaging.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpMessaging.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMessaging.TabIndex = 0;
            this.tlpMessaging.Name = "tlpMessaging";
            // 
            // tlpSend
            // 
            this.tlpSend.Controls.Add(this.lblTo, 0, 0);
            this.tlpSend.Controls.Add(this.cboRecipient, 1, 0);
            this.tlpSend.Controls.Add(this.txtMessage, 2, 0);
            this.tlpSend.Controls.Add(this.btnSend, 3, 0);
            this.tlpSend.ColumnCount = 4;
            this.tlpSend.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSend.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F));
            this.tlpSend.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSend.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSend.RowCount = 1;
            this.tlpSend.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpSend.AutoSize = true;
            this.tlpSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSend.TabIndex = 0;
            this.tlpSend.Name = "tlpSend";
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.lblTo.Text = "To";
            this.lblTo.TabIndex = 0;
            this.lblTo.Name = "lblTo";
            // 
            // cboRecipient
            // 
            this.cboRecipient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRecipient.FormattingEnabled = true;
            this.cboRecipient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboRecipient.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.cboRecipient.TabIndex = 1;
            this.cboRecipient.Name = "cboRecipient";
            // 
            // txtMessage
            // 
            this.txtMessage.MaxLength = 1000;
            this.txtMessage.PlaceholderText = "Type a message and press Enter";
            this.txtMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.txtMessage.TabIndex = 2;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMessage_KeyDown);
            // 
            // btnSend
            // 
            this.btnSend.AutoSize = true;
            this.btnSend.Margin = new System.Windows.Forms.Padding(3);
            this.btnSend.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSend.TabIndex = 3;
            this.btnSend.Name = "btnSend";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // dgvLog
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(70)))), ((int)(((byte)(120)))));
            this.dgvLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colLogTime,
                this.colLogDir,
                this.colLogType,
                this.colLogPeer,
                this.colLogText});
            this.dgvLog.AllowUserToAddRows = false;
            this.dgvLog.AllowUserToDeleteRows = false;
            this.dgvLog.AllowUserToResizeRows = false;
            this.dgvLog.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLog.BackgroundColor = System.Drawing.Color.White;
            this.dgvLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvLog.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvLog.ColumnHeadersHeight = 32;
            this.dgvLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLog.EnableHeadersVisualStyles = false;
            this.dgvLog.MultiSelect = false;
            this.dgvLog.ReadOnly = true;
            this.dgvLog.RowHeadersVisible = false;
            this.dgvLog.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLog.TabIndex = 1;
            this.dgvLog.Name = "dgvLog";
            // 
            // colLogTime
            // 
            this.colLogTime.FillWeight = 55F;
            this.colLogTime.HeaderText = "Time";
            this.colLogTime.ReadOnly = true;
            this.colLogTime.Name = "colLogTime";
            // 
            // colLogDir
            // 
            this.colLogDir.FillWeight = 32F;
            this.colLogDir.HeaderText = "Dir";
            this.colLogDir.ReadOnly = true;
            this.colLogDir.Name = "colLogDir";
            // 
            // colLogType
            // 
            this.colLogType.FillWeight = 95F;
            this.colLogType.HeaderText = "Type";
            this.colLogType.ReadOnly = true;
            this.colLogType.Name = "colLogType";
            // 
            // colLogPeer
            // 
            this.colLogPeer.FillWeight = 130F;
            this.colLogPeer.HeaderText = "Device / IP";
            this.colLogPeer.ReadOnly = true;
            this.colLogPeer.Name = "colLogPeer";
            // 
            // colLogText
            // 
            this.colLogText.FillWeight = 320F;
            this.colLogText.HeaderText = "Message";
            this.colLogText.ReadOnly = true;
            this.colLogText.Name = "colLogText";
            // 
            // flowLogBar
            // 
            this.flowLogBar.Controls.Add(this.chkToasts);
            this.flowLogBar.Controls.Add(this.btnClearLog);
            this.flowLogBar.AutoSize = true;
            this.flowLogBar.Padding = new System.Windows.Forms.Padding(0);
            this.flowLogBar.TabIndex = 2;
            this.flowLogBar.Name = "flowLogBar";
            // 
            // chkToasts
            // 
            this.chkToasts.AutoSize = true;
            this.chkToasts.Checked = true;
            this.chkToasts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkToasts.Text = "Show pop-up notifications";
            this.chkToasts.UseVisualStyleBackColor = true;
            this.chkToasts.TabIndex = 0;
            this.chkToasts.Name = "chkToasts";
            // 
            // btnClearLog
            // 
            this.btnClearLog.AutoSize = true;
            this.btnClearLog.Margin = new System.Windows.Forms.Padding(3);
            this.btnClearLog.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnClearLog.Text = "Clear log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            // 
            // grpStock
            // 
            this.grpStock.Controls.Add(this.tlpStock);
            this.grpStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpStock.Padding = new System.Windows.Forms.Padding(8, 6, 8, 8);
            this.grpStock.Text = "Inventory over the network (client asks the server)";
            this.grpStock.TabIndex = 2;
            this.grpStock.Name = "grpStock";
            // 
            // tlpStock
            // 
            this.tlpStock.Controls.Add(this.lblSku, 0, 0);
            this.tlpStock.Controls.Add(this.txtSku, 1, 0);
            this.tlpStock.Controls.Add(this.lblDelta, 2, 0);
            this.tlpStock.Controls.Add(this.numDelta, 3, 0);
            this.tlpStock.Controls.Add(this.lblReason, 4, 0);
            this.tlpStock.Controls.Add(this.txtReason, 5, 0);
            this.tlpStock.Controls.Add(this.flowStockButtons, 0, 1);
            this.tlpStock.SetColumnSpan(this.flowStockButtons, 6);
            this.tlpStock.ColumnCount = 6;
            this.tlpStock.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpStock.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlpStock.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpStock.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tlpStock.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpStock.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpStock.RowCount = 2;
            this.tlpStock.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpStock.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpStock.TabIndex = 0;
            this.tlpStock.Name = "tlpStock";
            // 
            // lblSku
            // 
            this.lblSku.AutoSize = true;
            this.lblSku.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.lblSku.Text = "SKU";
            this.lblSku.TabIndex = 0;
            this.lblSku.Name = "lblSku";
            // 
            // txtSku
            // 
            this.txtSku.MaxLength = 32;
            this.txtSku.PlaceholderText = "e.g. EL-001";
            this.txtSku.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSku.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSku.Margin = new System.Windows.Forms.Padding(3, 3, 12, 3);
            this.txtSku.TabIndex = 1;
            this.txtSku.Name = "txtSku";
            // 
            // lblDelta
            // 
            this.lblDelta.AutoSize = true;
            this.lblDelta.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.lblDelta.Text = "Change (+ / -)";
            this.lblDelta.TabIndex = 2;
            this.lblDelta.Name = "lblDelta";
            // 
            // numDelta
            // 
            this.numDelta.Maximum = new decimal(new int[] {100000, 0, 0, 0});
            this.numDelta.Minimum = new decimal(new int[] {100000, 0, 0, -2147483648});
            this.numDelta.Value = new decimal(new int[] {0, 0, 0, 0});
            this.numDelta.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numDelta.Margin = new System.Windows.Forms.Padding(3, 3, 12, 3);
            this.numDelta.TabIndex = 3;
            this.numDelta.Name = "numDelta";
            // 
            // lblReason
            // 
            this.lblReason.AutoSize = true;
            this.lblReason.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this.lblReason.Text = "Reason";
            this.lblReason.TabIndex = 4;
            this.lblReason.Name = "lblReason";
            // 
            // txtReason
            // 
            this.txtReason.MaxLength = 80;
            this.txtReason.PlaceholderText = "e.g. sold at the front desk";
            this.txtReason.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtReason.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtReason.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.txtReason.TabIndex = 5;
            this.txtReason.Name = "txtReason";
            // 
            // flowStockButtons
            // 
            this.flowStockButtons.Controls.Add(this.btnCheckStock);
            this.flowStockButtons.Controls.Add(this.btnApplyStock);
            this.flowStockButtons.Controls.Add(this.lblStockResult);
            this.flowStockButtons.AutoSize = true;
            this.flowStockButtons.Padding = new System.Windows.Forms.Padding(0);
            this.flowStockButtons.TabIndex = 6;
            this.flowStockButtons.Name = "flowStockButtons";
            // 
            // btnCheckStock
            // 
            this.btnCheckStock.AutoSize = true;
            this.btnCheckStock.Margin = new System.Windows.Forms.Padding(3);
            this.btnCheckStock.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnCheckStock.Text = "Check stock on server";
            this.btnCheckStock.UseVisualStyleBackColor = true;
            this.btnCheckStock.TabIndex = 0;
            this.btnCheckStock.Name = "btnCheckStock";
            this.btnCheckStock.Click += new System.EventHandler(this.btnCheckStock_Click);
            // 
            // btnApplyStock
            // 
            this.btnApplyStock.AutoSize = true;
            this.btnApplyStock.Margin = new System.Windows.Forms.Padding(3);
            this.btnApplyStock.MinimumSize = new System.Drawing.Size(84, 30);
            this.btnApplyStock.Text = "Apply change on server";
            this.btnApplyStock.UseVisualStyleBackColor = true;
            this.btnApplyStock.TabIndex = 1;
            this.btnApplyStock.Name = "btnApplyStock";
            this.btnApplyStock.Click += new System.EventHandler(this.btnApplyStock_Click);
            // 
            // lblStockResult
            // 
            this.lblStockResult.AutoSize = true;
            this.lblStockResult.Margin = new System.Windows.Forms.Padding(10, 9, 3, 3);
            this.lblStockResult.MaximumSize = new System.Drawing.Size(560, 0);
            this.lblStockResult.Text = "The server executes the request on ITS database.";
            this.lblStockResult.TabIndex = 2;
            this.lblStockResult.Name = "lblStockResult";
            // 
            // tmrClock
            // 
            this.tmrClock.Interval = 1000;
            this.tmrClock.Tick += new System.EventHandler(this.tmrClock_Tick);
            // 
            // NetworkPanel
            // 
            this.Controls.Add(this.tlpMain);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(1080, 700);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "NetworkPanel";
            this.Size = new System.Drawing.Size(1140, 660);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLog)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDelta)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.tlpLeft.ResumeLayout(false);
            this.tlpLeft.PerformLayout();
            this.grpServer.ResumeLayout(false);
            this.tlpServer.ResumeLayout(false);
            this.tlpServer.PerformLayout();
            this.flowServerButtons.ResumeLayout(false);
            this.flowServerButtons.PerformLayout();
            this.grpClient.ResumeLayout(false);
            this.tlpClient.ResumeLayout(false);
            this.tlpClient.PerformLayout();
            this.flowClientButtons.ResumeLayout(false);
            this.flowClientButtons.PerformLayout();
            this.tlpRight.ResumeLayout(false);
            this.tlpRight.PerformLayout();
            this.grpDevices.ResumeLayout(false);
            this.tlpDevices.ResumeLayout(false);
            this.tlpDevices.PerformLayout();
            this.grpMessaging.ResumeLayout(false);
            this.tlpMessaging.ResumeLayout(false);
            this.tlpMessaging.PerformLayout();
            this.tlpSend.ResumeLayout(false);
            this.tlpSend.PerformLayout();
            this.flowLogBar.ResumeLayout(false);
            this.flowLogBar.PerformLayout();
            this.grpStock.ResumeLayout(false);
            this.tlpStock.ResumeLayout(false);
            this.tlpStock.PerformLayout();
            this.flowStockButtons.ResumeLayout(false);
            this.flowStockButtons.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpLeft;
        private System.Windows.Forms.GroupBox grpServer;
        private System.Windows.Forms.TableLayoutPanel tlpServer;
        private System.Windows.Forms.Label lblBind;
        private System.Windows.Forms.ComboBox cboBind;
        private System.Windows.Forms.Label lblServerPort;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Label lblServerCode;
        private System.Windows.Forms.TextBox txtServerCode;
        private System.Windows.Forms.CheckBox chkAllowPublic;
        private System.Windows.Forms.CheckBox chkBroadcastInventory;
        private System.Windows.Forms.FlowLayoutPanel flowServerButtons;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Button btnStopServer;
        private System.Windows.Forms.Label lblServerStatusCaption;
        private System.Windows.Forms.Label lblServerStatus;
        private System.Windows.Forms.Label lblEndpointCaption;
        private System.Windows.Forms.Label lblServerEndpoint;
        private System.Windows.Forms.Label lblLocalIpCaption;
        private System.Windows.Forms.Label lblLocalIps;
        private System.Windows.Forms.GroupBox grpClient;
        private System.Windows.Forms.TableLayoutPanel tlpClient;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblClientPort;
        private System.Windows.Forms.TextBox txtClientPort;
        private System.Windows.Forms.Label lblDeviceName;
        private System.Windows.Forms.TextBox txtDeviceName;
        private System.Windows.Forms.Label lblClientCode;
        private System.Windows.Forms.TextBox txtClientCode;
        private System.Windows.Forms.FlowLayoutPanel flowClientButtons;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label lblClientStatusCaption;
        private System.Windows.Forms.Label lblClientStatus;
        private System.Windows.Forms.Label lblDurationCaption;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label lblTestCaption;
        private System.Windows.Forms.Label lblTestResult;
        private System.Windows.Forms.TableLayoutPanel tlpRight;
        private System.Windows.Forms.GroupBox grpDevices;
        private System.Windows.Forms.TableLayoutPanel tlpDevices;
        private System.Windows.Forms.DataGridView dgvDevices;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDevName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDevIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDevPort;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDevStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDevSince;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDevOnline;
        private System.Windows.Forms.Label lblDeviceCount;
        private System.Windows.Forms.GroupBox grpMessaging;
        private System.Windows.Forms.TableLayoutPanel tlpMessaging;
        private System.Windows.Forms.TableLayoutPanel tlpSend;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.ComboBox cboRecipient;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.DataGridView dgvLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLogTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLogDir;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLogType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLogPeer;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLogText;
        private System.Windows.Forms.FlowLayoutPanel flowLogBar;
        private System.Windows.Forms.CheckBox chkToasts;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.GroupBox grpStock;
        private System.Windows.Forms.TableLayoutPanel tlpStock;
        private System.Windows.Forms.Label lblSku;
        private System.Windows.Forms.TextBox txtSku;
        private System.Windows.Forms.Label lblDelta;
        private System.Windows.Forms.NumericUpDown numDelta;
        private System.Windows.Forms.Label lblReason;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.FlowLayoutPanel flowStockButtons;
        private System.Windows.Forms.Button btnCheckStock;
        private System.Windows.Forms.Button btnApplyStock;
        private System.Windows.Forms.Label lblStockResult;
        private System.Windows.Forms.Timer tmrClock;
    }
}
