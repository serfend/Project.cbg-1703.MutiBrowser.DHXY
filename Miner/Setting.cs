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
			LogInfo("初始化进程");
			MainReg = new Reg("sfMinerDigger").In("Main");
			Pid = Process.GetCurrentProcess().Id.ToString();
			ProcessCmdId = processId?? "Test";
			Logger.defaultPath = "Thread." + ProcessCmdId;
			threadSetting = new ThreadSetting(MainReg.In("Thread").In(ProcessCmdId))
			{
				CorePid = Pid
			};
			Init = true;
		}
		private bool init;
		private string pid;
		private string processCmdId;

		public Reg MainReg;
		private string lastInfo;
		public void LogInfo(string info)
		{
			if (lastInfo == info) return;
			lastInfo = info;
			Logger.SysLog(Name + info);
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
		[DllImport("kernel32")]
		static extern uint GetTickCount();
		public void RefreshRunTime()
		{
			ThreadReg.SetInfo("Runtime", GetTickCount());
		}
	}
}