using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpClient
{

	public delegate void ConnectToServer(object sender, ServerConnectEventArgs e);
	public class ServerConnectEventArgs:EventArgs
	{
	}
}
