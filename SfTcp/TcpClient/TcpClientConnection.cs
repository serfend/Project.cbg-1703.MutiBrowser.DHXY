using Cowboy.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SfTcp.TcpClient
{
	public class TcpClientConnection : ITcpClientSender,IDisposable
	{
		private readonly string ip;
		private readonly int port;
		TcpSocketClient client;
		public TcpClientConnection(string ip,int port)
		{
			this.ip = ip;
			this.port = port;
			this.client = new TcpSocketClient(IPAddress.Parse(ip), port);
			client.ServerConnected += Client_ServerConnected;
			client.ServerDataReceived += Client_ServerDataReceived;
			client.ServerDisconnected += Client_ServerDisconnected;
		}

		private void Client_ServerDisconnected(object sender, TcpServerDisconnectedEventArgs e)
		{
			OnDisconnected?.Invoke(sender, new ServerDisconnectEventArgs());
		}

		private void Client_ServerDataReceived(object sender, TcpServerDataReceivedEventArgs e)
		{
			var data = new byte[e.DataLength];
			Buffer.BlockCopy(e.Data, e.DataOffset, data, 0, e.DataLength);
			OnMessaged?.Invoke(sender, new ServerMessageEventArgs(data));
		}

		private void Client_ServerConnected(object sender, TcpServerConnectedEventArgs e)
		{
			OnConnected?.Invoke(sender, new ServerConnectEventArgs());
		}



		public string IP => "to server";

		public bool Connected => client.State==TcpSocketConnectionState.Connected;

		public event ServerMessage OnMessaged;
		public event ConnectToServer OnConnected;
		public event DisconnectFromServer OnDisconnected;

		public bool Connect()
		{
			client.Connect();
			return true;
		}

		public void Disconnect()
		{
			client.Close();
		}

		public bool Send(byte[] data)
		{
			client.Send(data);
			return true;
		}

		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (client != null) client.Dispose();
				}
				client = null;

				disposedValue = true;
			}
		}


		// 添加此代码以正确实现可处置模式。
		public void Dispose()
		{
			// 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
			Dispose(true);
			
		}
		#endregion
	}
}
