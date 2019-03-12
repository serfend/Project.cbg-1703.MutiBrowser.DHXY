using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Miner.Server;
using Miner.Goods.Equiment;

namespace Miner
{
	 namespace Goods {
		public class Goods
		{
			private string name;
			private string id;
			private readonly Node goodInfo;
			private string bookStatus;//可预订:2,已预订:4
			private string price;
			private double assumePrice = -1;//本地估价

			public static void StaticInit()
			{
				ITalentPriceHandle = new SortedDictionary<double, double>();
				IAchievementPriceHandle = new SortedDictionary<double, double>();
				RankRule = new SortedDictionary<string, double>();
			}
			public static void SetTalentRule(string info)
			{
				var ruleList = info.Split(',');
				foreach (var rule in ruleList)
				{
					var ru = rule.Split(':');
					ITalentPriceHandle.Add(Convert.ToDouble(ru[0]), Convert.ToDouble(ru[1]));
				}
			}
			public static void SetAchievementRule(string info)
			{
				var ruleList = info.Split(',');
				foreach (var rule in ruleList)
				{
					var ru = rule.Split(':');
					IAchievementPriceHandle.Add(Convert.ToDouble(ru[0]), Convert.ToDouble(ru[1]));
				}
			}
			public static void SetRankRule(string info)
			{
				var ruleList = info.Split(',');
				foreach (var rule in ruleList)
				{
					var ru = rule.Split(':');
					RankRule.Add(ru[0], Convert.ToDouble(ru[1]));
				}
			}
			private string rank;//等级数值
			private string iFlyupFlag;//是否飞升
			private string iTalent;//天赋点数
			private string iAchievement;//功绩点数
			private string iChengjiu;//帮派成就
			private string iSingleEnergyRate;//家具回灵

			private string iCash;//现金
			private string iSaving;//存款

			private List<Summon.Summon> summons;
			private List<Equipment> equipments;
			private Server.Server server;
			private string buyUrl;
			public void CheckAndSubmit()
			{

				if (Exist)
				{
					return;
				}
				else
				{
					//TODO 关闭内部估价日志
					//var cstr = new StringBuilder("新物品召唤兽种类:\n");
					//foreach (var summon in summons)
					//{
					//	cstr.AppendLine(summon.ID);
					//}
					//Program.setting.LogInfo(cstr.ToString(), server.ServerName);
				}
				//TODO vps下单
				//Server.Server.NewCheckBill(BuyUrl,MainInfo,server.LoginSession);

				Program.Tcp?.Send(new SfTcp.TcpMessage.RpCheckBillMessage(MainInfo));//服务器下单
			}
			public string MainInfo
			{
				get
				{
					StringBuilder cstr = new StringBuilder();
					string split = "##";
					cstr.Append(server.ServerName).Append(split).
						Append(Name).Append(split).
						Append(Price).Append("/").Append(AssumePrice).Append(split).
						Append(Rank).Append(split).
						Append(ITalent).Append(split).
						Append(IAchievement).Append(split).
						Append(IChengjiu).Append(split).
						Append(ISingleEnergyRate).Append(split).
						Append(BuyUrl).Append(split).
						Append(server.Id);
					return cstr.ToString();
				}
			}
			public bool Exist
			{
				get
				{
					bool result = this.server.ServerReg.GetInfo(ID) == "exist";
					if (!result) this.server.ServerReg.SetInfo(ID, "exist");
					return result;
				}
			}
			private void Init()
			{
				//rank = goodInfo["iGrade"].Data;  此来源不准确
				IFlyupFlag = goodInfo["iFlyupFlag"].Data;
				ITalent = goodInfo["iTalent"].Data;
				IAchievement = goodInfo["iAchievement"].Data;
				IChengjiu = goodInfo["iChengjiu"].Data;
				ISingleEnergyRate = goodInfo["iSingleEnergyRate"].Data;
				ICash = goodInfo["iCash"].Data;
				ISaving = goodInfo["iSaving"].Data;
				var SummonList = goodInfo["SummonList"];
				summons = new List<Summon.Summon>();
				foreach (var summon in SummonList)
				{
					var thisSummon = new Summon.Summon(summon,server);
					if (thisSummon.Valid) summons.Add(thisSummon);
				}
				var equipments = goodInfo["mpEquip"];//利用itype获取装备名称
				this.equipments = new List<Equipment>();
				foreach (var equipment in equipments.child)
				{
					var tmpRquiment = new Equipment(equipment,server);
					if(tmpRquiment.Init)
						this.equipments.Add(tmpRquiment);
				}
				equipments = goodInfo["ItemList"];
				foreach (var equipment in equipments.child)
				{
					var tmpEquiment = new Equipment(equipment,server);
					if (tmpEquiment.Init)
						this.equipments.Add(tmpEquiment);
				}
				equipments=goodInfo["mpFabao"];
				foreach (var equipment in equipments.child)
				{
					var tmpEquiment = new Equipment(equipment, server);
					if (tmpEquiment.Init)
						this.equipments.Add(tmpEquiment);
				}
				equipments = goodInfo["DiXingList"];
				foreach (var equipment in equipments.child)
				{
					var tmpEquiment = new Equipment(equipment, server);
					if (tmpEquiment.Init)
						this.equipments.Add(tmpEquiment);
				}
				
			}
			private double GetPrice()
			{
				var goodsPrice = GetGoodsPrice();
				var summonPrice = GetSummonsPrice();
				var equimentPrice = GetEquimentPrice();

				var sumPrice = goodsPrice + summonPrice + equimentPrice;
				//var priceRate = Convert.ToDouble(Program.setting.MainReg.In("Setting").In("Price").GetInfo("rate", "100"));
				var priceRate =Miner.Server.Server.AssumePriceRate;
				//Program.setting.LogInfo($"总估价:({goodsPrice}+{summonPrice}+{equimentPrice})*{priceRate}={summonPrice}*{priceRate}%={summonPrice*priceRate/100}),TimeStamp={HttpUtil.TimeStamp}",server.ServerName);
				return sumPrice*priceRate/100;
			}
			private double GetGoodsPrice()
			{
				double ITalentPrice = 0;
				double IAchievementPrice = 0;
				var thisTalent = Convert.ToDouble(this.ITalent);
				var thisAchievement = Convert.ToDouble(this.IAchievement);
				foreach (var talentRank in ITalentPriceHandle)
				{
					if (thisTalent > talentRank.Key) ITalentPrice = talentRank.Value;
					else break;
				}
				foreach (var achievementRank in IAchievementPriceHandle)
				{
					if (thisAchievement > achievementRank.Key) IAchievementPrice = achievementRank.Value;
					else break;
				}
				double IRankPrice;
				if (this.IFlyupFlag == "1")
					IRankPrice = Convert.ToDouble(RankRule.ContainsKey("飞升") ? RankRule["飞升"] : -40404);
				else
					IRankPrice = Convert.ToDouble(RankRule.ContainsKey("其他") ? RankRule["其他"] : -40406);
				var assert = Convert.ToDouble(ICash) + Convert.ToDouble(ISaving);
				assert /= 550000;//设置为单位财产价值
				assert = Math.Round(assert, 0);
				var 帮派成就价 =Math.Round( Convert.ToDouble(IChengjiu)/2000,0);
				var 家具回灵价 = Convert.ToDouble(iSingleEnergyRate) >= 500 ? 60 : 0;
				//Program.setting.LogInfo(string.Format("人物估价: Max(等级({0}),{6}等级=> 天赋({1})+成就({2}))+财产({3})+帮派成就({4})+家具回灵({5})", IRankPrice, ITalentPrice, IAchievementPrice, assert, 帮派成就价, 家具回灵价, IFlyupFlag == "1" ? "飞升" : "3转"),server.ServerName);
				return Math.Max( IRankPrice, this.IFlyupFlag == "1"?ITalentPrice + IAchievementPrice:0)+ assert+ 帮派成就价+ 家具回灵价;
			} 
			private double GetSummonsPrice()
			{
				double sumPrice = 0;
				foreach (var summon in summons)
				{
					sumPrice += summon.Price;
				}
				return sumPrice;
			}
			private double GetEquimentPrice()
			{
				double sumPrice = 0;
				//Program.setting.LogInfo(string.Format("共有{0}件装备", equipments.Count),server.ServerName);
				foreach(var equipment in equipments)
				{
					sumPrice += equipment.Price;
				}
				return sumPrice;
			}
			public static SortedDictionary<double, double> ITalentPriceHandle;
			public static SortedDictionary<double, double> IAchievementPriceHandle;
			public static SortedDictionary<string, double> RankRule;
			private string GetValue(string keyWord)
			{
				switch (keyWord)
				{
					case "Name": return this.Name;
					case "rank": return this.rank;
					case "iFlyupFlag": return this.IFlyupFlag;
					case "iTalent": return this.ITalent;
					case "iAchievement": return this.IAchievement;
					case "iChengjiu": return this.IChengjiu;

					case "iSingleEnergyRate": return this.ISingleEnergyRate;

					case "SummonNum": return this.summons.Count.ToString();
					case "SummonName":
						{
							StringBuilder cstr = new StringBuilder();
							foreach (var summon in this.summons)
								cstr.Append(summon.Name).Append(',');
							return cstr.ToString(0, cstr.Length - 1);
						}
					case "SummonRank":
						{
							StringBuilder cstr = new StringBuilder();
							foreach (var summon in this.summons)
								cstr.Append(summon.Rank).Append(',');
							return cstr.ToString(0, cstr.Length - 1);
						}
					case "SummonSpecial":
						{
							StringBuilder cstr = new StringBuilder();
							foreach (var summon in this.summons)
								cstr.Append(summon.HaveSpecialSkill ? "1" : "0").Append(',');
							return cstr.ToString(0, cstr.Length - 1);
						}
					case "SummoncDesc":
						{
							StringBuilder cstr = new StringBuilder();
							foreach (var summon in this.summons)
								cstr.Append(summon.CDesc).Append(',');
							return cstr.ToString(0, cstr.Length - 1);
						}
					case "SummonSkillName":
						{
							StringBuilder cstr = new StringBuilder();
							foreach (var summon in this.summons)
								foreach (var skill in summon.Skills)
									cstr.Append(skill.Name).Append(',');
							return cstr.ToString(0, cstr.Length - 1);
						}
					case "SummonSkillNum":
						{
							StringBuilder cstr = new StringBuilder();
							foreach (var summon in this.summons)
								cstr.Append(summon.Skills.Count).Append(',');
							return cstr.ToString(0, cstr.Length - 1);
						}
					case "SummonSpecialSkill":
						{
							StringBuilder cstr = new StringBuilder();
							foreach (var summon in this.summons)
								cstr.Append(summon.HaveSpecialSkill ? "1" : "0").Append(',');
							return cstr.ToString(0, cstr.Length - 1);
						}
					default:
						{
							throw new NotImplementedException("不支持的关键字:" + keyWord);
						}

				}
			}
			/// <summary>
			/// 用于估价的商品
			/// </summary>
			/// <param name="ownerServer"></param>
			/// <param name="name"></param>
			/// <param name="id">商品的ordersn</param>
			/// <param name="info">商品内置原始数据</param>
			/// <param name="buyUrl"></param>
			public Goods(Server.Server ownerServer, string name, string id, string info, string buyUrl)
			{
				this.BuyUrl = buyUrl;
				this.server = ownerServer;
				this.Name = name;
				this.ID = id;
				var convertInfo = info.Replace("([", "{").Replace("])", "}");
				convertInfo = convertInfo.Replace("({", "{").Replace("})", "}");
				this.goodInfo = new Node();
				this.goodInfo.Init(ref convertInfo, 0, convertInfo.Length);
				Init();
			}

			public string Name { get => name; set => name = value; }
			public string ID { get => id; set => id = value; }
			public string BookStatus { get => bookStatus; set => bookStatus = value; }
			public string Price { get => price; set => price = value; }
			public string Rank { get =>  rank; set => rank = value; }
			public string BuyUrl { get => buyUrl; set => buyUrl = value; }
			public string IFlyupFlag { get => iFlyupFlag; set => iFlyupFlag = value; }
			public string ITalent { get => iTalent; set => iTalent = value; }
			public string IAchievement { get => iAchievement; set => iAchievement = value; }
			public string IChengjiu { get => IChengjiu1; set => IChengjiu1 = value; }
			public string ISingleEnergyRate { get => ISingleEnergyRate1; set => ISingleEnergyRate1 = value; }
			public double AssumePrice
			{
				get
				{
					if (assumePrice == -1) assumePrice = GetPrice();
					return Math.Round(assumePrice,0);
				}
			}

			public string IChengjiu1 { get => iChengjiu; set => iChengjiu = value; }
			public string ISingleEnergyRate1 { get => iSingleEnergyRate; set => iSingleEnergyRate = value; }
			public string ICash { get => iCash; set => iCash = value; }
			public string ISaving { get => iSaving; set => iSaving = value; }
		}

	}
}
