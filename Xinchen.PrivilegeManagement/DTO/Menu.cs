namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    [Serializable]
    [Table("Menus")]
    public class Menu
    {
        //public virtual string Function { get; set; }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual int? ParentId { get; set; }

        public virtual int? PrivilegeId { get; set; }

        public virtual int Sort { get; set; }

        public virtual string Url { get; set; }
    }
}

