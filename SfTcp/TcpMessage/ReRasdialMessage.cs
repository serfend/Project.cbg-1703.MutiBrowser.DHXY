using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp.TcpMessage
{
	public class CmdReRasdialMessage : ITcpMessage
	{
		private string title = TcpMessageEnum.CmdReRasdial;

		public string Title { get => title; set => title = value; }
	}
	public class RpReRasdialMessage : ITcpMessage
	{
		private string title = TcpMessageEnum.RpReRasdial;

		public string Title { get => title; set => title = value; }
	}
}