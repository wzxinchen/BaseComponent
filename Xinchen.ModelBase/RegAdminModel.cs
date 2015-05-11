using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.ModelBase
{
    public class RegAdminModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户名不能为空")]
        public virtual string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "确认密码不能为空")]
        public string Password2 { get; set; }
    }
}
