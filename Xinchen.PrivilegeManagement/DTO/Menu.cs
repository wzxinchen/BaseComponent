namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.Enums;
    using Xinchen.Utils.DataAnnotations;

    [Serializable]
    public class Menu
    {
        //public virtual string Function { get; set; }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual int ParentId { get; set; }

        public virtual int? PrivilegeId { get; set; }

        public virtual int Sort { get; set; }

        public virtual string Url { get; set; }
        public virtual BaseStatuses Status { get; set; }
        public virtual string Description { get; set; }
    }
}

