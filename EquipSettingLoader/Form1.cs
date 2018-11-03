using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotNet4.Utilities.UtilCode;
using DotNet4.Utilities.UtilReg;
namespace EquipSettingLoader
{
	public partial class Form1 : Form
	{
		private Reg setting = new Reg("sfMinerDigger").In("Main").In("Setting").In("ServerData");
		public Form1()
		{
			InitializeComponent();
			OpShowLog.ShortcutsEnabled = false;
			SynButton();
		}

		private void CmdLoadEquipSetting_Click(object sender, EventArgs e)
		{
			var f = new OpenFileDialog()
			{
				Title = "读取装备表",
			};
			if (f.ShowDialog() == DialogResult.OK)
			{
				var filer = File.ReadAllText(f.FileName,Encoding.Default);
				var files = filer.Split(new string[] { "this." },StringSplitOptions.RemoveEmptyEntries);
				StringBuilder counter = new StringBuilder();
				foreach ( var line in files)
				{
					var ipair = line.Split('=');
					HdlData(ipair[0],ipair[1],  counter);
				}

				MessageBox.Show(string.Format("加载{0}个匹配表:{1}", files.Length,counter));
			}
		}

		private void HdlData(string v1, string v2,StringBuilder counter)
		{
			var reg = setting.In(v1);
			var node = new Node(ref v2,0, '\0');
			string key = "", value = "";
			var c = node.FirstChild;
			int nowIndex = 0;
			while (c!=null)
			{
				nowIndex++;
				if (c.Key == "")
				{
					key = c.FirstChild.Key;
					value = c.FirstChild.Next.Key?.Replace("\"","");
					
				}
				else
				{
					key = c.Key;
					value = c.Value?.Replace("\"", "");
					if (value == null)
					{
						value = key;
						key = nowIndex.ToString();
					}
				}

				reg.SetInfo(key, value);
				c = c.Next;
			}
			counter.AppendLine(string.Format("{0} 共计 {1} 项",v1,nowIndex));
		}
		private class Node
		{
			public override string ToString()
			{
				return string.Format("{0}={1}",Key,Value);
			}


			private Node next;
			private Node firstChild;
			private string key, value;
			private int keyBegin, keyEnd;
			private int valueBegin;
			public string Key { get => key; }
			public string Value { get => value; }
			private int endIndex, beginIndex;
			private bool keyBeenRead = false;
			private bool OnStringList = false;
			public bool haveNext = false;
			public Node(ref string info,int beginIndex,char beginWith)
			{
				this.beginIndex = beginIndex;
				keyEnd=keyBegin = beginIndex;
				for(int i = beginIndex; i < info.Length; i++)
				{
					var chr = info[i];
					//Console.WriteLine(string.Format("{0}:{1}",i,chr));
					if (OnStringList)
					{
						if (chr == '\'' || chr == '\"')
						{

						}
						else
						{
							if (keyBeenRead)
							{
							}
							else
							{
								keyEnd++;
							}
							continue;
						}
					}//排除所有的字符串内值
					switch (chr)
					{
						case '{':
						case '[':
							{
								var theChild = new Node(ref info, i + 1, info[i]);
								firstChild  = theChild;
								while (theChild.haveNext)
								{
									theChild.next = new Node(ref info, theChild.EndIndex+2, info[i]);
									theChild = theChild.next;
								}
								i = theChild.EndIndex ;
								break;
							}
						case ']':
						case '}':
							{
								if ((beginWith == '{' && info[i] == '}') || (beginWith=='['&& info[i]==']'))
								{
									endIndex = i;
									if (keyEnd>0)key = info.Substring(keyBegin, keyEnd - keyBegin);
									if(valueBegin>0)value = info.Substring(valueBegin, endIndex - valueBegin);
									return;
								}
								else
								{
									throw new Exception("数据匹配失败");
								}
							}
						case ',':
							{
								endIndex = i-1;
								if (keyBegin > 0) key = info.Substring(keyBegin, keyEnd - keyBegin );
								if (valueBegin > 0) value = info.Substring(valueBegin, endIndex - valueBegin+1);
								haveNext = true;
								return;
							}
						case ':':
							{
								keyBeenRead = true;
								valueBegin = i+1;
								break;
							}
						case '\"':
						case '\'':
							{
								OnStringList = !OnStringList;
								if (keyBeenRead)
								{
								}
								else
								{
									keyEnd++;
								}
								break;
							}
						default:
							{
								if (keyBeenRead)
								{
								}
								else
								{
									keyEnd++;
								}
								break;
							}
					}
				}
			}

			public int EndIndex { get => endIndex; set => endIndex = value; }
			public Node Next { get => next; set => next = value; }
			public Node FirstChild { get => firstChild; set => firstChild = value; }
		}
		private Reg IpChecker = new Reg("sfMinerDigger").In("Main").In("Data").In("RecordIp");
		private void SynButton() {
			CmdCheckIpValue.Text = IpChecker.GetInfo("checkIpValue") == "1"? "允许本机ip" : "检查使用代理";
			cmdEnableIpDetect.Text = IpChecker.GetInfo("checkIpRecord") == "1" ? "允许重复ip" : "去除重复ip";
		}
		private void CmdCheckIpValue_Click(object sender,EventArgs e)
		{
			if (sender is Button button)
			{
				if (button.Text == "检查使用代理")
				{
					button.Text = "允许本机ip";
					IpChecker.SetInfo("checkIpValue", "1");
				}
				else
				{
					button.Text = "检查使用代理";
					IpChecker.SetInfo("checkIpValue", "0");
				}
			};
		}
		private void CmdEnableIpDetect(object sender,EventArgs e)
		{
			if (sender is Button button) {
				if (button.Text == "去除重复ip")
				{
					button.Text = "允许重复ip";
					IpChecker.SetInfo("checkIpRecord","1");
				}
				else
				{
					button.Text = "去除重复ip";
					IpChecker.SetInfo("checkIpRecord", "0");
				}
			};

		}
		private void CmdLoadLog_Click(object sender, EventArgs e)
		{
			var f = new OpenFileDialog()
			{
				Title="选择需要读取的日志文件",
				Filter="log|*.log"
			};
			if (f.ShowDialog() == DialogResult.OK)
			{
				OpShowLog.Clear();
				//var fo = new SaveFileDialog()
				//{
				//	Title="选择输出",
				//	Filter="log|*.log"
				//};
				//if (fo.ShowDialog() == DialogResult.OK)
				//{
				string tmpInfo = "";
				//	using (var fs = new StreamReader(f.FileName, Encoding.Default))
				//	{
				//		using (var fos = new StreamWriter(fo.FileName, false, Encoding.Default))
				//		{
				//			while ((tmpInfo = fs.ReadLine()) != null)
				//			{
				//				var rawInf = EncryptHelper.AESDecrypt(tmpInfo);
				//				fos.WriteLine(rawInf);
				//			}
				//		}
				//	}
				//	MessageBox.Show("输出完成");
				//}
				using (var fs = new StreamReader(f.FileName, Encoding.Default))
				{
					while ((tmpInfo = fs.ReadLine()) != null)
					{
						var rawInf = EncryptHelper.AESDecrypt(tmpInfo);
						OpShowLog.AppendText("\n");
						OpShowLog.AppendText(rawInf);
					}
				}
			}
		}

		private void OpShowLog_TextChanged(object sender, EventArgs e)
		{

		}

		private void OpShowLog_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
		}

		private void OpShowLog_MouseDown(object sender, MouseEventArgs e)
		{
			
		}
	}
}
