using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ext.Net.MVC;
using Xinchen.PrivilegeManagement.ViewModel;
using System.Web;
using System.IO;
namespace Xinchen.ApplicationBase.Mvc.Views
{
    public abstract class ViewPage : IView, IViewDataContainer
    {
        protected UserSessionModel UserInfo { get; private set; }
        ViewContext _viewContext;
        ViewDataDictionary _viewData;
        protected HtmlHelper Html;
        protected UrlHelper Url;
        protected string HideWindowReloadGrid(string gridId)
        {
            return "parent.Ext.WindowMgr.getActive().hide();parent.App." + gridId + ".getStore().reload();";
        }
        protected Privilege Privilege
        {
            get
            {
                return PrivilegeFactory.GetPrivilege();
            }
        }

        protected void RenderMain(TextWriter writer)
        {
            writer.Write("<!DOCTYPE html>");
            writer.Write("<head>");
            writer.Write("<title>后台管理中心 - " + AppConfig.WebTitle + "</title>");
            writer.Write("</head><body></body>");
            writer.Write(Html.X().ResourceManager().ToHtmlString());
        }

        public virtual ViewDataDictionary ViewData
        {
            get
            {
                if (_viewData == null)
                {
                    _viewData = new ViewDataDictionary();
                }
                return _viewData;
            }
            set
            {
                _viewData = value;
            }
        }
        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            _viewContext = viewContext;
            ViewData = _viewContext.ViewData;
            UserInfo = ((UserSessionModel)HttpContext.Current.Session["userInfo"]);
            Html = new HtmlHelper(viewContext, this);
            Url = new UrlHelper(viewContext.RequestContext);
            RenderView(viewContext, writer);
        }

        public abstract void RenderView(ViewContext viewContext, System.IO.TextWriter writer);
    }
}
