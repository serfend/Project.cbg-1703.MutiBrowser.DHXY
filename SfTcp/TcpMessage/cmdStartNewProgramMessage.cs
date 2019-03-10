using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp.TcpMessage
{
	public class CmdStartNewProgramMessage:ITcpMessage
	{
		private string title= TcpMessageEnum.CmdStartNewProgram;

		public string Title { get => title; set => title = value; }
	}
}
