using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Miner
{
namespace Goods.Summon
	{
		public class SummomPriceRule
		{

			public static Dictionary<string, SummomPriceRule> SummomPriceRuleList;

			public static SortedList<double, double> 成长值list;

			public readonly string RuleId;

			public static double 成长值范围(double value)
			{
				double bestValue = 0;
				foreach (var key in 成长值list)
				{

					if (key.Value > value)
					{
						//Program.setting.LogInfo("成长值已匹配到"+key.Key);
						return bestValue;
					}
					else
					{
						bestValue = key.Key;
						//Program.setting.LogInfo("成长值不匹配("+value+"<"+ key.Value);
					}

				}
				return bestValue;
			}


			public SummomPriceRule(string name, string[] rules)
			{
				if (rules.Length < 11)
				{
					Program.setting.LogInfo(string.Format("添加召唤兽规则失败,{0}的规则错误", name), "设置选项");
					return;
				}
				var 成长值 = Convert.ToDouble(rules[2]);
				if (!成长值list.ContainsKey(成长值)) 成长值list.Add(成长值, 成长值);
				RuleId = GetSummonKindId(name, rules[1] == "神", 成长值范围(成长值), rules[3] == "是");
				if (SummomPriceRuleList.ContainsKey(RuleId))
				{
					Program.setting.LogInfo("添加召唤兽规则失败,已存在:" + RuleId, "设置选项");
				}
				else
				{
					try
					{
						RankPriceHandle = new SortedList<string, double>();
						SkillPriceHandle = new SortedList<string, double>();
						技能修炼PriceHandle = new SortedList<double, double>();
						SkillNumPriceHandle = new SortedList<double, double>();
						for (int i = 0; i < rules.Length; i++) rules[i] = rules[i].Trim('　');
						var tmpRules = rules[4].Split(',');
						foreach (var tmpRule in tmpRules)
						{
							var tmpRuleInfo = tmpRule.Split(':');
							RankPriceHandle.Add(tmpRuleInfo[0], Convert.ToDouble(tmpRuleInfo[1]));
						}

						tmpRules = rules[7].Split(',');
						foreach (var tmpRule in tmpRules)
						{
							var tmpRuleInfo = tmpRule.Split(':');
							技能修炼PriceHandle.Add(Convert.ToDouble(tmpRuleInfo[0] == "其他" ? "0" : tmpRuleInfo[0]), Convert.ToDouble(tmpRuleInfo[1]));
						}

						tmpRules = rules[9].Split(',');
						foreach (var tmpRule in tmpRules)
						{
							var tmpRuleInfo = tmpRule.Split(':');
							SkillNumPriceHandle.Add(Convert.ToDouble(tmpRuleInfo[0]), Convert.ToDouble(tmpRuleInfo[1]));
						}
						SkillPriceHandle = Summon.GetSkillList(rules[5]);
						BasePrice = Convert.ToDouble(rules[10]);
						SpecialSkillPrice = Convert.ToDouble(rules[6]);
						//觉醒技能Price = Convert.ToDouble(rules[11]); 未用上
						var 变色info = rules[8].Split(',');
						饰品变色price = Convert.ToDouble(变色info[0]);
						饰品未变色price = Convert.ToDouble(变色info[1]);
						SummomPriceRuleList.Add(RuleId, this);
						//Program.setting.LogInfo("添加:" + RuleId);
					}
					catch (Exception)
					{

						throw;
					}

				}
			}

			/// <summary>
			/// 依据种类锁定定价规则
			/// </summary>
			/// <param name="summon"></param>
			/// <returns></returns>
			public static SummomPriceRule GetSummonKindId(Summon summon)
			{
				var priceRule = GetSummonKindPriceList(summon);
				if (priceRule == null)
					Program.setting.LogInfo("类型库中不存在" + summon.ID + "当前库共有" + SummomPriceRuleList.Count + "种", summon.server.ServerName);
				return priceRule;
			}
			public static SummomPriceRule GetSummonKindPriceList(Summon summon)
			{
				SummomPriceRule best = null;
				foreach (var 成长值 in 成长值list)
				{

					if (成长值.Key <= summon.成长率)
					{
						var thisId = GetSummonKindId(summon.Name, summon.神兽, 成长值.Key, summon.变色);
						if (SummomPriceRuleList.ContainsKey(thisId)) best = SummomPriceRuleList[thisId];
					}
				}
				return best;
			}
			public static string GetSummonKindId(string name, bool 神兽, double 成长值, bool 变色)
			{
				var cstr = new StringBuilder();
				cstr.Append(name).Append("$").Append(神兽 ? "神" : "普").Append("$").Append(成长值).Append("$").Append(变色 ? "变色" : "不变色");
				return cstr.ToString();
			}

			public SortedList<string, double> RankPriceHandle;//3转:50 点化:100 其他:0
			public SortedList<string, double> SkillPriceHandle;//技能列表取最大
			public SortedList<double, double> SkillNumPriceHandle;//3:100 4:200
			public double SpecialSkillPrice;
			public double BasePrice;
			public double 饰品变色price, 饰品未变色price;
			public SortedList<double, double> 技能修炼PriceHandle;//999:100 1999:200

			internal double GetPrice(Summon summon)
			{
				double RankPrice = -1, SkillPrice = 0, 技能修炼price = 0, SkillNumPrice = 0, 饰品price = 0;
				foreach (var rankName in RankPriceHandle)
				{
					if (summon.Rank.Contains(rankName.Key))
					{
						RankPrice = rankName.Value;
						break;
					}
				}
				if (RankPrice == -1) RankPrice = RankPriceHandle["其他"];
				var baseSkill = "";
				foreach (var skill in summon.Skills)
				{
					if (SkillPriceHandle.ContainsKey(skill.Name))
					{
						var thisPrice = SkillPriceHandle[skill.Name];
						if (thisPrice > SkillPrice)
						{
							SkillPrice = thisPrice;
							baseSkill = skill.Name;
						}
					}
				}
				var SkillNum = summon.Skills.Count;
				foreach (var skillNumInfo in SkillNumPriceHandle)
				{
					if (SkillNum < skillNumInfo.Key)
					{
						break;
					}
					else
					{
						SkillNumPrice = Math.Max(SkillNumPrice, skillNumInfo.Value);
					}

				}
				var 饰品品质 = Convert.ToDouble(summon.饰品品质);
				饰品price = 饰品品质 >= 1000 ? 饰品变色price : 饰品品质 >= 250 ? 饰品未变色price : 0;
				var 技能修炼 = Convert.ToDouble(summon.修炼参数2);
				foreach (var 修炼 in 技能修炼PriceHandle)
				{
					if (技能修炼 < 修炼.Key)
					{
						break;
					}
					else
					{
						技能修炼price = Math.Max(技能修炼price, 修炼.Value);
					}

				}
				//Program.setting.LogInfo(string.Format("召唤兽估价max((等级{7}({0})+技能{8}({1})+技能数量{9}({2})+基础{10}({3})+修炼{11}({4})),(觉醒技能{12}({5})+饰品{13}({6})))", RankPrice, SkillPrice, SkillNumPrice, BasePrice, 技能修炼price, summon.HaveSpecialSkill ? SpecialSkillPrice : 0, 饰品price, summon.Rank, baseSkill + "." + summon.GetAllSkill(), SkillNum, summon.ID, summon.修炼 ? summon.修炼参数2 : "无修炼", summon.HaveSpecialSkill ? "已觉醒" : "未觉醒", 饰品品质), summon.server.ServerName);
				return Math.Max(RankPrice, Math.Max(SkillPrice, Math.Max(SkillNumPrice, Math.Max(BasePrice, 技能修炼price))) + (summon.HaveSpecialSkill ? SpecialSkillPrice : 0) + 饰品price);
			}

			internal static void Init()
			{
				SummomPriceRuleList = new Dictionary<string, SummomPriceRule>();
				成长值list = new SortedList<double, double>();
				var info = File.ReadAllLines(@"setting\默认价值.txt", Encoding.Default);
				var nowLine = 0;
				Goods.StaticInit();
				for (int i = 0; i < info.Length; i++)
				{
					if (info[i].StartsWith("//")) continue;
					var ruleInfo = info[i];
					if (info[i].IndexOf("//") > 0)
						ruleInfo = ruleInfo.Substring(0, info[i].IndexOf("//"));

					if (ruleInfo.Length > 0)
					{
						nowLine++;
						switch (nowLine)
						{
							case 1:
								{
									Goods.SetTalentRule(ruleInfo);
									Program.setting.LogInfo("SetTalentRule：" + ruleInfo, "设置选项");
									break;
								}
							case 2:
								{
									Goods.SetAchievementRule(ruleInfo);
									Program.setting.LogInfo("SetAchievementRule：" + ruleInfo, "设置选项");
									break;
								}
							case 3:
								{
									Goods.SetRankRule(ruleInfo);
									Program.setting.LogInfo("SetRankRule：" + ruleInfo, "设置选项");
									break;
								}
							default:
								{
									ruleInfo = info[i].Replace('，', ',').Split(new string[] { "//" }, StringSplitOptions.None)[0];
									Program.setting.LogInfo("SummonRule：" + ruleInfo, "设置选项");
									var rules = ruleInfo.Split('\\');
									var names = rules[0].Split(',');
									foreach (var name in names)
									{
										new SummomPriceRule(name, rules);

									}
									break;
								}
						}

					}
				}
				return;//用于激活static
			}
		}
	}
}
