namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    [Serializable]
    [Table("UserMenus")]
    public class UserMenu
    {
        public int ChildCount { get; set; }

        public string Description { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }

        public int Sort { get; set; }

        public int Status { get; set; }

        public string Url { get; set; }
    }
}

