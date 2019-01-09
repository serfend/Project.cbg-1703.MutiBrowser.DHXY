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
			client = new SfTcpClient("106.75.123.187", 31030) { 
				RecieveMessage = (tcp, msg) => {
					this.Invoke((EventHandler)delegate {
						Console.WriteLine(msg);
					});
				},
				Disconnected = (tcp) => {
					this.Invoke((EventHandler)delegate
					{
						textBox2.AppendText("\n已断开连接");
					});
				}
			};
			client.Send(GetBytesRawstring("528f4c7d4e209061aedf0ae308004500003454c8400040061358c0a82bf56a4b7bbbcf66793636e047b4000000008002447090ca0000020405b40103030801010402"));
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			client.Send(System.Text.Encoding.UTF8.GetBytes(textBox2.Text));
		}
		private byte[] GetBytesRawstring(string raw)
		{
			//
			byte[] result = new byte[raw.Length/2];
			for(int i = 0; i < raw.Length; i += 2)
			{
				if (raw[i] > 57)
				{

				}
				else
				{

				}
				result[i] = (byte)(raw[i]+ raw[i+1]);
			}
			return result;
		}
	}
}
