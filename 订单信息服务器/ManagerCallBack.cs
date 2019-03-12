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
		private static ListViewItem targetItem
		{
			get
			{
				return s == null ? null : Item;
			}
		}
		private static TcpConnection connection;
		public static TcpConnection s { set {
				Item = null;
				connection = value;
			} get => connection; }

		public static ListViewItem Item { get {
				if (item == null) item = frm.GetItem(s.Ip);
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
			ServerCallBack.RegCallback(TcpMessageEnum.RpCheckBill, ServerCallBack_RpCheckBill);
			ServerCallBack.RegCallback(TcpMessageEnum.RpClientWait, ServerCallBack_RpClientWait);
			ServerCallBack.RegCallback(TcpMessageEnum.RpClientRunReady, ServerCallBack_RpClientRunReady);
			ServerCallBack.RegCallback(TcpMessageEnum.RpPayAuthKey, ServerCallBack_RpPayAuthKey);
			ServerCallBack.RegCallback(TcpMessageEnum.RpBillSubmited, ServerCallBack_RpBillSubmited);
			ServerCallBack.RegCallback(ServerCallBack.DefaultCallBack, ServerCallBack_Default);
		}
		private static void ServerCallBack_RpBillSubmited(ClientMessageEventArgs e)
		{
			switch (e.Message["Status"].ToString())
			{
				case "New":
					ServerCallBack_RpBuildBill(e);
					break;
				case "Fail":
					ServerCallBack_RpFailBill(e);
					break;
				case "Success":
					ServerCallBack_RpSuccessBill(e);
					break;
			}
		}
		private static void ServerCallBack_MsgHeartBeat(ClientMessageEventArgs e)
		{
			s.Send(new MsgHeartBeatMessage());
		}
		private static void ServerCallBack_RpClientConnect(ClientMessageEventArgs e)
		{
			frm.ClientConnect(e.Message, targetItem, s);
		}
		private static void ServerCallBack_RpNameModefied(ClientMessageEventArgs e)
		{
			frm.NameModefied(e.Message, targetItem, s);
		}
		private static void ServerCallBack_RpInitCompleted(ClientMessageEventArgs e)
		{
			frm.InitComplete(e.Message, targetItem, s);
		}
		private static void ServerCallBack_MsgSynFileList(ClientMessageEventArgs e)
		{
			frm.AppendLog("vps" + s.AliasName + "请求获取文件");
			frm.HdlVpsFileSynRequest(e.Message["List"], s);
		}
		private static void ServerCallBack_RpStatus(ClientMessageEventArgs e)
		{
			targetItem.SubItems[3].Text = e.Message["Status"].ToString();
			if (e.Message["Status"].ToString().Contains(" 失败"))
			{
				s.Send(new CmdReRasdialMessage());
			}
		}
		private static void ServerCallBack_RpCheckBill(ClientMessageEventArgs e)
		{
			frm.HdlNewCheckBill(s, targetItem, e.Message["BillInfo"].ToString()); 
		}
		private static void ServerCallBack_RpReRasdial(ClientMessageEventArgs e)
		{
			targetItem.SubItems[3].Text = "VPS重拨号中";
			s.Disconnect();
		}
		private static void ServerCallBack_RpClientWait(ClientMessageEventArgs e)
		{
			frm.ClientWaiting(e.Message, targetItem, s);
		}
		private static void ServerCallBack_RpClientRunReady(ClientMessageEventArgs e)
		{
			targetItem.SubItems[3].Text = "初始化完成";
			frm.NewVpsAvailable(s.Ip);
		}

		private static void ServerCallBack_RpPayAuthKey(ClientMessageEventArgs e)
		{
			frm.AuthKey = e.Message["content"].ToString();
		}
		private static void ServerCallBack_RpBuildBill(ClientMessageEventArgs e)
		{
			targetItem.SubItems[3].Text = "开始下单";
		}
		private static void ServerCallBack_RpFailBill(ClientMessageEventArgs e)
		{
			targetItem.SubItems[3].Text = $"下单无效:{e.Message["Content"].ToString()}";
		}
		private static void ServerCallBack_RpSuccessBill(ClientMessageEventArgs e)
		{
			targetItem.SubItems[3].Text = "成功下单,即将付款";
			new Thread(() =>
			{
				frm.PayCurrentBill(frm._clientPayUser[s.Ip], e.Message["Content"].ToString(), (msg) =>
				{
					frm.Invoke((EventHandler)delegate
					{
						targetItem.SubItems[3].Text = msg;
					});
				});
			}).Start();
		}
		private static void ServerCallBack_Default(ClientMessageEventArgs e)
		{
			frm.AppendLog($"新消息[{s.AliasName}] {e.Title}:{e.Message["Content"]}");
			targetItem.SubItems[3].Text = e.Title;
		}
	}
	public static class ServerCallBack
	{
		public const string DefaultCallBack = "DefaultCallBack";
		private static Dictionary<string, Action<ClientMessageEventArgs>> dic;
		public static void Exec(object sender,ClientMessageEventArgs e) {
			var title = e.Title;
			dic.TryGetValue(title, out Action<ClientMessageEventArgs> action);
			if (action == null)
			{
				dic.TryGetValue(DefaultCallBack,out action);
				if(action==null) throw new ActionNotRegException($"命令[{title}]未被注册");
			}
			ServerCallBackStatic.s = sender as TcpConnection;
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
