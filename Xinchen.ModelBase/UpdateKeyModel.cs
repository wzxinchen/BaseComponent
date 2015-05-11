using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Xinchen.ModelBase
{
    public class UpdateKeyModel
    {
        [Required]
        public int Id { get; set; }
        [Required(AllowEmptyStrings=false,ErrorMessage="名称不能为空")]
        public string Name { get; set; }
    }
}
