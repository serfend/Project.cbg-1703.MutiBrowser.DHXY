using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 订单信息服务器.WebServerControl
{
	public class BaseMessage
	{
		[JsonProperty("title")]
		public string Title;
		[JsonProperty("content")]
		public string Content;
		[JsonProperty("error")]
		public string Error;
	}
	/// <summary>
	/// 浏览器终端初始化结果
	/// </summary>
	public class ClientInitMessage : BaseMessage
	{
		public ClientInitMessage()
		{
			Title = "init";
		}
		[JsonProperty("serverVersion")]
		public string Version;
	}
	/// <summary>
	/// 服务器提交新的订单
	/// </summary>
	public class NewBillMessage : BaseMessage
	{
		[JsonProperty("billInfo")]
		public SubmitBillInfo BillInfo;
		public NewBillMessage()
		{
			Title = "newBill";
		}
		public class SubmitBillInfo
		{
			[JsonProperty("orderId")]
			public string OrderId;
			[JsonProperty("eKey")]
			public string Ekey;
			[JsonProperty("psw")]
			public string Psw;
		}
	}

}
