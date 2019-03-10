using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp.TcpMessage
{
	public class RpNameModefiedMessage : ITcpMessage
	{
		private string title = TcpMessageEnum.RpNameModefied;
		private string newName;
		private bool askForSynInit;

		public RpNameModefiedMessage(string newName,bool askForSynInit)
		{
			this.newName = newName;
			this.AskForSynInit = askForSynInit;
		}

		public string Title { get => title; set => title=value; }
		public string NewName { get => newName; set => newName = value; }
		public bool AskForSynInit { get => askForSynInit; set => askForSynInit = value; }
	}
}
