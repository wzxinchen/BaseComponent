using PPD.XLinq.Provider;
using System;
using System.Collections;
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
        /// <summary>
        /// 缓存DbSet的所有实例，第一个Key为DataContext的类型
        /// </summary>
        static Dictionary<Type, Dictionary<string, object>> _dbSets = new Dictionary<Type, Dictionary<string, object>>();

        /// <summary>
        /// 缓存作为DataContext的DbSet类型的属性集，第一个Key为DataContext的类型
        /// </summary>
        static Dictionary<Type, Dictionary<string, PropertyInfo>> _dbSetProperties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        /// <summary>
        /// 缓存实体类型与DbSet的对应关系
        /// </summary>
        static Dictionary<Type, object> _entitieDbSets = new Dictionary<Type, object>();
        public static bool IsEntity(Type type)
        {
            return _entitieDbSets.Get(type) != null;
        }

        /// <summary>
        /// 根据实体类型获取对应的IEntityOperator
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public IEntityOperator GetEntityOperator(Type entityType)
        {
            return (IEntityOperator)_entitieDbSets.Get(entityType);
        }
        bool _enableProxy = false;

        public bool EnableProxy
        {
            get { return _enableProxy; }
        }

        /// <summary>
        /// 默认查询出来的数据不支持直接修改，通过此方法查询的不超过十条的数据可以支持直接修改
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public TResult QueryEnableProxy<TResult>(Func<TResult> query)
        {
            _enableProxy = true;
            var result = query();
            _enableProxy = false;
            return result;
        }
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
                QueryProvider provider = new QueryProvider(this, entityType);
                property = new DbSet<T>(provider);
                _entitieDbSets.Add(entityType, property);
                dbSets.Add(propertyName, property);
            }
            return (DbSet<T>)property;
        }
        public readonly static List<string> SupportProviders = new List<string>(){
            "SqlServer2008R2"
        };
        private string _connectionStringName;

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
            }
            if (string.IsNullOrWhiteSpace(SequenceTable))
            {
                SequenceTable = "Sequences";
            }
        }
        #region 开始
        public DataContext(string name)
        {
            _connectionStringName = name;
            _dataContextType = GetType();
            ConnectionString = ConfigurationManager.ConnectionStrings[_connectionStringName].ConnectionString;
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
                    var entityType = dbSet.Value.PropertyType.GetGenericArguments()[0];
                    QueryProvider provider = new QueryProvider(this, entityType);
                    var set = Activator.CreateInstance(dbSet.Value.PropertyType, provider);
                    _entitieDbSets.Add(entityType, set);
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
                var op = provider.CreateEntityOperator();
                foreach (IEntityOperator dbSet in dbSets.Values)
                {
                    var list = dbSet.GetAdding();
                    var total = op.InsertEntities(list);
                    if (total != list.Count)
                    {
                        throw new Exception("批量插入失败");
                    }
                    count += total;
                    var editings = dbSet.GetEditing();
                    var entityType = dbSet.GetEntityType();
                    var table = TableInfoManager.GetTable(entityType);
                    var keyColumn = table.Columns.FirstOrDefault(x => x.Value.IsKey).Value;
                    if (keyColumn == null)
                    {
                        throw new InvalidOperationException("实体" + entityType.FullName + "不存在主键，无法更新");
                    }
                    var getters = ExpressionReflector.GetGetters(entityType);
                    var keyGetter = getters.Get(keyColumn.PropertyInfo.Name);
                    if (keyGetter == null)
                    {
                        throw new InvalidOperationException("keyGetter为null");
                    }
                    foreach (var editing in editings)
                    {
                        var iGetUpdatedValue = editing as IGetUpdatedValues;
                        if (iGetUpdatedValue == null)
                        {
                            continue;
                        }
                        var values = iGetUpdatedValue.GetUpdatedValues();
                        if (values.Count <= 0)
                        {
                            continue;
                        }
                        if (values.Get(keyColumn.PropertyInfo.Name) != null)
                        {
                            throw new InvalidOperationException("不允许更新主键");
                        }
                        var keyValue = keyGetter(editing);
                        values.Add(keyColumn.Name, keyValue);
                        op.UpdateValues(keyColumn, table, values);
                    }

                    var removings = dbSet.GetRemoving();
                    var ids = new List<int>();
                    foreach (var removing in removings)
                    {
                        var kv = keyGetter(removing);
                        if (kv == null)
                        {
                            throw new InvalidOperationException("删除时主键必须有值");
                        }
                        ids.Add(Convert.ToInt32(kv));
                    }
                    if (ids.Any())
                    {
                        op.Delete(keyColumn, table, ids.ToArray());
                    }
                }
                scope.Complete();
                foreach (IEntityOperator item in dbSets.Values)
                {
                    item.ClearAdding();
                    item.ClearEditing();
                    item.ClearRemoving();
                }
            }
            return count;
        }

        internal IList QueryResult { get; set; }
    }
}
