using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.ApplicationBase.Model
{
    public class UserInfo
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        public Dictionary<int, Role> Roles { get; set; }
    }
}
