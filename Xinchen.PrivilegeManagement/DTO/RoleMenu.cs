namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class RoleMenu
    {
        public virtual int Id { get; set; }

        public virtual int MenuId { get; set; }

        public virtual int RoleId { get; set; }
    }
}

