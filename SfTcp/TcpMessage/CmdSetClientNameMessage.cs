using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class CmdSetClientNameMessage : ITcpMessage
	{
		private string title=TcpMessageEnum.CmdSetClientName;
		private string newName;

		public CmdSetClientNameMessage(string newName)
		{
			this.NewName = newName;
		}

		public string Title { get => title; set => title=value; }
		public string NewName { get => newName; set => newName = value; }
	}
}
