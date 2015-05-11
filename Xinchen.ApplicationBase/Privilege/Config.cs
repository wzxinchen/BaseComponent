using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.ApplicationBase.Privilege
{
    public class Config
    {
        private static Dictionary<int, string> _privileges = new Dictionary<int, string>();
        ///<summary>
        /// 注册一个权限
        /// </summary>
        /// <param name="privilege">权限编号</param>
        /// <param name="description">权限描述</param>
        public static void RegisterPrivilege(int privilege,string description)
        {
            if (_privileges.ContainsKey(privilege))
            {
                return;
            }

            _privileges.Add(privilege, description);
        }
    }
}
