﻿namespace 订单信息服务器
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
			this.OpLogCount = new System.Windows.Forms.Label();
			this.LstServerQueue = new System.Windows.Forms.ListView();
			this.serverTask_Index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.serverTask_Name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.serverTask_Handled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.serverTask_Num = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.IpSender = new System.Windows.Forms.TextBox();
			this.CmdDisconnect = new System.Windows.Forms.Button();
			this.CmdServerOn = new System.Windows.Forms.Button();
			this.OpLog = new System.Windows.Forms.TextBox();
			this.OpConnectionCount = new System.Windows.Forms.Label();
			this.LstConnection = new System.Windows.Forms.ListView();
			this.clientName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Ip = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.delay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Server = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.TabMain_Setting = new System.Windows.Forms.TabPage();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.IpPerVPShdl = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.IpTaskTimeOut = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.IpTaskInterval = new System.Windows.Forms.TextBox();
			this.CmdRedial = new System.Windows.Forms.Button();
			this.serverTask_Enable = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.TabMain.SuspendLayout();
			this.TabMain_VpsManager.SuspendLayout();
			this.TabMain_Setting.SuspendLayout();
			this.SuspendLayout();
			// 
			// TabMain
			// 
			this.TabMain.Controls.Add(this.TabMain_VpsManager);
			this.TabMain.Controls.Add(this.TabMain_Setting);
			this.TabMain.Location = new System.Drawing.Point(2, -1);
			this.TabMain.Name = "TabMain";
			this.TabMain.SelectedIndex = 0;
			this.TabMain.Size = new System.Drawing.Size(917, 991);
			this.TabMain.TabIndex = 0;
			// 
			// TabMain_VpsManager
			// 
			this.TabMain_VpsManager.Controls.Add(this.CmdRedial);
			this.TabMain_VpsManager.Controls.Add(this.LstGoodShow);
			this.TabMain_VpsManager.Controls.Add(this.OpLogCount);
			this.TabMain_VpsManager.Controls.Add(this.LstServerQueue);
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
			this.LstGoodShow.Location = new System.Drawing.Point(4, 730);
			this.LstGoodShow.Name = "LstGoodShow";
			this.LstGoodShow.Size = new System.Drawing.Size(901, 224);
			this.LstGoodShow.TabIndex = 36;
			this.LstGoodShow.UseCompatibleStateImageBehavior = false;
			this.LstGoodShow.View = System.Windows.Forms.View.Details;
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
			// OpLogCount
			// 
			this.OpLogCount.AutoSize = true;
			this.OpLogCount.Location = new System.Drawing.Point(537, 4);
			this.OpLogCount.Name = "OpLogCount";
			this.OpLogCount.Size = new System.Drawing.Size(53, 12);
			this.OpLogCount.TabIndex = 22;
			this.OpLogCount.Text = "数据日志";
			// 
			// LstServerQueue
			// 
			this.LstServerQueue.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.serverTask_Index,
            this.serverTask_Name,
            this.serverTask_Handled,
            this.serverTask_Num,
            this.serverTask_Enable});
			this.LstServerQueue.FullRowSelect = true;
			this.LstServerQueue.Location = new System.Drawing.Point(539, 138);
			this.LstServerQueue.Name = "LstServerQueue";
			this.LstServerQueue.Size = new System.Drawing.Size(366, 586);
			this.LstServerQueue.TabIndex = 26;
			this.LstServerQueue.UseCompatibleStateImageBehavior = false;
			this.LstServerQueue.View = System.Windows.Forms.View.Details;
			this.LstServerQueue.DoubleClick += new System.EventHandler(this.LstServerQueue_DoubleClick);
			// 
			// serverTask_Index
			// 
			this.serverTask_Index.Text = "区号";
			// 
			// serverTask_Name
			// 
			this.serverTask_Name.Text = "名称";
			this.serverTask_Name.Width = 120;
			// 
			// serverTask_Handled
			// 
			this.serverTask_Handled.Text = "已分配量";
			// 
			// serverTask_Num
			// 
			this.serverTask_Num.Text = "需分配量";
			// 
			// IpSender
			// 
			this.IpSender.Location = new System.Drawing.Point(4, 663);
			this.IpSender.Name = "IpSender";
			this.IpSender.Size = new System.Drawing.Size(211, 21);
			this.IpSender.TabIndex = 25;
			// 
			// CmdDisconnect
			// 
			this.CmdDisconnect.Location = new System.Drawing.Point(221, 663);
			this.CmdDisconnect.Name = "CmdDisconnect";
			this.CmdDisconnect.Size = new System.Drawing.Size(106, 21);
			this.CmdDisconnect.TabIndex = 24;
			this.CmdDisconnect.Text = "断开连接";
			this.CmdDisconnect.UseVisualStyleBackColor = true;
			this.CmdDisconnect.Click += new System.EventHandler(this.CmdDisconnect_Click);
			// 
			// CmdServerOn
			// 
			this.CmdServerOn.Location = new System.Drawing.Point(4, 690);
			this.CmdServerOn.Name = "CmdServerOn";
			this.CmdServerOn.Size = new System.Drawing.Size(99, 34);
			this.CmdServerOn.TabIndex = 23;
			this.CmdServerOn.Text = "发送";
			this.CmdServerOn.UseVisualStyleBackColor = true;
			// 
			// OpLog
			// 
			this.OpLog.Location = new System.Drawing.Point(539, 26);
			this.OpLog.Multiline = true;
			this.OpLog.Name = "OpLog";
			this.OpLog.Size = new System.Drawing.Size(366, 106);
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
            this.clientName,
            this.Type,
            this.Ip,
            this.status,
            this.delay,
            this.Server});
			this.LstConnection.FullRowSelect = true;
			this.LstConnection.LabelEdit = true;
			this.LstConnection.Location = new System.Drawing.Point(4, 26);
			this.LstConnection.Name = "LstConnection";
			this.LstConnection.Size = new System.Drawing.Size(529, 631);
			this.LstConnection.TabIndex = 19;
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
			// TabMain_Setting
			// 
			this.TabMain_Setting.Controls.Add(this.label5);
			this.TabMain_Setting.Controls.Add(this.label6);
			this.TabMain_Setting.Controls.Add(this.IpPerVPShdl);
			this.TabMain_Setting.Controls.Add(this.label4);
			this.TabMain_Setting.Controls.Add(this.label3);
			this.TabMain_Setting.Controls.Add(this.label2);
			this.TabMain_Setting.Controls.Add(this.IpTaskTimeOut);
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
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(179, 78);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 12);
			this.label5.TabIndex = 44;
			this.label5.Text = "个服务器";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 78);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(53, 12);
			this.label6.TabIndex = 43;
			this.label6.Text = "终端处理";
			// 
			// IpPerVPShdl
			// 
			this.IpPerVPShdl.Location = new System.Drawing.Point(65, 69);
			this.IpPerVPShdl.Name = "IpPerVPShdl";
			this.IpPerVPShdl.Size = new System.Drawing.Size(108, 21);
			this.IpPerVPShdl.TabIndex = 42;
			this.IpPerVPShdl.Tag = "RecordReg";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(179, 50);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(17, 12);
			this.label4.TabIndex = 41;
			this.label4.Text = "ms";
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
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 12);
			this.label2.TabIndex = 39;
			this.label2.Text = "采集超时";
			// 
			// IpTaskTimeOut
			// 
			this.IpTaskTimeOut.Location = new System.Drawing.Point(65, 42);
			this.IpTaskTimeOut.Name = "IpTaskTimeOut";
			this.IpTaskTimeOut.Size = new System.Drawing.Size(108, 21);
			this.IpTaskTimeOut.TabIndex = 38;
			this.IpTaskTimeOut.Tag = "RecordReg";
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
			// 
			// CmdRedial
			// 
			this.CmdRedial.Location = new System.Drawing.Point(221, 690);
			this.CmdRedial.Name = "CmdRedial";
			this.CmdRedial.Size = new System.Drawing.Size(106, 21);
			this.CmdRedial.TabIndex = 37;
			this.CmdRedial.Text = "重新拨号(vps)";
			this.CmdRedial.UseVisualStyleBackColor = true;
			this.CmdRedial.Click += new System.EventHandler(this.CmdRedial_Click);
			// 
			// serverTask_Enable
			// 
			this.serverTask_Enable.Text = "状态";
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
		private System.Windows.Forms.ListView LstServerQueue;
		private System.Windows.Forms.ColumnHeader serverTask_Index;
		private System.Windows.Forms.ColumnHeader serverTask_Name;
		private System.Windows.Forms.ColumnHeader serverTask_Handled;
		private System.Windows.Forms.ColumnHeader serverTask_Num;
		private System.Windows.Forms.TextBox IpSender;
		private System.Windows.Forms.Button CmdDisconnect;
		private System.Windows.Forms.Button CmdServerOn;
		private System.Windows.Forms.TextBox OpLog;
		private System.Windows.Forms.Label OpConnectionCount;
		private System.Windows.Forms.ListView LstConnection;
		private System.Windows.Forms.ColumnHeader clientName;
		private System.Windows.Forms.ColumnHeader Type;
		private System.Windows.Forms.ColumnHeader Ip;
		private System.Windows.Forms.ColumnHeader status;
		private System.Windows.Forms.ColumnHeader delay;
		private System.Windows.Forms.ColumnHeader Server;
		private System.Windows.Forms.TabPage TabMain_Setting;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox IpPerVPShdl;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox IpTaskTimeOut;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox IpTaskInterval;
		private System.Windows.Forms.Button CmdRedial;
		private System.Windows.Forms.ColumnHeader serverTask_Enable;
	}
}

