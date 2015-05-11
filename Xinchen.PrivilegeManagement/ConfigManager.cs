using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using Xinchen.PrivilegeManagement.DTO;
using Xinchen.PrivilegeManagement.Provider;

namespace Xinchen.PrivilegeManagement
{
    public class ConfigManager
    {
        private static PrivilegeConfig _privilegeConfig;
        static ConfigManager()
        {
            _privilegeConfig = (PrivilegeConfig)ConfigurationManager.GetSection("privilegeConfig");
            if (_privilegeConfig == null)
            {
                _privilegeConfig = new PrivilegeConfig();
                _privilegeConfig.Provider = (IPrivilegeContextProvider)Assembly.Load("Xinchen.PrivilegeManagement.DefaultProvider").CreateInstance("Xinchen.PrivilegeManagement.DefaultProvider.Provider.DefaultProvider");
            }
        }

        ///// <summary>
        ///// 注册所有权限
        ///// </summary>
        ///// <param name="enumType">枚举的Type</param>
        //public static void RegisterPrivileges(Type enumType)
        //{
        //}

        public static IPrivilegeContextProvider PrivilegeProvider
        {
            get { return _privilegeConfig.Provider; }
        }
    }
}
