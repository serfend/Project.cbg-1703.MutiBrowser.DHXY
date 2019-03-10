using System;

namespace SfTcp.TcpServer
{

	public delegate void ClientConnect(object sender, ClientConnectEventArgs e);
	public class ClientConnectEventArgs : EventArgs
	{

	}


}