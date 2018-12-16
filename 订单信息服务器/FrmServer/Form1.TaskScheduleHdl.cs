using DotNet4.Utilities.UtilCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 订单信息服务器
{
	public partial class Form1 : Form
	{
		/// <summary>
		/// 用于vps空闲等待队列
		/// </summary>
		private Queue<string> hdlVpsTaskScheduleQueue=new Queue<string>();
		/// <summary>
		/// vps是否可用，以ip为键
		/// </summary>
		private Dictionary<string, bool> AvailableVps = new Dictionary<string, bool>();
		private int _taskSchedule_Interval = 500;
		private int _taskSchedule_DefaultInQueueCount = 99;
		private Thread _threadTaskSchedule;
		public void StartTaskSchedule(bool start = true)
		{
			if (start)
			{
				if (_taskSchedule_Start) return;
				_taskSchedule_Start = true;
				_taskSchedule_threadStart = true;
				_threadTaskSchedule=new Thread(() =>
				{
					ThreadHdlVpsEndPoint();
				})
				{ IsBackground=true};
				_threadTaskSchedule.Start();
			}
			else
			{
				_taskSchedule_threadStart = false;
			}
		}
		private bool _taskSchedule_Start = false;
		private bool _taskSchedule_threadStart = false;
		/// <summary>
		/// 每隔固定时间，将队列第一个分配任务
		/// </summary>
		private void ThreadHdlVpsEndPoint()
		{
			while (_taskSchedule_threadStart)
			{
				try
				{
					_taskSchedule_Interval = Convert.ToInt32(IpTaskInterval.Text);
				}
				catch (Exception)
				{
					_taskSchedule_Interval = 500;
				}
				int hdlCount = 1;
				int beginTime = Environment.TickCount;
				if (_taskAllocatePause)
				{
					Thread.Sleep(200);
					continue;
				}
				for (int index = 1; index <= _taskSchedule_DefaultInQueueCount; index++)
				{
					if (hdlVpsTaskScheduleQueue.Count == 0) break;
						var vps = hdlVpsTaskScheduleQueue.Dequeue();
					if (vps != null)
					{
						var client = serverManager[vps];
						if (client != null)
						{
							int timeSpend = Environment.TickCount - beginTime;
							client.Send($"<newScheduleServerRun><taskStamp>{_taskSchedule_Interval * index-timeSpend}</taskStamp>");
							hdlCount++;
						}
					}
				}
				Thread.Sleep(_taskSchedule_Interval*(hdlCount));
			}
			_taskSchedule_Start = false;
		}
	}
}
