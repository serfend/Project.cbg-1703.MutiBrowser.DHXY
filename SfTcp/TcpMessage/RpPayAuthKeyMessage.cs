using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpPayAuthKeyMessage:ITcpMessage
	{
		private string title =TcpMessageEnum.RpPayAuthKey;
		private string key;

		public RpPayAuthKeyMessage(string key)
		{
			this.key = key;
		}

		public string Title { get => title; set => title = value; }
		public string Key { get => key; set => key = value; }
	}
}
