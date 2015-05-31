using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ext.Net.MVC;
namespace Xinchen.ApplicationBase.Mvc.Views
{
    public abstract class ViewPage : IView, IViewDataContainer
    {
        ViewContext _viewContext;
        ViewDataDictionary _viewData;
        protected HtmlHelper Html;
        public ViewDataDictionary ViewData
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
            Html = new HtmlHelper(viewContext, this);
            RenderView(viewContext, writer);
        }

        public abstract void RenderView(ViewContext viewContext, System.IO.TextWriter writer);
    }
}
