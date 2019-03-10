using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp.TcpMessage
{
	public class RpInitCompletedMessage:ITcpMessage
	{
		private string title = TcpMessageEnum.RpInitCompleted;

		public string Title { get => title; set => title = value; }
	}
}
