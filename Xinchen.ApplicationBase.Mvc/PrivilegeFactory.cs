using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.ApplicationBase.Mvc
{
    public class PrivilegeFactory
    {
        public static Privilege GetPrivilege()
        {
            var obj = CallContext.GetData("privilege");
            if(obj==null)
            {
                obj = new Privilege(new PrivilegeManagement.PrivilegeBase());
                CallContext.SetData("privilege", obj);
            }
            return (Privilege)obj;
        }
    }
}
