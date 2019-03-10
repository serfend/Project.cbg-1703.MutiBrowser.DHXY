using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SfTcp.TcpMessage
{
	public class RpClientConnectMessage:ITcpMessage
	{
		private string title = TcpMessageEnum.RpClientConnect;
		private string type;
		private string version;
		private string deviceId;
		private string name;

		public RpClientConnectMessage(string type, string version, string deviceId, string name)
		{
			this.type = type;
			this.version = version;
			this.deviceId = deviceId;
			this.name = name;
		}

		public string Type { get => type; set => type = value; }
		public string Version { get => version; set => version = value; }
		public string DeviceId { get => deviceId; set => deviceId = value; }
		public string Name { get => name; set => name = value; }
		public string Title { get => title; set => title = value; }
	}
}
