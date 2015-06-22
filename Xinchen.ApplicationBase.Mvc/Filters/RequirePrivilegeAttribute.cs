using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Xinchen.ApplicationBase.Mvc.Filters
{
    public class RequirePrivilegeAttribute : AuthorizeAttribute
    {
        public int Privilege { get; set; }
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var privilege = PrivilegeFactory.GetPrivilege();
            privilege.PrivilegeBase.Setup();
            privilege.PrivilegeBase.CheckLoginStatus();
            return privilege.PrivilegeBase.CheckPrivilege(Privilege);
            //if (!privilege.PrivilegeBase.CheckPrivilege(Privilege))
            //{
            //    base.Response.Redirect("~/privilegeerror.aspx", true);
            //}
            //return base.AuthorizeCore(httpContext);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/account/privilegeError");
        }
    }
}
