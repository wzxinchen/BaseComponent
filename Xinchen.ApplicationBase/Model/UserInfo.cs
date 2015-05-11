namespace Xinchen.ApplicationBase.Model
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class UserInfo
    {
        public Dictionary<int, Role> Roles { get; set; }

        public int SysNo { get; set; }

        public string UserID { get; set; }

        public string Username { get; set; }
    }
}

