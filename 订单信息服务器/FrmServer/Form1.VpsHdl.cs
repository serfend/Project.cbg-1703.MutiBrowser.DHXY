using DotNet4.Utilities.UtilReg;
using SfTcp;
using SfTcp.TcpServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 订单信息服务器
{
	public partial class Form1 
	{
		private class HdlServerInfo
		{
			string id;
			string name;
			string aeroId;
			string aeroName;

			string loginSession;
			int hdlNum;
			int nowNum;
			bool enable;
			public HdlServerInfo(string id, string name, string aeroId, string aeroName, int hdlNum)
			{
				this.Id = id;
				this.Name = name;
				this.AeroId = aeroId;
				this.AeroName = aeroName;
				this.HdlNum = this.NowNum = hdlNum;
			}

			public string Id { get => id; set => id = value; }
			public string Name { get => name; set => name = value; }
			public string AeroId { get => aeroId; set => aeroId = value; }
			public string AeroName { get => aeroName; set => aeroName = value; }
			public int HdlNum { get => hdlNum; set => hdlNum = value; }
			public int NowNum { get => nowNum; set => nowNum = value; }
			public bool Enable { get => enable; set => enable = value; }
			public string LoginSession { get => loginSession; set => loginSession = value; }

		}
		private class VPS
		{
			string name;
			string ip;
			private List<string> hdlServer;
			public VPS(string name, string ip)
			{
				this.Name = name;
				this.Ip = ip;
				HdlServer = new List<string>();
			}

			public string Name { get => name; set => name = value; }
			public string Ip { get => ip; set => ip = value; }
			public List<string> HdlServer { get => hdlServer; set => hdlServer = value; }
		}
		private Reg regServerInfo;
		private Dictionary<string, HdlServerInfo> serverInfoList = new Dictionary<string, HdlServerInfo>();//以区名对应区分配
		private Dictionary<string, VPS> allocVps = new Dictionary<string, VPS>();//以ip对应终端
		/// <summary>
		/// ip对应终端浏览器名称
		/// </summary>
		public Dictionary<string, string> _clientPayUser=new Dictionary<string, string>();
		private Reg regSettingVps;
		/// <summary>
		/// 将从任务列表中提取可用任务分配给VPS，VPS断开时撤回
		/// </summary>
		/// <param name="s"></param>
		private void BuildNewTaskToVps(TcpConnection s)
		{
			int singleHdl = 1;
			try
			{
				singleHdl = Convert.ToInt32(IpPerVPShdl.Text);
				if (singleHdl == 0)
				{
					IpPerVPShdl.Text = "1";
					singleHdl = 1;
				}
			}
			catch (Exception)
			{
				IpPerVPShdl.Text = singleHdl.ToString();
			}


			int interval = 1500, timeout = 100000;
			double assumePriceRate = 100;
			try
			{
				assumePriceRate = Convert.ToDouble(IpAssumePrice_Rate.Text);
			}
			catch (Exception)
			{
				IpAssumePrice_Rate.Text = assumePriceRate.ToString();
			}

			try
			{
				interval = Convert.ToInt32(IpTaskInterval.Text);
			}
			catch (Exception)
			{
				IpTaskInterval.Text = interval.ToString();
			}
			s.Send(new SfTcp.TcpMessage.CmdSynInitMessage(interval, timeout, ManagerHttpBase.TargetUrl, assumePriceRate));
		}
	}
}
