using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SfTcp
{
	public class TcpHttpMessage
	{
		private string Method;
		private string param;
		private string httpVersion;

		public TcpHttpMessage(string method, string param, string httpVersion)
		{
			Method1 = method;
			this.Param = param;
			this.HttpVersion = httpVersion;
			Console.WriteLine(param);
		}

		public string Method1 { get => Method; set => Method = value; }
		public string Param { get => param; set => param = value; }
		public string HttpVersion { get => httpVersion; set => httpVersion = value; }
	}
	public class TcpHttpResponse
	{
		private TcpServer server;
		public TcpHttpResponse(TcpServer server)
		{
			this.server = server;
		}
		public void Response(string info,string title="Serfend")
		{
			var cstr = new StringBuilder();
			cstr.AppendLine("HTTP/1.1 200 OK\r\nContent-Type: text/html\r\n\r\n");
			cstr.AppendLine("<html lang=\"zh-cn\"><head><meta charset = \"utf-8\" />");
			cstr.AppendLine(string.Format("<title>{0}</title>",title));
			cstr.AppendLine("</head><body>");
			cstr.AppendLine(info);
			cstr.AppendLine("</body></html>");
			server.Send(Encoding.UTF8.GetBytes(cstr.ToString()));
			server.client.Close();
		}
	}
	public class TcpServer
	{
		TcpListener listener;
		private Thread thread ;
		private Thread reporter;
		public TcpClient client;
		private BinaryWriter writter;
		private BinaryReader reader;
		private Action<string, TcpServer> Receive;//收到信息回调
		public Action<TcpServer> Connected;//连接成功回调
		public Action<TcpServer> Disconnected;//连接成功回调
		public Action<TcpHttpMessage, TcpHttpResponse> OnHttpRequest;//当来源为http方式时
		public bool IsLocal=false;
		public string Ip;
		public string clientName = "...";
		public TcpServer( Action<string,TcpServer> ReceiveInfo = null,Action<TcpHttpMessage, TcpHttpResponse> ReceiveHttp=null,int port=8009)
		{
			listener = new TcpListener(IPAddress.Any, port);
			this.Receive = ReceiveInfo;
			this.OnHttpRequest = ReceiveHttp;
			this.Connect();
		}
		public void Connect()
		{
			var t = new Thread(() =>
			{
				try
				{

					listener.Start();
					client = listener.AcceptTcpClient();
					listener.Stop();
					var ip = this.client.Client.RemoteEndPoint.ToString();
					if (ip.Contains("127.0.0.1")) IsLocal = true;
					this.Ip = this.client.Client.RemoteEndPoint.ToString();
					Connected?.Invoke(this);
					var stream = client.GetStream();
					writter = new BinaryWriter(stream);
					reader = new BinaryReader(stream);
					thread = new Thread(Reciving) { IsBackground = true };
					thread.Start();
					reporter = new Thread(() => {
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
					reporter.Start();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
				}
			})
			{ IsBackground=true};
			t.Start();
			
		}
		private void RecieveComplete(bool getEndPoint=false)
		{
			if (getEndPoint) cstr.Replace(this.TcpComplete,"");
;			ReceiveInfo(cstr.ToString());
			//Receive.Invoke(cstr.ToString(), this);
			cstr.Clear();
			lastLength = 0;
			reporterCounter = 0;
			nowCheckIndex = 0;
		}
		private int reporterCounter = 0;
		private int lastLength;
		private void ReceiveInfo(string info)
		{
			var firstLineIndex = info.IndexOf("\n");
			if (firstLineIndex > 0)
			{
				var firstLine = info.Substring(0, firstLineIndex - 1);
				var lineInfo = firstLine.Split(' ');
				if (lineInfo.Length == 3)
				{
					OnHttpRequest.Invoke(new TcpHttpMessage(lineInfo[0], lineInfo[1].Substring(1), lineInfo[2]), new TcpHttpResponse(this));
					return;
				}
			}
			Receive.Invoke(info,this);
		}
		public void Disconnect()
		{
			listener.Stop();
			if(client.Connected) client.Close();

		}
		private string TcpComplete {
			get => "#$%&'";
		}
		public bool Send(string info)
		{
			return Send(Encoding.UTF8.GetBytes(info+TcpComplete));
		}
		public bool Send(byte[] info)
		{
			if (client.Connected)
			{
				try
				{
					writter.Write(info);
					writter.Flush();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Tcp.Send()"+ex.Message);
					Disconnected.Invoke(this);
					return false;
				}
				return true;
			}
			else return false;
		}
		StringBuilder cstr = new StringBuilder();
		private int nowCheckIndex=0;
		private void Reciving()
		{
			while (true)
			{
				if (client.Connected)
				{
					try
					{
						var c = reader.ReadChar();
						cstr.Append(c);
						if (c == ('#' + nowCheckIndex))
						{
							nowCheckIndex++;
							if (nowCheckIndex == 5)
							{
								RecieveComplete(true);
								continue;
							}
						}
						
					}
					catch  (Exception ex)
					{
						Disconnected?.Invoke(this);
						Console.WriteLine("Tcp.Reciving()"+ex.Message);
						break;
					}
					
				}
				else
				{
					Console.WriteLine("已断开");
					Disconnected?.Invoke(this);
					break;
				}
			}
		}
	}
}
