using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Miner.Goods.Equiment
{
	/// <summary>
	///【独立】 以装备的默契作为估价
	/// </summary>
	public class EquimentPrivityPrice
	{
		private SortedDictionary<int, int> privityPriceRule;
		public EquimentPrivityPrice(string priceRuleInfo)
		{
			privityPriceRule = new SortedDictionary<int, int>();
			var rules = priceRuleInfo.Split(' ');
			foreach (var rule in rules)
			{
				var rulePair = rule.Split(':');
				privityPriceRule.Add(Convert.ToInt32(rulePair[0]), Convert.ToInt32(rulePair[1]));
			}
		}
		public int GetPrice(int value)
		{
			int nowPriceJudge = 0;
			foreach (var rule in privityPriceRule)
			{
				if (rule.Key >= value) break;
				nowPriceJudge = rule.Value;
			}
			return nowPriceJudge;
		}
	}
	/// <summary>
	/// 【独立】（常规、星卡）以装备的名称和包含属性进行估价 
	/// </summary>
	public class EquimentTypePrice
	{
		public static SortedDictionary<string, List<EquimentTypePrice>> priceRuleByType = new SortedDictionary<string, List<EquimentTypePrice>>();
		private SortedDictionary<int, int> priceRule;//以等级核算
		private List<string> containsProperty;//是否包含所有属性
		private string equipName;
		private int basePrice;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="rule">属性/属性/属性,等级:价格,等级:价格</param>


		public EquimentTypePrice(int basePrice, string[] rawRule)
		{

			priceRule = new SortedDictionary<int, int>();
			containsProperty = new List<string>();
			this.basePrice = basePrice;
			if (rawRule.Length > 1)
			{
				var properties = rawRule[1].Split('/');
				foreach (var p in properties) containsProperty.Add(p);
				for(int i = 2; i < rawRule.Length; i++)
				{
					var rankRule = rawRule[i].Split(':');
					priceRule.Add(Convert.ToInt32(rankRule[0]), Convert.ToInt32(rankRule[1]));
				}
			}
			
		}

		public int BasePrice { get => basePrice; set => basePrice = value; }
		public string EquipName { get => equipName; set => equipName = value; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="des"></param>
		/// <param name="rank">此处仅有部分装备有（非装备本身的等级）</param>
		/// <param name="success"></param>
		/// <returns></returns>
		public int GetPrice(Equipment e,out string success)
		{
			var containsProperties = new StringBuilder();
			foreach(var description in containsProperty)
			{
				if (/*!e.DesByType.Contains(description)&&*/!(e.DesInYellow.Contains(description)))
				{
					success = null;
					return 0;
				}
				containsProperties.Append(description).Append(",");
			}
			int result = basePrice;
			if (priceRule != null)
			{
				var eRank = Convert.ToInt32(HttpUtil.GetElement(e.DesByWeb,"等级 ","#"));
				if (eRank == 0)
				{
					var len = e.PreviousNameDescription.IndexOf("级");
					if (len > 0)
					{
						eRank = Convert.ToInt32(e.PreviousNameDescription.Substring(0, len));
					}
				}
					
				foreach (var r in priceRule)
				{
					if (eRank >= r.Key)
					{
						if (result < r.Value) result = r.Value;
					}
				}
				success = string.Format("装备 {0}：{3}，等级{1},价值{2} '{4}", e.Name, eRank,result, containsProperties.ToString(), e.DesInYellow);
			}
			else
			{
				success = string.Format("基础装备 {0}：{2}，价值{1}", e.Name,  result, containsProperties.ToString());
			}
			
			return result;
		}
	}
	/// <summary>
	/// 【独立】（仙器）以装备的描述是否包含特定的信息，以及描述中的等级进行估价
	/// </summary>
	public static class EquimentDescriptionPrice
	{
		public static SortedDictionary<int, int> priceRule=new SortedDictionary<int, int>();
		
		/// <summary>
		/// 从描述中匹配 【阶数】{rank}
		/// </summary>
		/// <param name="equipmentDescription"></param>
		/// <returns></returns>
		public static int GetPrice(string equipmentDescription,out string des)
		{

			var tmpInfo = HttpUtil.GetElement(equipmentDescription, "【阶数】", "#");
			if (tmpInfo == null)
			{
				des = "无阶数";
				return 0;
			}
			tmpInfo = tmpInfo.Replace(":", "");
			try
			{
				var rank = Convert.ToInt32(tmpInfo);
				int result = 0;
				foreach (var r in priceRule)
				{
					if (rank >= r.Key && result < r.Value) result = r.Value;
				}
				des = string.Format("{0}阶仙器价值{1}", rank, result);
				if (rank == 0) des += equipmentDescription;
				return result;
			}
			catch (Exception ex)
			{
				des = ex.Message;
				return 0;
			}
			
		}
		public static void Init(string rule)
		{
			var rules = rule.Split(',');
			foreach(var r in rules)
			{
				var p = r.Split(':');
				priceRule.Add(Convert.ToInt32(p[0]), Convert.ToInt32(p[1]));
			}
		}
	}

	public static class EquimentDescriptionOnMagicStoneRankPrice
	{
		public static SortedDictionary<int, int> priceRule = new SortedDictionary<int, int>();

		/// <summary>
		/// 从描述中匹配所有符石的等级
		/// </summary>
		/// <param name="equipmentDescription"></param>
		/// <returns></returns>
		public static int GetPrice(string equipmentDescription, out string des)
		{
			int result = 0;
			var cstr = new StringBuilder();
			cstr.Append("符石价值");
			var tmpInfo = HttpUtil.GetAllElements(equipmentDescription, "符石[", "级]");
			foreach(var item in tmpInfo)
			{
				var thisItemRank = Convert.ToInt32(item);
				cstr.Append(thisItemRank).Append(":");
				if (priceRule.ContainsKey(thisItemRank)){
					cstr.Append(priceRule[thisItemRank]).Append(" ");
					result += priceRule[thisItemRank];
				}
				else
				{
					cstr.Append("无 ");
				}
			}
			cstr.Append("总价:").Append(result);
			des = cstr.ToString() ;
			return result;
		}
		public static void Init(string rule)
		{
			var rules = rule.Split(',');
			foreach (var r in rules)
			{
				var p = r.Split(':');
				priceRule.Add(Convert.ToInt32(p[0]), Convert.ToInt32(p[1]));
			}
		}
	}
	/// <summary>
	/// 装备估价模块
	/// </summary>
	public class EquimentPrice
	{
		public static SortedDictionary<int, EquimentPrivityPrice> EquimentPrivityPrices;
		static EquimentPrice()
		{
			EquimentPrivityPrices = new SortedDictionary<int, EquimentPrivityPrice>();
			var rawInfos=File.ReadAllLines("setting\\装备.默契价格.txt");
			foreach(var rawInfo in rawInfos)
			{
				if (rawInfo.StartsWith("//")) continue;
				var rawPair = rawInfo.Split(',');
				EquimentPrivityPrices.Add(Convert.ToInt32(rawPair[0]),new EquimentPrivityPrice(rawPair[1]));
			}
			EquimentDescriptionPrice.Init(File.ReadAllText("setting\\装备.仙器价格.txt",Encoding.Default));
			rawInfos = File.ReadAllLines("setting\\装备.名称属性价格.txt",Encoding.Default);
			foreach(var line in rawInfos)
			{
				if (line.StartsWith("//")) continue;
				var rawRule = line.Split(',');
				var targetBase = rawRule[0].Split(':');
				if (!EquimentTypePrice.priceRuleByType.ContainsKey(targetBase[0]))
				{
					EquimentTypePrice.priceRuleByType.Add(targetBase[0], new List<EquimentTypePrice>());
				}
				var list = EquimentTypePrice.priceRuleByType[targetBase[0]];
				var item = new EquimentTypePrice(Convert.ToInt32(targetBase[1]),rawRule);
				list.Add(item);
			}
			var rawInfoer = File.ReadAllText("setting\\装备.符石价格.txt", Encoding.Default);
			EquimentDescriptionOnMagicStoneRankPrice.Init(rawInfoer);
		}

		internal static void Init()
		{
			return;//用于激活
		}
		/// <summary>
		/// 通过指定等级的装备条件下，以装备默契度计算价格
		/// </summary>
		/// <param name="value">默契</param>
		/// <returns></returns>
		public static int GetPrice(Equipment e)
		{
			//Program.setting.LogInfo($"装备=>{e.DesByWeb}", e.Server.ServerName);
			if (e.DesByWeb.Contains("禁止交易")) {
				//Program.setting.LogInfo("不可交易的装备", e.Server.ServerName);
				return 0;
			} ;
			int result = 0;
			if (EquimentPrice.EquimentPrivityPrices.ContainsKey(e.Rank))
			{
				result += EquimentPrice.EquimentPrivityPrices[e.Rank].GetPrice(e.NowPrivity);
				//if (result > 0) Program.setting.LogInfo($"{e.Rank}级默契:{e.NowPrivity}/{e.MaxPrivity },价值:{result}", e.Server.ServerName);
			}
			else
			{
				//Program.setting.LogInfo("不存在的等级规则:" + e.Rank, e.Server.ServerName);
			}
			var immotalsPrice = EquimentDescriptionPrice.GetPrice($"{e.DesByType}#r{e.PreviousNameDescription}",out string descriptionPrice);
			///*if(immotalsPrice>0)*/Program.setting.LogInfo(descriptionPrice, e.Server.ServerName, true);//仙器估价
			result += immotalsPrice;
			//星符、普通属性估价
			if (EquimentTypePrice.priceRuleByType.ContainsKey(e.Name))
			{
				var list = EquimentTypePrice.priceRuleByType[e.Name];
				bool anyOutput = false;
				foreach(var l in list)
				{
					var typePrice = l.GetPrice(e,out string successInfo);
					if (successInfo!=null)
					{
						Program.setting.LogInfo(successInfo, e.Server.ServerName, true);//普通装备
						result += typePrice;
						anyOutput = true;
						break;
					}
				}
				if (!anyOutput)
				{
					//Program.setting.LogInfo(string.Format("不存在的规则：{0}  '{1}", e.Name,e.DesInYellow), e.Server.ServerName, true);
				}
			}
			else
			{
				//Program.setting.LogInfo(string.Format("不存在的名称{0}", e.Name), e.Server.ServerName, true);
			}

			//符石估价

			var magicStonePrice = EquimentDescriptionOnMagicStoneRankPrice.GetPrice(e.DesByWeb, out string stoneDes);
			///*if(magicStonePrice>0) */Program.setting.LogInfo(stoneDes, e.Server.ServerName, true);
			result += magicStonePrice;
			return result;
		}
	}
}
