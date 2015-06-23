using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public class ConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("dataBase")]
        public string DataBase
        {
            get { return (string)base["dataBase"]; }
            set { base["dataBase"] = value; }
        }

        [ConfigurationProperty("connectionStringName")]
        public string ConnectionStringName
        {
            get
            {
                return (string)base["connectionStringName"];
            }
            set
            {
                base["connectionStringName"] = value;
            }
        }

        [ConfigurationProperty("dbFactoryName")]
        public string DbFactoryName
        {
            get
            {
                return (string)base["dbFactoryName"];
            }
            set
            {
                base["dbFactoryName"] = value;
            }
        }

        [ConfigurationProperty("sequenceTable")]
        public string SequenceTable
        {
            get
            {
                return (string)base["sequenceTable"];
            }
            set
            {
                base["sequenceTable"] = value;
            }
        }

        [ConfigurationProperty("sqlBuilder")]
        public string SqlBuilder
        {
            get
            {
                return (string)base["sqlBuilder"];
            }
            set
            {
                base["sqlBuilder"] = value;
            }
        }
    }
}
