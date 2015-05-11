using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.PrivilegeManagement.Provider;
using Xinchen.Utils.Entity;

namespace Xinchen.PrivilegeManagement.DefaultProvider.Provider
{
    public class PrivilegeProvider : IEntityProvider<PrivilegeManagement.Model.Privilege>
    {
        private ProviderContext _context = ProviderContext.GetInstance();
        private static object _syncLock = new object();
        private static PrivilegeProvider _instance;

        public static PrivilegeProvider GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new PrivilegeProvider();
                    }
                }
            }
            return _instance;
        }

        private PrivilegeProvider()
        {
        }


        public bool Exist(int id)
        {
            return _context.Privileges.ExistsById(id);
        }

        public bool IsEmpty()
        {
            return _context.Privileges.IsEmpty();
        }

        public int GetUniqueId()
        {
            throw new NotImplementedException();
        }

        public PrivilegeManagement.Model.Privilege Add(PrivilegeManagement.Model.Privilege entity)
        {
            var priviege = _context.Privileges.CreateProxy();
            EntityMapper.Map(entity, priviege);
            _context.Privileges.Create(priviege);
            return _context.CreateProxy(entity);
        }

        public PrivilegeManagement.Model.Privilege Get(int id)
        {
            var privilege = _context.Privileges.GetById(id);
            if (privilege == null)
            {
                return null;
            }
            return _context.CreateProxy(EntityMapper.Map<Model.Privilege, PrivilegeManagement.Model.Privilege>(privilege));
        }

        public PrivilegeManagement.Model.Privilege Get(Action<PrivilegeManagement.Model.Privilege> condition)
        {
            var priviegeRemote = _context.CreateProxy<PrivilegeManagement.Model.Privilege>();
            condition(priviegeRemote);
            var dict = _context.GetModifiedProperties(priviegeRemote);
            var privilegeLocal = _context.Privileges.GetBy(dict).FirstOrDefault();
            if (privilegeLocal == null)
            {
                return null;
            }
            return _context.CreateProxy(EntityMapper.Map<Model.Privilege, PrivilegeManagement.Model.Privilege>(privilegeLocal));
        }

        public void Update(PrivilegeManagement.Model.Privilege entity)
        {
            var privilege = _context.Privileges.CreateProxy();
            EntityMapper.Map(entity, privilege);
            _context.Privileges.Update(privilege);
        }


        public List<PrivilegeManagement.Model.Privilege> GetList(Action<PrivilegeManagement.Model.Privilege> condition)
        {
            throw new NotImplementedException();
        }
    }
}
