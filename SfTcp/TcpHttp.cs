using SfTcp.TcpServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		/// <summary>
		/// 检查字符串是否是Http请求
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		public static TcpHttpMessage CheckTcpHttpMessage(string info)
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
					for (int i = 1; i < linesLen; i++)
					{
						int checkIndex = lines[i].IndexOf(": ");
						if (checkIndex > 0)
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
						}
						else if (lines[i] != "\r")
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
		private TcpConnection server;
		public TcpHttpResponse(TcpConnection server)
		{
			this.Server = server;
		}

		public TcpConnection Server { get => server; set => server = value; }

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
		public void ResponseRaw(string info, int status = 200, string ContentType = "text/html")
		{
			var rawInfo = $"HTTP/1.1 {status} OK\r\nContent-Type: {ContentType}\r\n\r\n{info}";
			ResponseRawByte(Encoding.UTF8.GetBytes(rawInfo));
		}
		public void ResponseRawByte(byte[] info)
		{
			Server.Send(info);
			Server.Disconnect();
		}
	}
}
