using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SfTcp.TcpServer
{

	public delegate void ClientMessage(object sender, ClientMessageEventArgs e);
	public class ClientMessageEventArgs : EventArgs
	{
		private byte[] data;
		private bool isHttp;
		public ClientMessageEventArgs(byte[] data)
		{
			this.data = data;
		}

		public byte[] Data { get => data; set => data = value; }
		private bool analysed = false;
		private string title;
		private void AnalysisRaw()
		{
			if (analysed) return;
			analysed = true;
			rawString = Encoding.UTF8.GetString(data);
			dic = JToken.Parse(rawString);
			title = dic["Title"].ToString();
			
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

		public bool IsHttp { get => isHttp; private set => isHttp = value; }

		private string rawString;
	}
}