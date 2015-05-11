using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Xinchen.WebBase
{
    public class AjaxHttpModule : IHttpModule
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += context_PreSendRequestHeaders;
        }

        void context_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            HttpContext context = application.Context;
            if ((context.Response.StatusCode == 0x12e) && (context.Request.Headers.AllKeys.Contains("X-Requested-With")))
            {
                string redirectLocation = context.Response.RedirectLocation;
                context.Response.ClearContent();
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/html";
                context.Response.Charset = "utf-8";
                context.Response.Headers.Remove("location");
                context.Response.Output.Write("{\"script\":\"window.location.href='" + redirectLocation + "';\"}");
            }
        }
    }
}
