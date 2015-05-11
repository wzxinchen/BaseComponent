namespace Xinchen.ExtNetBase.TreePanelEx
{
    using Ext.Net;
    using System;
    using System.Collections.Generic;

    public interface INodeHelper
    {
        void ChangeParent(string[] sources, int target);
        void CreateNode(string name, int parentId);
        bool Exists(int id, string name);
        int GetChildCount(int nodeId);
        IList<Node> GetNodeItems(int parentId);
        void RemoveNode(int nodeId);
        void UpdateNode(int id, string name);

        IEnumerable<ModelField> CustomFields { get; }
    }
}

