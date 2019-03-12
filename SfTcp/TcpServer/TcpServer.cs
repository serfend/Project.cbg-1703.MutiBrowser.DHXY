
using Cowboy.Sockets;
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
		private Cowboy.Sockets.TcpSocketServer server;
		public ILog Log { set; get; }
		public int Port { get => port; set => port = value; }

		public TcpConnection this[string key] { get => list[key];set => list[key] = value; }
		private int port;
		public event ClientConnect OnConnect;
		public event ClientDisconnect OnDisconnect;
		public event ClientMessage OnMessage;
		public event ClientHttpMessage OnHttpMessage;
		private Dictionary<string, TcpConnection> list=new Dictionary<string, TcpConnection>();
		private ConcurrentDictionary<string, int> lastMessageStamp=new ConcurrentDictionary<string, int>();
		private bool isListening;
		/// <summary>
		/// 用于定时检查终端，并释放长时间无通讯的终端
		/// </summary>
		private Thread checkClientAlive;
		public TcpServer(int port)
		{
			var cfg = new TcpSocketServerConfiguration() {
				ReceiveBufferSize=1024000
			};
			server = new TcpSocketServer(port, cfg);
			server.ClientConnected += Server_ClientConnected;
			server.ClientDataReceived += Server_ClientDataReceived;
			server.ClientDisconnected += Server_ClientDisconnected;

			checkClientAlive = new Thread(CheckClientAlive) { IsBackground=true};
			checkClientAlive.Start();
			server.Listen();
		}

		private void Server_ClientDisconnected(object sender, TcpClientDisconnectedEventArgs e)
		{
			RaiseOnDisconnect(e.Session.RemoteEndPoint.ToString());
		}
		private volatile int anyClientMessage=0;
		private readonly ConcurrentDictionary<string, StringBuilder> clientMessageCache = new ConcurrentDictionary<string, StringBuilder>();
		private void Server_ClientDataReceived(object sender, TcpClientDataReceivedEventArgs e)
		{
			while (anyClientMessage < 0)
			{
				Thread.Sleep(0);
			}
			anyClientMessage = 10;//用于当消息未能正常处置时，等待交由回收线程处理
			var clientIp = e.Session.RemoteEndPoint.ToString();
			bool haveCache = clientMessageCache.ContainsKey(clientIp);

			byte[] raw = new byte[e.DataLength + 1];
			Buffer.BlockCopy(e.Data, e.DataOffset-1, raw, 0, e.DataLength+1);
			ClientMessageEventArgs msg;
			if (haveCache) msg = new ClientMessageEventArgs(clientMessageCache[clientIp], raw);
			else
				msg = new ClientMessageEventArgs(raw);
			msg.AnalysisRaw();//立即解析，若解析失败，表示为部分消息，则继续等待
			if (msg.Error)
			{
				clientMessageCache[clientIp] = new StringBuilder(msg.RawString);//消息处理无效，则建立缓存
			}
			else
			{
				RaiseOnMessage(clientIp, msg);
				if(haveCache)clientMessageCache.TryRemove(clientIp,out StringBuilder x);//消息处理无误，则消化缓存
			}
			
		}

		private void Server_ClientConnected(object sender, TcpClientConnectedEventArgs e)
		{
			var ip = e.Session.RemoteEndPoint.ToString();
			var connection = new TcpConnection(e.Session, ip, "null");
			list.Add(connection.Ip, connection);
			lastMessageStamp.AddOrUpdate(connection.Ip, Environment.TickCount, (key, value) =>
			{
				return Environment.TickCount;
			});
			OnConnect?.Invoke(connection, new ClientConnectEventArgs());
		}

		private void RaiseOnDisconnect(string s)
		{
			if (!list.ContainsKey(s)) return;
			var connection = this[s];
			list.Remove(s);
			lastMessageStamp.TryRemove(s,out int value);
			OnDisconnect?.Invoke(connection, new ClientDisconnectEventArgs());
		}
		private void RaiseOnMessage(string s,ClientMessageEventArgs e)
		{
			var connection = this[s];
			//Console.WriteLine($"message {connection.AliasName}->{d.RawString}");
			lastMessageStamp[s] = Environment.TickCount;
			OnMessage?.Invoke(connection, e);
		}
		/// <summary>
		/// 检查所有消息中是否有http消息
		/// </summary>
		private void CheckAnyMessageUnhandle()
		{
			anyClientMessage = -1;
			foreach (var msg in clientMessageCache)
			{
				if (list.ContainsKey(msg.Key))
				{
					var hdlMsg = TcpHttpMessage.CheckTcpHttpMessage(msg.Value.ToString());
					if (hdlMsg != null)
					{
						var connection = list[msg.Key];
						OnHttpMessage?.BeginInvoke(connection, new ClientHttpMessageEventArgs(hdlMsg, connection), (x) => { }, null);
					}
				}
			}
			clientMessageCache.Clear();
			anyClientMessage = 10;
		}
		private void CheckClientAlive()
		{
			int count = 0;
			while (true)
			{
				Thread.Sleep(100);
				if (anyClientMessage>0)
				{
					anyClientMessage--;
				}
				else
				{
					CheckAnyMessageUnhandle();
				}
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
							connection.Client.Shutdown();
							RaiseOnDisconnect(connection.Ip);
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
			server.Listen();
		}
		public void StopListening()
		{
			if (!isListening) return;
			isListening = false;
			server.Shutdown();
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
	public class AsyncServerMessageDispatcher :IAsyncTcpSocketServerMessageDispatcher
	{
		public Action<AsyncTcpSocketSession> OnConnect;
		public Action<string, ClientMessageEventArgs> OnMessage;
		public Action<string> OnDisconnect;
		public async Task OnSessionStarted(AsyncTcpSocketSession session)
		{
			//Console.WriteLine(string.Format("TCP session has connected {0}.", session));
			OnConnect?.Invoke(session);
			await Task.CompletedTask;
		}

		public async Task OnSessionDataReceived(AsyncTcpSocketSession session, byte[] data, int offset, int count)
		{
			var raw = new byte[count];
			Buffer.BlockCopy(data, offset, raw, 0, count);
			OnMessage?.Invoke(session.RemoteEndPoint.ToString(), new ClientMessageEventArgs(raw));
			await Task.CompletedTask;
		}

		public async Task OnSessionClosed(AsyncTcpSocketSession session)
		{
			//Console.WriteLine(string.Format("TCP session {0} has disconnected.", session));
			OnDisconnect?.Invoke(session.RemoteEndPoint.ToString());
			await Task.CompletedTask;
		}
	}
}
