using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Miner
{
	namespace Server
	{
		public class Server
		{
			public Reg ServerReg;
			public Reg ServerCmdBrowserReg;
			public static int DelayTime = 1500;
			private string id;
			private string serverName;
			private string aeroId;
			private string aeroName;
			static string GetServerBrowserId(string serverId)
			{
				return Program.setting.BrowserServerSeeting.GetInfo("Server" + serverId);
			}
			public Server(string id, string serverName, string aeroId, string aeroName)
			{
				this.Id = id;
				ServerReg = Program.setting.DataListServerReg.In("@" + id);
				ServerCmdBrowserReg = Program.setting.MainReg.In("ThreadCmd").In(GetServerBrowserId(id));
				this.ServerName = serverName;
				this.AeroId = aeroId;
				this.AeroName = aeroName;
			}
			public void Run(HttpClient http)
			{
				ReceiveInfo( http.GetAsync(TargetUrl));
				
			}
			private void ReceiveInfo(Task<HttpResponseMessage>response)
			{
				var myResponseStream = response.Result.Content.ReadAsStreamAsync().Result;
				using (var stream = new StreamReader(myResponseStream, Encoding.Default))
				{
					var str = stream.ReadToEnd();
					HdlResult(str);
				};
			}
			public void ExitAftert(string info, int delay = 10000)
			{
				Program.setting.threadSetting.Status =info.Length>10? info.Substring(0,10):info;
				Program.setting.LogInfo(info + "即将结束:" + delay,"主记录");
				Console.WriteLine("进程已结束,{0}ms后将退出此界面", delay);
				Thread.Sleep(delay);
				Environment.Exit(0);
			}
			private void HdlResult(string info)
			{

				var firstGoodInfoRaw = HttpUtil.GetElement(info, "generate_tips(", ")");
				if (firstGoodInfoRaw == null)
				{
					if (info.Contains("为了您的帐号安全，请登录之后继续访问"))
					{
						ExitAftert("失败:需登录", 1000);
					}
					else if (info.Contains("请输入验证码"))
					{
						ExitAftert("失败:需验证码.", 1000);
					}
					else if (info.Contains("系统繁忙"))
					{
						ExitAftert("失败:系统繁忙.", 1000);
					}
					else
					{
						ExitAftert("加载页面失败.", 1000);
						Logger.SysLog(info + "\n\n\n\n\n", "PageRaw");
					}
					return;
				}
				var firstGoodInfo = firstGoodInfoRaw.Split(new string[] { ", " }, StringSplitOptions.None);
				var firstGoodBookStatus = HttpUtil.GetElement(info, "<script>gen_bookind_btn(", ")");
				var firstGoodId = firstGoodInfo[1].Trim('\'');
				var firstGoodName = firstGoodInfo[2].Trim('\'');
				var firstGoodDetail = HttpUtil.GetElement(info, "<textarea id=\"large_equip_desc_", "/textarea>");
				firstGoodDetail = HttpUtil.GetElement(firstGoodDetail, ">", "<").Replace('\n', ' ');
				var firstGoodBookStatusInfo = firstGoodBookStatus.Split(new string[] { ", " }, StringSplitOptions.None);
				var firstGoodRank = HttpUtil.GetElement(info, "data_equip_level_desc=\"", "\"");
				firstGoodRank = firstGoodRank.Substring(2);
				var firstGood = new Goods.Goods(this, firstGoodName, firstGoodId, firstGoodDetail, HttpUtil.GetElement(info, "text-decoration:none;\" href=\"", "\""))
				{
					BookStatus = firstGoodBookStatusInfo[2],
					Price = firstGoodBookStatusInfo[1],
					Rank = firstGoodRank

				};
				firstGood.CheckAndSubmit();

			}
			public string TargetUrl
			{
				get => string.Format("http://xy2.cbg.163.com/cgi-bin/equipquery.py?act=fair_show_list&server_id={0}&areaid={1}&page=1&kind_id=45&query_order=create_time+DESC&server_name={2}&kind_depth=2", Id, AeroId, ServerName);
			}
			public string Id { get => id; set => id = value; }
			public string ServerName { get => serverName; set => serverName = value; }
			public string AeroId { get => aeroId; set => aeroId = value; }
			public string AeroName { get => aeroName; set => aeroName = value; }
		}
	}
}
