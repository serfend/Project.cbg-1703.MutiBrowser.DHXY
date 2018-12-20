using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SfTcp
{
	public class TcpServerManager
	{
		private List<TcpServer> list = new List<TcpServer>();
		public Action<TcpServer> ServerConnected;
		public Action<TcpServer> ServerDisconnected;
		/// <summary>
		/// tcpServer,title,content
		/// </summary>
		public Action<TcpServer, string,string> NormalMessage;
		public Action<TcpHttpMessage, TcpHttpResponse> HttpRequest;
		
		public TcpServer this[string ip]
		{
			get => list.Find((x) => x.Ip == ip);
		}
		public TcpServerManager()
		{
			NewTcp();
			threadCheckConnection = new Thread(() =>
			{
				while (true)
				{
					Thread.Sleep(10000);
					CheckConnection();
				}
			})
			{ IsBackground=true};
			threadCheckConnection.Start();
		}
		private readonly Thread threadCheckConnection;
		private Action<TcpServer> Connected() {
			return new Action<TcpServer>((x) =>
			{
				ServerConnected?.Invoke(x);
				Console.WriteLine("新的用户连接"+x.client.Client.RemoteEndPoint.ToString());
				list.Add(x);
				NewTcp();//继续侦听其他客户端
			});
		}
		private TcpServer NewTcp(int port=8009)
		{
			return new TcpServer((x,InnerInfo, s) => {
				s.lastMessageTime = Environment.TickCount;
				NormalMessage?.Invoke(s,x,InnerInfo);
				if (x.Contains("BrowserClientReport"))
				{
					if (InnerInfo.Contains("heartBeat"))
					{
						s.Send("<server.response>heartBeat</server.response>");
						return;
					}
					foreach (var p in list)
					{
						if (!p.IsLocal) p.Send("<server.command>" + InnerInfo + "</server.command>");
					}
				}else if (x.Contains("ping"))
				{
					s.Send($"<ping></ping>");
				}
			}, (x,s) => {
				HttpRequest.Invoke(x,s);
			}
			
			,port)
			{
				Connected = Connected(),
				Disconnected = (x) => {
					ServerDisconnected?.Invoke(x);
					list.Remove(x); }
			};
		}
		public int CheckConnection()
		{
			var tick = Environment.TickCount;
			var waitToDisConnect = new List<TcpServer>();
			foreach (var client in list)
			{
				if (tick - client.lastMessageTime > 60000&&  client.ID!="...") waitToDisConnect.Add(client) ;
			}
			foreach(var client in waitToDisConnect)
			{
				client.Disconnect();
			}
			return waitToDisConnect.Count;
		}
	
	}
}
