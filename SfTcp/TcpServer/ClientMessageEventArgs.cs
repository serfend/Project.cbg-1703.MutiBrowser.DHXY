using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SfTcp.TcpServer
{
	public delegate void ClientHttpMessage(object sender, ClientHttpMessageEventArgs e);
	public class ClientHttpMessageEventArgs : EventArgs
	{
		private TcpHttpMessage msg;
		private TcpHttpResponse rsp;

		public ClientHttpMessageEventArgs(TcpHttpMessage msg,TcpConnection server)
		{
			this.msg = msg;
			this.rsp = new TcpHttpResponse(server);
		}

		public TcpHttpResponse Response { get => rsp; set => rsp = value; }
		public TcpHttpMessage Message { get => msg; set => msg = value; }
	}
	public delegate void ClientMessage(object sender, ClientMessageEventArgs e);
	public class ClientMessageEventArgs : EventArgs
	{
		private byte[] data;
		public ClientMessageEventArgs(byte[] data)
		{
			this.data = data;
		}
		public ClientMessageEventArgs(StringBuilder raw,byte[] newData)
		{
			raw.Append(Encoding.UTF8.GetString(newData));
			rawString = raw.ToString();
			analysed = true;
			HandleRaw();
		}
		private bool error=false;
		public byte[] Data { get => data; set => data = value; }
		private bool analysed = false;
		private string title;
		public void AnalysisRaw()
		{
			if (analysed) return;
			analysed = true;
			rawString = Encoding.UTF8.GetString(data);
			HandleRaw();
		}

		private void HandleRaw()
		{
			if (data.Length == 0) {//空数据是想干嘛哦~
				error = true;
				return;
			}
			try
			{
				dic = JToken.Parse(rawString);
				title = dic["Title"]?.ToString();
				if (title == null) error = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"处理数据异常:{ex.Message}\n{rawString}");
				error = true;
			}
		}
		private JToken dic;
		public string Title
		{
			get
			{
				AnalysisRaw();
				return title;
			}
		}
		public JToken Message
		{
			get
			{
				AnalysisRaw();
				return dic;
			}
		}
		public string RawString
		{
			get
			{
				AnalysisRaw();
				return rawString;
			}
		}

		public bool Error { get => error; set => error = value; }

		private string rawString;
	}
}