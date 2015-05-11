namespace Xinchen.PrivilegeManagement.ViewModel
{
    using System;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.Enums;
    using Xinchen.Utils.DataAnnotations;

    public class RolePrivilegeModel
    {
        public virtual BaseStatuses Status { get; set; }

        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Privileges { get; set; }

        public virtual string Description { get; set; }
    }
}

