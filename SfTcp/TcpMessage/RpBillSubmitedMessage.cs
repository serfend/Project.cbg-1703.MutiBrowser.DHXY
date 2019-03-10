using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpBillSubmitedMessage:ITcpMessage
	{
		private string title = TcpMessageEnum.RpBillSubmited;
		public enum State
		{
			New,
			Success,
			Fail
		}
		private State status;
		private string r;
		public RpBillSubmitedMessage(State status,string reason=null)
		{
			this.status = status;
			r = reason;
		}

		public State Status { get => status; set => status = value; }
		public string Title { get => title; set => title = value; }
		/// <summary>
		/// 当状态为失败时，详细的原因
		/// </summary>
		public string R { get => r; set => r = value; }
	}
}
