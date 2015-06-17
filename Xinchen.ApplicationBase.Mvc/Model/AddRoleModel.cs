namespace Xinchen.ApplicationBase.Mvc.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.Enums;

    public class AddRoleModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "名字不能为空")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "必须选择状态")]
        public BaseStatuses Status { get; set; }
        public string Description { get; set; }
    }
}

