using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ApplicationBase.Model;
using Xinchen.ExtNetBase;
using Xinchen.ExtNetBase.TreePanelEx;
using Xinchen.PrivilegeManagement.Enums;
using Xinchen.Utils;

namespace Xinchen.ApplicationBase.UI.User
{
    public abstract class EditPageBase:PageBase
    {
        private FormPanel _formPanel;
        private TextField _txtUsername;
        private ComboBox _comboStatus;
        private Store _comboStatusStore;
        private TextArea _txtMemo;
        private TreePanelNodeMover _treeRoleSelector;
        private Button _btnSave;
        private int id;
        private PrivilegeManagement.DTO.User user;
        private Hidden _hidId;
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            _formPanel = new FormPanel();
            _formPanel.BodyPadding = 10;
            _formPanel.Layout = "table";
            _formPanel.LayoutConfig.Add(new TableLayoutConfig()
            {
                Columns = 2
            });
            _formPanel.ID = "addForm";
            _formPanel.DefaultAnchor = "100%";
            _txtUsername = new TextField();
            _txtUsername.ID = "txtUsername";
            _txtUsername.Name = "Username";
            _txtUsername.ColSpan = 1;
            _txtUsername.Width = 300;
            _txtUsername.AllowBlank = false;
            _txtUsername.FieldLabel = "用户名";
            _txtUsername.EmptyText = "输入用户名";
            _formPanel.Add(_txtUsername);
            _comboStatus = new ComboBox();
            _comboStatus.EmptyText = "选择状态";
            _comboStatus.ValueHiddenName = "Status";
            _comboStatus.SimpleSubmit = true;
            _comboStatusStore = new Store();
            _comboStatusStore.ID = "comboStoreStatus";
            _comboStatusStore.Model.Add(ComponentHelper.GetModel(
                new Dictionary<string, ModelFieldType>(){
            {"Id",ModelFieldType.Int},{"Name",ModelFieldType.String}
            }));
            _comboStatus.DisplayField = "Name";
            _comboStatus.ValueField = "Id";
            _comboStatus.Store.Add(_comboStatusStore);
            _comboStatus.FieldLabel = "状态";
            _comboStatus.Editable = false;
            _formPanel.Add(_comboStatus);
            _txtMemo = new TextArea();
            _txtMemo.Width = 750;
            _txtMemo.Name = "Description";
            _txtMemo.ColSpan = 2;
            _txtMemo.EmptyText = "简单描述一下用户的职责";
            _txtMemo.FieldLabel = "描述";
            _txtMemo.Height = 70;
            _formPanel.Add(_txtMemo);
            _treeRoleSelector = new TreePanelNodeMover();
            _treeRoleSelector.LeftReadData += _treeRoleSelector_LeftReadData;
            _treeRoleSelector.RightReadData += _treeRoleSelector_RightReadData;
            _treeRoleSelector.Height = 180;
            _treeRoleSelector.Width = 750;
            _treeRoleSelector.ColSpan = 2;
            _treeRoleSelector.ID = "treeRoles";
            _formPanel.Add(_treeRoleSelector);
            _btnSave = new Button();
            _btnSave.Icon = Icon.DatabaseSave;
            _btnSave.Text = "保存";
            _btnSave.DirectEvents.Click.Event += SaveUser;
            _btnSave.DirectEvents.Click.EventMask.Set("正在添加");
            _formPanel.Buttons.Add(_btnSave);
            _hidId = new Hidden();
            _hidId.Name = "Id";
            _formPanel.Controls.Add(_hidId);
            MainForm.Controls.Add(_formPanel);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            id = ConvertHelper.ToInt32(Request["id"]);
            user = privilege.GetUser(id);
            if (user == null)
            {
                ShowError("该用户不存在");
            }
            if (!X.IsAjaxRequest && !IsPostBack)
            {
                _comboStatusStore.DataSource = EnumHelper.GetList(typeof(BaseStatuses), (k, v) =>
                {
                    return new
                    {
                        Id = k,
                        Name = v
                    };
                });
                _comboStatusStore.DataBind();
                _txtUsername.SetValue(user.Username);
                _comboStatus.SetValue(user.Status);
                _txtMemo.SetValue(user.Description);
                _txtUsername.Disable();
                _hidId.SetValue(id);
            }
        }
        private void SaveUser(object sender, DirectEventArgs e)
        {
            var mr = GetModelFromPost<UpdateUserModel>();
            if (mr.Success)
            {
                try
                {
                    privilege.UpdateUser(mr.Model, _treeRoleSelector.GetSelected());
                    AlertCloseWindowRefreshGrid("修改成功", "gridUsers");
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
        }

        private void _treeRoleSelector_LeftReadData(object sender, NodeLoadEventArgs e)
        {
            e.Nodes.AddRange(privilege.GetUserNotHaveRoleNodes(id));
        }

        private void _treeRoleSelector_RightReadData(object sender, NodeLoadEventArgs e)
        {
            e.Nodes.AddRange(privilege.GetUserRoleNodes(id));
        }
        protected abstract override int Privilege { get; }
    }
}
