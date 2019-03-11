
using Cowboy.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpServer
{
	public class TcpServer:IDisposable
	{
		private Cowboy.Sockets.AsyncTcpSocketServer server;
		public ILog Log { set; get; }
		public int Port { get => port; set => port = value; }

		public TcpConnection this[string key] { get => list[key];set => list[key] = value; }
		private int port;
		public event ClientConnect OnConnect;
		public event ClientDisconnect OnDisconnect;
		public event ClientMessage OnMessage;
		private Dictionary<string, TcpConnection> list=new Dictionary<string, TcpConnection>();
		private bool isListening;

		public TcpServer(int port)
		{
			server = new Cowboy.Sockets.AsyncTcpSocketServer(port, new SimpleMessageDispatcher() {
				OnConnect = (s) => {
					var connection = new TcpConnection(s, s.RemoteEndPoint.ToString(), "null");
					list.Add(connection.Ip,connection);
					OnConnect?.Invoke(connection, new ClientConnectEventArgs());
				},
				OnMessage = (s, d)=>{
					var connection = this[s];
					//Console.WriteLine($"message {connection.AliasName}->{d.RawString}");
					OnMessage?.Invoke(connection, d);
				},
				OnDisconnect = (s) => {
					var connection = this[s];
					OnDisconnect?.Invoke(connection, new ClientDisconnectEventArgs());
				}
			});
			server.Listen();
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
	public class SimpleMessageDispatcher :IAsyncTcpSocketServerMessageDispatcher
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
