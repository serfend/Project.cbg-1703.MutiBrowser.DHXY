using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 多开浏览器子线程.util
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
		public void Submit(Action<string,bool>CallBack,FrmMain frm)
		{
			
			var http = new HttpClient();
			http.Item.Request.Cookies += LoginSession;
			frm.Text = "获取登录凭证" +http.Item.Request.Cookies ;
				http.GetHtml("https://xy2.cbg.163.com/cgi-bin/usertrade.py", "post", BillInfo, callBack: (x) => {
					frm.Invoke((EventHandler)delegate {
						var info = x.response.DataString(Encoding.Default);
						var result = HttpUtil.GetElement(info, "<!--页面内容-->", "</");
						frm.Text = "提交成功:" + result;
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
				});
			});
		}
		private string BillInfo { get {
				return string.Format("act=buy&reonsale_identify={0}&equipid={1}&serverid={2}&device_id={3}", Reonsale_identify,Equipid,Serverid, 697659108 + new Random().Next(1,55));
			} }

		public string LoginSession { get => loginSession; set => loginSession = value; }
		public string Reonsale_identify { get => reonsale_identify; set => reonsale_identify = value; }
		public string Equipid { get => equipid; set => equipid = value; }
		public string Serverid { get => serverid; set => serverid = value; }
	}
}
