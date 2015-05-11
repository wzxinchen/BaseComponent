using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xinchen.PrivilegeManagement.Enums
{
    [Flags]
    public enum BaseStatuses
    {
        [Description("有效")]
        Valid = 1,
        [Description("无效")]
        Invalid = 2
    }
}
