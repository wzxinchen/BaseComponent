using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.DbEntity;

namespace Xinchen.ApplicationBase.DAL
{
    public class PrivilegeContext:EntityContext
    {
        private static object _syncRoot = new object();
        private static PrivilegeContext _instance;
        public static PrivilegeContext GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new PrivilegeContext();
                    }
                }
            }

            return _instance;
        }

        private PrivilegeContext()
        {
        }
    }
}
