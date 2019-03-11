using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpSuccessBillMessage : ITcpMessage
	{
		private string title = TcpMessageEnum.RpSuccessBill;
		private string content;

		public RpSuccessBillMessage(string content)
		{
			this.content = content;
		}

		public string Title { get => title; set => title = value; }
		public string Content { get => content; set => content = value; }
	}
}
