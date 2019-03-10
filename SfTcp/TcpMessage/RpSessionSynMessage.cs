using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpSessionSynMessage:ITcpMessage
	{
		private string title =TcpMessageEnum.RpSessionSyn;
		private string session;

		public RpSessionSynMessage(string session)
		{
			this.session = session;
		}

		public string Title { get => title; set => title = value; }
		public string Session { get => session; set => session = value; }
	}
}
