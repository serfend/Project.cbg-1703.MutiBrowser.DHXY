using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpCheckBillMessage:ITcpMessage
	{
		private string title =TcpMessageEnum.RpCheckBill;
		private string billInfo;

		public RpCheckBillMessage(string billInfo)
		{
			this.billInfo = billInfo;
		}

		public string Title { get => title; set => title = value; }
		public string BillInfo { get => billInfo; set => billInfo = value; }
	}
}
