using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net;
using Xinchen.ExtNetBase.TreePanelEx;

namespace Xinchen.ApplicationBase
{
    public class MenuNodeHelper : INodeHelper
    {
        private Privilege _privilege;

        public MenuNodeHelper(Privilege privilege)
        {
            _privilege = privilege;
        }
        public List<Xinchen.ExtNetBase.TreePanelEx.Node> GetNodeItems(int parentId)
        {
            return _privilege.GetMenus(parentId);
        }

        public void ChangeParent(string[] sources, int target)
        {
            _privilege.ChangeMenusParent(sources, target);
        }

        public int GetChildCount(int nodeId)
        {
            throw new NotImplementedException();
        }

        public void RemoveNode(int nodeId)
        {
            throw new NotImplementedException();
        }

        public bool Exists(int id, string name)
        {
            throw new NotImplementedException();
        }

        public void CreateNode(string name, int parentId)
        {
            throw new NotImplementedException();
        }

        public void UpdateNode(int id, string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Ext.Net.ModelField> CustomFields
        {
            get { return new List<ModelField>(); }
        }
    }
}
