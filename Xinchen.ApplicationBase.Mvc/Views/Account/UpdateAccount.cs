using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xinchen.ExtNetBase.Mvc;
using Xinchen.PrivilegeManagement.DTO;
using Xinchen.PrivilegeManagement.Enums;
using Xinchen.Utils;

namespace Xinchen.ApplicationBase.Mvc.Views.Account
{
    public class UpdateAccount : ViewPage<User>
    {
        public override void RenderView(System.Web.Mvc.ViewContext viewContext, System.IO.TextWriter writer)
        {
            RenderMain(writer);
            var _formPanel = new ValidatedForm();
            _formPanel.BodyPadding = 10;
            _formPanel.Layout = "table";
            _formPanel.LayoutConfig.Add(new TableLayoutConfig()
            {
                Columns = 2
            });
            _formPanel.ID = "addForm";
            _formPanel.DefaultAnchor = "100%";
            var _txtUsername = new TextField();
            _txtUsername.ID = "txtUsername";
            _txtUsername.Name = "Username";
            _txtUsername.SetValue(Model.Username);
            _txtUsername.ColSpan = 1;
            _txtUsername.Width = 300;
            _txtUsername.AllowBlank = false;
            _txtUsername.FieldLabel = "用户名";
            _txtUsername.EmptyText = "输入用户名";
            _formPanel.Add(_txtUsername);
            var _comboStatus = new ComboBox();
            _comboStatus.EmptyText = "选择状态";
            _comboStatus.ValueHiddenName = "Status";
            _comboStatus.SimpleSubmit = true;
            var _comboStatusStore = new Store();
            _comboStatusStore.ID = "comboStoreStatus";
            _comboStatusStore.Model.Add(ComponentHelper.GetModel(
                new Dictionary<string, ModelFieldType>(){
            {"Id",ModelFieldType.Int},{"Name",ModelFieldType.String}
            }));
            _comboStatusStore.DataSource = EnumHelper.GetList(typeof(BaseStatuses), (k, v) =>
            {
                return new
                {
                    Id = k,
                    Name = v
                };
            });
            _comboStatusStore.DataBind();
            _comboStatus.SetValue(Model.Status.GetHashCode());
            _comboStatus.DisplayField = "Name";
            _comboStatus.ValueField = "Id";
            _comboStatus.Store.Add(_comboStatusStore);
            _comboStatus.FieldLabel = "状态";
            _comboStatus.Editable = false;
            _formPanel.Add(_comboStatus);
            var _txtMemo = new TextArea();
            _txtMemo.SetValue(Model.Description);
            _txtMemo.Width = 750;
            _txtMemo.Name = "Description";
            _txtMemo.ColSpan = 2;
            _txtMemo.EmptyText = "简单描述一下用户的职责";
            _txtMemo.FieldLabel = "描述";
            _txtMemo.Height = 70;
            _formPanel.Add(_txtMemo);
            var _treeRoleSelector = new TreePanelNodeMover();
            _treeRoleSelector.LeftReadProxy.Url = Url.Action("GetNotAddedRoles");
            _treeRoleSelector.LeftReadProxy.ExtraParams.Add(new Parameter("accountId", Model.Id.ToString(), ParameterMode.Raw));
            _treeRoleSelector.RightReadProxy.Url = Url.Action("GetAddedRoles");
            _treeRoleSelector.RightReadProxy.ExtraParams.Add(new Parameter("accountId", Model.Id.ToString(), ParameterMode.Raw));
            _treeRoleSelector.Height = 180;
            _treeRoleSelector.Width = 750;
            _treeRoleSelector.ColSpan = 2;
            _treeRoleSelector.ID = "treeRoles";
            _formPanel.Add(_treeRoleSelector);
            var _btnSave = new Button();
            _btnSave.Icon = Icon.DatabaseSave;
            _btnSave.Text = "保存";
            _formPanel.Buttons.Add(_btnSave);
            var _hidId = new Hidden();
            _hidId.Name = "Id";
            _formPanel.Controls.Add(_hidId);
            writer.Write(_formPanel.ToBuilder().ToHtmlString());
        }
    }
}
