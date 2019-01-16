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
			get => Convert.ToInt32(regMain.In(Today).GetInfo("RecordMoneyGetTime", "0"));
			set => regMain.In(Today).SetInfo("RecordMoneyGetTime", value.ToString());
		}
		public static int FitWebShowTime
		{
			get => Convert.ToInt32(regMain.In(Today).GetInfo("FitWebShowTime", "0"));
			set => regMain.In(Today).SetInfo("FitWebShowTime", value.ToString());
		}
		public static double RecordMoneyGet {
			get => Convert.ToDouble(regMain.In(Today).GetInfo("RecordMoneyGet","0"));
			set =>regMain.In(Today).SetInfo("RecordMoneyGet", value.ToString());
		} 
		public static int RecordMoneyGetTime
		{
			get => Convert.ToInt32(regMain.In(Today).GetInfo("RecordMoneyGetTime", "0"));
			set => regMain.In(Today).SetInfo("RecordMoneyGetTime", value.ToString());
		}
		private static string Today => DateTime.Now.ToString("yyyyMMdd");

		internal static void RecordBill(string goodName, double priceNum, double priceNumAssume)
		{
			var nowIndex = RecordMoneyGet;
			regMain.In(Today).SetInfo(nowIndex.ToString(),$"{goodName}:{priceNum}/{priceNumAssume}");
		}

		internal static string GetBillInfo(int i)
		{
			return regMain.In(Today).GetInfo(i.ToString());
		}
	}
}
