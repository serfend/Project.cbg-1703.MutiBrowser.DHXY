using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace 订单信息服务器.Bill
{
	/// <summary>
	/// 跟随BillInfo的http
	/// </summary>
	public class BillHttp:IDisposable
	{
		public readonly HttpClient http;
		public BillHttp()
		{
			//TODO 后续可能需要实现自动跟随cookies
			http = new HttpClient(new HttpClientHandler()
			{
				UseProxy = true,
				//Proxy = new WebProxy("127.0.0.1:8888"),
				//UseCookies = false
			});
			ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidate;
		}
		private static bool RemoteCertificateValidate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error) => true;

		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (http != null) http.Dispose();
				}

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
}
