namespace Xinchen.ExtNetBase
{
    using Ext.Net;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    public class GridPanelExDirectEvents : GridPanelDirectEvents
    {
        private ComponentDirectEvent _addButtonClick;

        public GridPanelExDirectEvents()
        {
        }

        public GridPanelExDirectEvents(Observable parent)
        {
            this.Parent = parent;
        }

        [ConfigOption("addButtonClick", typeof(DirectEventJsonConverter)), ListenerArgument(2, "group"), ListenerArgument(3, "e"), ListenerArgument(0, "view", typeof(TableView)), NotifyParentProperty(true), TypeConverter(typeof(ExpandableObjectConverter)), Description("当添加按钮被点击时发生"), ListenerArgument(1, "node", typeof(string)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ComponentDirectEvent AddButtonClick
        {
            get
            {
                return (this._addButtonClick ?? (this._addButtonClick = new ComponentDirectEvent(this)));
            }
        }

        public override ConfigOptionsCollection ConfigOptions
        {
            get
            {
                ConfigOptionsCollection configOptions = base.ConfigOptions;
                configOptions.Add("addButtonClick", new ConfigOption("addButtonClick", new SerializationOptions("add", typeof(DirectEventJsonConverter)), null, this.AddButtonClick));
                return configOptions;
            }
        }
    }
}

