using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.ExtNetBase
{
    public class FilterAttribute:Attribute
    {
        public bool Enabled { get; set; }
        public FilterAttribute()
        {
            Enabled = true;
        }

        /// <summary>
        /// 1、一个继承ForeignFilterBase实现通过外键进行过滤的类的Type，2、一个枚举值的Type
        /// </summary>
        public Type FilterType { get; set; }
    }
}
