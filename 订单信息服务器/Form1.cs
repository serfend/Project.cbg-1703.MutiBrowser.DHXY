using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilReg;
using File_Transfer;
using SfTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace 订单信息服务器
{
	public partial class Form1 : Form
	{
		private TcpServerManager serverManager;
		private TransferFileEngine transferFileEngine;
		private struct FileWaitingClient
		{
			SfTcp.TcpServer client;
			string fileRequire;
			public TcpServer Client { get => client; set => client = value; }
			public string FileRequire { get => fileRequire; set => fileRequire = value; }
		}
		private Queue<FileWaitingClient> fileSynQueue=new Queue<FileWaitingClient>();
		private void HdlVpsFileSynRequest(string x, SfTcp.TcpServer s)
		{
			if (transferFileEngine.Sender.SendingFileQueue.Count>0 && !transferFileEngine.Sender.IsSending) {
				fileSynQueue.Enqueue(new FileWaitingClient() {FileRequire=x,Client=s });
				return;
			}
			var t = new Task(() => {
				
				waittingFileInfo = x;//记录命令的详细信息

				this.Invoke((EventHandler)delegate { AppendLog("准备向终端" + s.clientName + " 传输文件"); });
				InitTransferEngine();
				Thread.Sleep(5000);
				s.Send("<ensureFileTransfer>" + new Random().Next(0, 1000));
				transferFileEngine.Connect();//文件发送引擎

			});
			t.Start();
		}
		private void InitTransferEngine()
		{
			if (transferFileEngine != null)
			{
				transferFileEngine.Dispose();
				transferFileEngine = null;
			}
			transferFileEngine = new TransferFileEngine(TcpFiletransfer.TcpTransferEngine.Connections.Connection.EngineModel.AsServer, "any", 8010);
			transferFileEngine.Connection.ConnectToClient += (x, xx) =>
			{
				if (xx.Result == File_Transfer.Model.ReceiverFiles.ReceiveResult.RequestAccepted)
				{
					var fileRequire = HttpUtil.GetAllElements(waittingFileInfo, "<fileRequest>", "</fileRequest>");
					this.Invoke((EventHandler)delegate { AppendLog("文件已提交至发送队列:" + fileRequire.Count); });
					foreach (var f in fileRequire)
					{
						transferFileEngine.SendingFile("同步设置/" + f);
					}

				}
				else { this.Invoke((EventHandler)delegate { AppendLog("文件服务器连接终端失败:" + xx.Info); }); }
			};
			transferFileEngine.Sender.SendingFileStartedEvent += (x, xx) =>
			{
				this.Invoke((EventHandler)delegate { AppendLog("开始传输文件:" + xx.FileName); });
			};
			transferFileEngine.Sender.SendingCompletedEvent += (x, xx) =>
			{
				this.Invoke((EventHandler)delegate
				{
					//AppendLog("文件传输结束:" + xx.Title + ":" + xx.Message);
					if (x.SendingFileQueue.Count == 0)
					{
						AppendLog("终端文件传输结束");
						if (fileSynQueue.Count > 0)
						{
							var fsq = fileSynQueue.Dequeue();
							HdlVpsFileSynRequest(fsq.FileRequire, fsq.Client);
						}
					}
				});
			};
		}
		private void InitServerManager()
		{
			serverManager = new TcpServerManager()
			{
				NormalMessage = (s, x, InnerInfo) => {
					this.Invoke((EventHandler)delegate
					{
						var targetItem = GetItem(s.Ip);
						if (targetItem != null)
						{
							if (x.Contains("heartBeat"))
							{
							} else if (x.Contains("RHB"))
							{
								targetItem.SubItems[4].Text = InnerInfo;
							}
							else if (x.Contains("clientConnect"))
							{
								if (InnerInfo.Contains("<browserInit>"))
								{
									var hdlServerName = HttpUtil.GetElementInItem(InnerInfo, "browserInit");
									targetItem.SubItems[3].Text = "等待订单";
									targetItem.SubItems[0].Text = hdlServerName;
									s.clientName = hdlServerName;
									BrowserIp[hdlServerName] = s.Ip;
								}
								else
								{
									targetItem.SubItems[3].Text = "初始化";
									targetItem.SubItems[0].Text = HttpUtil.GetElementInItem(InnerInfo, "connectCmdRequire");
									s.ID = HttpUtil.GetElementInItem(InnerInfo, "clientDeviceId");
									var clientName = regSettingVps.In(s.ID).GetInfo("clientName", targetItem.SubItems[0].Text);
									s.Send(string.Format("<setClientName>{0}</setClientName>", clientName));//用于确认当前名称并初始化
								}
							}
							else if (x.Contains("nameModefied"))
							{
								targetItem.SubItems[0].Text = HttpUtil.GetElementInItem(InnerInfo,"clientName");
								bool flag = (s.clientName == "..." && InnerInfo.Contains("<AskForSynInit>"));//首次初始化时尝试发送vps终端初始化
								
								s.clientName = targetItem.SubItems[0].Text;
								if(flag)
								{
									BuildNewTaskToVps(s, out string taskTitle);
									targetItem.SubItems[5].Text = taskTitle;
									SynLstTask();
								}
							}
							else if (x.Contains("InitComplete"))
							{
								//识别vps终端
								s.IsLocal = true;
								targetItem.SubItems[1].Text = "vps";

								//终端已初始化完成
								//synSetting,synFile
								//遍历 【同步文件】 下所有文件
								var dic = new DirectoryInfo(Application.StartupPath + "\\同步设置");
								var tmp = new StringBuilder();
								foreach (var f in dic.EnumerateFiles())
								{
									tmp.Append("<file><name>").Append(f.Name).Append("</name>").Append("<version>").Append(HttpUtil.GetMD5ByMD5CryptoService(f.FullName)).Append("</version></file>");
								}
								if (tmp.Length > 0)
								{
									tmp.Append("<versionCheck>");
									s.Send(tmp.ToString());
								}
								else
								{
									s.Send("<serverRun>");//无需同步
								}

							}
							else if (x.Contains("RequireFile"))//服务器接收到来自客户端请求文件的命令
							{
								this.Invoke((EventHandler)delegate { AppendLog("vps" + s.clientName + "请求获取文件"); });
								HdlVpsFileSynRequest(InnerInfo, s);
							}
							else if (x.Contains("Status"))
							{
								targetItem.SubItems[3].Text = InnerInfo;
								if(InnerInfo.Contains(" 失败"))
								{
									s.Send("<reRasdial>");
								}
							}
							else if (x.Contains("newCheckBill"))
							{
								HdlNewCheckBill(s.clientName, InnerInfo);
							}
							else if (x.Contains("reRasdial"))
							{
								targetItem.SubItems[3].Text = "VPS重拨号中";
								var tcp = serverManager[targetItem.SubItems[2].Text];
								tcp.Disconnect();
							}
							else
							{
								AppendLog("新消息[" + s.clientName + "] " + x + ":" + InnerInfo);
								targetItem.SubItems[3].Text = InnerInfo;
							}
						}

					});
				},
				ServerConnected = (x) => {
					this.Invoke((EventHandler)delegate {
						//AppendLog("已连接:" + x.Ip);
						var info = new string[6];
						info[1] = x.IsLocal ? "主机" : "终端";
						info[2] = x.Ip;
						info[0] = x.clientName;
						info[3] = "新建状态";
						info[4] = "未开始采集";//延迟
						info[5] = "暂无";
						LstConnection.Items.Add(new ListViewItem(info));

						var welcome = new Task(() => {
							Thread.Sleep(3000);
							x.Send("<welcome>" + DateTime.Now + "</welcome>");
						});
						welcome.Start();
					});
				},
				ServerDisconnected = (x) => {
					this.Invoke((EventHandler)delegate {
						//AppendLog("已断开:" + x.Ip);
						for (int i = 0; i < LstConnection.Items.Count; i++)
							if (LstConnection.Items[i].SubItems[2].Text == x.Ip)
								LstConnection.Items.RemoveAt(i);
						if (allocServer.ContainsKey(x.Ip))
						{
							var vps = allocServer[x.Ip];
							foreach(var server in vps.hdlServer)
							{
								serverInfoList[server].NowNum++;
							}
							allocServer.Remove(x.Ip);
							SynLstTask();
						}
					});
				},
				HttpRequest = (x, s) => {
					var cst = new StringBuilder();
					cst.AppendLine($"<h1>Hey,测试服务器已开启</h1><br><p>当前连接数:{LstConnection.Items.Count }</p>");
					cst.AppendLine($"<p>request: {x.Param}</p>");
					var checkIfHaveValue = x.Param.IndexOf(':') ;
					string requestPage, requestParam;
					if (checkIfHaveValue > 0)
					{
						requestPage = x.Param.Substring(0, checkIfHaveValue);
						requestParam = x.Param.Substring(checkIfHaveValue + 1);
					}
					else
					{
						requestPage=x.Param;
						requestParam = string.Empty;
					}
					switch (requestPage)
					{
						case "Status": {
								this.Invoke((EventHandler)delegate {
									var clientNum = this.LstConnection.Items.Count;
									var columnsNum = LstConnection.Columns.Count;
									cst.AppendLine($"<p>当前状态共有{clientNum}个连接</p><br>");
									cst.AppendLine($"打开网页次数: 手动:{ManagerHttpBase.UserWebShowTime}  自动:{ManagerHttpBase.FitWebShowTime}<br>");
									cst.AppendLine($"profit:{ManagerHttpBase.RecordMoneyGet}  times:{ManagerHttpBase.RecordMoneyGetTime}");
									cst.AppendLine("<table border=\"1\">");
									cst.AppendLine("<tr>");
									for (int i = 0; i < columnsNum; i++)
										cst.Append($"<th>{LstConnection.Columns[i].Text}</th>");

									cst.AppendLine("</tr>");
									for (int i = 0; i < clientNum; i++)
									{
										cst.AppendLine("<tr>");
										for (int j = 0; j < columnsNum; j++) cst.Append($"<td>{LstConnection.Items[i].SubItems[j].Text}</td>");
										cst.AppendLine("</tr>");
									}
									cst.Append("</table>");
									
								});
								break;
							}
						case "targetUrl":{
								var nowTargetUrl = ManagerHttpBase.TargetUrl;
								if (checkIfHaveValue > 0)
								{
									ManagerHttpBase.TargetUrl = requestParam;
								}
								cst.AppendLine($"targetPrevious: {nowTargetUrl}");
								cst.AppendLine($"targetNew: {ManagerHttpBase.TargetUrl}");
								break;
							}
					}
					s.Response(cst.ToString() );
				}
			};
		}

		private Reg regPreviousGood;
		/// <summary>
		/// 将新的物品添加到商品列表
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="InnerInfo"></param>
		private void HdlNewCheckBill(string sender, string InnerInfo)
		{
			var tmp = InnerInfo.Split(new string[] { "##" }, StringSplitOptions.None);
			if (tmp.Length < 9)
			{
				AppendLog(sender + " 无效的订单信息:" + InnerInfo);
				return;
			}
			var serverName = tmp[0];
			var goodName = tmp[1];
			var priceInfo = tmp[2];
			var Rank = tmp[3];
			var ITalent = tmp[4];
			var IAchievement = tmp[5];
			var IChengjiu = tmp[6];
			var ISingleEnergyRate = tmp[7];
			var BuyUrl = tmp[8];

			var ordersn = HttpUtil.GetElement(BuyUrl, "ordersn=", "&");
			var previousRecord = regPreviousGood.GetInfo(ordersn);
			if (previousRecord != "" && ordersn!="")
			{
				AppendLog(previousRecord+"已出现过此订单,"+ serverName);
				return;
			}
			regPreviousGood.SetInfo(ordersn, DateTime.Now.ToString());
			LstGoodShow.Items.Insert(0,new ListViewItem(tmp));
			if (LstGoodShow.Items.Count > 10) LstGoodShow.Items[10].Remove();
			ManagerHttpBase.FitWebShowTime++;
			var price = priceInfo.Split('/');
			double priceNum = 0, priceNumAssume = 0;
			if (price.Length == 2)
			{
				priceNum = Convert.ToDouble(price[0]);
				priceNumAssume = Convert.ToDouble(price[1]);
				if (priceNum < priceNumAssume)
				{
					var earnNum = priceNumAssume - priceNum;
					ManagerHttpBase.RecordMoneyGet += earnNum;
					ManagerHttpBase.RecordMoneyGetTime++;
				}
			}
			SendCmdToBrowserClient(serverName, $"<newCheckBill><targetUrl>{BuyUrl}</targetUrl><price>{priceNum}</price><assumePrice>{priceNumAssume }</assumePrice>");
		}
		/// <summary>
		/// 将订单信息发送至下单服务器
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="cmdInfo"></param>
		private void SendCmdToBrowserClient(string serverName, string cmdInfo)
		{
			if (!BrowserIp.ContainsKey(serverName))
			{
				AppendLog(serverName + " 对应的下单浏览器进程未启动");
				return;
			}
			else
			{
				var targetBrowser = BrowserIp[serverName];
				var client = serverManager[targetBrowser];
				client.Send(cmdInfo);
			}
		}
		private Dictionary<string, string> BrowserIp=new Dictionary<string, string>();//浏览器进程对应终端ip
		private void SynLstTask()
		{
			for(int i = 0; i < LstServerQueue.Items.Count; i++)
			{
				var item = LstServerQueue.Items[i];
				var id = item.SubItems[0].Text;
				if (serverInfoList.ContainsKey(id))
				{
					item.SubItems[2].Text = (serverInfoList[id].HdlNum-serverInfoList[id].NowNum).ToString();
				}
			}
		}

		/// <summary>
		/// 将从任务列表中提取可用任务分配给VPS，VPS断开时撤回
		/// </summary>
		/// <param name="s"></param>
		private void BuildNewTaskToVps(TcpServer s,out string taskTitle)
		{
			int singleHdl = 1;
			try
			{
				singleHdl = Convert.ToInt32(IpPerVPShdl.Text);
				if (singleHdl == 0)
				{
					IpPerVPShdl.Text = "1";
					singleHdl = 1;
				}
			}
			catch (Exception)
			{
				IpPerVPShdl.Text = singleHdl.ToString();
			}
			string hdlServer = GetFreeServer(singleHdl, s.Ip,out taskTitle);
			int interval = 1500, timeout = 100000;
			
			try
			{
				timeout = Convert.ToInt32(IpTaskTimeOut.Text);
			}
			catch (Exception)
			{
				IpTaskTimeOut.Text = timeout.ToString();
			}
			try
			{
				interval = Convert.ToInt32(IpTaskInterval.Text);
			}
			catch (Exception)
			{
				IpTaskInterval.Text = interval.ToString();
			}
			s.Send(string.Format("<SynInit><interval>{0}</interval><task>{1}</task><timeout>{2}</timeout></SynInit><InnerTargetUrl>{3}</InnerTargetUrl>", interval, hdlServer, timeout, ManagerHttpBase.TargetUrl));
		}
		private class HdlServerInfo
		{
			string id;
			string name;
			string aeroId;
			string aeroName;
			int hdlNum;
			int nowNum;
			bool enable;
			public HdlServerInfo(string id, string name, string aeroId, string aeroName, int hdlNum) 
			{
				this.Id = id;
				this.Name = name;
				this.AeroId = aeroId;
				this.AeroName = aeroName;
				this.HdlNum=this.NowNum = hdlNum;
			}

			public string Id { get => id; set => id = value; }
			public string Name { get => name; set => name = value; }
			public string AeroId { get => aeroId; set => aeroId = value; }
			public string AeroName { get => aeroName; set => aeroName = value; }
			public int HdlNum { get => hdlNum; set => hdlNum = value; }
			public int NowNum { get => nowNum; set => nowNum = value; }
			public bool Enable { get => enable; set => enable = value; }
		}
		private class VPS
		{
			string name;
			string ip;
			public List<string> hdlServer;

			public VPS(string name, string ip) 
			{
				this.Name = name;
				this.Ip = ip;
				hdlServer = new List<string>();
			}

			public string Name { get => name; set => name = value; }
			public string Ip { get => ip; set => ip = value; }
		}
		private Reg regServerInfo;
		private Dictionary<string, HdlServerInfo> serverInfoList=new Dictionary<string, HdlServerInfo>();
		private Dictionary<string, VPS> allocServer=new Dictionary<string, VPS>();//以ip对应终端
		private void InitServerTaskList()
		{
			var serverInfo = File.ReadAllLines("ServerInfo.txt",Encoding.Default);
			foreach(var server in serverInfo)
			{
				var info = server.Split(',');
				var data = new string[5];
				if (info.Length < 4) {
					AppendLog("【警告】无效的区信息:" + server);
					continue;
				}
				var hdlNum = info.Length == 5 ? Convert.ToInt32(info[4]) : 1;
				var s = new HdlServerInfo(info[0],info[1],info[2],info[3],hdlNum);
				if (serverInfoList.ContainsKey(s.Id))
				{
					AppendLog("【警告】重复的区:" + server);
					continue;
				}
				serverInfoList.Add(s.Id, s);
				data[0] = s.Id;//区号
				data[1] = s.Name;//名称
				data[2] = "0";//已分配
				data[3] = s.HdlNum.ToString();//需分配
				data[4] = regServerInfo.GetInfo(s.Id,"启用");
				s.Enable = (data[4] == "启用");
				LstServerQueue.Items.Add(new ListViewItem(data));
			}
		}
		private string waittingFileInfo;
		private ListViewItem GetItem(string ip)
		{
			foreach(var item in LstConnection.Items) {
				if (item is ListViewItem it)
				{
					if (it.SubItems[2].Text== ip) return it;
				}
			}
			return null;
		}
		
		private Reg regSetting;
		private Reg regSettingVps;
		public Form1()
		{
			regSetting = new Reg("sfMinerDigger").In("Setting");
			regSettingVps = regSetting.In("vps");
			regServerInfo = regSetting.In("ServerInfo");
			regPreviousGood = regSetting.In("Goods").In("history");
			InitializeComponent();
			InitHistorySettingOnFormctl();
			InitTransferEngine();
			InitServerTaskList();
			InitServerManager();
			LstConnection.AfterLabelEdit += CheckIfUserEditName;
		}

		private bool ctlSaveLoaded = false;
		private void InitHistorySettingOnFormctl()
		{
			var frmSetting = regSetting.In("Form").In("ServerFormMain");
			foreach (var ctl in this.TabMain_Setting.Controls)
			{
				if(ctl is TextBox t)
				{
					t.Text = frmSetting.GetInfo(t.Name);
					if (!ctlSaveLoaded)
					{
						t.TextChanged += (x, xx) => {
							frmSetting.SetInfo(t.Name, t.Text);
						};
					}
				}
			}
			ctlSaveLoaded = true;

		}

		/// <summary>
		/// 无需要的任务时则返回"Idle"，终端进入休眠模式，30秒后再次询问
		/// </summary>
		/// <param name="singleHdl"></param>
		/// <param name="ip"></param>
		/// <param name="taskTitle">输出本次所有任务的标题</param>
		/// <returns></returns>
		private string GetFreeServer(int singleHdl,string ip,out string taskTitle)
		{
			var vps = new VPS("", ip);
			var taskInfo = new StringBuilder();
			var tTitle = new StringBuilder();
			foreach(var s in serverInfoList)
			{
				if (singleHdl <= 0) break;
				if (s.Value.NowNum >0 && s.Value.Enable)
				{
					var t = s.Value;
					t.NowNum--;
					singleHdl--;
					vps.hdlServer.Add(t.Id);
					if (taskInfo.Length > 0)
					{
						tTitle.Append(",");
						taskInfo.Append("#");
					}
					taskInfo.Append(string.Format("<id>{0}</id><serverName>{1}</serverName><aeroId>{2}</aeroId><aeroName>{3}</aeroName>",t.Id,t.Name,t.AeroId,t.AeroName));
					tTitle.Append(t.Name);
				}
			}
			taskTitle = "Idle";
			if (taskInfo.Length == 0) return taskTitle;
			taskTitle = tTitle.ToString();
			allocServer.Add(vps.Ip,vps);
			return taskInfo.ToString();
		}

		private void CheckIfUserEditName(object sender, LabelEditEventArgs e)
		{
			try
			{
				var ip = LstConnection.Items[e.Item].SubItems[2].Text;
				TcpServer target = serverManager[ip];
				var clientName = e.Label;
				regSettingVps.In(target.ID).SetInfo("clientName", clientName);
				target.Send(string.Format("<setClientName>{0}</setClientName>", clientName));
			}
			catch (Exception ex)
			{
				AppendLog("修改终端名称失败:" + ex.Message);
			}
		
		}

		public void AppendLog(string info)
		{
			OpLog.AppendText("\n");
			OpLog.AppendText(string.Format("{0}>>{1}",DateTime.Now,info));
			if (OpLog.Text.Length > 1000)
			{
				OpLog.Clear();
			}
		}


		private void CmdDisconnect_Click(object sender, EventArgs e)
		{
			try
			{
				var nowSelect = LstConnection.SelectedItems[0].SubItems[2].Text;
				var tcp = serverManager[nowSelect];
				tcp.Disconnect();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void CmdRedial_Click(object sender, EventArgs e)
		{
			try
			{
				if(LstConnection.SelectedItems[0].SubItems[1].Text != "vps")
				{
					MessageBox.Show("仅VPS终端支持重新拨号");
					return;
				}
				var nowSelect = LstConnection.SelectedItems[0].SubItems[2].Text;
				var tcp = serverManager[nowSelect];
				tcp.Send("<reRasdial>");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		/// <summary>
		/// 切换启用状态
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LstServerQueue_DoubleClick(object sender, EventArgs e)
		{
			if (LstServerQueue.SelectedItems[0].SubItems[4].Text == "启用")
			{
				LstServerQueue.SelectedItems[0].SubItems[4].Text = "禁用";
				serverInfoList[LstServerQueue.SelectedItems[0].SubItems[0].Text].Enable = false;
			}
			else
			{
				serverInfoList[LstServerQueue.SelectedItems[0].SubItems[0].Text].Enable = true;
				LstServerQueue.SelectedItems[0].SubItems[4].Text = "启用";
			}
			regServerInfo.SetInfo(LstServerQueue.SelectedItems[0].SubItems[0].Text, LstServerQueue.SelectedItems[0].SubItems[4].Text);
		}

		private void LstGoodShow_DoubleClick_1(object sender, EventArgs e)
		{
			var target = LstGoodShow.SelectedItems[0];
			var targetUrl = target.SubItems[8].Text;
			Clipboard.SetText(targetUrl);
			ManagerHttpBase.UserWebShowTime++;
			SendCmdToBrowserClient(target.SubItems[0].Text, $"<showWeb><targetUrl>{targetUrl}</targetUrl></showWeb>");
		}

		private void CmdServerOn_Click(object sender, EventArgs e)
		{
			try
			{
				var nowSelect = LstConnection.SelectedItems[0].SubItems[2].Text;
				var tcp = serverManager[nowSelect];
				tcp.Send(IpSender.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
