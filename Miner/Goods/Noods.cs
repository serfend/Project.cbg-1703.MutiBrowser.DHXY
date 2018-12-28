using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miner
{
namespace Goods
	{

		public class Node
		{
			public Node parent;
			public Node next;
			public List<Node> child;
			public int nodeBegin, nodeEnd;
			private string key;
			private string value;
			public string Data
			{
				get => value;
				set => this.value = value.Trim('"');
			}
			public Node this[string key]
			{
				get
				{
					foreach (var node in this.child)
					{
						if (node.Key == key) return node;
					}
					return null;
				}
			}
			public override string ToString()
			{
				return "Key:" + this.Key + ",count:" + this.child.Count + ",value:" + this.Data;
			}
			public string Key { get => key; set => this.key = value.Trim('"'); }
			public Node(Node parent = null, int nowRank = 0)
			{
				this.nowRank = nowRank;
				this.parent = parent;
				child = new List<Node>();
			}
			private readonly int nowRank;
			public void Init(ref string info, int start, int length)
			{
				bool matchingString = false;
				nodeBegin = start;
				for (int i = start; i < length; i++)
				{
					if (matchingString)
					{
						if (info[i] == '"')//忽略所有引号内的内容
						{
							matchingString = false;
						}
						continue;
					}

					switch (info[i])
					{

						case '{':
							{
								//Debug.Print(new string(' ', nowRank)+"Child");
								var child = new Node(this, nowRank + 1);
								child.Init(ref info, i + 1, length);
								Extract(child);
								i = child.nodeEnd;
								break;
							}
						case '}':
							{
								nodeEnd = i;
								//Debug.Print(new string(' ', nowRank)+"EndNode");
								return;
							}
						case ':':
							{
								//Debug.Print(new string(' ', nowRank)+"NodeKey:" + info.Substring(nodeBegin, i - nodeBegin));
								Key = info.Substring(nodeBegin, i - nodeBegin);
								nodeBegin = i + 1;
								break;
							}
						case '"':
							{
								matchingString = true;
								break;
							}
						case ',':
							{
								//Debug.Print(new string(' ', nowRank)+"NodeValue:" + info.Substring(nodeBegin, i - nodeBegin));
								if (this.child.Count == 0) Data = info.Substring(nodeBegin, i - nodeBegin);
								next = new Node(parent, nowRank);
								next.Init(ref info, i + 1, length);
								nodeEnd = next.nodeEnd;
								return;
							}
					}
				}
				nodeEnd = length;
			}
			private void Extract(Node child)
			{
				Node node = child;
				do
				{
					this.child.Add(node);
					node = node.next;
				} while (node != null);
			}

			public IEnumerator<Node> GetEnumerator()
			{
				return ((IEnumerable<Node>)child).GetEnumerator();
			}


		}
	}
}
