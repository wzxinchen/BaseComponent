namespace Xinchen.ApplicationBase.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.PrivilegeManagement.Enums;

    public class AddUserModel
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "用户名不能为空")]
        public string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "请选择状态")]
        public BaseStatuses Status { get; set; }
        public string Description { get; set; }

    }
}

