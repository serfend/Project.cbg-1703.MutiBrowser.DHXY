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
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using Miner.ServerHandle;

namespace Miner
{
	class Program
	{
		public static Setting setting;
		public static ServerList servers;
		public static SfTcp.SfTcpClient Tcp;
		private static void StartNewProgram()
		{
			new Thread(() =>
			{
				Thread.Sleep(3000);
				Process.Start(Process.GetCurrentProcess().MainModule.FileName);
			}).Start();
		}
		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			StartNewProgram();
			new Task(()=> { MessageBox.Show("UnhandledException"+e.ExceptionObject.ToString()); }).Start();
			Thread.Sleep(5000);
			Environment.Exit(-1); //有此句则不弹异常对话框
		}
		private static Reg rootReg;
		public  enum VpsStatus
		{
			WaitConnect,
			Connecting,
			Syning,
			Running,
			Idle,
			Exception
		}
		public static VpsStatus vpsStatus =0;
		private static int disconnectTime=10;
		private static int idleTime = 30;
		private static int connectFailTime = 0;

		public static string TcpMainTubeIp= "2y155s0805.51mypc.cn";
		public static int TcpMainTubePort= 40899;
		public static string TcpFileTubeIp= "2y155s0805.51mypc.cn";
		public static int TcpFileTubePort= 15514;
		[STAThreadAttribute]
		static void Main(string[] args)
		{
			var fileName = Process.GetCurrentProcess().MainModule.FileName;
			var targetIp = HttpUtil.GetElement(fileName,"(", ")");
			if (targetIp !=null)
			{
				var tmpInfo = targetIp.Split('!');
				if (tmpInfo.Length == 4)
				{
					TcpMainTubeIp = tmpInfo[0];
					TcpMainTubePort = Convert.ToInt32(tmpInfo[1]);
					TcpFileTubeIp = tmpInfo[2];
					TcpFileTubePort = Convert.ToInt32(tmpInfo[3]);
				}
			}
				
			rootReg = new Reg("sfMinerDigger");
			clientId = rootReg.In("Main").In("Setting");
			
			int systemBegin = Environment.TickCount;
			Logger.OnLog += (x, xx) => { Console.WriteLine(xx.LogInfo); };
			if(rootReg.In("Setting").GetInfo("developeModel")=="1")
				Logger.IsOnDevelopeModel = true;
			
			//AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			//{
			//	Program.setting = new Setting("test");
			//	ServerRun();
			//}
			try
			{
				var mainThreadCounter =  rootReg.In("Main").In("Thread").In("Main");
				while (true)
				{
					Thread.Sleep(1000);
					switch (vpsStatus)
					{
						case VpsStatus.WaitConnect:
							{
								if (disconnectTime++ > 10 && anyTaskWorking == false)
								{
									if (connectFailTime++ > 2)
									{
										connectFailTime = 0;
										RedialToInternet();
										Program.setting.LogInfo("连接到服务器失败次数达上限,重新拨号", "通讯记录");

									}
									else {
										vpsStatus = VpsStatus.Connecting;
										InitTcp();
									};//尝试连接次数过多，则重连宽带
									
									disconnectTime = 0;
								}

								break;
							}
						case VpsStatus.Idle:
							{
								if (idleTime-- < 0 && anyTaskWorking == false)
								{
									HelloToServer();
									idleTime = 30;
								}
								break;
							}
						case VpsStatus.Running:
						case VpsStatus.Syning:
							{
								
								if (Tcp==null)
								{
									vpsStatus = VpsStatus.WaitConnect;
								}
								else
								{
									connectFailTime =0;
								}
								break;
							}


					}
				}
			}
			catch (Exception ex)
			{
				var info = ex.Message +"\n" + ex.Source + "\n" + ex.StackTrace;
				Logger.SysLog(info,"ExceptionLog");
				Clipboard.SetText(info);
				Thread.Sleep(5000);
			}
		}
		private static Reg clientId;
		private static void InitTcp()
		{
			
			if (Tcp != null)
			{
				Tcp.Dispose();
				Tcp = null;
				Thread.Sleep(1000);//等待资源释放
			}
			try
			{
				Tcp = new SfTcp.SfTcpClient(TcpMainTubeIp, TcpMainTubePort);
			}
			catch (Exception ex)
			{
				Logger.SysLog("建立连接失败 " + ex.Message,"ExceptionLog");
				vpsStatus = VpsStatus.WaitConnect;
				return;
			}
			InitCallBackTcp();
			HelloToServer();
		}
		private static void HdlRecieveMessage(SfTcp.SfTcpClient x,string xx)
		{
			try
			{
				Logger.SysLog(xx, "通讯记录");
				if (xx.Contains("<setClientName>"))
				{
					var ClientName = HttpUtil.GetElementInItem(xx, "setClientName");
					setting = new Setting(ClientName);
					clientId.SetInfo("VpsClientId", ClientName);
					Tcp.Send("nameModefied", string.Format("<clientName>{0}</clientName><AskForSynInit>", ClientName));
				}
				if (xx.Contains("SynInit"))
				{
					InitSetting(xx);
					Program.vpsStatus = VpsStatus.Syning;
					Tcp.Send("InitComplete", "");
				}
				if (xx.Contains("<serverRun>"))
				{
					ServerResetConfig();
				}
				if (xx.Contains("<setting>"))
				{
					SynSetting(xx);
				}
				if (xx.Contains("<versionCheck>"))
				{
					SynFile(xx);
				}
				if (xx.Contains("<ensureFileTransfer>"))
				{//客户端接收到来自服务器【可以开始传输】的指令
					TranslateFileStart();
				}
				if (xx.Contains("<InnerTargetUrl>"))
				{
					InnerTargetUrl = HttpUtil.GetElementInItem(xx, "InnerTargetUrl");
				}
				if (xx.Contains("<reRasdial>"))
				{
					Tcp.Send("reRasdial", "");
					RedialToInternet();
				}
				if (xx.Contains("<startNew>"))
				{
					StartNewProgram();
				}
				if (xx.Contains("<SubClose>"))
				{
					Environment.Exit(0);
				}
				if (xx.Contains("<SynServerLoginSession>"))
				{
					SynServerLoginSession(xx);
				}
				if (xx.Contains("<newScheduleServerRun>"))
				{
					vpsStatus = VpsStatus.Running;
					//TODO 此处实现时统无效，待以后修正
					//var sendStamp =Convert.ToInt64( HttpUtil.GetElementInItem(xx,"sendStamp"));
					//var sendStampStruct =SystemTimeWin32.FromStamp(sendStamp);
					//var result = SystemTimeWin32.SetSystemTime(ref sendStampStruct);
					var s=new Thread(() =>
					{
						var nextRuntimeStamp = Convert.ToInt64(HttpUtil.GetElementInItem(xx, "taskStamp"));
						//var tickCount = HttpUtil.TimeStamp;
						//Console.WriteLine(tickCount);
						var beginTime = Environment.TickCount;
						if (nextRuntimeStamp > 0)
						{
							while (beginTime - Environment.TickCount + nextRuntimeStamp >= 500)
							{
								long interval = beginTime - Environment.TickCount + nextRuntimeStamp;
								Tcp.Send("clientWait", interval.ToString());
								Thread.Sleep(500);
							}
							long leftInterval = beginTime - Environment.TickCount + nextRuntimeStamp;
							if (leftInterval > 0) Thread.Sleep((int)leftInterval);
						}
						Tcp.Send("clientWait", "-101");
						servers.ServerRun();
					})
					{ IsBackground=true};
					s.Start();
				}
			}
			catch (Exception ex)
			{
				Logger.SysLog("HdlReceiveMessage()" + ex.Message,"ExceptionLog");
			}
		}
		private static void InitCallBackTcp()
		{
			try
			{
				if (Tcp == null) return;
				Tcp.RecieveMessage = HdlRecieveMessage;
				Tcp.Disconnected = (x) => {
					Logger.SysLog("与服务器丢失连接", "主记录");
					Tcp?.Dispose();
					Tcp = null;
					anyTaskWorking = false;
					Program.vpsStatus = VpsStatus.WaitConnect;
				};
			}
			catch (Exception ex)
			{
				Console.WriteLine("InitCallBackTcp()"+ex.Message);
			}
		}
		private static void SynServerLoginSession(string setting)
		{
			var modefyServers = HttpUtil.GetAllElements(setting, "<Server>", "</Server>");
			foreach(var mdServer in modefyServers)
			{
				var name = HttpUtil.GetElementInItem(mdServer, "name");
				var loginSession = HttpUtil.GetElementInItem(mdServer, "login");
				foreach(var server in servers.HdlServer)
				{
					if (server.ServerName == name)
					{
						server.LoginSession = loginSession;
						Program.setting.LogInfo($"服务器登录凭证更新:{server.ServerName}->{loginSession}");
					}
				}
			}
		}
		public static void RedialToInternet()
		{
			Program.Tcp?.Dispose();
			var p = new CmdRasdial();
			p.DisRasdial();
			var t = new Task(() => {
				Thread.Sleep(1000);
				p.Rasdial();
				Program.vpsStatus = VpsStatus.WaitConnect;
			});
			t.Start();
		}

		private static void HelloToServer()
		{
			try
			{

				var vpsName = clientId.GetInfo("VpsClientId", "null");
				var clientDeviceId = clientId.GetInfo("clientDeviceId", HttpUtil.UUID);
				clientId.SetInfo("clientDeviceId", clientDeviceId);
				Tcp?.Send("clientConnect", $"<clientName>{vpsName}</clientName><clientDeviceId>{clientDeviceId}</clientDeviceId><version>{Assembly.GetExecutingAssembly().GetName().Version}</version>");
			}
			catch (Exception ex)
			{
				Console.WriteLine("HelloToServer()"+ex.Message);
			}
		}
		private static void TranslateFileStart()
		{
			Logger.SysLog("准备接收文件", "主记录");
			var fileEngine = new TransferFileEngine(TcpFiletransfer.TcpTransferEngine.Connections.Connection.EngineModel.AsClient,TcpFileTubeIp, TcpFileTubePort);
			fileEngine.Connection.ConnectedToServer += (xs, xxx) => {
				if (xxx.Success)
				{
					setting.LogInfo("连接到文件服务器,准备开始接收文件", "主记录");
					fileEngine.ReceiveFile(Environment.CurrentDirectory + "/setting");
				}
				else
				{
					setting.LogInfo("请求文件失败/结束:" + xxx.Info, "主记录");
				}
			};
			fileEngine.Receiver.ReceivingCompletedEvent += (xs, xxx) => {
				if (xxx.Result == File_Transfer.Model.ReceiverFiles.ReceiveResult.Completed)
				{
					setting.LogInfo("成功接收文件:" + xxx.Message + "(" + ++fileNowReceive + "/" + fileWaitToUpdate + ")");
					if (fileNowReceive >= fileWaitToUpdate)
					{
						setting.LogInfo("文件已同步完成");
						ServerResetConfig();
						return;
					}
					fileEngine.ReceiveFile(Environment.CurrentDirectory + "/setting");
				}
				else
				{
					setting.LogInfo(xxx.Title + ":" + xxx.Message);
				}
			};
			fileEngine.Connect();
		}
		private static int fileWaitToUpdate = 0,fileNowReceive=0;

		public static string InnerTargetUrl { get; internal set; }
		private static string settingTaskInfo;
		private static int settingDelayTime;
		private static double settingAssumePriceRate;
		private static void InitSetting(string settingInfo)
		{
			settingTaskInfo = HttpUtil.GetElementInItem(settingInfo, "task");
			settingDelayTime = Convert.ToInt32(HttpUtil.GetElementInItem(settingInfo, "interval"));
			settingAssumePriceRate = Convert.ToDouble(HttpUtil.GetElementInItem(settingInfo, "assumePriceRate"));
		}
		private static void SynFile(string xx)
		{
			Logger.SysLog("尝试同步设置", "主记录");
			var cstr = new StringBuilder();
			var verFiles = HttpUtil.GetAllElements(xx, "<file>", "</file>");
			fileWaitToUpdate = fileNowReceive = 0;
			foreach (var f in verFiles)
			{
				var fver = HttpUtil.GetElement(f, "<version>", "</version>");
				var fname = HttpUtil.GetElement(f, "<name>", "</name>");
				var localFile = HttpUtil.GetMD5ByMD5CryptoService("setting/"+fname);
				if (fver!= localFile)
				{
					fileWaitToUpdate++;
					//检测到hash不相同则更新
					cstr.Append("<fileRequest>").Append(fname).AppendLine("</fileRequest>");
				};
			}
			if (cstr.Length > 0)
			{
				Logger.SysLog(cstr.ToString(), "主记录");
				Tcp.Send("RequireFile",cstr.ToString());
			}
			else
			{
				ServerResetConfig();
			}
		}
		public static bool anyTaskWorking = false;
		private  static void ServerResetConfig()
		{
			vpsStatus = VpsStatus.Running;
			anyTaskWorking = true;
			servers = new ServerList();
			SummomPriceRule.Init();
			Goods.Equiment.EquimentPrice.Init();
			servers.ResetConfig(settingTaskInfo,settingDelayTime,settingAssumePriceRate);
			Tcp.Send("clientConfigComplete", "");
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

