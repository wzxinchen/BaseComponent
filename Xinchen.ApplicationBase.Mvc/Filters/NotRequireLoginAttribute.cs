using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Xinchen.ApplicationBase.Mvc.Filters
{
    public class NotRequireLoginAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return httpContext.Session["AccountId"] == null || string.IsNullOrWhiteSpace(httpContext.Session["AccountId"].ToString());
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/");
        }
    }
}
