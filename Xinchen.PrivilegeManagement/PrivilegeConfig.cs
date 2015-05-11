using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.PrivilegeManagement.Provider;

namespace Xinchen.PrivilegeManagement
{
    public class PrivilegeConfig
    {
        public IPrivilegeContextProvider Provider { get; internal set; }
    }
}
