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
	public class Client
	{
		private Session session;
		private string name;
		private ListViewItem viewItem;
		public Session Session { get => session; set => session = value; }
		public string Name { get => name; set => name = value; }
		public ListViewItem ViewItem { get => viewItem; set => viewItem = value; }
	}
	public partial class Form1
	{
		/// <summary>
		/// ip对应终端
		/// </summary>
		private Dictionary<string, Client> payClient;
		/// <summary>
		/// 终端名称对应ip
		/// </summary>
		private Dictionary<string, string> payClientIp;
		
		public void InitWebBrowserControl()
		{
			payClient = new Dictionary<string, Client>();
			payClientIp = new Dictionary<string, string>();
			var server = new WebSocket();
			server.OnConnect += Server_OnConnect;
			server.OnDisconnect += Server_OnDisconnect;
			server.OnNewMessage += Server_OnNewMessage;
			server.start(8008);
		}

		private void Server_OnNewMessage(object sender, ClientNewMessageEventArgs e)
		{
			var client = payClient[e.Session.IP];
			Console.WriteLine($"来自客户端[{e.Session.IP}]消息:{e.Msg}");
			if (e.Msg.Contains("<init>")) {
				var clientName = HttpUtil.GetElementInItem(e.Msg,"init");
				if (payClientIp.ContainsKey(clientName))
				{
					var msg = new ClientInitMessage()
					{
						Error=$"重复初始化浏览器{clientName}"
					};
					e.Session.Send(JsonConvert.SerializeObject(msg));
					return;
				}
				else
				{
					var msg = new ClientInitMessage()
					{
						Content = clientName
					};
					e.Session.Send(JsonConvert.SerializeObject(msg));
					client.Name = clientName;
				}
				payClientIp.Add(clientName, e.Session.IP);
				this.Invoke((EventHandler)delegate {
					AppendLog($"浏览器端初始化:{clientName}");
					var data = new string[2];
					data[0] = clientName;
					data[1] = "浏览器初始化";
					var item = new ListViewItem(data);
					client.ViewItem = item;
					LstBrowserClient.Items.Add(item);
				});
				
			};

		}

		private void Server_OnDisconnect(object sender, ClientDisconnectEventArgs e)
		{
			if (!payClient.ContainsKey(e.Session.IP)) return;
			var client = payClient[e.Session.IP];
			this.Invoke((EventHandler)delegate {
				var item = client.ViewItem;
				LstBrowserClient.Items.Remove(item);
			});
			payClientIp.Remove(client.Name);
			payClient.Remove(e.Session.IP);
		}

		private void Server_OnConnect(object sender, ClientConnectEventArgs e)
		{
			payClient.Add(e.Session.IP, new Client() {
				Session = e.Session,
			});
		}
	}

}
