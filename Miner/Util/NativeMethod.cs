
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;

//namespace Miner.Util
//{
//	internal class SystemTimeWin32
//	{
//		[DllImport("Kernel32.dll", CharSet = CharSet.Ansi)]
//		public static extern bool SetSystemTime(ref Systemtime sysTime);
//		[DllImport("Kernel32.dll")]
//		public static extern bool SetLocalTime(ref Systemtime sysTime);
//		[DllImport("Kernel32.dll")]
//		public static extern void GetSystemTime(ref Systemtime sysTime);
//		[DllImport("Kernel32.dll")]
//		public static extern void GetLocalTime(ref Systemtime sysTime);

//		/// <summary>
//		/// 时间结构体
//		/// </summary>
//		[StructLayout(LayoutKind.Sequential)]
//		public struct Systemtime
//		{
//			public ushort wYear;
//			public ushort wMonth;
//			public ushort wDayOfWeek;
//			public ushort wDay;
//			public ushort wHour;
//			public ushort wMinute;
//			public ushort wSecond;
//			public ushort wMiliseconds;
//		}

//		public static Systemtime FromStamp(long stamp)
//		{
//			var date = DateTime.FromFileTime(stamp);
//			return new Systemtime() {
//				wYear=(ushort)date.Year,
//				wMonth= (ushort)date.Month,
//				wDay= (ushort)date.Day,
//				wDayOfWeek= (ushort)date.DayOfWeek,
//				wHour= (ushort)date.Hour,
//				wMinute= (ushort)date.Minute,
//				wSecond= (ushort)date.Second,
//				wMiliseconds= (ushort)date.Millisecond
//			};
//		}
//	}
//}