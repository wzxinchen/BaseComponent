using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ApplicationBase.Model;
using Xinchen.ExtNetBase;

namespace Xinchen.ApplicationBase.UI.Account
{
    public abstract class ChangePasswordPageBase : PageBase
    {
        private TextField txtOldPassword;
        private TextField txtNewPassword;
        private TextField txtNewPassword2;
        private FormPanel formPanel;
        private Button btnAccept;
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            txtOldPassword = new TextField();
            txtOldPassword.Name = "OldPassword";
            txtOldPassword.ID = "txtOldPassword";
            txtOldPassword.FieldLabel = "旧密码";
            txtOldPassword.InputType = InputType.Password;
            txtNewPassword = new TextField()
            {
                Name = "NewPassword",
                ID = "txtNewPassword",
                InputType = InputType.Password,
                FieldLabel = "新密码"
            };
            txtNewPassword2 = new TextField()
            {
                Name = "NewPassword2",
                ID = "txtNewPassword2",
                InputType = InputType.Password,
                FieldLabel = "确认密码"
            };
            formPanel = new FormPanel();
            formPanel.BodyPadding = 20;
            formPanel.Add(txtOldPassword);
            formPanel.Add(txtNewPassword);
            formPanel.Add(txtNewPassword2);
            Controls.Add(formPanel);

            btnAccept = new Button()
            {
                Icon = Icon.Accept,
                Text = "修改",
                ID = "btnAccept"
            };
            btnAccept.DirectClick += btnAccept_DirectClick;
            btnAccept.DirectEvents.Click.EventMask.Set("正在修改");

            formPanel.Buttons.Add(btnAccept);
        }

        void btnAccept_DirectClick(object sender, DirectEventArgs e)
        {
            var mr = GetModelFromPost<ChangePasswordModel>();
            if (mr.Success)
            {
                if (CatchException(() =>
                {
                    privilege.ChangePassword(mr.Model.OldPassword, mr.Model.NewPassword, mr.Model.NewPassword2);
                }))
                {
                    AlertCloseWindow("修改成功");
                }
            }
            else
            {
                Alert(mr.Message);
            }
        }

        protected abstract override int Privilege { get; }
    }
}
