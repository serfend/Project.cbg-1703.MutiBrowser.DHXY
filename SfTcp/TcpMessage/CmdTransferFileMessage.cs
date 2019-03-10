using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class CmdTransferFileMessage : ITcpMessage
	{
		private string title = TcpMessageEnum.CmdTransferFile;
		public string Title { get => title; set => title = value; }
	}
}
