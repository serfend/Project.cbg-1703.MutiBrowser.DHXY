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
			this.clientName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Ip = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.delay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Server = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.OpConnectionCount = new System.Windows.Forms.Label();
			this.OpLogCount = new System.Windows.Forms.Label();
			this.CmdServerOn = new System.Windows.Forms.Button();
			this.CmdSendNotifications = new System.Windows.Forms.Button();
			this.IpSender = new System.Windows.Forms.TextBox();
			this.OpLog = new System.Windows.Forms.TextBox();
			this.LstServerQueue = new System.Windows.Forms.ListView();
			this.serverIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.serverName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.serverHandled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.serverTaskNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.IpTaskInterval = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.IpTaskTimeOut = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.IpPerVPShdl = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.LstGoodShow = new System.Windows.Forms.ListView();
			this.GoodShowServer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.GoodShowName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.GoodShowPriceInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.GoodShowRank = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.GoodShowTalent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.GoodShowAchievement = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.GoodShowSociatyAchievement = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.GoodShowFamilyRank = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.GoodShowBuyUrl = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// LstConnection
			// 
			this.LstConnection.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clientName,
            this.Type,
            this.Ip,
            this.status,
            this.delay,
            this.Server});
			this.LstConnection.FullRowSelect = true;
			this.LstConnection.LabelEdit = true;
			this.LstConnection.Location = new System.Drawing.Point(12, 33);
			this.LstConnection.Name = "LstConnection";
			this.LstConnection.Size = new System.Drawing.Size(529, 631);
			this.LstConnection.TabIndex = 0;
			this.LstConnection.UseCompatibleStateImageBehavior = false;
			this.LstConnection.View = System.Windows.Forms.View.Details;
			// 
			// clientName
			// 
			this.clientName.Text = "ID";
			this.clientName.Width = 73;
			// 
			// Type
			// 
			this.Type.Text = "连接";
			this.Type.Width = 46;
			// 
			// Ip
			// 
			this.Ip.Text = "IP";
			this.Ip.Width = 117;
			// 
			// status
			// 
			this.status.Text = "状态";
			this.status.Width = 141;
			// 
			// delay
			// 
			this.delay.Text = "延迟";
			this.delay.Width = 67;
			// 
			// Server
			// 
			this.Server.Text = "任务";
			this.Server.Width = 134;
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
			this.OpLogCount.Location = new System.Drawing.Point(545, 11);
			this.OpLogCount.Name = "OpLogCount";
			this.OpLogCount.Size = new System.Drawing.Size(53, 12);
			this.OpLogCount.TabIndex = 3;
			this.OpLogCount.Text = "数据日志";
			// 
			// CmdServerOn
			// 
			this.CmdServerOn.Location = new System.Drawing.Point(16, 940);
			this.CmdServerOn.Name = "CmdServerOn";
			this.CmdServerOn.Size = new System.Drawing.Size(99, 34);
			this.CmdServerOn.TabIndex = 4;
			this.CmdServerOn.Text = "发送";
			this.CmdServerOn.UseVisualStyleBackColor = true;
			this.CmdServerOn.Click += new System.EventHandler(this.CmdServerOn_Click);
			// 
			// CmdSendNotifications
			// 
			this.CmdSendNotifications.Location = new System.Drawing.Point(121, 940);
			this.CmdSendNotifications.Name = "CmdSendNotifications";
			this.CmdSendNotifications.Size = new System.Drawing.Size(106, 34);
			this.CmdSendNotifications.TabIndex = 5;
			this.CmdSendNotifications.Text = "新订单";
			this.CmdSendNotifications.UseVisualStyleBackColor = true;
			// 
			// IpSender
			// 
			this.IpSender.Location = new System.Drawing.Point(16, 913);
			this.IpSender.Name = "IpSender";
			this.IpSender.Size = new System.Drawing.Size(211, 21);
			this.IpSender.TabIndex = 6;
			// 
			// OpLog
			// 
			this.OpLog.Location = new System.Drawing.Point(547, 33);
			this.OpLog.Multiline = true;
			this.OpLog.Name = "OpLog";
			this.OpLog.Size = new System.Drawing.Size(366, 106);
			this.OpLog.TabIndex = 1;
			// 
			// LstServerQueue
			// 
			this.LstServerQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.serverIndex,
            this.serverName,
            this.serverHandled,
            this.serverTaskNum});
			this.LstServerQueue.FullRowSelect = true;
			this.LstServerQueue.LabelEdit = true;
			this.LstServerQueue.Location = new System.Drawing.Point(547, 145);
			this.LstServerQueue.Name = "LstServerQueue";
			this.LstServerQueue.Size = new System.Drawing.Size(366, 519);
			this.LstServerQueue.TabIndex = 7;
			this.LstServerQueue.UseCompatibleStateImageBehavior = false;
			this.LstServerQueue.View = System.Windows.Forms.View.Details;
			// 
			// serverIndex
			// 
			this.serverIndex.Text = "区号";
			// 
			// serverName
			// 
			this.serverName.Text = "名称";
			this.serverName.Width = 120;
			// 
			// serverHandled
			// 
			this.serverHandled.Text = "已分配量";
			// 
			// serverTaskNum
			// 
			this.serverTaskNum.Text = "需分配量";
			// 
			// IpTaskInterval
			// 
			this.IpTaskInterval.Location = new System.Drawing.Point(292, 913);
			this.IpTaskInterval.Name = "IpTaskInterval";
			this.IpTaskInterval.Size = new System.Drawing.Size(108, 21);
			this.IpTaskInterval.TabIndex = 9;
			this.IpTaskInterval.Tag = "RecordReg";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(233, 922);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 10;
			this.label1.Text = "采集间隔";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(233, 949);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 12);
			this.label2.TabIndex = 12;
			this.label2.Text = "采集超时";
			// 
			// IpTaskTimeOut
			// 
			this.IpTaskTimeOut.Location = new System.Drawing.Point(292, 940);
			this.IpTaskTimeOut.Name = "IpTaskTimeOut";
			this.IpTaskTimeOut.Size = new System.Drawing.Size(108, 21);
			this.IpTaskTimeOut.TabIndex = 11;
			this.IpTaskTimeOut.Tag = "RecordReg";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(406, 922);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(17, 12);
			this.label3.TabIndex = 13;
			this.label3.Text = "ms";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(406, 948);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(17, 12);
			this.label4.TabIndex = 14;
			this.label4.Text = "ms";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(233, 976);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(53, 12);
			this.label6.TabIndex = 16;
			this.label6.Text = "终端处理";
			// 
			// IpPerVPShdl
			// 
			this.IpPerVPShdl.Location = new System.Drawing.Point(292, 967);
			this.IpPerVPShdl.Name = "IpPerVPShdl";
			this.IpPerVPShdl.Size = new System.Drawing.Size(108, 21);
			this.IpPerVPShdl.TabIndex = 15;
			this.IpPerVPShdl.Tag = "RecordReg";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(406, 976);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 12);
			this.label5.TabIndex = 17;
			this.label5.Text = "个服务器";
			// 
			// LstGoodShow
			// 
			this.LstGoodShow.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.GoodShowServer,
            this.GoodShowName,
            this.GoodShowPriceInfo,
            this.GoodShowRank,
            this.GoodShowTalent,
            this.GoodShowAchievement,
            this.GoodShowSociatyAchievement,
            this.GoodShowFamilyRank,
            this.GoodShowBuyUrl});
			this.LstGoodShow.FullRowSelect = true;
			this.LstGoodShow.LabelEdit = true;
			this.LstGoodShow.Location = new System.Drawing.Point(12, 671);
			this.LstGoodShow.Name = "LstGoodShow";
			this.LstGoodShow.Size = new System.Drawing.Size(901, 224);
			this.LstGoodShow.TabIndex = 18;
			this.LstGoodShow.UseCompatibleStateImageBehavior = false;
			this.LstGoodShow.View = System.Windows.Forms.View.Details;
			this.LstGoodShow.DoubleClick += new System.EventHandler(this.LstGoodShow_DoubleClick);
			// 
			// GoodShowServer
			// 
			this.GoodShowServer.Text = "服务器";
			this.GoodShowServer.Width = 90;
			// 
			// GoodShowName
			// 
			this.GoodShowName.Text = "名称";
			this.GoodShowName.Width = 96;
			// 
			// GoodShowPriceInfo
			// 
			this.GoodShowPriceInfo.Text = "价格";
			this.GoodShowPriceInfo.Width = 77;
			// 
			// GoodShowRank
			// 
			this.GoodShowRank.Text = "等级";
			// 
			// GoodShowTalent
			// 
			this.GoodShowTalent.Text = "天赋";
			// 
			// GoodShowAchievement
			// 
			this.GoodShowAchievement.Text = "功绩";
			// 
			// GoodShowSociatyAchievement
			// 
			this.GoodShowSociatyAchievement.Text = "帮派点数";
			// 
			// GoodShowFamilyRank
			// 
			this.GoodShowFamilyRank.Text = "家族回灵";
			// 
			// GoodShowBuyUrl
			// 
			this.GoodShowBuyUrl.Text = "购买链接";
			this.GoodShowBuyUrl.Width = 333;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(920, 996);
			this.Controls.Add(this.LstGoodShow);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.IpPerVPShdl);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.IpTaskTimeOut);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.IpTaskInterval);
			this.Controls.Add(this.LstServerQueue);
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
		private System.Windows.Forms.Label OpConnectionCount;
		private System.Windows.Forms.Label OpLogCount;
		private System.Windows.Forms.Button CmdServerOn;
		private System.Windows.Forms.Button CmdSendNotifications;
		private System.Windows.Forms.ColumnHeader Type;
		private System.Windows.Forms.ColumnHeader Ip;
		private System.Windows.Forms.ColumnHeader status;
		private System.Windows.Forms.TextBox IpSender;
		private System.Windows.Forms.ColumnHeader clientName;
		private System.Windows.Forms.TextBox OpLog;
		private System.Windows.Forms.ColumnHeader delay;
		private System.Windows.Forms.ListView LstServerQueue;
		private System.Windows.Forms.ColumnHeader serverIndex;
		private System.Windows.Forms.ColumnHeader serverName;
		private System.Windows.Forms.ColumnHeader serverHandled;
		private System.Windows.Forms.ColumnHeader serverTaskNum;
		private System.Windows.Forms.ColumnHeader Server;
		private System.Windows.Forms.TextBox IpTaskInterval;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox IpTaskTimeOut;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox IpPerVPShdl;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ListView LstGoodShow;
		private System.Windows.Forms.ColumnHeader GoodShowServer;
		private System.Windows.Forms.ColumnHeader GoodShowName;
		private System.Windows.Forms.ColumnHeader GoodShowPriceInfo;
		private System.Windows.Forms.ColumnHeader GoodShowRank;
		private System.Windows.Forms.ColumnHeader GoodShowTalent;
		private System.Windows.Forms.ColumnHeader GoodShowAchievement;
		private System.Windows.Forms.ColumnHeader GoodShowSociatyAchievement;
		private System.Windows.Forms.ColumnHeader GoodShowFamilyRank;
		private System.Windows.Forms.ColumnHeader GoodShowBuyUrl;
	}
}

