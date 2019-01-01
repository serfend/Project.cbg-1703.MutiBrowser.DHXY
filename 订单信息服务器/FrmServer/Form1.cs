using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilInput;
using DotNet4.Utilities.UtilReg;
using SfTcp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using 订单信息服务器.Bill;

namespace 订单信息服务器
{
	public partial class Form1 : Form
	{
		private TcpServerManager serverManager;
		private Reg regSetting;
		public Form1()
		{
			regSetting = new Reg("sfMinerDigger").In("Setting");
			regSettingVps = regSetting.In("vps");
			regServerInfo = regSetting.In("ServerInfo");
			InitializeComponent();
			InitHistorySettingOnFormctl();
			InitTransferEngine();
			//InitServerTaskList();
			InitServerManager();
			StartTaskSchedule();
			InitPaySession();
			//ServerJsManager.Init();
			LstConnection.AfterLabelEdit += CheckIfUserEditName;
		}
		private bool ctlSaveLoaded = false;
		private void InitHistorySettingOnFormctl()
		{
			var frmSetting = regSetting.In("Form").In("ServerFormMain");
			foreach (var ctl in this.TabMain_Setting.Controls)
			{
				if (ctl is TextBox t)
				{
					t.Text = frmSetting.GetInfo(t.Name, t.Text);
					if (!ctlSaveLoaded)
					{
						t.TextChanged += (x, xx) => {
							frmSetting.SetInfo(t.Name, t.Text);
						};
					}
				}
			}
			ctlSaveLoaded = true;

		}


		
		
		
		private void SynLstTask()
		{
			for(int i = 0; i < LstServerQueue.Items.Count; i++)
			{
				var item = LstServerQueue.Items[i];
				var id = item.SubItems[0].Text;
				if (serverInfoList.ContainsKey(id))
				{
					item.SubItems[2].Text = (serverInfoList[id].HdlNum-serverInfoList[id].NowNum).ToString();
				}
			}
		}



		private bool _taskAllocatePause = false;
		/// <summary>
		/// 无需要的任务时则返回"Idle"，终端进入休眠模式，30秒后再次询问
		/// </summary>
		/// <param name="singleHdl"></param>
		/// <param name="ip"></param>
		/// <param name="taskTitle">输出本次所有任务的标题</param>
		/// <returns></returns>
		private string GetFreeServer(int singleHdl, string ip, out string taskTitle)
		{
			if (_taskAllocatePause)
			{
				taskTitle = "休眠状态";
				return "Idle";
			}
			var vps = new VPS("", ip);
			var taskInfo = new StringBuilder();
			var tTitle = new StringBuilder();
			foreach (var s in serverInfoList)
			{
				if (singleHdl <= 0) break;
				if (s.Value.NowNum > 0 && s.Value.Enable)
				{
					var t = s.Value;
					t.NowNum--;
					singleHdl--;
					vps.HdlServer.Add(t.Name);
					if (taskInfo.Length > 0)
					{
						tTitle.Append(",");
						taskInfo.Append("#");
					}
					taskInfo.Append($"<id>{t.Id}</id><serverName>{t.Name}</serverName><aeroId>{t.AeroId}</aeroId><aeroName>{t.AeroName}</aeroName><loginSession>{t.LoginSession}</loginSession>");
					tTitle.Append(t.Name);
				}
			}
			taskTitle = "Idle";
			if (taskInfo.Length == 0) return taskTitle;
			taskTitle = tTitle.ToString();
			allocServer.Add(vps.Ip, vps);
			return taskInfo.ToString();
		}
		
		
		

		private void CheckIfUserEditName(object sender, LabelEditEventArgs e)
		{
			try
			{
				var ip = LstConnection.Items[e.Item].SubItems[2].Text;
				TcpServer target = serverManager[ip];
				var clientName = e.Label;
				regSettingVps.In(target.ID).SetInfo("clientName", clientName);
				target.Send(string.Format("<setClientName>{0}</setClientName>", clientName));
			}
			catch (Exception ex)
			{
				AppendLog("修改终端名称失败:" + ex.Message);
			}

		}
		public void AppendLog(string info)
		{
			OpLog.AppendText("\n");
			OpLog.AppendText(string.Format("{0}>>{1}",DateTime.Now,info));
			if (OpLog.Text.Length > 1000)
			{
				OpLog.Clear();
			}
		}
		private ListViewItem GetItem(string ip)
		{
			foreach (var item in LstConnection.Items)
			{
				if (item is ListViewItem it)
				{
					if (it.SubItems[2].Text == ip) return it;
				}
			}
			return null;
		}
		#region event

		private void CmdDisconnect_Click(object sender, EventArgs e)
		{
			try
			{
				var nowSelect = LstConnection.SelectedItems[0].SubItems[2].Text;
				var tcp = serverManager[nowSelect];
				tcp.Disconnect();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void CmdRedial_Click(object sender, EventArgs e)
		{
			try
			{
				if (LstConnection.SelectedItems[0].SubItems[1].Text != "vps")
				{
					MessageBox.Show("仅VPS终端支持重新拨号");
					return;
				}
				var nowSelect = LstConnection.SelectedItems[0].SubItems[2].Text;
				var tcp = serverManager[nowSelect];
				tcp.Send("<reRasdial>");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		/// <summary>
		/// 切换启用状态
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LstServerQueue_DoubleClick(object sender, EventArgs e)
		{
			var targetServer = LstServerQueue.SelectedItems[0].SubItems[1].Text;
			if (!serverInfoList.ContainsKey(targetServer)) {
				var list = new StringBuilder();
				foreach (var item in serverInfoList) list.AppendLine($"{item.Value.Id}({item.Value.Name}):{item.Value.HdlNum}");
				MessageBox.Show($"当前区:{targetServer}未进入到列表中:\n{list.ToString()}");
				return;
			}
			if (LstServerQueue.SelectedItems[0].SubItems[4].Text == "启用")
			{
				LstServerQueue.SelectedItems[0].SubItems[4].Text = "禁用";
				serverInfoList[targetServer].Enable = false;
			}
			else
			{
				serverInfoList[targetServer].Enable = true;
				LstServerQueue.SelectedItems[0].SubItems[4].Text = "启用";
			}
			regServerInfo.SetInfo(LstServerQueue.SelectedItems[0].SubItems[0].Text, LstServerQueue.SelectedItems[0].SubItems[4].Text);
		}

		private void LstGoodShow_DoubleClick_1(object sender, EventArgs e)
		{
			var target = LstGoodShow.SelectedItems[0];
			var targetUrl = target.SubItems[8].Text;
			Clipboard.SetText(targetUrl);
			ManagerHttpBase.UserWebShowTime++;
			SendCmdToBrowserClient(target.SubItems[0].Text, $"<showWeb><targetUrl>{targetUrl}</targetUrl></showWeb>");
		}

		private void CmdServerOn_Click(object sender, EventArgs e)
		{
			try
			{
				var nowSelect = LstConnection.SelectedItems[0].SubItems[2].Text;
				var tcp = serverManager[nowSelect];
				tcp.Send(IpSender.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		private void CmdPauseTaskAllocate_Click(object sender, EventArgs e)
		{
			_taskAllocatePause = !_taskAllocatePause;
			if (_taskAllocatePause) CmdPauseTaskAllocate.Text = "唤醒终端";
			else CmdPauseTaskAllocate.Text = "暂停终端";
		}


		#endregion

		private void CmdPayBill_Click(object sender, EventArgs e)
		{
			var targetUser = InputBox.ShowInputBox("输入付款手机号", "输入付款手机号", "");
			if (!_paySession.ContainsKey(targetUser))
			{
				MessageBox.Show("无效的手机号");
				return;
			}
			PayCurrentBill(_paySession[targetUser]);
		}
	}
}
