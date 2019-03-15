using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilReg;
using Miner.util;
using SfTcp.TcpMessage;
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
			public Reg ServerReg = new Reg("sfMinerDigger").In("Main").In("Setting").In("goodhistory");
			public static int DelayTime = 1500;
			private string id;
			private string serverName;
			private string aeroId;
			private string aeroName;
			private string loginSession;
			public Server(string id, string serverName, string aeroId, string aeroName,string loginSession)
			{
				this.Id = id;
				this.ServerName = serverName;
				this.AeroId = aeroId;
				this.AeroName = aeroName;
				this.loginSession = loginSession;
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
			public static void ExitAftert(string info)
			{
				Program.Tcp?.Send(new RpReRasdialMessage(info));
				var logInfo = $"{info}->进程关闭";
				Console.WriteLine(logInfo);
				Program.setting.LogInfo(info + "进程关闭","主记录");

				Program.RedialToInternet();
				Program.vpsStatus = Program.VpsStatus.WaitConnect;
			}
			private void HdlResult(string info)
			{

				var firstGoodInfoRaw = HttpUtil.GetElement(info, "generate_tips(", ")");
				if (firstGoodInfoRaw == null)
				{
					if (info.Contains("为了您的帐号安全，请登录之后继续访问"))
					{
						ExitAftert("需登录 失败");
					}
					else if (info.Contains("请输入验证码"))
					{
						ExitAftert("需验证码 失败");
					}
					else if (info.Contains("系统繁忙"))
					{
						ExitAftert("系统繁忙 失败");
					}
					else if (info.Contains("该服务器已被合服"))
					{
						ExitAftert("已合服");
					}
					else if (info.Contains("请输入正确的服务器名称"))
					{
						ExitAftert("服务器名称无效");
					}
					else
					{
						ExitAftert("加载页面失败.");
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

			internal static void NewCheckBill(string url,string mainInfo,string loginSession)
			{
				var handler = new HttpClientHandler() { UseCookies = false };
				var client = new HttpClient(handler);
				client.DefaultRequestHeaders.Add("Cookie", loginSession);
				Program.Tcp?.Send(new RpCheckBillMessage(mainInfo));
				var resultStream=client.GetAsync(url).Result.Content.ReadAsStreamAsync().Result;
				using (var reader = new StreamReader(resultStream, Encoding.Default))
				{
					var info = reader.ReadToEnd();
					var frmInfo = HttpUtil.GetElement(info, "usertrade.py", "返回");
					var billPoster = new BillPoster(frmInfo, loginSession);
					billPoster.Submit((result, success) =>
					{
						if (success)
						{
							var t = new Task(() =>
							{
								Program.Tcp?.Send(new RpBillSubmitedMessage(RpBillSubmitedMessage.State.Success));
								Program.setting.LogInfo( $"下单成功 {url}", "下单记录");
							}
								);
							t.Start();
						}
						else
						{
							var t = new Task(() =>
							{
								Program.Tcp?.Send(new RpBillSubmitedMessage(RpBillSubmitedMessage.State.Fail));
								Program.setting.LogInfo( $"{result}\n{url}","下单记录");
							}
							);
							t.Start();
						};
					});

				}

			}

			public string TargetUrl
			{
				get => string.Format("http://xy2.cbg.163.com/cgi-bin/equipquery.py?act=fair_show_list&server_id={0}&areaid={1}&page=1&kind_id=45&query_order=create_time+DESC&server_name={2}&kind_depth=2", Id, AeroId, ServerName);
			}
			public string Id { get => id; set => id = value; }
			public string ServerName { get => serverName; set => serverName = value; }
			public string AeroId { get => aeroId; set => aeroId = value; }
			public string AeroName { get => aeroName; set => aeroName = value; }
			public static double AssumePriceRate { get; internal set; }
			public string LoginSession { get => loginSession; set => loginSession = value; }
		}
	}
}
