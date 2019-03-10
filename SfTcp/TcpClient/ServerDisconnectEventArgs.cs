using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpClient
{
	public delegate void DisconnectFromServer(object sender, ServerDisconnectEventArgs e);
	public class ServerDisconnectEventArgs:EventArgs
	{
	}
}
