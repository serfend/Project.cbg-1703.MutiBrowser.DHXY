using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DotNet4.Utilities.UtilCode;
using SfTcp;
namespace 远程连接测试
{
	public partial class Form1 : Form
	{
		private SfTcpClient client;
		private long lastHeartBeatTimeStamp;
		public Form1()
		{
			client = new SfTcpClient() {
				RecieveMessage = (tcp, msg) => {
					this.Invoke((EventHandler)delegate {
						if (msg.Contains("<ping>"))
						{
							var interval = HttpUtil.TimeStamp - lastHeartBeatTimeStamp;
							textBox1.Text = interval + "ms";
						}
						else
						{
							textBox2.AppendText("\n");
							textBox2.AppendText(msg);
						};
					});
				},
				Disconnected = (tcp) => {
					this.Invoke((EventHandler)delegate
					{
						textBox2.AppendText("\n已断开连接");
					});
				}
			};
			var threadPing = new Thread(() =>
			{
				while (true)
				{
					Thread.Sleep(1000);
					lastHeartBeatTimeStamp = HttpUtil.TimeStamp;
					client.Send("ping","");
				}
			})
			{ IsBackground=true};
			threadPing.Start();
			InitializeComponent();
		}
		
	}
}
