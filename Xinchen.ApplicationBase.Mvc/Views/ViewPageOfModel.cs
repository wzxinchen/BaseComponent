using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Xinchen.ApplicationBase.Mvc.Views
{
    public abstract class ViewPage<TModel>:ViewPage
    {
        public TModel Model
        {
            get
            {
                return (TModel)ViewData.Model;
            }
        }
        public abstract override void RenderView(System.Web.Mvc.ViewContext viewContext, System.IO.TextWriter writer);
    }
}
