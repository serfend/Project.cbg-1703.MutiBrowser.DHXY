using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp.TcpMessage
{
	public class CmdSubCloseMessage:ITcpMessage
	{
		private string title = TcpMessageEnum.CmdSubClose;

		public string Title { get => title; set => title = value; }
	}
}
