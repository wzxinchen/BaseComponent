using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ApplicationBase.Model;
using Xinchen.ExtNetBase;
using Xinchen.Utils;

namespace Xinchen.ApplicationBase.UI.User
{
    public abstract class ManagerPageBase : PageBase
    {
        Viewport _viewPort;
        GridPanelEx _grid;
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            _viewPort = new Viewport();
            _grid = new GridPanelEx();
            _grid.ItemType = typeof(UserManagerModel);
            _grid.ReadData += _grid_ReadData;
            _grid.Delete += _grid_Delete;
            _grid.EnableAdd = EnableAddUser;
            _grid.EnableEdit = EnableEditUser;
            _grid.EnableRemove = EnableRemoveUser;
            _grid.Region = Region.Center;
            _grid.ID = "gridUsers";
            _grid.EditorConfig = new GridPanelEditorConfig();
            var _addWindowConfig = new WindowConfig();
            _addWindowConfig.Height = 400;
            _addWindowConfig.Width = 800;
            _addWindowConfig.Url = AddUrl;
            _addWindowConfig.Title = "添加用户";
            _grid.EditorConfig.AddWindow = _addWindowConfig;
            var _editWindowConfig = new WindowConfig();
            _editWindowConfig.Height = 400;
            _editWindowConfig.Width = 800;
            _editWindowConfig.Url = EditUrl;
            _editWindowConfig.Title = "编辑用户";
            _editWindowConfig.ExtraParams.Add(new Parameter("id", "record.data.Id", ParameterMode.Raw));
            _grid.EditorConfig.EditWindow = _editWindowConfig;
            _viewPort.Add(_grid);
            _viewPort.Layout = "border";
            Controls.Add(_viewPort);
        }

        void _grid_Delete(object sender, DirectEventArgs e)
        {
            privilege.DeleteUser(ConvertHelper.ToArray<int>(e.ExtraParams["ids"]));
            _grid.GetStore().Reload();
        }

        void _grid_ReadData(object sender, StoreReadDataEventArgs e)
        {
            var filterLinked = ConvertToFilterLinked(e, fc =>
            {
                switch (fc.Field)
                {
                    case "Roles":
                        return "roles.Id";
                    case "Status":
                        return "users.Status";
                }
                return null;
            });
            var sort = ConvertToSort(e.Sort);
            int recordCount = 0;
            var store = sender as Store;
            store.DataSource = privilege.GetUsers(e.Page, e.Limit, out recordCount, filterLinked, sort);
            store.DataBind();
            e.Total = recordCount;
        }
        protected abstract override int Privilege { get; }

        protected abstract bool EnableAddUser { get; }

        protected abstract bool EnableEditUser { get; }

        protected abstract bool EnableRemoveUser { get; }

        protected virtual string AddUrl
        {
            get
            {
                return "~/user/add.aspx";
            }
        }

        protected virtual string EditUrl
        {
            get
            {
                return "~/user/edit.aspx?id={id}";
            }
        }
    }
}
