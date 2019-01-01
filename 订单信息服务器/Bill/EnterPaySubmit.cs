using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace 订单信息服务器.Bill
{
	/// <summary>
	/// 提交最终付款请求
	/// </summary>
	public class EnterPaySubmit
	{
		private SubmitResult data;
		private bool success;
		private BillInfo parent;
		public EnterPaySubmit(BillInfo parent)
		{
			this.parent = parent;
		}

		public bool Success { get => success; private set => success = value; }
		public SubmitResult Data { get => data; set => data = value; }
		
		private string Proposal => $"\"orderId\":\"{parent.FirstBill.orderId}\",\"balance\":{{ \"payAmount\":{parent.FirstBill.orderAmount}}}";
		public string RawInfo { get => rawInfo; set => rawInfo = value; }

		private string rawInfo;
		public void Submit()
		{
			var url = $"https://epay.163.com/cashier/m/ajaxPay?v=${HttpUtil.TimeStamp}";//1545203890961
																						//proposal: { "orderId":"2018122212JY61311834","balance":{ "payAmount":5} }
																						//securityValid: { }
																						//envData: { "term":"wap"}
																						//yidunToken: 
			var token = ServerJsManager.GetToken();
						 var message = new HttpRequestMessage(HttpMethod.Post, url)
			{
				Content = new FormUrlEncodedContent(new Dictionary<string, string> {
					{"proposal",Proposal},
					{ "securityValid", $"{{}}"},
					{ "envData","{\"term\":\"wap\"}"},
					{ "yidunToken",token}
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
