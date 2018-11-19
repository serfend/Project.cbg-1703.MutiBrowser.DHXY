using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 订单信息服务器
{
	static class ManagerHttpBase
	{
		private readonly static Reg regMain = new Reg("sfMinerDigger").In("Setting").In("HttpManager");
		private static string targetUrl = "";

		public static string TargetUrl { get {
				return regMain.GetInfo("targetUrl", "null");
			} set {
				targetUrl = value;
				regMain.SetInfo("targetUrl", value);
			} }
	
		public static int UserWebShowTime
		{
			get => Convert.ToInt32(regMain.GetInfo("RecordMoneyGetTime", "0"));
			set => regMain.SetInfo("RecordMoneyGetTime", value.ToString());
		}
		public static int FitWebShowTime
		{
			get => Convert.ToInt32(regMain.GetInfo("FitWebShowTime", "0"));
			set => regMain.SetInfo("FitWebShowTime", value.ToString());
		}
		public static double RecordMoneyGet {
			get => Convert.ToDouble(regMain.GetInfo("RecordMoneyGet","0"));
			set =>regMain.SetInfo("RecordMoneyGet", value.ToString());
		} 
		public static int RecordMoneyGetTime
		{
			get => Convert.ToInt32(regMain.GetInfo("RecordMoneyGetTime", "0"));
			set => regMain.SetInfo("RecordMoneyGetTime", value.ToString());
		}
	
	}
}
