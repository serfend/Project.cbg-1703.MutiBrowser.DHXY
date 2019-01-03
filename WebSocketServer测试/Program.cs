using DotNet4.Utilities.UtilCode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using 订单信息服务器.WebSocketServer;

namespace WebSocketServer测试
{
	public class BillInfo
	{
		[JsonProperty("orderId")]
		public string OrderId;
		[JsonProperty("name")]
		public string Name;
		[JsonProperty("eKey")]
		public string Ekey;
		[JsonProperty("psw")]
		public string Psw;
	}
	class Program
	{
		
		private static Dictionary<string, Session> dic=new Dictionary<string, Session>();
		static void Main(string[] args)
		{
			var server = new WebSocket();
			server.OnConnect += Server_OnConnect;
			server.OnDisconnect += Server_OnDisconnect;
			server.OnNewMessage += Server_OnNewMessage;
			server.start(8008);
			while (true)
			{
				lock (dic)
				{
					Thread.Sleep(10000);
					var info = new BillInfo() {
						OrderId = "2019010115JY27791135",
						Name = "test",
						Ekey = "937076",
						Psw="121212"
					};
					var json = Newtonsoft.Json.JsonConvert.SerializeObject(info);
					foreach (var client in dic.Values)
					{
						client.Send($"bill:{json}");
					}
				}
				
			}
		}

		private static void Server_OnNewMessage(object sender, ClientNewMessageEventArgs e)	
		{
			Console.WriteLine($"来自客户端[{e.Session.IP}]消息:{e.Msg}");
		}

		private static void Server_OnDisconnect(object sender, ClientDisconnectEventArgs e)
		{
			dic.Remove(e.Session.IP);
		}

		private static void Server_OnConnect(object sender, ClientConnectEventArgs e)
		{
			dic.Add(e.Session.IP,e.Session);
		}
	}
}
