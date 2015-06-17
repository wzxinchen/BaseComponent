using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xinchen.ApplicationBase.Mvc.Model;
using Xinchen.ExtNetBase;
using Ext.Net.MVC;
using Xinchen.ExtNetBase.Mvc;
using System.Web;
namespace Xinchen.ApplicationBase.Mvc.Views.Account
{
    public class Manager : ViewPage
    {
        public override void RenderView(System.Web.Mvc.ViewContext viewContext, System.IO.TextWriter writer)
        {
            writer.Write("<!DOCTYPE html>");
            writer.Write("<head>");
            writer.Write("<title>后台管理中心 - " + AppConfig.WebTitle + "</title>");
            writer.Write("</head><body></body>");
            writer.Write(Html.X().ResourceManager().ToHtmlString());
            var _viewPort = new Viewport();
            var _grid = new Xinchen.ExtNetBase.Mvc.GridPanelEx();
            _grid.ItemType = typeof(UserManagerModel);
            _grid.ReadUrl = Url.Action("GetAccounts");
            //_grid.ReadData += _grid_ReadData;
            //_grid.Delete += _grid_Delete;
            //_grid.EnableAdd = EnableAddUser;
            //_grid.EnableEdit = EnableEditUser;
            //_grid.EnableRemove = EnableRemoveUser;
            _grid.Region = Region.Center;
            _grid.ID = "gridUsers";
            _grid.EditorConfig = new GridPanelEditorConfig();
            var _addWindowConfig = new WindowConfig();
            _addWindowConfig.Height = 400;
            _addWindowConfig.Width = 800;
            _addWindowConfig.Url = Url.Action("AddAccount");
            _addWindowConfig.Title = "添加用户";
            _grid.EditorConfig.AddWindow = _addWindowConfig;
            var _editWindowConfig = new WindowConfig();
            _editWindowConfig.Height = 400;
            _editWindowConfig.Width = 800;
            _editWindowConfig.Url = Url.Action("UpdateAccount") + "?id=@id";
            _editWindowConfig.Title = "编辑用户";
            _editWindowConfig.ExtraParams.Add(new Parameter("id", "record.data.Id", ParameterMode.Raw));
            _grid.EditorConfig.EditWindow = _editWindowConfig;
            _viewPort.Add(_grid);
            _viewPort.Layout = "border";
            writer.Write(_viewPort.ToBuilder().ToHtmlString());
        }
    }
}
