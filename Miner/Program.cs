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
using Newtonsoft.Json.Linq;

namespace Miner
{
	class Program
	{
		public static Setting setting;
		public static ServerList servers;
		public static TcpClient Tcp;
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
		public static Reg rootReg;
		public static Reg clientId;
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
			
			
			Logger.OnLog += (x, xx) => { Console.WriteLine(xx.LogInfo); };
			if(rootReg.In("Setting").GetInfo("developeModel")=="1")
				Logger.IsOnDevelopeModel = true;
			
			try
			{
				var mainThreadCounter =  rootReg.In("Main").In("Thread").In("Main");
				while (true)
				{
					Thread.Sleep(1000);
					if(VpsStatus.Running!=vpsStatus)
						Console.WriteLine(vpsStatus);
					switch (vpsStatus)
					{
						case VpsStatus.Connecting:
							{
								if (disconnectTime++ > 5 && anyTaskWorking==false)
								{
									disconnectTime = 0;
									vpsStatus = VpsStatus.WaitConnect;
								}
								break;
							}
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
										disconnectTime = 0;
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
							{
								if(Environment.TickCount- servers.LastServerRunTime > 10000 )
								{
									vpsIsDigging = false;
									ServerBeginRun(0);
								}
								break;
							}
						case VpsStatus.Syning:
							{
								
								if (Tcp==null)
								{
									vpsStatus = VpsStatus.WaitConnect;
								}
								else
								{
									if (disconnectTime++ > 15 && anyTaskWorking == false)
									{
										disconnectTime = 0;
										vpsStatus = VpsStatus.WaitConnect;
									}
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
		private static bool vpsIsDigging = false;
		private static void InitCallBackTcp()
		{
			try
			{
				if (Tcp == null) return;
				Tcp.OnMessage += Tcp_OnMessage; 
				Tcp.OnDisconnected += Tcp_OnDisconnected;
				Tcp.OnConnected += Tcp_OnConnected;
				MinerCallBackInit();
				Tcp.Client.Connect();
			}
			catch (Exception ex)
			{
				Console.WriteLine("InitCallBackTcp()"+ex.Message);
				vpsStatus = VpsStatus.WaitConnect;
			}
		}

		private static void Tcp_OnConnected(object sender, ServerConnectEventArgs e)
		{
			vpsStatus = VpsStatus.Syning;
			Thread.Sleep(1000);
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

		private static void Tcp_OnMessage(object sender, ClientMessageEventArgs e)
		{
			//Logger.SysLog(e.RawString, "通讯记录");
			try
			{
				Console.WriteLine($"来自服务器:{e.RawString}");
				MinerCallBack.Exec(e);
			}
			catch (ActionNotRegException ex)
			{
				Console.WriteLine($"读取数据发生异常:{ex.Message}\n{e.RawString}");
			}
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
			
			var t = new Task(() => {
				Thread.Sleep(1000);
				var p = new CmdRasdial();
				p.DisRasdial();
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
					fileEngine.ReceiveFile(Environment.CurrentDirectory + "\\setting");
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
			fileEngine.Receiver.ReceivingStartedEvent += (xs, xxx) =>
			{
				setting.LogInfo($"开始传输文件:{xxx.FileName}");
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
				Tcp.Send(new MsgSynFileListMessage(requestFileList));
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

		
		#region MinerCallBack	
		private static void MinerCallBackInit()
		{
			MinerCallBack.Init();
			MinerCallBack.RegCallback(TcpMessageEnum.MsgHeartBeat,MinerCallBack_MsgHeartBeat);
			MinerCallBack.RegCallback(TcpMessageEnum.CmdSetClientName, MinerCallBack_CmdSetClientName);
			MinerCallBack.RegCallback(TcpMessageEnum.CmdSynInit, MinerCallBack_CmdSynInit);
			MinerCallBack.RegCallback(TcpMessageEnum.CmdServerRun, MinerCallBack_CmdServerRun);
			MinerCallBack.RegCallback(TcpMessageEnum.MsgSynFileList, MinerCallBack_MsgSynFileList);
			MinerCallBack.RegCallback(TcpMessageEnum.CmdTransferFile, MinerCallBack_CmdTransferFile);
			MinerCallBack.RegCallback(TcpMessageEnum.CmdModefyTargetUrl, MinerCallBack_CmdModefyTargetUrl);
			MinerCallBack.RegCallback(TcpMessageEnum.CmdReRasdial, MinerCallBack_CmdReRasdial);
			MinerCallBack.RegCallback(TcpMessageEnum.CmdStartNewProgram, MinerCallBack_CmdStartNewProgram);
			MinerCallBack.RegCallback(TcpMessageEnum.CmdSubClose, MinerCallBack_CmdSubClose);
			MinerCallBack.RegCallback(TcpMessageEnum.MsgSynSession, MinerCallBack_MsgSynSession);
			MinerCallBack.RegCallback(TcpMessageEnum.CmdServerRunSchedule, MinerCallBack_CmdServerRunSchedule);

		}
		private static void MinerCallBack_MsgHeartBeat(ClientMessageEventArgs e)
		{
			Console.WriteLine("服务器保持连接确认");
		}

		private static void MinerCallBack_CmdSetClientName(ClientMessageEventArgs e)
		{
			var ClientName = e.Message["NewName"]?.ToString();
			setting = new Setting(ClientName);
			clientId.SetInfo("VpsClientId", ClientName);
			Tcp.Send(new RpNameModefiedMessage(ClientName, true));
		}

		private static void MinerCallBack_CmdSynInit(ClientMessageEventArgs e)
		{
			var interval = e.Message["Interval"];
			var assumePriceRate = e.Message["AssumePriceRate"];
			if (interval == null || assumePriceRate == null)
			{
				Tcp.Send(new RpMsgInvalidMessage("synInit"));
				return;
			}
			InitSetting(Convert.ToInt32(interval), Convert.ToDouble(assumePriceRate));
			Program.vpsStatus = VpsStatus.Syning;
			Tcp.Send(new RpInitCompletedMessage());
		}
		private static void MinerCallBack_CmdServerRun(ClientMessageEventArgs e)
		{
			ServerResetConfig();
		}
		private static void MinerCallBack_MsgSynFileList(ClientMessageEventArgs e)
		{
			var rawList = e.Message["List"];
			var list = new List<SynSingleFile>();

			foreach (var item in rawList)
			{
				list.Add(new SynSingleFile() {
					Name= item["Name"]?.ToString(),
					Version= item["Version"]?.ToString()
				});
			}
			
			var synFileList = new MsgSynFileListMessage(list);
			SynFile(synFileList);
		}
		private static void MinerCallBack_CmdTransferFile(ClientMessageEventArgs e)
		{
			TranslateFileStart();
		}
		private static void MinerCallBack_CmdModefyTargetUrl(ClientMessageEventArgs e)
		{
			InnerTargetUrl = e.Message["NewUrl"]?.ToString();
		}
		private static void MinerCallBack_CmdReRasdial(ClientMessageEventArgs e)
		{
			Tcp.Send(new RpReRasdialMessage("cmd"));
			RedialToInternet();
		}
		private static void MinerCallBack_CmdStartNewProgram(ClientMessageEventArgs e)
		{
			StartNewProgram();
		}
		private static void MinerCallBack_CmdSubClose(ClientMessageEventArgs e)
		{
			Environment.Exit(0);
		}
		private static void MinerCallBack_MsgSynSession(ClientMessageEventArgs e)
		{
			var synLoginItemList = new List<SynSessionItem>();
			var synLoginSession = new MsgSynSessionMessage(synLoginItemList);
			SynServerLoginSession(synLoginSession);
		}
		private static void MinerCallBack_CmdServerRunSchedule(ClientMessageEventArgs e)
		{
			
			var s = new Thread(() =>
			{
				var nextRuntimeStamp = Convert.ToInt32(e.Message["TaskStamp"]?.ToString());
				ServerBeginRun(nextRuntimeStamp);
			})
			{ IsBackground = true };
			s.Start();
		}

		private static void ServerBeginRun(int nextRuntimeStamp)
		{
			vpsStatus = VpsStatus.Running;
			if (vpsIsDigging)
			{
				Console.WriteLine("警告,同时出现多个采集实例");
				return;
			}
			vpsIsDigging = true;
			try
			{
				//if (nextRuntimeStamp > 499) Tcp.Send(new RpClientWaitMessage(0, 0, 101));//开始等待
				Thread.Sleep(nextRuntimeStamp);
				//if (nextRuntimeStamp > 499) Tcp.Send(new RpClientWaitMessage(0, 0, -101));//结束等待
				//	if (servers == null)
				//	{
				//		Console.WriteLine("servers未初始化");
				//		return;
				//	}
				var ticker = new Win32.HiperTicker();
				ticker.Record();
				//Thread.Sleep(60);
				int hdlGoodNum = servers.ServerRun();
				var avgInterval = (int)(ticker.Duration / 1000);// Program.setting.threadSetting.RefreshRunTime((int)(ticker.Duration / 1000));
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
		}

		#endregion
	}
}

