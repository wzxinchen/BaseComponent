using Ext.Net;
using Newtonsoft.Json;
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
    public abstract class AddPageBase : PageBase
    {
        protected abstract override int Privilege { get; }
        private FormPanel _formPanel;
        private TextField _txtUsername;
        private ComboBox _comboStatus;
        private Store _comboStatusStore;
        private TextArea _txtMemo;
        private TreePanelNodeMover _treeRoleSelector;
        private Button _btnAdd;
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
            _comboStatusStore = new Store();
            _comboStatusStore.ID = "comboStoreStatus";
            _comboStatusStore.Model.Add(ComponentHelper.GetModel(
                new Dictionary<string, ModelFieldType>(){
            {"Id",ModelFieldType.Int},{"Name",ModelFieldType.String}
            }));
            _comboStatus.ValueHiddenName = "Status";
            _comboStatus.SimpleSubmit = true;
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
            _txtMemo.FieldLabel = "描述";
            _txtMemo.Height = 80;
            _formPanel.Add(_txtMemo);
            _treeRoleSelector = new TreePanelNodeMover();
            _treeRoleSelector.LeftReadData += _treeRoleSelector_LeftReadData;
            //_treeRoleSelector.RightReadData += _treeRoleSelector_RightReadData;
            _treeRoleSelector.Height = 180;
            _treeRoleSelector.Width = 750;
            _treeRoleSelector.ColSpan = 2;
            _treeRoleSelector.ID = "treeRoles";
            _formPanel.Add(_treeRoleSelector);
            _btnAdd = new Button();
            _btnAdd.Text = "添加";
            _btnAdd.Icon = Icon.Add;
            _btnAdd.DirectEvents.Click.Event += AddUser;
            _btnAdd.DirectEvents.Click.EventMask.Set("正在添加");
            _formPanel.Buttons.Add(_btnAdd);
            MainForm.Controls.Add(_formPanel);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
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
            }
        }

        void AddUser(object sender, DirectEventArgs e)
        {
            var mr = GetModelFromPost<AddUserModel>();
            if (mr.Success)
            {
                try
                {
                    privilege.AddUser(mr.Model, _treeRoleSelector.GetSelected());
                    AlertCloseWindowRefreshGrid("添加成功", "gridUsers");
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

        void _treeRoleSelector_RightReadData(object sender, NodeLoadEventArgs e)
        {

        }

        void _treeRoleSelector_LeftReadData(object sender, NodeLoadEventArgs e)
        {
            e.Nodes.AddRange(privilege.GetRoleNodes());
        }
    }
}
