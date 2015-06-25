namespace Xinchen.PrivilegeManagement.DefaultProvider.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;
    using Xinchen.DbUtils.DataAnnotations;

    [Table("UserRoles")]
    public class UserRole
    {
        [AutoIncrement, Key]
        public virtual int Id { get; set; }

        public virtual int RoleId { get; set; }

        public virtual int UserId { get; set; }
    }
}

