namespace Xinchen.ApplicationBase.Mvc
{
    using Ext.Net;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Transactions;
    using Xinchen.ApplicationBase.Mvc.Model;
    using Xinchen.DbUtils;
    using Xinchen.PrivilegeManagement;
    using Xinchen.PrivilegeManagement.Enums;
    using Xinchen.PrivilegeManagement.DTO;
    using Xinchen.PrivilegeManagement.ViewModel;
    using System.Linq.Expressions;
    using System.Linq.Dynamic;
    using Xinchen.DbUtils.DynamicExpression;
    public class Privilege
    {
        private Xinchen.PrivilegeManagement.PrivilegeBase _privilegeBase;

        public Privilege(Xinchen.PrivilegeManagement.PrivilegeBase pb)
        {
            this._privilegeBase = pb;
        }

        public Xinchen.PrivilegeManagement.DTO.Role AddRole(AddRoleModel addRoleModel)
        {
            if (this._privilegeBase.GetRole(addRoleModel.Name) != null)
            {
                throw new ApplicationException("该名称已存在");
            }
            return this._privilegeBase.AddRole(addRoleModel.Name, addRoleModel.Status, null);
        }

        public Xinchen.PrivilegeManagement.DTO.Role AddRole(AddRoleModel addRoleModel, int[] privileges)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Xinchen.PrivilegeManagement.DTO.Role role = this._privilegeBase.GetRole(addRoleModel.Name);
                if (role != null)
                {
                    throw new ApplicationException("该名称已存在");
                }
                role = this._privilegeBase.AddRole(addRoleModel.Name, addRoleModel.Status, addRoleModel.Description, null);
                foreach (int num in privileges)
                {
                    this._privilegeBase.AddRolePrivilege(role.Id, num);
                }
                scope.Complete();
                return role;
            }
        }

        public Xinchen.PrivilegeManagement.DTO.User AddUser(AddUserModel addUserModel)
        {
            Xinchen.PrivilegeManagement.DTO.User user = this._privilegeBase.GetUser(addUserModel.Username);
            if (user != null)
            {
                throw new ApplicationException("该名称已存在");
            }
            using (TransactionScope scope = new TransactionScope())
            {
                user = this._privilegeBase.AddUser(addUserModel.Username, addUserModel.Description, addUserModel.Status);
                foreach (int num in addUserModel.RoleIds)
                {
                    this._privilegeBase.AddUserRole(num, user.Id);
                }
                scope.Complete();
            }
            return user;
        }

        public void ChangePassword(string oldPwd, string pwd, string newpwd)
        {
            this._privilegeBase.ChangePassword(oldPwd, pwd, newpwd);
        }

        public void DeleteUser(params int[] userId)
        {
            this.PrivilegeBase.DeleteUser(userId);
        }

        public void Disable(params int[] userId)
        {
            this.PrivilegeBase.Disable(userId);
        }

        public void DisableRole(params int[] rolesId)
        {
            this._privilegeBase.DisableRole(rolesId);
        }

        public void Enable(params int[] userId)
        {
            this.PrivilegeBase.Enable(userId);
        }

        public void EnableRole(params int[] rolesId)
        {
            this._privilegeBase.EnableRole(rolesId);
        }

        public Xinchen.PrivilegeManagement.DTO.Privilege GetMenuPrivilege(int menuId)
        {
            return this._privilegeBase.GetMenuPrivilege(menuId);
        }

        //public List<Xinchen.ExtNetBase.TreePanelEx.Node> GetMenus(int parentId)
        //{
        //    IList<Xinchen.PrivilegeManagement.DTO.Menu> menus = this._privilegeBase.GetMenus(parentId);
        //    List<Xinchen.ExtNetBase.TreePanelEx.Node> list2 = new List<Xinchen.ExtNetBase.TreePanelEx.Node>();
        //    foreach (Xinchen.PrivilegeManagement.DTO.Menu menu in menus)
        //    {
        //        Xinchen.ExtNetBase.TreePanelEx.Node item = new Xinchen.ExtNetBase.TreePanelEx.Node
        //        {
        //            Id = menu.Id,
        //            Name = menu.Name
        //        };
        //        list2.Add(item);
        //    }
        //    return list2;
        //}

        public NodeCollection GetNavigationMenus(int parentId)
        {
            IList<UserMenu> navigationMenus = this._privilegeBase.GetNavigationMenus(parentId);
            var list2 = new NodeCollection();
            foreach (UserMenu menu in navigationMenus)
            {
                string url = menu.Url;
                Ext.Net.Node item = new Ext.Net.Node
                {
                    Text = menu.Name,
                    NodeID = menu.Id.ToString(),
                    Leaf = menu.ChildCount <= 0,
                    Icon = Ext.Net.Icon.Page,
                    Qtip = menu.Description,
                    Expanded = true
                };
                list2.Add(item);
            }
            return list2;
        }

        public Xinchen.PrivilegeManagement.DTO.Privilege GetPrivilege(int id)
        {
            return this._privilegeBase.GetPrivilege(id);
        }
        public List<Ext.Net.Node> GetRoleCanAddPrivilegeNodes(int roleId)
        {
            List<Ext.Net.Node> list = new List<Ext.Net.Node>();
            IList<Xinchen.PrivilegeManagement.DTO.Privilege> privileges = this._privilegeBase.GetRoleNotHavePrivileges(roleId);
            foreach (Xinchen.PrivilegeManagement.DTO.Privilege privilege in privileges)
            {
                Ext.Net.Node item = new Ext.Net.Node
                {
                    NodeID = privilege.Id.ToString(),
                    Text = privilege.Description,
                    Leaf = true
                };
                list.Add(item);
            }
            return list;
        }

        public List<Ext.Net.Node> GetPrivilegeNodes()
        {
            List<Ext.Net.Node> list = new List<Ext.Net.Node>();
            IList<Xinchen.PrivilegeManagement.DTO.Privilege> privileges = this._privilegeBase.GetPrivileges();
            foreach (Xinchen.PrivilegeManagement.DTO.Privilege privilege in privileges)
            {
                Ext.Net.Node item = new Ext.Net.Node
                {
                    NodeID = privilege.Id.ToString(),
                    Text = privilege.Description,
                    Leaf = true
                };
                list.Add(item);
            }
            return list;
        }

        public IList<Xinchen.PrivilegeManagement.DTO.Privilege> GetPrivileges()
        {
            return this._privilegeBase.GetPrivileges();
        }

        public Xinchen.PrivilegeManagement.DTO.Role GetRole(int id)
        {
            return this._privilegeBase.GetRole(id);
        }

        public List<Ext.Net.Node> GetRoleNodes()
        {
            List<Ext.Net.Node> list = new List<Ext.Net.Node>();
            var roles = this._privilegeBase.GetRoles();
            foreach (Xinchen.PrivilegeManagement.DTO.Role role in roles)
            {
                Ext.Net.Node item = new Ext.Net.Node
                {
                    NodeID = role.Id.ToString(),
                    Text = role.Name,
                    Leaf = true
                };
                list.Add(item);
            }
            return list;
        }

        public IEnumerable<Ext.Net.Node> GetRoleNotHavePrivilegeNodes(int _roleId)
        {
            List<Ext.Net.Node> list = new List<Ext.Net.Node>();
            var roleNotHavePrivileges = this._privilegeBase.GetRoleNotHavePrivileges(_roleId);
            foreach (Xinchen.PrivilegeManagement.DTO.Privilege privilege in roleNotHavePrivileges)
            {
                Ext.Net.Node item = new Ext.Net.Node
                {
                    NodeID = privilege.Id.ToString(),
                    Text = privilege.Description,
                    Leaf = true
                };
                list.Add(item);
            }
            return list;
        }

        public IEnumerable<Ext.Net.Node> GetRolePrivilegeNodes(int _roleId)
        {
            List<Ext.Net.Node> list = new List<Ext.Net.Node>();
            var rolePrivileges = this._privilegeBase.GetRolePrivileges(_roleId);
            foreach (Xinchen.PrivilegeManagement.DTO.Privilege privilege in rolePrivileges)
            {
                Ext.Net.Node item = new Ext.Net.Node
                {
                    NodeID = privilege.Id.ToString(),
                    Text = privilege.Description,
                    Leaf = true
                };
                list.Add(item);
            }
            return list;
        }

        public IList<Xinchen.PrivilegeManagement.DTO.Role> GetRoles()
        {
            return this._privilegeBase.GetRoles();
        }

        public List<RolePrivilegeModel> GetRoles(int page, int pageSize, out int recordCount, FilterLinked filterLinked = null, Sort sort = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string condition = string.Empty, order = string.Empty;
            if (filterLinked != null)
            {
                condition = filterLinked.ToString(parameters);
            }
            if (sort != null)
            {
                order = sort.ToString();
            }
            //string dynamicConditions = this.ParseDynamicConditions(e, parameters, fieldMap);
            var list2 = this._privilegeBase.GetRoles(page, pageSize, condition, order, out recordCount, parameters.Values.ToArray()).ToList();
            list2.ForEach(delegate(RolePrivilegeModel role)
            {
                if (role.Name == this._privilegeBase.SystemRoleName)
                {
                    role.Privileges = "全部权限";
                }
            });
            return list2;
        }

        public Xinchen.PrivilegeManagement.DTO.User GetUser(int _userId)
        {
            return this._privilegeBase.GetUser(_userId);
        }

        public IEnumerable<Ext.Net.Node> GetUserNotHaveRoleNodes(int _userId)
        {
            List<Ext.Net.Node> list = new List<Ext.Net.Node>();
            var userNotHaveRoles = this._privilegeBase.GetUserNotHaveRoles(_userId);
            foreach (Xinchen.PrivilegeManagement.DTO.Role role in userNotHaveRoles)
            {
                Ext.Net.Node item = new Ext.Net.Node
                {
                    NodeID = role.Id.ToString(),
                    Text = role.Name,
                    Leaf = true
                };
                list.Add(item);
            }
            return list;
        }

        public IEnumerable<Ext.Net.Node> GetUserRoleNodes(int _userId)
        {
            List<Ext.Net.Node> list = new List<Ext.Net.Node>();
            var userRoles = this._privilegeBase.GetUserRoles(_userId);
            foreach (Xinchen.PrivilegeManagement.DTO.Role role in userRoles)
            {
                Ext.Net.Node item = new Ext.Net.Node
                {
                    NodeID = role.Id.ToString(),
                    Text = role.Name,
                    Leaf = true
                };
                list.Add(item);
            }
            return list;
        }

        public PageResult<UserRoleDetailInfo> GetUsers(int page, int pageSize, IList<SqlFilter> filters, Sort sort)
        {
            // int[] rolesId = null;
            //if (filters != null)
            //{
            //    foreach (var filter in filters)
            //    {
            //        switch (filter.Name)
            //        {
            //            case "Roles":
            //                rolesId = filter.GetValue<List<int>>().ToArray();
            //                break;
            //        }
            //    }
            //}
            //Func<IQueryable<UserRoleDetailInfo>, IQueryable<UserRoleDetailInfo>> extra = query =>
            //{
            //    if (filters != null)
            //    {
            //        foreach (var filter in filters)
            //        {
            //            switch (filter.Name)
            //            {
            //                case "Roles":
            //                    break;
            //                case "Status":
            //                    var status = filter.Value as IList<int>;
            //                    query = query.Where(x => status.Contains((int)x.Status));
            //                    break;
            //                default:
            //                    query = query.Where(filter.ToString(), filter.Value);
            //                    break;
            //            }
            //        }
            //    }
            //    if (sort == null)
            //    {
            //        sort = new Sort();
            //        sort.Field = "Id";
            //        sort.SortOrder = SortOrder.DESCENDING;
            //    }
            //    query = query.OrderBy(sort.Field + " " + sort.SortOrder.ToString());
            //    return query;
            //};

            var pr = _privilegeBase.GetUsers(page, pageSize, filters, sort);
            var userRoleDict = _privilegeBase.GetUserRoles().GroupBy(x => x.UserId);
            var roles = _privilegeBase.GetRoles().ToDictionary(x => x.Id);
            foreach (var userRole in pr.Data)
            {
                var userRoleIds = userRoleDict.FirstOrDefault(x => x.Key == userRole.Id);
                if (userRoleIds == null)
                {
                    continue;
                }
                var userRoles = new List<string>();
                foreach (var item in userRoleIds)
                {
                    var roleId = item.RoleId;
                    Role role = null;
                    if (roles.TryGetValue(roleId, out role))
                    {
                        userRoles.Add(role.Name);
                    }
                }
                var tmp = userRole;
                tmp.Roles = string.Join(",", userRoles);
            }
            return pr;
        }

        //public IList<UserRoleInfo> GetUsers(StoreReadDataEventArgs arg2, Func<string, string> fieldMap = null)
        //{
        //    List<object> parameters = null;
        //    string condition = this.ParseDynamicConditions(arg2, parameters, fieldMap);
        //    return this._privilegeBase.GetUsers(arg2.Page, arg2.Limit, condition, new object[] { parameters });
        //}

        private string ParseDynamicConditions(StoreReadDataEventArgs e, List<object> parameters, Func<string, string> fieldMap = null)
        {
            string str = e.Parameters["filter"];
            List<string> list = new List<string>();
            if (!string.IsNullOrEmpty(str))
            {
                FilterConditions conditions = new FilterConditions(str);
                foreach (FilterCondition condition in conditions.Conditions)
                {
                    Comparison comparison = condition.Comparison;
                    string field = condition.Field;
                    if (fieldMap != null)
                    {
                        field = fieldMap(field);
                    }
                    string str3 = field.Replace(".", "");
                    FilterType type = condition.Type;
                    object obj2 = condition.Value<object>();
                    if (!(obj2 is JValue))
                    {
                        goto Label_0221;
                    }
                    JValue value2 = obj2 as JValue;
                    object obj3 = value2.Value;
                    switch (value2.Type)
                    {
                        case JTokenType.Integer:
                            obj3 = Convert.ToInt32(obj3);
                            break;

                        case JTokenType.Float:
                            obj3 = Convert.ToDecimal(obj3);
                            break;

                        case JTokenType.String:
                            obj3 = Convert.ToString(obj3);
                            break;

                        case JTokenType.Boolean:
                            obj3 = Convert.ToBoolean(obj3);
                            break;

                        case JTokenType.Date:
                            obj3 = Convert.ToDateTime(obj3);
                            break;

                        default:
                            throw new NotSupportedException("未支持的JTokenType" + value2.Type);
                    }
                    switch (comparison)
                    {
                        case Comparison.Eq:
                            if (obj3 != null)
                            {
                                break;
                            }
                            list.Add(field + " is null");
                            goto Label_0208;

                        case Comparison.Gt:
                            list.Add(field + ">@" + str3);
                            goto Label_0208;

                        case Comparison.Lt:
                            list.Add(field + "<@" + str3);
                            goto Label_0208;

                        default:
                            goto Label_0208;
                    }
                    if (type == FilterType.String)
                    {
                        list.Add(field + " like '%'+@" + str3 + "+'%'");
                    }
                    else
                    {
                        list.Add(field + "=@" + str3);
                    }
                Label_0208:
                    if (obj3 != null)
                    {
                        parameters.Add(obj3);
                    }
                    continue;
                Label_0221:
                    if (obj2 is JArray)
                    {
                        JArray source = obj2 as JArray;
                        List<int> values = new List<int>();
                        source.ToList<JToken>().ForEach(delegate(JToken x)
                        {
                            values.Add(x.Value<int>());
                        });
                        list.Add(field + " in (" + string.Join<int>(",", values) + ")");
                    }
                }
            }
            return string.Join(" and ", list);
        }

        private string ParseDynamicSorts(StoreReadDataEventArgs e)
        {
            List<string> values = new List<string>();
            foreach (DataSorter sorter in e.Sort)
            {
                values.Add(sorter.Property + " " + sorter.Direction);
            }
            return string.Join(",", values);
        }

        public void RegAdmin(string userName, string password, string password2)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }
            if (string.IsNullOrEmpty(password2))
            {
                throw new ArgumentNullException("password2");
            }
            if (password != password2)
            {
                throw new ApplicationException("两次输入密码不一致");
            }
            using (TransactionScope scope = new TransactionScope())
            {
                Department department = this._privilegeBase.AddDepartment("系统部门", "系统默认的部门");
                Xinchen.PrivilegeManagement.DTO.Role role = this._privilegeBase.AddRole("系统角色", BaseStatuses.Valid, "系统角色", new int?(department.Id));
                Xinchen.PrivilegeManagement.DTO.User user = new Xinchen.PrivilegeManagement.DTO.User
                {
                    DepartmentId = new int?(department.Id),
                    CreateTime = DateTime.Now,
                    Username = userName,
                    Password = password,
                    Status = BaseStatuses.Valid
                };
                user = this._privilegeBase.RegAdmin(user);
                this._privilegeBase.AddUserRole(role.Id, user.Id);
                scope.Complete();
            }
        }

        public void ResetPassword(params int[] userId)
        {
            this.PrivilegeBase.ResetPassword("123456", userId);
        }

        public Xinchen.PrivilegeManagement.DTO.Role UpdateRole(UpdateRoleModel updateRoleModel, int[] privilegeIds)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Xinchen.PrivilegeManagement.DTO.Role role = this._privilegeBase.GetRole(updateRoleModel.Id);
                if (role == null)
                {
                    throw new ApplicationException("该角色不存在");
                }
                role.Description = updateRoleModel.Description;
                role.Status = updateRoleModel.Status;
                role = this._privilegeBase.UpdateRole(role);
                var rolePrivileges = this.PrivilegeBase.GetRolePrivileges(role.Id).ToList();
                int[] numArray = privilegeIds;
                for (int i = 0; i < numArray.Length; i++)
                {
                    Predicate<Xinchen.PrivilegeManagement.DTO.Privilege> match = null;
                    int privilegeId = numArray[i];
                    if (match == null)
                    {
                        match = x => x.Id == privilegeId;
                    }
                    if (!rolePrivileges.Exists(match))
                    {
                        this.PrivilegeBase.AddRolePrivilege(role.Id, privilegeId);
                    }
                }
                foreach (Xinchen.PrivilegeManagement.DTO.Privilege privilege in rolePrivileges)
                {
                    if (!privilegeIds.Contains<int>(privilege.Id))
                    {
                        this.PrivilegeBase.RemoveRolePrivilege(privilege.Id, role.Id);
                    }
                }
                scope.Complete();
                return role;
            }
        }

        public Xinchen.PrivilegeManagement.DTO.User UpdateUser(UpdateUserModel updateUserModel, int[] rolesId)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                Xinchen.PrivilegeManagement.DTO.User user = this._privilegeBase.GetUser(updateUserModel.Id);
                if (user == null)
                {
                    throw new ApplicationException("该用户不存在");
                }
                user.Description = updateUserModel.Description;
                user.Status = updateUserModel.Status;
                user = this._privilegeBase.UpdateUser(user);
                var userRoles = this.PrivilegeBase.GetUserRoles(user.Id).ToList();
                int[] numArray = rolesId;
                for (int i = 0; i < numArray.Length; i++)
                {
                    Predicate<Xinchen.PrivilegeManagement.DTO.Role> match = null;
                    int roleId = numArray[i];
                    if (match == null)
                    {
                        match = x => x.Id == roleId;
                    }
                    if (!userRoles.Exists(match))
                    {
                        this.PrivilegeBase.AddUserRole(roleId, user.Id);
                    }
                }
                foreach (Xinchen.PrivilegeManagement.DTO.Role role in userRoles)
                {
                    if (!rolesId.Contains<int>(role.Id))
                    {
                        this.PrivilegeBase.RemoveUserRole(role.Id, user.Id);
                    }
                }
                scope.Complete();
                return user;
            }
        }

        public Xinchen.PrivilegeManagement.PrivilegeBase PrivilegeBase
        {
            get
            {
                return this._privilegeBase;
            }
        }

        public UserSessionModel UserInfo
        {
            get
            {
                return this._privilegeBase.UserInfo;
            }
        }

        public void ChangeMenusParent(string[] sources, int target)
        {
            _privilegeBase.ChangeMenusParent(sources, target);
        }

        public Xinchen.PrivilegeManagement.DTO.Menu GetMenu(int menuId)
        {
            return _privilegeBase.GetMenu(menuId);
        }

        public void DeleteRoles(params int[] roleIds)
        {
            PrivilegeBase.DeleteRoles(roleIds);
        }
    }
}

