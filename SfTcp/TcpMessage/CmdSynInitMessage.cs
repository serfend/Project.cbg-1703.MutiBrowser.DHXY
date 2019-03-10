using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp.TcpMessage
{
	public class CmdSynInitMessage:ITcpMessage
	{
		private string title= TcpMessageEnum.CmdSynInit;
		private int interval;
		private int timeout;
		private string innerTargetUrl;
		private double assumePriceRate;

		public CmdSynInitMessage(int interval, int timeout, string innerTargetUrl, double assumePriceRate)
		{
			this.Interval = interval;
			this.timeout = timeout;
			this.innerTargetUrl = innerTargetUrl;
			this.assumePriceRate = assumePriceRate;
		}

		public string Title { get => title; set => title = value; }
		public int Timeout { get => timeout; set => timeout = value; }
		public string InnerTargetUrl { get => innerTargetUrl; set => innerTargetUrl = value; }
		public double AssumePriceRate { get => assumePriceRate; set => assumePriceRate = value; }
		public int Interval { get => interval; set => interval = value; }
	}
}
