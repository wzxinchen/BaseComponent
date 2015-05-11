using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Xinchen.ScheduledTask.Winform.Model.FormModel
{
    public class AddTaskModel
    {
        [Required(ErrorMessage = "名称不能为空", AllowEmptyStrings = false)]
        public string Name { get; set; }
        [Required(ErrorMessage = "请选择程序集文件", AllowEmptyStrings = false)]
        public string Dll { get; set; }
        [Required(ErrorMessage = "类型不能为空", AllowEmptyStrings = false)]
        public string Type { get; set; }
        [Required(ErrorMessage = "方法不能为空", AllowEmptyStrings = false)]
        public string Method { get; set; }
        [Required(ErrorMessage = "间隔不能为空", AllowEmptyStrings = false)]
        [Range(1, 100000000, ErrorMessage = "间隔时间不能小于等于0")]
        public int Interval { get; set; }
        public int RunType { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}
