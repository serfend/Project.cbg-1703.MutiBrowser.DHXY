using SfTcp.TcpClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miner
{


	public static class MinerCallBack
	{

		private static Dictionary<string, Action<ClientMessageEventArgs>> dic;
		public static void Exec(ClientMessageEventArgs e) {
			dic.TryGetValue(e.Title, out Action<ClientMessageEventArgs> action);
			if (action == null)
			{
				throw new ActionNotRegException($"命令[{e.Title}]未被注册");
			}
			else
			{
				action.Invoke( e);
			}
		}
		static MinerCallBack()
		{
			Init();
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
		public static void Init()
		{
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
