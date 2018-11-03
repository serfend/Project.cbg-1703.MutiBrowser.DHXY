namespace EquipSettingLoader
{
	partial class Form1
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.CmdLoadEquipSetting = new System.Windows.Forms.Button();
			this.CmdLoadLog = new System.Windows.Forms.Button();
			this.OpShowLog = new System.Windows.Forms.TextBox();
			this.cmdEnableIpDetect = new System.Windows.Forms.Button();
			this.CmdCheckIpValue = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// CmdLoadEquipSetting
			// 
			this.CmdLoadEquipSetting.Location = new System.Drawing.Point(12, 12);
			this.CmdLoadEquipSetting.Name = "CmdLoadEquipSetting";
			this.CmdLoadEquipSetting.Size = new System.Drawing.Size(139, 43);
			this.CmdLoadEquipSetting.TabIndex = 0;
			this.CmdLoadEquipSetting.Text = "装备匹配表";
			this.CmdLoadEquipSetting.UseVisualStyleBackColor = true;
			this.CmdLoadEquipSetting.Click += new System.EventHandler(this.CmdLoadEquipSetting_Click);
			// 
			// CmdLoadLog
			// 
			this.CmdLoadLog.Location = new System.Drawing.Point(12, 61);
			this.CmdLoadLog.Name = "CmdLoadLog";
			this.CmdLoadLog.Size = new System.Drawing.Size(139, 43);
			this.CmdLoadLog.TabIndex = 1;
			this.CmdLoadLog.Text = "读取日志";
			this.CmdLoadLog.UseVisualStyleBackColor = true;
			this.CmdLoadLog.Click += new System.EventHandler(this.CmdLoadLog_Click);
			// 
			// OpShowLog
			// 
			this.OpShowLog.Location = new System.Drawing.Point(156, 12);
			this.OpShowLog.Multiline = true;
			this.OpShowLog.Name = "OpShowLog";
			this.OpShowLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.OpShowLog.Size = new System.Drawing.Size(649, 574);
			this.OpShowLog.TabIndex = 2;
			this.OpShowLog.TextChanged += new System.EventHandler(this.OpShowLog_TextChanged);
			this.OpShowLog.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OpShowLog_KeyPress);
			this.OpShowLog.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OpShowLog_MouseDown);
			// 
			// cmdEnableIpDetect
			// 
			this.cmdEnableIpDetect.Location = new System.Drawing.Point(11, 539);
			this.cmdEnableIpDetect.Name = "cmdEnableIpDetect";
			this.cmdEnableIpDetect.Size = new System.Drawing.Size(139, 43);
			this.cmdEnableIpDetect.TabIndex = 1;
			this.cmdEnableIpDetect.Text = "去除重复ip";
			this.cmdEnableIpDetect.UseVisualStyleBackColor = true;
			this.cmdEnableIpDetect.Click += new System.EventHandler(this.CmdEnableIpDetect);
			// 
			// CmdCheckIpValue
			// 
			this.CmdCheckIpValue.Location = new System.Drawing.Point(12, 490);
			this.CmdCheckIpValue.Name = "CmdCheckIpValue";
			this.CmdCheckIpValue.Size = new System.Drawing.Size(139, 43);
			this.CmdCheckIpValue.TabIndex = 1;
			this.CmdCheckIpValue.Text = "检查使用代理";
			this.CmdCheckIpValue.UseVisualStyleBackColor = true;
			this.CmdCheckIpValue.Click += new System.EventHandler(this.CmdCheckIpValue_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(817, 594);
			this.Controls.Add(this.OpShowLog);
			this.Controls.Add(this.CmdCheckIpValue);
			this.Controls.Add(this.cmdEnableIpDetect);
			this.Controls.Add(this.CmdLoadLog);
			this.Controls.Add(this.CmdLoadEquipSetting);
			this.Name = "Form1";
			this.Text = "设置加载";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button CmdLoadEquipSetting;
		private System.Windows.Forms.Button CmdLoadLog;
		private System.Windows.Forms.TextBox OpShowLog;
		private System.Windows.Forms.Button cmdEnableIpDetect;
		private System.Windows.Forms.Button CmdCheckIpValue;
	}
}

