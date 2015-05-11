namespace Xinchen.ExtNetBase.TreePanelEx
{
    using Ext.Net;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.UI;

    [ToolboxData("<{0}:TreeGrid runat=server></{0}:TreeGrid>"), DefaultProperty("Text")]
    public class TreeGrid : TreePanel
    {
        private bool _enableRemove = true, _enableEdit = true;
        //private int _level = 1;
        private INodeHelper _nodeHelper;
        private TreeStore _treeStore;

        private void btnRefresh_DirectClick(object sender, DirectEventArgs e)
        {
            this._treeStore.LoadProxy();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (this._nodeHelper == null)
            {
                _nodeHelper = new NodeHelper(Table);
            }
            TreeStore.Config config = new TreeStore.Config
            {
                NodeParam = "parentId"
            };
            this._treeStore = new TreeStore(config);
            this._treeStore.ID = "_treeStore";
            Model model = new Model
            {
                Fields = { new ModelField("Id", ModelFieldType.String), new ModelField("Name", ModelFieldType.String), new ModelField("ParentId", ModelFieldType.String) }
            };
            model.Fields.AddRange(this._nodeHelper.CustomFields);
            this._treeStore.Model.Add(model);
            this._treeStore.Proxy.Add(new PageProxy());
            this.Store.Add(this._treeStore);
            this._treeStore.ReadData += new TreeStoreBase.ReadDataEventHandler(this.treeStore_ReadData);
            base.RemoteRemove += new TreePanel.RemoteRemoveEventHandler(this.TreePanelEx_RemoteRemove);
            base.RemoteEdit += new TreePanel.RemoteEditEventHandler(this.TreePanelEx_RemoteEdit);
            base.RemoteMove += new TreePanel.RemoteMoveEventHandler(this.TreePanelEx_RemoteMove);
            Ext.Net.Button button = new Ext.Net.Button
            {
                Text = "刷新",
                Handler = "App." + this.ID + ".getStore().load();"
            };
            Toolbar toolbar = new Toolbar();
            toolbar.Items.Add(button);
            this.TopBar.Add(toolbar);
            if (!Ext.Net.X.IsAjaxRequest)
            {
                //base.Listeners.NodeDragOver.Handler = "var recs = dragData.records;var prev=-1;for(var i=0;i<recs.length;i++){var recData=recs[i].data;if(prev==-1){prev=recData.Level;}else{if(prev!=recData.Level){return false;}}}if(targetNode.data.Level>=" + this.Level + ")return false;return true;";
                Parameter parameters = new Parameter
                {
                    Name = "parentId",
                    Value = "arguments[1].data.parentId",
                    Mode = ParameterMode.Raw
                };
                this.RemoteExtraParams.Add(parameters);
                this.On("beforeRemoteAction", new JFunction("Ext.net.Mask.show({ msg : '正在加载' });"));
                JFunction fn = new JFunction("Ext.net.Mask.hide();");
                this.On("remoteActionRefusal", fn);
                this.On("remoteActionException", fn);
                this.On("remoteActionSuccess", fn);
                this.On("remoteEditRefusal", new JFunction("Ext.Msg.alert('失败了')"));
                this._treeStore.On("beforeload", new JFunction("Ext.net.Mask.show({ msg : '正在加载' });"));
                this._treeStore.On("load", new JFunction("Ext.net.Mask.hide();"));
                Ext.Net.Node node = new Ext.Net.Node();
                node.CustomAttributes.Add(new ConfigItem("Id", ""));
                node.CustomAttributes.Add(new ConfigItem("Name", "根"));
                node.NodeID = "0";
                node.Expanded = true;
                node.Text = "根";
                this.Root.Add(node);
                Column column = new Column();
                this.ColumnModel.Columns.Add(column);
                TreeColumn column2 = new TreeColumn();
                this.ColumnModel.Columns.Add(column2);
                ActionColumn column3 = new ActionColumn();
                if (this.EnableRemove)
                {
                    ActionItem item2 = new ActionItem();
                    column3.Items.Add(item2);
                    item2.Icon = Ext.Net.Icon.PageWhiteDelete;
                    item2.Handler = "var record=arguments[5];var tree = App." + this.ID + ";var node = tree.getStore().getNodeById(record.data.Id) || tree.getStore().getNewRecords()[0];Ext.Msg.confirm(\"提示\", \"会删除相关的数据，无法恢复，确认删除？\", function (r) {if (r == \"yes\") {tree.removeNode(node);return;App.direct.RemoveNode(record.data.Id, {success: function (result) {if (result.Success) {node.remove();node.commit();} else {Ext.Msg.alert(\"错误\", result.Message);}},eventMask: {showMask: true,msg: \"正在删除\"}});}});";
                    item2.Tooltip = "删除";
                }
                this.ColumnModel.Columns.Add(column3);
                column.ID = "col1";
                column.DataIndex = "Id";
                column.Width = 50;
                column.Text = "编号";
                column2.ID = "col2";
                column2.DataIndex = "Name";
                column2.Width = 300;
                column2.Text = "名称";
                column3.ID = "col3";
                column3.Text = "操作";
                column3.Width = 60;
                column3.Align = Alignment.Center;
                if (EnableEdit)
                {
                    ActionItem item = new ActionItem();
                    column3.Items.Add(item);
                    item.Icon = Ext.Net.Icon.PageWhiteAdd;
                    item.Handler = "var record=arguments[5]; var tree = App." + this.ID + ";var ep = tree.editingPlugin;var node,store = tree.getStore();if (record.data.Id) {node = store.getNodeById(record.data.Id);}else{node = store.getRootNode();}node.expand(false, function () {node = node.appendChild({Name:'新节点'});setTimeout(function () {ep.startEdit(node, tree.columns[1]);}, 200);});";
                    item.Tooltip = "添加子节点";
                    CellEditing editing = new CellEditing();
                    editing.Listeners.CancelEdit.Handler = " if (e.record.data.Id) {e.record.reject();} else {e.record.remove(true);}";
                    this.Plugins.Add(editing);
                    this.Editor.Add(new TextField());
                    TreeView view = new TreeView();
                    this.View.Add(view);
                    TreeViewDragDrop drop = new TreeViewDragDrop
                    {
                        DragText = "移动到",
                        AppendOnly = true
                    };
                    view.Plugins.Add(drop);
                }
                this.Mode = TreePanelMode.Remote;

            }
            base.OnLoad(e);
        }

        private void ShowError(string msg, RemoteActionEventArgs e)
        {
            e.RefusalMessage = msg;
            e.Accept = false;
            Ext.Net.X.Msg.Alert("错误", msg, new JFunction("var tree=App." + this.ID + ";tree.editingPlugin.startEdit(tree.getSelectionModel().getSelection()[0],tree.columns[1]);")).Show();
        }

        private void TreePanelEx_RemoteEdit(object sender, RemoteEditEventArgs e)
        {
            if (!EnableEdit)
            {
                return;
            }
            string json = e.Json;
            string nodeID = e.NodeID;
            string str3 = e.ExtraParams["parentId"];
            try
            {
                int parentId = Convert.ToInt32(str3);
                int id = Convert.ToInt32(nodeID);
                if (id == 0)
                {
                    this._nodeHelper.CreateNode(json, parentId);
                }
                else
                {
                    this._nodeHelper.UpdateNode(id, json);
                }
                e.Accept = true;
            }
            catch (ArgumentException exception)
            {
                this.ShowError(exception.Message, e);
            }
            catch (ApplicationException ae)
            {
                ShowError(ae.Message, e);
            }
            catch (DbException exception2)
            {
                this.ShowError(exception2.Message, e);
            }
        }

        private void TreePanelEx_RemoteMove(object sender, RemoteMoveEventArgs e)
        {
            if (!EnableEdit) return;
            try
            {
                string[] source = e.Nodes.ToArray();
                string targetNodeID = e.TargetNodeID;
                if (source.Contains<string>(targetNodeID))
                {
                    throw new ArgumentException("不能移动到自身");
                }
                int target = Convert.ToInt32(targetNodeID);
                this._nodeHelper.ChangeParent(source, target);
                //int num2 = this._nodeHelper.GetItemLevel(target) + 1;
                foreach (string str2 in source)
                {
                    NodeProxy nodeById = this.GetNodeById(str2);
                    nodeById.Set("ParentId", target);
                    // nodeById.Set("Level", num2);
                }
                e.Accept = true;
            }
            catch (ArgumentException exception)
            {
                this.ShowError(exception.Message, e);
            }
            catch (DbException exception2)
            {
                this.ShowError(exception2.Message, e);
            }
        }

        private void TreePanelEx_RemoteRemove(object sender, RemoteRemoveEventArgs e)
        {
            try
            {
                if (!this.EnableRemove)
                {
                    throw new ArgumentException("删除被禁用");
                }
                if (!string.IsNullOrEmpty(e.NodeID))
                {
                    this._nodeHelper.RemoveNode(Convert.ToInt32(e.NodeID));
                }
                e.Accept = true;
            }
            catch (ArgumentException exception)
            {
                Ext.Net.X.Msg.Alert("错误", exception.Message).Show();
            }
            catch (DbException exception2)
            {
                Ext.Net.X.Msg.Alert("错误", exception2.Message).Show();
            }
        }

        private void treeStore_ReadData(object sender, NodeLoadEventArgs e)
        {
            IList<Xinchen.ExtNetBase.TreePanelEx.Node> nodeItems = this._nodeHelper.GetNodeItems(Convert.ToInt32(e.ExtraParams["parentId"]));
            foreach (Xinchen.ExtNetBase.TreePanelEx.Node node in nodeItems)
            {
                Ext.Net.Node node2 = new Ext.Net.Node
                {
                    Text = node.Name,
                    NodeID = node.Id.ToString(),
                    Leaf = false,
                    AllowDrag = true,
                    AllowDrop = true
                };
                ConfigItem item = new ConfigItem
                {
                    Name = "Id",
                    Value = node2.NodeID
                };
                node.CustomAttributes.Add(item);
                ConfigItem parameters = new ConfigItem
                {
                    Name = "Name",
                    Value = node2.Text
                };
                ConfigItem item3 = new ConfigItem
                {
                    Name = "ParentId",
                    Value = node.ParentId.ToString()
                };
                node2.CustomAttributes.Add(parameters);
                node2.CustomAttributes.Add(item3);
                node2.CustomAttributes.AddRange(node.CustomAttributes);
                e.Nodes.Add(node2);
            }
        }

        [Meta, Description("是否支持删除"), Category("2. Observable"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), ConfigOption("enableRemove", JsonMode.Object)]
        public bool EnableRemove
        {
            get
            {
                return this._enableRemove;
            }
            set
            {
                this._enableRemove = value;
            }
        }

        [Category("2. Observable"), Meta, PersistenceMode(PersistenceMode.InnerProperty), Description("支持几级分类"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), ConfigOption("level", JsonMode.Object)]
        //public int Level
        //{
        //    get
        //    {
        //        return this._level;
        //    }
        //    set
        //    {
        //        this._level = value;
        //    }
        //}

        public INodeHelper NodeHelper
        {
            get
            {
                return this._nodeHelper;
            }
            set
            {
                this._nodeHelper = value;
            }
        }

        [ConfigOption("table", JsonMode.Object), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Description("表名"), Meta, Category("2. Observable")]
        public string Table { get; set; }

        public bool EnableEdit
        {
            get
            {
                return _enableEdit;
            }
            set
            {
                _enableEdit = value;
            }
        }
    }
}

