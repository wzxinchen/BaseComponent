namespace Xinchen.ApplicationBase.Mvc.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.Enums;

    public class UpdateRoleModel
    {
        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "请选择状态")]
        public BaseStatuses Status { get; set; }

        [Required]
        public int Id { get; set; }
    }
}

