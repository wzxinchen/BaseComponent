using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ext.Net.MVC;
namespace Xinchen.ApplicationBase.Mvc.Views.Account
{
    public class Login : ViewPage
    {
        public override void RenderView(ViewContext viewContext, System.IO.TextWriter writer)
        {
            HtmlHelper html = new HtmlHelper(viewContext, this);
            writer.Write("<!DOCTYPE html>");
            writer.Write("<head>");
            writer.Write("<title>用户登录</title>");
            writer.Write(html.X().ResourceManager().ToHtmlString());
            writer.Write("</head>");

        }
    }
}
