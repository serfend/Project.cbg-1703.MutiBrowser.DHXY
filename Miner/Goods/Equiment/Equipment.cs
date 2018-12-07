using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilReg;
using Miner.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miner.Goods.Equiment
{
	public class Equipment
	{
		public override string ToString()
		{
			return string.Format("{0}:{1}",name,desByWeb);
		}
		private int maxPrivity, nowPrivity;
		private int itype;
		private string name;
		private string desByType;
		private string desByWeb;
		private string desInYellow;
		private string previousNameDescription="";//当存在【原物品】时有效
		private bool init = false;
		private Server.Server server;
		private static Reg TypeConvertor=new Reg("sfMinerDigger").In("Main").In("Setting").In("ServerData");
		/// <summary>
		/// 通过iType判断匹配装备的名称和描述
		/// 其中，描述中将会包含装备的 阶数（如果是仙器）
		/// 符石按描述中的等级（符石[{等级}]）
		/// 普通装备按 等级 {rank}# 判断等级
		/// 星卡按普通装备名进行匹配，不判断等级
		/// </summary>
		/// <param name="rawInfo"></param>
		/// <param name="server"></param>
		public Equipment(Node rawInfo,Server.Server server)
		{
			if (rawInfo.child.Count < 3) return;
			this.Server = server;
			desByWeb= rawInfo["cDesc"].Data;
			var rawPrivityInfo = HttpUtil.GetElement(desByWeb, "默契度 ", "#");
			if (rawPrivityInfo != null)
			{
				var tmpPrivityInfo = rawPrivityInfo.Split('/');
				if (tmpPrivityInfo.Length < 2)
				{
					Program.setting.LogInfo("装备加载失败,在 默契度 处:" + desByWeb, server.ServerName);
				}
				else
				{
					NowPrivity = Convert.ToInt32(tmpPrivityInfo[0]);
					MaxPrivity = Convert.ToInt32(tmpPrivityInfo[1]);
					
				}
			}
			Init = true;
			Itype = Convert.ToInt32(rawInfo["iType"]?.Data);
			var nameDic = TypeConvertor.In("equip_name_dict");
			var desDic = TypeConvertor.In("equip_desc_dict");
			Name = nameDic.GetInfo(Itype.ToString()).Replace("'","");
			DesByType = desDic.GetInfo(Itype.ToString());

			var getAllYellowDes = new StringBuilder();
			var tmpInfo = HttpUtil.GetAllElements(DesByWeb + "#c", "#cFEFF72","#c");
			foreach(var df in tmpInfo)
			{
				if(df.Length>0)
				{
					getAllYellowDes.Append(df).Append("#r");
					if (df.Contains("原物品"))
					{
						var infos = df.Split(' ');
						Name = infos[2].Replace("#r","");
						PreviousNameDescription = infos[1];
						//System.Windows.Forms.MessageBox.Show((string.Format("Name:{0},Des:{1}",Name,PreviousNameDescription)));
					}
				}
			}
			DesInYellow = getAllYellowDes.ToString();
		}
		public int Price {
			get {
				return EquimentPrice.GetPrice(this);
			}
		}
		/// <summary>
		/// 以默契度判断的等级
		/// </summary>
		public int Rank
		{
			get => MaxPrivity / 2000 + 1;
		}
		public int MaxPrivity { get => maxPrivity; set => maxPrivity = value; }
		public int NowPrivity { get => nowPrivity; set => nowPrivity = value; }
		public bool Init { get => init; set => init = value; }
		public Server.Server Server { get => server; set => server = value; }
		public int Itype { get => itype; set => itype = value; }
		/// <summary>
		/// 以itype，取本地【equip_name_dict】库
		/// </summary>
		public string Name { get => name; set => name = value; }
		/// <summary>
		/// 以商品itype，取本地【equip_desc_dict】库
		/// </summary>
		public string DesByType { get => desByType; set => desByType = value; }
		public string DesByWeb { get => desByWeb; set => desByWeb = value; }
		public string DesInYellow { get => desInYellow; set => desInYellow = value; }
		/// <summary>
		/// 取“原物品”中的名称
		/// </summary>
		public string PreviousNameDescription { get => previousNameDescription; set => previousNameDescription = value; }
	}
}
