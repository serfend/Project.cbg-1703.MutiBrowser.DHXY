using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp.TcpMessage
{
	public class MsgSynSessionMessage : ITcpMessage
	{
		private string title = TcpMessageEnum.MsgSynSession;

		private List<SynSessionItem> list;
		public MsgSynSessionMessage(List<SynSessionItem> list)
		{
			this.List = list;
		}

		public List<SynSessionItem> List { get => list; set => list = value; }
		public string Title { get => title; set => title = value; }
	}
	public class SynSessionItem
	{
		private string aliasName;
		private string loginSession;

		public SynSessionItem(string aliasName, string loginSession)
		{
			this.aliasName = aliasName;
			this.loginSession = loginSession;
		}

		public string LoginSession { get => loginSession; set => loginSession = value; }
		public string AliasName { get => aliasName; set => aliasName = value; }
	}
}
