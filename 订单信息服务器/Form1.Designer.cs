namespace 订单信息服务器
{
	partial class Form1
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
				if(transferFileEngine!=null)transferFileEngine.Dispose();
				transferFileEngine = null;
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.LstConnection = new System.Windows.Forms.ListView();
			this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Ip = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.OpLog = new System.Windows.Forms.TextBox();
			this.OpConnectionCount = new System.Windows.Forms.Label();
			this.OpLogCount = new System.Windows.Forms.Label();
			this.CmdServerOn = new System.Windows.Forms.Button();
			this.CmdSendNotifications = new System.Windows.Forms.Button();
			this.IpSender = new System.Windows.Forms.TextBox();
			this.clientName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// LstConnection
			// 
			this.LstConnection.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clientName,
            this.Type,
            this.Ip,
            this.status});
			this.LstConnection.FullRowSelect = true;
			this.LstConnection.LabelEdit = true;
			this.LstConnection.Location = new System.Drawing.Point(12, 33);
			this.LstConnection.Name = "LstConnection";
			this.LstConnection.Size = new System.Drawing.Size(475, 452);
			this.LstConnection.TabIndex = 0;
			this.LstConnection.UseCompatibleStateImageBehavior = false;
			this.LstConnection.View = System.Windows.Forms.View.Details;
			this.LstConnection.SelectedIndexChanged += new System.EventHandler(this.LstConnection_SelectedIndexChanged);
			// 
			// Type
			// 
			this.Type.Text = "连接";
			this.Type.Width = 46;
			// 
			// Ip
			// 
			this.Ip.Text = "IP";
			this.Ip.Width = 144;
			// 
			// status
			// 
			this.status.Text = "状态";
			this.status.Width = 111;
			// 
			// OpLog
			// 
			this.OpLog.Location = new System.Drawing.Point(511, 33);
			this.OpLog.Multiline = true;
			this.OpLog.Name = "OpLog";
			this.OpLog.Size = new System.Drawing.Size(366, 488);
			this.OpLog.TabIndex = 1;
			// 
			// OpConnectionCount
			// 
			this.OpConnectionCount.AutoSize = true;
			this.OpConnectionCount.Location = new System.Drawing.Point(14, 11);
			this.OpConnectionCount.Name = "OpConnectionCount";
			this.OpConnectionCount.Size = new System.Drawing.Size(53, 12);
			this.OpConnectionCount.TabIndex = 2;
			this.OpConnectionCount.Text = "当前连接";
			// 
			// OpLogCount
			// 
			this.OpLogCount.AutoSize = true;
			this.OpLogCount.Location = new System.Drawing.Point(509, 11);
			this.OpLogCount.Name = "OpLogCount";
			this.OpLogCount.Size = new System.Drawing.Size(53, 12);
			this.OpLogCount.TabIndex = 3;
			this.OpLogCount.Text = "数据日志";
			// 
			// CmdServerOn
			// 
			this.CmdServerOn.Location = new System.Drawing.Point(12, 517);
			this.CmdServerOn.Name = "CmdServerOn";
			this.CmdServerOn.Size = new System.Drawing.Size(99, 34);
			this.CmdServerOn.TabIndex = 4;
			this.CmdServerOn.Text = "发送";
			this.CmdServerOn.UseVisualStyleBackColor = true;
			this.CmdServerOn.Click += new System.EventHandler(this.CmdServerOn_Click);
			// 
			// CmdSendNotifications
			// 
			this.CmdSendNotifications.Location = new System.Drawing.Point(131, 517);
			this.CmdSendNotifications.Name = "CmdSendNotifications";
			this.CmdSendNotifications.Size = new System.Drawing.Size(106, 34);
			this.CmdSendNotifications.TabIndex = 5;
			this.CmdSendNotifications.Text = "新订单";
			this.CmdSendNotifications.UseVisualStyleBackColor = true;
			// 
			// IpSender
			// 
			this.IpSender.Location = new System.Drawing.Point(12, 490);
			this.IpSender.Name = "IpSender";
			this.IpSender.Size = new System.Drawing.Size(225, 21);
			this.IpSender.TabIndex = 6;
			// 
			// clientName
			// 
			this.clientName.DisplayIndex = 0;
			this.clientName.Text = "ID";
			this.clientName.Width = 130;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(889, 578);
			this.Controls.Add(this.IpSender);
			this.Controls.Add(this.CmdSendNotifications);
			this.Controls.Add(this.CmdServerOn);
			this.Controls.Add(this.OpLogCount);
			this.Controls.Add(this.OpConnectionCount);
			this.Controls.Add(this.OpLog);
			this.Controls.Add(this.LstConnection);
			this.Name = "Form1";
			this.Text = "服务器";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView LstConnection;
		private System.Windows.Forms.TextBox OpLog;
		private System.Windows.Forms.Label OpConnectionCount;
		private System.Windows.Forms.Label OpLogCount;
		private System.Windows.Forms.Button CmdServerOn;
		private System.Windows.Forms.Button CmdSendNotifications;
		private System.Windows.Forms.ColumnHeader Type;
		private System.Windows.Forms.ColumnHeader Ip;
		private System.Windows.Forms.ColumnHeader status;
		private System.Windows.Forms.TextBox IpSender;
		private System.Windows.Forms.ColumnHeader clientName;
	}
}

