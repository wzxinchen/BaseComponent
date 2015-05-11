namespace Xinchen.PrivilegeManagement.Provider
{
    using System;
using System.Collections.Generic;
using Xinchen.DbUtils;
using Xinchen.PrivilegeManagement.DTO;

    public interface IPrivilegeContextProvider
    {
        IRepository GetRepository();
        //TModel GetModelBySql<TModel>(string sql, params object[] parameters);
        //IList<TModel> GetModelsBySql<TModel>(string sql, params object[] parameters);
        //IList<TModel> GetModelsBySql<TModel>(string sql, string sort, int page, int pageSize, out int recordCount, params object[] parameters);

        //IEntityRepository<Department> DepartmentProvider { get; }

        bool EnableMenuEdit { get; }

        string HomePage { get; }

        string LoginPage { get; }
        //IRepository GetRepository();

        //IEntityRepository<Menu> GetMenuRepository();

        //IEntityRepository<Privilege> GetPrivilegeRepository();

        string RegisterAdminPage { get; }

        //IEntityRepository<RoleMenu> GetRoleMenuRepository();

        //IEntityRepository<RolePrivilege> GetRolePrivilegeRepository();

        //IEntityRepository<Role> GetRoleRepository();

        string SystemRoleName { get; }

        //IEntityRepository<User> GetUserRepository();

        //IEntityRepository<UserRole> GetUserRoleRepository();
    }
}

