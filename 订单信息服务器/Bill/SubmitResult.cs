using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace 订单信息服务器.Bill
{
	public class ErrorMap
	{
	}

	public class SubmitResult
	{
		/// <summary>
		/// 
		/// </summary>
		public string result { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string errorMsg { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string errorCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ErrorMap errorMap { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string bankUrl { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string data { get; set; }

	}
}
