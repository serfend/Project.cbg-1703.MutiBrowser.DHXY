using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNet4.Utilities.UtilCode;
using File_Transfer;
using SfTcp;
using SfTcp.TcpServer;

namespace 订单信息服务器
{
    public partial class Form1
	{
		private TransferFileEngine transferFileEngine;
		private Dictionary<string,string> waittingFileInfo;
		private struct FileWaitingClient
		{
			TcpConnection client;
			Dictionary<string, string> fileRequire;
			public TcpConnection Client { get => client; set => client = value; }
			public Dictionary<string,string> FileRequire { get => fileRequire; set => fileRequire = value; }
		}

		private Queue<FileWaitingClient> fileSynQueue = new Queue<FileWaitingClient>();
		private void InitTransferEngine()
		{
			if (transferFileEngine != null)
			{
				transferFileEngine.Dispose();
				transferFileEngine = null;
			}
			transferFileEngine = new TransferFileEngine(TcpFiletransfer.TcpTransferEngine.Connections.Connection.EngineModel.AsServer, "any", 8010);
			transferFileEngine.Connection.ConnectToClient += (x, xx) =>
			{
				if (xx.Result == File_Transfer.ReceiverFiles.ReceiveResult.RequestAccepted)
				{
					this.Invoke((EventHandler)delegate { AppendLog("文件已提交至发送队列:" + waittingFileInfo.Count); });
					foreach (var f in waittingFileInfo)
					{
						transferFileEngine.SendingFile("同步设置/" + f);
					}

				}
				else { this.Invoke((EventHandler)delegate { AppendLog("文件服务器连接终端失败:" + xx.Info); }); }
			};
			transferFileEngine.Sender.SendingFileStartedEvent += (x, xx) =>
			{
				this.Invoke((EventHandler)delegate { AppendLog("开始传输文件:" + xx.FileName); });
			};
			transferFileEngine.Sender.SendingCompletedEvent += (x, xx) =>
			{
				this.Invoke((EventHandler)delegate
				{
					//AppendLog("文件传输结束:" + xx.Title + ":" + xx.Message);
					if (x.SendingFileQueue.Count == 0)
					{
						AppendLog("终端文件传输结束");
						if (fileSynQueue.Count > 0)
						{
							var fsq = fileSynQueue.Dequeue();
							HdlVpsFileSynRequest(fsq.FileRequire, fsq.Client);
						}
					}
				});
			};
		}

		private void HdlVpsFileSynRequest(Dictionary<string,string> transferFileList,TcpConnection s)
		{
			if (transferFileEngine.Sender.SendingFileQueue.Count > 0 && !transferFileEngine.Sender.IsSending)
			{
				fileSynQueue.Enqueue(new FileWaitingClient() { FileRequire = transferFileList, Client = s });
				return;
			}
			var t = new Task(() => {

				waittingFileInfo = transferFileList;//记录命令的详细信息

				this.Invoke((EventHandler)delegate { AppendLog("准备向终端" + s.AliasName + " 传输文件"); });
				InitTransferEngine();
				Thread.Sleep(5000);
				s.Send("ensureFileTransfer",new Random().Next(0, 1000).ToString());
				transferFileEngine.Connect();//文件发送引擎

			});
			t.Start();
		}
	}
}
