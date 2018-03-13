using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilHttp;
using DotNet4.Utilities.UtilReg;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Miner
{
	class ServerList
	{
		private Dictionary<string, Server> serverList;
		private List<Server> hdlServer;
		public ServerList()
		{
			hdlServer = new List<Server>();
			Program.setting.LogInfo("ServerListInitilize");
			serverList = new Dictionary<string, Server>();
			var ServerInfo = Program.setting.MainReg.In("Setting").In("ServerInfo").In("ServerList");
			var index = 1;
			do
			{
				var thisServer = ServerInfo.GetInfo("Server " + index);
				if (thisServer == "") break;
				index++;
				var info = thisServer.Split(',');
				serverList[info[0]] = new Server(info[0],info[1],info[2],info[3]);
			} while (true);
		}
		public void ResetTask(string taskCmd)
		{
			if (taskCmd == "#subClose#")
			{
				Program.setting.LogInfo("进程通过命令被终止");
				Environment.Exit(0);
			}
			var tasks = taskCmd.Split('#');
			hdlServer = new List<Server>(tasks.Length);
			foreach (var task in tasks)
			{
				if (task == "") continue;
				var target = serverList.ContainsKey(task)? serverList[task]:null;
				if (target == null) Program.setting.LogInfo("无效的服务器:" + task);
				else hdlServer.Add(target);
			}
			Program.setting.LogInfo(string.Format("目标服务器加载完成,共计{0}个", hdlServer.Count));
		}
		private int runTimeRecord = 1;
		public void Run()
		{
			
			RunTimeRecord();
			Main();
		}
		private void RunTimeRecord()
		{
			Program.setting.threadSetting.RefreshRunTime();
			Thread.Sleep(500);
			new Task(() =>
			{
				RunTimeRecord();
			}).Start();
		}
		private void Main()
		{
			if (hdlServer.Count == 0)
			{
				Program.setting.LogInfo("无需要处理的服务器");
				Thread.Sleep(Server.DelayTime);
			}
			else
				Program.setting.LogInfo("第" + runTimeRecord++ + "次运行开始");
			var config = Program.setting.threadSetting;
			var haveRefresh = config.ThisThreadIsRefresh;
			if (haveRefresh)
			{
				Program.setting.LogInfo("更新设置");
				ResetTask(config.Task);
				Server.DelayTime = config.DelayTime;
				Server.DelayTime = Server.DelayTime <= 100 ? 100 : Server.DelayTime;
			}
			foreach(var server in hdlServer)
			{
				server.Run();
				Program.setting.LogInfo(string.Format("{1} 采集结束,{0}ms后重新开始", Server.DelayTime,server.ServerName));
				Thread.Sleep(Server.DelayTime);
			}
			new Task(() => Main()).RunSynchronously();
		}
		internal class Server
		{
			private Reg reg;
			public static int DelayTime=1500;
			private string id;
			private string serverName;
			private string aeroId;
			private string aeroName;

			public Server(string id, string serverName, string aeroId, string aeroName)
			{
				this.Id = id;
				this.ServerName = serverName;
				this.AeroId = aeroId;
				this.AeroName = aeroName;
			}
			public void Run()
			{
				var result = HttpClient.http.GetHtml(TargetUrl).document.response;
				HdlResult(result.DataString(Encoding.Default));
			}
			private void HdlResult(string info)
			{

				var firstGoodInfoRaw = HttpUtil.GetElement(info, "generate_tips(", ")");
				if (firstGoodInfoRaw == null)
				{
					if (info.Contains("为了您的帐号安全，请登录之后继续访问"))
					{
						Program.setting.LogInfo("请登录之后继续访问.");
					}
					else if (info.Contains("请输入验证码"))
					{
						Program.setting.LogInfo("需要验证码.");
					}
					else if (info.Contains("系统繁忙"))
					{
						Program.setting.LogInfo("系统繁忙.");
					}
					else
					{
						Program.setting.LogInfo("加载页面失败.");
						Logger.SysLog(info+"\n\n\n\n\n", "PageRaw");
					}
					return;
				}
				var firstGoodInfo= firstGoodInfoRaw.Split(new string[] {", " },StringSplitOptions.None);
				var firstGoodId = firstGoodInfo[1].Trim('\'');
				var firstGoodName = firstGoodInfo[2].Trim('\'');
				var firstGoodDetail = HttpUtil.GetElement(info, "<textarea id=\"large_equip_desc_", "/textarea>");
				firstGoodDetail = HttpUtil.GetElement(firstGoodDetail, ">", "<").Replace('\n',' ');
				var firstGood = new Goods(firstGoodName, firstGoodId, firstGoodDetail);
			}
			public string TargetUrl
			{
				get => string.Format("http://xy2.cbg.163.com/cgi-bin/equipquery.py?act=fair_show_list&server_id={0}&areaid={1}&page=1&kind_id=45&query_order=create_time+DESC&server_name={2}&kind_depth=2",Id,AeroId,ServerName);
			}
			public string Status
			{
				get => reg.GetInfo("Status");
				set => reg.SetInfo("Status", value);
			}
			public string Id { get => id; set => id = value; }
			public string ServerName { get => serverName; set => serverName = value; }
			public string AeroId { get => aeroId; set => aeroId = value; }
			public string AeroName { get => aeroName; set => aeroName = value; }
		}
		internal class Goods
		{
			private string name;
			private string id;
			public Goods(string name,string id,string info)
			{
				this.name = name;
				this.id = id;
				var convertInfo = info.Replace("([", "{").Replace("])","}");
				convertInfo = convertInfo.Replace("({", "{").Replace("})", "}");
				convertInfo = convertInfo.Replace(",}", "}");
				var node = new Node();
				node.Init(ref convertInfo, 0, convertInfo.Length);
				Debug.Print(node.ToString(0));
			}
			internal class Node
			{
				public Node parent;
				public Node next;
				public List<Node> child;
				public int nodeBegin, nodeEnd;
				private string key;
				private string value;
				public string Data {
					get => value;
					set=> this.value = value;
				}
				public Node this[string key]
				{
					get
					{
						foreach (var node in child)
						{
							if (node.Key == key) return node;
						}
						return null;
					}
				}
				public  string ToString(int Rank)
				{
					var node = this;
					StringBuilder cstr = new StringBuilder();
					var lineSpace = new string(' ', Rank);
					cstr.Append(lineSpace).Append('{').Append('\n');
					do
					{
						cstr.Append(lineSpace).Append(' ');
						if (node.child.Count > 0)
						{
							cstr.Append("list[").Append(node.child.Count).Append("]\n");
							foreach(var childNode in node.child)
							{
								cstr.Append(childNode.ToString(Rank + 1));
							}
						}
						else
						{
							cstr.Append(node.Key).Append(node.Data).Append('\n');
						}
						
						node = node.next;
					} while (node != null);
					cstr.Append(lineSpace).Append('}').Append('\n');
					return cstr.ToString();
				}
				public string Key { get => key; set => this.key = value; }
				public Node(Node parent = null)
				{
					this.parent = parent;
					child = new List<Node>();
				}
				public void Init(ref string info,int start,int length)
				{
					bool matchingString = false;
					nodeBegin = start;
					for (int i = start; i < length; i++)
					{
						if (matchingString)
						{
							if (info[i] == '"')//忽略所有引号内的内容
							{
								matchingString = false;
							}
							continue;
						}
						switch (info[i])
						{
							case '{': {
									Debug.Print("NewNode");
									var child = new Node(this);
									child.Init(ref info, i + 1, length);
									this.child.Add(child);
									i = child.nodeEnd + 2;
									break;
								}
							case '}':
								{
									nodeEnd = i - 1;
									Debug.Print("EndNode:"+ info.Substring(nodeBegin, nodeEnd - nodeBegin + 1));
									Data = info.Substring(nodeBegin, nodeEnd - nodeBegin + 1);
									return;
								}
							case ':':
								{
									Debug.Print("NodeKey:" + info.Substring(nodeBegin, i - nodeBegin));
									key = info.Substring(nodeBegin, i - nodeBegin);
									nodeBegin = i + 1;
									break;
								}
							case '"':
								{
									matchingString = true;
									break;
								}
							case ',':
								{
									Debug.Print("NextNode:"+ info.Substring(nodeBegin, i - nodeBegin));
									Data = info.Substring(nodeBegin, i - nodeBegin);
									next = new Node(parent);
									next.Init(ref info, i + 1, length);
									nodeEnd = next.nodeEnd;
									return;
								}
						}
					}
					nodeEnd = length;
				}
			}
		}
	}
}
