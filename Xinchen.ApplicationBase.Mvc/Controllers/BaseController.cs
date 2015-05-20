using Ext.Net;
using Ext.Net.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Xinchen.ApplicationBase.Mvc.Controllers
{
    public class BaseController:Controller
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
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExtError()
        {
            List<string> msgs = new List<string>();
            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    msgs.Add(error.ErrorMessage);
                }
            }
            return ExtError(string.Join("<br />", msgs));
        }

        public ActionResult ExtError(string msg)
        {
            X.Msg.Alert("错误", msg).Show();
            return this.Direct();
        }

        //internal ActionResult Success()
        //{
        //    return Success("操作成功");
        //}
        public ActionResult Alert(string msg)
        {
            X.Msg.Alert("提示", msg).Show();
            return this.Direct();
        }

        public ActionResult AlertCloseWindowRefreshParentGrid(string msg, string windowId, string gridId)
        {
            X.Msg.Alert("提示", msg, new JFunction()
            {
                Handler = "App." + windowId + ".close();parent.App." + gridId + ".getStore().reload();"
            }).Show();
            return this.Direct();
        }
        public ActionResult CloseWindowRefreshParentGrid(string windowId, string gridId)
        {
            X.AddScript("parent.App." + windowId + ".close();parent.App." + gridId + ".getStore().reload();");
            return this.Direct();
        }
    }
}
