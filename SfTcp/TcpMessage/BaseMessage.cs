using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public interface ITcpMessage
	{
		string Title { set; get; }
	}
	public class BaseMessage : ITcpMessage
	{
		private string title;

		public string Title { get => title; set => title = value; }
	}
	public class NormalMessage : ITcpMessage
	{
		private string content;
		private string title;

		public NormalMessage( string title, string info)
		{
			this.content = info;
			this.title = title;
		}

		public string Content { get => content; set => content = value; }
		public string Title { get => title; set => title=value; }
	}
	public class StatusMessage : ITcpMessage
	{
		private string status;
		private string title=TcpMessageEnum.RpStatus;

		public StatusMessage(string status)
		{
			this.status = status;
		}

		public string Status { get => status; set => status = value; }
		public string Title { get => title; set => title = value; }
	}
}
