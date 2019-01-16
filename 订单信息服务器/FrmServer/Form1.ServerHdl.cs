using DotNet4.Utilities.UtilCode;
using SfTcp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 订单信息服务器
{
	/// <summary>
	/// 用于记录间隔时间平均值
	/// </summary>
	public class TimeTicker
	{
		private int lastTick;
		private int delaySumCount;
		private int totalTick;
		private int maxRecordTime=10;
		public TimeTicker()
		{
			LastTick = Environment.TickCount;
		}
		public int LastTick { get => lastTick; set => lastTick = value; }
		/// <summary>
		/// 最大记录次数，默认为10
		/// </summary>
		public int MaxRecordTime { get => maxRecordTime; set {
				if (value < 1) return;
				maxRecordTime = value;
				delaySumCount = 0;
			} }

		public int Record()
		{
			if (delaySumCount < maxRecordTime) delaySumCount++;
			int interval = Environment.TickCount - LastTick;
			LastTick = Environment.TickCount;
			totalTick += (interval - totalTick) / delaySumCount;
			return totalTick;
		}
	}
	public partial class Form1 
	{
		private Dictionary<string, TimeTicker> _dicVpsWorkBeginTime = new Dictionary<string, TimeTicker>();
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
							switch (x)
							{
								case "heartBeat":
									s.Send("heartBeatResponse");
									break;
								case "RHB":
									var now = Environment.TickCount;
									var interval = _dicVpsWorkBeginTime[s.Ip].Record();
									targetItem.SubItems[4].Text =$"{interval}({InnerInfo})";
									break;
								case "clientConnect":
									ClientConnect(InnerInfo,targetItem,s);
									break;
								case "nameModefied":
									NameModefied(InnerInfo,targetItem,s);
									break;
								case "InitComplete":
									InitComplete(InnerInfo,targetItem,s);
									break;
								case "RequireFile"://服务器接收到来自客户端请求文件的命令
									this.Invoke((EventHandler)delegate { AppendLog("vps" + s.clientName + "请求获取文件"); });
									HdlVpsFileSynRequest(InnerInfo, s);
									break;
								case "Status":
									targetItem.SubItems[3].Text = InnerInfo;
									if (InnerInfo.Contains(" 失败"))
									{
										s.Send("<reRasdial>");
									}
									break;
								case "newCheckBill":
									HdlNewCheckBill(s.clientName, InnerInfo);
									break;
								case "reRasdial":
									targetItem.SubItems[3].Text = "VPS重拨号中";
									s.Disconnect();
									break;
								case "clientWait":
									ClientWaiting(InnerInfo,targetItem, s);
									break;
								case "clientConfigComplete":
									targetItem.SubItems[3].Text = "初始化完成";
									NewVpsAvailable(s.Ip);
									break;
								case "loginSession":
									SynLoginSession(InnerInfo, targetItem, s);
									break;
								case "payAuthKey":
									AuthKey = InnerInfo;
									break;
								case "buildBill":
									targetItem.SubItems[3].Text = "开始下单";
									break;
								case "failBill":
									targetItem.SubItems[3].Text = $"下单无效:{InnerInfo}";
									break;
								case "successBill":
									targetItem.SubItems[3].Text = "成功下单,即将付款";
									PayCurrentBill(_clientPayUser[s.Ip],InnerInfo,(msg)=> {
										MessageBox.Show(msg);
									});
									break;
								default:
									AppendLog("新消息[" + s.clientName + "] " + x + ":" + InnerInfo);
									targetItem.SubItems[3].Text = InnerInfo;
									break;
							}
						}

					});
				},
				ServerConnected = (x) => {
					this.Invoke((EventHandler)delegate {
						//AppendLog("已连接:" + x.Ip);
						var info = new string[7];
						info[1] = x.IsLocal ? "主机" : "终端";
						info[2] = x.Ip;
						info[0] = x.clientName;
						info[3] = "新建状态";
						info[4] = "未开始采集";//延迟
						info[5] = "暂无";//任务
						info[6] = "未知";//版本
						var item = new ListViewItem(info);
						_ConnectVpsClientLstViewItem.Add(x.Ip, item);
						_dicVpsWorkBeginTime.Add(x.Ip,new TimeTicker());
						LstConnection.Items.Add(item);
						_clientPayUser.Add(x.Ip, "...");
						var welcome = new Task(() => {
							Thread.Sleep(3000);
							x.Send("<welcome>" + DateTime.Now + "</welcome>");
						});
						welcome.Start();
					});
				},
				ServerDisconnected = (x) => {
					this?.Invoke((EventHandler)delegate {
						//AppendLog("已断开:" + x.Ip);
						LstConnection.Items.Remove(_ConnectVpsClientLstViewItem[x.Ip]);
						AvailableVps[x.Ip]=false;
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
				},
				HttpRequest = (x, s) => {
					var cst = new StringBuilder();
					cst.AppendLine($"<h1>Hey,测试服务器已开启</h1><br><p>当前连接数:{LstConnection.Items.Count }</p>");
					cst.AppendLine($"<p>request: {x.Param}</p>");
					var checkIfHaveValue = x.Param.IndexOf(':');
					string requestPage, requestParam;
					if (checkIfHaveValue > 0)
					{
						requestPage = x.Param.Substring(0, checkIfHaveValue);
						requestParam = x.Param.Substring(checkIfHaveValue + 1);
						requestParam = requestParam.Replace("%3C", "<").Replace("%3E",">");
					}
					else
					{
						requestPage = x.Param;
						requestParam = string.Empty;
					}
					switch (requestPage)
					{
						case "Status":
							{
								this.Invoke((EventHandler)delegate {
									var clientNum = this.LstConnection.Items.Count;
									var columnsNum = LstConnection.Columns.Count;
									cst.AppendLine($"<p>当前状态共有{clientNum}个连接</p><br>");
									cst.AppendLine($"<p>authKey:{AuthKey}</p>");
									cst.AppendLine($"打开网页次数: 手动:{ManagerHttpBase.UserWebShowTime}  自动:{ManagerHttpBase.FitWebShowTime}<br>");
									cst.AppendLine($"profit:{ManagerHttpBase.RecordMoneyGet}  times:{ManagerHttpBase.RecordMoneyGetTime}");
									for(int i = 0; i < ManagerHttpBase.RecordMoneyGetTime; i++)
									{
										var info = ManagerHttpBase.GetBillInfo(i);
										if (info.Length == 0) continue;
										cst.AppendLine($"{i}:{info}<br>");
									}
									cst.AppendLine("<table border=\"1\">");
									cst.AppendLine("<tr>");
									for (int i = 0; i < columnsNum; i++)
										cst.Append($"<th>{LstConnection.Columns[i].Text}</th>");

									cst.AppendLine("</tr>");
									var clientTypeCounter = new Dictionary<string, int>();
									for (int i = 0; i < clientNum; i++)
									{
										var clientTypeName = LstConnection.Items[i].SubItems[1].Text;
										if (!clientTypeCounter.ContainsKey(clientTypeName)) clientTypeCounter.Add(clientTypeName, 1);
										else clientTypeCounter[clientTypeName]++;
										cst.AppendLine("<tr>");
										for (int j = 0; j < columnsNum; j++) {
											cst.Append($"<td>{LstConnection.Items[i].SubItems[j].Text}</td>");
										}
										cst.Append($"<td><a href=\"/CmdInfo:{LstConnection.Items[i].SubItems[2].Text}:<SubClose>\">关闭</td>");
										cst.Append($"<td><a href=\"/CmdInfo:{LstConnection.Items[i].SubItems[2].Text}:<startNew>\">新增</td>");
										cst.Append($"<td><a href=\"/CmdInfo:{LstConnection.Items[i].SubItems[2].Text}:<reRasdial>\">重连</td>");
										cst.AppendLine("</tr>");
									}
									cst.Append("</table>");
									cst.AppendLine("<div id=\"clientTypeCount\">");
									foreach(var item in clientTypeCounter)
									{
										cst.AppendLine($"{item.Key}:{item.Value}");
									}
									cst.AppendLine("</div>");
									cst.AppendLine($"<div>{OpLog.Text}</div>");
								});
								break;
							}
						case "targetUrl":
							{
								var nowTargetUrl = ManagerHttpBase.TargetUrl;
								if (checkIfHaveValue > 0)
								{
									ManagerHttpBase.TargetUrl = requestParam;
								}
								cst.AppendLine($"targetPrevious: {nowTargetUrl}");
								cst.AppendLine($"targetNew: {ManagerHttpBase.TargetUrl}");
								break;
							}
						case "CmdInfo":
							{
								var cmdInfo = requestParam.Split(':');
								if (cmdInfo.Length < 2)
								{
									cst.AppendLine($"无效的指令{requestParam},指令格式:CmdInfo:target#cmd");
									break;
								}
								var targetClient = serverManager[cmdInfo[0]];
								if (targetClient == null)
								{
									cst.AppendLine("无效的IP");
								}
								else
								{
									targetClient.Send(cmdInfo[1]);
									cst.AppendLine($"已向终端{targetClient.clientName}发送指令{cmdInfo[1]}");
								}
								break;
							}
					}
					s.Response(cst.ToString());
				}
			};
			serverManager.StartListening();
		}

		private void SynLoginSession(string InnerInfo, ListViewItem targetItem, TcpServer s)
		{
			if (serverInfoList.ContainsKey(s.clientName))
			{
				serverInfoList[s.clientName].LoginSession = InnerInfo;
				bool anyVpsApply = false;
				foreach (var vps in allocVps)
				{
					if (vps.Value.HdlServer.Contains(s.clientName))
					{
						serverManager[vps.Value.Ip].Send($"<SynServerLoginSession><Server><name>{s.clientName}</name><login>{InnerInfo}</login></Server>");
						anyVpsApply = true;
					}
				}
				if (!anyVpsApply)
				{
					AppendLog($"当前无{s.clientName}所需要应用的vps终端");
				}
			}
			else
			{
				AppendLog($"无效的浏览器终端:{s.clientName}");
			}
		}

		private void ClientWaiting(string InnerInfo, ListViewItem targetItem, TcpServer s)
		{
			var interval = Convert.ToInt32(InnerInfo);
			if (interval == -101)
			{
				//已开始作业
				targetItem.SubItems[3].Text = "采集作业中";
			}
			else if (interval <= 0)
			{
				targetItem.SubItems[3].Text = $"已处理:{-interval},等待下次分配";
				NewVpsAvailable(s.Ip);
			}
			else if(interval==101)
			{
				targetItem.SubItems[3].Text = $"即将开始";
			}
			else
			{
				targetItem.SubItems[3].Text = $"终端等待:{InnerInfo}";
			}
		}

		private void NameModefied(string InnerInfo, ListViewItem targetItem, TcpServer s)
		{
			targetItem.SubItems[0].Text = HttpUtil.GetElementInItem(InnerInfo, "clientName");
			bool flag = (s.clientName == "..." && InnerInfo.Contains("<AskForSynInit>"));//首次初始化时尝试发送vps终端初始化

			s.clientName = targetItem.SubItems[0].Text;
			if (flag)
			{
				BuildNewTaskToVps(s);
				targetItem.SubItems[5].Text = "采集进程";
			}
		}

		private void InitComplete(string innerInfo, ListViewItem targetItem, TcpServer s)
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

				hdlVpsTaskScheduleQueue.Enqueue(s.Ip);
				if (!AvailableVps.ContainsKey(s.Ip))
					AvailableVps.Add(s.Ip, true);
				else
					AvailableVps[s.Ip] = true;
			}
		}

		private void ClientConnect(string InnerInfo,ListViewItem targetItem,TcpServer s)
		{
			var hdlServerName = HttpUtil.GetElementInItem(InnerInfo, "clientName");
			var version = HttpUtil.GetElementInItem(InnerInfo, "version"); version = version.Length > 0 ? version : "未知";
			targetItem.SubItems[6].Text = version;
			targetItem.SubItems[0].Text = hdlServerName;
			if (InnerInfo.Contains("<browserInit>"))
			{
				targetItem.SubItems[3].Text = "等待订单";
				s.clientName = hdlServerName;
				BrowserIp[hdlServerName] = s.Ip;
				_clientPayUser[s.Ip] = hdlServerName;
			}
			else if (InnerInfo.Contains("<androidAuthInit>"))
			{
				targetItem.SubItems[3].Text = "同步将军令";
				s.clientName = hdlServerName;
			}
			else
			{
				targetItem.SubItems[3].Text = "初始化";
				s.ID = HttpUtil.GetElementInItem(InnerInfo, "clientDeviceId");
				var clientName = regSettingVps.In(s.ID).GetInfo("clientName", targetItem.SubItems[0].Text);
				s.Send(string.Format("<setClientName>{0}</setClientName>", clientName));//用于确认当前名称并初始化
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
