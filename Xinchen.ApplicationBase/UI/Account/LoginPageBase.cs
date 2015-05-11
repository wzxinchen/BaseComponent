using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Xinchen.ApplicationBase.UI;
using Xinchen.ExtNetBase.Buttons;
using Xinchen.ModelBase;

namespace Xinchen.ApplicationUI.Account
{
    public abstract class LoginPageBase : PageBase
    {
        Ext.Net.Image imgVerify = null;
        HtmlForm loginForm = null;
        private Window window;
        private FormPanel formPanel;
        private Ext.Net.Button btnLogin;
        private Ext.Net.Button btnChangeImage;
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            foreach (Control control in Controls)
            {
                if (loginForm == null)
                {
                    loginForm = control as HtmlForm;
                    if (loginForm != null) break;
                }
            }
            if (loginForm == null)
            {
                throw new ArgumentException("登录页面中至少添加一个服务器端表单");
            }
            window = new Window();
            window.Title = "用户登录";
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
                FieldLabel = "验证码",
                AllowBlank = false,
                ColSpan = 1,
                InputType = Ext.Net.InputType.Text,
                Name = "VerifyCode"
            });
            imgVerify = new Ext.Net.Image()
            {
                Width = 100,
                ImageUrl = "login.aspx?action=VerifyImage",
                Height = 22,
                ColSpan = 1,
                ID = "imgVerify"
            };
            formPanel.Items.Add(imgVerify);
            window.Items.Add(formPanel);
            loginForm.Controls.Add(window);
            btnChangeImage = new Ext.Net.Button();
            btnChangeImage.Text = "更换验证码";
            btnChangeImage.OnClientClick = "App.direct.ChangeImage();";
            window.Buttons.Add(btnChangeImage);
            btnLogin = new KeyGoButton();
            btnLogin.Text = "登录";
            btnLogin.ID = "btnLogin";
            btnLogin.OnClientClick = "App.direct.Login();";
            window.Buttons.Add(btnLogin);
        }
        protected void Page_Load(EventArgs e)
        {

        }

        [DirectMethod]
        public void ChangeImage()
        {
            ChangeVerifyImage(imgVerify);
        }

        [DirectMethod]
        public void Login()
        {
            var mr = GetModelFromPost<LoginModel>();
            if (mr.Success)
            {
                try
                {
                    privilege.PrivilegeBase.Login(mr.Model.Username, mr.Model.Password);
                    Response.Redirect("~/index.aspx", true);
                }
                catch (ApplicationException ae)
                {
                    Alert(ae.Message);
                }
            }
            else
            {
                Alert(mr.Message);
            }
            ChangeVerifyImage(imgVerify);
        }

        protected abstract override int Privilege { get; }
    }
}
