using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 多开浏览器子线程.Inner
{
	class Ccmd
	{
		public DateTime lastRun;
		public bool notFirstTimeRun;
		public long GetCmd( out string targetUrl)
		{
			Program.reg.In("Main").In("Setting").In("cmd").SetInfo(Program.thisExeThreadId + ".lastRunTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
			return NeedRefresh(out targetUrl);
		}
		public void Refresh()
		{
			lastRun = DateTime.Now;
		}
		private string GetNowCmd(string threadId)
		{
			string rel = Program.reg.In("Main").In("ThreadCmd").In(threadId).GetInfo("cmd");
			SetRefresh(threadId);
			return rel;
		}
		private int NeedRefresh(out string targetUrl)
		{
			if (!notFirstTimeRun)
			{
				notFirstTimeRun = true;
				targetUrl = GetDefaultUrl();
				SetRefresh(Program.thisExeThreadId);
				return 2333;
			}
			if ((DateTime.Now - lastRun).TotalSeconds < 10)
			{
				targetUrl = "无需更新";
				return -1;
			}
			switch (GetNowCmd(Program.thisExeThreadId))
			{
				case "":
					{
						targetUrl = "无任何操作";
						return -1;
					}
				case "subClose":
					{
						targetUrl = "关闭进程";
						return 404;
					}
				case "refresh":
					{
						targetUrl = "仅刷新";
						return 1;
					}
				case "newWeb":
					{
						targetUrl = GetNextUrl();
						return 233;
					}
				case "newBill":
					{

						targetUrl = GetNextUrl();
						return 101;
					}
				default:
					{
						targetUrl = "未能识别的指令";
						//Console.WriteLine("CCmd：有未能识别的指令出现");
						return 0;
					}
			}
		}
		public string GetWebInfo(string name)
		{
			return  Program.reg.In("Main").In("Setting").In("cmd").GetInfo(Program.thisExeThreadId + "." + name);
		}
		public string GetNextUrl()
		{
			return Program.reg.In("Main").In("ThreadCmd").In(Program.thisExeThreadId).GetInfo("url");
		}
		private void SetRefresh(string threadId)
		{
			Program.reg.In("Main").In("ThreadCmd").In(threadId).SetInfo("cmd","");
		}
		private string GetDefaultUrl()
		{
			string target = GetWebInfo("defaultUrl");
			if (target.Length == 0)
			{
				// MsgBox getMeNames & "未指定的区，请在主线程设置"
				return "http://xy2.cbg.163.com/";
			}
			else
			{
				return target;
			}
		}
	}
}