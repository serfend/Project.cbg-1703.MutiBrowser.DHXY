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
			this.TabMain = new System.Windows.Forms.TabControl();
			this.TabMain_VpsManager = new System.Windows.Forms.TabPage();
			this.CmdPayBill = new System.Windows.Forms.Button();
			this.CmdPauseTaskAllocate = new System.Windows.Forms.Button();
			this.CmdRedial = new System.Windows.Forms.Button();
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
			this.GoodShowServerNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.OpLogCount = new System.Windows.Forms.Label();
			this.IpSender = new System.Windows.Forms.TextBox();
			this.CmdDisconnect = new System.Windows.Forms.Button();
			this.CmdServerOn = new System.Windows.Forms.Button();
			this.OpLog = new System.Windows.Forms.TextBox();
			this.OpConnectionCount = new System.Windows.Forms.Label();
			this.LstConnection = new System.Windows.Forms.ListView();
			this.LstConnection_ClientName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.LstConnection_Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.LstConnection_Ip = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.LstConnection_status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.LstConnection_delay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.LstConnection_Server = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.LstConnection_version = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.TabMain_Setting = new System.Windows.Forms.TabPage();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.IpAssumePrice_Rate = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.IpPerVPShdl = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.IpTaskInterval = new System.Windows.Forms.TextBox();
			this.TabMain_Pay = new System.Windows.Forms.TabPage();
			this.OpAuthCodeShow = new System.Windows.Forms.Label();
			this.CmdPay_EditVerify = new System.Windows.Forms.Button();
			this.CmdPay_NewVerify = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.LstPayClient = new System.Windows.Forms.ListView();
			this.PhoneNum = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Verify = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ServerHdl = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Psw = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.TabMain.SuspendLayout();
			this.TabMain_VpsManager.SuspendLayout();
			this.TabMain_Setting.SuspendLayout();
			this.TabMain_Pay.SuspendLayout();
			this.SuspendLayout();
			// 
			// TabMain
			// 
			this.TabMain.Controls.Add(this.TabMain_VpsManager);
			this.TabMain.Controls.Add(this.TabMain_Setting);
			this.TabMain.Controls.Add(this.TabMain_Pay);
			this.TabMain.Location = new System.Drawing.Point(2, -1);
			this.TabMain.Name = "TabMain";
			this.TabMain.SelectedIndex = 0;
			this.TabMain.Size = new System.Drawing.Size(917, 991);
			this.TabMain.TabIndex = 0;
			// 
			// TabMain_VpsManager
			// 
			this.TabMain_VpsManager.Controls.Add(this.CmdPayBill);
			this.TabMain_VpsManager.Controls.Add(this.CmdPauseTaskAllocate);
			this.TabMain_VpsManager.Controls.Add(this.CmdRedial);
			this.TabMain_VpsManager.Controls.Add(this.LstGoodShow);
			this.TabMain_VpsManager.Controls.Add(this.OpLogCount);
			this.TabMain_VpsManager.Controls.Add(this.IpSender);
			this.TabMain_VpsManager.Controls.Add(this.CmdDisconnect);
			this.TabMain_VpsManager.Controls.Add(this.CmdServerOn);
			this.TabMain_VpsManager.Controls.Add(this.OpLog);
			this.TabMain_VpsManager.Controls.Add(this.OpConnectionCount);
			this.TabMain_VpsManager.Controls.Add(this.LstConnection);
			this.TabMain_VpsManager.Location = new System.Drawing.Point(4, 22);
			this.TabMain_VpsManager.Name = "TabMain_VpsManager";
			this.TabMain_VpsManager.Padding = new System.Windows.Forms.Padding(3);
			this.TabMain_VpsManager.Size = new System.Drawing.Size(909, 965);
			this.TabMain_VpsManager.TabIndex = 0;
			this.TabMain_VpsManager.Text = "终端管理";
			this.TabMain_VpsManager.UseVisualStyleBackColor = true;
			// 
			// CmdPayBill
			// 
			this.CmdPayBill.Location = new System.Drawing.Point(464, 690);
			this.CmdPayBill.Name = "CmdPayBill";
			this.CmdPayBill.Size = new System.Drawing.Size(69, 34);
			this.CmdPayBill.TabIndex = 39;
			this.CmdPayBill.Text = "付款";
			this.CmdPayBill.UseVisualStyleBackColor = true;
			this.CmdPayBill.Click += new System.EventHandler(this.CmdPayBill_Click);
			// 
			// CmdPauseTaskAllocate
			// 
			this.CmdPauseTaskAllocate.Location = new System.Drawing.Point(262, 690);
			this.CmdPauseTaskAllocate.Name = "CmdPauseTaskAllocate";
			this.CmdPauseTaskAllocate.Size = new System.Drawing.Size(88, 34);
			this.CmdPauseTaskAllocate.TabIndex = 38;
			this.CmdPauseTaskAllocate.Text = "暂停终端";
			this.CmdPauseTaskAllocate.UseVisualStyleBackColor = true;
			this.CmdPauseTaskAllocate.Click += new System.EventHandler(this.CmdPauseTaskAllocate_Click);
			// 
			// CmdRedial
			// 
			this.CmdRedial.Location = new System.Drawing.Point(168, 690);
			this.CmdRedial.Name = "CmdRedial";
			this.CmdRedial.Size = new System.Drawing.Size(88, 34);
			this.CmdRedial.TabIndex = 37;
			this.CmdRedial.Text = "重新拨号(vps)";
			this.CmdRedial.UseVisualStyleBackColor = true;
			this.CmdRedial.Click += new System.EventHandler(this.CmdRedial_Click);
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
            this.GoodShowBuyUrl,
            this.GoodShowServerNo});
			this.LstGoodShow.FullRowSelect = true;
			this.LstGoodShow.Location = new System.Drawing.Point(4, 730);
			this.LstGoodShow.Name = "LstGoodShow";
			this.LstGoodShow.Size = new System.Drawing.Size(901, 224);
			this.LstGoodShow.TabIndex = 36;
			this.LstGoodShow.UseCompatibleStateImageBehavior = false;
			this.LstGoodShow.View = System.Windows.Forms.View.Details;
			this.LstGoodShow.DoubleClick += new System.EventHandler(this.LstGoodShow_DoubleClick_1);
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
			// GoodShowServerNo
			// 
			this.GoodShowServerNo.Text = "区号";
			// 
			// OpLogCount
			// 
			this.OpLogCount.AutoSize = true;
			this.OpLogCount.Location = new System.Drawing.Point(537, 4);
			this.OpLogCount.Name = "OpLogCount";
			this.OpLogCount.Size = new System.Drawing.Size(53, 12);
			this.OpLogCount.TabIndex = 22;
			this.OpLogCount.Text = "数据日志";
			// 
			// IpSender
			// 
			this.IpSender.Location = new System.Drawing.Point(4, 663);
			this.IpSender.Name = "IpSender";
			this.IpSender.Size = new System.Drawing.Size(529, 21);
			this.IpSender.TabIndex = 25;
			// 
			// CmdDisconnect
			// 
			this.CmdDisconnect.Location = new System.Drawing.Point(79, 690);
			this.CmdDisconnect.Name = "CmdDisconnect";
			this.CmdDisconnect.Size = new System.Drawing.Size(83, 34);
			this.CmdDisconnect.TabIndex = 24;
			this.CmdDisconnect.Text = "断开连接";
			this.CmdDisconnect.UseVisualStyleBackColor = true;
			this.CmdDisconnect.Click += new System.EventHandler(this.CmdDisconnect_Click);
			// 
			// CmdServerOn
			// 
			this.CmdServerOn.Location = new System.Drawing.Point(4, 690);
			this.CmdServerOn.Name = "CmdServerOn";
			this.CmdServerOn.Size = new System.Drawing.Size(69, 34);
			this.CmdServerOn.TabIndex = 23;
			this.CmdServerOn.Text = "发送";
			this.CmdServerOn.UseVisualStyleBackColor = true;
			this.CmdServerOn.Click += new System.EventHandler(this.CmdServerOn_Click);
			// 
			// OpLog
			// 
			this.OpLog.Location = new System.Drawing.Point(539, 26);
			this.OpLog.Multiline = true;
			this.OpLog.Name = "OpLog";
			this.OpLog.Size = new System.Drawing.Size(366, 551);
			this.OpLog.TabIndex = 20;
			// 
			// OpConnectionCount
			// 
			this.OpConnectionCount.AutoSize = true;
			this.OpConnectionCount.Location = new System.Drawing.Point(6, 4);
			this.OpConnectionCount.Name = "OpConnectionCount";
			this.OpConnectionCount.Size = new System.Drawing.Size(53, 12);
			this.OpConnectionCount.TabIndex = 21;
			this.OpConnectionCount.Text = "当前连接";
			// 
			// LstConnection
			// 
			this.LstConnection.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LstConnection_ClientName,
            this.LstConnection_Type,
            this.LstConnection_Ip,
            this.LstConnection_status,
            this.LstConnection_delay,
            this.LstConnection_Server,
            this.LstConnection_version});
			this.LstConnection.FullRowSelect = true;
			this.LstConnection.LabelEdit = true;
			this.LstConnection.Location = new System.Drawing.Point(4, 26);
			this.LstConnection.Name = "LstConnection";
			this.LstConnection.Size = new System.Drawing.Size(529, 631);
			this.LstConnection.TabIndex = 19;
			this.LstConnection.UseCompatibleStateImageBehavior = false;
			this.LstConnection.View = System.Windows.Forms.View.Details;
			// 
			// LstConnection_ClientName
			// 
			this.LstConnection_ClientName.Text = "ID";
			this.LstConnection_ClientName.Width = 57;
			// 
			// LstConnection_Type
			// 
			this.LstConnection_Type.Text = "连接";
			this.LstConnection_Type.Width = 46;
			// 
			// LstConnection_Ip
			// 
			this.LstConnection_Ip.Text = "端口";
			this.LstConnection_Ip.Width = 57;
			// 
			// LstConnection_status
			// 
			this.LstConnection_status.Text = "状态";
			this.LstConnection_status.Width = 141;
			// 
			// LstConnection_delay
			// 
			this.LstConnection_delay.Text = "延迟";
			this.LstConnection_delay.Width = 48;
			// 
			// LstConnection_Server
			// 
			this.LstConnection_Server.Text = "任务";
			this.LstConnection_Server.Width = 77;
			// 
			// LstConnection_version
			// 
			this.LstConnection_version.Text = "版本";
			this.LstConnection_version.Width = 120;
			// 
			// TabMain_Setting
			// 
			this.TabMain_Setting.Controls.Add(this.label7);
			this.TabMain_Setting.Controls.Add(this.label8);
			this.TabMain_Setting.Controls.Add(this.IpAssumePrice_Rate);
			this.TabMain_Setting.Controls.Add(this.label5);
			this.TabMain_Setting.Controls.Add(this.label6);
			this.TabMain_Setting.Controls.Add(this.IpPerVPShdl);
			this.TabMain_Setting.Controls.Add(this.label3);
			this.TabMain_Setting.Controls.Add(this.label1);
			this.TabMain_Setting.Controls.Add(this.IpTaskInterval);
			this.TabMain_Setting.Location = new System.Drawing.Point(4, 22);
			this.TabMain_Setting.Name = "TabMain_Setting";
			this.TabMain_Setting.Padding = new System.Windows.Forms.Padding(3);
			this.TabMain_Setting.Size = new System.Drawing.Size(909, 965);
			this.TabMain_Setting.TabIndex = 1;
			this.TabMain_Setting.Text = "设置";
			this.TabMain_Setting.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(179, 75);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(11, 12);
			this.label7.TabIndex = 47;
			this.label7.Text = "%";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(6, 75);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(53, 12);
			this.label8.TabIndex = 46;
			this.label8.Text = "估价比率";
			// 
			// IpAssumePrice_Rate
			// 
			this.IpAssumePrice_Rate.Location = new System.Drawing.Point(65, 66);
			this.IpAssumePrice_Rate.Name = "IpAssumePrice_Rate";
			this.IpAssumePrice_Rate.Size = new System.Drawing.Size(108, 21);
			this.IpAssumePrice_Rate.TabIndex = 45;
			this.IpAssumePrice_Rate.Tag = "RecordReg";
			this.IpAssumePrice_Rate.Text = "100";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(179, 48);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 12);
			this.label5.TabIndex = 44;
			this.label5.Text = "个服务器";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(53, 12);
			this.label6.TabIndex = 43;
			this.label6.Text = "终端处理";
			// 
			// IpPerVPShdl
			// 
			this.IpPerVPShdl.Location = new System.Drawing.Point(65, 39);
			this.IpPerVPShdl.Name = "IpPerVPShdl";
			this.IpPerVPShdl.Size = new System.Drawing.Size(108, 21);
			this.IpPerVPShdl.TabIndex = 42;
			this.IpPerVPShdl.Tag = "RecordReg";
			this.IpPerVPShdl.Text = "1";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(179, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(17, 12);
			this.label3.TabIndex = 40;
			this.label3.Text = "ms";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 37;
			this.label1.Text = "采集间隔";
			// 
			// IpTaskInterval
			// 
			this.IpTaskInterval.Location = new System.Drawing.Point(65, 15);
			this.IpTaskInterval.Name = "IpTaskInterval";
			this.IpTaskInterval.Size = new System.Drawing.Size(108, 21);
			this.IpTaskInterval.TabIndex = 36;
			this.IpTaskInterval.Tag = "RecordReg";
			this.IpTaskInterval.Text = "0";
			// 
			// TabMain_Pay
			// 
			this.TabMain_Pay.Controls.Add(this.OpAuthCodeShow);
			this.TabMain_Pay.Controls.Add(this.CmdPay_EditVerify);
			this.TabMain_Pay.Controls.Add(this.CmdPay_NewVerify);
			this.TabMain_Pay.Controls.Add(this.label2);
			this.TabMain_Pay.Controls.Add(this.LstPayClient);
			this.TabMain_Pay.Location = new System.Drawing.Point(4, 22);
			this.TabMain_Pay.Name = "TabMain_Pay";
			this.TabMain_Pay.Size = new System.Drawing.Size(909, 965);
			this.TabMain_Pay.TabIndex = 2;
			this.TabMain_Pay.Text = "付款";
			this.TabMain_Pay.UseVisualStyleBackColor = true;
			// 
			// OpAuthCodeShow
			// 
			this.OpAuthCodeShow.AutoSize = true;
			this.OpAuthCodeShow.Location = new System.Drawing.Point(118, 10);
			this.OpAuthCodeShow.Name = "OpAuthCodeShow";
			this.OpAuthCodeShow.Size = new System.Drawing.Size(47, 12);
			this.OpAuthCodeShow.TabIndex = 4;
			this.OpAuthCodeShow.Text = "将军令:";
			// 
			// CmdPay_EditVerify
			// 
			this.CmdPay_EditVerify.Location = new System.Drawing.Point(118, 790);
			this.CmdPay_EditVerify.Name = "CmdPay_EditVerify";
			this.CmdPay_EditVerify.Size = new System.Drawing.Size(98, 33);
			this.CmdPay_EditVerify.TabIndex = 3;
			this.CmdPay_EditVerify.Text = "编辑";
			this.CmdPay_EditVerify.UseVisualStyleBackColor = true;
			this.CmdPay_EditVerify.Click += new System.EventHandler(this.CmdPay_EditVerify_Click);
			// 
			// CmdPay_NewVerify
			// 
			this.CmdPay_NewVerify.Location = new System.Drawing.Point(14, 790);
			this.CmdPay_NewVerify.Name = "CmdPay_NewVerify";
			this.CmdPay_NewVerify.Size = new System.Drawing.Size(98, 33);
			this.CmdPay_NewVerify.TabIndex = 2;
			this.CmdPay_NewVerify.Text = "新增";
			this.CmdPay_NewVerify.UseVisualStyleBackColor = true;
			this.CmdPay_NewVerify.Click += new System.EventHandler(this.CmdPay_NewVerify_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(101, 12);
			this.label2.TabIndex = 1;
			this.label2.Text = "付款登录凭证管理";
			// 
			// LstPayClient
			// 
			this.LstPayClient.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.PhoneNum,
            this.Verify,
            this.ServerHdl,
            this.Psw});
			this.LstPayClient.FullRowSelect = true;
			this.LstPayClient.Location = new System.Drawing.Point(13, 25);
			this.LstPayClient.Name = "LstPayClient";
			this.LstPayClient.Size = new System.Drawing.Size(690, 754);
			this.LstPayClient.TabIndex = 0;
			this.LstPayClient.UseCompatibleStateImageBehavior = false;
			this.LstPayClient.View = System.Windows.Forms.View.Details;
			// 
			// PhoneNum
			// 
			this.PhoneNum.Text = "浏览器名称";
			this.PhoneNum.Width = 120;
			// 
			// Verify
			// 
			this.Verify.Text = "凭证";
			this.Verify.Width = 40;
			// 
			// ServerHdl
			// 
			this.ServerHdl.Text = "管理区";
			this.ServerHdl.Width = 300;
			// 
			// Psw
			// 
			this.Psw.Text = "密码";
			this.Psw.Width = 150;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(920, 1031);
			this.Controls.Add(this.TabMain);
			this.Name = "Form1";
			this.Text = "服务器";
			this.TabMain.ResumeLayout(false);
			this.TabMain_VpsManager.ResumeLayout(false);
			this.TabMain_VpsManager.PerformLayout();
			this.TabMain_Setting.ResumeLayout(false);
			this.TabMain_Setting.PerformLayout();
			this.TabMain_Pay.ResumeLayout(false);
			this.TabMain_Pay.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl TabMain;
		private System.Windows.Forms.TabPage TabMain_VpsManager;
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
		private System.Windows.Forms.Label OpLogCount;
		private System.Windows.Forms.TextBox IpSender;
		private System.Windows.Forms.Button CmdDisconnect;
		private System.Windows.Forms.Button CmdServerOn;
		private System.Windows.Forms.TextBox OpLog;
		private System.Windows.Forms.Label OpConnectionCount;
		private System.Windows.Forms.ListView LstConnection;
		private System.Windows.Forms.ColumnHeader LstConnection_ClientName;
		private System.Windows.Forms.ColumnHeader LstConnection_Type;
		private System.Windows.Forms.ColumnHeader LstConnection_Ip;
		private System.Windows.Forms.ColumnHeader LstConnection_status;
		private System.Windows.Forms.ColumnHeader LstConnection_delay;
		private System.Windows.Forms.ColumnHeader LstConnection_Server;
		private System.Windows.Forms.TabPage TabMain_Setting;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox IpPerVPShdl;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox IpTaskInterval;
		private System.Windows.Forms.Button CmdRedial;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox IpAssumePrice_Rate;
		private System.Windows.Forms.Button CmdPauseTaskAllocate;
		private System.Windows.Forms.ColumnHeader LstConnection_version;
		private System.Windows.Forms.TabPage TabMain_Pay;
		private System.Windows.Forms.Button CmdPay_EditVerify;
		private System.Windows.Forms.Button CmdPay_NewVerify;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView LstPayClient;
		private System.Windows.Forms.ColumnHeader PhoneNum;
		private System.Windows.Forms.ColumnHeader Verify;
		private System.Windows.Forms.ColumnHeader ServerHdl;
		private System.Windows.Forms.ColumnHeader Psw;
		private System.Windows.Forms.Label OpAuthCodeShow;
		private System.Windows.Forms.Button CmdPayBill;
		private System.Windows.Forms.ColumnHeader GoodShowServerNo;
	}
}

