using DotNet4.Utilities.UtilCode;
using SfTcp.TcpMessage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 订单信息服务器
{
	public partial class Form1 
	{
		/// <summary>
		/// 用于vps空闲等待队列
		/// </summary>
		private ConcurrentQueue<string> hdlVpsTaskScheduleQueue=new ConcurrentQueue<string>();
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
				int hdlCount = 0;
				
				if (_taskAllocatePause)
				{
					Thread.Sleep(200);
					continue;
				}
				var ticker = new Win32.HiperTicker();
				ticker.Record();
				for (int index = 1; index <= _taskSchedule_DefaultInQueueCount; index++)
				{
					string vps = string.Empty;
					if (hdlVpsTaskScheduleQueue.Count == 0) break;
					if (!hdlVpsTaskScheduleQueue.TryDequeue(out vps)) { continue; }; 
					if (vps != null)
					{
						var client = server[vps];
						if (client != null)
						{
							int taskDelayTime = _taskSchedule_Interval * index -(int)( ticker.Duration/1000);
							_dicVpsWorkBeginTime[client.Ip].RecordBegin(taskDelayTime);
							client.Send(new CmdServerRunScheduleMessage(taskDelayTime));
							hdlCount++;
						}
					}
				}
				hdlCount = hdlCount == 0 ? 1 : hdlCount;
				Thread.Sleep(_taskSchedule_Interval*(hdlCount));
				//Thread.Sleep(500);
			}
			_taskSchedule_Start = false;
		}
	}
}
