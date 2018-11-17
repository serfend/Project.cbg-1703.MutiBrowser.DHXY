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
			Syning,
			Running,
			Idle,
			Exception
		}
		public static VpsStatus vpsStatus =0;
		private static int disconnectTime=10;

		public static string TcpMainTubeIp= "1s68948k74.imwork.net";
		public static int TcpMainTubePort= 16397;
		public static string TcpFileTubeIp= "1s68948k74.imwork.net";
		public static int TcpFileTubePort=30712;
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
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			int systemBegin = Environment.TickCount;
			Logger.OnLog += (x, xx) => { Console.WriteLine(xx.LogInfo); };
			if(rootReg.In("Setting").GetInfo("developeModel")=="1")
				Logger.IsOnDevelopeModel = true;
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
								if (disconnectTime++ > 10 && anyTaskWorking==false)
								{
									vpsStatus = VpsStatus.Connecting;
									InitTcp();
									disconnectTime = 0;
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
			var clientId = rootReg.In("Main").In("Setting");
			var vpsName = clientId.GetInfo("VpsClientId", "null");
			var clientDeviceId = clientId.GetInfo("clientDeviceId",HttpUtil.UUID);
			clientId.SetInfo("clientDeviceId", clientDeviceId);
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
			Tcp.RecieveMessage = (x, xx) =>{
				Logger.SysLog(xx, "通讯记录");
				if (xx.Contains("<setClientName>"))
				{
					var ClientName = HttpUtil.GetElementInItem(xx, "setClientName");
					setting = new Setting(ClientName);
					clientId.SetInfo("VpsClientId",ClientName);
					Tcp.Send("nameModefied", string.Format("<clientName>{0}</clientName><AskForSynInit>", ClientName));
				}
				if (xx.Contains("SynInit"))
				{
					InitSetting(xx);
					Program.vpsStatus = VpsStatus.Syning;
					Tcp.Send("InitComplete", "");
				}
				if (xx.Contains("<serverRun>")){
					ServerRun();
				}
				if (xx.Contains("<setting>"))
				{
					SynSetting(xx);
				}
				if (xx.Contains("<versionCheck>")){
					SynFile(xx);
				}
				if (xx.Contains("<ensureFileTransfer>")) {//客户端接收到来自服务器【可以开始传输】的指令
					TranslateFileStart();
				}
				if (xx.Contains("<InnerTargetUrl>"))
				{
					InnerTargetUrl = HttpUtil.GetElementInItem(xx, "InnerTargetUrl");
				}
				if (xx.Contains("<reRasdial>"))
				{
					Tcp.Send("reRasdial", "");
					var p = new CmdRasdial();
					p.DisRasdial();
					p.Rasdial();
					Program.vpsStatus = VpsStatus.WaitConnect;
				}
			};
			Tcp.Disconnected = (x) => {
				Logger.SysLog("与服务器丢失连接", "主记录");
				Program.vpsStatus = VpsStatus.WaitConnect;
			};
			Tcp.Send("clientConnect","<connectCmdRequire>" + vpsName + "</connectCmdRequire><clientDeviceId>"+ clientDeviceId+"</clientDeviceId>");
		}
		private static void TranslateFileStart()
		{
			Logger.SysLog("准备接收文件", "主记录");
			var fileEngine = new TransferFileEngine(TcpFiletransfer.TcpTransferEngine.Connections.Connection.EngineModel.AsClient, "1s68948k74.imwork.net", 30712);
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
					setting.LogInfo("成功接收文件:" + xxx.Message + "(" + fileNowReceive++ + "/" + fileWaitToUpdate + ")");
					if (fileNowReceive >= fileWaitToUpdate)
					{
						setting.LogInfo("文件已同步完成");
						ServerRun();
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
		private static void InitSetting(string settingInfo)
		{
			settingTaskInfo = HttpUtil.GetElementInItem(settingInfo, "task");
			settingDelayTime = Convert.ToInt32(HttpUtil.GetElementInItem(settingInfo, "interval"));
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
				ServerRun();
			}
		}
		public static bool anyTaskWorking = false;
		private  static void ServerRun()
		{
			vpsStatus = VpsStatus.Running;
			anyTaskWorking = true;
			servers = new ServerList();
			SummomPriceRule.Init();
			Goods.Equiment.EquimentPrice.Init();
			Program.setting.threadSetting.Status = "初始化完成";
			servers.Run(settingTaskInfo,settingDelayTime);
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

