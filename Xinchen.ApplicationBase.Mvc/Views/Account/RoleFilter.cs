using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ExtNetBase.Mvc;
using Xinchen.PrivilegeManagement;

namespace Xinchen.ApplicationBase.Mvc.UI.User
{
    public class RoleFilter : ForeignFilterBase
    {
        public override object GetData()
        {
            var privilege = PrivilegeFactory.GetPrivilege();
            return privilege.GetRoles();
        }
    }
}
