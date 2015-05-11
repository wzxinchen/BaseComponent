using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.PrivilegeManagement.Provider;

namespace Xinchen.PrivilegeManagement.DefaultProvider.Provider
{
    public class RoleMenuProvider:IEntityProvider<PrivilegeManagement.Model.RoleMenu>
    {
        private ProviderContext _context = ProviderContext.GetInstance();
        private static object _syncRoot = new object();
        private static RoleMenuProvider _instance;
        public static RoleMenuProvider GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new RoleMenuProvider();
                    }
                }
            }

            return _instance;
        }

        private RoleMenuProvider()
        {
            
        }
        public bool Exist(int id)
        {
            throw new NotImplementedException();
        }


        public PrivilegeManagement.Model.RoleMenu Get(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(PrivilegeManagement.Model.RoleMenu entity)
        {
            throw new NotImplementedException();
        }


        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public int GetUniqueId()
        {
            throw new NotImplementedException();
        }

        public PrivilegeManagement.Model.RoleMenu Add(PrivilegeManagement.Model.RoleMenu entity)
        {
            throw new NotImplementedException();
        }

        public PrivilegeManagement.Model.RoleMenu Get(Action<PrivilegeManagement.Model.RoleMenu> condition)
        {
            throw new NotImplementedException();
        }


        public List<PrivilegeManagement.Model.RoleMenu> GetList(Action<PrivilegeManagement.Model.RoleMenu> condition)
        {
            throw new NotImplementedException();
        }
    }
}
