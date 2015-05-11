namespace Xinchen.ExtNetBase
{
    using Ext.Net;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Web.UI;

    public class WindowConfig
    {
        public WindowConfig()
        {
            this.ExtraParams = new ParameterCollection();
        }

        [NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Category("2. Observable"), ConfigOption("extraParams", JsonMode.Object), Description("额外参数"), Meta, DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ParameterCollection ExtraParams { get; private set; }

        public int Height { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public int Width { get; set; }
    }
}

