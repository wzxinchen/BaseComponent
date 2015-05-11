using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.PrivilegeManagement.Model;
using Xinchen.PrivilegeManagement.Provider;
using Xinchen.Utils.Entity;

namespace Xinchen.PrivilegeManagement.DefaultProvider.Provider
{
    public class UserRoleProvider : IEntityProvider<PrivilegeManagement.Model.UserRole>
    {
        private ProviderContext _context = ProviderContext.GetInstance();
        private static object _syncRoot = new object();
        private static UserRoleProvider _instance;
        public static UserRoleProvider GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new UserRoleProvider();
                    }
                }
            }

            return _instance;
        }

        private UserRoleProvider()
        {

        }
        public bool Exist(int id)
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

        public PrivilegeManagement.Model.UserRole Add(PrivilegeManagement.Model.UserRole entity)
        {
            Model.UserRole userRole = new Model.UserRole();
            userRole.RoleId = entity.RoleId;
            userRole.UserId = entity.UserId;
            _context.UserRoles.Create(userRole);
            return entity;
        }

        public PrivilegeManagement.Model.UserRole Get(int id)
        {
            throw new NotImplementedException();
        }

        public PrivilegeManagement.Model.UserRole Get(Action<PrivilegeManagement.Model.UserRole> condition)
        {
            throw new NotImplementedException();
        }

        public void Update(PrivilegeManagement.Model.UserRole entity)
        {
            throw new NotImplementedException();
        }


        public List<PrivilegeManagement.Model.UserRole> GetList(Action<PrivilegeManagement.Model.UserRole> condition)
        {
            var userRoleRemote = DynamicProxy.CreateDynamicProxy<UserRole>();
            condition(userRoleRemote);
            var dict = DynamicProxy.GetModifiedProperties(userRoleRemote);
            var listLocal = _context.UserRoles.GetBy(dict);
            var listRemote = new List<PrivilegeManagement.Model.UserRole>();
            foreach (var userRole in listLocal)
            {
                PrivilegeManagement.Model.UserRole userRemote = new UserRole();
                EntityMapper.Map(userRole, userRemote);
                listRemote.Add(userRemote);
            }
            return listRemote;
        }
    }
}
