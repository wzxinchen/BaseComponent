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
        public static string DataBase { get; internal set; }

        public static string ConnectionStringName { get; internal set; }
        public static string DbFactoryName { get; internal set; }

        public static string SequenceTable { get; internal set; }
        public static string SqlBuilder { get; internal set; }
    }
}
