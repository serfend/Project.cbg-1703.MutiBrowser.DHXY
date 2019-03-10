using System;

namespace SfTcp.TcpServer
{
	public delegate void ClientDisconnect(object sender, ClientDisconnectEventArgs e);
	public class ClientDisconnectEventArgs : EventArgs
	{
	}
}