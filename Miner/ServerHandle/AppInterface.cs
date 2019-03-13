using DotNet4.Utilities.UtilCode;
using SfTcp.TcpMessage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace Miner.ServerHandle
{
	public class AppInterface:IDisposable
	{
		private readonly string goodListUrl = "https://xy2-android2.cbg.163.com/cbg-center/query.py?kindid=45&pass_fair_show=0&need_check_license=1&app_version_code=2302&package_name=com.netease.xy2cbg&platform=android&app_version=2.3.0&device_id={0}&device_name=m3s&os_version=5.1.1&channel=ANDNETEASE&page=1&os_version_code=22&os_name=m3s";
		//获取商品详情 device_id game_ordersn serverid
		private readonly string goodDetailUrl = "https://xy2-android2.cbg.163.com/cbg-center/query.py?act=get_equip_detail&platform=android&app_version=2.3.0&serverid={2}&game_ordersn={1}&device_id={0}&need_check_license=1&device_name=m3s&app_version_code=2302&os_version=5.1.1&os_version_code=22&os_name=m3s&package_name=com.netease.xy2cbg";
		private readonly string device_id;
		private HttpClient http;
		private GoodList data;
		private bool isFirstTime = true;
		private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)=>true;
		public AppInterface()
		{
			device_id = Device_id();
			http = new HttpClient();
			ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
		}
		private static string Device_id()
		{
			return $"xy2.{HttpUtil.UfUID}";
		}

		internal GoodList GetGoodList()
		{
			var url = string.Format(goodListUrl, device_id);
			var response = http.GetAsync(url).Result;
			var rawInfo= response.Content.ReadAsStringAsync().Result;
			var info = HttpUtil.DecodeUnicode(rawInfo);
			try
			{
				data = Json.JsonDeserializeBySingleData<GoodList>(info);
			}
			catch (Exception ex)
			{
				MessageBox.Show("处理AppInterface.GetGoodList()失败\n"+ex.Message+"\n"+info);
			}
			if (data.equip_list==null)
			{
				throw new GoodListNoDataException(data.msg);
			}
			return data;
		}
		private Dictionary<string, int> goodsBeenHandle = new Dictionary<string, int>();
		internal List<Equip_listItem> GetNeedHandle()
		{
			var list = new List<Equip_listItem>();
			foreach(var item in data.equip_list)
			{
				if (!goodsBeenHandle.ContainsKey(item.game_ordersn))
				{
					goodsBeenHandle.Add(item.game_ordersn,1);
					if(!isFirstTime)
						list.Add(item);
				}
			}
			isFirstTime = false;
			return list;
		}
		internal GoodDetail GetGoodDetail(Equip_listItem targetGood)
		{
			var url = string.Format(goodDetailUrl, device_id,targetGood.game_ordersn,targetGood.serverid);
			var response = http.GetAsync(url).Result;
			var rawInfo = response.Content.ReadAsStringAsync().Result;
			var info = HttpUtil.DecodeUnicode(rawInfo);
			try
			{
				return Json.JsonDeserializeBySingleData<GoodDetail>(info);
			}
			catch (Exception ex)
			{
				var exceptionInfo = "处理AppInterface.GetGoodDetail()失败\n" + ex.Message + "\n" + info;
				Program.Tcp?.Send(new RpGoodDetailFailMessage(ex.Message));
				File.WriteAllText("exception.txt",exceptionInfo);
			}
			return null;

		}

		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if(http!=null)http.Dispose();
				}
				http = null;
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
	public class GoodListNoDataException : Exception
	{
		public GoodListNoDataException() { }
		public GoodListNoDataException(string message) : base(message) { }
		public GoodListNoDataException(string message, Exception inner) : base(message, inner) { }
		protected GoodListNoDataException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
