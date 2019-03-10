using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp.TcpMessage
{
	public class MsgHeartBeatMessage:ITcpMessage
	{
		private string title = TcpMessageEnum.MsgHeartBeat;

		public string Title { get => title; set => title = value; }
	}
}
