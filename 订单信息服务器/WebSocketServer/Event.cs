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
		private string _data;
		private readonly Session _session;
		public ClientNewMessageEventArgs(Session session,string data) {
			this.Data = data;
			this._session = session;
		}

		public string Data { get => _data; set => _data = value; }

		public Session Session => _session;
	}
}
