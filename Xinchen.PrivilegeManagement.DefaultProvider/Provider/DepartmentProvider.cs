using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.PrivilegeManagement.Provider;

namespace Xinchen.PrivilegeManagement.DefaultProvider.Provider
{
    public class DepartmentProvider : IEntityProvider<PrivilegeManagement.Model.Department>
    {
        private ProviderContext _context = ProviderContext.GetInstance();
        private static object _syncRoot = new object();
        private static DepartmentProvider _instance;
        public static DepartmentProvider GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new DepartmentProvider();
                    }
                }
            }

            return _instance;
        }

        private DepartmentProvider()
        {
        }
        public bool Exist(int id)
        {
            return _context.Departments.ExistsById(id);
        }


        public PrivilegeManagement.Model.Department Get(int id)
        {
            var department = _context.Departments.GetById(id);

            if (department == null)
            {
                return null;
            }

            PrivilegeManagement.Model.Department d = new PrivilegeManagement.Model.Department();
            d.Description = department.Description;
            d.Id = department.Id;
            d.Name = department.Name;
            d.Status = department.Status;
            return d;
        }

        public void Update(PrivilegeManagement.Model.Department entity)
        {
            _context.Departments.Update(new Model.Department()
            {
                Id = entity.Id,
                Status = entity.Status,
                Name = entity.Name,
                Description = entity.Description
            });
        }


        public PrivilegeManagement.Model.Department Get(Action<PrivilegeManagement.Model.Department> condition)
        {
            throw new NotImplementedException();
        }


        public bool IsEmpty()
        {
            return _context.Departments.IsEmpty();
        }

        public int GetUniqueId()
        {
            return _context.Departments.GetSequenceId();
        }

        public PrivilegeManagement.Model.Department Add(PrivilegeManagement.Model.Department entity)
        {
            Model.Department dept = new Model.Department();
            dept.Description = entity.Description;
            dept.Id = entity.Id;
            dept.Name = entity.Name;
            dept.Status = entity.Status;
            _context.Departments.Create(dept);
            return _context.CreateProxy(entity);
        }


        public List<PrivilegeManagement.Model.Department> GetList(Action<PrivilegeManagement.Model.Department> condition)
        {
            throw new NotImplementedException();
        }
    }
}
