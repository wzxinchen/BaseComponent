using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.DbEntity
{
    public class Column
    {
        [DataGridColumn("名称")]
        public string Name { get; set; }
        [DataGridColumn("名称",Visible=false)]
        public Type DataType { get; set; }

        public string DataTypeString { get; set; }
        [DataGridColumn("长度")]
        public int Length { get; set; }
        [DataGridColumn(DisplayName = "可为空")]
        public bool IsNullable { get; set; }
        public bool IsKey { get; set; }
        public bool IsAutoIncrement { get; set; }

        public string GetTypeString()
        {
            if (!DataType.IsValueType)
            {
                return DataTypeString;
            }
            if (IsNullable)
            {
                return DataTypeString + "?";
            }
            return DataTypeString;
        }
    }
}
