using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public class MsgSynFileListMessage : ITcpMessage
	{
		private string title = TcpMessageEnum.MsgSynFileList;
		private List<SynSingleFile> list;

		public MsgSynFileListMessage(List<SynSingleFile> list)
		{
			this.List = list;
			Console.WriteLine($"文件同步信息:{list.Count}");
		}

		public string Title { get => title; set => title = value; }
		public List<SynSingleFile> List { get => list; set => list = value; }
	}
	public class SynSingleFile
	{
		private string name;
		private string version;

		public string Name { get => name; set => name = value; }
		public string Version { get => version; set => version = value; }
	}
}
