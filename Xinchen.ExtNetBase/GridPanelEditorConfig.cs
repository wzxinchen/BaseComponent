namespace Xinchen.ExtNetBase
{
    using Ext.Net;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using Xinchen.ExtNetBase.TreePanelEx;

    public class GridPanelEditorConfig
    {
        public GridPanelEditorConfig()
        {
            this.ExtraParams = new ParameterCollection();
            Mode = EditorMode.Window;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Meta, Category("2. Observable"), Description("指定添加窗口的样式"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), ConfigOption("addWindow", JsonMode.Object)]
        public WindowConfig AddWindow { get; set; }

        [Description("指定编辑窗口的样式"), NotifyParentProperty(true), Category("2. Observable"), Meta, PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), ConfigOption("editWindow", JsonMode.Object)]
        public WindowConfig EditWindow { get; set; }

        [NotifyParentProperty(true), Description("额外参数"), PersistenceMode(PersistenceMode.InnerProperty), Meta, Category("2. Observable"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), ConfigOption("extraParams", JsonMode.Object)]
        public ParameterCollection ExtraParams { get; private set; }

        public EditorMode Mode { get; set; }
    }
}

