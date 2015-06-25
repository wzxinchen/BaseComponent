namespace Xinchen.PrivilegeManagement.DefaultProvider.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;
    using Xinchen.Utils.DataAnnotations;

    [Table("Users")]
    public class User
    {
        //public virtual int? ConfigId { get; set; }

        public virtual DateTime CreateTime { get; set; }

        //public virtual int? DepartmentId { get; set; }

        //public virtual string Description { get; set; }

        public virtual int Status { get; set; }

        [Key]
        public virtual int Id { get; set; }

        public virtual string Password { get; set; }

        public virtual string Username { get; set; }
    }
}

