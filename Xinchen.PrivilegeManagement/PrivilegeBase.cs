namespace Xinchen.PrivilegeManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Transactions;
    using System.Web;
    using Xinchen.PrivilegeManagement.Enums;
    using Xinchen.PrivilegeManagement.DTO;
    using Xinchen.PrivilegeManagement.Provider;
    using Xinchen.PrivilegeManagement.ViewModel;
    using Xinchen.Utils;
    using Xinchen.Utils.DataAnnotations;
    using System.Linq.Expressions;
    using Xinchen.DbUtils;
    using Xinchen.ObjectMapper;
    public class PrivilegeBase
    {
        private HttpContext _httpContext = HttpContext.Current;
        private UserSessionModel _userInfo;

        public Department AddDepartment(string name, string description)
        {
            Department entity;
            using (TransactionScope scope = new TransactionScope())
            {
                var db = PrivilegeContextProvider.GetRepository();
                using (db)
                {
                    entity = new Department
                    {
                        Description = description,
                        Name = name,
                        Id = db.Use<Department>().GetSequenceId(),
                        Status = (int)BaseStatuses.Valid
                    };
                    var departmentUnit = db.Use<Department>();
                    departmentUnit.Add(entity);
                    db.SaveChanges();
                }
                scope.Complete();
            }
            return entity;
        }

        public Role AddRole(string name, BaseStatuses status, string description, int? departmentId = new int?())
        {
            Role entity;
            using (TransactionScope scope = new TransactionScope())
            {
                using (var reps = PrivilegeContextProvider.GetRepository())
                {
                    entity = new Role
                    {
                        Name = name,
                        DepartmentId = departmentId,
                        Status = status,
                        Id = reps.Use<Role>().GetSequenceId(),
                        Description = description
                    };
                    entity = reps.Use<Role>().Add(entity);
                    reps.SaveChanges();
                }
                scope.Complete();
            }
            return entity;
        }

        public void AddRoleMenu(int roleId, int menuId)
        {
            RoleMenu entity = new RoleMenu
            {
                RoleId = roleId,
                MenuId = menuId
            };
            this.PrivilegeContextProvider.GetRepository().Use<RoleMenu>().AddAndSaveChanges(entity);
        }

        public void AddRolePrivilege(int roleId, int privilegeId)
        {
            RolePrivilege entity = new RolePrivilege
            {
                PrivilegeId = privilegeId,
                RoleId = roleId
            };
            using (var reps = PrivilegeContextProvider.GetRepository())
            {
                reps.Use<RolePrivilege>().Add(entity);
                reps.SaveChanges();
            }
        }

        public User AddUser(string username, string description, BaseStatuses enabled)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                User entity = new User
                {
                    Username = username,
                    Status = enabled,
                    Password = StringHelper.EncodePassword(username, "123456"),
                    Id = this.PrivilegeContextProvider.GetRepository().Use<User>().GetSequenceId(),
                    CreateTime = DateTime.Now,
                    Description = description
                };
                var reps = this.PrivilegeContextProvider.GetRepository();
                reps.Use<User>().Add(entity);
                reps.SaveChanges();
                scope.Complete();
                return entity;
            }
        }

        public void AddUserRole(int roleId, int userId)
        {
            UserRole entity = new UserRole
            {
                RoleId = roleId,
                UserId = userId
            };
            var reps = ConfigManager.PrivilegeProvider.GetRepository();
            reps.Use<UserRole>().Add(entity);
            reps.SaveChanges();
        }

        public void ChangePassword(string oldPwd, string pwd, string newpwd)
        {
            if (this._userInfo == null)
            {
                throw new ApplicationException("修改密码之前请先登录");
            }
            using (TransactionScope scope = new TransactionScope())
            {
                var reps = PrivilegeContextProvider.GetRepository();
                User entity = reps.Use<User>().Get(x => x.Id == _userInfo.Id);
                if (entity == null)
                {
                    throw new ApplicationException("当前登录用户不存在");
                }
                if (pwd != newpwd)
                {
                    throw new ApplicationException("确认密码与新密码不一致");
                }
                if (StringHelper.EncodePassword(this._userInfo.Username, oldPwd) != entity.Password)
                {
                    throw new ApplicationException("旧密码不正确");
                }
                entity.Password = StringHelper.EncodePassword(this._userInfo.Username, pwd);
                reps.SaveChanges();
                scope.Complete();
            }
        }

        /// <summary>
        /// 检查是否首次启动
        /// </summary>
        public static void CheckFirstStart()
        {
            if (!ConfigManager.PrivilegeProvider.GetRepository().Use<User>().Any(x => true))
            {
                HttpRuntime.Cache["FirstStart"] = true;
            }
        }

        public void CheckLoginStatus()
        {
            if (IsRegisterPage())
            {
                return;
            }
            if (this._httpContext.Session["UserInfo"] == null)
            {
                if (!this.IsLoginPage() && !IsVerifyImagePage())
                {
                    string loginPage = ConfigManager.PrivilegeProvider.LoginPage;
                    if (loginPage.Contains("{url}"))
                    {
                        loginPage = loginPage.Replace("{url}", this._httpContext.Request.Url.ToString());
                    }
                    this._httpContext.Response.Redirect(loginPage, true);
                }
            }
            else if (this.IsLoginPage())
            {
                string homePage = this._httpContext.Request.Url.ToString();
                if (string.IsNullOrEmpty(homePage) || this.IsLoginPage(homePage))
                {
                    homePage = ConfigManager.PrivilegeProvider.HomePage;
                }
                this._httpContext.Response.Redirect(homePage, true);
            }
        }

        public bool CheckPrivilege(int privilege)
        {
            this._userInfo = (UserSessionModel)this._httpContext.Session["UserInfo"];
            if (privilege == -1)
            {
                return true;
            }
            if (privilege == 0)
            {
                if (this._userInfo == null)
                {
                    return false;
                }
                return true;
            }
            if (this._httpContext.Session["UserInfo"] == null)
            {
                return false;
            }
            if (_userInfo.Roles.Values.Count<Role>(x => (x.Name == this.PrivilegeContextProvider.SystemRoleName)) > 0)
            {
                return true;
            }
            return _userInfo.Privileges.ContainsKey(privilege);
        }

        public void DeleteUser(params int[] userId)
        {
            var reps = this.PrivilegeContextProvider.GetRepository();
            reps.Use<User>().Remove(x => userId.Contains(x.Id));
            reps.SaveChanges();
            reps.Dispose();
        }

        public void Disable(params int[] userId)
        {
            var reps = this.PrivilegeContextProvider.GetRepository();
            reps.Use<User>().Update(x => userId.Contains(x.Id), x => new User()
            {
                Status = BaseStatuses.Invalid
            });
            reps.SaveChanges();
            reps.Dispose();
        }

        public void DisableRole(int[] rolesId)
        {
            var reps = this.PrivilegeContextProvider.GetRepository();
            reps.Use<Role>().Update(x => rolesId.Contains(x.Id), x => new Role()
            {
                Status = BaseStatuses.Invalid
            });
            reps.SaveChanges();
            reps.Dispose();
        }

        public void Enable(params int[] userId)
        {
            var reps = this.PrivilegeContextProvider.GetRepository();
            reps.Use<User>().Update(x => userId.Contains(x.Id), x => new User()
            {
                Status = BaseStatuses.Valid
            });
            reps.SaveChanges();
            reps.Dispose();
        }

        public void EnableRole(int[] rolesId)
        {
            var reps = this.PrivilegeContextProvider.GetRepository();
            reps.Use<Role>().Update(x => rolesId.Contains(x.Id), x => new Role()
            {
                Status = BaseStatuses.Valid
            });
            reps.SaveChanges();
            reps.Dispose();
        }

        public Menu GetMenu(int menuId)
        {
            return this.PrivilegeContextProvider.GetRepository().Use<Menu>().Get(x => x.Id == menuId);
        }

        public Privilege GetMenuPrivilege(int menuId)
        {
            return this.PrivilegeContextProvider.GetRepository().Use<Privilege>().Get(x => x.Id == menuId);
        }

        public IList<Menu> GetMenus(int parentId)
        {
            return ConfigManager.PrivilegeProvider.GetRepository().Use<Menu>().GetList(x => x.ParentId == parentId);
        }

        public IList<UserMenu> GetNavigationMenus(int parentId)
        {
            IList<UserMenu> list;
            string sql = string.Empty;
            var repo = ConfigManager.PrivilegeProvider.GetRepository();
            if (this._userInfo.Roles.Count<KeyValuePair<int, Role>>(x => (x.Value.Name == ConfigManager.PrivilegeProvider.SystemRoleName)) > 0)
            {
                var menus = repo.Use<Menu>().GetList(x => x.Status == BaseStatuses.Valid);
                list = (from menu in menus
                        where menu.ParentId == parentId
                        select new UserMenu()
                        {
                            ChildCount = (from subMenu in menus where subMenu.ParentId == menu.Id select subMenu).Count(),
                            Description = menu.Description,
                            Id = menu.Id,
                            Name = menu.Name,
                            ParentId = menu.ParentId,
                            Sort = menu.Sort,
                            Status = menu.Status,
                            Url = menu.Url
                        }).ToList();
                //sql = "SELECT menus.Id,\r\n                    menus.Name,menus.Url,menus.Description,menus.ParentId,menus.Sort,menus.Status,\r\n                    (SELECT COUNT(1) FROM dbo.Menus childs WHERE childs.ParentId=menus.Id) ChildCount\r\n                    FROM dbo.Menus menus WHERE menus.Status=" + (int)BaseStatuses.Valid + @" and menus.ParentId=" + parentId.ToString() + " order by sort";
                //list = ConfigManager.PrivilegeProvider.GetRepository().Queries<UserMenu>(sql);
            }
            else
            {
                list = new List<UserMenu>();
                if (UserInfo.Roles.Count > 0)
                {
                    var menus = from menu in repo.Use<Menu>().Query
                                join roleprivilege in repo.Use<RolePrivilege>().Query on menu.PrivilegeId equals roleprivilege.PrivilegeId
                                select new
                                {
                                    roleprivilege.RoleId,
                                    menu.Id,
                                    menu.Name,
                                    menu.ParentId,
                                    menu.PrivilegeId,
                                    menu.Sort,
                                    menu.Status,
                                    menu.Url,
                                    menu.Description
                                };
                    list = (from menu in menus
                            where UserInfo.Roles.Keys.Contains(menu.RoleId) && menu.ParentId == parentId
                            select new UserMenu()
                            {
                                ChildCount = (from subMenu in menus where subMenu.ParentId == menu.Id select subMenu).Count(),
                                Description = menu.Description,
                                Id = menu.Id,
                                Name = menu.Name,
                                ParentId = menu.ParentId,
                                Sort = menu.Sort,
                                Status = menu.Status,
                                Url = menu.Url
                            }).ToList();
                    //sql = "SELECT  menus.Id ,\r\n        menus.Name ,\r\n        menus.Sort ,\r\n        menus.ParentId ,\r\n        menus.Description ,\r\n        menus.Url ,\r\n        menus.Status ,\r\n        menus.Level,\r\n                     (SELECT COUNT(1) FROM dbo.Menus childs WHERE childs.ParentId=menus.Id) ChildCount\r\nFROM    dbo.RolePrivileges rolePrivileges\r\n        INNER JOIN dbo.Menus menus ON menus.PrivilegeId = rolePrivileges.PrivilegeId\r\n        WHERE rolePrivileges.RoleId in(" + string.Join<int>(",", this.UserInfo.Roles.Keys) + ") order by sort";
                    //IList<UserMenu> list2 = ConfigManager.PrivilegeProvider.GetRepository().Queries<UserMenu>(sql);
                    //foreach (UserMenu menu in list2)
                    //{
                    //    if (menu.ParentId == parentId)
                    //    {
                    //        list.Add(menu);
                    //    }
                    //    else
                    //    {
                    //        UserMenu modelBySql = this.PrivilegeContextProvider.GetRepository().Queries<UserMenu>("SELECT  menus.Id ,\r\n        menus.Name ,\r\n        menus.Sort ,\r\n        menus.ParentId ,\r\n        menus.Description ,\r\n        menus.Url ,\r\n        menus.Status ,\r\n        menus.Level,\r\n                   (SELECT COUNT(1) FROM dbo.Menus childs WHERE childs.ParentId=menus.Id) ChildCount\r\nFROM   dbo.Menus menus where Id=" + menu.ParentId.ToString()).FirstOrDefault();
                    //        if ((modelBySql != null) && (modelBySql.ParentId == parentId))
                    //        {
                    //            list.Add(modelBySql);
                    //        }
                    //    }
                    //}
                }
            }
            return (from x in list
                    orderby x.Sort
                    select x).ToList<UserMenu>();
        }

        public Privilege GetPrivilege(int id)
        {
            return this.PrivilegeContextProvider.GetRepository().Use<Privilege>().Get(x => x.Id == id);
        }

        public IList<Privilege> GetPrivileges()
        {
            return this.PrivilegeContextProvider.GetRepository().Use<Privilege>().GetList(x => true);
        }

        public Role GetRole(int id)
        {
            return this.PrivilegeContextProvider.GetRepository().Use<Role>().Get(x => x.Id == id);
        }

        public Role GetRole(string name)
        {
            return this.PrivilegeContextProvider.GetRepository().Use<Role>().Get(x => x.Name == name);
        }

        public IList<RoleMenu> GetRoleMenus(int roleId)
        {
            return this.PrivilegeContextProvider.GetRepository().Use<RoleMenu>().GetList(x => x.RoleId == roleId);
        }

        public IList<Privilege> GetRoleNotHavePrivileges(int _roleId)
        {
            return this.PrivilegeContextProvider.GetRepository().Queries<Privilege>("SELECT Id ,\r\n         Name ,\r\n         Description  FROM dbo.Privileges WHERE Id NOT IN (SELECT    rolePrivileges.PrivilegeId\r\n  FROM      dbo.Privileges privileges\r\n            INNER JOIN dbo.RolePrivileges rolePrivileges ON rolePrivileges.PrivilegeId = privileges.Id\r\n  WHERE     rolePrivileges.RoleId = " + _roleId + ")");
        }

        public IList<Privilege> GetRolePrivileges(int _roleId)
        {
            return this.PrivilegeContextProvider.GetRepository().Queries<Privilege>(" SELECT privileges.Id ,\r\n         privileges.Name ,\r\n         privileges.Description ,\r\n         rolePrivileges.Id ,\r\n         rolePrivileges.RoleId ,\r\n         rolePrivileges.PrivilegeId FROM dbo.Privileges privileges\r\n  INNER JOIN dbo.RolePrivileges rolePrivileges ON rolePrivileges.PrivilegeId = privileges.Id\r\n  WHERE rolePrivileges.RoleId=" + _roleId);
        }

        public IList<Role> GetRoles()
        {
            return this.PrivilegeContextProvider.GetRepository().Use<Role>().GetList(x => true);
        }

        public IList<RolePrivilegeModel> GetRoles(int page, int pageSize, string condition, string sort, out int recordCount, params object[] parameters)
        {
            string sql = "SELECT DISTINCT roles.Id,\r\n       roles.Name,\r\n       roles.Status ,roles.Description,       ( STUFF(( SELECT    ',' + privileges1.Description\r\n                 FROM      dbo.Roles roles1\r\n       left JOIN dbo.RolePrivileges rolePrivileges1 ON rolePrivileges1.RoleId = roles.Id\r\n       left JOIN dbo.Privileges privileges1 ON privileges1.Id = rolePrivileges1.PrivilegeId\r\n       WHERE roles1.Id=roles.Id\r\n               FOR\r\n                 XML PATH('')\r\n               ), 1, 1, '') ) privileges\r\nFROM    dbo.Roles roles\r\n       left JOIN dbo.RolePrivileges rolePrivileges ON rolePrivileges.RoleId = roles.Id\r\n       left JOIN dbo.Privileges privileges ON privileges.Id = rolePrivileges.PrivilegeId";
            if (!string.IsNullOrEmpty(condition))
            {
                sql = sql + " where " + condition;
            }
            recordCount = -1;
            return this.PrivilegeContextProvider.GetRepository().Queries<RolePrivilegeModel>(sql);
        }

        public User GetUser(int _userId)
        {
            return this.PrivilegeContextProvider.GetRepository().Use<User>().Get(x => x.Id == _userId);
        }

        public User GetUser(string username)
        {
            return this.PrivilegeContextProvider.GetRepository().Use<User>().Get(x => x.Username == username);
        }

        public IList<Role> GetUserNotHaveRoles(int _userId)
        {
            var repo = PrivilegeContextProvider.GetRepository();
            var roleIds = repo.Use<UserRole>().GetList(x => x.UserId == _userId).Select(x => x.RoleId);
            return repo.Use<Role>().GetList(x => !roleIds.Contains(x.Id));
            //return this.PrivilegeContextProvider.GetRepository().Queries<Role>("SELECT  roles.Id ,\r\n        roles.Name ,\r\n        roles.DepartmentId ,\r\n        roles.Status ,\r\n        roles.Description\r\nFROM    dbo.Roles roles\r\nWHERE   roles.Id NOT IN ( SELECT    RoleId\r\n                          FROM      dbo.UserRoles\r\n                          WHERE     UserId = " + _userId + ")");
        }

        public IList<Role> GetUserRoles(int _userId)
        {
            var repo = PrivilegeContextProvider.GetRepository();
            var roleIds = repo.Use<UserRole>().GetList(x => x.UserId == _userId).Select(x => x.RoleId);
            return repo.Use<Role>().GetList(x => roleIds.Contains(x.Id));
        }


        public IList<UserRole> GetUserRoles()
        {
            return this.PrivilegeContextProvider.GetRepository().Use<UserRole>().Query.ToList();
        }

        public PageResult<UserRoleDetailInfo> GetUsers(int page, int limit, IList<SqlFilter> filters, params Sort[] sorts)
        {
            var repo = PrivilegeContextProvider.GetRepository();
            //var pr = repo.Use<User>().Query.Where(filters).Page(page, limit);
            var map = new Dictionary<string, string>();
            map.Add("Roles", "RoleId");
            return (from user in repo.Use<User>().Query
                    join userRole in repo.Use<UserRole>().Query on user.Id equals userRole.UserId into us
                    from u in us.DefaultIfEmpty()
                    select new UserRoleDetailInfo()
                    {
                        CreateTime = user.CreateTime,
                        Description = user.Description,
                        Id = user.Id,
                        RoleId = u.RoleId,
                        Status = user.Status,
                        Username = user.Username
                    }).Where(filters, map).OrderBy(sorts).Distinct().Page(page, limit);
            //            string sql = @"SELECT DISTINCT users.Id ,
            //                    Username ,
            //        users. CreateTime,
            //        ( STUFF(( SELECT    ',' + roles1.Name\r\n                 FROM      dbo.Users users1\r\n        left JOIN dbo.UserRoles userRoles1 ON userRoles1.UserId = users1.Id\r\n        left JOIN dbo.Roles roles1 ON roles1.Id = userRoles1.RoleId\r\n       WHERE users.Id=users1.Id\r\n               FOR\r\n                 XML PATH('')\r\n               ), 1, 1, '') ) Roles,\r\n         users.Status,users.Description\r\nFROM    dbo.Users users\r\n        left JOIN dbo.UserRoles userRoles ON userRoles.UserId = users.Id\r\n        left JOIN dbo.Roles roles ON roles.Id = userRoles.RoleId";
            //            if (!string.IsNullOrEmpty(condition))
            //            {
            //                sql += " where " + condition;
            //            }
            //            return this.PrivilegeContextProvider.GetModelsBySql<UserRoleInfo>(sql, sort, page, limit, out recordCount, parameters);
        }

        public bool IsLoginPage()
        {
            return this.IsLoginPage(this._httpContext.Request.Url.ToString());
        }

        public bool IsVerifyImagePage()
        {
            return _httpContext.Request.Url.ToString().ToLower().Contains("/account/verifyimage");
        }

        public bool IsLoginPage(string url)
        {
            return url.Contains(ConfigManager.PrivilegeProvider.LoginPage.Replace("~", ""));
        }

        public bool IsRegisterPage(string url)
        {
            return url.Contains(ConfigManager.PrivilegeProvider.RegisterAdminPage.Replace("~", ""));
        }

        public bool IsRegisterPage()
        {
            return IsRegisterPage(_httpContext.Request.Url.ToString());
        }

        public void Login(string username, string password)
        {
            Func<RolePrivilege, bool> predicate = null;
            UserSessionModel userInfo = null;
            using (TransactionScope scope = new TransactionScope())
            {
                string pwd = StringHelper.EncodePassword(username, password);
                using (var reps = PrivilegeContextProvider.GetRepository())
                {
                    //var reps = reps.Use<Privilege>();
                    // var userRoleReps = ConfigManager.PrivilegeProvider.GetRepository();
                    //var roleReps = ConfigManager.PrivilegeProvider.GetRepository();
                    //var rolePrivilegeReps = ConfigManager.PrivilegeProvider.GetRolePrivilegeRepository();
                    //var privilegeReps = ConfigManager.PrivilegeProvider.GetRepository();
                    User user = reps.Use<User>().Get(x => x.Username == username && x.Password == pwd);
                    if (user == null)
                    {
                        throw new ApplicationException("用户名或密码错误");
                    }
                    if (user.Status == BaseStatuses.Invalid)
                    {
                        throw new ApplicationException("用户已被禁用");
                    }
                    userInfo = new UserSessionModel
                    {
                        Id = user.Id,
                        Username = user.Username
                    };
                    IList<UserRole> list = reps.Use<UserRole>().GetList(x => x.UserId == user.Id);
                    foreach (UserRole role in list)
                    {
                        int roleId = role.RoleId;
                        Role role2 = reps.Use<Role>().Get(x => x.Id == role.RoleId);
                        if (role2.Status == BaseStatuses.Invalid)
                        {
                            throw new ApplicationException("用户所拥有的角色[" + role2.Name + "]被禁用，无法登录");
                        }
                        userInfo.Roles.Add(roleId, Mapper.Map<Role, Role>(role2));
                        IList<RolePrivilege> source = reps.Use<RolePrivilege>().GetList(x => x.RoleId == roleId);
                        if (predicate == null)
                        {
                            predicate = x => !userInfo.Privileges.ContainsKey(x.PrivilegeId);
                        }
                        foreach (RolePrivilege privilege in source.Where<RolePrivilege>(predicate))
                        {
                            int privilegeId = privilege.PrivilegeId;
                            Privilege privilege2 = reps.Use<Privilege>().Get(x => x.Id == privilegeId);
                            userInfo.Privileges.Add(privilegeId, Mapper.Map<Privilege, Privilege>(privilege2));
                        }
                    }
                }
            }

            HttpContext.Current.Session["UserInfo"] = userInfo;
        }

        public User RegAdmin(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("username");
            }
            var reps = PrivilegeContextProvider.GetRepository();
            using (reps)
            {
                var userUnit = reps.Use<User>();
                if (userUnit.Any(x => true))
                {
                    throw new ApplicationException("系统已存在用户，无法直接注册管理员");
                }
                var roleUnit = reps.Use<Role>();
                var role = new Role()
                {
                    Description = PrivilegeContextProvider.SystemRoleName,
                    Id = roleUnit.GetSequenceId(),
                    Name = PrivilegeContextProvider.SystemRoleName
                };
                roleUnit.Add(role);
                User entity = new User
                {
                    Password = StringHelper.EncodePassword(user.Username, user.Password),
                    Status = BaseStatuses.Valid,
                    CreateTime = user.CreateTime,
                    Description = user.Description,
                    Username = user.Username,
                    DepartmentId = user.DepartmentId,
                    Id = userUnit.GetSequenceId()
                };
                user = userUnit.Add(entity);
                UserRole userRole = new UserRole();
                userRole.RoleId = role.Id;
                userRole.UserId = entity.Id;
                reps.Use<UserRole>().Add(userRole);
                reps.SaveChanges();
                HttpRuntime.Cache.Remove("FirstStart");
                return user;
            }
        }

        /// <summary>
        /// 更新注册权限枚举
        /// </summary>
        /// <param name="enumType"></param>
        public static void RegisterPrivileges(Type enumType)
        {
            int num2;
            string[] names = Enum.GetNames(enumType);
            Array values = Enum.GetValues(enumType);
            int length = names.Length;
            var set = new HashSet<string>();
            for (num2 = 0; num2 < length; num2++)
            {
                if (set.Contains(names[num2]))
                {
                    throw new ArgumentOutOfRangeException("权限名称重复：" + names[num2]);
                }
                set.Add(names[num2]);
            }
            using (TransactionScope scope = new TransactionScope())
            {
                var privilegeProvider = ConfigManager.PrivilegeProvider.GetRepository();
                var sysRole = privilegeProvider.Use<Role>().Get(x => x.Name == ConfigManager.PrivilegeProvider.SystemRoleName);
                Dictionary<int, Privilege> sysRolePrivileges = new Dictionary<int, Privilege>();
                if (sysRole != null)
                {
                    sysRolePrivileges =
                        (from rp in privilegeProvider.Use<RolePrivilege>().Query
                         join p in privilegeProvider.Use<Privilege>().Query on rp.PrivilegeId equals p.Id
                         join r in privilegeProvider.Use<Role>().Query on rp.RoleId equals r.Id
                         where r.Name == sysRole.Name
                         select p).ToDictionary(x => x.Id);
                }
                for (num2 = 0; num2 < length; num2++)
                {
                    Privilege privilege;
                    string name = names[num2];
                    int id = (int)values.GetValue(num2);
                    PrivilegeDescriptionAttribute attribute = AttributeHelper.GetAttribute<PrivilegeDescriptionAttribute>(enumType.GetField(name));
                    if (privilegeProvider.Use<Privilege>().Any(x => x.Id == id))
                    {
                        privilege = privilegeProvider.Use<Privilege>().Get(x => x.Id == id);
                        if ((privilege.Name != name)
                            || ((attribute != null) && ((attribute.Description != privilege.Description))))
                        {
                            privilege.Description = (attribute == null) ? name : attribute.Description;
                            privilege.Name = name;
                        }
                    }
                    else
                    {
                        privilege = new Privilege
                        {
                            Description = (attribute == null) ? name : attribute.Description,
                            Name = name,
                            Id = id
                        };
                        privilegeProvider.Use<Privilege>().Add(privilege);
                    }
                    if (!sysRolePrivileges.ContainsKey(privilege.Id))
                    {
                        var rolePrivilege = new RolePrivilege()
                        {
                            RoleId = sysRole.Id,
                            PrivilegeId = privilege.Id
                        };
                        privilegeProvider.Use<RolePrivilege>().Add(rolePrivilege);
                    }
                }
                privilegeProvider.SaveChanges();
                scope.Complete();
            }
        }

        public void RemoveRoleMenu(int roleId, int menuId)
        {
            this.PrivilegeContextProvider.GetRepository().Use<RoleMenu>().Remove(x => x.RoleId == roleId && x.MenuId == menuId);
        }

        public void RemoveRolePrivilege(int privilegeId, int roleId)
        {
            var reps = this.PrivilegeContextProvider.GetRepository();
            reps.Use<RolePrivilege>().Remove(x => x.PrivilegeId == privilegeId && x.RoleId == roleId);
            reps.SaveChanges();
            reps.Dispose();
        }

        public void RemoveUserRole(int roleId, int userId)
        {
            var reps = ConfigManager.PrivilegeProvider.GetRepository();
            reps.Use<UserRole>().Remove(x => x.RoleId == roleId && x.UserId == userId);
            reps.SaveChanges();
            reps.Dispose();
        }

        public void ResetPassword(string password, params int[] userIds)
        {
            var reps = PrivilegeContextProvider.GetRepository();
            foreach (int num in userIds)
            {
                User entity = reps.Use<User>().Get(x => x.Id == num);
                if (entity != null)
                {
                    entity.Password = StringHelper.EncodePassword(entity.Username, password);
                }
            }
            reps.SaveChanges();
            reps.Dispose();
        }

        /// <summary>
        /// 如果是首次启动，则转入管理员注册页面
        /// </summary>
        public void Setup()
        {
            IPrivilegeContextProvider privilegeProvider = ConfigManager.PrivilegeProvider;
            if (HttpRuntime.Cache["FirstStart"] != null)
            {
                if (!HttpContext.Current.Request.Url.ToString().Contains(privilegeProvider.RegisterAdminPage.Replace("~", "")))
                {
                    HttpContext.Current.Response.Redirect(privilegeProvider.RegisterAdminPage, true);
                }
            }
            else if (HttpContext.Current.Request.Url.ToString().Contains(privilegeProvider.RegisterAdminPage.Replace("~", "")))
            {
                HttpContext.Current.Response.Redirect(privilegeProvider.LoginPage, true);
            }
        }

        public Role UpdateRole(Role role)
        {
            using (var reps = PrivilegeContextProvider.GetRepository())
            {
                reps.Use<Role>().Update(role);
                reps.SaveChanges();
            }
            return role;
        }

        public User UpdateUser(User user)
        {
            using (var reps = PrivilegeContextProvider.GetRepository())
            {
                reps.Use<User>().Update(user);
                reps.SaveChanges();
            }
            return user;
        }

        public IPrivilegeContextProvider PrivilegeContextProvider
        {
            get
            {
                return ConfigManager.PrivilegeProvider;
            }
        }

        public string SystemRoleName
        {
            get
            {
                return this.PrivilegeContextProvider.SystemRoleName;
            }
        }

        public UserSessionModel UserInfo
        {
            get
            {
                return this._userInfo;
            }
        }

        public void ChangeMenusParent(string[] sources, int target)
        {
            var ss = Array.ConvertAll(sources, Convert.ToInt32);
            PrivilegeContextProvider.GetRepository().Use<Menu>().Update(x => ss.Contains(x.Id), x => new Menu()
            {
                ParentId = target
            });
        }

        public void DeleteRoles(params int[] roleIds)
        {
            using (var reps = PrivilegeContextProvider.GetRepository())
            {
                reps.Use<Role>().Remove(x => roleIds.Contains(x.Id));
                reps.SaveChanges();
            }
        }
    }
}

