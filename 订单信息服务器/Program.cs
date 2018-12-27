using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace 订单信息服务器
{
	
	static class Program
	{
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
			var rawInfo = File.ReadAllLines("js/watchman.min.js");
			var cst = new StringBuilder();
			int nowMarketCount = 0;
			for (int i=0;i<rawInfo.Length;i++)
			{
				var line = rawInfo[i];
				cst.AppendLine(line);
				if (line.Contains("function ") || line.Contains("= function("))
				{
					cst.AppendLine($"console.log(\"{nowMarketCount++ + i + 1} : {line.Replace("\"", "'")}  is calling\");//TODO marked by IDE");
				}
			}
			File.WriteAllText("js/new.js", cst.ToString());

			var js = new JsHelp.JShelp();
			var ts = new JsHelp.JsConsole();
			ts.Http = new Bill.BillHttp().http;
			ts.Js = js;
			js.scriptControl.AddObject("console", ts);
			js.Load("js/global.js");
			js.Load("js/tool.min.js");

			var initCmd = File.ReadAllText("js/init.js", Encoding.UTF8);
			js.Excute(initCmd);
			var testCmd = File.ReadAllText("js/_main.js");
			js.Excute(testCmd);
			var result = js.Excute("result");
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				Application.Run(new Form1());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
		}
	}
}
