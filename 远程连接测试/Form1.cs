using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DotNet4.Utilities.UtilCode;
using SfBaseTcp.Net.Sockets;
namespace 远程连接测试
{
	public partial class Form1 : Form
	{
		private TCPClient client;
		private TCPListener server;
		private void DisableBtn()
		{
			button1.Enabled = false;
			button2.Enabled = false;
		}
		private void AsClient()
		{
			DisableBtn();
			client = new TCPClient();
			client.ReceiveCompleted += Client_ReceiveCompleted;
			client.SendCompleted += Client_SendCompleted;
			client.ConnectCompleted += Client_ConnectCompleted;
			client.DisconnectCompleted += Client_DisconnectCompleted;
			client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8009));
		}
		private int lastCount;
		private void AsServer()
		{
			DisableBtn();
			server = new TCPListener() { Port=8009};
			server.AcceptCompleted += Server_AcceptCompleted;
			server.SendCompleted += Server_SendCompleted;
			server.ReceiveCompleted += Server_ReceiveCompleted;
			server.DisconnectCompleted += Server_DisconnectCompleted;
			server.Start();
			var t=new Thread(() => {
				while (true)
				{
					Thread.Sleep(2000);
					int nowCount = server.Count;
					if (nowCount != lastCount)
					{
						AppendText($"客户端数量:{nowCount}");
						lastCount = nowCount;
					};
				}
			}) { IsBackground = true };
			t.Start();
		}
		public Form1()
		{
			InitializeComponent();
		}
		private void AppendText(string raw)
		{
			this.Invoke((EventHandler)delegate {
				textBox2.Text = $"{raw}\r\n{textBox2.Text}";
				if (textBox2.Text.Length > 10000) textBox2.Text = "";
			});
		}


		private  void Server_DisconnectCompleted(object sender, SocketEventArgs e)
		{
			AppendText($"Disconnect:{e.Socket.RemoteEndPoint.ToString()}");
		}

		private  void Server_ReceiveCompleted(object sender, SocketEventArgs e)
		{
			var raw = Encoding.UTF8.GetString(e.Data);
			AppendText($"ReceiveComplete:{e.Socket.RemoteEndPoint.ToString()}\n{raw}");
			e.Socket.Send(Encoding.UTF8.GetBytes("cmd"));
		}

		private  void Server_SendCompleted(object sender, SocketEventArgs e)
		{
			AppendText($"SendCompleted:{e.Socket.RemoteEndPoint.ToString()}");
		}

		private  void Server_AcceptCompleted(object sender, SocketEventArgs e)
		{
			AppendText($"Accept:{e.Socket.RemoteEndPoint.ToString()}");
		}

		private  void Client_DisconnectCompleted(object sender, SocketEventArgs e)
		{
			AppendText("断开连接完成");
		}
		private Thread clientMsg;
		private  void Client_ConnectCompleted(object sender, SocketEventArgs e)
		{
			AppendText("连接完成");
			clientMsg= new Thread(() => {
				while (true)
				{
					client.Send(Encoding.UTF8.GetBytes(textBox3.Text));
					Thread.Sleep(200);
				}
			});
			clientMsg.IsBackground = true;
			clientMsg.Start();
		}

		private  void Client_SendCompleted(object sender, SocketEventArgs e)
		{
			AppendText($"发送完成");
		}

		private  void Client_ReceiveCompleted(object sender, SocketEventArgs e)
		{
			var raw = Encoding.UTF8.GetString(e.Data);
			AppendText($"接收信息:{raw}");
			new Thread(()=> {
				Thread.Sleep(100);
				e.Socket.Send(Encoding.UTF8.GetBytes(textBox3.Text));
			}).Start();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			AsClient();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			client?.Send(Encoding.UTF8.GetBytes(textBox3.Text));
		}

		private void button2_Click(object sender, EventArgs e)
		{
			AsServer();
		}
	}
}
