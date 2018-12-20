using DotNet4.Utilities.UtilInput;
using DotNet4.Utilities.UtilReg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 订单信息服务器.Bill;

namespace 订单信息服务器
{
	public partial class Form1 : Form
	{
		/// <summary>
		/// 同步将军令
		/// </summary>
		private string _payAuthKey = "";
		/// <summary>
		/// 手机号对应的登录
		/// </summary>
		private Dictionary<string, string> _paySession=new Dictionary<string, string>();
		/// <summary>
		/// 区号对应手机号
		/// </summary>
		private Dictionary<string, string> _payServerHdl = new Dictionary<string, string>();
		private Reg regMain = new Reg().In("Setting").In("PaySession");

		/// <summary>
		/// 用于记录注册表索引,请勿外部使用
		/// </summary>
		private Dictionary<int, string> _keyPairPaySession=new Dictionary<int, string>();

		public string PayAuthKey { get => _payAuthKey; set {
				_payAuthKey = value;
				if (value.Length != 6)
				{
					MessageBox.Show($"【警告】新的将军令可能有误,其为:{value}");
				}
			} }

		/// <summary>
		/// 初始化支付登录凭证记录
		/// </summary>
		private void InitPaySession()
		{
			for(int i =1; ; i++)
			{
				var name= regMain.GetInfo(i.ToString());
				if (name == null||name.Length==0) return;
				_keyPairPaySession.Add(i, name);
				var item = regMain.In(name);
				var session = item.GetInfo("session");
				var hdlServer = item.GetInfo("hdlServer");
				var data = new string[3];
				data[0] = name;
				data[1] = session;
				data[2] = hdlServer;
				var tmp = new ListViewItem(data);
				LstPayClient.Items.Add(tmp);
			}
		}
		private void CmdPay_NewVerify_Click(object sender, EventArgs e)
		{
			var phone = InputBox.ShowInputBox("输入账号", "支付页面登录的账号");
			var item = GetVerifyItem(phone);
			if (item == null)
			{
				var data = new string[3];
				data[0] = phone;
				item = new ListViewItem(data);
				LstPayClient.Items.Add(item);
			}
			else
			{
				if (MessageBox.Show(this, $"已存在手机号{phone}\nkey:{item.SubItems[1].Text}\n新增将导致覆盖,确认新增吗?", "确认", MessageBoxButtons.OKCancel) == DialogResult.Cancel) {
					return;
				} ;
			}
			LstPayClient.MultiSelect = false;
			item.Selected = true;
			CmdPay_EditVerify_Click(this, EventArgs.Empty);
		}

		private void CmdPay_EditVerify_Click(object sender, EventArgs e)
		{
			if (LstPayClient.SelectedItems.Count == 0)
			{
				MessageBox.Show("未选中任何项");
				return;
			}
			var item = LstPayClient.SelectedItems[0];
			var key = InputBox.ShowInputBox("输入登录凭证", "凭证可从网页cookies中复制NTES_SESS的值", item.SubItems[1].Text);
			var hdlServer = InputBox.ShowInputBox("输入管理区", "管理区以区号码记录，\"|\"分割",item.SubItems[2].Text);
			item.SubItems[1].Text = key;
			item.SubItems[2].Text = hdlServer;
			if (!_paySession.ContainsKey(item.SubItems[0].Text)) _paySession.Add(item.SubItems[0].Text, key);
			else _paySession[item.SubItems[0].Text] = key;
			var list = item.SubItems[2].Text.Split('|');
			foreach(var i in list)
			{
				if (!_payServerHdl.ContainsKey(i))
				{
					_payServerHdl.Add(i, item.SubItems[0].Text);
				}
				else
				{
					_payServerHdl[i] = item.SubItems[0].Text;
				}
			}
			CmdPaySaveItem(item.SubItems[0].Text, key, hdlServer);
		}
		private void CmdPaySaveItem(string phone,string session,string hdlServer)
		{
			var item = regMain.In(phone);
			if (!_keyPairPaySession.ContainsValue(phone))
			{
				_keyPairPaySession.Add(_keyPairPaySession.Count + 1, phone);
				regMain.SetInfo(_keyPairPaySession.Count.ToString(), phone);
			}
			item.SetInfo("session", session) ;
			item.SetInfo("hdlServer",hdlServer);

		}
		/// <summary>
		/// 提交当前订单付款行为
		/// 【future】当密码或将军令为空时，将从基类中获取数据
		/// </summary>
		/// <param name="session">付款凭证</param>
		/// <param name="psw">预置密码</param>
		/// <param name="authKey">预置将军令</param>
		private void PayCurrentBill(string session, string psw = null, string authKey = null)
		{
			BillInfo root = null;
			try
			{
				root = new BillInfo(session);
			}
			catch (Exception ex)
			{
				MessageBox.Show("加载订单列表失败:" + ex.Message);
				return;
			}

			root.GetData();
			var f = new EnterPassword(psw, root);
			f.Submit();
			if (!f.Success)
			{
				MessageBox.Show("密码提交失败:" + f.Data.errorMsg);
				return;
			}
			var f2 = new EnterAuthKey(root);
			f2.Submit(authKey);
			if (!f2.Success)
			{
				MessageBox.Show("将军令提交失败:" + f2.Data.errorMsg);
				return;
			}
		}
		/// <summary>
		/// 检查订单号是否被处理过
		/// </summary>
		private Dictionary<string, bool> _BillRecord = new Dictionary<string, bool>();
		/// <summary>
		/// 将新的物品添加到商品列表
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="InnerInfo"></param>
		private void HdlNewCheckBill(string sender, string InnerInfo)
		{
			try
			{
				var tmp = InnerInfo.Split(new string[] { "##" }, StringSplitOptions.None);
				if (tmp.Length < 10)
				{
					AppendLog(sender + " 无效的订单信息:" + InnerInfo);
					return;
				}
				var serverName = tmp[0];
				var goodName = tmp[1];
				var priceInfo = tmp[2];
				var Rank = tmp[3];
				var ITalent = tmp[4];
				var IAchievement = tmp[5];
				var IChengjiu = tmp[6];
				var ISingleEnergyRate = tmp[7];
				var BuyUrl = tmp[8];
				var serverNum = tmp[9];
				if (_BillRecord.ContainsKey(BuyUrl))
				{
					//AppendLog(ordersn + "已出现过此订单," + serverName);
					return;
				}
				AppendLog("新的订单:" + BuyUrl);
				_BillRecord.Add(BuyUrl, true);
				LstGoodShow.Items.Insert(0, new ListViewItem(tmp));
				if (LstGoodShow.Items.Count > 10) LstGoodShow.Items[10].Remove();

				var price = priceInfo.Split('/');
				double priceNum = 0, priceNumAssume = 0;
				if (price.Length == 2)
				{
					priceNum = Convert.ToDouble(price[0]);
					priceNumAssume = Convert.ToDouble(price[1]);
					//priceNumAssume *= (Convert.ToDouble(IpAssumePrice_Rate.Text) / 100);
					if (priceNum < priceNumAssume)
					{

						var earnNum = priceNumAssume - priceNum;
						if (earnNum / priceNum < 5)
						{
							ManagerHttpBase.RecordMoneyGet += earnNum;
							ManagerHttpBase.RecordMoneyGetTime++;
						}
					}
					ManagerHttpBase.FitWebShowTime++;
				}
				if (_payServerHdl.ContainsKey(serverNum))
				{
					var phoneTarget = _payServerHdl[serverNum];
					if (_paySession.ContainsKey(phoneTarget))
					{
						//TODO 获取支付凭证并付款
						var session = _paySession[phoneTarget];
						if (MessageBox.Show(this, $"订单:\n{InnerInfo}", "付款确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
						{
							//PayCurrentBill(session, "121212", PayAuthKey);
						}
					}
					else
					{
						AppendLog($"当前区{serverNum}管理的账号{phoneTarget}无支付凭证");
					}
				}
				else
				{
					AppendLog($"当前区{serverNum}暂无管理的账号");
				};

				SendCmdToBrowserClient(serverName, $"<newCheckBill><targetUrl>{BuyUrl}</targetUrl><price>{priceNum}</price><assumePrice>{priceNumAssume }</assumePrice>");
			}
			catch (Exception ex)
			{
				AppendLog("订单处理异常:" + ex.Message);
			}
		}
		/// <summary>
		/// 通过账号获取item
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private ListViewItem GetVerifyItem(string key)
		{
			foreach(var item in LstPayClient.Items)
			{
				if(item is ListViewItem i)
				{
					if (i.SubItems[0].Text == key) return i;
				}
			}
			return null;
		}
	}
}
