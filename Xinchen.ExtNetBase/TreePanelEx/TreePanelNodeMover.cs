namespace Xinchen.ExtNetBase.TreePanelEx
{
    using Ext.Net;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Xinchen.ExtNetBase;
    using Xinchen.Utils;
    using System.Linq;

    [DefaultProperty("Text"), ToolboxData("<{0}:TreePanelNodeMover runat=server></{0}:TreePanelNodeMover>")]
    public class TreePanelNodeMover : Ext.Net.Panel
    {
        private List<int> _selectedIds;
        private Model _treeModelLeft;
        private Model _treeModelRight;
        private TreePanel _treePanelLeft;
        private TreePanel _treePanelRight;
        private TreeStore _treeStoreLeft;
        private TreeStore _treeStoreRight;
        public void RemoveLeftNode(string nodeId)
        {
            _treePanelLeft.GetNodeById(nodeId).Remove();
        }
        public event TreeStoreBase.ReadDataEventHandler LeftReadData;
        public event NodesAddEventHandler NodesAdding;
        private bool _enableBatchAdd = true;

        public bool EnableBatchAdd
        {
            get { return _enableBatchAdd; }
            set { _enableBatchAdd = value; }
        }
        private bool _enableBatchRemove = true;

        public bool EnableBatchRemove
        {
            get { return _enableBatchRemove; }
            set { _enableBatchRemove = value; }
        }

        public event TreeStoreBase.ReadDataEventHandler RightReadData;

        private void _treePanelLeft_Submit(object sender, SubmitEventArgs e)
        {
            List<SubmittedNode> children = e.RootNode.Children;
            foreach (SubmittedNode node in children)
            {
                Ext.Net.Node node2 = new Ext.Net.Node
                {
                    NodeID = node.NodeID,
                    Text = node.Text,
                    Leaf = true
                };
                this._treePanelRight.GetRootNode().InsertChild(0, node2);
                this._treePanelLeft.GetNodeById(node.NodeID).Remove();
                this._selectedIds.Add(ConvertHelper.ToInt32(node.NodeID));
            }
        }

        private void _treePanelRight_Submit(object sender, SubmitEventArgs e)
        {
            List<SubmittedNode> children = e.RootNode.Children;
            foreach (SubmittedNode node in children)
            {
                Ext.Net.Node node2 = new Ext.Net.Node
                {
                    NodeID = node.NodeID,
                    Text = node.Text,
                    Leaf = true
                };
                this._treePanelLeft.GetRootNode().InsertChild(0, node2);
                this._treePanelRight.GetNodeById(node.NodeID).Remove();
                this._selectedIds.Remove(ConvertHelper.ToInt32(node.NodeID));
            }
        }

        private void _treeStoreLeft_ReadData(object sender, NodeLoadEventArgs e)
        {
            if (this.LeftReadData != null)
            {
                this.LeftReadData(sender, e);
            }
        }

        private void _treeStoreRight_ReadData(object sender, NodeLoadEventArgs e)
        {
            if (this.RightReadData != null)
            {
                this.RightReadData(sender, e);
                foreach (Ext.Net.Node node in e.Nodes)
                {
                    this._selectedIds.Add(ConvertHelper.ToInt32(node.NodeID));
                }
            }
        }

        private void btnAddSelected_DirectClick(object sender, DirectEventArgs e)
        {
            this.MoveNode(this._treePanelLeft, this._treePanelRight);
        }

        private void btnRemoveSelected_DirectClick(object sender, DirectEventArgs e)
        {
            this.MoveNode(this._treePanelRight, this._treePanelLeft);
        }

        /// <summary>
        /// 获取已选择的记录
        /// </summary>
        /// <returns></returns>
        public int[] GetSelected(bool clearCahce = true)
        {
            int[] numArray = this._selectedIds.ToArray();
            if (clearCahce)
            {
                this.Context.Cache.Remove("_selectedIds");
            }
            return numArray;
        }

        private void InitTreePanel(TreePanel treePanel, TreeStore treeStore, Model model)
        {
            treePanel.RootVisible = false;
            treePanel.AutoDataBind = true;
            Ext.Net.Parameter parameters = new Ext.Net.Parameter
            {
                Name = "id",
                Value = "record.data.id",
                Mode = ParameterMode.Raw
            };
            treePanel.DirectEvents.ItemDblClick.ExtraParams.Add(parameters);
            treePanel.DirectEvents.ItemDblClick.EventMask.ShowMask = true;
            treePanel.DirectEvents.ItemDblClick.EventMask.Msg = "正在添加";
            treeStore.AutoDataBind = true;
            treeStore.AutoLoad = true;
            model.Fields.Add(new ModelField("Id", ModelFieldType.Int));
            model.Fields.Add(new ModelField("Name", ModelFieldType.String));
            treeStore.Model.Add(model);
            treeStore.Proxy.Add(new PageProxy());
            treePanel.Store.Add(treeStore);
            Ext.Net.Node item = new Ext.Net.Node
            {
                NodeID = "0",
                Text = "根",
                Expanded = true
            };
            treeStore.Root.Add(item);
            Add(treePanel);
        }

        private void LeftItemDblClick_Event(object sender, DirectEventArgs e)
        {
            this.MoveNode(this._treePanelLeft, this._treePanelRight);
        }

        private void MoveNode(TreePanel source, TreePanel target)
        {
            List<SubmittedNode> selectedNodes = source.SelectedNodes;
            if (selectedNodes != null)
            {
                if (NodesAdding != null)
                {
                    var nodeIds = selectedNodes.Select(x => Convert.ToInt32(x.NodeID));
                    var args = new NodesAddEventArgs();
                    args.NodeIds.AddRange(nodeIds);
                    NodesAdding(this, args);
                    if (args.CancelAdd)
                    {
                        Ext.Net.X.Msg.Alert("提示", args.ErrorMessage).Show();
                        return;
                    }
                }
                foreach (SubmittedNode node in selectedNodes)
                {
                    target.GetRootNode().InsertChild(0, node.ToProxyNode());
                    source.GetNodeById(node.NodeID).Remove();
                    if (target.ID.Contains("Right"))
                    {
                        //增加
                        this._selectedIds.Add(ConvertHelper.ToInt32(node.NodeID));
                    }
                    else
                    {
                        this._selectedIds.Remove(ConvertHelper.ToInt32(node.NodeID));
                    }
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            this._selectedIds = (List<int>)this.Context.Cache["_selectedIds"];
            if (this._selectedIds == null)
            {
                this._selectedIds = new List<int>();
                this.Context.Cache["_selectedIds"] = this._selectedIds;
            }
            this.Layout = "hbox";
            HBoxLayoutConfig item = new HBoxLayoutConfig
            {
                Align = HBoxAlign.Middle,
                DefaultMargins = "0"
            };

            this.LayoutConfig.Add(item);
            this._treePanelLeft = new TreePanel();
            this._treePanelLeft.ID = "_treePanelLeft";
            this._treePanelLeft.DirectEvents.ItemDblClick.Event += new ComponentDirectEvent.DirectEventHandler(this.LeftItemDblClick_Event);
            this._treePanelLeft.Submit += new TreePanel.SubmitEventHandler(this._treePanelLeft_Submit);
            this._treePanelLeft.DirectEvents.Submit.EventMask.Set("正在添加");
            this._treePanelLeft.Title = "可添加";
            _treePanelLeft.MultiSelect = EnableBatchAdd;
            this._treePanelLeft.Height = this.Height;
            double width = (this.Width.Value - 75.0) / 2.0;
            this._treePanelLeft.Width = new Unit(width);
            this._treeStoreLeft = new TreeStore();
            this._treeStoreLeft.ReadData += new TreeStoreBase.ReadDataEventHandler(this._treeStoreLeft_ReadData);
            this._treeModelLeft = new Model();
            this.InitTreePanel(this._treePanelLeft, this._treeStoreLeft, this._treeModelLeft);
            Ext.Net.Panel component = new Ext.Net.Panel
            {
                Width = 75,
                ButtonAlign = Alignment.Center,
                Layout = "vbox"
            };
            if (!Ext.Net.X.IsAjaxRequest)
            {
                component.Border = false;
            }
            var buttons = new List<Ext.Net.Button>();
            if (EnableBatchAdd)
            {
                Ext.Net.Button button = new Ext.Net.Button
                {
                    Text = "全部添加 >>",
                    TextAlign = ButtonTextAlign.Center
                };
                button.Listeners.Click.Handler = "#{" + this._treePanelLeft.ID + "}.submitNodes();";
                button.DirectEvents.Click.EventMask.Set("正在添加");
                buttons.Add(button);
            }
            Ext.Net.Button button2 = new Ext.Net.Button
            {
                Text = "添　　加　> ",
                TextAlign = ButtonTextAlign.Center
            };
            buttons.Add(button2);
            button2.DirectClick += new ComponentDirectEvent.DirectEventHandler(this.btnAddSelected_DirectClick);
            button2.DirectEvents.Click.EventMask.Set("正在添加");
            this._treePanelRight = new TreePanel();
            this._treePanelRight.ID = "_treePanelRight";
            this._treePanelRight.Title = "已添加";
            this._treePanelRight.DirectEvents.Submit.EventMask.Set("正在移除");
            this._treePanelRight.Submit += new TreePanel.SubmitEventHandler(this._treePanelRight_Submit);
            _treePanelRight.MultiSelect = EnableBatchRemove;
            this._treePanelRight.Height = this.Height;
            this._treePanelRight.DirectEvents.ItemDblClick.Event += new ComponentDirectEvent.DirectEventHandler(this.RightItemDblClick_Event);
            this._treePanelRight.Width = new Unit(width);
            this._treeStoreRight = new TreeStore();
            this._treeStoreRight.ReadData += new TreeStoreBase.ReadDataEventHandler(this._treeStoreRight_ReadData);
            this._treeModelRight = new Model();
            if (EnableBatchRemove)
            {
                Ext.Net.Button button3 = new Ext.Net.Button
                {
                    Text = "<< 全部移除",
                    TextAlign = ButtonTextAlign.Center
                };
                button3.Listeners.Click.Handler = "#{" + this._treePanelRight.ID + "}.submitNodes();";
                buttons.Add(button3);
            }
            Ext.Net.Button button4 = new Ext.Net.Button
            {
                Text = "<　移　　除",
                TextAlign = ButtonTextAlign.Center
            };
            buttons.Add(button4);
            button4.DirectClick += new ComponentDirectEvent.DirectEventHandler(this.btnRemoveSelected_DirectClick);
            button4.DirectEvents.Click.EventMask.Set("正在移除");
            component.Add(buttons.ToArray());
            this.Add(component);
            this.InitTreePanel(this._treePanelRight, this._treeStoreRight, this._treeModelRight);
            base.OnInit(e);
        }

        private void RightItemDblClick_Event(object sender, DirectEventArgs e)
        {
            this.MoveNode(this._treePanelRight, this._treePanelLeft);
        }
    }
}