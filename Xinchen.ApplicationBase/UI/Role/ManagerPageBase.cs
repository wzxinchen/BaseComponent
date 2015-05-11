using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ApplicationBase.Model;
using Xinchen.ExtNetBase;
using Xinchen.Utils;

namespace Xinchen.ApplicationBase.UI.Role
{
    public abstract class ManagerPageBase : PageBase
    {
        private GridPanelEx _grid;
        private Viewport _viewPort;
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            _viewPort = new Viewport();
            _grid = new GridPanelEx();
            _grid.ItemType = typeof(RoleManagerModel);
            _grid.ReadData += _grid_ReadData;
            _grid.Delete += _grid_Delete;
            _grid.EnableAdd = EnableAddRole;
            _grid.EnableEdit = EnableEditRole;
            _grid.EnableRemove = EnableRemoveRole;
            _grid.Region = Region.Center;
            _grid.ID = "gridRoles";
            _grid.EditorConfig = new GridPanelEditorConfig();
            var _addWindowConfig = new WindowConfig();
            _addWindowConfig.Height = 400;
            _addWindowConfig.Width = 800;
            _addWindowConfig.Url = AddUrl;
            _addWindowConfig.Title = "添加角色";
            _grid.EditorConfig.AddWindow = _addWindowConfig;
            var _editWindowConfig = new WindowConfig();
            _editWindowConfig.Height = 400;
            _editWindowConfig.Width = 800;
            _editWindowConfig.Url = EditUrl;
            _editWindowConfig.Title = "编辑角色";
            _editWindowConfig.ExtraParams.Add(new Parameter("id", "record.data.Id", ParameterMode.Raw));
            _grid.EditorConfig.EditWindow = _editWindowConfig;
            _viewPort.Add(_grid);
            _viewPort.Layout = "border";
            Controls.Add(_viewPort);
        }

        private void _grid_Delete(object sender, DirectEventArgs e)
        {
            privilege.DeleteRoles(ConvertHelper.ToArray<int>(e.ExtraParams["ids"]));
            _grid.GetStore().Reload();
        }

        private void _grid_ReadData(object sender, StoreReadDataEventArgs e)
        {
            var store = (Store)sender;
            var filterLinked = ConvertToFilterLinked(e, fc =>
            {
                switch (fc.Field)
                {
                    case "Name":
                        return "roles.Name";
                    case "Privileges":
                        return "privileges.Id";
                }
                return null;
            });
            var sort = ConvertToSort(e.Sort);
            int recordCount = 0;
            store.DataSource = privilege.GetRoles(e.Page, e.Limit, out recordCount, filterLinked, sort);
            store.DataBind();
            e.Total = recordCount;
        }
        protected abstract override int Privilege { get; }

        public abstract bool EnableAddRole { get; }

        public abstract bool EnableEditRole { get; }

        public abstract bool EnableRemoveRole { get; }

        public virtual string AddUrl
        {
            get
            {
                return "~/role/add.aspx";
            }
        }

        public virtual string EditUrl
        {
            get
            {
                return "~/role/edit.aspx?id={id}";
            }
        }
    }
}
