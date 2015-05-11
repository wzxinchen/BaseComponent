using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.PrivilegeManagement.Provider;
using Xinchen.Utils.Entity;

namespace Xinchen.PrivilegeManagement.DefaultProvider.Provider
{
    //public class RoleProvider : IEntityProvider<PrivilegeManagement.Model.Role>
    //{
    //    private ProviderContext _context = ProviderContext.GetInstance();
    //    private static object _syncLock = new object();
    //    private static RoleProvider _instance;

    //    public static RoleProvider GetInstance()
    //    {
    //        if (_instance == null)
    //        {
    //            lock (_syncLock)
    //            {
    //                if (_instance == null)
    //                {
    //                    _instance = new RoleProvider();
    //                }
    //            }
    //        }
    //        return _instance;
    //    }

    //    private RoleProvider()
    //    {
    //    }

    //    public bool Exist(int id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool IsEmpty()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public int GetUniqueId()
    //    {
    //        return _context.Roles.GetSequenceId();
    //    }

    //    public PrivilegeManagement.Model.Role Add(PrivilegeManagement.Model.Role entity)
    //    {
    //        var roleEntity = DynamicProxy.CreateDynamicProxy<Model.Role>();// new Model.Role();
    //        roleEntity.DepartmentId = entity.DepartmentId;
    //        roleEntity.Id = entity.Id;
    //        roleEntity.Status = entity.Status;
    //        roleEntity.Name = entity.Name;
    //        _context.Roles.Create(roleEntity);
    //        return DynamicProxy.CreateDynamicProxy(entity);
    //    }

    //    public PrivilegeManagement.Model.Role Get(int id)
    //    {
    //        var roleLocal = _context.Roles.GetById(id);
    //        var role = EntityMapper.Map<Model.Role, PrivilegeManagement.Model.Role>(roleLocal);
    //        var roleProxy = _context.CreateProxy(role);
    //        return roleProxy;
    //    }

    //    public PrivilegeManagement.Model.Role Get(Action<PrivilegeManagement.Model.Role> condition)
    //    {
    //        var roleRemoteProxy = DynamicProxy.CreateDynamicProxy<PrivilegeManagement.Model.Role>();
    //        condition(roleRemoteProxy);
    //        var dict = _context.GetModifiedProperties(roleRemoteProxy);
    //        var roleLocal = _context.Roles.GetBy(dict).FirstOrDefault();
    //        if (roleLocal == null)
    //        {
    //            return null;
    //        }
    //        var role = EntityMapper.Map<Model.Role, PrivilegeManagement.Model.Role>(roleLocal);
    //        var roleProxy = _context.CreateProxy(role);
    //        return roleProxy;
    //    }

    //    public void Update(PrivilegeManagement.Model.Role entity)
    //    {
    //        var roleProxy = DynamicProxy.CreateDynamicProxy<Model.Role>();
    //        EntityMapper.Map(entity, roleProxy);
    //        _context.Roles.Update(roleProxy);
    //    }


    //    public List<PrivilegeManagement.Model.Role> GetList(Action<PrivilegeManagement.Model.Role> condition)
    //    {
    //        var roleRemoteProxy = DynamicProxy.CreateDynamicProxy<PrivilegeManagement.Model.Role>();
    //        condition(roleRemoteProxy);
    //        var dict = _context.GetModifiedProperties(roleRemoteProxy);
    //        var roleLocalList = _context.Roles.GetBy(dict);
    //        List<PrivilegeManagement.Model.Role> roles = new List<PrivilegeManagement.Model.Role>();
    //        foreach (var role1 in roleLocalList)
    //        {
    //            roles.Add(EntityMapper.Map<Model.Role, PrivilegeManagement.Model.Role>(role1));
    //        }
    //        return roles;
    //    }
    //}
}
