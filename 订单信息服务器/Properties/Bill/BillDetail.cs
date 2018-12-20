using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace 订单信息服务器.Bill
{
	public class BillDetail
	{
		private string billId;
		private readonly BillInfo parent;
		private BillDetailJson data;
		public BillDetail(string billId,BillInfo parent)
		{
			this.billId = billId;
			this.parent = parent;
		}
		public BillDetailJson GetData()
		{
			var url = $"https://epay.163.com/wap/h5/trade/detailView.htm#orderId={BillId}";
			var message = new HttpRequestMessage(HttpMethod.Post, "https://epay.163.com/wap/h5/ajax/trade/listView.htm")
			{
				Content = new FormUrlEncodedContent(new Dictionary<string, string> {
					{ "queryCondition","{\"isAjax\":true,\"page\":1}"},
					{ "envData","{\"term\":\"wap\"}"}
				})
			};
			message.Headers.Add("Cookie", $"NTES_SESS={parent.NTES_SESS}");
			var result = parent.billHttp.http.SendAsync(message).Result.Content.ReadAsStringAsync().Result;
			data = Json.JsonDeserializeBySingleData<BillDetailJson>(result);
			return data;
		}
		public string BillId { get => billId; set => billId = value; }
	}

	public class BillDetailJson
	{
	}
}
