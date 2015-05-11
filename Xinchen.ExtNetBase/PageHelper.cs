namespace Xinchen.ExtNetBase
{
    using Ext.Net;
    using System;

    public class PageHelper
    {
        public void Alert(string msg)
        {
            X.Msg.Alert("提示", msg);
        }

        public void AlertCloseWindow(string msg, string handler)
        {
            X.Msg.Alert("提示", msg, "parent.Ext.WindowMgr.getActive().hide();" + handler).Show();
        }

        public void AlertCloseWindowRefreshGrid(string msg, string gridID)
        {
            this.AlertCloseWindow(msg, "parent.Ext.getCmp('" + gridID + "').getStore().load();");
        }
    }
}

