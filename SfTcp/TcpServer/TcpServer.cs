
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SfTcp.TcpServer
{
	public class TcpServer:IDisposable
	{
		private SfBaseTcp.Net.Sockets.TCPListener server;
		public ILog Log { set; get; }
		public int Port { get => port; set => port = value; }

		public TcpConnection this[string key] { get {
				list.TryGetValue(key,out TcpConnection result);
				return result;
			} set => list[key] = value; }
		private int port;
		public event ClientConnect OnConnect;
		public event ClientDisconnect OnDisconnect;
		public event ClientMessage OnMessage;
		public event ClientHttpMessage OnHttpMessage;
		private ConcurrentDictionary<string, TcpConnection> list;
		private ConcurrentDictionary<string, int> lastMessageStamp=new ConcurrentDictionary<string, int>();
		private bool isListening;
		/// <summary>
		/// 用于定时检查终端，并释放长时间无通讯的终端
		/// </summary>
		//private Thread checkClientAlive;
		public TcpServer(int port)
		{
			list = new ConcurrentDictionary<string, TcpConnection>();
			server = new SfBaseTcp.Net.Sockets.TCPListener() {
				Port=port
			};
			server.DisconnectCompleted += Server_DisconnectCompleted; ;
			server.ReceiveCompleted += Server_ReceiveCompleted; ;
			server.AcceptCompleted += Server_AcceptCompleted; ;

			//checkClientAlive = new Thread(CheckClientAlive) { IsBackground=true};
			//checkClientAlive.Start(); 此处自动检测活性暂不可用
			server.Start();
		}

		private void Server_AcceptCompleted(object sender, SfBaseTcp.Net.Sockets.SocketEventArgs e)
		{
			lock (list)
			{
				var ip = e.Socket.RemoteEndPoint.ToString();
				TcpConnection connection;
				connection = new TcpConnection(e.Socket, "null");
				if (list == null)
				{
					throw new Exception("list is not init");
				}
				list.AddOrUpdate(ip, connection,(clientIp,clientConnect)=>clientConnect);
				//lastMessageStamp.AddOrUpdate(connection.Ip, Environment.TickCount, (key, value) =>
				//{
				//	return Environment.TickCount;
				//});
				OnConnect?.BeginInvoke(connection, new ClientConnectEventArgs(), (x) => { }, null);
			}
		}

		private void Server_DisconnectCompleted(object sender, SfBaseTcp.Net.Sockets.SocketEventArgs e)
		{
			RaiseOnDisconnect(e.Socket.RemoteEndPoint.ToString());
		}

		private void Server_ReceiveCompleted(object sender, SfBaseTcp.Net.Sockets.SocketEventArgs e)
		{
			string ip = e.Socket.RemoteEndPoint.ToString();
			
			
			var msg = new ClientMessageEventArgs(e.Data);
			Console.WriteLine($"{ip}->{msg.RawString}");
			if (msg.Error)
			{
				var httpMsg = TcpHttpMessage.CheckTcpHttpMessage(msg.RawString);
				if (httpMsg != null)
				{
					list.TryGetValue(ip, out TcpConnection connection);
					if(connection!=null&& connection.Client.IsConnected) OnHttpMessage?.Invoke(sender, new ClientHttpMessageEventArgs(httpMsg, connection));
					return;
				}
				else
				{
					//throw new MsgInvalidException(msg.RawString);
					return;
				}
			}
			RaiseOnMessage(ip, msg);
		}


		private void RaiseOnDisconnect(string s)
		{
			lock (list)
			{
				if (!list.ContainsKey(s)) return;
				var connection = this[s];
				list.TryRemove(s,out TcpConnection removeConnection);
				//lastMessageStamp.TryRemove(s,out int value);
				OnDisconnect?.BeginInvoke(connection, new ClientDisconnectEventArgs(),(x)=> { },null);
			}
		}
		private void RaiseOnMessage(string s,ClientMessageEventArgs e)
		{
			lock (list)
			{
				var connection = this[s];
				//Console.WriteLine($"message {connection.AliasName}->{d.RawString}");
				//lastMessageStamp[s] = Environment.TickCount;
				OnMessage?.BeginInvoke(connection, e, (x) => { }, null);
			}
		}
		private void CheckClientAlive()
		{
			int count = 0;
			while (true)
			{
				if (count++ > 100)
				{
					count = 0;
					int nowTime = Environment.TickCount;
					foreach(var c in list)
					{
						var connection=c.Value;
						if (connection == null)
						{
							throw new Exception("集合中存在空对象");
						}
						if (nowTime - lastMessageStamp[connection.Ip] > 20000)
						{
							connection.Client.Disconnect();
							//当上一行代码实现了关闭，则注释此行 RaiseOnDisconnect(connection.Ip);
							break;
						}
					}
				}
			}
		}
		private void Disconnect(TcpConnection client)
		{
			client.Disconnect();
		}




		public void StartListening()
		{
			isListening = true;
			server.Start();
		}
		public void StopListening()
		{
			if (!isListening) return;
			isListening = false;
			server.Stop();
		}

		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if(server!=null)StopListening();
				}
				server = null;
				disposedValue = true;
			}
		}


		public void Dispose()
		{
			// 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
			Dispose(true);
		}
		#endregion

	}

	[Serializable]
	public class MsgInvalidException : Exception
	{
		public MsgInvalidException() { }
		public MsgInvalidException(string message) : base(message) { }
		public MsgInvalidException(string message, Exception inner) : base(message, inner) { }
		protected MsgInvalidException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
