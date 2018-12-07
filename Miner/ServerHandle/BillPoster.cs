using DotNet4.Utilities.UtilCode;
using Miner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner.util
{
	class BillPoster
	{
		private string loginSession;
		private string reonsale_identify;
		private string equipid;
		private string serverid;
		public BillPoster(string infoInit,string loginSession)
		{
			this.LoginSession = loginSession;
			var tmp = HttpUtil.GetElement(infoInit, "reonsale_identify", "/>");
			Reonsale_identify = HttpUtil.GetElement(tmp, "value=\"", "\"");
			tmp = HttpUtil.GetElement(infoInit, "equipid", "/>");
			Equipid = HttpUtil.GetElement(tmp, "value=\"", "\"");
			tmp = HttpUtil.GetElement(infoInit, "serverid", "/>");
			Serverid = HttpUtil.GetElement(tmp, "value=\"", "\"");
		}
		public void Submit(Action<string, bool> CallBack)
		{

			var http = new HttpClient(new HttpClientHandler() { UseCookies = false });
			http.DefaultRequestHeaders.Add("Cookies", LoginSession);
			Program.setting.LogInfo($"获取登录凭证{LoginSession}", "下单记录");
			if (LoginSession.Length < 5)
			{
				Program.setting.LogInfo($"获取登录凭证失败");
				CallBack.Invoke("无凭证", false);
				return;
			}
			var s = http.PostAsync("https://xy2.cbg.163.com/cgi-bin/usertrade.py", new FormUrlEncodedContent(BillInfoList)).Result.Content.ReadAsStreamAsync().Result;
			using (var stream = new StreamReader(s, Encoding.Default))
			{
				var info = stream.ReadToEnd();

				var result = HttpUtil.GetElement(info, "<!--页面内容-->", "</");
				Program.setting.LogInfo($"提交成功:{ result}");
				for (int i = result.Length - 1; i > 0; i--)
				{
					if (result[i] == '>')
					{
						result = result.Substring(i);
						break;
					}
				}
				if (result.Length == 1)
				{
					if (info.Contains("建议尽快完成支付，最先完成支付的玩家才可以成功购得该商品"))
					{
						CallBack.Invoke("下单成功", true);
						return;
					}
					CallBack.Invoke(result, false);
				}
				CallBack.Invoke("下单失败:" + result, false);
			}
		}
		private string BillInfo { get {
				return string.Format("act=buy&reonsale_identify={0}&equipid={1}&serverid={2}&device_id={3}", Reonsale_identify,Equipid,Serverid, 697659108 + new Random().Next(1,55));
			} }
		private List<KeyValuePair<string,string>> BillInfoList
		{
			get {
				var list = new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("act", "buy"),
					new KeyValuePair<string, string>("reonsale_identify", Reonsale_identify),
					new KeyValuePair<string, string>("equipid", Equipid),
					new KeyValuePair<string, string>("serverid", Serverid),
					new KeyValuePair<string, string>("device_id", (697659108 + new Random().Next(1, 55)).ToString())
				};
				return list;
			}
		}
		public string LoginSession { get => loginSession; set => loginSession = value; }
		public string Reonsale_identify { get => reonsale_identify; set => reonsale_identify = value; }
		public string Equipid { get => equipid; set => equipid = value; }
		public string Serverid { get => serverid; set => serverid = value; }
	}
}
