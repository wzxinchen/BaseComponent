using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Xinchen.ApplicationBase.Mvc.Views.Account
{
    public class LoginView : IView
    {
        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            writer.Write("呵呵哒");
        }
    }
}
