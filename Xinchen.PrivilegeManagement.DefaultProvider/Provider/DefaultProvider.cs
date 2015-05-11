namespace Xinchen.PrivilegeManagement.DefaultProvider.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text.RegularExpressions;
    using Xinchen.ObjectMapper;
    using Xinchen.PrivilegeManagement.DefaultProvider;
    using Xinchen.PrivilegeManagement.DTO;
    using Xinchen.PrivilegeManagement.Provider;

    [Serializable]
    public class DefaultProvider : IPrivilegeContextProvider
    {
        private ProviderContext _context = ProviderContext.GetInstance();

        public TModel GetModelBySql<TModel>(string sql, params object[] parameters)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();
            MatchCollection matchs = Regex.Matches(sql, @"@(\w+)");
            for (int i = 0; i < matchs.Count; i++)
            {
                list.Add(this._context.CreateParameter<object>(matchs[i].Groups[1].Value, parameters[i]));
            }
            return this._context.GetModel<TModel>(sql, list.ToArray());
        }

        public IList<TModel> GetModelsBySql<TModel>(string sql, params object[] parameters)
        {
            return this._context.GetModels<TModel>(sql, parameters);
        }

        public IList<TModel> GetModelsBySql<TModel>(string sql, string sort, int page, int pageSize, out int recordCount, params object[] parameters)
        {
            List<IDbDataParameter> list = new List<IDbDataParameter>();
            MatchCollection matchs = Regex.Matches(sql, @"@(\w+)");
            for (int i = 0; i < parameters.Length; i++)
            {
                list.Add(this._context.CreateParameter<object>(matchs[i].Groups[1].Value, parameters[i]));
            }
            //recordCount = this._context.DbHelper.ExecuteCount("select count(1) from(" + sql + ") ttt", list.ToArray());
            //if (string.IsNullOrEmpty(sort))
            //{
            //    sort = "id";
            //}
            //string[] strArray = new string[] { "select * from (SELECT ROW_NUMBER() OVER ( ORDER BY ", sort, " ) indexer ,* from (", sql, ") t) tt where tt.indexer>", ((page - 1) * pageSize).ToString(), " and tt.indexer<=", (page * pageSize).ToString() };
            //string str = string.Concat(strArray);
            DataSet ds = _context.DbHelper.ExecutePager(sql, sort, page, pageSize, out recordCount, list.ToArray());
            return Mapper.MapList<TModel>(ds);
            //return this._context.GetModels<TModel>(str, list.ToArray());
        }

        public IEntityProvider<DepartmentPrivilege> DepartmentPrivilegeProvider
        {
            get
            {
                return EntityProvider<DepartmentPrivilege, Model.DepartmentPrivilege>.GetInstance();
            }
        }

        public IEntityProvider<Department> DepartmentProvider
        {
            get
            {
                return EntityProvider<Department, Model.Department>.GetInstance();
            }
        }

        public bool EnableMenuEdit
        {
            get
            {
                return true;
            }
        }

        public string HomePage
        {
            get
            {
                return "~/index.aspx";
            }
        }

        public IEntityProvider<Menu> MenuProvider
        {
            get
            {
                return EntityProvider<Menu, Model.Menu>.GetInstance();
            }
        }

        public IEntityProvider<Privilege> PrivilegeProvider
        {
            get
            {
                return EntityProvider<Privilege, Model.Privilege>.GetInstance();
            }
        }

        public IEntityProvider<RoleMenu> RoleMenuProvider
        {
            get
            {
                return EntityProvider<RoleMenu, Model.RoleMenu>.GetInstance();
            }
        }

        public IEntityProvider<RolePrivilege> RolePrivilegeProvider
        {
            get
            {
                return EntityProvider<RolePrivilege, Model.RolePrivilege>.GetInstance();
            }
        }

        public IEntityProvider<Role> RoleProvider
        {
            get
            {
                return EntityProvider<Role, Model.Role>.GetInstance();
            }
        }

        public string SystemRoleName
        {
            get
            {
                return "系统角色";
            }
        }

        public IEntityProvider<User> UserProvider
        {
            get
            {
                return EntityProvider<User, Model.User>.GetInstance();
            }
        }

        public IEntityProvider<UserRole> UserRoleProvider
        {
            get
            {
                return EntityProvider<UserRole, Model.UserRole>.GetInstance();
            }
        }

        string IPrivilegeContextProvider.LoginPage
        {
            get
            {
                return "~/account/login.aspx";
            }
        }

        string IPrivilegeContextProvider.RegisterAdminPage
        {
            get
            {
                return "~/account/registerAdmin.aspx";
            }
        }
    }
}

