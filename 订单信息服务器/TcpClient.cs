using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SfTcp
{
	public class SfTcpClient:IDisposable
	{
		public TcpClient client;
		private NetworkStream stream;
		private BinaryReader br;
		private BinaryWriter bw;
		public Action<SfTcpClient, string> RecieveMessage;
		public Action<SfTcpClient> Disconnected;
		
		
		private Thread reporterThread, receiverThread;
		private int lastLength, reporterCounter;
		public SfTcpClient(string ip,int port)
		{
			Console.WriteLine("尝试与服务器建立连接.");
			try
			{
				client = new TcpClient(ip, port);
			}
			catch (Exception ex)
			{
				MessageBox.Show("连接建立失败:"+ ex.Message);
				return;
			}
			Console.WriteLine("连接建立");
			stream = client.GetStream();
			bw = new BinaryWriter(stream);
			br = new BinaryReader(stream);
			receiverThread = new Thread(Reciving) { IsBackground=true};
			
			reporterThread = new Thread(() => {
				while (true)
				{
					var thisLen = cstr.Length;
					if (thisLen == lastLength && thisLen > 0 && reporterCounter++ > 50)
					{
						RecieveComplete();
					}
					else
					{
						lastLength = thisLen;
					}
					Thread.Sleep(10);
				}
			})
			{ IsBackground = true };
			reporterThread.Start();
			receiverThread.Start();
		}
		private void RecieveComplete(bool getEndPoint = false)
		{
			if (getEndPoint) cstr.Replace(this.TcpComplete, "");
			RecieveMessage?.BeginInvoke(this, cstr.ToString(), (x) => { }, null);
			//Receive.Invoke(cstr.ToString(), this);
			cstr.Clear();
			lastLength = 0;
			reporterCounter = 0;
			nowCheckIndex = 0;
			br.BaseStream.Flush();
		}
		public virtual bool Send(string key,string info)
		{
			var safeMessage = string.Format("<{0}>{1}</{0}>{2}",key, EncryptHelper.AESEncrypt(info), TcpComplete);
			return Send(Encoding.UTF8.GetBytes(safeMessage ));
		}
		public virtual bool Send(byte[] info)
		{
			if (client!=null&&client.Connected)
			{
				try
				{
					bw.Write(info);
					bw.Flush();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Tcp.Send()" + ex.Message);
					Disconnected?.BeginInvoke(this, (x) => { }, null);
					return false;
				}
				return true;
			}
			else return false;
		}
		private string TcpComplete
		{
			get => "#$%&'";
		}
		StringBuilder cstr = new StringBuilder();
		private int nowCheckIndex =0;
		private void Reciving()
		{
			while (true)
			{
				if (client.Connected)
				{
					try
					{
						var c = br.ReadChar();
						cstr.Append(c);
						if (c == '#' + nowCheckIndex)
						{
							nowCheckIndex++;
							if (nowCheckIndex == 5)
							{
								RecieveComplete(true);
								continue;
							}
						}
						
					}
					catch (Exception ex)
					{
						Disconnected?.BeginInvoke(this, (x) => { }, null);
						Console.WriteLine("Tcp.Reciving()" + ex.Message);
						break;
					}

				}
				else
				{
					Console.WriteLine("已断开");
					Disconnected?.BeginInvoke(this, (x) => { }, null);
					break;
				}
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					receiverThread?.Abort();
					reporterThread?.Abort();
					bw?.Dispose();
					br?.Dispose();
					stream?.Dispose();
					client?.Close();
				}
				client = null;
				stream = null;
				bw = null;
				br = null;

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
			Dispose(true);

		}
		#endregion
	}
}
