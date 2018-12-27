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
	public partial class Form1 : Form
	{

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
								s.Send("heartBeatResponse");
							}
							else if (x.Contains("RHB"))
							{
								targetItem.SubItems[4].Text = InnerInfo;
							}
							else if (x.Contains("clientConnect"))
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
								}
								else if (InnerInfo.Contains("<androidAuthInit>"))
								{
									var phone = HttpUtil.GetElementInItem(InnerInfo, "phone");
									_clientPayUser[s.Ip] = phone;
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
							else if (x.Contains("nameModefied"))
							{
								targetItem.SubItems[0].Text = HttpUtil.GetElementInItem(InnerInfo, "clientName");
								bool flag = (s.clientName == "..." && InnerInfo.Contains("<AskForSynInit>"));//首次初始化时尝试发送vps终端初始化

								s.clientName = targetItem.SubItems[0].Text;
								if (flag)
								{
									BuildNewTaskToVps(s, out string taskTitle);
									targetItem.SubItems[5].Text = "采集进程"; //taskTitle;
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
									hdlVpsTaskScheduleQueue.Enqueue(s.Ip);//s.Send("<serverRun>");//无需同步
									if (!AvailableVps.ContainsKey(s.Ip))
										AvailableVps.Add(s.Ip, true);
									else
										AvailableVps[s.Ip]= true;
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
								if (InnerInfo.Contains(" 失败"))
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
							else if (x.Contains("clientWait"))
							{
								var interval = Convert.ToInt32(InnerInfo);
								if (interval == -101)
								{
									//已开始作业
									targetItem.SubItems[3].Text = "采集作业中";
								}else if (interval <= 0)
								{
									targetItem.SubItems[3].Text = $"已处理:{-interval},等待下次分配";
									NewVpsAvailable(s.Ip);
								}
								else
								{
									targetItem.SubItems[3].Text = $"等待{interval}ms";
								}
							}
							else if (x.Contains("clientConfigComplete"))
							{
								targetItem.SubItems[3].Text = "初始化完成";
								NewVpsAvailable(s.Ip);
							}
							else if (x.Contains("loginSession"))
							{
								if (serverInfoList.ContainsKey(s.clientName))
								{
									serverInfoList[s.clientName].LoginSession = InnerInfo;
									bool anyVpsApply = false;
									foreach (var vps in allocServer)
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
							else if (x.Contains("payAuthKey"))
							{
								AuthKey=InnerInfo;
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
						var info = new string[7];
						info[1] = x.IsLocal ? "主机" : "终端";
						info[2] = x.Ip;
						info[0] = x.clientName;
						info[3] = "新建状态";
						info[4] = "未开始采集";//延迟
						info[5] = "暂无";//任务
						info[6] = "未知";//版本
						LstConnection.Items.Add(new ListViewItem(info));
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
						for (int i = 0; i < LstConnection.Items.Count; i++)
							if (LstConnection.Items[i].SubItems[2].Text == x.Ip)
							{
								LstConnection.Items.RemoveAt(i);
								AvailableVps[x.Ip] = false;
								break;
							}
						if (allocServer.ContainsKey(x.Ip))
						{
							var vps = allocServer[x.Ip];
							foreach (var server in vps.HdlServer)
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

		private void NewVpsAvailable(string ip)
		{
			hdlVpsTaskScheduleQueue.Enqueue(ip);//s.Send("<serverRun>");//无需同步
			if (!AvailableVps.ContainsKey(ip))
				AvailableVps.Add(ip, true);
			else
				AvailableVps[ip] = true;
		}

		private void InitServerTaskList()
		{
			var serverInfo = File.ReadAllLines("ServerInfo.txt", Encoding.Default);
			foreach (var server in serverInfo)
			{
				var info = server.Split(',');
				var data = new string[5];
				if (info.Length < 4)
				{
					AppendLog("【警告】无效的区信息:" + server);
					continue;
				}
				var hdlNum = info.Length == 5 ? Convert.ToInt32(info[4]) : 1;
				var s = new HdlServerInfo(info[0], info[1], info[2], info[3], hdlNum);
				if (serverInfoList.ContainsKey(s.Name))
				{
					AppendLog("【警告】重复的区:" + server);
					continue;
				}
				serverInfoList.Add(s.Name, s);
				data[0] = s.Id;//区号
				data[1] = s.Name;//名称
				data[2] = "0";//已分配
				data[3] = s.HdlNum.ToString();//需分配
				data[4] = regServerInfo.GetInfo(s.Id, "启用");
				s.Enable = (data[4] == "启用");
				LstServerQueue.Items.Add(new ListViewItem(data));
			}
		}
	}
}
