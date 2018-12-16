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
using Miner.ServerHandle;

namespace Miner
{
	namespace Server
	{
		class ServerList
		{
			private List<Server> hdlServer;
			public ServerList()
			{
				HdlServer = new List<Server>();

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
				HdlServer = new List<Server>(tasks.Length);
				foreach (var task in tasks)
				{
					if (task == "") continue;
					var target = new Server(HttpUtil.GetElementInItem(task,"id"), HttpUtil.GetElementInItem(task, "serverName"), HttpUtil.GetElementInItem(task, "aeroId"), HttpUtil.GetElementInItem(task, "aeroName"),HttpUtil.GetElementInItem(task,"loginSession"));
					HdlServer.Add(target);
				}
				Program.setting.threadSetting.Status = string.Format("目标服务器加载完成,共计{0}个", HdlServer.Count);
			}
			private int runTimeRecord = 0;
			
			#region checkip
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
			#endregion
			public void ResetConfig(string taskInfo, int delayTime,double assumePriceRate)
			{
				//TODO 暂时关闭任务分配 ResetTask(taskInfo);
				Server.DelayTime = delayTime;
				Server.DelayTime = Server.DelayTime <= 100 ? 100 : Server.DelayTime;
				Server.AssumePriceRate = assumePriceRate;
				
				//if (HdlServer.Count == 0)
				//{
				//	Program.setting.threadSetting.Status = ("无需处理的服务器");
				//	Program.vpsStatus = Program.VpsStatus.Idle;
				//}

				Program.setting.threadSetting.RefreshRunTime(0);
			}
			private int lastRunTime = 0;
			private AppInterface appInterface = new AppInterface();
			public void ServerRun()
			{
				lastRunTime = Environment.TickCount;
				runTimeRecord++;
				int hdlGoodNum = 0;
				try
				{
					var goodList = appInterface.GetGoodList();
					var list = appInterface.GetNeedHandle();
					foreach(var goodItem in list)
					{
						var goodDetailRaw =appInterface.GetGoodDetail(goodItem);
						if (goodDetailRaw != null)
						{
							var goodDetail = goodDetailRaw.equip;
							var good = new Goods.Goods(new Server(goodDetail.serverid.ToString(), goodDetail.server_name, goodDetail.areaid.ToString(), goodDetail.area_name, ""), goodDetail.equip_name, goodDetail.game_ordersn, goodDetail.equip_desc, goodDetail.equip_detail_url)
							{
								Price = goodDetail.price_desc,
								BookStatus = goodDetail.status_desc,
								Rank = goodDetail.level_desc
							};
							good.CheckAndSubmit();
							hdlGoodNum++;
						}
					}
					Thread.Sleep(200);//TODO 后期需要时取消
					Program.Tcp?.Send("clientWait", $"-{hdlGoodNum}");
				}
				//catch (GoodListNoDataException ex)
				//{
				//	Program.Tcp?.Send("clientWait", "-1");
				//	Server.ExitAftert ( $"#:{ex.Message}");
				//	Program.anyTaskWorking = false;
				//	return;
				//}
				catch (Exception ex)
				{
					Program.Tcp?.Send("clientWait", "-1");
					Server.ExitAftert($"#:{ex.Message}");
					Program.anyTaskWorking = false;
					return;
				}
				try
				{

					var interval = Environment.TickCount - lastRunTime;
					Program.setting.threadSetting.RefreshRunTime(interval);

					#region 暂时关闭
					//if (HdlServer.Count == 0) {
					//	Thread.Sleep(500);
					//	Program.anyTaskWorking = false;
					//	return;
					//}
					//if (nowIndex == HdlServer.Count) {nowIndex = 0;};
					if (Program.vpsStatus == Program.VpsStatus.Idle || Program.vpsStatus == Program.VpsStatus.WaitConnect)
					{
						Program.anyTaskWorking = false;
						return;
					}
					if (Program.vpsStatus == Program.VpsStatus.Idle || Program.vpsStatus == Program.VpsStatus.WaitConnect)
					{
						Program.anyTaskWorking = false;
						return;
					}


					//Program.setting.threadSetting.Status = string.Format("{1}次: {0}", "App接口", runTimeRecord);
					#endregion
					//if (new Random().Next(1, 100) > 90)
					//{
					//	var t = new Task(() => {
					//		var targetUrl = Program.InnerTargetUrl;
					//		if (targetUrl == null || targetUrl.Length == 0 || targetUrl == "null") return;
					//		var targetResult = http.GetAsync(targetUrl);
					//		var myResponseStream = targetResult.Result.Content.ReadAsStreamAsync().Result;
					//	});
					//	t.Start();
					//}

				}
				catch (Exception ex)
				{
					Program.Tcp?.Send("clientWait", "-1");
					Server.ExitAftert($"处理结束后发生异常:{ex.Message}");
					Program.anyTaskWorking = false;
					return;
				}
			}

			public List<Server> HdlServer { get => hdlServer; set => hdlServer = value; }


		}
		

	}
	
}
