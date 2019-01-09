using DotNet4.Utilities.UtilInput;
using DotNet4.Utilities.UtilReg;
using Newtonsoft.Json;
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
using 订单信息服务器.Properties.Bill;
using 订单信息服务器.WebServerControl;

namespace 订单信息服务器
{
	
	public partial class Form1 : Form
	{
		/// <summary>
		/// 手机号对应的登录
		/// </summary>
		private Dictionary<string, PayUser> _paySession=new Dictionary<string, PayUser>();
		/// <summary>
		/// 区号对应手机号
		/// </summary>
		private Dictionary<string, string> _payServerHdl = new Dictionary<string, string>();

		private Reg regMain = new Reg().In("Setting").In("PaySession");

		/// <summary>
		/// 用于记录注册表索引,请勿外部使用
		/// </summary>
		private Dictionary<int, string> _keyPairPaySession=new Dictionary<int, string>();

		private string _authKey;
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
				var psw = item.GetInfo("psw");
				var data = new string[4];
				data[0] = name;
				data[1] = session;
				data[2] = hdlServer;
				data[3] = psw;
				_paySession.Add(name, new PayUser(name, session, hdlServer, psw));
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
				var data = new string[4];
				data[0] = phone;
				item = new ListViewItem(data);
				LstPayClient.MultiSelect = false;
				item.Selected = true;
				LstPayClient.Items.Add(item);
			}
			else
			{
				if (MessageBox.Show(this, $"已存在手机号{phone}\nkey:{item.SubItems[1].Text}\n新增将导致覆盖,确认新增吗?", "确认", MessageBoxButtons.OKCancel) == DialogResult.Cancel) {
					return;
				} ;
			}
			
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
			var session = InputBox.ShowInputBox("输入登录凭证", "凭证可从网页listView.html中复制cookie的值", item.SubItems[1].Text);
			var hdlServer = InputBox.ShowInputBox("输入管理区", "管理区以区号码记录，\"|\"分割",item.SubItems[2].Text);
			var psw = InputBox.ShowInputBox("输入密码", "账号的密码", item.SubItems[3].Text);
			item.SubItems[1].Text = session;
			var list = hdlServer.Split('|');
			var sortlist=list.ToList<string>();
			sortlist.Sort((x,y)=> {
				return Convert.ToInt32(x) - Convert.ToInt32(y);
			});
			item.SubItems[2].Text = string.Join<string>("|", sortlist);
			item.SubItems[3].Text = psw;
			var user= new PayUser(item.SubItems[0].Text, session, hdlServer, psw);
			if (!_paySession.ContainsKey(item.SubItems[0].Text)) _paySession.Add(user.UserName, user);
			else _paySession[user.UserName] = user;
			
			foreach (var i in list)
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
			CmdPaySaveItem(item.SubItems[0].Text, session, hdlServer,psw);
		}
		private void CmdPaySaveItem(string phone,string session,string hdlServer,string psw)
		{
			var item = regMain.In(phone);
			if (!_keyPairPaySession.ContainsValue(phone))
			{
				_keyPairPaySession.Add(_keyPairPaySession.Count + 1, phone);
				regMain.SetInfo(_keyPairPaySession.Count.ToString(), phone);
			}
			item.SetInfo("session", session) ;
			item.SetInfo("hdlServer",hdlServer);
			item.SetInfo("psw", psw);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serverNo"></param>
		/// <param name="InnerInfo"></param>
		private void PayCurrentBill(string serverNo,string InnerInfo="")
		{
			if (_payServerHdl.ContainsKey(serverNo))
			{
				var phoneTarget = _payServerHdl[serverNo];
				if (_paySession.ContainsKey(phoneTarget))
				{
					//TODO 获取支付凭证并付款
					var session = _paySession[phoneTarget];
					if (MessageBox.Show(this, $"订单:\n{InnerInfo}", "付款确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						PayCurrentBill(session);
					}
				}
				else
				{
					AppendLog($"当前区{serverNo}管理的账号{phoneTarget}无支付凭证");
				}
			}
			else
			{
				AppendLog($"当前区{serverNo}暂无管理的账号");
			};
		}
		private void PayCurrentBill(PayUser user)
		{
			PayCurrentBill(user.UserName,user.Session, user.Psw, AuthKey);
		}
		/// <summary>
		/// 提交当前订单付款行为
		/// 【future】当密码或将军令为空时，将从基类中获取数据
		/// </summary>
		/// <param name="name">下单用户名称</param>
		/// <param name="session">付款凭证</param>
		/// <param name="psw">预置密码</param>
		/// <param name="authKey">预置将军令</param>
		private void PayCurrentBill(string name,string session, string psw = null, string authKey = null)
		{
			if (!payClientIp.ContainsKey(name)) {
				MessageBox.Show($"用户名{name}所对应的浏览器未开启");
				return;
			} ;
			if (!payClient.ContainsKey(payClientIp[name]))
			{
				MessageBox.Show($"浏览器终端[{name}]未启动");
				return;
			}
			bool anyException = false;
			var root = new BillInfo(session);
			Client client = null;
			NewBillMessage item = null;
			var getBillInfo = new Task(()=> {
				try
				{
					root.GetData();
				}
				catch (Exception ex)
				{
					anyException = true;
					MessageBox.Show($"订单失败:{ex.Message}");
					return;
				}
			});
			var userInput = new Task(()=> {
				
				if (authKey == null) authKey = InputBox.ShowInputBox("输入将军令", "当前将军令为空，请输入");
				client = payClient[payClientIp[name]];
				item = new NewBillMessage()
				{
					BillInfo = new NewBillMessage.SubmitBillInfo()
					{
						Ekey = authKey,
						Psw = psw,
					}
				};
			});
			userInput.Start();
			getBillInfo.Start();
			Task.WaitAll(new Task[] { userInput, getBillInfo });
			if (anyException)
			{
				return;
			}
			item.BillInfo.OrderId = root.FirstBill.orderId;
			client.Session.Send(JsonConvert.SerializeObject(item));
			

		}
		/// <summary>
		/// 检查订单号是否被处理过
		/// </summary>
		private Dictionary<string, bool> _BillRecord = new Dictionary<string, bool>();
		private string lastExceptAuthCode;
		public string AuthKey { get => _authKey; set {
				if (value.Length != 6)
				{
					if(lastExceptAuthCode==value)
						AppendLog($"【警告】新的将军令可能有误,其为:{value}");
					lastExceptAuthCode = value;
					return;
				}
				OpAuthCodeShow.Text = $"将军令:{value}";
				_authKey = value;
			} }

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

				var price = priceInfo.Split('/');
				double priceNum = 0, priceNumAssume = 0;
				if (price.Length == 2)
				{
					priceNum = Convert.ToDouble(price[0]);
					priceNumAssume = Convert.ToDouble(price[1]);
				}
				SendCmdToBrowserClient(serverName, $"<newCheckBill><targetUrl>{BuyUrl}</targetUrl><price>{priceNum}</price><assumePrice>{priceNumAssume }</assumePrice>");


				AppendLog("新的订单:" + BuyUrl);
				_BillRecord.Add(BuyUrl, true);
				LstGoodShow.Items.Insert(0, new ListViewItem(tmp));
				if (LstGoodShow.Items.Count > 10) LstGoodShow.Items[10].Remove();

				if (priceNum < priceNumAssume)
				{
					var earnNum = priceNumAssume - priceNum;
					if (earnNum / priceNum < 5)
					{
						ManagerHttpBase.RecordMoneyGet += earnNum;
						ManagerHttpBase.RecordMoneyGetTime++;
					}
					PayCurrentBill(serverNum, InnerInfo);
				}
				ManagerHttpBase.FitWebShowTime++;
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
