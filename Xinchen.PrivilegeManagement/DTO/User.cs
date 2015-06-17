namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.Enums;

    [Serializable]
    [Table("Users")]
    public class User
    {
        public virtual DateTime CreateTime { get; set; }

        public virtual int? DepartmentId { get; set; }

        public virtual string Description { get; set; }

        public virtual BaseStatuses Status { get; set; }

        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual int Id { get; set; }

        public virtual string Password { get; set; }

        public virtual string Username { get; set; }
    }
}

