namespace 多开浏览器子线程
{
	partial class FrmMain
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
				Program.Tcp?.Dispose();
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
			this.components = new System.ComponentModel.Container();
			this.WebShow = new System.Windows.Forms.WebBrowser();
			this.ipWebShowUrl = new System.Windows.Forms.TextBox();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.menuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.iE设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.设置IE版本ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.iE9ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.当前版本ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.BtnSetBootPage = new System.Windows.Forms.Button();
			this.LbShowStatus = new System.Windows.Forms.Label();
			this.GPctlInfo = new System.Windows.Forms.GroupBox();
			this.ISPrice = new System.Windows.Forms.Label();
			this.BtnMinimun = new System.Windows.Forms.Button();
			this.BtnShowBuyList = new System.Windows.Forms.Button();
			this.BtnSynLoginSession = new System.Windows.Forms.Button();
			this.menuMain.SuspendLayout();
			this.GPctlInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// WebShow
			// 
			this.WebShow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.WebShow.Location = new System.Drawing.Point(2, 61);
			this.WebShow.Margin = new System.Windows.Forms.Padding(2);
			this.WebShow.MinimumSize = new System.Drawing.Size(15, 16);
			this.WebShow.Name = "WebShow";
			this.WebShow.Size = new System.Drawing.Size(896, 555);
			this.WebShow.TabIndex = 0;
			this.WebShow.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebShow_Navigating);
			// 
			// ipWebShowUrl
			// 
			this.ipWebShowUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.ipWebShowUrl.Location = new System.Drawing.Point(2, 35);
			this.ipWebShowUrl.Margin = new System.Windows.Forms.Padding(2);
			this.ipWebShowUrl.Name = "ipWebShowUrl";
			this.ipWebShowUrl.Size = new System.Drawing.Size(775, 21);
			this.ipWebShowUrl.TabIndex = 1;
			// 
			// btnRefresh
			// 
			this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRefresh.Location = new System.Drawing.Point(781, 35);
			this.btnRefresh.Margin = new System.Windows.Forms.Padding(2);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(46, 22);
			this.btnRefresh.TabIndex = 2;
			this.btnRefresh.Text = "刷新";
			this.btnRefresh.UseVisualStyleBackColor = true;
			this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
			// 
			// menuMain
			// 
			this.menuMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iE设置ToolStripMenuItem});
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(112, 26);
			// 
			// iE设置ToolStripMenuItem
			// 
			this.iE设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.设置IE版本ToolStripMenuItem,
            this.当前版本ToolStripMenuItem});
			this.iE设置ToolStripMenuItem.Name = "iE设置ToolStripMenuItem";
			this.iE设置ToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
			this.iE设置ToolStripMenuItem.Text = "IE设置";
			// 
			// 设置IE版本ToolStripMenuItem
			// 
			this.设置IE版本ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iE9ToolStripMenuItem});
			this.设置IE版本ToolStripMenuItem.Name = "设置IE版本ToolStripMenuItem";
			this.设置IE版本ToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.设置IE版本ToolStripMenuItem.Text = "设置IE版本";
			// 
			// iE9ToolStripMenuItem
			// 
			this.iE9ToolStripMenuItem.Name = "iE9ToolStripMenuItem";
			this.iE9ToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
			this.iE9ToolStripMenuItem.Text = "IE9";
			this.iE9ToolStripMenuItem.Click += new System.EventHandler(this.IE9ToolStripMenuItem_Click);
			// 
			// 当前版本ToolStripMenuItem
			// 
			this.当前版本ToolStripMenuItem.Name = "当前版本ToolStripMenuItem";
			this.当前版本ToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
			this.当前版本ToolStripMenuItem.Text = "当前版本";
			this.当前版本ToolStripMenuItem.Click += new System.EventHandler(this.当前版本ToolStripMenuItem_Click);
			// 
			// BtnSetBootPage
			// 
			this.BtnSetBootPage.Location = new System.Drawing.Point(2, 9);
			this.BtnSetBootPage.Margin = new System.Windows.Forms.Padding(2);
			this.BtnSetBootPage.Name = "BtnSetBootPage";
			this.BtnSetBootPage.Size = new System.Drawing.Size(130, 22);
			this.BtnSetBootPage.TabIndex = 5;
			this.BtnSetBootPage.Text = "将此网页设置为启动页";
			this.BtnSetBootPage.UseVisualStyleBackColor = true;
			this.BtnSetBootPage.Click += new System.EventHandler(this.BtnSetBootPage_Click);
			// 
			// LbShowStatus
			// 
			this.LbShowStatus.AutoSize = true;
			this.LbShowStatus.Location = new System.Drawing.Point(137, 9);
			this.LbShowStatus.Name = "LbShowStatus";
			this.LbShowStatus.Size = new System.Drawing.Size(0, 12);
			this.LbShowStatus.TabIndex = 6;
			// 
			// GPctlInfo
			// 
			this.GPctlInfo.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.GPctlInfo.Controls.Add(this.ISPrice);
			this.GPctlInfo.Controls.Add(this.BtnMinimun);
			this.GPctlInfo.Controls.Add(this.BtnShowBuyList);
			this.GPctlInfo.Location = new System.Drawing.Point(12, 313);
			this.GPctlInfo.Name = "GPctlInfo";
			this.GPctlInfo.Size = new System.Drawing.Size(248, 84);
			this.GPctlInfo.TabIndex = 7;
			this.GPctlInfo.TabStop = false;
			// 
			// ISPrice
			// 
			this.ISPrice.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.ISPrice.Location = new System.Drawing.Point(8, 17);
			this.ISPrice.Name = "ISPrice";
			this.ISPrice.Size = new System.Drawing.Size(234, 21);
			this.ISPrice.TabIndex = 7;
			this.ISPrice.Text = "12500/10000";
			this.ISPrice.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// BtnMinimun
			// 
			this.BtnMinimun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnMinimun.Location = new System.Drawing.Point(12, 40);
			this.BtnMinimun.Margin = new System.Windows.Forms.Padding(2);
			this.BtnMinimun.Name = "BtnMinimun";
			this.BtnMinimun.Size = new System.Drawing.Size(113, 36);
			this.BtnMinimun.TabIndex = 6;
			this.BtnMinimun.Text = "最小化";
			this.BtnMinimun.UseVisualStyleBackColor = true;
			this.BtnMinimun.Click += new System.EventHandler(this.BtnMinimun_Click);
			// 
			// BtnShowBuyList
			// 
			this.BtnShowBuyList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnShowBuyList.Location = new System.Drawing.Point(127, 40);
			this.BtnShowBuyList.Margin = new System.Windows.Forms.Padding(2);
			this.BtnShowBuyList.Name = "BtnShowBuyList";
			this.BtnShowBuyList.Size = new System.Drawing.Size(116, 36);
			this.BtnShowBuyList.TabIndex = 5;
			this.BtnShowBuyList.Text = "立即下单";
			this.BtnShowBuyList.UseVisualStyleBackColor = true;
			// 
			// BtnSynLoginSession
			// 
			this.BtnSynLoginSession.Location = new System.Drawing.Point(142, 9);
			this.BtnSynLoginSession.Margin = new System.Windows.Forms.Padding(2);
			this.BtnSynLoginSession.Name = "BtnSynLoginSession";
			this.BtnSynLoginSession.Size = new System.Drawing.Size(112, 22);
			this.BtnSynLoginSession.TabIndex = 8;
			this.BtnSynLoginSession.Text = "同步登录信息";
			this.BtnSynLoginSession.UseVisualStyleBackColor = true;
			this.BtnSynLoginSession.Click += new System.EventHandler(this.BtnSynLoginSession_Click);
			// 
			// FrmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(900, 627);
			this.Controls.Add(this.BtnSynLoginSession);
			this.Controls.Add(this.GPctlInfo);
			this.Controls.Add(this.LbShowStatus);
			this.Controls.Add(this.BtnSetBootPage);
			this.Controls.Add(this.btnRefresh);
			this.Controls.Add(this.ipWebShowUrl);
			this.Controls.Add(this.WebShow);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "FrmMain";
			this.Text = "frmMain";
			this.Load += new System.EventHandler(this.BtnRefresh_Click);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.FrmMain_MouseClick);
			this.Resize += new System.EventHandler(this.FrmMain_Resize);
			this.menuMain.ResumeLayout(false);
			this.GPctlInfo.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}



		#endregion

		private System.Windows.Forms.WebBrowser WebShow;
		private System.Windows.Forms.TextBox ipWebShowUrl;
		private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.ContextMenuStrip menuMain;
		private System.Windows.Forms.ToolStripMenuItem iE设置ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 设置IE版本ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem iE9ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 当前版本ToolStripMenuItem;
		private System.Windows.Forms.Button BtnSetBootPage;
		private System.Windows.Forms.Label LbShowStatus;
		private System.Windows.Forms.GroupBox GPctlInfo;
		private System.Windows.Forms.Label ISPrice;
		private System.Windows.Forms.Button BtnMinimun;
		private System.Windows.Forms.Button BtnShowBuyList;
		private System.Windows.Forms.Button BtnSynLoginSession;
	}
}

