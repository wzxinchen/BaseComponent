using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ExtNetBase;
using Xinchen.PrivilegeManagement;

namespace Xinchen.ApplicationBase.UI.Role
{
    public class PrivilegeFilter : ForeignFilterBase
    {
        public override object GetData()
        {
            var privilege = new Privilege(new PrivilegeBase());
            return privilege.GetPrivileges().Select(x => new { x.Id, Name = x.Description });
        }
    }
}
