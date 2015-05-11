using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ModelBase;

namespace Xinchen.ExtNetBase
{
    public class YNComboBox : ComboBox
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Ext.Net.X.IsAjaxRequest)
            {
                Items.Add(new ListItem("是", true));
                Items.Add(new ListItem("否", false));
                ValueHiddenName = Name;
                ID = "combo" + Name;
                Name = string.Empty;
                SimpleSubmit = true;
                EmptyText = "请选择";
            }
        }
    }
}
