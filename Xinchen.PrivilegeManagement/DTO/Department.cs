namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.Runtime.CompilerServices;

    [Serializable]
    public class Department
    {
        public virtual string Description { get; set; }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual int Status { get; set; }
    }
}

