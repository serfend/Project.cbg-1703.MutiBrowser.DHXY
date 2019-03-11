using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpClient
{
	public delegate void ServerMessage(object sender, ClientMessageEventArgs e);
	public class ClientMessageEventArgs:EventArgs
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
				dic = JToken.Parse(rawString);
				title = dic["Title"].ToString();
			}
			catch (Exception ex)
			{
				title = ex.Message;
			}
		}
		private JToken dic;
		public string Title { get {
				AnalysisRaw();
				return title;
			} }
		public JToken Message
		{
			get {
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
