using SfTcp.TcpMessage;
using SfTcp.TcpServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 订单信息服务器;

namespace Server
{
	public static class ServerCallBackStatic
	{
		private static Form1 frm;
		
		
		private static ListViewItem item;
		private static ListViewItem TargetItem
		{
			get
			{
				if (S == null)
				{
					MessageBox.Show("禁止在Connection初始化前使用实例");
					return null;
				}
				return Item;
			}
		}
		private static TcpConnection connection;
		public static TcpConnection S { set {
				Item = null;
				connection = value;
			} get => connection; }

		public static ListViewItem Item { get {
				if (item == null) item = frm.GetItem(S.Ip);
				if (item == null)
				{
					MessageBox.Show($"未能找到{S.Ip}对应的实例");
					return null;
				}
				return item;
			} set => item = value; }
		public static void Init(Form1 invoker)
		{
			Server.ServerCallBack.Init(invoker);
			frm = invoker;
			InitCallBack();
		}
		public static void InitCallBack()
		{
			ServerCallBack.RegCallback(TcpMessageEnum.MsgHeartBeat, ServerCallBack_MsgHeartBeat);
			ServerCallBack.RegCallback(TcpMessageEnum.RpClientConnect, ServerCallBack_RpClientConnect);
			ServerCallBack.RegCallback(TcpMessageEnum.RpNameModefied, ServerCallBack_RpNameModefied);
			ServerCallBack.RegCallback(TcpMessageEnum.RpInitCompleted, ServerCallBack_RpInitCompleted);
			ServerCallBack.RegCallback(TcpMessageEnum.MsgSynFileList, ServerCallBack_MsgSynFileList);
			ServerCallBack.RegCallback(TcpMessageEnum.RpStatus, ServerCallBack_RpStatus);
			ServerCallBack.RegCallback(TcpMessageEnum.RpReRasdial, ServerCallBack_RpReRasdial);
			ServerCallBack.RegCallback(TcpMessageEnum.RpCheckBill, ServerCallBack_RpCheckBill);
			ServerCallBack.RegCallback(TcpMessageEnum.RpClientWait, ServerCallBack_RpClientWait);
			ServerCallBack.RegCallback(TcpMessageEnum.RpClientRunReady, ServerCallBack_RpClientRunReady);
			ServerCallBack.RegCallback(TcpMessageEnum.RpPayAuthKey, ServerCallBack_RpPayAuthKey);
			ServerCallBack.RegCallback(TcpMessageEnum.RpBillSubmited, ServerCallBack_RpBillSubmited);
			ServerCallBack.RegCallback(ServerCallBack.DefaultCallBack, ServerCallBack_Default);
		}
		private static void ServerCallBack_RpBillSubmited(ClientMessageEventArgs e)
		{
			switch (e.Message["Status"]?.ToString())
			{
				case "0":
					ServerCallBack_RpBuildBill(e);
					break;
				case "2":
					ServerCallBack_RpFailBill(e);
					break;
				case "1":
					ServerCallBack_RpSuccessBill(e);
					break;
			}
		}
		private static void ServerCallBack_MsgHeartBeat(ClientMessageEventArgs e)
		{
			if(!TargetItem.SubItems[3].Text.Contains("心跳")) TargetItem.SubItems[3].Text += "心跳";
			else
			{
				TargetItem.SubItems[3].Text=TargetItem.SubItems[3].Text.Replace("心跳", "");
			}
			S.Send(new MsgHeartBeatMessage());
		}
		private static void ServerCallBack_RpClientConnect(ClientMessageEventArgs e)
		{
			frm.ClientConnect(e.Message, TargetItem, S);
		}
		private static void ServerCallBack_RpNameModefied(ClientMessageEventArgs e)
		{
			frm.NameModefied(e.Message, TargetItem, S);
		}
		private static void ServerCallBack_RpInitCompleted(ClientMessageEventArgs e)
		{
			frm.InitComplete(e.Message, TargetItem, S);
		}
		private static void ServerCallBack_MsgSynFileList(ClientMessageEventArgs e)
		{
			frm.AppendLog("vps" + S.AliasName + "请求获取文件");
			frm.HdlVpsFileSynRequest(e.Message["List"], S);
		}
		private static void ServerCallBack_RpStatus(ClientMessageEventArgs e)
		{
			
			var status = e.Message["Status"]?.ToString();
			status = status ?? "未知";
			TargetItem.SubItems[3].Text = status;
			if (status.Contains(" 失败"))
			{
				S.Send(new CmdReRasdialMessage());
			}
		}
		private static void ServerCallBack_RpCheckBill(ClientMessageEventArgs e)
		{
			frm.HdlNewCheckBill(S, TargetItem, e.Message["BillInfo"]?.ToString()); 
		}
		private static void ServerCallBack_RpReRasdial(ClientMessageEventArgs e)
		{
			TargetItem.SubItems[3].Text = $"重启:{e.Message["Reason"]?.ToString()}";
			S.Disconnect();
		}
		private static void ServerCallBack_RpClientWait(ClientMessageEventArgs e)
		{
			frm.ClientWaiting(e.Message, TargetItem, S);
		}
		private static void ServerCallBack_RpClientRunReady(ClientMessageEventArgs e)
		{
			TargetItem.SubItems[3].Text = "初始化完成";
			frm.NewVpsAvailable(S.Ip);
		}

		private static void ServerCallBack_RpPayAuthKey(ClientMessageEventArgs e)
		{
			frm.AuthKey = e.Message["AuthKey"]?.ToString();
		}
		private static void ServerCallBack_RpBuildBill(ClientMessageEventArgs e)
		{
			TargetItem.SubItems[3].Text = "开始下单";
		}
		private static void ServerCallBack_RpFailBill(ClientMessageEventArgs e)
		{
			TargetItem.SubItems[3].Text = $"{e.Message["R"]?.ToString()}";
		}
		private static void ServerCallBack_RpSuccessBill(ClientMessageEventArgs e)
		{
			TargetItem.SubItems[3].Text = "成功下单,即将付款";
			new Thread(() =>
			{
				frm.PayCurrentBill(frm._clientPayUser[S.Ip], e.Message["Content"]?.ToString(), (msg) =>
				{
					frm.Invoke((EventHandler)delegate
					{
						TargetItem.SubItems[3].Text = msg;
					});
				});
			}).Start();
		}
		private static void ServerCallBack_Default(ClientMessageEventArgs e)
		{
			frm.AppendLog($"新消息[{S.AliasName}] {e.Title}:{e.Message["Content"]}");
			TargetItem.SubItems[3].Text = e.Title;
		}
	}
	public static class ServerCallBack
	{
		public const string DefaultCallBack = "DefaultCallBack";
		private static Dictionary<string, Action<ClientMessageEventArgs>> dic;
		public static void Exec(object sender,ClientMessageEventArgs e) {
			Action<ClientMessageEventArgs> action;
			lock (dic)
			{
				var title = e.Title;
				dic.TryGetValue(title, out  action);
				if (action == null)
				{
					dic.TryGetValue(DefaultCallBack, out action);
					if (action == null) throw new ActionNotRegException($"命令[{title}]未被注册");
				}
				ServerCallBackStatic.S = sender as TcpConnection;
				if (ServerCallBackStatic.S == null)
				{
					MessageBox.Show($"CallBack.Exec发现无效的执行,Conncetion未实例\n{e.RawString}");
					return;
				}
			}
			frm.Invoke(action, new object[] { e });
		}
		static ServerCallBack()
		{
			
		}
		public static void RegCallback(string title,Action<ClientMessageEventArgs> CallBack)
		{
			if (dic.ContainsKey(title))
			{
				throw new CallbackBeenRegException();
			}
			else
			{
				dic.Add(title, CallBack);
			}
		}
		private static 订单信息服务器.Form1 frm;
		public static void Init(订单信息服务器.Form1 invoker)
		{
			frm = invoker;
			dic = new Dictionary<string, Action<ClientMessageEventArgs>>();
		}
	}


	[Serializable]
	public class CallbackBeenRegException : Exception
	{
		public CallbackBeenRegException() { }
		public CallbackBeenRegException(string message) : base(message) { }
		public CallbackBeenRegException(string message, Exception inner) : base(message, inner) { }
		protected CallbackBeenRegException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
	[Serializable]
	public class ActionNotRegException : Exception
	{
		public ActionNotRegException() { }
		public ActionNotRegException(string message) : base(message) { }
		public ActionNotRegException(string message, Exception inner) : base(message, inner) { }
		protected ActionNotRegException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
