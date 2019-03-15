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
using SfTcp.TcpMessage;

namespace Miner
{
	namespace Server
	{
		class ServerList:IDisposable
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
			private int lastServerRunTime = 0;
			
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
			public void ResetConfig( int delayTime,double assumePriceRate)
			{
				Server.DelayTime = delayTime;
				Server.DelayTime = Server.DelayTime <= 100 ? 100 : Server.DelayTime;
				Server.AssumePriceRate = assumePriceRate;
			}
			private AppInterface appInterface = new AppInterface();

			public int ServerRun()
			{
				LastServerRunTime=Environment.TickCount;
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
				}
				catch (GoodListNoDataException ex)
				{
					Server.ExitAftert($"#:{ex.Message}");
					Program.anyTaskWorking = false;
					return -1;
				}
				try
				{

					#region 暂时关闭
					if (Program.vpsStatus == Program.VpsStatus.Idle || Program.vpsStatus == Program.VpsStatus.WaitConnect)
					{
						Program.anyTaskWorking = false;
						return 0;
					}
					//Program.setting.threadSetting.Status = string.Format("{1}次: {0}", "App接口", runTimeRecord);
					#endregion

				}
				catch (Exception ex)
				{
					Program.Tcp?.Send(new RpClientWaitMessage(0,0, -1));
					Server.ExitAftert($"处理结束后发生异常:{ex.Message}");
					Program.anyTaskWorking = false;
					return 0;
				}
				return hdlGoodNum;
			}

			public List<Server> HdlServer { get => hdlServer; set => hdlServer = value; }
			public int LastServerRunTime { get => lastServerRunTime; set => lastServerRunTime = value; }

			#region IDisposable Support
			private bool disposedValue = false; // 要检测冗余调用

			protected virtual void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if (disposing)
					{
						if (appInterface != null) appInterface.Dispose();
					}
					appInterface = null;
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
