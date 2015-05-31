using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ext.Net.MVC;

namespace Xinchen.ApplicationBase.Mvc
{
    public class ExtNetBaseController : MvcBaseController
    {
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