namespace Xinchen.PrivilegeManagement.DefaultProvider.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.Utils.DataAnnotations;

    [Table("Menus")]
    public class Menu
    {
        public virtual string Description { get; set; }

        [Key]
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual int ParentId { get; set; }

        public virtual int? PrivilegeId { get; set; }

        public virtual int Sort { get; set; }

        public virtual int Status { get; set; }

        public virtual string Url { get; set; }
    }
}

