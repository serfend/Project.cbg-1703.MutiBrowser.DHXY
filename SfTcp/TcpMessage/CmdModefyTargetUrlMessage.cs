using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class CmdModefyTargetUrlMessage : ITcpMessage
	{
		private string title = TcpMessageEnum.CmdModefyTargetUrl;
		public string Title { get => title; set => title = value; }
		public string NewUrl { get => newUrl; set => newUrl = value; }

		private string newUrl;

		public CmdModefyTargetUrlMessage(string newUrl)
		{
			this.newUrl = newUrl;
		}
	}
}
