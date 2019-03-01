using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace 订单信息服务器.FrmServer
{
	public class HttpUserInfo
	{
		[JsonProperty("ip")]
		public string IP;
		[JsonProperty("name")]
		public string ClientName;
		
	}
}
