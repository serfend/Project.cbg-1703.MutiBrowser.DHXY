using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 订单信息服务器
{
	static class ManagerHttpBase
	{
		private readonly static Reg regMain=new Reg("sfMinerDigger").In("Setting").In("HttpManager");
		private static string targetUrl = "";

		public static string TargetUrl { get {
				return regMain.GetInfo("targetUrl", "null");
			} set {
				targetUrl = value;
				regMain.SetInfo("targetUrl", value);
			} }

		public static int UserWebShowTime { get => userWebShowTime; set => userWebShowTime = value; }
		public static int FitWebShowTime { get => fitWebShowTime; set => fitWebShowTime = value; }
		public static double RecordMoneyGet { get => recordMoneyGet; set => recordMoneyGet = value; }
		public static int RecordMoneyGetTime { get => recordMoneyGetTime; set => recordMoneyGetTime = value; }

		private static int fitWebShowTime;
		private static int userWebShowTime;

		private static double recordMoneyGet;
		private static int recordMoneyGetTime;
	}
}
