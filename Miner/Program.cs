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
using SfTcp.TcpClient;
using SfTcp.TcpMessage;
using TcpFiletransfer.TcpTransferEngine.Connections;

namespace Miner
{
	class Program
	{
		public static Setting setting;
		public static ServerList servers;
		public static SfTcp.TcpClient.TcpClient Tcp;
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

		public static string TcpMainTubeIp= "127.0.0.1";
		public static int TcpMainTubePort= 8009;
		public static string TcpFileTubeIp= "127.0.0.1";
		public static int TcpFileTubePort= 8010;
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
								if (disconnectTime++ > 5 && anyTaskWorking == false)
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
			Console.WriteLine("重置通信节点");
			if (Tcp != null)
			{
				Tcp?.Dispose();
				Tcp = null;
				Thread.Sleep(1000);//等待资源释放
			}
			try
			{
				Tcp = new TcpClient(TcpMainTubeIp, TcpMainTubePort);
			}
			catch (Exception ex)
			{
				Logger.SysLog("建立连接失败 " + ex.Message,"ExceptionLog");
				vpsStatus = VpsStatus.WaitConnect;
				return;
			}
			InitCallBackTcp();
		}
		private static void HdlRecieveMessage(TcpClient x, ServerMessageEventArgs e)
		{
			Logger.SysLog(e.RawString, "通讯记录");
			switch (e.Title)
			{
				case TcpMessageEnum.MsgHeartBeat:
					Console.WriteLine("服务器保持连接确认");
					break;
				case TcpMessageEnum.CmdSetClientName:
					var ClientName = e.Message["NewName"].ToString();
					setting = new Setting(ClientName);
					clientId.SetInfo("VpsClientId", ClientName);
					Tcp.Send(new RpNameModefiedMessage(ClientName, true));
					break;
				case TcpMessageEnum.CmdSynInit:
					InitSetting(Convert.ToInt32(e.Message["Interval"].ToString()), Convert.ToDouble(e.Message["AssumePriceRate"].ToString()));
					Program.vpsStatus = VpsStatus.Syning;
					Tcp.Send(new RpInitCompletedMessage());
					break;
				case TcpMessageEnum.CmdServerRun:
					ServerResetConfig();
					break;
				case TcpMessageEnum.MsgSynFileList:
					var synFileList = new MsgSynFileListMessage((List<SynSingleFile>)e.Message["List"]);
					SynFile(synFileList);
					break;
				case TcpMessageEnum.CmdTransferFile://客户端接收到来自服务器【可以开始传输】的指令
					TranslateFileStart();
					break;
				case TcpMessageEnum.CmdModefyTargetUrl:
					InnerTargetUrl = e.Message["NewUrl"].ToString();
					break;
				case TcpMessageEnum.CmdReRasdial:
					Tcp.Send(new RpReRasdialMessage());
					RedialToInternet();
					break;
				case TcpMessageEnum.CmdStartNewProgram:
					StartNewProgram();
					break;
				case TcpMessageEnum.CmdSubClose:
					Environment.Exit(0);
					break;
				case TcpMessageEnum.MsgSynSession:
					var synLoginItemList = (List<SynSessionItem>)e.Message["List"];
					var synLoginSession = new MsgSynSessionMessage(synLoginItemList);
					SynServerLoginSession(synLoginSession);
					break;
				case TcpMessageEnum.CmdServerRunSchedule:
					{
						vpsStatus = VpsStatus.Running;
						if (vpsIsDigging)
						{
							Console.WriteLine("警告,同时出现多个采集实例");
							return;
						}
						vpsIsDigging = true;
							//TODO 此处实现时统无效，待以后修正
							//var sendStamp =Convert.ToInt64( HttpUtil.GetElementInItem(xx,"sendStamp"));
							//var sendStampStruct =SystemTimeWin32.FromStamp(sendStamp);
							//var result = SystemTimeWin32.SetSystemTime(ref sendStampStruct);
							var s = new Thread(() =>
						{
							try
							{
								Tcp.Send(new RpClientWaitMessage(0,0,101));//开始等待
								var nextRuntimeStamp = Convert.ToInt32(e.Message["TaskStamp"].ToString());
								//var tickCount = HttpUtil.TimeStamp;
								//Console.WriteLine(tickCount);
								Thread.Sleep(nextRuntimeStamp);
								Tcp.Send(new RpClientWaitMessage(0,0,-101));//结束等待
								if (servers == null)
								{
									Console.WriteLine("servers未初始化");
									return;
								}
								int lastRunTime = Environment.TickCount;
								int hdlGoodNum=servers.ServerRun();
								int interval = Environment.TickCount - lastRunTime;
								var avgInterval = Program.setting.threadSetting.RefreshRunTime(interval);
								//TODO 此处估价似乎也有延迟
								Program.Tcp?.Send(new RpClientWaitMessage(avgInterval, hdlGoodNum, 0));
							}
							catch (Exception ex)
							{
								Console.WriteLine($"处理日程失败;{ex.Message}");
							}
							finally
							{
								vpsIsDigging = false;
							}
						})
						{ IsBackground = true };
						s.Start();
						break;
					}
			}
		}
		private static bool vpsIsDigging = false;
		private static void InitCallBackTcp()
		{
			try
			{
				if (Tcp == null) return;
				Tcp.OnMessage += Tcp_OnMessage; 
				Tcp.OnDisconnected += Tcp_OnDisconnected;
				Tcp.OnConnected += Tcp_OnConnected;

				Tcp.Client.Connect();
			}
			catch (Exception ex)
			{
				Console.WriteLine("InitCallBackTcp()"+ex.Message);
			}
		}

		private static void Tcp_OnConnected(object sender, ServerConnectEventArgs e)
		{
			HelloToServer();
		}

		private static void Tcp_OnDisconnected(object sender, ServerDisconnectEventArgs e)
		{
			Logger.SysLog("与服务器丢失连接", "主记录");
			Tcp?.Dispose();
			Tcp = null;
			anyTaskWorking = false;
			Program.vpsStatus = VpsStatus.WaitConnect;
		}

		private static void Tcp_OnMessage(object sender, ServerMessageEventArgs e)
		{
			HdlRecieveMessage(sender as TcpClient, e);
		}

		private static void SynServerLoginSession(MsgSynSessionMessage setting)
		{
			foreach(var mdServer in setting.List)
			{
				foreach(var server in servers.HdlServer)
				{
					if (server.ServerName == mdServer.AliasName)
					{
						server.LoginSession = mdServer.LoginSession;
						Program.setting.LogInfo($"服务器登录凭证更新:{server.ServerName}->{mdServer.LoginSession}");
					}
				}
			}
		}
		public static void RedialToInternet()
		{
			//Program.Tcp?.Dispose();
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
				Tcp?.Send(new RpClientConnectMessage("vps", Assembly.GetExecutingAssembly().GetName().Version.ToString(), clientDeviceId, vpsName));
			}
			catch (Exception ex)
			{
				Console.WriteLine("HelloToServer()"+ex.Message);
			}
		}
		private static void TranslateFileStart()
		{
			Logger.SysLog("准备接收文件", "主记录");
			var fileEngine = new TransferFileEngine(Connection.EngineModel.AsClient,TcpFileTubeIp, TcpFileTubePort);
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
				if (xxx.Result == File_Transfer.ReceiverFiles.ReceiveResult.Completed)
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
		public static object TcpFiletransfer { get; private set; }

		private static int settingDelayTime;
		private static double settingAssumePriceRate;
		private static void InitSetting(int interval,double assumePriceRate)
		{
			settingDelayTime = interval;
			settingAssumePriceRate = assumePriceRate;
		}
		private static void SynFile(MsgSynFileListMessage fileList)
		{
			Logger.SysLog("尝试同步设置", "主记录");
			var requestFileList = new List<SynSingleFile>();
			fileWaitToUpdate = fileNowReceive = 0;
			foreach (var f in fileList.List)
			{
				var localFile = HttpUtil.GetMD5ByMD5CryptoService("setting/"+f.Name);
				if (f.Version!= localFile)
				{
					fileWaitToUpdate++;
					//检测到hash不相同则更新
					requestFileList.Add(new SynSingleFile() {
						Name=f.Name
					});
				};
			}
			if (requestFileList.Count > 0)
			{
				StringBuilder logInfo = new StringBuilder();
				requestFileList.ForEach((x) => logInfo.Append('\n').Append(x.Name));
				Logger.SysLog(logInfo.ToString(), "主记录");
				Tcp.Send(new MsgSynFileListMessage(requestFileList) { Title= "RequireFile" });
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
			servers.ResetConfig(settingDelayTime,settingAssumePriceRate);
			Tcp.Send(new RpClientRunReadyMessage());
		}
	}
}

