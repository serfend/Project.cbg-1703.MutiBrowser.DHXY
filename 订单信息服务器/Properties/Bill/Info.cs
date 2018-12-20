
using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace 订单信息服务器.Bill
{
	[Serializable]
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
	[Serializable]
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

	[Serializable]
	public class Data
	{
		/// <summary>
		/// 
		/// </summary>
		public List<ResultListItem> resultList { get; set; }
	}

	[Serializable]
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
	/// <summary>
	/// 记录账号全局信息
	/// </summary>
	public class BillInfo
	{
		private string _NTES_SESS;
		private BillInfoJson data;
		public BillHttp billHttp=new BillHttp();
		public BillInfo(string ntes_SESS)
		{
			NTES_SESS = ntes_SESS;
			ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
		}
		private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error) => true;
		public BillInfoJson GetData()
		{
			var message = new HttpRequestMessage(HttpMethod.Post, "https://epay.163.com/wap/h5/ajax/trade/listView.htm")
			{
				Content = new FormUrlEncodedContent(new Dictionary<string, string> {
					{ "queryCondition","{\"isAjax\":true,\"page\":1}"},
					{ "envData","{\"term\":\"wap\"}"}
				})
			};
			message.Headers.Add("Cookie", $"NTES_SESS={NTES_SESS}");
			var result = billHttp.http.SendAsync(message).Result.Content.ReadAsStringAsync().Result;
			data= Json.JsonDeserializeBySingleData<BillInfoJson>(result);
			var t = data?.result;
			if (t == "error")
			{
				throw new BillListLoadException(data.errorMsg);
			}
			return data;
			
		}
		public ResultListItem FirstBill { get {
				var result = data?.result;
				return data.data.resultList[0];
			} }  

		public string NTES_SESS { get => _NTES_SESS; set => _NTES_SESS = value; }

		public static void Test()
		{
			var t = new Bill.BillInfo("KkDtslqSVpzFEybq032fVYaaTxbMAmmG47Mi98fQweRnBlf2BpU6VjfD0UFEESWFP4K35oImjlNx2tkyl1XTvYP_LDrqj5unL3xsyTiBn5JhzuXBMTFLiv3laMHFjIG7UEkyXK4CIDL94sSdNSL_VTBVJJRkk2RYR6WXe7xLVLjYhD.eEvX4a6AHGNnZ.C8UIyvUNrSTqC7ej");
			var r = t.GetData();
			Console.WriteLine($"当前第一个订单为{t.FirstBill.orderId},{t.FirstBill.goodName},金额:{t.FirstBill.orderAmount}");
		}
	}

	[Serializable]
	public class BillListLoadException : Exception
	{
		public BillListLoadException():base("加载订单列表异常") { }
		public BillListLoadException(string message) : base(message) { }
		public BillListLoadException(string message, Exception inner) : base(message, inner) { }
		protected BillListLoadException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}