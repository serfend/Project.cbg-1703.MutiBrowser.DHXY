using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpBuildBillMessage : ITcpMessage
	{
		private string title = TcpMessageEnum.RpBuildBill;
		private string content;

		public RpBuildBillMessage(string content)
		{
			this.content = content;
		}

		public string Title { get => title; set => title = value; }
		public string Content { get => content; set => content = value; }
	}
}
