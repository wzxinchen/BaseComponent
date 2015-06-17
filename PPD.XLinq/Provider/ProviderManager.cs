using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider
{
    public class ProviderManager : ConfigurationSection
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
    }
}
