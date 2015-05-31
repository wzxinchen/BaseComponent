using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.DbUtils
{
    public class BaseEntity
    {
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool IsEnabled { get; set; }
    }
}
