namespace Xinchen.ExtNetBase.TreePanelEx
{
    using Ext.Net;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class Node
    {
        public Node()
        {
            this.CustomAttributes = new List<ConfigItem>();
        }

        public List<ConfigItem> CustomAttributes { get; private set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }
    }
}

