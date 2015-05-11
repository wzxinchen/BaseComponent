using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ExtNetBase;
using Xinchen.PrivilegeManagement;

namespace Xinchen.ApplicationBase.UI.User
{
    public class RoleFilter : ForeignFilterBase
    {
        public override object GetData()
        {
            var privilege = new Privilege(new PrivilegeBase());
            return privilege.GetRoles();
        }
    }
}
