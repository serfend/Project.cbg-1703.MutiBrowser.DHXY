using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using DotNet4.Utilities.UtilReg;
namespace 多开浏览器子线程
{
	partial class FrmMain : Form
	{
		private Inner.Ccmd CCmd=new Inner.Ccmd();
		System.Threading.Timer t;
		public FrmMain()
		{
			//RegUtil.SetIEBrowserTempPath();
			InitializeComponent();
			
			//RegUtil.SetIEBrowserTempPathEnd();
			WebShow.ScriptErrorsSuppressed = true;
			
			WebShow.DocumentCompleted += WebShow_DocumentCompleted;
			WebShow.NewWindow += WebShow_NewWindow;
			ipWebShowUrl.KeyPress += IpWebShowUrl_KeyPress;
			t=new System.Threading.Timer(new TimerCallback(CheckNewCmd),null,2000,200);
			this.Show();
			int[] tmpBounds=RegUtil.GetFormPos(this);
			this.Left = tmpBounds[0];
			this.Top = tmpBounds[1];
			this.Width = tmpBounds[2];
			this.Height = tmpBounds[3];
		}

		#region 逻辑
		public void CheckNewCmd(object t)
		{
				switch (CCmd.GetCmd(out string targetUrl))
				{
					case -1:
						break;
					case 404:
						this.Invoke((EventHandler)delegate { this.Close(); });
						return;
					case 233:
						{
							this.Invoke((EventHandler)delegate {
								this.Focus();
								this.TopMost = true;
								this.WindowState = FormWindowState.Normal;
								ReNavigateWeb(targetUrl);
							});

						}
						break;
					case 2333:
						{
							this.Invoke((EventHandler)delegate {
								this.Text = Program.thisExeThreadId + ":" + CCmd.GetWebInfo("name");
								if (targetUrl.Length == 0)
									targetUrl = "http://xy2.cbg.163.com/";
								else
									Console.WriteLine("读取网页成功:" + CCmd.GetWebInfo("name"));
								ReNavigateWeb(targetUrl);
							});

							break;
						}
					case 1:
						this.Invoke((EventHandler)delegate { WebShow.Refresh(); });
						Console.WriteLine("页面刷新");
						break;
				}
		}
		#endregion
		#region 事件
		private void WebShow_NewWindow(object sender, CancelEventArgs e)
		{
			
			try
			{
				string tmpUrl = WebShow.Document.ActiveElement.GetAttribute("href");
				if (tmpUrl.Length == 0) return;
				e.Cancel = true;
				ReNavigateWeb(tmpUrl);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
		private void WebShow_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			
			LbShowStatus.Text ="更新网页:" + e.Url.OriginalString ;
			if (e.Url.ToString() != WebShow.Url.ToString())
				return;
			ipWebShowUrl.Text = e.Url.ToString ();
			TryLoadBill(false);

			CCmd.Refresh();
		}
		private void TryLoadBill(bool IsUserAction = false)
		{
			var submitTarget = WebShow.Document.GetElementById("buy_btn");
			if (submitTarget != null)
			{

				bool canSubmit = IsPriceSuit(out string bidInfo);
				LbShowStatus.Text = "检测到进入预约界面(" + (canSubmit ? "报价符合" : "报价不符") + " " + bidInfo + ")";
				canSubmit |= IsUserAction;
				if (submitTarget.OuterHtml.Contains("登录"))
				{
					LbShowStatus.Text += ",未登录,取消自动提交";
				}
				else
				{
					LbShowStatus.Text += ",已登录";

					if (canSubmit)
					{
						LbShowStatus.Text += ",自动提交";
						WebShow.Document.GetElementById("equip_info").InvokeMember("submit");
					}
					else
					{
						LbShowStatus.Text += ",报价不符,取消自动提交";
					}

				}
			}
		}
		private bool IsPriceSuit(out string bidInfo)
		{
			bidInfo = Program.reg.In("Main").In("ThreadCmd").In(Program.thisExeThreadId).GetInfo("Price");
			string[] temp = bidInfo.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries);
			if (temp.Length < 1) return false;
			double.TryParse(temp[0], out double askBid);
			double.TryParse(temp[1], out double myBid);
			
			return (myBid >= askBid);
		}
		private void BtnRefresh_Click(object sender, EventArgs e)
		{
			ReNavigateWeb();
		}
		private void ReNavigateWeb(string url=null)
		{
			if (url == null) url = ipWebShowUrl.Text;
			else ipWebShowUrl.Text = url;
			if (!url.StartsWith ("http") ) ipWebShowUrl.Text = "http://" + url;
			WebShow.Navigate(ipWebShowUrl.Text );
		}
					
		private void IpWebShowUrl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == 13) ReNavigateWeb();
		}

		private void FrmMain_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				this.menuMain.Show (MousePosition);
			}
		}

		private void IE9ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			BrowserVersionEdit.SetIEcomp(WebShow);
		}

		private void 当前版本ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("当前版本:"+WebShow.Version);
		}

		private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			RegUtil.SetFormPos(this);
			RegUtil.SetIEBrowserTempPathEnd();
			if (e.CloseReason == CloseReason.WindowsShutDown)
			{
				//此处应处理立即保存快照
			}
		}

		private void WebShow_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			Console.WriteLine("导航:"+ e.TargetFrameName + " Url:" + e.Url);
		}

		private void BtnSetBootPage_Click(object sender, EventArgs e)
		{
			Program.reg.In("Main").In("setting").In("cmd").SetInfo(Program.thisExeThreadId + ".defaultUrl", ipWebShowUrl.Text);
			MessageBox.Show("已将默认启动页面设置到" + ipWebShowUrl.Text);
		}

		private void BtnShowBuyList_Click(object sender, EventArgs e)
		{
			TryLoadBill(true);
		}
	}
#endregion
}
