﻿using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Miner
{
	class CmdRasdial
	{
		private string userName;
		private string userPsw;
		private string rasName;

		public CmdRasdial(string userName, string userPsw, string rasName)
		{
			this.userName = userName;
			this.userPsw = userPsw;
			this.rasName = rasName;
		}
		public CmdRasdial()
		{
			var info = File.ReadAllLines("宽带拨号.txt",Encoding.Default);
			rasName = info[0];
			userName = info[1];
			userPsw = info[2];
		}

		public string UserName { get => userName; set => userName = value; }
		public string UserPsw { get => userPsw; set => userPsw = value; }
		public string RasName { get => rasName; set => rasName = value; }

		public void DisRasdial()
		{
			CmdRun("Rasdial "+ rasName +" "+ userName +" "+ userPsw);
		}
		public void Rasdial()
		{
			CmdRun("Rasdial" + rasName + "/ DISCONNECT");
		}
		public void CmdRun(string str)
		{

			System.Diagnostics.Process p = new System.Diagnostics.Process();
			p.StartInfo.FileName = "cmd.exe";
			p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
			p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
			p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
			p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
			p.StartInfo.CreateNoWindow = true;//不显示程序窗口
			p.Start();//启动程序

			//向cmd窗口发送输入信息
			p.StandardInput.WriteLine(str + "&exit");

			p.StandardInput.AutoFlush = true;
			//p.StandardInput.WriteLine("exit");
			//向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
			//同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令



			//获取cmd窗口的输出信息
			string output = p.StandardOutput.ReadToEnd();

			//StreamReader reader = p.StandardOutput;
			//string line=reader.ReadLine();
			//while (!reader.EndOfStream)
			//{
			//    str += line + "  ";
			//    line = reader.ReadLine();
			//}

			p.WaitForExit();//等待程序执行完退出进程
			p.Close();


			Logger.SysLog(output, "Cmd");
		}
	}
}
