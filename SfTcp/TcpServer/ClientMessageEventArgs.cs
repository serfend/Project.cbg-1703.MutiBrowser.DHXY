using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SfTcp.TcpServer
{

	public delegate void ClientMessage(object sender, ClientMessageEventArgs e);
	public class ClientMessageEventArgs : EventArgs
	{
		private byte[] data;

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
			try
			{
				rawString = Encoding.UTF8.GetString(data);
				dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(rawString);
				title = dic["Title"].ToString();
			}
			catch (Exception ex)
			{
				title = ex.Message;
			}
		}
		private Dictionary<string, object> dic;
		public string Title
		{
			get
			{
				AnalysisRaw();
				return title;
			}
		}
		public Dictionary<string, object> Message
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
		private string rawString;
	}
}