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
			client = new SfTcpClient("111.225.9.110", 16555) { 
				RecieveMessage = (tcp, msg) => {
					this.Invoke((EventHandler)delegate {
						long now = HttpUtil.TimeStamp- lastHeartBeatTimeStamp;
						textBox2.AppendText($"{now}ms : {msg}\n");
					});
				},
				Disconnected = (tcp) => {
					this.Invoke((EventHandler)delegate
					{
						textBox2.AppendText("\n已断开连接");
					});
				}
			};
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			lastHeartBeatTimeStamp = HttpUtil.TimeStamp;
			client.Send("ping","");
		}

	}
}
