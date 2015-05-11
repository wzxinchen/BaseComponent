using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.Utils.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute:Attribute
    {
        public TableAttribute(string tableName)
        {
            this.TableName = tableName;
        }
        public string TableName { get; set; }
    }
}
