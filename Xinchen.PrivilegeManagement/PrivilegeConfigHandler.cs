using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using Xinchen.PrivilegeManagement.Provider;

namespace Xinchen.PrivilegeManagement
{
    public class PrivilegeConfigHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            var providerClassNode = section.Attributes["provider"];
            var assemblyName = "Xinchen.PrivilegeManagement.DefaultProvider";
            var className = "DefaultProvider";
            if (providerClassNode != null)
            {
                assemblyName = providerClassNode.Value.Split(',')[1];
                className = providerClassNode.Value.Split(',')[0];
            }
            IPrivilegeContextProvider provider = (IPrivilegeContextProvider)Assembly.Load(assemblyName).CreateInstance(className);
            if (provider == null)
            {
                throw new NotImplementedException("未找到提供者程序");
            }

            return new PrivilegeConfig()
            {
                Provider = provider,
            };
        }
    }
}
