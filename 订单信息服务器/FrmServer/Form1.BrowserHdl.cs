using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 订单信息服务器
{
	public partial class Form1 : Form
	{

		private Dictionary<string, string> BrowserIp = new Dictionary<string, string>();//浏览器进程对应终端ip

		/// <summary>
		/// 将订单信息发送至下单服务器
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="cmdInfo"></param>
		private void SendCmdToBrowserClient(string serverName, string cmdInfo)
		{
			try
			{
				if (!BrowserIp.ContainsKey(serverName))
				{
					AppendLog(serverName + " 对应的下单浏览器进程未启动");
					return;
				}
				else
				{
					var targetBrowser = BrowserIp[serverName];
					var client = serverManager[targetBrowser];
					if (client == null)
					{
						AppendLog("未找到目标浏览器:"+serverName);
					}else
					client.Send(cmdInfo);
				}
			}
			catch (Exception ex)
			{
				AppendLog("提交到浏览器异常:"+ex.Message);
			}
		}
	}
}
