using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ExtNetBase.TreePanelEx;
using Xinchen.ExtNetBase;
using Xinchen.Utils;
using Xinchen.PrivilegeManagement.Enums;
using Xinchen.ApplicationBase.Model;
namespace Xinchen.ApplicationBase.UI.Role
{
    public abstract class EditPageBase : PageBase
    {
        private FormPanel _formPanel;
        private TextField _txtRolename;
        private ComboBox _comboStatus;
        private Store _comboStatusStore;
        private TextArea _txtMemo;
        private TreePanelNodeMover _treePrivilegeSelector;
        private Button _btnSave;
        private int id;
        private PrivilegeManagement.DTO.Role role;
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
            _txtRolename = new TextField();
            _txtRolename.ID = "txtname";
            _txtRolename.Name = "Name";
            _txtRolename.ColSpan = 1;
            _txtRolename.Width = 300;
            _txtRolename.AllowBlank = false;
            _txtRolename.FieldLabel = "角色名";
            _txtRolename.EmptyText = "输入角色名";
            _formPanel.Add(_txtRolename);
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
            _txtMemo.EmptyText = "简单描述一下角色的功能";
            _txtMemo.FieldLabel = "描述";
            _txtMemo.Height = 70;
            _formPanel.Add(_txtMemo);
            _treePrivilegeSelector = new TreePanelNodeMover();
            _treePrivilegeSelector.LeftReadData += _treePrivilegeSelector_LeftReadData;
            _treePrivilegeSelector.RightReadData += _treePrivilegeSelector_RightReadData;
            _treePrivilegeSelector.Height = 180;
            _treePrivilegeSelector.Width = 750;
            _treePrivilegeSelector.ColSpan = 2;
            _treePrivilegeSelector.ID = "treeRoles";
            _formPanel.Add(_treePrivilegeSelector);
            _btnSave = new Button();
            _btnSave.Icon = Icon.DatabaseSave;
            _btnSave.Text = "保存";
            _btnSave.DirectEvents.Click.Event += SaveRole;
            _btnSave.DirectEvents.Click.EventMask.Set("正在添加");
            _formPanel.Buttons.Add(_btnSave);
            _hidId = new Hidden();
            _hidId.Name = "Id";
            _formPanel.Controls.Add(_hidId);
            MainForm.Controls.Add(_formPanel);
        }

        private void SaveRole(object sender, DirectEventArgs e)
        {
            var mr = GetModelFromPost<UpdateRoleModel>();
            if (mr.Success)
            {
                try
                {
                    privilege.UpdateRole(mr.Model, _treePrivilegeSelector.GetSelected());
                    AlertCloseWindowRefreshGrid("修改成功", "gridRoles");
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            id = ConvertHelper.ToInt32(Request["id"]);
            role = privilege.GetRole(id);
            if (role == null)
            {
                ShowError("该角色不存在");
            }
            if (!X.IsAjaxRequest && !IsPostBack)
            {
                _txtRolename.SetValue(role.Name);
                _comboStatus.SetValue(role.Status);
                _txtMemo.SetValue(role.Description);
                _txtRolename.Disable();
                _hidId.SetValue(id);
            }
        }

        private void _treePrivilegeSelector_RightReadData(object sender, NodeLoadEventArgs e)
        {
            e.Nodes.AddRange(privilege.GetRolePrivilegeNodes(id));
        }

        private void _treePrivilegeSelector_LeftReadData(object sender, NodeLoadEventArgs e)
        {
            e.Nodes.AddRange(privilege.GetRoleCanAddPrivilegeNodes(id));
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
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
        protected abstract override int Privilege { get; }
    }
}
