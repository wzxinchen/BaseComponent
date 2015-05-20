using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xinchen.ApplicationBase.Mvc.Filters;
using Xinchen.ApplicationBase.Mvc.Views.Account;

namespace Xinchen.ApplicationBase.Mvc.Controllers
{
    public class AccountController:BaseController
    {
        [NotRequireLogin]
        public ActionResult Login()
        {
            return View(new LoginView());
        }
    }
}
