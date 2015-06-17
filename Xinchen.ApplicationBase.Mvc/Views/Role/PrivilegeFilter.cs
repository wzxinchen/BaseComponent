using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ExtNetBase;
using Xinchen.ExtNetBase.Mvc;
using Xinchen.PrivilegeManagement;

namespace Xinchen.ApplicationBase.Mvc.Views.Role
{
    public class PrivilegeFilter : ForeignFilterBase
    {
        public override object GetData()
        {
            var privilege = PrivilegeFactory.GetPrivilege();
            return privilege.GetPrivileges().Select(x => new { x.Id, Name = x.Description });
        }
    }
}
