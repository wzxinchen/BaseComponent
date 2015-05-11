using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.ApplicationBase.Privilege
{
    public class PrivilegeChecker
    {
        BLL.Privilege _bllPrivilege = BLL.Privilege.GetInstance();
        public bool Check(int userSysNo,int privilege)
        {
            return false;
            _bllPrivilege.GetPrivilege(userSysNo);
        }
    }
}
