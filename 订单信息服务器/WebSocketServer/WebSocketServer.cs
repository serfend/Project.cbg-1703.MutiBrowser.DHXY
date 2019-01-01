﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace 订单信息服务器.WebSocketServer
{
	/// <summary>
	/// 实现和浏览器的交互
	/// </summary>
	public class WebSocket
	{
		public event WebSocketClientConnect OnConnect;
		public delegate void WebSocketClientConnect(object sender,ClientConnectEventArgs e);
		public event WebSocketClientDisconnect OnDisconnect;
		public delegate void WebSocketClientDisconnect(object sender, ClientDisconnectEventArgs e);
		public event WebSocketNewMessage OnNewMessage;
		public delegate void WebSocketNewMessage(object sender, ClientNewMessageEventArgs e);
		private Dictionary<string, Session> SessionPool = new Dictionary<string, Session>();
		private Dictionary<string, string> MsgPool = new Dictionary<string, string>();

		#region 启动WebSocket服务
		/// <summary>
		/// 启动WebSocket服务
		/// </summary>
		public void start(int port)
		{
			Socket SockeServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			SockeServer.Bind(new IPEndPoint(IPAddress.Any, port));
			SockeServer.Listen(20);
			SockeServer.BeginAccept(new AsyncCallback(Accept), SockeServer);
		}
		#endregion

		#region 处理客户端连接请求
		/// <summary>
		/// 处理客户端连接请求
		/// </summary>
		/// <param name="result"></param>
		private void Accept(IAsyncResult socket)
		{
			// 还原传入的原始套接字
			Socket SockeServer = (Socket)socket.AsyncState;
			// 在原始套接字上调用EndAccept方法，返回新的套接字
			Socket SockeClient = SockeServer.EndAccept(socket);
			byte[] buffer = new byte[4096];
			try
			{
				//接收客户端的数据
				SockeClient.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Recieve), SockeClient);
				//保存登录的客户端
				Session session = new Session(buffer, SockeClient);
				lock (SessionPool)
				{
					if (SessionPool.ContainsKey(session.IP))
					{
						this.SessionPool.Remove(session.IP);
					}
					this.SessionPool.Add(session.IP, session);
				}
				//准备接受下一个客户端
				SockeServer.BeginAccept(new AsyncCallback(Accept), SockeServer);
				OnConnect?.Invoke(this,new ClientConnectEventArgs(session));
				Console.WriteLine(string.Format("WebSocketServer:Client {0} connected", SockeClient.RemoteEndPoint));
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error : " + ex.ToString());
			}
		}
		#endregion

		#region 处理接收的数据
		/// <summary>
		/// 处理接受的数据
		/// </summary>
		/// <param name="socket"></param>
		private void Recieve(IAsyncResult socket)
		{
			Socket SockeClient = (Socket)socket.AsyncState;
			string IP = SockeClient.RemoteEndPoint.ToString();
			if (SockeClient == null || !SessionPool.ContainsKey(IP))
			{
				return;
			}
			var client = SessionPool[IP];
			try
			{
				int length = SockeClient.EndReceive(socket);
				byte[] buffer = SessionPool[IP].buffer;
				SockeClient.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(Recieve), SockeClient);
				string msg = Encoding.UTF8.GetString(buffer, 0, length);
				//  websocket建立连接的时候，除了TCP连接的三次握手，websocket协议中客户端与服务器想建立连接需要一次额外的握手动作
				if (msg.Contains("Sec-WebSocket-Key"))
				{
					client.HandShake(buffer, length);
					return;
				}
				if (!client.Connected) return;
				msg = AnalyzeClientData(buffer, length);
				//byte[] msgBuffer = PackageServerData("服务器收到消息:"+ msg);
				//client.Send(msgBuffer);
				OnNewMessage?.Invoke(this, new ClientNewMessageEventArgs(client, msg));
			}
			catch(Exception ex)
			{
				Console.WriteLine("接收客户端数据异常:"+ ex.Message);
				try
				{
					SockeClient.Disconnect(true);
				}
				catch { }
				OnDisconnect?.Invoke(this,new ClientDisconnectEventArgs(client));
				Console.WriteLine("WebSocketServer:client {0} disconnect", IP);
				SessionPool.Remove(IP);
			}
		}
		#endregion

		#region 客户端和服务端的响应
		/*
         * 客户端向服务器发送请求
         * 
         * GET / HTTP/1.1
         * Origin: http://localhost:1416
         * Sec-WebSocket-Key: vDyPp55hT1PphRU5OAe2Wg==
         * Connection: Upgrade
         * Upgrade: Websocket
         *Sec-WebSocket-Version: 13
         * User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko
         * Host: localhost:8064
         * DNT: 1
         * Cache-Control: no-cache
         * Cookie: DTRememberName=admin
         * 
         * 服务器给出响应
         * 
         * HTTP/1.1 101 Switching Protocols
         * Upgrade: websocket
         * Connection: Upgrade
         * Sec-WebSocket-Accept: xsOSgr30aKL2GNZKNHKmeT1qYjA=
         * 
         * 在请求中的“Sec-WebSocket-Key”是随机的，服务器端会用这些数据来构造出一个SHA-1的信息摘要。把“Sec-WebSocket-Key”加上一个魔幻字符串
         * “258EAFA5-E914-47DA-95CA-C5AB0DC85B11”。使用 SHA-1 加密，之后进行 BASE-64编码，将结果做为 “Sec-WebSocket-Accept” 头的值，返回给客户端
         */
		#endregion


		#region 处理接收的数据
		/// <summary>
		/// 处理接收的数据
		/// 参考 http://www.cnblogs.com/smark/archive/2012/11/26/2789812.html
		/// </summary>
		/// <param name="recBytes"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		private string AnalyzeClientData(byte[] recBytes, int length)
		{
			int start = 0;
			// 如果有数据则至少包括3位
			if (length < 2) return "";
			// 判断是否为结束针
			bool IsEof = (recBytes[start] >> 7) > 0;
			// 暂不处理超过一帧的数据
			if (!IsEof) return "";
			start++;
			// 是否包含掩码
			bool hasMask = (recBytes[start] >> 7) > 0;
			// 不包含掩码的暂不处理
			if (!hasMask) return "";
			// 获取数据长度
			UInt64 mPackageLength = (UInt64)recBytes[start] & 0x7F;
			start++;
			// 存储4位掩码值
			byte[] Masking_key = new byte[4];
			// 存储数据
			byte[] mDataPackage;
			if (mPackageLength == 126)
			{
				// 等于126 随后的两个字节16位表示数据长度
				mPackageLength = (UInt64)(recBytes[start] << 8 | recBytes[start + 1]);
				start += 2;
			}
			if (mPackageLength == 127)
			{
				// 等于127 随后的八个字节64位表示数据长度
				mPackageLength = (UInt64)(recBytes[start] << (8 * 7) | recBytes[start] << (8 * 6) | recBytes[start] << (8 * 5) | recBytes[start] << (8 * 4) | recBytes[start] << (8 * 3) | recBytes[start] << (8 * 2) | recBytes[start] << 8 | recBytes[start + 1]);
				start += 8;
			}
			mDataPackage = new byte[mPackageLength];
			for (UInt64 i = 0; i < mPackageLength; i++)
			{
				mDataPackage[i] = recBytes[i + (UInt64)start + 4];
			}
			Buffer.BlockCopy(recBytes, start, Masking_key, 0, 4);
			for (UInt64 i = 0; i < mPackageLength; i++)
			{
				mDataPackage[i] = (byte)(mDataPackage[i] ^ Masking_key[i % 4]);
			}
			return Encoding.UTF8.GetString(mDataPackage);
		}
		#endregion

		#region 发送数据
		/// <summary>
		/// 把发送给客户端消息打包处理（拼接上谁什么时候发的什么消息）
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="message">Message.</param>
		private byte[] PackageServerData(string msg)
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
	}
}
