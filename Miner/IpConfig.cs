using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Miner
{
	public class IpConfig
	{
		private List<string> list;
		public string Status;
		public IpConfig(int amount)
		{
			var http = new HttpClient();
			var url = string.Format("http://mapi.baibianip.com/getproxy?type=dymatic&apikey=2c9e5380544564fe718722e53ba54a9c&count={0}&unique=1&lb=2&format=text&sort=0&resf=1", amount);
			var response = http.GetAsync(new Uri(url)).Result;
			var resRaw = response.Content.ReadAsStringAsync().Result;
			if (resRaw.Contains("success")) {//不成功
				Status = resRaw;
			}
			else
			{
				//var ipListRaw = HttpUtil.GetAllElements(resRaw, "ip\":\"", "}");
				list = new List<string>
				{
					//foreach(var ipRaw in ipListRaw)
					//{
					//	var ip = HttpUtil.GetElementLeft(ipRaw, "\"");
					//	var port = HttpUtil.GetElementRight(ipRaw, "\":");
					//	list.Add(string.Format("{0}:{1}", ip, port));
					//}
					//Status = resRaw;// HttpUtil.GetElement(resRaw, "error_message\":\"","}");
					resRaw
				};

			}
			
		}
		public string this[int index]
		{
			get {
				if (list == null) return null;
				return list[index];
			}
		}
		public int Count { get => (list == null ?   0: list.Count); }
	}
}