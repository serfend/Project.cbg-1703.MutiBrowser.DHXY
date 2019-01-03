using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace 订单信息服务器.WebSocketServer
{
	public class Session
	{
		#region 打包请求连接数据
		/// <summary>
		/// 打包请求连接数据
		/// </summary>
		/// <param name="handShakeBytes"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static byte[] PackageHandShakeData(byte[] handShakeBytes, int length)
		{
			string handShakeText = Encoding.UTF8.GetString(handShakeBytes, 0, length);
			string key = string.Empty;
			Regex reg = new Regex(@"Sec\-WebSocket\-Key:(.*?)\r\n");
			Match m = reg.Match(handShakeText);
			if (m.Value != "")
			{
				key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)\r\n", "$1").Trim();
			}
			byte[] secKeyBytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
			string secKey = Convert.ToBase64String(secKeyBytes);
			var responseBuilder = new StringBuilder();
			responseBuilder.Append("HTTP/1.1 101 Switching Protocols" + "\r\n");
			responseBuilder.Append("Upgrade: websocket" + "\r\n");
			responseBuilder.Append("Connection: Upgrade" + "\r\n");
			responseBuilder.Append("Sec-WebSocket-Accept: " + secKey + "\r\n\r\n");
			return Encoding.UTF8.GetBytes(responseBuilder.ToString());
		}
		#endregion
		#region 发送数据
		/// <summary>
		/// 把发送给客户端消息打包处理（拼接上谁什么时候发的什么消息）
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="message">Message.</param>
		private static byte[] PackageServerData(string msg)
		{
			byte[] content = null;
			byte[] temp = Encoding.UTF8.GetBytes(msg);
			if (temp.Length < 126)
			{
				content = new byte[temp.Length + 2];
				content[0] = 0x81;
				content[1] = (byte)temp.Length;
				Buffer.BlockCopy(temp, 0, content, 2, temp.Length);
			}
			else if (temp.Length < 0xFFFF)
			{
				content = new byte[temp.Length + 4];
				content[0] = 0x81;
				content[1] = 126;
				content[2] = (byte)(temp.Length & 0xFF);
				content[3] = (byte)(temp.Length >> 8 & 0xFF);
				Buffer.BlockCopy(temp, 0, content, 4, temp.Length);
			}
			return content;
		}
		#endregion
		private Socket _sockeclient;
		private byte[] _buffer;
		private string _ip;
		private bool _connected;
		/// <summary>
		/// 建立握手连接
		/// </summary>
		/// <param name="data"></param>
		/// <param name="len"></param>
		public void HandShake(byte[] data,int len)
		{
			SockeClient.Send(PackageHandShakeData(data, len));
			Connected = true;
		}
		public bool Send(string info)
		{
			if (!Connected) return false;
			try
			{
				SockeClient.Send(PackageServerData(info));
			}
			catch (Exception ex)
			{
				Console.WriteLine("WebSocket.Send()"+ex.Message);
				return false;
			}
			return true;
		}
		public Session(byte[] buffer, Socket sockeClient)
		{
			this.buffer = buffer;
			SockeClient = sockeClient;
			IP = sockeClient.RemoteEndPoint.ToString();
		}

		public Socket SockeClient
		{
			private set { _sockeclient = value; }
			get { return _sockeclient; }
		}

		public byte[] buffer
		{
			private set { _buffer = value; }
			get { return _buffer; }
		}

		public string IP
		{
			private set { _ip = value; }
			get { return _ip; }
		}


		public bool Connected { get => _connected;private set => _connected = value; }
	}
}
