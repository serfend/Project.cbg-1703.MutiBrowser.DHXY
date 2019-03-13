using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpMsgInvalidMessage:ITcpMessage
	{
		private string title =TcpMessageEnum.RpMsgInvalid;
		private string error;

		public RpMsgInvalidMessage(string error)
		{
			this.Error = error;
		}

		public string Title { get => title; set => title = value; }
		public string Error { get => error; set => error = value; }
	}
}
