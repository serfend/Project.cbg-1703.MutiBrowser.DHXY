using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JsHelp
{
	[ComVisible(true)]
	public class JsConsole
	{
		private const string JSTARGETFILE = "js/watchman.min.js";
		private HttpClient http;
		private JShelp js;
		public HttpClient Http { get => http; set => http = value; }
		internal JShelp Js { get => js; set => js = value; }
		private string jsFileContent;
		public JsConsole()
		{
			jsFileContent = File.ReadAllText(JSTARGETFILE,Encoding.UTF8);
		}
		private Dictionary<string, string> storageDic = new Dictionary<string, string>();
		public void setStorage(string key,string value)
		{
			Console.WriteLine($"console.setStorage({key})==>{value}");
			storageDic[key] = value;
		}
		public string getStorage(string key)
		{
			var result= storageDic.ContainsKey(key) ? storageDic[key] : "defaultValue";
			Console.WriteLine($"console.getStorage({key})={result}");
			return result;
		}

		public void log(string info)
		{
			Console.WriteLine(info);
		}
		public void alert(string info)
		{
			MessageBox.Show(info);
		}
		/// <summary>
		/// 通过内置回调数据
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public string getHttpContent(string url)
		{
			return Http.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
		}
		/// <summary>
		/// 通过基本http回调
		/// </summary>
		/// <param name="url"></param>
		/// <param name="method"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public string httpRequest(string url,string method,string data)
		{
			var http =new DotNet4.Utilities.UtilHttp.HttpClient();
			var result= http.GetHtml(url, method, data).document.response.DataString(Encoding.UTF8);
			return result;
		}
		/// <summary>
		/// 初始化watchman
		/// </summary>
		public void InitWatchMan()
		{
			js.Load(JSTARGETFILE);
		}

		public void ReplaceFileContent(string raw,string newInfo)
		{
			if (raw.Length == 0 || newInfo.Length == 0) return;
			jsFileContent=jsFileContent.Replace($"\"{raw}\"", $"\"{newInfo}\"");
		}
		public void SaveFile()
		{
			File.WriteAllText("js/tmp.js", jsFileContent);
		}
		/// <summary>
		/// 获取时间戳
		/// </summary>
		/// <returns></returns>
		public string getTime() => HttpUtil.TimeStamp.ToString();

		public string unescape(string raw) => HttpUtil.DecodeUnicode(raw);
	}
}
