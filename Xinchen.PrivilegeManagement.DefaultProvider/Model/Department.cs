namespace Xinchen.PrivilegeManagement.DefaultProvider.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.Utils.DataAnnotations;

    [Table("Departments")]
    public class Department
    {
        public virtual string Description { get; set; }

        [Key]
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual int Status { get; set; }
    }
}

