using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace Xinchen.ModelBase
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "用户名不能为空")]
        public virtual string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "密码不能为空")]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "验证码不能为空")]
        [CustomValidation(typeof(LoginModel), "TryValidateObject", ErrorMessage = "验证码错误")]
        public string VerifyCode { get; set; }

        public static ValidationResult TryValidateObject(
            string instance,
            ValidationContext validationContext
            )
        {
            if (string.IsNullOrEmpty(instance))
            {
                return new ValidationResult("验证码为空");
            }
            var context = HttpContext.Current;
            if (context == null)
            {
                return new ValidationResult("只支持网站项目的验证");
            }
            var session = context.Session;
            if (session == null)
            {
                return new ValidationResult("当前设置不支持Session，所以无法验证验证码");
            }
            if (session["VerifyCode"] == null)
            {
                return new ValidationResult("验证码未生成或已过期");
            }
            if (session["VerifyCode"].ToString() != instance)
            {
                return new ValidationResult("验证码错误");
            }
            return ValidationResult.Success;
        }
    }
}
