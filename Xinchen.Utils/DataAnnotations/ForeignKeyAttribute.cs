using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.Utils.DataAnnotations
{
    public class ForeignKeyAttribute:Attribute
    {
        /// <summary>
        /// 外键属性所在实体的类型
        /// </summary>
        public Type ForeignType { get; set; }

        /// <summary>
        /// 外键属性所在实体的属性名
        /// </summary>
        public string ForeignProperty { get; set; }

        /// <summary>
        /// 外键属性对应的实体映射到自身实体的属性名
        /// </summary>
        public string SelfProperty { get; set; }
    }
}
