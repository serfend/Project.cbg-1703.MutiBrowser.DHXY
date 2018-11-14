using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilReg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Miner.Goods;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Net.Http;

namespace Miner
{
	namespace Server
	{
		class ServerList:IDisposable
		{
			private HttpClient http;
			private Dictionary<string, Server> serverList;
			private List<Server> hdlServer;
			public ServerList()
			{
				hdlServer = new List<Server>();

				serverList = new Dictionary<string, Server>();
				var ServerInfo = Program.setting.MainReg.In("Setting").In("ServerInfo").In("ServerList");
				var index = 1;
				do
				{
					var thisServer = ServerInfo.GetInfo("Server " + index);
					if (thisServer == "") break;
					index++;
					var info = thisServer.Split(',');
					serverList[info[0]] = new Server(info[0], info[1], info[2], info[3]);
				} while (true);
				var globalIpRecorder = Program.setting.MainReg.In("Data").In("RecordIp");
				//此版本仅适用本机vps
				http = new HttpClient();
			}
			public void ResetTask(string taskCmd)
			{
				if (taskCmd == "#subClose#")
				{
					Program.setting.threadSetting.Status = "进程被关闭";
					Program.vpsStatus = Program.VpsStatus.WaitConnect;
				}
				var tasks = taskCmd.Split('#');
				hdlServer = new List<Server>(tasks.Length);
				foreach (var task in tasks)
				{
					if (task == "") continue;
					var target = serverList.ContainsKey(task) ? serverList[task] : null;

					if (target == null) Program.setting.threadSetting.Status = ("获取失败,无效的服务器:" + task);
					else hdlServer.Add(target);
				}
				Program.setting.threadSetting.Status = string.Format("目标服务器加载完成,共计{0}个", hdlServer.Count);
			}
			private int runTimeRecord = 0;
			public void Run()
			{
				try
				{
					Main();
					ServerRun(0);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
				}
			}
			private bool isUseSelfIp=true;
			private void CheckSelfIp()
			{
				string selfIp = Program.setting.MainReg.In("Setting").GetInfo("SelfIp");
				var result=http.GetAsync("http://pv.sohu.com/cityjson").Result;
				var netIp = result.Content.ReadAsStringAsync().Result;
				if(netIp.Contains("cip"))
				netIp = HttpUtil.GetElement(netIp, "cip\": \"", "\"");
				else
				{
					Program.setting.threadSetting.Status = "检查本地ip失败"+netIp;
					Environment.Exit(0);
				}
				isUseSelfIp = netIp.Contains(selfIp);
			}
			private void Main()
			{
				Program.setting.threadSetting.RefreshRunTime();
				
				
				var config = Program.setting.threadSetting;
				var haveRefresh = config.ThisThreadIsRefresh;

				if (haveRefresh)
				{
					Program.setting.threadSetting.Status = ("更新设置");
					ResetTask(config.Task);
					Server.DelayTime = config.DelayTime;
					Server.DelayTime = Server.DelayTime <= 100 ? 100 : Server.DelayTime;
				}
				if (hdlServer.Count == 0)
				{
					Program.setting.threadSetting.Status = ("无需要处理的服务器");
					Thread.Sleep(5000);
					Environment.Exit(0);
				}
				//else
				//	Program.setting.threadSetting.Status = hdlServer.Count + "个服务器运行开始";
				
				Program.setting.threadSetting.RefreshRunTime();
			}
			private void ServerRun(int nowIndex)
			{
				runTimeRecord++;
				if (hdlServer.Count == 0) {
					Thread.Sleep(500);
					return;
				}
				if (nowIndex == hdlServer.Count) {
					Main();
					nowIndex = 0;
				};
				hdlServer[nowIndex].Run(http);
				/*if(runTimeRecord%10==0)*/
				Program.setting.threadSetting.Status = string.Format("{1}次刷新 {0} 结束", hdlServer[nowIndex].ServerName, runTimeRecord);
				if (new Random().Next(1, 100) > 90)
				{
					var t = new Task(() => {
						var targetUrl = Program.setting.MainReg.GetInfo("TargetUrl");
						var targetResult=http.GetAsync(targetUrl);
						var myResponseStream = targetResult.Result.Content.ReadAsStreamAsync().Result;
					});
					t.Start();
				}
				Program.setting.threadSetting.RefreshRunTime();
				Thread.Sleep(Server.DelayTime);
				new Thread(() => {
					ServerRun(nowIndex + 1);
				}).Start();
			}

			#region IDisposable Support
			private bool disposedValue = false; // 要检测冗余调用

			protected virtual void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if (disposing)
					{
						if (http != null) http.Dispose();
					}
					http = null;
					disposedValue = true;
				}
			}

			// 添加此代码以正确实现可处置模式。
			public void Dispose()
			{
				// 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
				Dispose(true);
			}
			#endregion

		}
		

	}
	
}
