using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.ExtNetBase.Buttons
{
    public class KeyAddButton:Button
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Icon = Ext.Net.Icon.KeyAdd;
        }
    }
}
