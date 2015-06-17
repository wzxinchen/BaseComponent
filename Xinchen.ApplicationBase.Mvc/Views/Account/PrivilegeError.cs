using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ext.Net.MVC;
namespace Xinchen.ApplicationBase.Mvc.Views.Account
{
    public class PrivilegeError : ViewPage
    {
        public override void RenderView(System.Web.Mvc.ViewContext viewContext, System.IO.TextWriter writer)
        {

            writer.Write("<!DOCTYPE html>");
            writer.Write("<head>");
            writer.Write("<title>用户登录</title>");
            var x = Html.X();
            writer.Write(Html.X().ResourceManager().ToHtmlString());
            writer.Write("<script type='text/javascript'>");
            writer.Write(@"Ext.Msg.alert('提示','权限不足',function(){var active=parent.Ext.WindowMgr.getActive();
            if (active)
                active.hide();
            else {
                parent.App.tabWork.remove(parent.App.tabWork.getActiveTab());
            }
        }");
            writer.Write("</script>");
            writer.Write("</head>");
        }
    }
}
