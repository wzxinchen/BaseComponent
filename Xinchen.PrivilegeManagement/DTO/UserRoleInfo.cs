namespace Xinchen.PrivilegeManagement.DTO
{
    using System;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.Enums;

    public class UserRoleDetailInfo
    {
        public DateTime CreateTime { get; set; }

        public string Description { get; set; }

        public BaseStatuses Status { get; set; }

        public int Id { get; set; }

        public string Roles { get; set; }

        public string Username { get; set; }

        public int RoleId { get; set; }
    }
}

