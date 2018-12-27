using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 订单信息服务器.Bill
{
	public class PlatformOrderDate
	{
		/// <summary>
		/// 
		/// </summary>
		public int date { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int day { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int hours { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int minutes { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int month { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int nanos { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int seconds { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public long time { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int timezoneOffset { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int year { get; set; }
	}
	public class ResultListItem
	{
		/// <summary>
		/// 买入
		/// </summary>
		public string behavior { get; set; }
		/// <summary>
		/// 金钱
		/// </summary>
		public string goodName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string orderAmount { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string orderId { get; set; }
		/// <summary>
		/// 藏宝阁审核中
		/// </summary>
		public string orderState { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string orderTime { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string platformId { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public PlatformOrderDate platformOrderDate { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string platformOrderId { get; set; }
	}

	public class Data
	{
		/// <summary>
		/// 
		/// </summary>
		public List<ResultListItem> resultList { get; set; }
	}

	public class BillInfoJson
	{
		/// <summary>
		/// 
		/// </summary>
		public Data data { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string errorCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string errorMsg { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string result { get; set; }
	}
}
