using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilReg;
using File_Transfer;
using SfTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
		private TransferFileEngine transferFileEngine;
		private struct FileWaitingClient
		{
			SfTcp.TcpServer client;
			string fileRequire;
			public TcpServer Client { get => client; set => client = value; }
			public string FileRequire { get => fileRequire; set => fileRequire = value; }
		}
		private Queue<FileWaitingClient> fileSynQueue=new Queue<FileWaitingClient>();
		private void HdlVpsFileSynRequest(string x, SfTcp.TcpServer s)
		{
			if (transferFileEngine.Sender.SendingFileQueue.Count>0 && !transferFileEngine.Sender.IsSending) {
				fileSynQueue.Enqueue(new FileWaitingClient() {FileRequire=x,Client=s });
				return;
			}
			var t = new Task(() => {

				waittingFileInfo = x;//记录命令的详细信息

				this.Invoke((EventHandler)delegate { AppendLog("准备向终端" + s.clientName + " 传输文件"); });
				InitTransferEngine();
				s.Send("<ensureFileTransfer>" + new Random().Next(0, 1000));
				transferFileEngine.Connect();//文件发送引擎

			});
			t.Start();
		}
		private void InitTransferEngine()
		{
			if (transferFileEngine != null)
			{
				transferFileEngine.Dispose();
				transferFileEngine = null;
			}
			transferFileEngine = new TransferFileEngine(TcpFiletransfer.TcpTransferEngine.Connections.Connection.EngineModel.AsServer, "", 8010);
			transferFileEngine.Connection.ConnectToClient += (x, xx) => {
				if (xx.Result == File_Transfer.Model.ReceiverFiles.ReceiveResult.RequestAccepted)
				{
					var fileRequire = HttpUtil.GetAllElements(waittingFileInfo, "<fileRequest>", "</fileRequest>");
					this.Invoke((EventHandler)delegate { AppendLog("文件已提交至发送队列:" + fileRequire.Count); });
					foreach (var f in fileRequire)
					{
						transferFileEngine.SendingFile("同步设置/" + f);
					}

				}
				else { this.Invoke((EventHandler)delegate { AppendLog("文件服务器连接终端失败:" + xx.Info); }); }
			};
			transferFileEngine.Sender.SendingFileStartedEvent += (x, xx) => {
				this.Invoke((EventHandler)delegate { AppendLog("开始传输文件:" + xx.FileName); });
			};
			transferFileEngine.Sender.SendingCompletedEvent += (x, xx) =>
			{
				this.Invoke((EventHandler)delegate {
					//AppendLog("文件传输结束:" + xx.Title + ":" + xx.Message);
					if (x.SendingFileQueue.Count==0)
					{
						AppendLog("终端文件传输结束" );
						if (fileSynQueue.Count > 0)
						{
							var fsq = fileSynQueue.Dequeue();
							HdlVpsFileSynRequest(fsq.FileRequire, fsq.Client);
						}
					}
				});
			};
		}
		private string waittingFileInfo;
		private ListViewItem GetItem(string ip)
		{
			foreach(var item in LstConnection.Items) {
				if (item is ListViewItem it)
				{
					if (it.SubItems[2].Text== ip) return it;
				}
			}
			return null;
		}
		private Reg regSetting;
		public Form1()
		{
			InitializeComponent();
			InitTransferEngine();
			regSetting = new Reg("sfMinerDigger").In("Setting").In("vps");
			t =new TcpServerManager() { NormalMessage=(s,x, InnerInfo) => {
				this.Invoke((EventHandler)delegate
				{
					var targetItem = GetItem(s.Ip);
					if (targetItem != null)
					{
						if (x.Contains("heartBeat"))
						{ }
						else if (x.Contains("clientConnect"))
						{
							targetItem.SubItems[3].Text = "初始化";
							targetItem.SubItems[0].Text =HttpUtil.GetElementInItem(InnerInfo, "connectCmdRequire");
							s.ID= HttpUtil.GetElementInItem(InnerInfo, "clientDeviceId");
							var clientName = regSetting.In(s.ID).GetInfo("clientName", targetItem.SubItems[0].Text);
							s.Send(string.Format("<setClientName>{0}</setClientName>", clientName));//用于确认当前名称并初始化
						}
						else if (x.Contains("InitComplete"))
						{
							//终端已初始化完成
							//synSetting,synFile
							//遍历 【同步文件】 下所有文件
							var dic =new  DirectoryInfo(Application.StartupPath+"\\同步设置");
							var tmp = new StringBuilder();
							foreach (var f in dic.EnumerateFiles())
							{
								tmp.Append("<file><name>").Append(f.Name).Append("</name>").Append("<version>").Append(HttpUtil.GetMD5ByMD5CryptoService(f.FullName)).Append("</version></file>");
							}
							if (tmp.Length > 0)
							{
								tmp.Append("<versionCheck>");
								s.Send(tmp.ToString());
							}
							else
							{
								s.Send("<serverRun>");//无需同步
							}
							
						}else if (x.Contains("RequireFile"))//服务器接收到来自客户端请求文件的命令
						{
							this.Invoke((EventHandler)delegate { AppendLog("vps" + s.clientName + "请求获取文件"); });
							HdlVpsFileSynRequest(InnerInfo, s);
						}
						else
						{
							AppendLog("新消息[" + s.clientName + "] "+ x+":" + InnerInfo);
							targetItem.SubItems[3].Text = InnerInfo;
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
