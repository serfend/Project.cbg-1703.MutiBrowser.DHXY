using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpClient
{
	public delegate void ServerMessage(object sender, ServerMessageEventArgs e);
	public class ServerMessageEventArgs:EventArgs
	{
		private byte[] data;

		public ServerMessageEventArgs(byte[] data)
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
				dic = JsonConvert.DeserializeObject<Dictionary<string,object>>(rawString);
				title = dic["Title"].ToString();
			}
			catch (Exception ex)
			{
				title = ex.Message;
			}
		}
		private Dictionary<string, object> dic;
		public string Title { get {
				AnalysisRaw();
				return title;
			} }
		public Dictionary<string, object> Message
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
