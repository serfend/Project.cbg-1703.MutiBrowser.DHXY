using DotNet4.Utilities.UtilReg;
using SfTcp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 订单信息服务器
{
	public partial class Form1 : Form
	{
		private class HdlServerInfo
		{
			string id;
			string name;
			string aeroId;
			string aeroName;
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
		}
		private class VPS
		{
			string name;
			string ip;
			public List<string> hdlServer;

			public VPS(string name, string ip)
			{
				this.Name = name;
				this.Ip = ip;
				hdlServer = new List<string>();
			}

			public string Name { get => name; set => name = value; }
			public string Ip { get => ip; set => ip = value; }
		}
		private Reg regServerInfo;
		private Dictionary<string, HdlServerInfo> serverInfoList = new Dictionary<string, HdlServerInfo>();
		private Dictionary<string, VPS> allocServer = new Dictionary<string, VPS>();//以ip对应终端
		private Reg regSettingVps;
		/// <summary>
		/// 将从任务列表中提取可用任务分配给VPS，VPS断开时撤回
		/// </summary>
		/// <param name="s"></param>
		private void BuildNewTaskToVps(TcpServer s, out string taskTitle)
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
			string hdlServer = GetFreeServer(singleHdl, s.Ip, out taskTitle);
			int interval = 1500, timeout = 100000;

			try
			{
				timeout = Convert.ToInt32(IpTaskTimeOut.Text);
			}
			catch (Exception)
			{
				IpTaskTimeOut.Text = timeout.ToString();
			}
			try
			{
				interval = Convert.ToInt32(IpTaskInterval.Text);
			}
			catch (Exception)
			{
				IpTaskInterval.Text = interval.ToString();
			}
			s.Send(string.Format("<SynInit><interval>{0}</interval><task>{1}</task><timeout>{2}</timeout></SynInit><InnerTargetUrl>{3}</InnerTargetUrl>", interval, hdlServer, timeout, ManagerHttpBase.TargetUrl));
		}
	}
}
