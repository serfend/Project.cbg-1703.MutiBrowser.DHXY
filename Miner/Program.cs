using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Goods;
using Miner.Server;
using Miner.Goods.Summon;
using System.Threading;
using System.Windows.Forms;
using DotNet4.Utilities.UtilCode;

using System.IO;
using File_Transfer;

namespace Miner
{
	class Program
	{
		public static Setting setting;
		public static ServerList servers;
		public static SfTcp.SfTcpClient Tcp;
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.ExceptionObject.ToString());
			Thread.Sleep(5000);
			Environment.Exit(-1); //有此句则不弹异常对话框
		}
		private static Reg rootReg;
		public  enum VpsStatus
		{
			WaitConnect,
			Connecting,
			Running,
			Exception
		}
		public static VpsStatus vpsStatus =0;
		//public static IpConfig IpConfig;
		[STAThreadAttribute]
		static void Main(string[] args)
		{
			try
			{
				rootReg = new Reg("sfMinerDigger");
				AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
				int systemBegin = Environment.TickCount;
				Logger.OnLog += (x, xx) => { Console.WriteLine(xx.LogInfo); };
				
				
				var mainThreadCounter =  rootReg.In("Main").In("Thread").In("Main");
				while (true)
				{
					Thread.Sleep(1000);
					switch (vpsStatus)
					{
						case VpsStatus.WaitConnect:
							{
								InitTcp();
								vpsStatus = VpsStatus.Connecting;
								break;
							}

					}

				}
			}
			catch (Exception ex)
			{
				var info = ex.Message +"\n" + ex.Source + "\n" + ex.StackTrace;
				Program.setting.LogInfo(info,"ExceptionLog");
				Clipboard.SetText(info);
				Thread.Sleep(5000);
			}
		}
		private static void InitTcp()
		{
			var clientId = rootReg.In("Main").In("Setting");
			var vpsName = clientId.GetInfo("VpsClientId", "null");
			var clientDeviceId = clientId.GetInfo("clientDeviceId",HttpUtil.UUID);
			Tcp = new SfTcp.SfTcpClient();
			Tcp.RecieveMessage = (x, xx) =>
			{
				Logger.SysLog(xx, "通讯记录");
				if (xx.Contains("<setClientName>"))
				{
					var ClientName = HttpUtil.GetElementInItem(xx, "setClientName");
					setting = new Setting(ClientName);
					clientId.SetInfo("VpsClientId",ClientName);
					Program.vpsStatus = VpsStatus.Running;
					Tcp.Send("<InitComplete>");
				}
				if (xx.Contains("<serverRun>"))
				{
					ServerRun();
				}
				if (xx.Contains("<setting>"))
				{
					SynSetting(xx);
				}

				if (xx.Contains("<versionCheck>"))
				{
					SynFile(xx);
				}
				
			};
			Tcp.Disconnected = (x) => {
				Program.vpsStatus = VpsStatus.WaitConnect;
				Logger.SysLog("与服务器丢失连接.", "主记录");
			};
			
			Tcp.Send("<connectCmdRequire>" + vpsName + "</connectCmdRequire><clientDeviceId>"+ clientDeviceId+"</clientDeviceId>");
		}
		private static void SynFile(string xx)
		{
			var cstr = new StringBuilder();
			var verFiles = HttpUtil.GetAllElements(xx, "<file>", "</file>");
			foreach (var f in verFiles)
			{
				var fver = HttpUtil.GetElement(f, "<version>", "</version>");
				var fname = HttpUtil.GetElement(f, "<name>", "</name>");
				var localFile = HttpUtil.GetMD5ByMD5CryptoService(fname);
				if (fver!= localFile)
				{
					//检测到版本低则更新
					cstr.AppendLine("<fileRequest>").Append(fname).Append("</fileRequest>");
				};
			}
			if (cstr.Length > 0)
			{
				Logger.SysLog(cstr.ToString(),"主记录");
				Tcp.Send(cstr.ToString());
				var fileEngine = new TransferFileEngine(TcpFiletransfer.TcpTransferEngine.Connections.Connection.EngineModel.AsClient, "1s68948k74.imwork.net",30712);
				fileEngine.Connection.ConnectedToServer += (x,xxx) => {
					if (xxx.Success)
					{
						setting.LogInfo("连接到文件服务器", "主记录");
						fileEngine.ReceiveFile(Environment.CurrentDirectory+"/setting");
					}
					else
					{
						setting.LogInfo("请求文件失败:" + xxx.Info,"主记录");
					}
				};
				fileEngine.Receiver.ReceivingCompletedEvent += (x, xxx) => {
					if (xxx.Result == File_Transfer.Model.ReceiverFiles.ReceiveResult.Completed)
					{
						fileEngine.ReceiveFile(Environment.CurrentDirectory + "/setting");
					}
				};
				fileEngine.Connect();
				
			}
		}
		private  static void ServerRun()
		{
			servers = new ServerList();
			SummomPriceRule.Init();
			Goods.Equiment.EquimentPrice.Init();
			Program.setting.threadSetting.Status = "初始化完成";
			//setting.LogInfo("进程加载完成,等待1000ms", "主记录");
			//while(Environment.TickCount- systemBegin < 1000)
			//{
			//	Thread.Sleep(50);
			//}
			servers.Run();
			Tcp.Send("<clientExit>");
			setting.LogInfo("进程退出", "主记录");
		}
		private static void SynSetting(string raw) {
			var settings = HttpUtil.GetAllElements(raw, "<setting>", "</setting>");
			foreach (var s in settings)
			{
				var key = HttpUtil.GetElement(s, "<key>", "</key>");
				var value = HttpUtil.GetElement(s, "<value>", "</value>");
				var keyPath = key.Split('/');
				var tmpReg = rootReg.In("Main");
				for (int i = 0; i < keyPath.Length - 1; i++)
				{
					tmpReg = tmpReg.In(keyPath[i]);
				}
				tmpReg.SetInfo(keyPath[keyPath.Length - 1], value);
			}
		}
	}
}

