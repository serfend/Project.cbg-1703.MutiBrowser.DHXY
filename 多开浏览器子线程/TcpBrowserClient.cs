using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 多开浏览器子线程
{
	public class TcpBrowserClient:SfTcp.SfTcpClient
	{
		public TcpBrowserClient():base("127.0.0.1",8009) { }
	}
}
