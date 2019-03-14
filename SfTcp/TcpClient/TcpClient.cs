using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SfTcp.TcpMessage;

namespace SfTcp.TcpClient
{
	public class TcpClient:IDisposable
	{
		public event ConnectToServer OnConnected;
		public event DisconnectFromServer OnDisconnected;
		public event ServerMessage OnMessage;
		private TcpClientConnection client;
		public TcpClient(string ip,int port)
		{
			Console.WriteLine($"尝试连接到{ip}:{port}");
			Client = new TcpClientConnection(ip,port);
			Client.OnConnected += (x, xx) => {
				Console.WriteLine($"连接成功到{ip}:{port}");
				OnConnected?.Invoke(this, xx);
			};
			Client.OnDisconnected += (x, xx) => {
				Console.WriteLine($"与服务器丢失连接:{ip}:{port}");
				OnDisconnected?.Invoke(this, xx);
			};
			Client.OnMessaged += (x, xx) =>
			{
				//Console.WriteLine($"clientReceive:{xx.RawString}");
				OnMessage?.Invoke(this, xx);
			};
		}
		/// <summary>
		/// 发送常规消息
		/// </summary>
		/// <param name="title"></param>
		/// <param name="info"></param>
		public bool Send(string title, string info,int cmd)
		{
			return Send(new NormalMessage(title, info));
		}

		/// <summary>
		/// 强制以文本方式发送
		/// </summary>
		/// <param name="raw"></param>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public bool Send(string raw, int cmd)
		{
			return Send(Encoding.UTF8.GetBytes(raw));
		}
		/// <summary>
		/// 发送一般消息
		/// </summary>
		/// <param name="raw">可转换Json格式的实体类，并继承BaseMessage</param>
		public bool Send(ITcpMessage raw)
		{
			return Send(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(raw)));
		}
		/// <summary>
		/// 发送原始数据
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool Send(byte[] data)
		{
			return Client.Send(data);
		}
		public void Disconnect()
		{
			Client.Disconnect();
		}


		#region IDisposable Support
		private bool disposedValue = false; // 要检测冗余调用

		public TcpClientConnection Client { get => client; set => client = value; }

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if(Client!=null)Client.Dispose();
				}
				Client = null;
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
