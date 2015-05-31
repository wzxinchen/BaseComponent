using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Xinchen.ApplicationBase.Mvc
{
    public class MvcBaseController : BaseController
    {
        public ActionResult Error()
        {
            List<string> msgs = new List<string>();
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    msgs.Add(error.ErrorMessage);
                }
            }
            return Error(string.Join("<br />", msgs));
        }

        public ActionResult Error(string msg)
        {
            return Json(new
            {
                success = false,
                message = msg
            });
        }

        internal ActionResult Success()
        {
            return Success("操作成功");
        }

        public ActionResult CatchExceptoin(Action action, Func<bool, string, ActionResult> onComplete = null)
        {
            try
            {
                action();
                if (onComplete != null)
                {
                    return onComplete(true, null);
                }
                return Success();
            }
            catch (ApplicationException ae)
            {
                if (onComplete != null)
                {
                    return onComplete(false, ae.Message);
                }
                return Error(ae.Message);
            }
        }

        internal ActionResult Success(string msg)
        {
            return Json(new
            {
                success = true,
                message = msg
            },JsonRequestBehavior.AllowGet);
        }
    }
}