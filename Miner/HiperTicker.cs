using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;

namespace Win32
{
	public class HiperTicker
	{
		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceCounter(
			out long lpPerformanceCount);

		[DllImport("Kernel32.dll")]
		private static extern bool QueryPerformanceFrequency(
			out long lpFrequency);

		private long startTime, stopTime;
		private long freq;

		// 构造函数 
		public HiperTicker()
		{
			startTime = 0;
			stopTime = 0;

			if (QueryPerformanceFrequency(out freq) == false)
			{
				// 不支持高性能计数器 
				throw new Win32Exception();
			}
		}

		// 开始计时器 
		public void Record()
		{
			// 来让等待线程工作 
			Thread.Sleep(0);

			QueryPerformanceCounter(out startTime);
		}



		// 返回计时器经过时间
		public int Duration
		{
			get
			{
				QueryPerformanceCounter(out stopTime);
				double result= ((double)(stopTime - startTime) / (double)freq)*1000000;
				return (int)result;
			}
		}
	}
}