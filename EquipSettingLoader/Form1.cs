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
				var filer = File.ReadAllText(f.FileName,Encoding.Default).Replace("\n","").Replace("\r","").Replace(";","");
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
			public string Key { get => Key1; }
			public string Value { get => Value1; }
			private int endIndex, beginIndex;
			private bool keyBeenRead = false;
			private bool OnStringList = false;
			public bool haveNext = false;
			public Node(ref string info,int beginIndex,char beginWith)
			{
				this.BeginIndex = beginIndex;
				KeyEnd=KeyBegin = beginIndex;
				for(int i = beginIndex; i < info.Length; i++)
				{
					var chr = info[i];
					//Console.WriteLine(string.Format("{0}:{1}",i,chr));
					if (OnStringList1)
					{
						if (chr == '\'' || chr == '\"')
						{

						}
						else
						{
							if (KeyBeenRead)
							{
							}
							else
							{
								KeyEnd++;
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
									EndIndex1 = i;
									if (KeyEnd>0)Key1 = info.Substring(KeyBegin, KeyEnd - KeyBegin);
									if(ValueBegin>0)Value1 = info.Substring(ValueBegin, EndIndex1 - ValueBegin);
									return;
								}
								else
								{
									//throw new Exception("数据匹配失败");
								}
								break;
							}
						case ',':
							{
								EndIndex1 = i-1;
								if (KeyBegin > 0) Key1 = info.Substring(KeyBegin, KeyEnd - KeyBegin );
								if (ValueBegin > 0) Value1 = info.Substring(ValueBegin, EndIndex1 - ValueBegin+1);
								haveNext = true;
								return;
							}
						case ':':
							{
								KeyBeenRead = true;
								ValueBegin = i+1;
								break;
							}
						case '\"':
						case '\'':
							{
								OnStringList1 = !OnStringList1;
								if (KeyBeenRead)
								{
								}
								else
								{
									KeyEnd++;
								}
								break;
							}
						default:
							{
								if (KeyBeenRead)
								{
								}
								else
								{
									KeyEnd++;
								}
								break;
							}
					}
				}
			}

			public int EndIndex { get => EndIndex1; set => EndIndex1 = value; }
			public Node Next { get => next; set => next = value; }
			public Node FirstChild { get => firstChild; set => firstChild = value; }
			public string Key1 { get => key; set => key = value; }
			public string Value1 { get => value; set => this.value = value; }
			public int KeyBegin { get => keyBegin; set => keyBegin = value; }
			public int KeyEnd { get => keyEnd; set => keyEnd = value; }
			public int ValueBegin { get => valueBegin; set => valueBegin = value; }
			public int EndIndex1 { get => endIndex; set => endIndex = value; }
			public int BeginIndex { get => beginIndex; set => beginIndex = value; }
			public bool KeyBeenRead { get => keyBeenRead; set => keyBeenRead = value; }
			public bool OnStringList1 { get => OnStringList; set => OnStringList = value; }
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
				bool devMode= (new Reg("sfMinerDigger").In("Setting").GetInfo("developeModel") == "1") ;
				var fo = new SaveFileDialog()
				{
					Title = "选择输出",
					Filter = "log|*.log"
				};
				StreamWriter fos=null;
				if (devMode)
				{
					MessageBox.Show("2333");
					if (fo.ShowDialog() == DialogResult.OK)
					{
						fos = new StreamWriter(fo.FileName, false, Encoding.Default);
					}
				}
				string tmpInfo = "";

				
				using (var fs = new StreamReader(f.FileName, Encoding.Default))
				{

					var cst = new StringBuilder();
					while ((tmpInfo = fs.ReadLine()) != null)
					{
						var rawInf = EncryptHelper.AESDecrypt(tmpInfo);
						cst.AppendLine(rawInf);
					}
					if (fos != null) {
						fos.WriteLine(cst.ToString());
						fos.Dispose();
					}
					OpShowLog.Text = cst.ToString();
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
