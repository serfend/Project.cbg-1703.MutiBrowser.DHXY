﻿
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
		private Cowboy.Sockets.AsyncTcpSocketServer server;
		public ILog Log { set; get; }
		public int Port { get => port; set => port = value; }

		public TcpConnection this[string key] { get => list[key];set => list[key] = value; }
		private int port;
		public event ClientConnect OnConnect;
		public event ClientDisconnect OnDisconnect;
		public event ClientMessage OnMessage;
		private Dictionary<string, TcpConnection> list=new Dictionary<string, TcpConnection>();
		private ConcurrentDictionary<string, int> lastMessageStamp=new ConcurrentDictionary<string, int>();
		private bool isListening;
		/// <summary>
		/// 用于定时检查终端，并释放长时间无通讯的终端
		/// </summary>
		private Thread checkClientAlive;
		public TcpServer(int port)
		{
			server = new Cowboy.Sockets.AsyncTcpSocketServer(port, new SimpleMessageDispatcher() {
				OnConnect = (s) => {
					var connection = new TcpConnection(s, s.RemoteEndPoint.ToString(), "null");
					list.Add(connection.Ip, connection);
					lastMessageStamp.AddOrUpdate(connection.Ip, Environment.TickCount,(key,value)=> {
						return Environment.TickCount;
					});
					OnConnect?.Invoke(connection, new ClientConnectEventArgs());
				},
				OnMessage = RaiseOnMessage,
				OnDisconnect = RaiseOnDisconnect
			});
			checkClientAlive = new Thread(CheckClientAlive) { IsBackground=true};
			checkClientAlive.Start();
			server.Listen();
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
		private void CheckClientAlive()
		{
			int count = 0;
			while (true)
			{
				Thread.Sleep(1000);
				if (count++ > 20)
				{
					count = 0;
					int nowTime = Environment.TickCount;
					foreach(var c in list)
					{
						if (nowTime - lastMessageStamp[c.Value.Ip] > 20000)
						{
							c.Value.Disconnect();
							RaiseOnDisconnect(c.Key);
							continue;
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
