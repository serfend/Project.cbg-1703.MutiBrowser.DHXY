
﻿using DotNet4.Utilities.UtilCode;
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
		private string raw;
		private string method;
		private string param;
		private string payLoad;
		private string httpVersion;
		private Dictionary<string, string> headers;
		public  static TcpHttpMessage CheckTcpHttpMessage(string info)
		{
			var firstLineIndex = info.IndexOf("\n");
			if (firstLineIndex > 0)
			{
				var firstLine = info.Substring(0, firstLineIndex - 1);
				var lineInfo = firstLine.Split(' ');
				if (lineInfo.Length == 3)
				{
					
					var httpMessage = new TcpHttpMessage(lineInfo[0], lineInfo[1].StartsWith("/") ? lineInfo[1].Substring(1) : lineInfo[1], lineInfo[2]);
					var lines = info.Split('\n');
					int linesLen = lines.Length;
					for (int i=1;i< linesLen;i++)
					{
						int checkIndex = lines[i].IndexOf(": ");
						if (checkIndex>0)
						{
							string[] inItem = new string[] {
								lines[i].Substring(0,checkIndex),
								lines[i].Substring(checkIndex+1)
							};
							
							inItem[1] = inItem[1].Remove(inItem[1].Length - 1, 1);
							if (httpMessage.Headers.ContainsKey(inItem[0]))
							{
								httpMessage.Headers[inItem[0]] += $";{inItem[1]}";
							}
							else
							{
								httpMessage.Headers.Add(inItem[0], inItem[1]);
							}
						}else if (lines[i]!="\r")
						{
							httpMessage.PayLoad = lines[i];
						}
					}
					httpMessage.Raw = info;
					return httpMessage;
				}
			}
			return null;
		}
		public TcpHttpMessage(string method, string param, string httpVersion)
		{
			Method = method;
			this.Param = param;
			this.HttpVersion = httpVersion;
			headers = new Dictionary<string, string>();
			Console.WriteLine($"TcpHttpMessage(){method}:{param}/{httpVersion}");
		}
		public string Param { get => param; set => param = value; }
		public string HttpVersion { get => httpVersion; set => httpVersion = value; }
		public string Method { get => method; set => method = value; }
		public Dictionary<string, string> Headers { get => headers; set => headers = value; }
		public string Raw { get => raw; set => raw = value; }
		public string PayLoad { get => payLoad; set => payLoad = value; }
	}
	public class TcpHttpResponse
	{
		private TcpServer server;
		public TcpHttpResponse(TcpServer server)
		{
			this.Server = server;
		}

		public TcpServer Server { get => server; set => server = value; }

		public void Response(string info, string title = "Serfend")
		{
			var cstr = new StringBuilder();
			cstr.AppendLine("<html lang=\"zh-cn\"><head><meta charset = \"utf-8\" />");
			cstr.AppendLine(string.Format("<title>{0}</title>", title));
			cstr.AppendLine("</head><body>");
			cstr.AppendLine(info);
			cstr.AppendLine("</body></html>");
			ResponseRaw(cstr.ToString());
		}
		public void ResponseRaw(string info,int status=200,string ContentType="text/html")
		{
			var rawInfo=$"HTTP/1.1 {status} OK\r\nContent-Type: {ContentType}\r\n\r\n{info}";
			ResponseRawByte(Encoding.UTF8.GetBytes(rawInfo));
		}
		public void ResponseRawByte(byte[] info)
		{
			Server.Send(info);
			Server.client.Close();
		}
	}
	public class TcpServer : IDisposable
	{
		#region 属性
		TcpListener listener;
		private Thread thread;
		private Thread reporter;
		public TcpClient client;
		private BinaryWriter writter;
		private BinaryReader reader;
		/// <summary>
		/// title,content,tcpServer
		/// </summary>
		private readonly Action<string, string, TcpServer> Receive;//收到信息回调
		public Action<TcpServer> Connected;//连接成功回调
		public Action<TcpServer> Disconnected;//连接成功回调
		public Action<TcpHttpMessage, TcpHttpResponse> OnHttpRequest;//当来源为http方式时
		public bool IsLocal = false;
		public string Ip;
		public string ID = "null";
		public string clientName = "...";
		public enum ConnectProtocal
		{
			Null,
			Base64,
			Aes
		}
		public ConnectProtocal connectProtocal = ConnectProtocal.Base64;
		#endregion
		public TcpServer(Action<string, string, TcpServer> ReceiveInfo = null, Action<TcpHttpMessage, TcpHttpResponse> ReceiveHttp = null, int port = 8009)
		{
			listener = new TcpListener(IPAddress.Any, port);
			this.Receive = ReceiveInfo;
			this.OnHttpRequest = ReceiveHttp;
		}
		public void Connect()
		{
			try
			{
				listener.Start();
				while (!listener.Pending())
				{
					Thread.Sleep(10);
				}
				client = listener.AcceptTcpClient();
				listener.Stop();
				this.Ip = this.client.Client.RemoteEndPoint.ToString();
				if (this.Ip.Contains("127.0.0.1")) IsLocal = true;
				//var checkPortOnly = Ip.IndexOf(':');
				//if (checkPortOnly > 0) Ip = Ip.Substring(checkPortOnly + 1);
				Connected.BeginInvoke(this, (x) => { }, null);
				var stream = client.GetStream();
				writter = new BinaryWriter(stream);
				reader = new BinaryReader(stream);
				thread = new Thread(Reciving) { IsBackground = true };
				thread.Start();
				reporter = new Thread(() => {
					while (true)
					{
						var thisLen = cstr.Length;
						if (thisLen == lastLength && thisLen > 0)
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


		}
		private void RecieveComplete()
		{
			ReceiveInfo(cstr.ToString());
			//Receive.Invoke(cstr.ToString(), this);
			cstr.Clear();
			lastLength = 0;
			reader.BaseStream.Flush();
		}
		private int lastLength;
		private void ReceiveInfo(string info)
		{
			var httpMessage =  TcpHttpMessage.CheckTcpHttpMessage(info);
			if (httpMessage != null)
			{
				OnHttpRequest?.BeginInvoke(httpMessage, new TcpHttpResponse(this), (x) => { }, null);
			}
			if (!info.StartsWith("<")) return;
			var title = HttpUtil.GetElement(info, "<", ">");
			var raw = HttpUtil.GetElement(info, ">", "<");
			if (info.IndexOf("</" + title + ">", 0) < 0) return;
			var content = GetContent(raw);
			//MessageBox.Show(content);
			Receive?.BeginInvoke(title, content, this, (x) => { }, null);
		}
		private bool _protocalEnsure = false;
		public string GetContent(string raw)
		{
			string content = string.Empty;
			switch (this.connectProtocal)
			{
				case ConnectProtocal.Aes:
					{
						content = EncryptHelper.AESDecrypt(raw);
						if (content == string.Empty && !_protocalEnsure)
						{
							this.connectProtocal = ConnectProtocal.Base64;
							return GetContent(raw);
						}
						_protocalEnsure = true;
						break;
					}
				case ConnectProtocal.Base64:
					{
						content = EncryptHelper.Base64Decode(raw);
						if (content == string.Empty)
						{
							this.connectProtocal = ConnectProtocal.Null;
							return GetContent(raw);
						}
						break;
					}
				case ConnectProtocal.Null:
					{
						this.connectProtocal = ConnectProtocal.Aes;
						return raw;
					}
			}
			return content;
		}
		public void Disconnect()
		{
			listener.Stop();
			if (client.Connected) client.Close();

		}

		public bool Send(string info)
		{
			return Send(Encoding.UTF8.GetBytes(info));
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
					Console.WriteLine("Tcp.Send()" + ex.Message);
					Disconnected?.BeginInvoke(this, (x) => { }, null);
					return false;
				}
				return true;
			}
			else return false;
		}
		StringBuilder cstr = new StringBuilder();
		private void Reciving()
		{
			while (true)
			{
				if (client.Connected)
				{
					try
					{
						if (client.Available > 0)
						{
							var c = reader.ReadChar();
							cstr.Append(c);
						}
						else
						{
							Thread.Sleep(1);
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
		internal int lastMessageTime;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (client != null) client.Close();
					if (writter != null) writter.Dispose();
					if (reader != null) reader.Dispose();
				}
				client = null;
				writter = null;
				reader = null;
				disposedValue = true;
			}
		}


		// 添加此代码以正确实现可处置模式。
		public void Dispose()
		{
			// 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
			Dispose(true);

		}
		#endregion
	}
}