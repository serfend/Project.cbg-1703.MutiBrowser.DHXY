using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp
{
	public interface ITcpClientSender
	{
		event TcpClient.ServerMessage OnMessaged;
		event TcpClient.ConnectToServer OnConnected;
		event TcpClient.DisconnectFromServer OnDisconnected;
		bool Send(byte[] data);
		void Disconnect();
		string IP { get; }
		bool Connected { get; }
		bool Connect();
	}
	public interface ITcpServerSender
	{
		event TcpServer.ClientMessage OnMessaged;
		event TcpServer.ClientConnect OnConnected;
		event TcpServer.ClientDisconnect OnDisconnected;
		bool Send(byte[] data);
		void Disconnect();
		string IP { get; }
		bool Connected { get; }
		bool Connect();
	}
}
