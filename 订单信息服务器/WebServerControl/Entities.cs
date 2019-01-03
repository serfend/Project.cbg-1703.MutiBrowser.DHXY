using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 订单信息服务器.WebServerControl
{
	public class BaseMessage
	{
		[JsonProperty("title")]
		public string Title;
		[JsonProperty("content")]
		public string Content;
		[JsonProperty("error")]
		public string Error;
	}
	public class ClientInitMessage : BaseMessage
	{
		public ClientInitMessage()
		{
			Title = "init";
		}
		[JsonProperty("serverVersion")]
		public string Version;
	}
}
