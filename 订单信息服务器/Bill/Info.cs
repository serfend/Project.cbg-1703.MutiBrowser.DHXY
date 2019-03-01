
using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 订单信息服务器.Bill
{
	
	/// <summary>
	/// 记录账号全局信息
	/// </summary>
	public class BillInfo:IDisposable
	{
		private string _NTES_SESS;
		private BillInfoJson data;
		public BillHttp billHttp=new BillHttp();
		public BillInfo(string ntes_SESS)
		{
			NTES_SESS = ntes_SESS;
			
		}

		public BillInfoJson GetData(int tryTime=10,Action<string> callback=null)
		{
			if (tryTime <= 0)
			{
				throw new BillListLoadException("因尝试次数过多，导致获取订单失败");
			}
			var message = new HttpRequestMessage(HttpMethod.Post, "https://epay.163.com/wap/h5/ajax/trade/listView.htm")
			{
				Content = new FormUrlEncodedContent(new Dictionary<string, string> {
				{ "queryCondition","{\"isAjax\":true,\"page\":1}"},
				{ "envData","{\"term\":\"wap\"}"}
			})
			};
			message.Headers.Add("Cookie", $"{NTES_SESS}");
			message.Headers.Host = "epay.163.com";
			var http = new HttpClient(new HttpClientHandler() {
				UseCookies=false
			}) {
						
			};
		
			var result =http.SendAsync(message).Result.Content.ReadAsStringAsync().Result;
			try
			{
				data = Json.JsonDeserializeBySingleData<BillInfoJson>(result);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n" + result);
				return null;
			}
			var t = data?.result;

			if (t == "error")
			{
				throw new BillListLoadException(data.errorMsg);
			}
			if (data.data.resultList.Count==0)
			{
				callback?.Invoke($"{tryTime} 当前无数据");
				Thread.Sleep(200);
				return GetData(tryTime - 1, callback);
			}
			else if (data.data.resultList[0].orderState != "待付款")
			{
				callback?.Invoke($"{tryTime} {data.data.resultList[0].orderState}");
				Thread.Sleep(100);
				return GetData(tryTime-1,callback);
			}
			else
			return data;
		}
		public ResultListItem FirstBill { get {
				var result = data?.result;
				return data.data.resultList[0];
			} }  

		public string NTES_SESS { get => _NTES_SESS; set => _NTES_SESS = value; }


		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (billHttp != null) billHttp.Dispose();
				}
				billHttp = null;
				disposedValue = true;
			}
		}


		// 添加此代码以正确实现可处置模式。
		public void Dispose()
		{
			// 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
			Dispose(true);
		}
		#endregion
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