using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Miner
{
namespace Goods.Summon
	{
		public class Summon
		{


			private bool valid;

			private string name;
			private string rank;
			private int rankNum;
			private double _成长率;
			private bool _变色;
			private bool _神兽;
			private string cDesc;
			private List<Skill> skills;
			public bool 修炼;
			public string 修炼参数1, 修炼参数2;

			private bool haveSpecialSkill;
			private string _饰品品质;


			public static SortedDictionary<string, SortedList<string, double>> SkillPriceList;
			public static Dictionary<string, int> InvalidRankRule;
			public static Dictionary<string, int> InvalidNameRule;
			public static Dictionary<string, int> InvalidcDesRule;
			static Summon()
			{
				SkillPriceList = new SortedDictionary<string, SortedList<string, double>>();
				InvalidRankRule = new Dictionary<string, int>();
				InvalidNameRule = new Dictionary<string, int>();
				InvalidcDesRule = new Dictionary<string, int>();
				var InvalidRuleInfo = File.ReadAllLines("setting\\屏蔽规则.txt", Encoding.Default);
				var rankRules = InvalidRuleInfo[0].Split(':')[1].Split(' ');
				var nameRules = InvalidRuleInfo[1].Split(':')[1].Split(' ');
				var cDesRules = InvalidRuleInfo[2].Split(':')[1].Split(' ');
				foreach (var rankRule in rankRules) InvalidRankRule[rankRule] = 1;
				foreach (var nameRule in nameRules) InvalidNameRule[nameRule] = 1;
				foreach (var cDesRule in cDesRules) InvalidcDesRule[cDesRule] = 1;

			}
			public static SortedList<string, double> GetSkillList(string listName)
			{
				if (SkillPriceList.ContainsKey(listName)) return SkillPriceList[listName];
				var skillInfo = File.ReadAllLines("setting\\" + listName);
				var skillList = new SortedList<string, double>();
				foreach (var line in skillInfo)
				{
					var skillPrice = line.Split(' ');
					if (skillPrice.Length == 2)
					{
						skillList[skillPrice[0]] = Convert.ToDouble(skillPrice[1]);
					}
					else
					{
						Program.setting.LogInfo("添加召唤兽技能失败.格式错误:" + line,"设置选项");
					}
				}
				SkillPriceList[listName] = skillList;
				return skillList;
			}
			public Server.Server server;
			public Summon(Node Summon,Server.Server server)
			{
				if (Summon.child.Count < 3) return;
				this.server = server;
				var raw_data = Summon["raw_data"];
				var baseInfo = raw_data["base"].Data.Replace(' ', '#');
				Rank = HttpUtil.GetElement(baseInfo, "等级#", "#");

				成长率 = Convert.ToDouble(HttpUtil.GetElement(baseInfo, "成长率#", "#"));
				Name = raw_data["name"].Data;
				CDesc = Summon["cDesc"].Data;
				变色 = CDesc.Contains("变色");
				var skillInfo = raw_data["skill"].Data.Replace("；", "#r");
				HaveSpecialSkill = !skillInfo.Contains("未觉醒");
				var skillRaw = HttpUtil.GetAllElements(skillInfo, "技能", "#r");
				if (skillInfo.Contains("技能修炼"))
				{
					修炼 = true;
					var tmp = HttpUtil.GetElement(skillInfo, "修炼(", ")");
					修炼参数1 = HttpUtil.GetElementLeft(tmp, "/");
					修炼参数2 = tmp.Substring(修炼参数1.Length + 1);
				}
				if (skillInfo.Contains("神兽技能格")) 神兽 = true;
				Skills = new List<Skill>();
				foreach (var skill in skillRaw)
				{
					var thisSkill = new Skill(skill);
					if (thisSkill.Valid) Skills.Add(thisSkill);
				}
				if (CDesc.Contains("饰品"))
					饰品品质 = HttpUtil.GetElement(CDesc, "品质：", "#");
				else
					饰品品质 = "-1";
				foreach (var rankRule in InvalidRankRule)
				{
					if (Rank.Contains(rankRule.Key)) return;
				}
				if (InvalidNameRule.ContainsKey(Name)) return;
				foreach (var cDesRule in InvalidcDesRule)
				{
					if (CDesc.Contains(cDesRule.Key)) return;
				}
				valid = true;
			}

			public bool Valid { get => valid; set => valid = value; }
			public bool HaveSpecialSkill { get => haveSpecialSkill; set => haveSpecialSkill = value; }
			public string Name { get => name; set => name = value; }
			public string Rank { get => rank; set => rank = value; }
			public double 成长率 { get => _成长率; set => _成长率 = value; }
			public bool 变色 { get => _变色; set => _变色 = value; }
			public string CDesc { get => cDesc; set => cDesc = value; }
			public int RankNum { get => rankNum; set => rankNum = value; }
			internal List<Skill> Skills { get => skills; set => skills = value; }

			private string id;
			public string ID { set => id = value; get => SummomPriceRule.GetSummonKindId(Name, 神兽, 成长率, 变色); }
			private SummomPriceRule priceHdl;
			private double price = -1;
			public double Price
			{
				get
				{
					if (price > -1) return price;
					if (priceHdl == null)
					{
						priceHdl = SummomPriceRule.GetSummonKindId(this);

						return priceHdl == null ? 0 : priceHdl.GetPrice(this);
					}
					price = priceHdl.GetPrice(this);
					return price;
				}
			}

			public string 饰品品质 { get => _饰品品质; set => _饰品品质 = value; }
			public bool 神兽 { get => _神兽; set => _神兽 = value; }

			internal class Skill
			{
				private bool valid;
				private string name;
				public Skill(string info)
				{
					valid = !info.Contains("未开启");
					name = HttpUtil.GetElementRight(info, ":");

				}

				public bool Valid { get => valid; set => valid = value; }
				public string Name { get => name; set => name = value; }
			}

			internal string GetAllSkill()
			{
				var cstr = new StringBuilder();
				foreach (var skill in skills)
				{
					cstr.Append(skill.Name).Append(",");
				}
				return cstr.ToString(0, cstr.Length - 1);
			}
		}
	}
}
