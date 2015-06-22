namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class UserRole
    {
        public virtual int Id { get; set; }

        public virtual int RoleId { get; set; }

        public virtual int UserId { get; set; }
    }
}

