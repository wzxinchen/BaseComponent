using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.SchemaModel
{
    public class Column
    {
        public string Name { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public Table Table { get; set; }

        public bool IsKey { get; set; }
        public bool IsAutoIncreament { get; set; }
    }
}
