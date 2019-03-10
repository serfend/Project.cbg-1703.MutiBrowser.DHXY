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
using SfTcp.TcpClient;
namespace 远程连接测试
{
	public partial class Form1 : Form
	{
		private TcpClient client;
		private long lastHeartBeatTimeStamp;
		public Form1()
		{
			client = new TcpClient("127.0.0.1", 8009) { 

			};
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{

		}

	}
}
