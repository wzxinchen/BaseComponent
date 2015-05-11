using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.PrivilegeManagement.Provider;

namespace Xinchen.PrivilegeManagement.DefaultProvider.Provider
{
    public class RolePrivilegeProvider : IEntityProvider<PrivilegeManagement.Model.RolePrivilege>
    {
        private ProviderContext _context = ProviderContext.GetInstance();
        private static object _syncRoot = new object();
        private static RolePrivilegeProvider _instance;
        public static RolePrivilegeProvider GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new RolePrivilegeProvider();
                    }
                }
            }

            return _instance;
        }

        private RolePrivilegeProvider()
        {
        }

        public bool Exist(int id)
        {
            return _context.EntitySet<Model.RolePrivilege>().ExistsById(id);
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public int GetUniqueId()
        {
            throw new NotImplementedException();
        }

        public PrivilegeManagement.Model.RolePrivilege Add(PrivilegeManagement.Model.RolePrivilege entity)
        {
            throw new NotImplementedException();
        }

        public PrivilegeManagement.Model.RolePrivilege Get(int id)
        {
            throw new NotImplementedException();
        }

        public PrivilegeManagement.Model.RolePrivilege Get(Action<PrivilegeManagement.Model.RolePrivilege> condition)
        {
            throw new NotImplementedException();
        }

        public void Update(PrivilegeManagement.Model.RolePrivilege entity)
        {
            throw new NotImplementedException();
        }


        public List<PrivilegeManagement.Model.RolePrivilege> GetList(Action<PrivilegeManagement.Model.RolePrivilege> condition)
        {
            throw new NotImplementedException();
        }
    }
}
