namespace Xinchen.ApplicationBase.Model
{
    using System;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.Enums;

    public class UpdateUserModel
    {
        public string Description { get; set; }

        public BaseStatuses Status { get; set; }

        public int Id { get; set; }
    }
}

