namespace Xinchen.PrivilegeManagement.DefaultProvider
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Runtime.CompilerServices;
    using Xinchen.DbEntity;
    using Xinchen.PrivilegeManagement.DefaultProvider.Model;

    public class ProviderContext : EntityContext
    {
        //private static Dictionary<Type, object> _entitySets = new Dictionary<Type, object>();
        private static ProviderContext _instance;
        private static object _syncLock = new object();

        private ProviderContext()
            : base(ConfigurationManager.AppSettings["PrivilegeProviderConnection"])
        {
            //this.Privileges = new EntitySet<Privilege>("EGOL");
            //this.Departments = new EntitySet<Department>("EGOL");
            //this.DepartmentPrivileges = new EntitySet<DepartmentPrivilege>("EGOL");
            //this.Menus = new EntitySet<Menu>("EGOL");
            //this.Roles = new EntitySet<Role>("EGOL");
            //this.RoleMenus = new EntitySet<RoleMenu>("EGOL");
            //this.RolePrivileges = new EntitySet<RolePrivilege>("EGOL");
            //this.Users = new EntitySet<User>("EGOL");
            //this.UserRoles = new EntitySet<UserRole>("EGOL");
            //_entitySets.Add(typeof(Privilege), this.Privileges);
            //_entitySets.Add(typeof(Department), this.Departments);
            //_entitySets.Add(typeof(DepartmentPrivilege), this.DepartmentPrivileges);
            //_entitySets.Add(typeof(Menu), this.Menus);
            //_entitySets.Add(typeof(Role), this.Roles);
            //_entitySets.Add(typeof(RoleMenu), this.RoleMenus);
            //_entitySets.Add(typeof(RolePrivilege), this.RolePrivileges);
            //_entitySets.Add(typeof(User), this.Users);
            //_entitySets.Add(typeof(UserRole), this.UserRoles);
        }

        public static ProviderContext GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new ProviderContext();
                    }
                }
            }
            return _instance;
        }

        public EntitySet<DepartmentPrivilege> DepartmentPrivileges { get; private set; }

        public EntitySet<Department> Departments { get; private set; }

        public EntitySet<Menu> Menus { get; private set; }

        public EntitySet<Privilege> Privileges { get; private set; }

        public EntitySet<RoleMenu> RoleMenus { get; private set; }

        public EntitySet<RolePrivilege> RolePrivileges { get; private set; }

        public EntitySet<Role> Roles { get; private set; }

        public EntitySet<UserRole> UserRoles { get; private set; }

        public EntitySet<User> Users { get; private set; }

    }
}

