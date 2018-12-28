using JsHelp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 订单信息服务器
{
	public static class ServerJsManager
	{
		private static	JShelp js = new JsHelp.JShelp();
		private static	JsConsole ts = new JsHelp.JsConsole();
		public static void Init()
		{
			//var rawInfo = File.ReadAllLines("js/watchman.min.js");
			//var cst = new StringBuilder();
			//int nowMarketCount = 0;
			//for (int i=0;i<rawInfo.Length;i++)
			//{
			//	var line = rawInfo[i];
			//	cst.AppendLine(line);
			//	if (line.Contains("function ") || line.Contains("= function("))
			//	{
			//		cst.AppendLine($"console.log(\"{nowMarketCount++ + i + 1} : {line.Replace("\"", "'")}  is calling\");//TODO marked by IDE");
			//	}
			//}
			//File.WriteAllText("js/new.js", cst.ToString());

			
			ts.Http = new Bill.BillHttp().http;
			ts.Js = js;
			js.scriptControl.AddObject("console", ts);
			js.Load("js/global.js");
			js.Load("js/tool.min.js");

			var initCmd = File.ReadAllText("js/init.js", Encoding.UTF8);
			js.Excute(initCmd);
			MessageBox.Show($"订单付款接口初始化完成:{GetToken()}");
		}
		public static string GetToken() {
			var testCmd = File.ReadAllText("js/_main.js");
			js.Excute(testCmd);
			return js.Excute("final_result").ToString();
		}
	}
}
