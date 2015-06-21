using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.SchemaModel
{
    /// <summary>
    /// 从实体类型分析出来的数据库表信息
    /// </summary>
    public class Table
    {
        public Table()
        {
            Columns = new Dictionary<string, Column>();
        }
        public string DataBase { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public Dictionary<string,Column> Columns { get;private set; }
    }
}
