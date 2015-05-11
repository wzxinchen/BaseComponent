using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ExtNetBase.Buttons;
using Xinchen.ExtNetBase;
using Xinchen.Utils;
namespace Xinchen.ApplicationBase.UI
{
    public abstract class IndexPageBase : PageBase
    {
        private Viewport _viewPort;
        private Panel _copyright;
        private Panel _menu;
        private TabPanel _workArea;
        private Panel _title;
        private Panel _personPanel;
        private Window winChangePassword;
        private Button btnExit;
        private TreePanel _menuPanel;
        private TreeStore _menuStore;
        private Window _winParentWindow;
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            #region 总体布局
            _viewPort = new Viewport();
            _viewPort.Layout = "border";
            _copyright = new Panel();
            _copyright.Title = "版权";
            _copyright.TitleAlign = TitleAlign.Center;
            _copyright.Collapsible = false;
            _copyright.Region = Region.South;
            _copyright.Split = true;
            _menu = new Panel();
            _menu.Title = "导航菜单";
            _menu.Collapsible = true;
            _menu.Region = Region.West;
            _menu.Split = true;
            _menu.Width = 200;
            _workArea = new Ext.Net.TabPanel();
            _workArea.Title = "欢迎使用";
            _workArea.Region = Region.Center;
            _workArea.ID = "tabWork";
            _title = new Panel();
            _title.Title = WebName;
            _title.Collapsible = false;
            _title.Region = Region.North;
            _title.Split = true;
            _viewPort.Items.Add(_title);
            _viewPort.Items.Add(_workArea);
            _viewPort.Items.Add(_copyright);
            _viewPort.Items.Add(_menu);
            #endregion

            #region 个人区
            _personPanel = new Ext.Net.Panel();
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
            avatarImg.ImageUrl = BaseResource.avatar;
            _personPanel.Add(avatarImg);
            _personPanel.Add(new Label(userInfo.Username));

            ButtonGroup buttonGroup = new ButtonGroup();
            buttonGroup.Width = 80;
            buttonGroup.Layout = "vbox";
            buttonGroup.Add(new KeyButton()
            {
                Text = "修改密码",
                ID = "btnChangePassword",
                OnClientClick = "App.winChangePassword.show();App.winChangePassword.getLoader().load();"
            });

            btnExit = new Button()
            {
                Text = "安全退出",
                ID = "btnExit",
                Icon = Icon.KeyDelete
            };
            var clickEvent = btnExit.DirectEvents.Click;
            clickEvent.Event += clickEvent_Event;
            clickEvent.EventMask.Set("正在退出");
            clickEvent.Confirmation.ConfirmRequest = true;
            clickEvent.Confirmation.Title = "提示";
            clickEvent.Confirmation.Message = "确定退出？";
            buttonGroup.Add(btnExit);
            _personPanel.Add(buttonGroup);
            _menu.Add(_personPanel);
            winChangePassword = new Window()
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
                    Url = ResolveClientUrl("~/user/changepassword.aspx"),
                    Mode = LoadMode.Frame
                }
            };
            winChangePassword.Loader.LoadMask.Set("正在加载");
            Controls.Add(winChangePassword);
            #endregion

            _menuPanel = new TreePanel()
            {
                Title = "功能菜单",
                Height = 500,
                RootVisible = false,
                ID = "mainMenu"
            };

            _menuStore = new TreeStore()
            {
                NodeParam = "parentId"
            };
            _menuStore.ReadData += _menuStore_ReadData;
            _menuPanel.Store.Add(_menuStore);
            _menuPanel.Root.Add(new Node()
            {
                NodeID = "0",
                Text = "Root",
                Expanded = true
            });
            _menu.Add(_menuPanel);
            var itemClick = _menuPanel.DirectEvents.ItemClick;
            itemClick.Before = "var tree=arguments[0],eventType=arguments[1],eventName=arguments[2],extraParams=arguments[3];var tab = App.tabWork.getComponent('menu' + extraParams.id);if (tab) {App.tabWork.setActiveTab(tab);return false;}return tree.getStore().getNodeById(extraParams.id).isLeaf();";
            itemClick.EventMask.Set("正在加载");
            itemClick.Event += itemClick_Event;
            itemClick.ExtraParams.Add(new Parameter("id", "record.data.id", ParameterMode.Raw));

            #region 隐藏顶级窗口
            _winParentWindow = new Window();
            _winParentWindow.Hidden = true;
            _winParentWindow.Loader = new ComponentLoader();
            _winParentWindow.Loader.Mode = LoadMode.Frame;
            _winParentWindow.Width = 800;
            _winParentWindow.Modal = true;
            _winParentWindow.Height = 600;
            _winParentWindow.ID = "_topWin";
            Controls.Add(_winParentWindow);
            #endregion

            Controls.Add(_viewPort);
        }

        void itemClick_Event(object sender, DirectEventArgs e)
        {
            int menuId = ConvertHelper.ToInt32(e.ExtraParams["id"]);
            var menu =privilege.GetMenu(menuId);
            if (menu == null)
            {
                Alert("该菜单不存在");
                return;
            }
            if (menu.PrivilegeId == null)
            {
                Alert("该菜单没有对应的权限");
                return;
            }
            var menuPrivilege = privilege.GetPrivilege(menu.PrivilegeId.Value);
            if (menuPrivilege == null)
            {
                Alert("该菜单对应的权限不存在");
                return;
            }
            if (!HasPrivilege(menuPrivilege.Id))
            {
                Alert("您没有权限使用该菜单");
                return;
            }
            if (string.IsNullOrEmpty(menu.Url)) return;
            string menuIdString = "menu" + menu.Id.ToString();
            var tabPage = _workArea.Items.FirstOrDefault(x => x.ID == menuIdString);
            if (tabPage == null)
            {
                tabPage = new Ext.Net.Panel(new Ext.Net.Panel.Config()
                {
                    Title = menu.Name,
                    Closable = true
                });
                tabPage.ID = menuIdString;
                tabPage.Loader = new ComponentLoader(new ComponentLoader.Config()
                {
                    Mode = LoadMode.Frame
                });
                tabPage.Loader.Url = menu.Url;
                tabPage.Loader.LoadMask.ShowMask = true;
                tabPage.Loader.LoadMask.Msg = "正在加载";
                tabPage.AddTo(_workArea);
            }
            _workArea.SetActiveTab(tabPage);
        }

        void _menuStore_ReadData(object sender, NodeLoadEventArgs e)
        {
            e.Nodes.AddRange(privilege.GetNavigationMenus(Convert.ToInt32(e.ExtraParams["parentId"])));
        }

        void clickEvent_Event(object sender, DirectEventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/user/login.aspx");
        }
        protected abstract override int Privilege { get; }

        protected abstract string WebName { get; }
    }
}
