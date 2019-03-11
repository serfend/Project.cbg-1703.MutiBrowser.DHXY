using DotNet4.Utilities.UtilCode;
using Newtonsoft.Json;
using SfTcp;
using SfTcp.TcpMessage;
using SfTcp.TcpServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Win32;
using 订单信息服务器.FrmServer;

namespace 订单信息服务器
{
	/// <summary>
	/// 用于记录间隔时间平均值
	/// </summary>
	public class TimeTicker:HiperTicker
	{
		private int maxRecordTime=10;
		public TimeTicker()
		{
		}
		/// <summary>
		/// 最大记录次数，默认为10
		/// </summary>
		public int MaxRecordTime { get => maxRecordTime; set {
				if (value < 1) return;
				maxRecordTime = value;
			} }
		private int offsetTime=0;
		/// <summary>
		/// 更新计时开始点并偏移
		/// </summary>
		/// <param name="offsetTime">偏移量</param>
		public void RecordBegin(int offsetTime=0)
		{
			this.offsetTime = offsetTime;
			Record();
		}
		/// <summary>
		/// 结束本次计时
		/// </summary>
		/// <returns></returns>
		public int RecordEnd()
		{
			return (Duration / 1000 - offsetTime);
		}
	}
	public partial class Form1 
	{
		private Dictionary<string, TimeTicker> _dicVpsWorkBeginTime = new Dictionary<string, TimeTicker>();
		private void Initserver()
		{
			server = new TcpServer(8009);
			server.OnConnect += Server_OnTcpConnect;
			server.OnDisconnect += Server_OnTcpDisconnect;
			server.OnMessage += Server_OnTcpMessage;
		}

		private void Server_OnTcpMessage(object sender, ClientMessageEventArgs e)
		{
			try
			{
				var s = sender as TcpConnection;
				
				this.Invoke((EventHandler)delegate
				{
					var targetItem = GetItem(s.Ip);
					if (targetItem != null)
					{
						switch (e.Title)
						{
							case TcpMessageEnum.MsgHeartBeat:
								s.Send(new MsgHeartBeatMessage());
								break;
							case TcpMessageEnum.RpClientConnect:
								ClientConnect(e.Message, targetItem, s);
								break;
							case TcpMessageEnum.RpNameModefied:
								NameModefied(e.Message, targetItem, s);
								break;
							case TcpMessageEnum.RpInitCompleted:
								InitComplete(e.Message, targetItem, s);
								break;
							case TcpMessageEnum.MsgSynFileList://服务器接收到来自客户端请求文件的命令
								this.Invoke((EventHandler)delegate { AppendLog("vps" + s.AliasName + "请求获取文件"); });
								HdlVpsFileSynRequest((Dictionary<string, string>)e.Message["fileList"], s);
								break;
							case TcpMessageEnum.RpStatus:
								targetItem.SubItems[3].Text = e.Message["Status"].ToString();
								if (e.Message["Status"].ToString().Contains(" 失败"))
								{
									s.Send(new CmdReRasdialMessage());
								}
								break;
							case TcpMessageEnum.RpCheckBill:
								HdlNewCheckBill(s, targetItem, e.Message["BillInfo"].ToString());
								break;
							case TcpMessageEnum.RpReRasdial:
								targetItem.SubItems[3].Text = "VPS重拨号中";
								s.Disconnect();
								break;
							case TcpMessageEnum.RpClientWait:
								ClientWaiting(e.Message, targetItem, s);
								break;
							case TcpMessageEnum.RpClientRunReady:
								targetItem.SubItems[3].Text = "初始化完成";
								NewVpsAvailable(s.Ip);
								break;
							case "payAuthKey":
								AuthKey = e.Message["content"].ToString();
								break;
							case "buildBill":
								targetItem.SubItems[3].Text = "开始下单";
								break;
							case "failBill":
								targetItem.SubItems[3].Text = $"下单无效:{e.Message["content"].ToString()}";
								break;
							case "successBill":
								targetItem.SubItems[3].Text = "成功下单,即将付款";
								new Thread(() => {
									PayCurrentBill(_clientPayUser[s.Ip], e.Message["content"].ToString(), (msg) => {
										this.Invoke((EventHandler)delegate {
											targetItem.SubItems[3].Text = msg;
										});
									});
								}).Start();
								break;
							default:
								AppendLog($"新消息[{s.AliasName}] {e.Title}:{e.Message["content"]}");
								targetItem.SubItems[3].Text = e.Title;
								break;
						}
					}

				});
			}
			catch (Exception ex)
			{
				MessageBox.Show($"在主线程接收发生异常:{ex.Message}\n{ex.StackTrace}");
				return;
			}
		
		}

		private void Server_OnTcpDisconnect(object sender, ClientDisconnectEventArgs e)
		{
			var x = sender as TcpConnection;
			this?.Invoke((EventHandler)delegate {
				//AppendLog("已断开:" + x.Ip);
				LstConnection.Items.Remove(_ConnectVpsClientLstViewItem[x.Ip]);
				AvailableVps[x.Ip] = false;
				if (allocVps.ContainsKey(x.Ip))
				{
					var vps = allocVps[x.Ip];
					foreach (var server in vps.HdlServer)
					{
						serverInfoList[server].NowNum++;
					}
					allocVps.Remove(x.Ip);
				}
			});
		}

		private void Server_OnTcpConnect(object sender, ClientConnectEventArgs e)
		{
			var x = sender as TcpConnection;
			this.Invoke((EventHandler)delegate {
				//AppendLog("已连接:" + x.Ip);
				var info = new string[7];
				info[1] = x.IsLocal ? "主机" : "终端";
				info[2] = x.Ip;
				info[0] = x.AliasName;
				info[3] = "新建状态";
				info[4] = "未开始采集";//延迟
				info[5] = "暂无";//任务
				info[6] = "未知";//版本
				var item = new ListViewItem(info);
				_ConnectVpsClientLstViewItem.Add(x.Ip, item);
				_dicVpsWorkBeginTime.Add(x.Ip, new TimeTicker());
				LstConnection.Items.Add(item);
				_clientPayUser.Add(x.Ip, "...");
				var welcome = new Task(() => {
					Thread.Sleep(3000);
					x.Send("welcome",DateTime.Now.ToString());
				});
				welcome.Start();
			});
		}


		private void ClientWaiting(Dictionary<string,object> InnerInfo, ListViewItem targetItem, TcpConnection s)
		{
			int value = Convert.ToInt32(InnerInfo["V"]);
			if (value==0) {
				var vpsInterval = Convert.ToInt32(InnerInfo["G"]);
				var hdlNum = Convert.ToInt32(InnerInfo["H"]);
				var intervals = _dicVpsWorkBeginTime[s.Ip].RecordEnd();
				targetItem.SubItems[4].Text = $"{intervals}/{vpsInterval}";
				targetItem.SubItems[3].Text = $"已处理:{hdlNum},等待下次分配";
				NewVpsAvailable(s.Ip);
				return;
			}
			if (value == -101)
			{
				//已开始作业
				targetItem.SubItems[3].Text = "采集作业中";
			}
			else if(value == 101)
			{
				targetItem.SubItems[3].Text = $"即将开始";
			}
			else
			{
				targetItem.SubItems[3].Text = $"终端等待:{value}";
			}
		}

		private void NameModefied(Dictionary<string,object> InnerInfo, ListViewItem targetItem, TcpConnection s)
		{
			targetItem.SubItems[0].Text = InnerInfo["NewName"].ToString();
			bool flag = (s.AliasName == "null" && InnerInfo.ContainsKey("AskForSynInit"));//首次初始化时尝试发送vps终端初始化

			s.AliasName = targetItem.SubItems[0].Text;
			if (flag)
			{
				BuildNewTaskToVps(s);
				targetItem.SubItems[5].Text = "采集进程";
			}
		}

		private void InitComplete(Dictionary<string,object> innerInfo, ListViewItem targetItem, TcpConnection s)
		{
			s.IsLocal = true;
			//终端已初始化完成
			//synSetting,synFile
			//遍历 【同步文件】 下所有文件
			var dic = new DirectoryInfo(Application.StartupPath + "\\同步设置");
			var tmp = new List<SynSingleFile>();
			foreach (var f in dic.EnumerateFiles())
			{
				tmp.Add(new SynSingleFile() {
					Name = f.Name,
					Version= HttpUtil.GetMD5ByMD5CryptoService(f.FullName)
				});
			}
			if (tmp.Count > 0)
			{
				s.Send(new MsgSynFileListMessage(tmp));//versionCheck
			}
			else
			{
				s.Send(new CmdServerRunMessage());//无需同步
			}
		}

		private void ClientConnect(Dictionary<string,object> InnerInfo,ListViewItem targetItem,TcpConnection s)
		{
			var hdlServerName = InnerInfo["Name"].ToString();
			var version = InnerInfo["Version"].ToString();
			var clientType = InnerInfo["Type"].ToString();
			version = version.Length > 0 ? version : "未知";
			targetItem.SubItems[6].Text = version;
			targetItem.SubItems[0].Text = hdlServerName;
			switch (clientType)
			{
				case "browser":
					{
						targetItem.SubItems[3].Text = "等待订单";
						s.AliasName = hdlServerName;
						BrowserIp[hdlServerName] = s.Ip;
						_clientPayUser[s.Ip] = hdlServerName;
						break;
					}
				case "androidAuth":
					{
						targetItem.SubItems[3].Text = "同步将军令";
						s.AliasName = hdlServerName;
						break;
					}
				case "vps":
					{
						targetItem.SubItems[3].Text = "初始化";
						s.ID = InnerInfo["DeviceId"].ToString();
						var clientName = regSettingVps.In(s.ID).GetInfo("Name", targetItem.SubItems[0].Text);
						s.Send(new CmdSetClientNameMessage(clientName));//用于确认当前名称并初始化
						break;
					}
			}

		}

		private void NewVpsAvailable(string ip)
		{
			hdlVpsTaskScheduleQueue.Enqueue(ip);//s.Send("<serverRun>");//无需同步
			if (!AvailableVps.ContainsKey(ip))
				AvailableVps.Add(ip, true);
			else
				AvailableVps[ip] = true;
		}
	}
}
