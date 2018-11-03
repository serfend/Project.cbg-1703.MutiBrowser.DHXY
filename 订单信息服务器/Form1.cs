﻿using DotNet4.Utilities.UtilCode;
using SfTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 订单信息服务器
{
	public partial class Form1 : Form
	{
		private TcpServerManager t;
		private ListViewItem GetItem(string ip)
		{
			foreach(var item in LstConnection.Items) {
				if (item is ListViewItem it)
				{
					if (it.SubItems[1].Text== ip) return it;
				}
			}
			return null;
		}
		public Form1()
		{
			InitializeComponent();
			t=new TcpServerManager() { NormalMessage=(s,x)=> {
				this.Invoke((EventHandler)delegate
				{
					var targetItem = GetItem(s.Ip);
					if (targetItem != null)
					{
						if (x.Contains("heartBeat"))
						{ }
						else if (x.Contains("<connectCmdRequire>"))
						{
							targetItem.SubItems[3].Text = "初始化";
							targetItem.SubItems[0].Text =HttpUtil.GetElementInItem(x, "connectCmdRequire");
							//s.Send("<connectCmd>test</connectCmd>");
						}
						else
						{
							AppendLog("新消息[" + s.client.Client.RemoteEndPoint.ToString() + "] " + x);
							targetItem.SubItems[3].Text = x;
						}
					}
					
				});
			} ,
			ServerConnected=(x)=> {
				this.Invoke((EventHandler)delegate {
					AppendLog("已连接:" + x.Ip);
					var info = new string[4];
					info[1] = x.IsLocal?"主机":"终端";
					info[2] = x.Ip;
					info[0] = x.clientName;
					LstConnection.Items.Add(new ListViewItem(info));

					var welcome = new Task(()=> {
						Thread.Sleep(3000);
						x.Send("<welcome>" + DateTime.Now + "</welcome>");
					});
					welcome.Start();
				});
			},
			ServerDisconnected = (x) => {
					this.Invoke((EventHandler)delegate {
						AppendLog("已断开:" + x.Ip);
						for(int i = 0; i < LstConnection.Items.Count; i++)
							if (LstConnection.Items[i].SubItems[2].Text == x.Ip)
								LstConnection.Items.RemoveAt(i);
					});
			},
				HttpRequest = (x, s) => {
					s.Response(string.Format("<h1>Hey,测试服务器已开启</h1><br><p>当前连接数:{0}</p>",LstConnection.Items.Count));
				}
			};
			LstConnection.AfterLabelEdit += CheckIfUserEditName;
		}

		private void CheckIfUserEditName(object sender, LabelEditEventArgs e)
		{
			MessageBox.Show(e.Item.ToString());
		}

		public void AppendLog(string info)
		{
			OpLog.AppendText("\n");
			OpLog.AppendText(string.Format("{0}>>{1}",DateTime.Now,info));
			
		}

		private void CmdServerOn_Click(object sender, EventArgs e)
		{
			try
			{
				var nowSelect = LstConnection.SelectedItems[0].SubItems[2].Text;
				var tcp = t[nowSelect];
				tcp.Send(IpSender.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void LstConnection_SelectedIndexChanged(object sender, EventArgs e)
		{

		}
	}
}
