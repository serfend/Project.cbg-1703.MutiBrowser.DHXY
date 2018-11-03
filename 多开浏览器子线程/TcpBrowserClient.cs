using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 多开浏览器子线程
{
	public class TcpBrowserClient:SfTcp.SfTcpClient
	{
		/// <summary>
		/// 方便服务器识别当前终端的类别
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public override bool Send(string info)
		{
			return base.Send("<mobile>"+info);
		}
	}
}
