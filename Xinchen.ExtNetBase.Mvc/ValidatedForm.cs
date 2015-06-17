using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.ExtNetBase.Mvc
{
    public class ValidatedForm:FormPanel
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AddAfterClientInitScript("App." + ID + @".submitData=function(callBack){
        this.submit({
    success: function(form, action) {
        callBack(action.result);
    },
    failure: function(form, action) {
        switch (action.failureType) {
            case Ext.form.action.Action.CONNECT_FAILURE:
Ext.Msg.info({ui: 'error', title: '错误', html: '连接失败', iconCls: '#Information',queue:'top'});
                break;
            case Ext.form.action.Action.SERVER_INVALID:
Ext.Msg.info({ui: 'error', title: '错误', html: action.result.message, iconCls: '#Information',queue :'top'});
            if(callBack)callBack(action.result);
       }
    }
});
            };");
        }
    }
}
