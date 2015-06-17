using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Attributes
{
    public class TableAttribute : System.ComponentModel.DataAnnotations.Schema.TableAttribute
    {
        public string DataBaseName { get; set; }
        public TableAttribute(string dbName, string tableName)
            : base(tableName)
        {

        }
    }
}
