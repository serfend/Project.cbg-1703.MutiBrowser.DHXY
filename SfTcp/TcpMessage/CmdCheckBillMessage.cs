using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class CmdCheckBillMessage:ITcpMessage
	{
		private string title = TcpMessageEnum.CmdCheckBillUrl;
		public enum Action
		{
			submit,
			show
		}
		private Action act;
		private double price = 999;
		private double assumePrice;
		private string url;

		public CmdCheckBillMessage(Action act, string url, double assumePrice=0, double price=999)
		{
			this.act = act;
			this.price = price;
			this.assumePrice = assumePrice;
			this.url = url;
		}

		public string Title { get => title; set => title = value; }
		public double Price { get => price; set => price = value; }
		public double AssumePrice { get => assumePrice; set => assumePrice = value; }
		public string Url { get => url; set => url = value; }
		public Action Act { get => act; set => act = value; }
	}
}
