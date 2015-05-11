using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.Utils.DataAnnotations
{
    /// <summary>
    /// 用于自动初始化表格的列属性
    /// </summary>
    public class DataGridColumnAttribute : Attribute
    {
        public DataGridColumnAttribute(string name)
        {
            DisplayName = name;
            Visible = true;
            Width = -1;

        }

        /// <summary>
        /// 如果该列实际由枚举值组成，则该值用于设置这个枚举的Type
        /// </summary>
        public Type ValueType { get; set; }

        public DataGridColumnAttribute()
        {
            Width = -1;
        }
        public string DisplayName { get; set; }

        public bool Visible { get; set; }

        public int Width { get; set; }

        public string Format { get; set; }

        public string EmptyText { get; set; }

    }
}
