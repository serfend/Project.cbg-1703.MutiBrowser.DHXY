using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 订单信息服务器.WebSocketServer
{
	public class ClientConnectEventArgs : EventArgs				
	{
		private Session _session;

		public ClientConnectEventArgs(Session session)
		{
			Session = session;
		}

		public Session Session { get => _session; set => _session = value; }
	}
	public class ClientDisconnectEventArgs : EventArgs 
	{
		private Session _session;
		public ClientDisconnectEventArgs(Session session){
			this._session = session;
		}

		public Session Session { get => _session; set => _session = value; }
	}
	public class ClientNewMessageEventArgs : EventArgs
	{
		private string _msg;
		private readonly Session _session;
		public ClientNewMessageEventArgs(Session session,string msg) {
			this.Msg = msg;
			this._session = session;
		}

		public Session Session => _session;
		/// <summary>
		/// 来自客户端的数据
		/// </summary>
		public string Msg { get => _msg; set => _msg = value; }
	}
}
