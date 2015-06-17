namespace Xinchen.ApplicationBase.Mvc.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;

    public class ChangePasswordModel
    {
        [Required(AllowEmptyStrings=false, ErrorMessage="新密码不能为空")]
        public string NewPassword { get; set; }

        [Required(AllowEmptyStrings=false, ErrorMessage="确认密码不能为空")]
        public string NewPassword2 { get; set; }

        [Required(AllowEmptyStrings=false, ErrorMessage="旧密码不能为空")]
        public string OldPassword { get; set; }
    }
}

