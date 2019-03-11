using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpFailBillMessage : ITcpMessage
	{
		private string title = TcpMessageEnum.RpFailBill;
		private string content;

		public RpFailBillMessage(string content)
		{
			this.content = content;
		}

		public string Title { get => title; set => title = value; }
		public string Content { get => content; set => content = value; }
	}
}
