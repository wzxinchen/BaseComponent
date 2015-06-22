namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class RolePrivilege
    {
        public virtual int Id { get; set; }

        public virtual int PrivilegeId { get; set; }

        public virtual int RoleId { get; set; }
    }
}

