using Ext.Net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Xinchen.DbUtils;
using Xinchen.ExtNetBase.Mvc;
using Xinchen.PrivilegeManagement;
using Ext.Net.MVC;
namespace Xinchen.ApplicationBase.Mvc
{
    public class BaseControllerInternal : Controller
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
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Success()
        {
            return Success("操作成功");
        }
        public ActionResult Success(string msg)
        {
            return Json(new
            {
                success = true,
                message = msg
            }, JsonRequestBehavior.AllowGet);
        }
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

        public ActionResult Info(string msg)
        {
            X.Msg.Info("提示", msg, AnchorPoint.Top).Show();
            return this.Direct();
        }
        protected bool HasPrivilege(int privilegeId)
        {
            return this.privilege.PrivilegeBase.CheckPrivilege(privilegeId);
        }
        public MemoryStream GetVerifyImage(string code, int width = 60, int height = 0x16, float fontSize = 14f)
        {
            Bitmap image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.White);
            Random random = new Random();
            for (int i = 0; i < 12; i++)
            {
                int num2 = random.Next(image.Width);
                int num3 = random.Next(image.Width);
                int num4 = random.Next(image.Height);
                int num5 = random.Next(image.Height);
                graphics.DrawLine(new Pen(Color.LightGray), num2, num4, num3, num5);
            }
            Font font = new Font("Arial", fontSize, FontStyle.Italic | FontStyle.Bold);
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.Gray, 1.2f, true);
            graphics.DrawString(code, font, brush, (float)0f, (float)0f);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Gif);
            image.Dispose();
            graphics.Dispose();
            return stream;
        }

        public IList<SqlFilter> ToFilters(FilterConditions e)
        {
            return FilterConverter.ConvertToFilters(e);
        }

        public Sort ToSort(DataSorter[] sorters)
        {
            return FilterConverter.ConvertToSorter(sorters);
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

        protected Privilege privilege;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            privilege = PrivilegeFactory.GetPrivilege();
        }
    }
}