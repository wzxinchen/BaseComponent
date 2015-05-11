using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ApplicationBase.DAL;
using Xinchen.ApplicationBase.Model;

namespace Xinchen.ApplicationBase.BLL
{
    public class Privilege
    {
        private static object _syncRoot = new object();
        private static Privilege _instance;
        public static Privilege GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new Privilege();
                    }
                }
            }

            return _instance;
        }

        private Privilege()
        {
        }
        private PrivilegeContext context = PrivilegeContext.GetInstance();

        //public UserInfo GetPrivilege(int usersysno)
        //{
        //    return null;
        //}

    }
}
