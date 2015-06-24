using PPD.XLinq.Provider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public class ConfigManager
    {
        static DatabaseTypes? databaseType = null;
        public static DatabaseTypes DataBaseType
        {
            get
            {
                return databaseType.HasValue ? databaseType.Value : (databaseType = (DatabaseTypes)Enum.Parse(typeof(DatabaseTypes), DataBase)).Value;
            }
        }
        public static string DataBase { get; internal set; }

        public static string ConnectionStringName { get; internal set; }
        public static string DbFactoryName { get; internal set; }

        public static string SequenceTable { get; internal set; }
        public static string SqlBuilder { get; internal set; }
    }
}
