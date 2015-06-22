namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class Privilege
    {
        public virtual string Description { get; set; }

        public virtual int Id { get; set; }

        //public virtual int? MenuId { get; set; }

        public virtual string Name { get; set; }
    }
}

