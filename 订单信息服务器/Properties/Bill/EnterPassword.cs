
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
	public class EnterPassword
	{
		private string psw;
		private string pswRaw;
		private SubmitResult data;
		private bool success;
		private BillInfo parent;
		public EnterPassword(string psw,BillInfo parent)
		{
			this.parent = parent;
			Psw = psw;
		}

		public string Psw { get => pswRaw; set {
				psw = HttpUtil.ToMD5(value);
				pswRaw = value;
			} }

		public bool Success { get => success; private set => success = value; }
		public SubmitResult Data { get => data; set => data = value; }

		private string SecurityValid => $"{{\"shortPayPassword\":\"{psw}\"}}";
		public void Submit()
		{
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
			Data = Json.JsonDeserializeBySingleData<SubmitResult>(result);
			Success = Data.result == "success";
		}
	}
}