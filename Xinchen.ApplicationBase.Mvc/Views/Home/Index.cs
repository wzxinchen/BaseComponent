using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ext.Net.MVC;
using Ext.Net;
namespace Xinchen.ApplicationBase.Mvc.Views.Home
{
    public class Index : ViewPage
    {
        public override void RenderView(System.Web.Mvc.ViewContext viewContext, System.IO.TextWriter writer)
        {
            writer.Write("<!DOCTYPE html>");
            writer.Write("<head>");
            writer.Write("<title>后台管理中心 - " + AppConfig.WebTitle + "</title>");
            writer.Write("</head><body></body>");
            var x = Html.X();
            writer.Write(x.ResourceManager().ToHtmlString());
            #region 总体布局
            var viewPort = new Viewport();
            viewPort.Layout = "border";
            var _copyright = new Panel();
            _copyright.Title = AppConfig.WebName + " 版权所有";
            _copyright.TitleAlign = TitleAlign.Center;
            _copyright.Collapsible = false;
            _copyright.Region = Region.South;
            _copyright.Split = true;
            var _menu = new Panel();
            _menu.Title = "导航菜单";
            _menu.Collapsible = true;
            _menu.Region = Region.West;
            _menu.Split = true;
            _menu.Width = 200;
            var _workArea = new Ext.Net.TabPanel();
            _workArea.Title = "欢迎使用";
            _workArea.Region = Region.Center;
            _workArea.ID = "tabWork";
            var _title = new Panel();
            _title.Title = AppConfig.WebTitle;
            _title.Collapsible = false;
            _title.Region = Region.North;
            _title.Split = true;
            viewPort.Items.Add(_title);
            viewPort.Items.Add(_workArea);
            viewPort.Items.Add(_copyright);
            viewPort.Items.Add(_menu);
            #endregion

            #region 个人区
            var _personPanel = new Ext.Net.Panel();
            _personPanel.Collapsed = true;
            _personPanel.Collapsible = true;
            _personPanel.Title = "欢迎使用";
            _personPanel.Height = 110;
            _personPanel.BodyPadding = 10;
            _personPanel.Layout = "table";
            _personPanel.LayoutConfig.Add(new TableLayoutConfig()
            {
                Columns = 2
            });
            Image avatarImg = new Image();
            avatarImg.RowSpan = 2;
            avatarImg.Width = avatarImg.Height = 70;
            //avatarImg.ImageUrl = BaseResource.avatar;
            _personPanel.Add(avatarImg);
            _personPanel.Add(new Label(UserInfo.Username));

            ButtonGroup buttonGroup = new ButtonGroup();
            buttonGroup.Width = 80;
            buttonGroup.Layout = "vbox";
            buttonGroup.Add(new Button()
            {
                Icon= Ext.Net.Icon.Key,
                Text = "修改密码",
                ID = "btnChangePassword",
                OnClientClick = "App.winChangePassword.show();App.winChangePassword.getLoader().load();"
            });

            var btnExit = new Button()
            {
                Text = "安全退出",
                ID = "btnExit",
                Icon = Icon.KeyDelete
            };

            buttonGroup.Add(btnExit);
            _personPanel.Add(buttonGroup);
            _menu.Add(_personPanel);
            var winChangePassword = new Window()
            {
                Icon = Icon.Key,
                BodyPadding = 10,
                Width = 300,
                Height = 210,
                Modal = true,
                Hidden = true,
                AutoShow = false,
                ID = "winChangePassword",
                Title = "修改密码",
                Loader = new ComponentLoader()
                {
                    Url = Url.Action("changePassword", "account"),
                    Mode = LoadMode.Frame
                }
            };
            #endregion

            var _menuPanel = new TreePanel()
            {
                Title = "功能菜单",
                Height = 500,
                RootVisible = false,
                ID = "mainMenu"
            };

            var _menuStore = new TreeStore()
             {
                 NodeParam = "parentId"
             };
            _menuStore.Proxy.Add(x.AjaxProxy().Url(Url.Action("GetMenus")).ActionMethods(y => y.Read = HttpMethod.POST));
            //_menuStore.ReadData += _menuStore_ReadData;
            _menuPanel.Store.Add(_menuStore);
            _menuPanel.Root.Add(new Node()
            {
                NodeID = "0",
                Text = "Root",
                Expanded = true
            });
            _menu.Add(_menuPanel);
            var itemClick = _menuPanel.DirectEvents.ItemClick;
            itemClick.Before = "var tree=arguments[0],eventType=arguments[1],eventName=arguments[2],extraParams=arguments[3];var tab = App.tabWork.getComponent('menu' + extraParams.menuid);if (tab) {App.tabWork.setActiveTab(tab);return false;}return tree.getStore().getNodeById(extraParams.menuid).isLeaf();";
            itemClick.Url = Url.Action("OpenPage");
            itemClick.ExtraParams.Add(new Parameter("menuid", "record.data.id", ParameterMode.Raw));
            writer.Write(viewPort.ToBuilder().ToHtmlString());
        }
    }
}
