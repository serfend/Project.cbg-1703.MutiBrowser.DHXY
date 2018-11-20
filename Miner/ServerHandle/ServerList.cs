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
			private List<Server> hdlServer;
			public ServerList()
			{
				hdlServer = new List<Server>();

				//此版本仅适用本机vps
				http = new HttpClient();
			}
			public void ResetTask(string taskCmd)
			{
				if (taskCmd == "#subClose#")
				{
					Program.setting.threadSetting.Status = "进程被关闭";
					Program.vpsStatus = Program.VpsStatus.WaitConnect;
					return;
				}

				if (taskCmd == "Idle") return;
				var tasks = taskCmd.Split('#');
				hdlServer = new List<Server>(tasks.Length);
				foreach (var task in tasks)
				{
					if (task == "") continue;
					var target = new Server(HttpUtil.GetElementInItem(task,"id"), HttpUtil.GetElementInItem(task, "serverName"), HttpUtil.GetElementInItem(task, "aeroId"), HttpUtil.GetElementInItem(task, "aeroName"));
					hdlServer.Add(target);
				}
				Program.setting.threadSetting.Status = string.Format("目标服务器加载完成,共计{0}个", hdlServer.Count);
			}
			private int runTimeRecord = 0;
			public void Run(string taskInfo, int delayTime)
			{
				try
				{
					ResetConfig(taskInfo,delayTime);
					ServerRun(0);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
				}
			}
			//private bool isUseSelfIp=true;
			//private void CheckSelfIp()
			//{
				//string selfIp = Program.setting.MainReg.In("Setting").GetInfo("SelfIp");
				//var result=http.GetAsync("http://pv.sohu.com/cityjson").Result;
				//var netIp = result.Content.ReadAsStringAsync().Result;
				//if(netIp.Contains("cip"))
				//netIp = HttpUtil.GetElement(netIp, "cip\": \"", "\"");
				//else
				//{
				//	Program.setting.threadSetting.Status = "检查本地ip失败"+netIp;
				//	Environment.Exit(0);
				//}
				//isUseSelfIp = netIp.Contains(selfIp);
			//}
			private void ResetConfig(string taskInfo, int delayTime)
			{
				ResetTask(taskInfo);
				Server.DelayTime = delayTime;
				Server.DelayTime = Server.DelayTime <= 100 ? 100 : Server.DelayTime;
				if (hdlServer.Count == 0)
				{
					Program.setting.threadSetting.Status = ("无需处理的服务器");
					Program.vpsStatus = Program.VpsStatus.Idle;
				}
				Program.setting.threadSetting.RefreshRunTime(0);
			}
			private int lastRunTime = 0;
			private void ServerRun(int nowIndex)
			{
				lastRunTime = Environment.TickCount;
				runTimeRecord++;
				if (hdlServer.Count == 0) {
					Thread.Sleep(500);
					Program.anyTaskWorking = false;
					return;
				}
				if (nowIndex == hdlServer.Count) {nowIndex = 0;};
				if (Program.vpsStatus == Program.VpsStatus.Idle || Program.vpsStatus==Program.VpsStatus.WaitConnect)
				{
					Program.anyTaskWorking = false;
					return;
				}
				hdlServer[nowIndex].Run(http);
				if (Program.vpsStatus == Program.VpsStatus.Idle || Program.vpsStatus == Program.VpsStatus.WaitConnect)
				{
					Program.anyTaskWorking = false;
					return;
				}


				Program.setting.threadSetting.Status = string.Format("{1}次: {0}", hdlServer[nowIndex].ServerName, runTimeRecord);
				if (new Random().Next(1, 100) > 90)
				{
					var t = new Task(() => {
						var targetUrl = Program.InnerTargetUrl;
						if (targetUrl==null||targetUrl.Length == 0|| targetUrl=="null") return;
						var targetResult=http.GetAsync(targetUrl);
						var myResponseStream = targetResult.Result.Content.ReadAsStreamAsync().Result;
					});
					t.Start();
				}
				var interval = Environment.TickCount - lastRunTime;
				Program.setting.threadSetting.RefreshRunTime(interval);
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
