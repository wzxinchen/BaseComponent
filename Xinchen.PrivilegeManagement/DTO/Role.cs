namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.Enums;

    [Serializable]
    public class Role
    {
        public virtual int? DepartmentId { get; set; }

        public string Description { get; set; }

        public virtual BaseStatuses Status { get; set; }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
    }
}

