using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net;
using Xinchen.ExtNetBase;
using Xinchen.ExtNetBase.Mvc;
namespace Xinchen.ApplicationBase.Mvc.Views.Account
{
    public class Login : ViewPage
    {
        public override void RenderView(ViewContext viewContext, System.IO.TextWriter writer)
        {
            writer.Write("<!DOCTYPE html>");
            writer.Write("<head>");
            writer.Write("<title>用户登录</title>");
            var x = Html.X();
            writer.Write(Html.X().ResourceManager().ToHtmlString());
            var loginWindow = new Window()
            {
                Title = "用户登录",
                Width = 300,
                Height = 200,
                Closable = false,
                Modal = true,
                Icon = Icon.Key,
                Draggable = false,
                BodyPadding = 10,
                Resizable = false
            };
            var formPanel = new ValidatedForm()
            {
                Border = true,
                BodyPadding = 10,
                ID = "loginForm",
                Url = Url.Action("login")
            };
            formPanel.Layout = "table";
            formPanel.LayoutConfig.Add(new TableLayoutConfig()
            {
                Columns = 2
            });
            loginWindow.Add(formPanel);
            var textField = new TextField()
            {
                FieldLabel = "用户名",
                Name = "Username",
                ColSpan = 2,
                AllowBlank = false,
                BlankText = "用户名不能为空"
            };
            var passwordField = new TextField()
            {
                FieldLabel = "密码",
                Name = "Password",
                ColSpan = 2,
                AllowBlank = false,
                InputType = Ext.Net.InputType.Password,
                BlankText = "密码不能为空"
            };
            var verifyImg = new Image()
            {
                Width = 100,
                ImageUrl = Url.Action("VerifyImage"),
                Height = 22,
                ColSpan = 1,
                ID = "imgVerify"
            };
            var verifyCodeField = new TextField()
            {
                FieldLabel = "验证码",
                ColSpan = 1,
                AllowBlank = false,
                BlankText = "验证码不能为空'",
                Name="VerifyCode"
            };
            formPanel.Add(textField);
            formPanel.Add(passwordField);
            formPanel.Add(verifyCodeField);
            formPanel.Add(verifyImg);
            var btnLogin = new Button()
            {
                Text = "登录",
                Icon = Icon.KeyStart,

            };
            var btnChangeImage = new Button()
            {
                Text = "更换验证码",
                Icon = Icon.ImageEdit,
                ID = "btnChangeImage"
            };
            btnChangeImage.Handler = "var src='" + verifyImg.ImageUrl + "';src=src+'?'+new Date().valueOf();App.imgVerify.setImageUrl(src);";
            btnLogin.Handler = @"App.loginForm.submitData(function(r){if(r.success){location.href='" + @Url.Action("index", "home") + "';}else{App.btnChangeImage.handler();}});";
            loginWindow.Buttons.Add(btnChangeImage);
            loginWindow.Buttons.Add(btnLogin);
            writer.Write(loginWindow.ToBuilder().ToHtmlString());
            writer.Write("</head>");

        }
    }
}
