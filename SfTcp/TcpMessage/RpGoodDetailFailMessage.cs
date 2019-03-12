using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpGoodDetailFailMessage:ITcpMessage
	{
		private string title = TcpMessageEnum.RpGoodDetailFail;
		private string content;

		public RpGoodDetailFailMessage(string content)
		{
			this.content = content;
		}

		public string Title { get => title; set => title = value; }
		public string Content { get => content; set => content = value; }
	}
}
