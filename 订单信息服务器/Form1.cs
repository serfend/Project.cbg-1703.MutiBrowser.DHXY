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
			transferFileEngine = new TransferFileEngine(TcpFiletransfer.TcpTransferEngine.Connections.Connection.EngineModel.AsServer, "", 8010);
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
							} else if (x.Contains("RefreshHeartbeat"))
							{
								targetItem.SubItems[4].Text = InnerInfo;
							}
							else if (x.Contains("clientConnect"))
							{
								targetItem.SubItems[3].Text = "初始化";
								targetItem.SubItems[0].Text = HttpUtil.GetElementInItem(InnerInfo, "connectCmdRequire");
								s.ID = HttpUtil.GetElementInItem(InnerInfo, "clientDeviceId");
								var clientName = regSetting.In(s.ID).GetInfo("clientName", targetItem.SubItems[0].Text);
								s.Send(string.Format("<setClientName>{0}</setClientName>", clientName));//用于确认当前名称并初始化
							}
							else if (x.Contains("nameModefied"))
							{
								targetItem.SubItems[0].Text = HttpUtil.GetElementInItem(InnerInfo,"clientName");
								if (s.clientName == "..." && InnerInfo.Contains("<AskForSynInit>"))//首次初始化时尝试发送vps终端初始化
								{
									BuildNewTaskToVps(s,out string taskTitle);
									targetItem.SubItems[5].Text = taskTitle;
									SynLstTask();
								}
								s.clientName = InnerInfo;
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
							}
							else if (x.Contains("newCheckBill"))
							{
								AppendLog("订单" + InnerInfo);
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
						AppendLog("已连接:" + x.Ip);
						var info = new string[6];
						info[1] = x.IsLocal ? "主机" : "终端";
						info[2] = x.Ip;
						info[0] = x.clientName;
						info[3] = "新建状态";
						info[4] = "等待连接";//延迟
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
						AppendLog("已断开:" + x.Ip);
						for (int i = 0; i < LstConnection.Items.Count; i++)
							if (LstConnection.Items[i].SubItems[2].Text == x.Ip)
								LstConnection.Items.RemoveAt(i);
						if (allocServer.ContainsKey(x.Ip))
						{
							var vps = allocServer[x.Ip];
							foreach(var server in vps.hdlServer)
							{
								var s = serverInfoList[server];
								s.NowNum--;
							}
							allocServer.Remove(x.Ip);
							SynLstTask();
						}
					});
				},
				HttpRequest = (x, s) => {
					s.Response(string.Format("<h1>Hey,测试服务器已开启</h1><br><p>当前连接数:{0}</p>", LstConnection.Items.Count));
				}
			};
		}

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
			s.Send(string.Format("<SynInit><interval>{0}</interval><task>{1}</task><timeout>{2}</timeout></SynInit>", interval, hdlServer, timeout));
		}
		private struct HdlServerInfo
		{
			string id;
			string name;
			string aeroId;
			string aeroName;
			int hdlNum;
			int nowNum;

			public HdlServerInfo(string id, string name, string aeroId, string aeroName, int hdlNum) : this()
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
		}
		private struct VPS
		{
			string name;
			string ip;
			public List<string> hdlServer;

			public VPS(string name, string ip) : this()
			{
				this.Name = name;
				this.Ip = ip;
				hdlServer = new List<string>();
			}

			public string Name { get => name; set => name = value; }
			public string Ip { get => ip; set => ip = value; }
		}
		private Dictionary<string, HdlServerInfo> serverInfoList=new Dictionary<string, HdlServerInfo>();
		private Dictionary<string, VPS> allocServer=new Dictionary<string, VPS>();//以ip对应终端
		private void InitServerTaskList()
		{
			var serverInfo = File.ReadAllLines("ServerInfo.txt",Encoding.Default);
			foreach(var server in serverInfo)
			{
				var info = server.Split(',');
				var data = new string[4];
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
		public Form1()
		{
			regSetting = new Reg("sfMinerDigger").In("Setting").In("vps");
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
			var frmSetting = new Reg("sfMinerDigger").In("Setting").In("Form").In("ServerFormMain");
			foreach (var ctl in this.Controls)
			{
				if(ctl is TextBox t)
				{
					if (t.Tag==null || t.Tag.ToString() != "RecordReg") continue;
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
				if (s.Value.NowNum < s.Value.HdlNum)
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
				regSetting.In(target.ID).SetInfo("clientName", clientName);
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
