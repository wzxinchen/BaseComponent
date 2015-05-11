using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Xinchen.ApplicationBase.Model;
using Xinchen.ExtNetBase;
using Xinchen.ExtNetBase.TreePanelEx;
using Xinchen.PrivilegeManagement.Enums;
using Xinchen.Utils;

namespace Xinchen.ApplicationBase.UI.Role
{
    public abstract class AddPageBase : PageBase
    {
        private FormPanel _formPanel;
        private TextField _txtRolename;
        private ComboBox _comboStatus;
        private Store _comboStatusStore;
        private TextArea _txtMemo;
        private TreePanelNodeMover _treePrivilegeSelector;
        private Button _btnSave;

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
            //_treePrivilegeSelector.RightReadData += _treePrivilegeSelector_RightReadData;
            _treePrivilegeSelector.Height = 180;
            _treePrivilegeSelector.Width = 750;
            _treePrivilegeSelector.ColSpan = 2;
            _treePrivilegeSelector.ID = "treeRoles";
            _formPanel.Add(_treePrivilegeSelector);
            _btnSave = new Button();
            _btnSave.Icon = Icon.Add;
            _btnSave.Text = "添加";
            _btnSave.DirectEvents.Click.Event += AddRole;
            _btnSave.DirectEvents.Click.EventMask.Set("正在添加");
            _formPanel.Buttons.Add(_btnSave);
            MainForm.Controls.Add(_formPanel);
        }

        private void AddRole(object sender, DirectEventArgs e)
        {
            var mr = GetModelFromPost<AddRoleModel>();
            if (mr.Success)
            {
                try
                {
                    privilege.AddRole(mr.Model, _treePrivilegeSelector.GetSelected());
                    AlertCloseWindowRefreshGrid("添加成功", "gridRoles");
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //TreePanel panel = new TreePanel();
            //var root = new Ext.Net.Node();
            //root.NodeID = "1";
            //root.Text = "a";
            //root.Children.Add(new Ext.Net.Node()
            //{
            //    NodeID = "2",
            //    Text = "111",
            //    Leaf = true
            //});
            //panel.Root.Add(root);
            //TreeStore store = new TreeStore();
            //store.ReadData += store_ReadData;
            //panel.Store.Add(store);
            //panel.RootVisible = false;
            //var model = new Ext.Net.Model();
            //model.Fields.Add(new ModelField("Id", ModelFieldType.Int));
            //model.Fields.Add(new ModelField("Name", ModelFieldType.String));
            //store.AutoDataBind = true;
            //store.AutoLoad = true;
            //Ext.Net.Parameter parameters = new Ext.Net.Parameter
            //{
            //    Name = "id",
            //    Value = "record.data.id"
            //};
            //panel.DirectEvents.ItemDblClick.ExtraParams.Add(parameters);
            //store.Model.Add(model);
            //store.Proxy.Add(new PageProxy());
            //panel.DirectEvents.ItemDblClick.Event+=ItemDblClick_Event;
            //Form.Controls.Add(panel);
        }

        void store_ReadData(object sender, NodeLoadEventArgs e)
        {

        }

        void _treePrivilegeSelector_RightReadData(object sender, NodeLoadEventArgs e)
        {
            //e.Nodes.AddRange(privilege.GetRolePrivilegeNodes(ConvertHelper.ToInt32(e.ExtraParams["id"])));
        }

        public void _treePrivilegeSelector_LeftReadData(object sender, NodeLoadEventArgs e)
        {
            e.Nodes.AddRange(privilege.GetRoleCanAddPrivilegeNodes(ConvertHelper.ToInt32(e.ExtraParams["id"])));
        }
        protected abstract override int Privilege { get; }
    }
}
