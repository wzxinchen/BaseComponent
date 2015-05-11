namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class UserRolePrivilege
    {
        public virtual int? PrivilegeId { get; set; }

        public virtual string PrivilegeName { get; set; }

        public virtual int? RoleId { get; set; }

        public virtual string RoleName { get; set; }
    }
}

