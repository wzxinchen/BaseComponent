namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class DepartmentPrivilege
    {
        public virtual int DepartmentId { get; set; }

        public virtual int Id { get; set; }

        public virtual int PrivilegeId { get; set; }
    }
}

