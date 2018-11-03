using DotNet4.Utilities.UtilReg;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Miner
{
	public class Setting
	{
		public ThreadSetting threadSetting;
		public Setting(string processId=null)
		{
			MainReg = new Reg("sfMinerDigger").In("Main");
			Pid = Process.GetCurrentProcess().Id.ToString();
			ProcessCmdId = processId?? "Test";
			Logger.defaultPath = "log\\Thread." + ProcessCmdId;
			threadSetting = new ThreadSetting(MainReg.In("Thread").In(ProcessCmdId))
			{
				CorePid = Pid
			};
			var dataListReg = new Reg("sfMinerDigger").In("Main").In("Data").In("DataList");
			DataListServerReg = dataListReg.In("Server");
			DataListCoreReg = dataListReg.In("Core").In("@"+ ProcessCmdId);
			BrowserServerSeeting =MainReg.In("setting").In("Cmd").In("Server");
			Init = true;
		}
		private bool init;
		private string pid;
		private string processCmdId;

		public Reg MainReg;
		public Reg BrowserServerSeeting;
		public Reg DataListServerReg;
		public Reg DataListCoreReg;
		private string lastInfo;
		public void LogInfo(string info,string CataPath, bool ignoreDuplicate=false)
		{
			if (lastInfo == info && !ignoreDuplicate) return;
			lastInfo = info;
			Logger.SysLog(Name + info, CataPath);
		}
		public string Name
		{
			get=> "Miner"+Pid+"["+ProcessCmdId+"]";
		}
		public bool Init { get => init;private set => init = value; }
		public string Pid { get => pid; private set => pid = value; }
		public string ProcessCmdId { get => processCmdId; private set => processCmdId = value; }

	}
	public class ThreadSetting
	{
		public Reg ThreadReg;
		public ThreadSetting(Reg ThreadReg)
		{
			this.ThreadReg = ThreadReg;
		}
		public bool ThisThreadIsRefresh
		{
			get
			{
				var checkInfo = ThreadReg.GetInfo("Refreshed");
				if (checkInfo == "") return false;
				ThreadReg.SetInfo("Refreshed", "");
				return true;
			}
		}
		public string CorePid
		{
			set => ThreadReg.SetInfo("corePid", value);
		}
		public int DelayTime
		{
			get => Convert.ToInt32( ThreadReg.GetInfo("runInterval", "1500"));
		}
		public string Task
		{
			get => ThreadReg.GetInfo("Task");
		}
		public string Status
		{
			set {
				Program.setting.LogInfo("Status:"+value,"主记录");
				ThreadReg.SetInfo("Status", value.Length>10?value.Substring(0,10): value);
			}
			get => ThreadReg.GetInfo("Status");
		}
		public bool NeedNewIp
		{
			set {
				ThreadReg.SetInfo("needNewIp", "1");
			}
			get => ThreadReg.GetInfo("needNewIp")=="1";
		}
		public string Ip
		{
			set
			{
				ThreadReg.SetInfo("Ip", value);
			}
			get => ThreadReg.GetInfo("Ip");
		}
		[DllImport("kernel32")]
		static  extern uint GetTickCount();
		public void RefreshRunTime()
		{
			ThreadReg.SetInfo("Runtime", GetTickCount());
		}
	}
}