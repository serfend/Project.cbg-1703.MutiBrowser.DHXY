
using SfBaseTcp.Net.Sockets;
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
		SfBaseTcp.Net.Sockets.TCPClient client;
		public TcpClientConnection(string ip,int port)
		{
			this.ip = ip;
			this.port = port;
			this.client = new TCPClient();

			client.ConnectCompleted += Client_ConnectCompleted;
			client.DisconnectCompleted += Client_DisconnectCompleted;
			client.ReceiveCompleted += Client_ReceiveCompleted;
		}

		private void Client_ReceiveCompleted(object sender, SocketEventArgs e)
		{
			OnMessaged?.Invoke(sender, new ClientMessageEventArgs(e.Data));
		}

		private void Client_DisconnectCompleted(object sender, SocketEventArgs e)
		{
			OnDisconnected?.Invoke(sender, new ServerDisconnectEventArgs());
		}

		private void Client_ConnectCompleted(object sender, SocketEventArgs e)
		{
			OnConnected?.Invoke(sender, new ServerConnectEventArgs());
		}




		public string IP => "to server";

		public bool Connected => client.IsConnected;

		public event ServerMessage OnMessaged;
		public event ConnectToServer OnConnected;
		public event DisconnectFromServer OnDisconnected;

		public bool Connect()
		{
			if (client!=null&&client.IsConnected) Disconnect();
			client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
			return true;
		}

		public void Disconnect()
		{
			client.Disconnect();
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
