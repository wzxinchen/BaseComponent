namespace Xinchen.PrivilegeManagement.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.DTO;

    [Serializable]
    public class UserSessionModel
    {
        public UserSessionModel()
        {
            this.Roles = new Dictionary<int, Role>();
            this.Privileges = new Dictionary<int, Privilege>();
        }

        public int Id { get; set; }

        public Dictionary<int, Privilege> Privileges { get; private set; }

        public Dictionary<int, Role> Roles { get; private set; }

        public string Username { get; set; }
    }
}

