using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miner.ServerHandle
{
	public class Order_headersItem
	{
		/// <summary>
		/// 
		/// </summary>
		public string field { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<string> direction { get; set; }
		/// <summary>
		/// 最近寄售
		/// </summary>
		public string name { get; set; }
	}

	public class Equip_listItem
	{
		/// <summary>
		/// 小孩:1 最高评价:3995
		/// </summary>
		public string subtitle { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int raw_equip_status { get; set; }
		/// <summary>
		/// 天若有情
		/// </summary>
		public string equip_server_name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int storage_type { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int serverid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string appointed_roleid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int areaid { get; set; }
		/// <summary>
		/// 天若有情
		/// </summary>
		public string server_name { get; set; }
		/// <summary>
		/// 飞升158级
		/// </summary>
		public string level_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string highlights { get; set; }
		/// <summary>
		/// 人界 
		/// </summary>
		public string equip_area_name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int equip_status { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int equip_serverid { get; set; }
		/// <summary>
		/// 人界 
		/// </summary>
		public string area_name { get; set; }
		/// <summary>
		/// 小孩:1 最高评价:3995
		/// </summary>
		public string desc_sumup_short { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int status { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string product { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int price { get; set; }
		/// <summary>
		/// 上架中
		/// </summary>
		public string status_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string price_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int pass_fair_show { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string icon { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string unit_price_desc { get; set; }
		/// <summary>
		/// 小孩:1 最高评价:3995
		/// </summary>
		public string desc_sumup { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int equip_areaid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string game_ordersn { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string equip_face_img { get; set; }
		/// <summary>
		/// 逆天魔
		/// </summary>
		public string equip_name { get; set; }
		/// <summary>
		/// 逆天魔
		/// </summary>
		public string format_equip_name { get; set; }
	}
	public class GoodList
	{
		public string msg { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int status { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string order_field { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<int> kind_id { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<Order_headersItem> order_headers { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string search_type { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string order_direction { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int num_per_page { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<Equip_listItem> equip_list { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string can_cross_buy { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int page { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string is_login { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string is_last_page { get; set; }
	}


	public class Equip
	{
		/// <summary>
		/// 
		/// </summary>
		public string has_collect { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string min_price { get; set; }
		/// <summary>
		/// 至尊·牛
		/// </summary>
		public string owner_nickname { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int fair_show_remain_seconds { get; set; }
		/// <summary>
		/// 7天23时
		/// </summary>
		public string expire_time { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string fair_show_poundage_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<string> collectors { get; set; }
		/// <summary>
		/// 465.0元
		/// </summary>
		public string last_price_desc { get; set; }
		/// <summary>
		/// (["iImm_All":0,"mpHalo":(["count":5,]),"mpFabao":([6:(["iGrade":61,"cDesc":"#R品质#s:#s珍藏#r#R等级#s:#s61#r#G契合度#s#s#s#s#s#s#s#s1441#G活跃度#s#s#s#s300(300)#r#G援助#s#s#s#s#s#s10.0(0.0)#G伤害#s#s#s#s14.7(14.7)#r#Y抗混乱#s5.7#r#cACDBEE符石#s:#s#r#Y#s#s#s#s#o100039空#r#s#s#s#s#o100039空#r#s#s#s#s#o100039空#r#s#s#s#s#o100040未开启#r#s#s#s#s#o100040未开启#r#cACDBEE天赋技能#s:#s#r#Y#s#s#s#s低级灵性增强#s#r#cACDBEE擅长技能#s:#s#r#Y#s#s#s#s清音寂灵#s#r#cACDBEE技能#s:#s#r#Y#s#s#s#s#o100003(刹那光电)#s1人合技#r#s#s#s#s#o100004(疾风骤雨)#s4人合技#r#s#s#s#s#o100003(刹那光电)#s2人合技#r#s#s#s#s#o100042未开启#r#s#s#s#s#o100042未开启#r","iHasSuperLock":0,"iType":26030,]),4:(["iGrade":1,"cDesc":"#R品质#s:#s把玩#r#R等级#s:#s1#r#r#Y抗鬼火#s2.2#r","iHasSuperLock":0,"iType":26033,]),2:(["iGrade":44,"cDesc":"#R品质#s:#s把玩#r#R等级#s:#s44#r#r#Y抗混乱#s5.2#r","iHasSuperLock":0,"iType":26036,]),1:(["iGrade":70,"cDesc":"#R品质#s:#s把玩#r#R等级#s:#s70#r#r#Y抗混乱#s6.0#r","iHasSuperLock":0,"iType":26041,]),0:(["iGrade":68,"cDesc":"#R品质#s:#s把玩#r#R等级#s:#s68#r#r#Y抗混乱#s5.9#r","iHasSuperLock":0,"iType":26040,]),]),"mpEngravingSuit":([]),"iInitBabyCount":3,"iDigong":40,"iProf":5,"iRich":409000,"BabyList":({(["iQinmi":7,"cName":"小孩1","mpEquip":([]),"cEnd":"","iNaili":0,"cAge":"7岁3个月","iOpinion":3233,"iWanxing":0,"iWenbao":363,"iXiaoxin":18,"iPanni":493,"iDaode":36,"iMingqi":498,"iNeili":0,"iZhili":24,"iQizhi":0,"QianghuaList":([3:(["desc":"未开启","id":"未开启","level":0,]),2:(["desc":"未开启","id":"未开启","level":0,]),1:(["desc":"未开启","id":"未开启","level":0,]),]),"TianziList":([15:"使用震慑法术时有3.39%几率增加一个攻击单位",5:"每回合玩家行动前有5.09%几率对自身释放单体的加速法术，该法术效果为增加200SP，持续时间为1回合",24:"物理攻击时有10.18%几率达到100%命中效果",]),"iMoney":100,"iPolishTaskState":0,"iSex":2,"iPilao":283,]),(["iQinmi":2,"cName":"小孩2","mpEquip":([]),"cEnd":"","iNaili":0,"cAge":"6岁0个月","iOpinion":1568,"iWanxing":0,"iWenbao":0,"iXiaoxin":1,"iPanni":0,"iDaode":36,"iMingqi":0,"iNeili":0,"iZhili":24,"iQizhi":0,"QianghuaList":([3:(["desc":"未开启","id":"未开启","level":0,]),2:(["desc":"未开启","id":"未开启","level":0,]),1:(["desc":"未开启","id":"未开启","level":0,]),]),"TianziList":([18:"受到火系法术攻击时有30%几率抵消2839点的伤害",34:"每回合开始时有1.10%几率摆脱混乱状态",32:"使用多体昏睡法术时有1.89%几率增加一个攻击单位",]),"iMoney":2160,"iPolishTaskState":0,"iSex":2,"iPilao":0,]),(["iQinmi":0,"cName":"小孩3","mpEquip":([]),"cEnd":"","iNaili":0,"cAge":"0岁0个月","iOpinion":0,"iWanxing":0,"iWenbao":0,"iXiaoxin":0,"iPanni":0,"iDaode":0,"iMingqi":0,"iNeili":0,"iZhili":0,"iQizhi":0,"QianghuaList":([3:(["desc":"未开启","id":"未开启","level":0,]),2:(["desc":"未开启","id":"未开启","level":0,]),1:(["desc":"未开启","id":"未开启","level":0,]),]),"TianziList":([3:"增加MP",2:"增加HP",1:"增加AP",]),"iMoney":0,"iPolishTaskState":0,"iSex":2,"iPilao":0,]),}),"iCor":148,"iFlyupExpPoint":0,"iSingleEnergyRate":286,"iPayXianyu":0,"iFlyupFlag":0,"iMp":30341,"usernum":414772113,"mpResist":(["add_seal":"3.0%","water_cruel_rate":"5.0%","cruel_rate":"5.0%","re_thunder":"12.3%","re_fire":"17.3%","re_water":"12.3%","re_wind":"12.3%","def_rate":"58.3%","beat_water":"55.7%","fatal_rate":"78.1%","double_rate":"11.0%","counter_rate":"13.0%","re_faint":"49.5%","re_seal":"18.5%","re_poison":"18.5%","re_absorb":"18.4%","re_disorder":"41.3%","add_poison_rate":"15.1%","re_forget":"20.6%","re_wildfire":"11.0%","att_rate":"12.4%",]),"iCash":157909,"mpTiangang":([14808:(["count":1,"record":0,]),14792:(["count":1,"record":0,]),14802:(["count":1,"record":0,]),14795:(["count":1,"record":0,]),]),"iFlyupExtPoint":0,"arrZhuanzhiSkill":({}),"mpTrinket":(["count":7,]),"iTalent":5,"mpAllAttr":([2:(["iSpe":920,"iStr":148,"iMag":148,"iCor":148,"cName":"敏魔","iPoint":0,]),1:(["iSpe":148,"iStr":915,"iMag":148,"iCor":149,"cName":"大力","iPoint":4,]),]),"ItemList":({(["cDesc":"#cFEFF72#cFEFF72#cFEFF72等级需求 1转60级#r#cFEFF72灵性要求 90#r #cFEFF72#cFEFF72灵性 +24#r 附加气血 +1400#r 耐久 1379/1500#r#c4BF24B雷法反击 +5.1%#r 被攻击时释放乾坤借速 +4.7%#r 根骨 +3#r  ","iHasSuperLock":0,"iAmount":0,"iType":3533,]),(["cDesc":"#cFEFF72#cFEFF72原物品 11级高级装备 九转水晶#r#cFEFF72等级需求 100级或3转#r#cFEFF72#cFEFF72附加气血 +3520#r 附加魔法 +3136#r 敏捷 +20#r 抗封印 +20.2%#r 耐久 980/1100#r#c4BF24B火系狂暴几率 +9.0%#r 敏捷 +41#r #r#cF8FCF8【套装】#r#c4BF24B灵性 +1#r 物理吸收率 +1.2%#r #cE43D31制作人 ─莫天承─#r ","iHasSuperLock":0,"iAmount":0,"iType":16091,]),(["cDesc":"#cFEFF72#cFEFF72#cFEFF72等级需求 140级或3转#r#cFEFF72#cFEFF72加强加防法术 +5.8%#r 加强震慑 +5.1%#r 连击率 +17.4%#r 基础攻击 +3090#r 耐久 1379/1500#r#c00FFFF附加属性：根骨 +5#r #c4BF24B附加攻击 +488#r 强力克水 +29.0%#r 附加速度 +4#r #cF0640F#o25591 5级 加强震慑 +6.0% 禁交易#r#n#cE43D31制作人 至尊·牛#r ","iHasSuperLock":0,"iAmount":0,"iType":1035,]),(["cDesc":"#cFEFF72#cFEFF72#cFEFF72等级需求 100级或3转#r#cFEFF72#cFEFF72根骨 +16#r 附加速度 +115#r 抗火 +6.1%#r 耐久 1090/1100#r#c4BF24B力量 +3#r 逃跑几率 +8.0%#r 连击率 +18.0%#r 抗雷 +8.0%#r 抗风 +3.0%#r #cE43D31制作人 大爱无情#r ","iHasSuperLock":0,"iAmount":0,"iType":2231,]),(["cDesc":"#cFEFF72#cFEFF72#cFEFF72等级需求 140级或3转#r#cFEFF72#cFEFF72加强加防法术 +5.2%#r 加强震慑 +4.7%#r 物理吸收率 +8.7%#r 抗中毒 +14.1%#r 抗混乱 +19.2%#r 防御 +3080#r 耐久 1392/1500#r#c4BF24B强力克木 +6.0%#r 被攻击时释放乾坤借速 +2.0%#r 属性需求 -10.0%#r 抗遗忘 +7.0%#r 抗风 +7.0%#r #cE43D31制作人 大话西游2经典版#r ","iHasSuperLock":0,"iAmount":0,"iType":2139,]),(["cDesc":"#cFEFF72#cFEFF72#cFEFF72等级需求 1转100级#r功绩需求 1800#r#cFEFF72#cFEFF72尘埃落定 +3656#r 增加强克效果 +7.3%#r 品质 914/1000#r 耐久 3879/4000#r#c4BF24B强力克水 +10.9%#r #c00FFFF培养 0#n#r ","iHasSuperLock":0,"iAmount":0,"iType":3930,]),(["cDesc":"#cFEFF72#cFEFF72#cFEFF72等级需求 2转120级#r#cFEFF72敏捷要求 420#r #cFEFF72#cFEFF72根骨 +4#r 附加气血 +1200#r 抗混乱 +27.3%#r 耐久 2637/3000#r#c4BF24B附加速度 +17#r 抗中毒 +5.1%#r 抗封印 +5.1%#r  ","iHasSuperLock":0,"iAmount":0,"iType":2656,]),(["cDesc":"#cFEFF72#cFEFF72#cFEFF72等级需求 1转60级#r#cFEFF72灵性要求 10#r #cFEFF72#cFEFF72附加气血 +1400#r 附加魔法 +1400#r 抗混乱 +22.3%#r 耐久 1379/1500#r#c4BF24B附加速度 +13#r 抗混乱 +3.8%#r  ","iHasSuperLock":0,"iAmount":0,"iType":3923,]),(["cDesc":"#cFEFF72#cFEFF72#cFEFF72等级需求 2转120级#r#cFEFF72敏捷要求 420#r #cFEFF72#cFEFF72附加气血 +1200#r 敏捷 +8#r 抗遗忘 +30.0%#r 耐久 2637/3000#r#c4BF24B风法反击 +15.1%#r 被攻击时释放魔神附身 +14.1%#r  ","iHasSuperLock":0,"iAmount":0,"iType":3726,]),(["cDesc":"#cFEFF72#cFEFF72#cFEFF72等级需求 2转120级#r#cFEFF72敏捷要求 420#r #cFEFF72#cFEFF72灵性 +4#r 抗遗忘 +28.5%#r 耐久 2637/3000#r#c4BF24B附加速度 +26#r 狂暴几率 +12.1%#r  ","iHasSuperLock":0,"iAmount":0,"iType":3626,]),(["cDesc":"#cFEFF72#cFEFF72#cFEFF72等级需求 2转120级#r#cFEFF72敏捷要求 420#r #cFEFF72#cFEFF72附加气血 +1200#r 敏捷 +48#r 耐久 2637/3000#r#c4BF24B灵性 +8#r 被攻击时释放含情脉脉 +14.1%#r  ","iHasSuperLock":0,"iAmount":0,"iType":3546,]),}),"iBuyBabyCount":0,"iShitu":10042,"mpSkill":([111:25000,95:10550,94:10729,93:11675,92:25017,91:25017,85:13081,84:10369,83:16048,115:15545,82:25000,114:10798,81:25000,113:11341,112:25000,]),"iProfGrade":118,"cOrg":" 海纳百川 ","iTou":0,"iSpe":920,"iGrade":148,"iOrgXiulian":0,"mpFoot":(["count":3,]),"mpFashion":(["data":({"财主帽","猫耳朵",}),"count":2,]),"iTili":1453,"iCurAttr":2,"mpRei2":([]),"mpRei":(["re_absorb":"18.4%","iMp":"36.9%","iDog":"15.3%","def_rate":"30.8%","iHp":"36.9%",]),"mpEquip":([11:(["cDesc":"#cFEFF72#cFEFF72等级需求 2转120级#r#cFEFF72力量要求 450#r #cFEFF72#cFEFF72附加攻击 +1800#r 根骨 +8#r 抗鬼火 +33.1%#r 耐久 2937/3000#r#c4BF24B附加攻击 +1001#r 逃跑几率 +14.1%#r  ","iHasSuperLock":0,"iType":3626,]),10:(["cDesc":"#cFEFF72#cFEFF72等级需求 2转120级#r#cFEFF72灵性要求 10#r #cFEFF72#cFEFF72附加气血 +2600#r 附加魔法 +2600#r 抗昏睡 +31.0%#r 耐久 2859/3000#r#c4BF24B致命几率 +12.1%#r 附加毒攻击几率 +15.1%#r  ","iHasSuperLock":0,"iType":3926,]),9:(["cDesc":"#cFEFF72#cFEFF72等级需求 2转120级#r#cFEFF72力量要求 450#r #cFEFF72#cFEFF72附加攻击 +1600#r 根骨 +12#r 抗水 +36.0%#r 耐久 2937/3000#r#c4BF24B加成攻击 +10.1%#r 致命几率 +12.1%#r  ","iHasSuperLock":0,"iType":3726,]),8:(["cDesc":"#cFEFF72#cFEFF72等级需求 2转120级#r#cFEFF72力量要求 450#r #cFEFF72#cFEFF72力量 +48#r 附加攻击 +1600#r 耐久 2937/3000#r#c4BF24B雷法反击 +10.1%#r 化血成碧 +1601#r 上善若水 +1601#r  ","iHasSuperLock":0,"iType":3546,]),7:(["cDesc":"#cFEFF72原物品 6级佩饰 血瓷戒指#r#cFEFF72等级需求 2转120级#r#cFEFF72力量要求 450#r #cFEFF72#cFEFF72力量 +48#r 附加攻击 +1600#r 耐久 2937/3000#r#c4BF24B根骨 +8#r 上善若水 +2401#r #r#cF8FCF8【套装】#c00FFFF把玩#r#c4BF24B附水攻击 +1.3%#r 被攻击时释放乾坤借速 +1.4%#r ","suit":(["skills":({908,0,3,}),"mask":536704,]),"iHasSuperLock":0,"iType":16096,]),6:(["cDesc":"#cFEFF72#cFEFF72等级需求 2转120级#r#cFEFF72力量要求 450#r #cFEFF72#cFEFF72力量 +8#r 附加攻击 +1600#r 抗混乱 +31.0%#r 耐久 2937/3000#r#c4BF24B附封印攻击 +10.1%#r 抗三尸虫 +734#r 连击率 +10.1%#r  ","iHasSuperLock":0,"iType":2656,]),5:(["cDesc":"#cFEFF72#cFEFF72等级需求 140级或3转#r#cFEFF72力量要求 450#r #cFEFF72#cFEFF72狂暴几率 +18.8%#r 忽视防御几率 +75.6%#r 忽视防御程度 +81.9%#r 基础攻击 +13800#r 耐久 1479/1500#r#c4BF24B强力克水 +22.0%#r 反击 +9#r 加强雷 +7.0%#r #cF0640F#o25611 5级 附加攻击 +3800 禁交易#r#n#cE43D31制作人 至尊·牛#r ","iHasSuperLock":0,"iType":1035,]),4:(["cDesc":"#cFEFF72#cFEFF72等级需求 100级或3转#r#cFEFF72#cFEFF72灵性 +27#r 附加气血 +2944#r 附加魔法 +3424#r 抗遗忘 +20.6%#r 耐久 979/1100#r#c4BF24B强力克水 +33.0%#r 水系狂暴几率 +5.0%#r 敏捷 +26#r 加强封印 +3.0%#r #cE43D31制作人 〃情谊菲菲。#r ","iHasSuperLock":0,"iType":2331,]),3:(["cDesc":"#cFEFF72#cFEFF72等级需求 100级或3转#r#cFEFF72#cFEFF72灵性 +16#r 附加速度 +117#r 抗混乱 +5.9%#r 耐久 1079/1100#r#c4BF24B抗鬼火 +11.0%#r 致命几率 +40.0%#r 反击率 +13.0%#r #cF0640F#o25661 5级 强力克水 +22.7% 禁交易#r#n#cE43D31制作人 Pve、女神#r ","iHasSuperLock":0,"iType":2231,]),2:(["cDesc":"#cFEFF72#cFEFF72等级需求 100级或3转#r#cFEFF72力量要求 340#r #cFEFF72#cFEFF72物理吸收率 +9.7%#r 抗混乱 +22.0%#r 防御 +7980#r 耐久 1179/1200#r#c4BF24B每回合HP +52#r 被攻击时释放魔神附身 +12.0%#r 抗鬼火 +4.0%#r 抗灵宝伤害 +9.0%#r #cE43D31制作人 大话西游2经典版#r ","iHasSuperLock":0,"iType":2136,]),1:(["cDesc":"#cFEFF72#cFEFF72等级需求 30级#r#cFEFF72#cFEFF72物理吸收率 +18.0%#r 耐久 499/500#r#c4BF24B附加攻击 +473#r 致命几率 +21.0%#r 连击率 +11.0%#r 抗火 +5.0%#r #cE43D31制作人 琴雄儿#r ","iHasSuperLock":0,"iType":2019,]),]),"ZFList":([2:(["iChengjiu":1041,"iGrade":126,"iLevel":7,"iSkill":27623,]),]),"iSaving":11340,"iAp":6401,"iHp":39020,"iPoint":0,"iStr":148,"SummonList":({(["cDesc":"#cFFFFFF小熊   #cEF60512转136级#r#cFBFFC1成长率 1.350#r#c7DDE8C气血  67973#cF6FCF5(152) #c7DDE8C攻击 28757#cF6FCF5( 234)#r#c7DDE8C法力  24969#cF6FCF5(  0) #c7DDE8C速度   286#cF6FCF5(  76)#r#cD8AB6C内　丹 隔山打牛 2转94级#r#cD8AB6C　　　 浩然正气 2转98级#r#cD8AB6C　　　 暗渡陈仓 1转118级#r#cACDBEE炼妖次数 8#r#c8EDEFF抗物理: 75.0(#cFFFFFF30.0#c8EDEFF+45.0)#r#c8EDEFF抗混乱: 75.0(#cFFFFFF30.0#c8EDEFF+45.0)#r#r#r#r#r#r技能1:春风拂面；技能2:高级清明术#r技能3:未开启；#r#Y装备：#o110000 #o110000 #o110000 #r#Y觉醒技：#n   未觉醒#r#cA49BC8金:20 木:30 水: 0 火: 0 土:50#r亲密度:514544#r不可交易#r","iSpecial":0,"iHasSuperLock":0,"raw_data":(["lianyao":(["desc":"抗物理: 75.0(30.0+45.0)#r抗混乱: 75.0(30.0+45.0)#r#r#r#r#r#r","count":8,]),"equip":"#Y装备：#o110000 #o110000 #o110000 #r","other":"金:20 木:30 水: 0 火: 0 土:50#r亲密度:514544#r不可交易#r","neidan":"隔山打牛 2转94级#r浩然正气 2转98级#r暗渡陈仓 1转118级#r","base":"等级 2转136级#r成长率 1.350#r气血  67973(152) 攻击 28757( 234)#r法力  24969(  0) 速度   286(  76)#r","name":"小熊","skill":"技能1:春风拂面；技能2:高级清明术#r技能3:未开启；#r#Y觉醒技：#n   未觉醒#r",]),"equip":({}),"iType":102203,]),(["cDesc":"#cFFFFFF当康   #cEF60511转120级#r#cFBFFC1成长率 1.375#r#c7DDE8C气血 140879#cF6FCF5(196) #c7DDE8C攻击  4369#cF6FCF5(  17)#r#c7DDE8C法力  19800#cF6FCF5(  0) #c7DDE8C速度   251#cF6FCF5(  63)#r#cD8AB6C内　丹 万佛朝宗 1转110级#r#cD8AB6C　　　 凌波微步 1转119级#r#cD8AB6C　　　 #r#cACDBEE炼妖次数 6#r#c8EDEFF抗封印: 69.0(#cFFFFFF30.0#c8EDEFF+39.0)#r#c8EDEFF抗混乱: 69.0(#cFFFFFF30.0#c8EDEFF+39.0)#r#r#r#r#r#r天赋:一阶天赋#r技能1:帐饮东都；技能2:未开启#r技能3:未开启；#r#Y装备：#o110000 #o110000 #o110000 #r#Y觉醒技：#n   未觉醒#r#cA49BC8金:15 木: 0 水: 0 火: 0 土:85#r亲密度:32499#r","iSpecial":0,"iHasSuperLock":0,"raw_data":(["lianyao":(["desc":"抗封印: 69.0(30.0+39.0)#r抗混乱: 69.0(30.0+39.0)#r#r#r#r#r#r","count":6,]),"equip":"#Y装备：#o110000 #o110000 #o110000 #r","other":"金:15 木: 0 水: 0 火: 0 土:85#r亲密度:32499#r","neidan":"万佛朝宗 1转110级#r凌波微步 1转119级#r#r","base":"等级 1转120级#r成长率 1.375#r气血 140879(196) 攻击  4369(  17)#r法力  19800(  0) 速度   251(  63)#r","name":"当康","skill":"天赋:一阶天赋#r技能1:帐饮东都；技能2:未开启#r技能3:未开启；#r#Y觉醒技：#n   未觉醒#r",]),"equip":({}),"iType":103077,]),(["cDesc":"#cFFFFFF凤凰   #cEF60511转113级#r#cFBFFC1成长率 1.375#r#c7DDE8C气血  96399#cF6FCF5( 36) #c7DDE8C攻击  4603#cF6FCF5(  48)#r#c7DDE8C法力  37314#cF6FCF5(180) #c7DDE8C速度   237#cF6FCF5(  60)#r#cD8AB6C内　丹 祝融取火 1转97级#r#cD8AB6C　　　 霹雳流星 1转79级#r#cD8AB6C　　　 #r#cACDBEE炼妖次数 8#r#c8EDEFF抗混乱: 42.0(#cFFFFFF30.0#c8EDEFF+12.0)#r#c8EDEFF抗封印: 30.0(#cFFFFFF30.0#c8EDEFF+0.0)#r#c8EDEFF抗鬼火: 12.0   #c8EDEFF抗　雷: 34.0#r#c8EDEFF抗　水: 22.0   #c8EDEFF抗　火: 9.0#r#r#r#r技能1:灵逸；技能2:未开启#r技能3:未开启；#r#Y装备：#o110000 #o110000 #o110000 #r#Y觉醒技：#n   未觉醒#r#cA49BC8金: 0 木: 0 水: 0 火:95 土: 5#r亲密度:21908#r","iSpecial":0,"iHasSuperLock":0,"raw_data":(["lianyao":(["desc":"抗混乱: 42.0(30.0+12.0)#r抗封印: 30.0(30.0+0.0)#r抗鬼火: 12.0   抗　雷: 34.0#r抗　水: 22.0   抗　火: 9.0#r#r#r#r","count":8,]),"equip":"#Y装备：#o110000 #o110000 #o110000 #r","other":"金: 0 木: 0 水: 0 火:95 土: 5#r亲密度:21908#r","neidan":"祝融取火 1转97级#r霹雳流星 1转79级#r#r","base":"等级 1转113级#r成长率 1.375#r气血  96399( 36) 攻击  4603(  48)#r法力  37314(180) 速度   237(  60)#r","name":"凤凰","skill":"技能1:灵逸；技能2:未开启#r技能3:未开启；#r#Y觉醒技：#n   未觉醒#r",]),"equip":({}),"iType":102071,]),(["cDesc":"#cFFFFFF黄金兽   #cEF60513转153级#r#cFBFFC1成长率 1.575#r#c7DDE8C气血  58254#cF6FCF5( 11) #c7DDE8C攻击 54053#cF6FCF5( 178)#r#c7DDE8C法力  36869#cF6FCF5(  0) #c7DDE8C速度   615#cF6FCF5( 238)#r#cD8AB6C内　丹 暗渡陈仓 3转152级#r#cD8AB6C　　　 浩然正气 3转152级#r#cD8AB6C　　　 隔山打牛 3转129级#r#cACDBEE炼妖次数 8#r#c8EDEFF抗物理: 69.0(#cFFFFFF30.0#c8EDEFF+39.0)#r#c8EDEFF抗混乱: 75.0(#cFFFFFF30.0#c8EDEFF+45.0)#r#r#r#r#r#r技能1:神工鬼力；技能2:分裂攻击#r技能3:仙风道骨；#r#Y装备：#o110000 #o25420 #o25426 #r#Y觉醒技：#n   未觉醒#r#cA49BC8金:100 木: 0 水: 0 火: 0 土: 0#r亲密度:32892#r","iSpecial":0,"iHasSuperLock":0,"raw_data":(["lianyao":(["desc":"抗物理: 69.0(30.0+39.0)#r抗混乱: 75.0(30.0+45.0)#r#r#r#r#r#r","count":8,]),"equip":"#Y装备：#o110000 #o25420 #o25426 #r","other":"金:100 木: 0 水: 0 火: 0 土: 0#r亲密度:32892#r","neidan":"暗渡陈仓 3转152级#r浩然正气 3转152级#r隔山打牛 3转129级#r","base":"等级 3转153级#r成长率 1.575#r气血  58254( 11) 攻击 54053( 178)#r法力  36869(  0) 速度   615( 238)#r","name":"黄金兽","skill":"技能1:神工鬼力；技能2:分裂攻击#r技能3:仙风道骨；#r#Y觉醒技：#n   未觉醒#r",]),"equip":({"","#W【等级】1级#r【装备部位】兽铃#r#n#Y【等级需求】0转60级#r根骨 +5#r 【品质】95/100#r【通灵】1/1000#r【耐久】488/500#r#n#G被攻击时释放魔神附身 +0.4%#r 梅花三弄等级 +1#r #n#c00FCF0培养 0#r#n#r#c18CDCD未开启觉醒(开启铃、环、甲之觉醒，可领悟觉醒技)#r#n","#W【等级】1级#r【装备部位】兽甲#r#n#Y【等级需求】0转60级#r力量 +8#r 【品质】85/100#r【通灵】1/1000#r【耐久】488/500#r#n#G霹雳流星等级 +1#r 万佛朝宗等级 +1#r 抗强力克木 +0.5%#r #n#c00FCF0培养 0#r#n#r#c18CDCD未开启觉醒(开启铃、环、甲之觉醒，可领悟觉醒技)#r#n",}),"iType":102097,]),}),"iBoxAmount":5,"iZhuanzhiDir":0,"iAchievement":2915,"mpMultiRide":(["data":({"逐浪",}),"count":1,]),"mpFlyFabao":(["equip":"NyU0001qpos","skill":(["level":32,"progress":0,]),"flyfabao":(["NyU000016Bw":(["equip_time":1500877759,"cGblKey":"NyU000016Bw","iSpecial":0,"iLevel":1,"iLast":529,"iType":14364,"iKind":0,"iRep":0,]),"NyU0001qpos":(["equip_time":1512368099,"cGblKey":"NyU0001qpos","iSpecial":0,"iLevel":3,"iLast":4012,"iType":14411,"iKind":0,"iRep":210,]),]),]),"cName":"至尊·牛","iSuitPoint":15,"RideList":({(["iCor":5,"iStr":17,"iMag":5,"iTili":88,"cName":"黑熊","mpSkill":([2:({12,"增加AP2.5%，增加狂暴几率3.7%，忽视防御程度5.0%，忽视防御几率5.0%",}),1:({12,"增加SP0.9%，增加命中率1.9%，增加连击率1.9%，增加致命几率1.9%",}),]),"iGrade":2,"iUpExp":70,"iType":1,"iFlyup":0,]),(["iCor":11,"iStr":11,"iMag":5,"iTili":58,"cName":"黑豹","mpSkill":([9:({1,"增加HP最大值0.7%，抗混乱2.6%，抗昏睡2.6%，抗封印2.6%，抗遗忘2.6%",}),1:({1,"增加SP0.7%，增加命中率1.5%，增加连击率1.5%，增加致命几率1.5%",}),]),"iGrade":0,"iUpExp":0,"iType":2,"iFlyup":0,]),(["iCor":5,"iStr":5,"iMag":16,"iTili":400,"cName":"魔獜","mpSkill":([6:({219,"增加MP最大值5.2%，增加火系杀伤力7.9%，增加风系杀伤力7.9%，增加雷系杀伤力7.9%，增加水系杀伤力7.9%，增加鬼火杀伤力7.9%",}),8:({219,"增加SP0.8%，抗火2.5%，抗水2.5%，抗雷2.5%，抗风2.5%，抗鬼火2.5%",}),]),"iGrade":28,"iUpExp":12171,"iType":3,"iFlyup":0,]),(["iCor":11,"iStr":5,"iMag":11,"iTili":51,"cName":"巨魔狮","mpSkill":([9:({1,"增加HP最大值0.9%，抗混乱3.2%，抗昏睡3.2%，抗封印3.2%，抗遗忘3.2%",}),8:({1,"增加SP0.9%，抗火2.7%，抗水2.7%，抗雷2.7%，抗风2.7%，抗鬼火2.7%",}),]),"iGrade":0,"iUpExp":0,"iType":4,"iFlyup":0,]),(["iCor":15,"iStr":5,"iMag":5,"iTili":53,"cName":"奔雷","mpSkill":([7:({1,"增加HP最大值0.8%，抗物理3.3%，抗震慑1.6%，抗毒2.5%，抗三尸虫500",}),9:({1,"增加HP最大值0.8%，抗混乱2.9%，抗昏睡2.9%，抗封印2.9%，抗遗忘2.9%",}),]),"iGrade":0,"iUpExp":0,"iType":5,"iFlyup":0,]),(["iCor":5,"iStr":23,"iMag":0,"iTili":1300,"cName":"大风鸟","mpSkill":([2:({48927,"增加AP21.6%，增加狂暴几率32.4%，忽视防御程度43.2%，忽视防御几率43.2%",}),1:({49127,"增加SP7.9%，增加命中率15.8%，增加连击率15.8%，增加致命几率15.8%",}),]),"iGrade":100,"iUpExp":170676,"iType":6,"iFlyup":0,]),}),"iDevote":308444,"iMagicComplete":1,"iSp":1237,"iChengjiu":11700,"iUpExp":7401952,"iMag":148,"iDesc":11,"host":1540,"iFreeXianyu":0,"iRace":2,"iRei":3,"DiXingList":({}),])
		/// </summary>
		public string equip_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string detail_title_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string create_time_desc { get; set; }
		/// <summary>
		/// 天山雪
		/// </summary>
		public string equip_server_name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int storage_type { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string create_time { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int equipid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string allow_multi_order { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<List<string>> selling_info { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string selling_equipid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int collect_num { get; set; }
		/// <summary>
		/// ￥465.00 大话西游Ⅱ 地界-天山雪
		/// </summary>
		public string share_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string appointed_roleid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int areaid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string can_change_price { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int server_timestamp { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string owner_urs { get; set; }
		/// <summary>
		/// 天山雪
		/// </summary>
		public string server_name { get; set; }
		/// <summary>
		/// 3转148级
		/// </summary>
		public string level_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string allow_bargain { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string owner_roleid { get; set; }
		/// <summary>
		/// 地界
		/// </summary>
		public string equip_area_name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string equip_detail_url { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int equip_count { get; set; }
		/// <summary>
		/// 骨精灵 3转148级 小孩:3 最高评价:3233
		/// </summary>
		public string share_title { get; set; }
		/// <summary>
		/// 465.0元
		/// </summary>
		public string web_last_price_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int equip_level { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int sell_expire_remain_seconds { get; set; }
		/// <summary>
		/// 至尊·牛
		/// </summary>
		public string selling_nickname { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int price { get; set; }
		/// <summary>
		/// 小孩:3 最高评价:3233
		/// </summary>
		public string desc_sumup_short { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int status { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int accept_bargain { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<string> cross_buy_serverid_list { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string equip_type { get; set; }
		/// <summary>
		/// 地界
		/// </summary>
		public string area_name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string fair_show_end_time_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int serverid { get; set; }
		/// <summary>
		/// 上架中
		/// </summary>
		public string status_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string selling_roleid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string show_end_lock_time_tip { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string equip_type_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string price_desc { get; set; }
		/// <summary>
		/// 剩余 23时59分
		/// </summary>
		public string time_desc { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int pass_fair_show { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string is_my_equip { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int first_onsale_price { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int fair_show_poundage { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int kindid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string fair_show_end_time { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int owner_uid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string allow_cross_buy { get; set; }
		/// <summary>
		/// 小孩:3 最高评价:3233
		/// </summary>
		public string desc_sumup { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string game_ordersn { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string equip_face_img { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string raw_fair_show_end_time_desc { get; set; }
		/// <summary>
		/// 骨精灵
		/// </summary>
		public string equip_name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int is_due_offsale { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string allow_share { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string hostnum { get; set; }
		/// <summary>
		/// 骨精灵
		/// </summary>
		public string format_equip_name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string selling_time { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string icon { get; set; }
	}

	public class GoodDetail
	{
		/// <summary>
		/// 
		/// </summary>
		public int status { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Equip equip { get; set; }
	}
}
