using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp.TcpMessage
{
	public class CmdServerRunScheduleMessage:ITcpMessage
	{
		private string title = TcpMessageEnum.CmdServerRunSchedule;
		private int taskStamp;

		public CmdServerRunScheduleMessage(int taskStamp)
		{
			this.taskStamp = taskStamp;
		}

		public string Title { get => title; set => title = value; }
		public int TaskStamp { get => taskStamp; set => taskStamp = value; }
	}
}
