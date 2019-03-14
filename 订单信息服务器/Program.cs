using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using 订单信息服务器.WebSocketServer;

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
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; ;
				Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
				Application.Run(new Form1());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			MessageBox.Show($"{e.Exception.Message}\n{e.Exception.StackTrace}", "线程错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.ExceptionObject.ToString(), "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
	public class RandomInfo
	{
		public string name;
	}
}
