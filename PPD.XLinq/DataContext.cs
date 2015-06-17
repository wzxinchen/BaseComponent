using PPD.XLinq.Provider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public class DataContext
    {
        static IList<PropertyInfo> _dbsetProperties;
        static IList<Type> _tables;

        public static IList<Type> Tables
        {
            get { return DataContext._tables; }
        }
        static Type _dbSetType = typeof(DbSet<>);
        public static string DataBase
        {
            get;
            private set;
        }

        public static string DbFactoryName
        {
            get;
            private set;
        }
        public static string ConnectionString
        {
            get;
            private set;
        }
        public readonly static List<string> SupportProviders = new List<string>(){
            "SqlServer2008R2"
        };

        static DataContext()
        {
            var pm = ConfigurationManager.GetSection("xlinq") as ProviderManager;
            if (pm == null || string.IsNullOrWhiteSpace(pm.DataBase))
            {
                DataBase = "SqlServer2008R2";
                DbFactoryName = "System.Data.SqlClient";
            }
            else if (SupportProviders.Contains(pm.DataBase))
            {
                DataBase = pm.DataBase;
                DbFactoryName = pm.DbFactoryName;
            }
            else
            {
                throw new NotSupportedException("只支持以下提供者：" + string.Join(",", SupportProviders));
            }
            if (pm != null)
            {
                ConnectionString = ConfigurationManager.ConnectionStrings[pm.ConnectionStringName].ConnectionString;
            }
        }
        public DataContext(string name)
        {
            if (_dbsetProperties == null)
            {
                _dbsetProperties = this.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)).ToList();
                _tables = _dbsetProperties.Select(x => x.PropertyType.GetGenericArguments()[0]).ToList();
            }
            ConnectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
            foreach (var dbSet in _dbsetProperties)
            {
                QueryProvider provider = new QueryProvider(dbSet.PropertyType.GetGenericArguments()[0]);
                var set = Activator.CreateInstance(dbSet.PropertyType, provider);
                dbSet.SetValue(this, set);
            }
        }
    }
}
