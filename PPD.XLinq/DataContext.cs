using PPD.XLinq.Provider;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xinchen.DynamicObject;
using Xinchen.Utils;

namespace PPD.XLinq
{
    public class DataContext
    {
        static Dictionary<Type, Dictionary<string, object>> _dbSets = new Dictionary<Type, Dictionary<string, object>>();
        static Dictionary<Type, Dictionary<string, PropertyInfo>> _dbSetProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        //static IList<Type> _tables;

        //public static IList<Type> Tables
        //{
        //    get { return DataContext._tables; }
        //}
        static Type _dbSetType = typeof(DbSet<>);
        Type _dataContextType;
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

        public static string SequenceTable { get; private set; }

        public DbSet<T> Set<T>()
        {
            Dictionary<string, object> dbSets;
            if (!_dbSets.TryGetValue(_dataContextType, out dbSets))
            {
                throw new Exception("初始有问题");
            }
            object property = null;
            var entityType = typeof(T);
            var typeName = entityType.Name;
            var propertyName = StringHelper.ToPlural(typeName);
            if (!dbSets.TryGetValue(propertyName, out property))
            {
                QueryProvider provider = new QueryProvider(entityType);
                property = new DbSet<T>(provider);
                dbSets.Add(propertyName, property);
            }
            return (DbSet<T>)property;
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
                SequenceTable = pm.SequenceTable;
                if (string.IsNullOrWhiteSpace(SequenceTable))
                {
                    SequenceTable = "Sequences";
                }
            }
        }
        #region 开始
        public DataContext(string name)
        {
            _dataContextType = GetType();
            ConnectionString = ConfigurationManager.ConnectionStrings[name].ConnectionString;
            Dictionary<string, PropertyInfo> dbSetProperties;
            if (!_dbSetProperties.TryGetValue(_dataContextType, out dbSetProperties))
            {
                dbSetProperties = this.GetType().GetProperties().Where(x => x.PropertyType.IsGenericType && x.PropertyType.GetGenericTypeDefinition() == _dbSetType).ToDictionary(x => x.Name);
                _dbSetProperties.Add(_dataContextType, dbSetProperties);
            }
            Dictionary<string, object> dbSets;
            if (!_dbSets.TryGetValue(_dataContextType, out dbSets))
            {
                dbSets = new Dictionary<string, object>();
                foreach (var dbSet in dbSetProperties)
                {
                    QueryProvider provider = new QueryProvider(dbSet.Value.PropertyType.GetGenericArguments()[0]);
                    var set = Activator.CreateInstance(dbSet.Value.PropertyType, provider);
                    dbSets.Add(dbSet.Key, set);
                }
                _dbSets.Add(_dataContextType, dbSets);
            }
            foreach (var dbSetProperty in dbSetProperties)
            {
                dbSetProperty.Value.SetValue(this, dbSets[dbSetProperty.Key]);
            }
        }
        #endregion

        public int SaveChanges()
        {
            Dictionary<string, object> dbSets;
            if (!_dbSets.TryGetValue(_dataContextType, out dbSets))
            {
                throw new Exception("初始有问题");
            }
            var provider = ProviderFactory.CreateProvider(DataContext.DataBase);
            var count = 0;
            using (var scope = new TransactionScope())
            {
                foreach (IOperateAddingEntities item in dbSets.Values)
                {
                    var list = item.GetAddingEntities();
                    count += provider.CreateEntityAdder().InsertEntities(list);
                }
                scope.Complete();
                foreach (IOperateAddingEntities item in dbSets.Values)
                {
                    item.ClearAddingEntities();
                }
            }
            return count;
        }
    }
}
