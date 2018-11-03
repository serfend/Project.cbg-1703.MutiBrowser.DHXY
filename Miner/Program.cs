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
				var clientId = rootReg.In("Main").In("Setting");
				var vpsName = clientId.GetInfo("VpsClientId","null");

				Tcp = new SfTcp.SfTcpClient();
				Tcp.RecieveMessage = (x, xx) =>
				{
					if (xx.Contains("<setClientName>"))
					{
						setting = new Setting(HttpUtil.GetElementInItem(xx, "setClientName"));
						clientId.SetInfo("VpsClientId", HttpUtil.GetElementInItem(xx, "clientId"));
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
						var cstr = new StringBuilder();
						var verFiles = HttpUtil.GetAllElements(xx, "<file>", "</file>");
						foreach(var f in verFiles)
						{
							var fver = HttpUtil.GetElement(f, "<version>", "</version>");
							var fname = HttpUtil.GetElement(f, "<name>", "</name>");
							if (Convert.ToInt32(fver) > Convert.ToInt32(rootReg.In("setting").In("fileVersion").GetInfo(fname,"0"))) {
								//检测到版本低则更新
								cstr.Append("<fileRequest>").Append(fname).Append("</fileRequest>");
							};
						}
						if (cstr.Length > 0)
						{
							Tcp.Send(cstr.ToString());
						}
					}
					if (xx.Contains("<fileContent>"))
					{
						var transFile = HttpUtil.GetAllElements(xx,"<fileContent>","</fileContent>");
						foreach(var transFileRaw in transFile)
						{
							using (var transf = File.Create(HttpUtil.GetElementInItem(transFileRaw, "filePath")))
							{
								var data = System.Text.Encoding.Default.GetBytes(HttpUtil.GetElementInItem(transFileRaw,"content"));
								transf.Write(data, 0, data.Length);
							}
						}

					}
				};
				Tcp.Send("<connectCmdRequire>"+ vpsName + "</connectCmdRequire>");
				var mainThreadCounter =  rootReg.In("Main").In("Thread").In("Main");
				while (true)
				{
					Thread.Sleep(1000);
					var nowCount = Environment.TickCount;
					try
					{
						var mainThreadCount = Convert.ToInt32(mainThreadCounter.GetInfo("LastRunTime", "0"));
						//if (Math.Abs(nowCount - mainThreadCount) > 10000) Environment.Exit(0);
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message + "  LastRunTime:" + mainThreadCounter.GetInfo("LastRunTime", "0"));
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

