using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.ModelBase
{
    public class KeyValueModel
    {
        [DataGridColumn("编号")]
        public virtual int Id { get; set; }
        [DataGridColumn("名称")]
        public virtual string Name { get; set; }
    }
}
