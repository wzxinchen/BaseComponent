namespace Xinchen.PrivilegeManagement.DefaultProvider.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.Utils.DataAnnotations;

    [Table("Roles")]
    public class Role
    {
        public virtual int? DepartmentId { get; set; }

        public virtual int Status { get; set; }

        [Key]
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
    }
}

