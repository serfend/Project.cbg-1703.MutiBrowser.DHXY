using DotNet4.Utilities.UtilCode;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 订单信息服务器.WebServerControl;
using 订单信息服务器.WebSocketServer;

namespace 订单信息服务器
{
	public partial class Form1:Form
	{
		private Dictionary<string, Session> clientList;
		private Dictionary<string, string> payClient;
		public void InitWebBrowserControl()
		{
			clientList = new Dictionary<string, Session>();
			payClient = new Dictionary<string, string>();
			var server = new WebSocket();
			server.OnConnect += Server_OnConnect;
			server.OnDisconnect += Server_OnDisconnect;
			server.OnNewMessage += Server_OnNewMessage;
			server.start(8008);
		}

		private void Server_OnNewMessage(object sender, ClientNewMessageEventArgs e)
		{
			Console.WriteLine($"来自客户端[{e.Session.IP}]消息:{e.Msg}\n{e.Data}");
			if (e.Msg.Contains("<init>")) {
				var clientName = HttpUtil.GetElementInItem(e.Msg,"init");
				payClient.Add(clientName, e.Session.IP);
				this.Invoke((EventHandler)delegate {
					AppendLog($"浏览器端初始化:{clientName}");
				});
				var msg = new ClientInitMessage() {
					Content=clientName
				};
				e.Session.Send(JsonConvert.SerializeObject(msg));
			};

		}

		private void Server_OnDisconnect(object sender, ClientDisconnectEventArgs e)
		{
			clientList.Remove(e.Session.IP);
			
			foreach(var item in payClient)
			{
				if (item.Key == e.Session.IP)
				{
					payClient.Remove(item.Key);
					return;
				}
			}
		}

		private void Server_OnConnect(object sender, ClientConnectEventArgs e)
		{
			clientList.Add(e.Session.IP, e.Session);
		}
	}

}
