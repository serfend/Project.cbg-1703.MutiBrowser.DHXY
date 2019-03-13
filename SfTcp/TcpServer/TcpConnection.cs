
using Newtonsoft.Json;
using SfBaseTcp.Net.Sockets;
using SfTcp.TcpMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpServer
{
	public class TcpConnection
	{
		private ISocket client;
		private string aliasName;
		public TcpConnection(ISocket client,  string name)
		{
			this.Client = client;
			this.AliasName = name;
		}
		/// <summary>
		/// 发送常规消息
		/// </summary>
		/// <param name="title"></param>
		/// <param name="info"></param>
		public bool Send(string title,string info)
		{
			return Send(new NormalMessage(title,info));
		}

		/// <summary>
		/// 强制以文本方式发送
		/// </summary>
		/// <param name="raw"></param>
		/// <param name="cmd"></param>
		/// <returns></returns>
		public bool Send(string raw,int cmd)
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
			if (client.IsConnected) {
				Console.WriteLine($"send->{this.Ip} byte[{data.Length}]");
				client.Send(data);
			}
			return true;
		}
		public void Disconnect()
		{
			client.Disconnect();
		}
		public ISocket Client { get => client; set => client = value; }
		public string Ip { get => client.RemoteEndPoint.ToString(); }
		public string AliasName { get => aliasName; set => aliasName = value; }
		public string ID { get; set; }
		public bool IsLocal { get;  set; }
	}
}
