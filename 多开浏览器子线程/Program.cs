using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 多开浏览器子线程
{
	static class Program
	{
		
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Init(args);
			Application.Run(new FrmMain());
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

			reg = new DotNet4.Utilities.UtilReg.Reg();
			reg = reg.In("sfMinerDigger");
			//MessageBox.Show(string.Format("子线程已创建到{0}号线程",thisExeThreadId));
		}
	}
}
