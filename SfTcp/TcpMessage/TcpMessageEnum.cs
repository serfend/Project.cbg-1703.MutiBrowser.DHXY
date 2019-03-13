using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfTcp.TcpMessage
{
	public static class TcpMessageEnum
	{
		/// <summary>
		/// 命令终端启动
		/// </summary>
		public const string CmdServerRun = "cmdStart";
		/// <summary>
		/// 命令终端以任务模式开始采集
		/// </summary>
		public const string CmdServerRunSchedule = "cmdRun";
		/// <summary>
		/// 设置终端别名
		/// </summary>
		public const string CmdSetClientName = "cmdSetClientName";
		/// <summary>
		/// 命令终端启动一个新的实例
		/// </summary>
		public const string CmdStartNewProgram = "cmdStartNewProgram";
		/// <summary>
		/// 命令终端结束自己
		/// </summary>
		public const string CmdSubClose = "cmdSubClose";
		/// <summary>
		/// 命令终端初始化同步设置
		/// </summary>
		public const string CmdSynInit = "cmdSynInit";
		/// <summary>
		/// 修改终端目标访问地址
		/// </summary>
		public const string CmdModefyTargetUrl = "cmdModefyTargetUrl";
		/// <summary>
		/// 进行心跳包
		/// </summary>
		public const string MsgHeartBeat = "msgHeartBeat";
		/// <summary>
		/// 同步文件列表信息
		/// </summary>
		public const string MsgSynFileList = "msgSynFileListMessage";
		/// <summary>
		/// 命令终端开始传输文件
		/// </summary>
		public const string CmdTransferFile = "cmdTransferFile";
		/// <summary>
		/// 同步登录凭证
		/// </summary>
		public const string MsgSynSession = "msgSynServerLoginSession";
		/// <summary>
		/// 终端更新别名的报告
		/// </summary>
		public const string RpNameModefied = "rpNameModefied";
		/// <summary>
		/// 命令终端重新拨号
		/// </summary>
		public const string CmdReRasdial = "cmdReRasdial";
		/// <summary>
		/// 终端重拨号完成的报告
		/// </summary>
		public const string RpReRasdial = "rpReRasdial";
		/// <summary>
		/// 终端连接完成的报告
		/// </summary>
		public const string RpClientConnect = "rpClientConnect";
		/// <summary>
		/// 终端已完成初始化的数据报告
		/// </summary>
		public const string RpInitCompleted = "rpInitComplete";
		/// <summary>
		/// 终端报告自身状态
		/// </summary>
		public const string RpStatus = "rpStatus";
		/// <summary>
		/// 终端报告新的可用订单
		/// </summary>
		public const string RpCheckBill = "rpCheckBill";
		/// <summary>
		/// （ScheduleTask回调）终端同步当前延时情况
		/// </summary>
		public const string RpClientWait="rpClientWait";
		/// <summary>
		/// 收到cmdServerRun后完成初始化的报告
		/// </summary>
		public const string RpClientRunReady = "rpServerRunReady";
		/// <summary>
		/// 订单已下单成功的回调
		/// </summary>
		public const string RpBillSubmited = "rpBillSubmited";
		/// <summary>
		/// 浏览器进程将自己Session同步到服务器
		/// </summary>
		public const string RpSessionSyn = "rpSessionSyn";
		/// <summary>
		/// 同步将军令信息到服务器
		/// </summary>
		public const string RpPayAuthKey = "rpPayAuthKey";
		/// <summary>
		/// vps终端处理商品详情发生异常
		/// </summary>
		public const string RpGoodDetailFail = "rpGoodDetailFail";
		/// <summary>
		/// 命令浏览器对指定链接进行操纵
		/// </summary>
		public const string CmdCheckBillUrl="cmdCheckBillUrl";
		/// <summary>
		/// 反馈本次通信无效的信息
		/// </summary>
		public static string RpMsgInvalid = "rpMsgInvalid";
	}
}
