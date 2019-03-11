using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilReg;
using SfTcp.TcpMessage;
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
			ProcessCmdId = processId?? "Test";
			Logger.defaultPath = "log\\Thread." + ProcessCmdId;
			threadSetting = new ThreadSetting()
			{
				
			};
			Init = true;
		}
		private bool init;
		private string processCmdId;


		private string lastInfo;
		public void LogInfo(string info,string CataPath="主记录", bool ignoreDuplicate=false)
		{
			if (lastInfo == info && !ignoreDuplicate) return;
			lastInfo = info;
			Logger.SysLog(Name + info, CataPath);
		}
		public string Name
		{
			get=> "Miner"+"["+ProcessCmdId+"]";
		}
		public bool Init { get => init;private set => init = value; }
		public string ProcessCmdId { get => processCmdId; private set => processCmdId = value; }

	}
	public class ThreadSetting
	{


		public string Status
		{
			set {
				Program.setting.LogInfo("Status:"+value,"主记录");
				var contentValue = value.Length > 10 ? value.Substring(0, 10) : value;
				Program.Tcp?.Send(new StatusMessage(contentValue));
			}
		}
		private int delaySumCount=0;
		private int totalDelayTime = 0;
		public int RefreshRunTime(int interval)
		{
			if (interval == 0)
			{
				return 0;
			}
			else
			{
				if (delaySumCount < 10) delaySumCount++;
				 totalDelayTime += (interval-totalDelayTime)/ delaySumCount;
				return totalDelayTime;
				
			}
		}
	}
}