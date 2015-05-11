using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Xinchen.ExtNetBase.Buttons;
using Xinchen.ModelBase;

namespace Xinchen.ApplicationBase.UI.Account
{
    public abstract class RegisterAdminPageBase : PageBase
    {
        private HtmlForm regForm;
        private Window window;
        private FormPanel formPanel;
        private Button btnReg;
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            foreach (Control control in Controls)
            {
                if (regForm == null)
                {
                    regForm = control as HtmlForm;
                    if (regForm != null) break;
                }
            }
            if (regForm == null)
            {
                throw new ArgumentException("登录页面中至少添加一个服务器端表单");
            }
            window = new Window();
            window.Title = "管理员注册";
            window.Closable = false;
            window.Icon = Icon.Key;
            window.Width = 320;
            window.Height = 185;
            formPanel = new FormPanel();
            formPanel.BodyStyle = "padding:20px;";
            formPanel.Layout = "table";
            formPanel.LayoutConfig.Add(new TableLayoutConfig()
            {
                Columns = 2
            });
            formPanel.Items.Add(new TextField()
            {
                FieldLabel = "用户名",
                AllowBlank = false,
                ColSpan = 2,
                Name = "Username"
            });
            formPanel.Items.Add(new TextField()
            {
                FieldLabel = "密码",
                AllowBlank = false,
                ColSpan = 2,
                InputType = Ext.Net.InputType.Password,
                Name = "Password"
            });
            formPanel.Items.Add(new TextField()
            {
                FieldLabel = "确认密码",
                AllowBlank = false,
                ColSpan = 1,
                InputType = Ext.Net.InputType.Password,
                Name = "Password2"
            });
            window.Items.Add(formPanel);
            regForm.Controls.Add(window);
            btnReg = new KeyAddButton();
            btnReg.Text = "注册";
            btnReg.ID = "btnReg";
            btnReg.OnClientClick = "App.direct.Reg({eventMask:{showMask:true,msg:'正在注册'}});";
            window.Buttons.Add(btnReg);
        }

        [DirectMethod]
        public void Reg()
        {
            var mr = GetModelFromPost<RegAdminModel>();
            if (!mr.Success)
            {
                Alert(mr.Message);
                return;
            }
            try
            {
                privilege.RegAdmin(mr.Model.Username, mr.Model.Password, mr.Model.Password2);
                Alert("注册成功，下面准备登录",privilege.PrivilegeBase.PrivilegeContextProvider.LoginPage);
            }
            catch (ApplicationException ae)
            {
                Alert(ae.Message);
            }
        }

        protected abstract override int Privilege { get; }
    }
}
