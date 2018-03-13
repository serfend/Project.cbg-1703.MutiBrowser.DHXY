using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miner
{
	class Program
	{
		public static Setting setting;
		public static ServerList servers;
		static void Main(string[] args)
		{
			Logger.OnLog += (x,xx) =>{ Console.WriteLine(x); };
			setting = new Setting(args.Length > 0 ? args[0] : null);
			servers = new ServerList();
			setting.LogInfo("进程加载完成");
			servers.Run();
			setting.LogInfo("进程退出");
		}
	}
}
