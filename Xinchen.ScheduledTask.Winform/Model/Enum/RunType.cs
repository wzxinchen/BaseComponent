using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xinchen.ScheduledTask.Winform.Model.Enum
{
    public enum RunType
    {
        /// <summary>
        /// 每隔多少时间执行
        /// </summary>
        [Description("隔多少秒运行")]
        Interval=1,
        /// <summary>
        /// 每天指定时间执行
        /// </summary>
        [Description("指定时间运行")]
        Timing=2
    }
}
