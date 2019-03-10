using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class CmdServerRunMessage : ITcpMessage
	{
		private string title =TcpMessageEnum.CmdServerRun;
		public string Title { get => title; set => title=value; }
	}
}
