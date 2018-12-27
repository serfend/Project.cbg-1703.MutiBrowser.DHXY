
using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace 订单信息服务器.Bill
{
	/// <summary>
	/// 输入将军令
	/// </summary>
	public class EnterAuthKey
	{
		private string authKey;

		private SubmitResult data;
		private bool success;
		private BillInfo parent;
		public EnterAuthKey( BillInfo parent)
		{
			this.parent = parent;
		}

		public bool Success { get => success; private set => success = value; }
		public string AuthKey { get => authKey; set => authKey = value; }
		public SubmitResult Data { get => data; set => data = value; }

		private string SecurityValid => $"{{\"eKey\":\"{AuthKey}\"}}";

		public string RawInfo { get => rawInfo; set => rawInfo = value; }

		private string rawInfo;
		public void Submit(string authKey)
		{
			AuthKey = authKey;
			var url = $"https://epay.163.com/cashier/m/security/verifyPayItems?v=${HttpUtil.TimeStamp}";//1545203890961
			var message = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = new FormUrlEncodedContent(new Dictionary<string, string> {
					{ "securityValid", $"{SecurityValid}"},
					{ "orderId",$"{parent.FirstBill.orderId}" },
					{ "envData","{\"term\":\"wap\"}"}
		})
			};
			message.Headers.Add("Cookie", $"NTES_SESS={parent.NTES_SESS}");
			var result = parent.billHttp.http.SendAsync(message).Result.Content.ReadAsStringAsync().Result;
			rawInfo = result;
			Data = Json.JsonDeserializeBySingleData<SubmitResult>(result);
			Success = Data.result == "success";
		}
	}


}