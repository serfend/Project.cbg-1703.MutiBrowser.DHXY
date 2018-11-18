using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 多开浏览器子线程
{
	static class Program
	{
		public static SfTcp.SfTcpClient Tcp ;

		private static void InitTcp()
		{
			Tcp = new TcpBrowserClient();
			Tcp.Disconnected += (x) => { InitTcp(); };
		}
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			InitTcp();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Init(args);
			

			try
			{
				Application.Run(new FrmMain());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
			}
		}
		public static DotNet4.Utilities.UtilReg.Reg reg ;
		public static string thisExeThreadId;
		private static void Init(string[] args)
		{
			if (args.Length == 0)
			{
				thisExeThreadId = "1";
			}
			else
			{
				thisExeThreadId = args[0];
			}

			reg = new DotNet4.Utilities.UtilReg.Reg("sfMinerDigger");
			//MessageBox.Show(string.Format("子线程已创建到{0}号线程",thisExeThreadId));
		}
	}
}
