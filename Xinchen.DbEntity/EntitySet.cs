namespace Xinchen.DbEntity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Transactions;
    using Xinchen.DbUtils;
    using Xinchen.DynamicObject;
    using Xinchen.ObjectMapper;
    using Xinchen.Utils;
    using Xinchen.Utils.DataAnnotations;
    using Xinchen.Utils.Entity;

    public class EntitySet<TEntity> : IEntitySet<TEntity>
    {
        private PropertyInfo _autoIncrementProperty;
        private EntityMapper<TEntity> _entityMapper;
        private Type _entityType;
        private Xinchen.DbEntity.DbHelper _helper;
        private PropertyInfo _keyProperty;
        private PropertyInfo[] _properties;
        private Dictionary<string, PropertyInfo> _propertyDict;
        /// <summary>
        /// 已加过[]
        /// </summary>
        private string[] _propertyNames;
        private TableAttribute _tableAttr;
        private static Type _tableAttrType;

        /// <summary>
        /// 获取select子句
        /// </summary>
        /// <param name="condtion"></param>
        /// <returns></returns>
        public string GetSelectSql(string condition)
        {
            string sql = "select " + string.Join(",", _propertyNames) + " from " + TableName;
            if(!string.IsNullOrWhiteSpace(condition))
            {
                sql += " where " + condition;
            }
            return sql;
        }

        static EntitySet()
        {
            EntitySet<TEntity>._tableAttrType = typeof(TableAttribute);
        }
        public EntitySet(EntityContext context)
        {
            this._helper = Xinchen.DbEntity.DbHelper.GetInstance(context.DbHelper.ConnectionString);
            this._entityType = typeof(TEntity);
            this._entityMapper = new EntityMapper<TEntity>();
            this._properties = this._entityMapper.Properties;
            this._propertyNames = new string[this._properties.Length];
            this._propertyDict = new Dictionary<string, PropertyInfo>();
            for (int i = 0; i < this._properties.Length; i++)
            {
                this._propertyNames[i] = "[" + this._properties[i].Name + "]";
                this._propertyDict.Add(this._properties[i].Name, this._properties[i]);
            }
            this._tableAttr = (TableAttribute)this._entityType.GetCustomAttributes(EntitySet<TEntity>._tableAttrType, true).FirstOrDefault<object>();
            this._keyProperty = this._properties.FirstOrDefault<PropertyInfo>(propertyInfo => AttributeHelper.GetAttribute<KeyAttribute>(propertyInfo) != null);
            if (this._keyProperty == null)
            {
                throw new EntityException("实体没有主键:" + TypeName);
            }
            this._autoIncrementProperty = this._properties.FirstOrDefault<PropertyInfo>(x => AttributeHelper.GetAttribute<AutoIncrementAttribute>(x) != null);
        }

        //private string BuildSql(string sql, List<IDbDataParameter> dbParameters, params DynamicSqlParam[] sqlParams)
        //{
        //    List<string> conditions = new List<string>();
        //    List<string> sorts = new List<string>();
        //    this.BuildSql(conditions, dbParameters, sqlParams);
        //    this.BuildSql(sorts, sqlParams);
        //    return this.BuildSql(sql, conditions, sorts);
        //}

        //private string BuildSql(string sql, List<string> conditions, List<string> orderbys)
        //{
        //    sql = sql.ToLower();
        //    if (conditions.Count > 0)
        //    {
        //        if (sql.IndexOf("{@dynamicCondition}", StringComparison.InvariantCultureIgnoreCase) > 0)
        //        {
        //            sql = Regex.Replace(sql, "{@dynamicCondition}", " and " + string.Join(" and ", conditions), RegexOptions.IgnoreCase);
        //        }
        //        else
        //        {
        //            if (!sql.EndsWith("where "))
        //            {
        //                if (sql.EndsWith("where"))
        //                {
        //                    sql = sql + " ";
        //                }
        //                else
        //                {
        //                    sql = sql + " where ";
        //                }
        //            }
        //            sql = sql + string.Join(" and ", conditions);
        //        }
        //    }
        //    else if (sql.IndexOf("{@dynamicCondition}", StringComparison.InvariantCultureIgnoreCase) > 0)
        //    {
        //        sql = Regex.Replace(sql, "{@dynamicCondition}", string.Empty, RegexOptions.IgnoreCase);
        //    }
        //    if (orderbys.Count > 0)
        //    {
        //        sql = sql + " order by " + string.Join(",", orderbys);
        //    }
        //    return sql;
        //}

        ///// <summary>
        ///// 将给定的DynamicSqlParam参数中的排序参数转换成条件字符串列表
        ///// </summary>
        ///// <param name="sorts">用于返回转换结果集</param>
        ///// <param name="sqlParams">要转换的动态sql参数</param>
        //private void BuildSql(List<string> sorts, params DynamicSqlParam[] sqlParams)
        //{
        //    foreach (DynamicSqlSorter sorter in from x in sqlParams
        //                                        where x.Type == DynamicSqlParamType.Sort
        //                                        select x)
        //    {
        //        sorts.Add(sorter.ToSql());
        //    }
        //}

        /// <summary>
        /// 将提供的不带条件子句的sql语句拼接为完整的sql语句
        /// </summary>
        /// <param name="sql">原始sql</param>
        /// <param name="filterLinked">条件链表</param>
        /// <param name="sort">排序对象</param>
        /// <returns></returns>
        string BuildSql(string sql, FilterLinked filterLinked, Sort sort, List<IDbDataParameter> dbParameters)
        {
            if (dbParameters == null)
            {
                throw new ArgumentNullException("dbParameters");
            }
            StringBuilder sqlBuilder = new StringBuilder(sql);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (filterLinked != null)
            {
                string condition = filterLinked.ToString(parameters);
                foreach (var parameterName in parameters.Keys)
                {
                    dbParameters.Add(CreateParameter(parameterName, parameters[parameterName]));
                }
                sqlBuilder.Append(" where ").Append(condition);
            }
            if (sort != null)
            {
                string order = sort.ToString();
                sqlBuilder.Append(" order by ").Append(order);
            }
            return sqlBuilder.ToString();
        }

        ///// <summary>
        ///// 将给定的DynamicSqlParam参数中的搜索参数转换成条件字符串列表，参数化查询的值存入IDbDataParameter集合中
        ///// </summary>
        ///// <param name="conditions">用于返回转换结果集</param>
        ///// <param name="dbParameters">用于返回参数化查询值集</param>
        ///// <param name="sqlParams">要转换的动态sql参数</param>
        //private void BuildSql(List<string> conditions, List<IDbDataParameter> dbParameters, params DynamicSqlParam[] sqlParams)
        //{
        //    foreach (DynamicSqlFilter filter in from x in sqlParams
        //                                        where x.Type == DynamicSqlParamType.Filter
        //                                        select x)
        //    {
        //        Dictionary<string, object> dictionary;
        //        dictionary = new Dictionary<string, object>();
        //        conditions.Add(filter.ToSql(dictionary));
        //        foreach (KeyValuePair<string, object> pair in dictionary)
        //        {
        //            dbParameters.Add(this.CreateParameter("@" + pair.Key, pair.Value));
        //        }
        //    }
        //}

        public int Count(string sql, params IDbDataParameter[] dbParameters)
        {
            return this._helper.ExecuteCount(sql, dbParameters);
        }

        public TEntity Create(TEntity entity)
        {
            Dictionary<string, object> modifiedProperties;
            if (DynamicProxy.IsProxy(entity.GetType()))
            {
                modifiedProperties = DynamicProxy.GetModifiedProperties(entity);
            }
            else
            {
                modifiedProperties = this._entityMapper.GetPropertyValues(entity);
            }
            List<string> values = new List<string>();
            List<string> list2 = new List<string>();
            List<IDbDataParameter> list3 = new List<IDbDataParameter>();
            foreach (PropertyInfo info in this._properties)
            {
                if (this._autoIncrementProperty != info)
                {
                    values.Add("[" + info.Name + "]");
                    list2.Add("@" + info.Name);
                    IDbDataParameter item = this._helper.CreateParameter(Xinchen.DbEntity.DbHelper.TypeMapper[info.PropertyType], "@" + info.Name, DBNull.Value);
                    if (modifiedProperties.ContainsKey(info.Name))
                    {
                        item.Value = modifiedProperties[info.Name];
                    }
                    if (this._keyProperty == info)
                    {
                        int result = 0;
                        if (int.TryParse(Convert.ToString(item.Value), out result) && (result <= 0))
                        {
                            throw new EntityException("检测到主键值为" + result.ToString() + "，是否忘记给主键赋值？实体名：" + this._entityType.Name);
                        }
                    }
                    if (item.Value == null)
                    {
                        item.Value = DBNull.Value;
                    }
                    list3.Add(item);
                }
            }
            string sql = string.Format("insert into {0} ({1}) values({2});", "[" + this.TableName + "]", string.Join(",", values), string.Join(",", list2));
            if (this._autoIncrementProperty != null && DbHelper.ProviderName != "System.Data.OleDb")
            {
                sql = sql + "SELECT @@IDENTITY;";
                int num2 = this._helper.ExecuteScalarCount(sql, list3.ToArray());
                if (this._autoIncrementProperty != null)
                {
                    this._entityMapper.SetValue(entity, this._autoIncrementProperty.Name, num2);
                }
            }
            else
            {
                this._helper.ExecuteUpdate(sql, list3.ToArray());
            }
            return DynamicProxy.CreateDynamicProxy<TEntity>(entity);
        }

        public IDbDataParameter CreateParameter(string field, object value)
        {
            Type propertyType = null;
            if (this._propertyDict.ContainsKey(field))
            {
                PropertyInfo info = this._propertyDict[field];
                propertyType = info.PropertyType;
            }
            else
            {
                propertyType = value.GetType();
            }
            IDbDataParameter parameter = this._helper.CreateParameter(Xinchen.DbEntity.DbHelper.TypeMapper[propertyType], field, value);
            if (parameter.Value == null)
            {
                parameter.Value = DBNull.Value;
            }
            return parameter;
        }

        public TEntity CreateProxy()
        {
            return (TEntity)Activator.CreateInstance(DynamicProxy.CreateDynamicProxyType<TEntity>());
        }

        public TEntity CreateProxy(TEntity entity)
        {
            return DynamicProxy.CreateDynamicProxy<TEntity>(entity);
        }

        public void Delete(TEntity group)
        {
            Dictionary<string, object> propertyValues = this._entityMapper.GetPropertyValues(group);
            if (this._keyProperty != null)
            {
                propertyValues.Remove(this._keyProperty.Name);
            }
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            List<string> list2 = this.ParseFromProxyEntityDictionary(propertyValues, dbParameters);
            string sql = "delete from dbo." + this.TableName + " where " + string.Join(" and ", list2.ToArray());
            this._helper.ExecuteUpdate(sql, dbParameters.ToArray());
        }

        public void DeleteAll()
        {
            string sql = "delete from [" + this.TableName + "]";
            this._helper.ExecuteUpdate(sql);
        }

        public void DeleteBy(Action<TEntity> where)
        {
            TEntity local = (TEntity)Activator.CreateInstance(DynamicProxy.CreateDynamicProxyType<TEntity>());
            where(local);
            Dictionary<string, object> modifiedProperties = DynamicProxy.GetModifiedProperties(local);
            this.DeleteBy(modifiedProperties);
        }

        public void DeleteBy(string where)
        {
            if (string.IsNullOrEmpty(where))
            {
                throw new ArgumentNullException("where");
            }
            string sql = "delete from [" + this.TableName + "] where " + where;
            this._helper.ExecuteUpdate(sql);
        }

        public void DeleteBy(Dictionary<string, object> dict)
        {
            List<string> values = new List<string>();
            List<IDbDataParameter> list2 = new List<IDbDataParameter>();
            foreach (string str in dict.Keys)
            {
                values.Add("[" + str + "]=@" + str);
                Type propertyType = this._propertyDict[str].PropertyType;
                IDbDataParameter item = this._helper.CreateParameter("@" + str, Xinchen.DbEntity.DbHelper.TypeMapper[propertyType], dict[str]);
                if (item.Value == null)
                {
                    item.Value = DBNull.Value;
                }
                list2.Add(item);
            }
            string sql = "delete from [" + this.TableName + "] where " + string.Join(" and ", values);
            this._helper.ExecuteUpdate(sql, list2.ToArray());
        }

        public void DeleteById(params int[] id)
        {
            string sql = string.Format("delete from [" + this.TableName + "] where id in ({0})", string.Join<int>(",", id));
            this._helper.ExecuteUpdate(sql);
        }

        public bool Exists(string sql, params IDbDataParameter[] dbParameters)
        {
            if (Regex.Match(sql, @"select.*?Count\(.*?\) from").Success)
            {
                throw new ArgumentException("调用EntitySet.Exists方法时不要使用select count子句");
            }
            return (this._helper.ExecuteCount(sql, dbParameters) > 0);
        }

        //public bool Exists(string sql, params IDbDataParameter[] dbParameters)
        //{
        //    return (this._helper.ExecuteScalarCount(sql, dbParameters) > 0);
        //}

        public bool Exists(int id)
        {
            string sql = string.Format("select {0} from {1} where {2} = {3}", string.Join(",", this._propertyNames), TableName, _keyProperty.Name, id);
            return this.Exists(sql);
        }

        //public bool Exists(string where)
        //{
        //    string sql = "select count(1) from [" + this.TableName + "] where " + where;
        //    return this.Exists(sql);
        //}

        public bool Exists(Action<TEntity> where)
        {
            TEntity local = (TEntity)Activator.CreateInstance(DynamicProxy.CreateDynamicProxyType<TEntity>());
            where(local);
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            string condition = this.ParseConditionFromProxyEntity(local, dbParameters);
            string sql = "select " + string.Join(",", _properties.Select(x => x.Name)) + " from " + TableName;
            if (!string.IsNullOrWhiteSpace(condition))
            {
                sql += " where " + condition;
            }
            return this.Exists(sql, dbParameters.ToArray());
        }

        //public bool Exists(string where, params IDbDataParameter[] dbParameters)
        //{
        //    return this.Exists("select count(1) [" + this.TableName + "] where " + where, dbParameters);
        //}

        public IList<TEntity> GetList()
        {
            return this.GetList(string.Join(",", this._propertyNames), "1=1", null, (string)null);
        }

        public IList<TEntity> GetList(Action<TEntity> where)
        {
            TEntity local = (TEntity)Activator.CreateInstance(DynamicProxy.CreateDynamicProxyType<TEntity>());
            where(local);
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            string strWhere = this.ParseConditionFromProxyEntity(local, dbParameters);
            return this.GetList(string.Join(",", this._propertyNames), strWhere, null, dbParameters.ToArray());
        }

        public IList<TEntity> GetList(Dictionary<string, object> modifiedProperties)
        {
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            List<string> values = this.ParseFromProxyEntityDictionary(modifiedProperties, dbParameters);
            return this.GetList(string.Join(",", this._propertyNames), string.Join(" and ", values), null, dbParameters.ToArray());
        }

        public IList<TCustom> GetList<TCustom>(string sql)
        {
            return this.GetList<TCustom>(sql, new IDbDataParameter[0]);
        }

        public IList<TEntity> GetList(string sql, params IDbDataParameter[] dbParameters)
        {
            DataSet ds = this._helper.ExecuteQuery(sql, dbParameters);
            IList<TEntity> list = Mapper.MapList<TEntity>(ds);
            if (list.Count <= 10)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = DynamicProxy.CreateDynamicProxy<TEntity>(list[i]);
                }
            }
            return list;
        }

        public IList<TModel> GetList<TModel>(string sql, params IDbDataParameter[] dbParameters)
        {
            return Mapper.MapList<TModel>(this._helper.ExecuteQuery(sql, dbParameters));
        }

        public IList<TEntity> GetList(string sql, FilterLinked filterLinked = null, Sort sort = null)
        {
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            sql = this.BuildSql(sql, filterLinked, sort, dbParameters);
            return this.GetList(sql, dbParameters.ToArray());
        }

        public IList<TModel> GetList<TModel>(string sql, FilterLinked filterLinked = null, Sort sort = null)
        {
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            sql = this.BuildSql(sql, filterLinked, sort, dbParameters);
            DataSet ds = this._helper.ExecuteQuery(sql, dbParameters);
            return Mapper.MapList<TModel>(ds);
        }

        public IDictionary<TKey, TModel> GetDictionary<TModel, TKey>(string sql, Func<TModel, TKey> keySelector, FilterLinked filterLinked, Sort sort)
        {
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            sql = this.BuildSql(sql, filterLinked, sort, dbParameters);
            DataSet ds = this._helper.ExecuteQuery(sql);
            return Mapper.Map<TKey, TModel>(ds, x => keySelector(x));
        }

        public IList<TEntity> GetList(string fields, string strWhere, string strOrder = null, string strGroup = null)
        {
            if (string.IsNullOrEmpty(fields))
            {
                throw new ArgumentNullException("fields");
            }
            string sql = new SqlBuilder(this.TableName, fields) { Where = strWhere, Order = strOrder, Group = strGroup }.BuildSql();
            return this.GetList(sql, new IDbDataParameter[0]);
        }

        public IList<TEntity> GetList(string fields, string strWhere, string strOrder = null, params IDbDataParameter[] dbParameters)
        {
            if (string.IsNullOrEmpty(fields))
            {
                throw new ArgumentNullException("fields");
            }
            string sql = new SqlBuilder(this.TableName, fields) { Where = strWhere, Order = strOrder }.BuildSql();
            return this.GetList(sql, dbParameters);
        }

        //public IList<TEntity> GetBy(string fields, Action<TEntity> where, string strOrder = null, string strGroup = null)
        //{
        //    TEntity local = (TEntity)Activator.CreateInstance(DynamicProxy.CreateDynamicProxyType<TEntity>());
        //    where(local);
        //    List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
        //    string strWhere = this.ParseConditionFromProxyEntity(local, dbParameters);
        //    return this.GetList(fields, strWhere, strOrder, strGroup);
        //}

        public TEntity GetSingle(int id)
        {
            return this.GetList(string.Join(",", this._propertyNames), this._keyProperty.Name + "=" + id.ToString(), null, (string)null).FirstOrDefault<TEntity>();
        }

        public TEntity GetSingle(Action<TEntity> condition)
        {
            TEntity local = (TEntity)Activator.CreateInstance(DynamicProxy.CreateDynamicProxyType<TEntity>());
            condition(local);
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            string strWhere = this.ParseConditionFromProxyEntity(local, dbParameters);
            return GetList(string.Join(",", this._propertyNames), strWhere, null, dbParameters.ToArray()).FirstOrDefault();
        }

        public IDictionary<TKey, TEntity> GetDictionary<TKey>(Func<TEntity, TKey> keySelector)
        {
            return this.GetDictionaryBy<TKey>(string.Join(",", this._propertyNames), "1=1", keySelector, null, null);
        }

        public IDictionary<TKey, TEntity> GetDictionaryBy<TKey>(string sql, Func<TEntity, TKey> keySelector, params IDbDataParameter[] dbParameters)
        {
            DataSet ds = this._helper.ExecuteQuery(sql, dbParameters);
            return this._entityMapper.Map<TKey>(ds, keySelector);
        }

        public IDictionary<TKey, TEntity> GetDictionaryBy<TKey>(string sql, Func<TEntity, TKey> keySelector, FilterLinked filterLinked = null, Sort sort = null)
        {
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            sql = this.BuildSql(sql, filterLinked, sort, dbParameters);
            return this.GetDictionaryBy<TKey>(sql, keySelector, dbParameters.ToArray());
        }

        //public Dictionary<TKey, TModel> GetDictionaryBy<TModel, TKey>(string sql, Func<TModel, TKey> keySelector, params DynamicSqlParam[] sqlParams) where TModel : class, new()
        //{
        //    List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
        //    sql = this.BuildSql(sql, dbParameters, sqlParams);
        //    DataSet ds = this._helper.ExecuteQuery(sql);
        //    return this.EntityMapper.Map<TModel, TKey>(ds, keySelector);
        //}
        public IDictionary<TKey, TModel> GetDictionaryBy<TModel, TKey>(string sql, Func<TModel, TKey> keySelector, FilterLinked sqlFilter = null, Sort sort = null)
        {
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            sql = this.BuildSql(sql, sqlFilter, sort, dbParameters);
            DataSet ds = this._helper.ExecuteQuery(sql);
            return Mapper.Map<TKey, TModel>(ds, x => keySelector(x));
        }

        private IDictionary<TKey, TEntity> GetDictionaryBy<TKey>(string fields, string strWhere, Func<TEntity, TKey> keySelector, string strOrder = null, string strGroup = null)
        {
            if (string.IsNullOrEmpty(fields))
            {
                throw new ArgumentNullException("fields");
            }
            string sql = new SqlBuilder(this.TableName, fields) { Where = strWhere, Order = strOrder, Group = strGroup }.BuildSql();
            return this.GetDictionaryBy<TKey>(sql, keySelector, new IDbDataParameter[0]);
        }

        public TFieldType GetMax<TFieldType>(string field)
        {
            string sql = "select max(" + field + ") from [" + this.TableName + "]";
            return (TFieldType)this._helper.ExecuteScalar(sql);
        }

        public int GetSequenceId()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    string sql = "select [count] from dbo.Sequences where Name='" + this.TableName + "'";
                    DataSet ds = this._helper.ExecuteQuery(sql);
                    if (Util.HasRow(ds))
                    {
                        int num = ConvertHelper.ToInt32(ds.Tables[0].Rows[0][0]) + 1;
                        sql = "update dbo.Sequences set [Count]=" + num.ToString() + " where Name='" + this.TableName + "'";
                        this._helper.ExecuteUpdate(sql);
                        return num;
                    }
                    sql = "insert into dbo.[Sequences](Name,[Count]) values('" + this.TableName + "',1);select @@IDENTITY";
                    this._helper.ExecuteUpdate(sql);
                    return 1;
                }
                finally
                {
                    scope.Complete();
                }
            }
        }

        public bool IsEmpty()
        {
            return (this._helper.ExecuteScalarCount("select count(1) from [" + this.TableName + "] ", new IDbDataParameter[0]) <= 0);
        }

        /// <summary>
        /// 进行分页查询
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sql"></param>
        /// <param name="filterLinked">条件链表</param>
        /// <param name="sort">排序规则</param>
        /// <returns></returns>
        public PageResult<TModel> Page<TModel>(int page, int rows, string sql, FilterLinked filterLinked = null, Sort sort = null)
        {
            if (sort == null)
            {
                throw new ArgumentNullException("sort");
            }
            PageResult<TModel> result = new PageResult<TModel>();
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            //List<string> conditions = new List<string>();
            //List<string> sorts = new List<string>();
            //this.BuildSql(conditions, dbParameters, sqlParams);
            //this.BuildSql(sorts, sqlParams);
            //if (sqlParams.Count<DynamicSqlParam>(x => (x.Type == DynamicSqlParamType.Sort)) <= 0)
            //{
            //    throw new ArgumentException("分页时至少指定一个排序规则");
            //}
            sql = BuildSql(sql, filterLinked, sort, dbParameters);
            result.Page = page;
            result.RecordCount = Count(sql, dbParameters.ToArray());
            result.PageCount = (int)Math.Ceiling((double)(((double)result.RecordCount) / ((double)rows)));
            string str = Regex.Replace(sql.TrimStart(), "^select?", "SELECT top 100 percent ROW_NUMBER() OVER(ORDER BY " + string.Join(",", sort.ToString()) + ") _index,", RegexOptions.IgnoreCase);
            object[] objArray = new object[] { "select * from (", str, ") pagerTmp where pagerTmp._index between ", (((page - 1) * rows) + 1).ToString(), " and ", page * rows };
            str = string.Concat(objArray);
            result.Data = this.GetList<TModel>(str, dbParameters.ToArray());
            return result;
        }

        public PageResult<TEntity> Page(int page, int rows, string sql, FilterLinked filterLinked = null, Sort sort = null)
        {
            return Page<TEntity>(page, rows, sql, filterLinked, sort);
        }

        private string ParseConditionFromProxyEntity(TEntity proxyEntity, List<IDbDataParameter> dbParameters)
        {
            Dictionary<string, object> modifiedProperties = DynamicProxy.GetModifiedProperties(proxyEntity);
            List<string> values = this.ParseFromProxyEntityDictionary(modifiedProperties, dbParameters);
            return string.Join(" and ", values);
        }

        private List<string> ParseFromProxyEntityDictionary(Dictionary<string, object> dict, List<IDbDataParameter> dbParameters)
        {
            List<string> list = new List<string>();
            foreach (string str in dict.Keys)
            {
                Type propertyType = this._propertyDict[str].PropertyType;
                IDbDataParameter item = this._helper.CreateParameter("@" + str, Xinchen.DbEntity.DbHelper.TypeMapper[propertyType], dict[str]);
                if (dict[str] != null)
                {
                    list.Add("[" + str + "]=@" + str);
                }
                else
                {
                    list.Add("[" + str + "] is null");
                    item.Value = DBNull.Value;
                }
                dbParameters.Add(item);
            }
            return list;
        }

        private string ParseSetStringFromProxEntity(TEntity proxyEntity, List<IDbDataParameter> dbParameters)
        {
            Dictionary<string, object> modifiedProperties = DynamicProxy.GetModifiedProperties(proxyEntity);
            List<string> values = this.ParseFromProxyEntityDictionary(modifiedProperties, dbParameters);
            return string.Join(",", values);
        }

        public int ScalarCount(Action<TEntity> where)
        {
            TEntity local = DynamicProxy.CreateDynamicProxy<TEntity>();
            where(local);
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            string str = this.ParseConditionFromProxyEntity(local, dbParameters);
            string sql = "select count(1) from " + this.TableName;
            if (!string.IsNullOrEmpty(str))
            {
                sql = sql + " where " + str;
            }
            return this._helper.ExecuteScalarCount(sql, dbParameters.ToArray());
        }

        public void Update(TEntity entity)
        {
            Dictionary<string, object> modifiedProperties;
            int id = 0;
            if (this._entityType.FullName == entity.GetType().BaseType.FullName)
            {
                modifiedProperties = DynamicProxy.GetModifiedProperties(entity);
                id = Convert.ToInt32(this._entityMapper.GetValue(entity, this._keyProperty.Name));
            }
            else
            {
                modifiedProperties = this._entityMapper.GetPropertyValues(entity);
                id = Convert.ToInt32(modifiedProperties[this._keyProperty.Name]);
            }
            this.Update(modifiedProperties, id);
        }

        private void Update(Dictionary<string, object> dict, int id)
        {
            string str = this._keyProperty.Name + "=@" + this._keyProperty.Name;
            string format = "update {0} set {1} where " + str;
            List<string> values = new List<string>();
            List<IDbDataParameter> list2 = new List<IDbDataParameter>();
            foreach (string str3 in dict.Keys)
            {
                if (this._keyProperty.Name != str3)
                {
                    values.Add("[" + str3 + "]=@" + str3);
                    list2.Add(this.CreateParameter(str3, dict[str3]));
                }
            }
            if (id != 0)
            {
                list2.Add(this.CreateParameter(this._keyProperty.Name, id));
            }
            format = string.Format(format, "[" + this.TableName + "]", string.Join(",", values));
            this.Update(format, list2.ToArray());
        }

        public void Update(Action<TEntity> where, Action<TEntity> update)
        {
            TEntity whereProxy = DynamicProxy.CreateDynamicProxy<TEntity>();
            TEntity updateProxy = DynamicProxy.CreateDynamicProxy<TEntity>();
            where(whereProxy);
            update(updateProxy);
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            List<IDbDataParameter> list2 = new List<IDbDataParameter>();
            string str = this.ParseConditionFromProxyEntity(whereProxy, dbParameters);
            string str2 = this.ParseSetStringFromProxEntity(updateProxy, list2);
            string sql = "update [" + this.TableName + "] set " + str2 + " where " + str;
            list2.AddRange(dbParameters);
            this._helper.ExecuteUpdate(sql, list2.ToArray());
        }

        public void Update(string sql, params IDbDataParameter[] dbParameters)
        {
            this._helper.ExecuteUpdate(sql, dbParameters);
        }

        public Xinchen.DbEntity.DbHelper DbHelper
        {
            get
            {
                return this._helper;
            }
        }

        public EntityMapper<TEntity> EntityMapper
        {
            get
            {
                return this._entityMapper;
            }
        }

        public string TableName
        {
            get
            {
                return ((this._tableAttr == null) ? this._entityType.Name : this._tableAttr.TableName);
            }
        }

        public string TypeName
        {
            get
            {
                return this._entityType.Name;
            }
        }
    }
}

