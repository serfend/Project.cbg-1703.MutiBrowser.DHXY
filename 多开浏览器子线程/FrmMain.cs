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
using System.Reflection;
using SfTcp.TcpClient;
using SfTcp.TcpMessage;

namespace 多开浏览器子线程
{
	
	partial class FrmMain : Form
	{
		private  void InitTcp()
		{
			Program.Tcp = new TcpBrowserClient();
			Program.Tcp.OnMessage += ReceiveMessage; ;
			Program.Tcp.OnDisconnected += (x,xx) => {
				Thread.Sleep(500);
				InitTcp();
			};
			Thread.Sleep(500);
			Program.Tcp?.Send(new RpClientConnectMessage("browser", Assembly.GetExecutingAssembly().GetName().Version.ToString(),Program.reg.In("Setting").GetInfo("deviceId", HttpUtil.UUID), CCmd.GetWebInfo("server")));
		}


		private bool frmClosing = false;
		Thread ThreadMonitor;
		private Inner.Ccmd CCmd = new Inner.Ccmd();
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
			this.FormClosing += (x, xx) => { frmClosing = true; RegUtil.SetFormPos(this); };
			ipWebShowUrl.KeyPress += IpWebShowUrl_KeyPress;
			ThreadMonitor = new Thread(() => {
				int heartBeatCount = 0;
				while (!frmClosing)
				{
					Thread.Sleep(1000);
					var cmd = CCmd.GetCmd(out string targetUrl);
					CheckNewCmd(cmd, targetUrl);
					heartBeatCount++;
					if (heartBeatCount > 10)
					{
						Program.Tcp?.Send(new MsgHeartBeatMessage());
					}
				}
			}) { IsBackground = true };

			this.Show();
			int[] tmpBounds = RegUtil.GetFormPos(this);
			this.Left = tmpBounds[0];
			this.Top = tmpBounds[1];
			this.Width = tmpBounds[2];
			this.Height = tmpBounds[3];

			InitTcp();

			ThreadMonitor.Start();

		}

		#region 逻辑
		private double price, assumePrice;
		private void ReceiveMessage(object sender, ClientMessageEventArgs e)
		{

			switch (e.Title)
			{
				case TcpMessageEnum.CmdCheckBillUrl:
					{

						//<newCheckBill><targetUrl>baidu.com</targetUrl><price>999</price><assumePrice>0</assumePrice></newCheckBill>
						var targetUrl = e.Message["Url"].ToString();
						price = Convert.ToDouble(e.Message["Price"]);
						assumePrice = Convert.ToDouble(e.Message["AssumePrice"]);
						switch (e.Message["Act"].ToString())
						{
							case "submit":
								{ 
									CheckNewCmd(CmdInfo.ShowWeb, targetUrl);
									break;
								}
							case "show":
								{
									if (assumePrice > price)
										CheckNewCmd(CmdInfo.SubmitBill, targetUrl);
									else CheckNewCmd(CmdInfo.ShowWeb, targetUrl);
									break;
								}
						}
						
						
						break;
					}
				case TcpMessageEnum.MsgSynSession:
					SynLoginSession();
					break;
			}
		}

		private void SynLoginSession()
		{
			Program.Tcp?.Send(new RpSessionSynMessage($"sid={GetNowLoginCookies()}"));
			Text = "已同步登录状态到服务器";
		}

		public enum CmdInfo
		{
			None,
			SubClose,
			ShowWeb,
			InitWeb,
			SubmitBill,
			OnlyRefresh
		}
		public void CheckNewCmd(CmdInfo cmdInfo,string targetUrl)
		{
				switch (cmdInfo)
				{
					case CmdInfo.None:
						break;
					case CmdInfo.SubClose:
						this.Invoke((EventHandler)delegate { this.Close(); });
						return;
					case CmdInfo.ShowWeb:
						{
							this.Invoke((EventHandler)delegate {
								this.Focus();
								this.TopMost = true;
								this.WindowState = FormWindowState.Normal;
								ReNavigateWeb(targetUrl);
							});

						}
						break;
					case CmdInfo.InitWeb:
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
					case CmdInfo.SubmitBill:
					{
						this.Invoke((EventHandler)delegate {
							this.Focus();
							this.TopMost = true;
							this.WindowState = FormWindowState.Normal;
							
							TrySubmitBill(targetUrl);//支持浏览器下单
						});
						break;
					}
					case CmdInfo.OnlyRefresh:
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
		
		private void TrySubmitBill(string url)
		{
			
			var http = new HttpClient();
			//使用webbrowser登录cookies进行获取订单？
			//未登录=》登录超时，请重新登录！
			//返回订单信息
			var cookiesLogin = $"sid={GetNowLoginCookies()}";
			Program.Tcp?.Send(new RpBillSubmitedMessage(RpBillSubmitedMessage.State.New));
			http.Item.Request.Cookies +=  cookiesLogin;
			http.GetHtml(url,callBack:(x)=> {
				var info =x.response.DataString(Encoding.Default);
				var frmInfo = HttpUtil.GetElement(info, "usertrade.py", "返回");
				var billPoster = new BillPoster(frmInfo, cookiesLogin);
				this.Invoke((EventHandler)delegate
				{
					billPoster.Submit((result, success) => {
						Program.reg.In("Bill").In("record").In(DateTime.Now.ToString("yyyyMMdd")).SetInfo(DateTime.Now.ToString("hhmmssffff"), result);
						if (success)
						{
							var t = new Task(() => {
								Program.Tcp?.Send(new RpBillSubmitedMessage(RpBillSubmitedMessage.State.Success));
								this.Invoke((EventHandler)delegate {
									Text = ("下单成功 " + url);
								});
							}
								);
							t.Start();
						}
						else
						{
							var t = new Task(() => {
								this.Invoke((EventHandler)delegate {
									Program.Tcp?.Send(new RpBillSubmitedMessage(RpBillSubmitedMessage.State.Fail, result));
									Text = (result + "\n" + url);
								});
							}
							);
							t.Start();
						};
						Console.WriteLine(result);
					}, this);
				});
			});
			ReNavigateWeb(url);



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
				
				bool canSubmit = price<=assumePrice && assumePrice>0;
				string bidInfo = $"{price}/{assumePrice}";
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
						Program.reg.In("Bill").In("record").In(DateTime.Now.ToString("yyyyMMdd")).SetInfo(DateTime.Now.ToString("hhmmssffff"), LbShowStatus.Text);
						Thread.Sleep(500);//TODO 此处提交订单后 延迟500ms
						Program.Tcp?.Send(new RpBillSubmitedMessage(RpBillSubmitedMessage.State.Success));
					}
					else if (canSubmit)
					{
						LbShowStatus.Text += ",自动提交";
						WebShow.Document.GetElementById("equip_info").InvokeMember("submit");
					}
					else
					{
						LbShowStatus.Text += ",报价不符,取消自动提交";
					}

				}
				assumePrice = 0;//重置估价
			}
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

		private void BtnSynLoginSession_Click(object sender, EventArgs e)
		{
			SynLoginSession();
		}

		private void BtnBillPosterTest_Click(object sender, EventArgs e)
		{
			this.Text = "测试下单接口";
			TrySubmitBill(ipWebShowUrl.Text);
		}

		private void BtnMinimun_Click(object sender, EventArgs e)
		{
			this.WindowState = FormWindowState.Minimized;
		}
	}
#endregion
}