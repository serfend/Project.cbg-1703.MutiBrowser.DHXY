
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace 订单信息服务器.Properties.Bill
{
	public class PayUser
	{
		private string userName;
		private string session;
		private List<string> hdlServer;
		private string psw;
		public PayUser(string userName, string session, string hdlServer, string psw)
		{
			this.userName = userName;
			this.session = session;
			this.hdlServer = hdlServer.Split('|').ToList<string>();
			this.psw = psw;
		}

		public string UserName { get => userName; set => userName = value; }
		public string Session { get => session; set => session = value; }
		public List<string> HdlServer { get => hdlServer; set => hdlServer = value; }
		public string Psw { get => psw; set => psw = value; }
	}
}