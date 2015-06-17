using Ext.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Xinchen.ExtNetBase
{
    [ToolboxData("<{0}:ComboBoxBase runat=server></{0}:ComboBoxBase>")]
    public class ComboBoxBase : ComboBox
    {
        private Store _store;

        public object DataSource
        {
            get
            {
                return _store.DataSource;
            }
            set
            {
                _store.DataSource = value;
            }
        }

        public void BindData()
        {
            _store.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _store = new Store();
            _store.Model.Add(ComponentHelper.GetModel(new Dictionary<string, ModelFieldType>() { 
            {"Id",ModelFieldType.Int},{"Name",ModelFieldType.String}
            }));
            if (!Ext.Net.X.IsAjaxRequest)
            {
                DisplayField = "Name";
                ValueField = "Id";
                SimpleSubmit = true;
            }
            Store.Add(_store);
        }
    }
}
