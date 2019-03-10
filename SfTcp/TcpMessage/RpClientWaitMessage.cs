using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class RpClientWaitMessage:ITcpMessage
	{
		private string title = TcpMessageEnum.RpClientWait;

		public string Title { get => title; set => title = value; }
		/// <summary>
		/// 全局延时
		/// </summary>
		public int G { get => g; set => g = value; }
		/// <summary>
		/// 采集耗时
		/// </summary>
		public int H { get => h; set => h = value; }
		/// <summary>
		/// 其他附加信息
		/// </summary>
		public int V { get => v; set => v = value; }

		private int g;
		private int h;
		private int v;

		public RpClientWaitMessage(int g, int h, int v)
		{
			this.g = g;
			this.h = h;
			this.v = v;
		}
	}
}
