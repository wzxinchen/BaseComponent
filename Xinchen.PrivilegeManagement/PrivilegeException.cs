using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.PrivilegeManagement
{
    public class PrivilegeException:ApplicationException
    {
        public PrivilegeException(string msg):base(msg)
        {
            
        }
    }
}
