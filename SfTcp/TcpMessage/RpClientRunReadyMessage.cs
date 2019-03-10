using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpClientRunReadyMessage:ITcpMessage
	{
		private string title = TcpMessageEnum.RpClientRunReady;

		public string Title { get => title; set => title = value; }
	}
}
