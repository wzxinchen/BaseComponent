namespace Xinchen.PrivilegeManagement.DefaultProvider.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.Utils.DataAnnotations;

    [Table("RoleMenus")]
    public class RoleMenu
    {
        [AutoIncrement, Key]
        public virtual int Id { get; set; }

        public virtual int MenuId { get; set; }

        public virtual int RoleId { get; set; }
    }
}

