namespace Xinchen.PrivilegeManagement.DefaultProvider.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.Utils.DataAnnotations;

    [Table("RolePrivileges")]
    public class RolePrivilege
    {
        [AutoIncrement, Key]
        public virtual int Id { get; set; }

        public virtual int PrivilegeId { get; set; }

        public virtual int RoleId { get; set; }
    }
}

