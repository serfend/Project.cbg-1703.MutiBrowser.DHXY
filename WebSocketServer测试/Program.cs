using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using 订单信息服务器.WebSocketServer;

namespace WebSocketServer测试
{
	class Program
	{
		private static Dictionary<string, Session> dic=new Dictionary<string, Session>();
		static void Main(string[] args)
		{
			var server = new WebSocket();
			server.OnConnect += Server_OnConnect;
			server.OnDisconnect += Server_OnDisconnect;
			server.OnNewMessage += Server_OnNewMessage;
			server.start(8009);
			while (true)
			{
				lock (dic)
				{
					foreach (var client in dic.Values)
					{
						client.Send($"服务器数据同步{DateTime.Now}");
					}
				}
				Thread.Sleep(2000);
			}
		}

		private static void Server_OnNewMessage(object sender, ClientNewMessageEventArgs e)
		{
			Console.WriteLine($"来自客户端[{e.Session.IP}]消息:{e.Data}");
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
