using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xinchen.ScheduledTask.Winform.Model.Enum
{
    public enum Status
    {
        /// <summary>
        /// 正在运行
        /// </summary>
        [Description("正在运行")]
        Runing = 1,
        /// <summary>
        /// 已暂停
        /// </summary>
        [Description("已暂停")]
        Pause = 2,
        /// <summary>
        /// 已停止
        /// </summary>
        [Description("已停止")]
        Stop = 3,
        /// <summary>
        /// 运行出错
        /// </summary>
        [Description("运行出错")]
        Error = 4,
        /// <summary>
        /// 等待运行
        /// </summary>
        [Description("等待运行")]
        WaitRun = 5
    }
}
