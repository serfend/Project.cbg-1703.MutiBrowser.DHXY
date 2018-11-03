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
using System.IO;
using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilHttp;
using 多开浏览器子线程.util;

namespace 多开浏览器子线程
{
	partial class FrmMain : Form
	{
		private Inner.Ccmd CCmd=new Inner.Ccmd();
		public FrmMain()
		{
			//RegUtil.SetIEBrowserTempPath();
			InitializeComponent();
			//HttpClient.UsedFidder = true;
			//RegUtil.SetIEBrowserTempPathEnd();
			WebShow.ScriptErrorsSuppressed = true;
			
			WebShow.DocumentCompleted += WebShow_DocumentCompleted;
			WebShow.NewWindow += WebShow_NewWindow;
			BtnShowBuyList.Click += (x, xx) => {
				TryLoadBill(true);
			};
			ipWebShowUrl.KeyPress += IpWebShowUrl_KeyPress;
			var t = new Thread(()=> {
				while (true)
				{
					Thread.Sleep(200);
					CheckNewCmd();
				}
			}) { IsBackground=true};
			
			this.Show();
			int[] tmpBounds=RegUtil.GetFormPos(this);
			this.Left = tmpBounds[0];
			this.Top = tmpBounds[1];
			this.Width = tmpBounds[2];
			this.Height = tmpBounds[3];
			if (Tcp==null||Tcp.client==null||!Tcp.client.Connected)
			{
				this.Close();
			}
			t.Start();
		}

		#region 逻辑
		public void CheckNewCmd()
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
					case 101:
					{
						this.Invoke((EventHandler)delegate {
							this.Focus();
							this.TopMost = true;
							this.WindowState = FormWindowState.Normal;
							this.WebShow.Visible = false;
							TrySubmitBill(targetUrl);
							
						});
						break;
					}
					case 1:
						this.Invoke((EventHandler)delegate {
							this.Text = Program.thisExeThreadId + ":" + CCmd.GetWebInfo("name") + "--刷新";
							WebShow.Refresh();
						});
						
						break;
				}
		}
		#endregion
		#region 事件
		private string GetNowLoginCookies()
		{
			var init = CookieReader.GetCookie(WebShow.Url.AbsoluteUri);
			var cookies = init.Split(';');
			foreach(var cookie in cookies)
			{
				var pairs = cookie.Split('=');
				if (pairs[0].Trim(' ') == "sid")
				{
					Console.WriteLine(pairs[1]);
					return pairs[1];
				}
			}
			return null;
		}
		private SfTcp.SfTcpClient Tcp=new SfTcp.SfTcpClient(true);
		private void TrySubmitBill(string url)
		{
			
			var http = new HttpClient();
			//TODO 使用webbrowser登录cookies进行获取订单？
			//未登录=》登录超时，请重新登录！
			//返回订单信息
			var cookiesLogin = "sid=" + GetNowLoginCookies();
			Tcp.Send("<client.command><stamp>" + HttpUtil.TimeStamp + "</stamp><buildBill></buildBill></client.command>");
			http.Item.Request.Cookies +=  cookiesLogin;
			http.GetHtml(url,callBack:(x)=> {
				var info =x.response.DataString(Encoding.Default);
				var frmInfo = HttpUtil.GetElement(info, "usertrade.py", "返回");
				var billPoster = new BillPoster(frmInfo, cookiesLogin);
				this.Invoke((EventHandler)delegate
				{
					billPoster.Submit((result, success) => {
						Program.reg.In("Bill").In("record").SetInfo(DateTime.Now.ToString("yyyyMMddhhmmss"), result);
						if (success)
						{
							this.BeginInvoke((EventHandler)delegate {
								ReNavigateWeb(url);
							});
							var t = new Task(() => {
								Tcp.Send("<client.command><stamp>" + HttpUtil.TimeStamp + "</stamp><newBill></newBill></client.command>");
								this.Invoke((EventHandler)delegate {
									Text = ("下单成功\n" + url);
									this.WebShow.Visible = true;
								});
							}
								);
							t.Start();
						}
						else
						{
							var t = new Task(() => {
								this.Invoke((EventHandler)delegate {
									Tcp.Send("<client.command><stamp>" + HttpUtil.TimeStamp + "</stamp><failBill></failBill>"+ result+"</client.command>");
									Text = (result + "\n" + url);
									this.WebShow.Visible = true;
								});
							}
							);
							t.Start();
						};
						Console.WriteLine(result);
					}, this);
				});
			});




		}
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
			
			//LbShowStatus.Text ="更新网页:" + e.Url.OriginalString ;
			if (e.Url.ToString() != WebShow.Url.ToString())
				return;
			ipWebShowUrl.Text = e.Url.ToString ();
			CCmd.Refresh();
			TryLoadBill(false);

			
		}
		private void TryLoadBill(bool IsUserAction = false)
		{
			var submitTarget = WebShow.Document.GetElementById("buy_btn");
			if (submitTarget != null)
			{
				bool canSubmit = IsPriceSuit(out string bidInfo);
				ISPrice.Text = bidInfo;
				LbShowStatus.Text = "检测到进入预约界面(" + (canSubmit ? "报价符合" : "报价不符") + " " + bidInfo + ")";
				WebShow.Document.InvokeScript("tab_onclick", new object[] { 3 });
				canSubmit |= IsUserAction;
				if (submitTarget.OuterHtml.Contains("登录"))
				{
					LbShowStatus.Text += ",未登录,取消自动提交";
					//this.WindowState = FormWindowState.Minimized;
				}
				else
				{
					LbShowStatus.Text += ",已登录";
					var stream = WebShow.DocumentStream;
					var reader = new StreamReader(stream, Encoding.Default);
					var BuyUserRedictInfo = reader.ReadToEnd();
					BuyUserRedictInfo = HttpUtil.GetElement(BuyUserRedictInfo, "<dt>指定", "</dl>");
					BuyUserRedictInfo = HttpUtil.GetElement(BuyUserRedictInfo, "<dd>", "</dd>");
					if (BuyUserRedictInfo!=null)
					{
						int len = 0;
						foreach (var c in BuyUserRedictInfo)
						{
							if (c != ' ') len++;
						}
						if (len > 0)
						{
							//MessageBox.Show(BuyUserRedictInfo);
							LbShowStatus.Text += "检测到有指定,隐藏";
							this.WindowState = FormWindowState.Minimized;
							return;
						}

					}
					if (IsUserAction)
					{
						LbShowStatus.Text += ",用户主动提交";
						WebShow.Document.GetElementById("equip_info").InvokeMember("submit");
						Tcp.Send("<client.command><stamp>" + HttpUtil.TimeStamp + "</stamp><newBill></newBill></client.command>");
					}
					else if (canSubmit)
					{
						LbShowStatus.Text += ",自动提交";
						//WebShow.Document.GetElementById("equip_info").InvokeMember("submit");
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
			var priceInfo = Program.reg.In("Main").In("ThreadCmd").In(Program.thisExeThreadId);
			bidInfo = priceInfo.GetInfo("Price");
			priceInfo.SetInfo("Price","none");
			string[] temp = bidInfo.Split(new string[] { "," },StringSplitOptions.RemoveEmptyEntries);
			if (temp.Length < 2 || bidInfo=="none") return false;
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
		private void FrmMain_Resize(object sender, EventArgs e)
		{
			GPctlInfo.Top = (int)(this.Height * 0.55);
			GPctlInfo.Left = 0;
		}

		private void BtnMinimun_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}
	}
#endregion
}