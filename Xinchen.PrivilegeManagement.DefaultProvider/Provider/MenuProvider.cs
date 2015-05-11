using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.PrivilegeManagement.Provider;
using Xinchen.Utils.Entity;

namespace Xinchen.PrivilegeManagement.DefaultProvider.Provider
{
    public class MenuProvider : IEntityProvider<PrivilegeManagement.Model.Menu>
    {
        private ProviderContext _context = ProviderContext.GetInstance();
        private static object _syncRoot = new object();
        private static MenuProvider _instance;
        public static MenuProvider GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new MenuProvider();
                    }
                }
            }

            return _instance;
        }

        private MenuProvider()
        {
        }

        public bool Exist(int id)
        {
            return _context.Menus.ExistsById(id);
        }

        public bool IsEmpty()
        {
            return _context.Menus.IsEmpty();
        }

        public int GetUniqueId()
        {
            return _context.Menus.GetSequenceId();
        }

        public PrivilegeManagement.Model.Menu Add(PrivilegeManagement.Model.Menu entity)
        {
            Model.Menu menu = EntityMapper.Map<PrivilegeManagement.Model.Menu, Model.Menu>(entity);
            _context.Menus.Create(menu);
            return _context.CreateProxy(entity);
        }

        public PrivilegeManagement.Model.Menu Get(int id)
        {
            var menu = _context.Menus.GetById(id);
            return _context.CreateProxy(EntityMapper.Map<Model.Menu, PrivilegeManagement.Model.Menu>(menu));
        }

        public PrivilegeManagement.Model.Menu Get(Action<PrivilegeManagement.Model.Menu> condition)
        {
            var menu = _context.CreateProxy<PrivilegeManagement.Model.Menu>();
            condition(menu);
            var dict = _context.GetModifiedProperties(menu);
            var pmenu = _context.Menus.GetBy(dict).FirstOrDefault();
            if (pmenu == null)
            {
                return null;
            }
            return _context.CreateProxy(EntityMapper.Map<Model.Menu, PrivilegeManagement.Model.Menu>(pmenu));
        }

        public void Update(PrivilegeManagement.Model.Menu entity)
        {
            var proxy = _context.Menus.CreateProxy();
            var rMenu = EntityMapper.Map<PrivilegeManagement.Model.Menu, Model.Menu>(entity);
            _context.Menus.Update(rMenu);
        }


        public List<PrivilegeManagement.Model.Menu> GetList(Action<PrivilegeManagement.Model.Menu> condition)
        {
            var menu = _context.CreateProxy<PrivilegeManagement.Model.Menu>();
            condition(menu);
            var dict = _context.GetModifiedProperties(menu);
            var menus = _context.Menus.GetBy(dict);
            List<PrivilegeManagement.Model.Menu> pmenus = new List<PrivilegeManagement.Model.Menu>();
            foreach (var menu1 in menus)
            {
                pmenus.Add(EntityMapper.Map<Model.Menu, PrivilegeManagement.Model.Menu>(menu1));
            }
            return pmenus;
        }
    }
}
