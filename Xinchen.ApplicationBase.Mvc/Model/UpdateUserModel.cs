namespace Xinchen.ApplicationBase.Mvc.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.Enums;

    public class UpdateUserModel
    {
        public string Description { get; set; }

        [Required]
        public BaseStatuses Status { get; set; }

        [Required]
        public int Id { get; set; }
    }
}

