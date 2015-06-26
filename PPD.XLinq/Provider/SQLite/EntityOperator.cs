using PPD.XLinq.Provider.SqlServer2008R2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xinchen.DynamicObject;
using Xinchen.Utils;
namespace PPD.XLinq.Provider.SQLite
{
    internal class EntityOperator : IEntityOperator
    {
        ProviderBase _provider;
        SqlExecutorBase _sqlExecutor;
        SqlBuilderBase _sqlBuilder;
        public EntityOperator(ProviderBase provider)
        {
            _provider = provider;
            _sqlExecutor = _provider.CreateSqlExecutor();
            _sqlBuilder = _provider.CreateSqlBuilderFactory().CreateSqlBuilder();
        }
        #region 批量插入
        int IEntityOperator.InsertEntities(ArrayList list)
        {
            if (list.Count <= 0)
            {
                return 0;
            }
            var type = list[0].GetType();
            var table = TableInfoManager.GetTable(type);
            var columns = table.Columns;
            var keyColumn = table.Columns.FirstOrDefault(x => x.Value.IsKey).Value;
            var count = 0;
            var maxIndex = 0;
            bool autoIncreament = keyColumn != null && keyColumn.IsAutoIncreament;
            //SqlExecutor executor = null;
            var getters = ExpressionReflector.GetGetters(type);
            var setters = ExpressionReflector.GetSetters(type);
            Action<object, object> keySetter = null;
            if (keyColumn != null)
            {
                keySetter = setters.Get(keyColumn.PropertyInfo.Name);
            }
            if (!autoIncreament)
            {
                var obj = _sqlExecutor.ExecuteScalar(string.Format("select max(Count) from {0} where Name='{1}'", ConfigManager.SequenceTable, table.Name), new Dictionary<string, object>());
                if (obj == DBNull.Value)
                {
                    _sqlExecutor.ExecuteNonQuery(string.Format("insert into {0}(Name,Count) values('{1}',{2})", ConfigManager.SequenceTable, table.Name, 0), new Dictionary<string, object>());
                }
                else
                {
                    maxIndex = Convert.ToInt32(obj);
                }
            }
            #region 使用Insert语句插入
            int page, limit = 10;
            page = (int)Math.Ceiling(list.Count / (double)limit);
            int pageIndex = 1;
            var insertStart = "insert into {0}({1}) values{2}";
            var tableName = string.Empty;
            if (!string.IsNullOrWhiteSpace(table.DataBase))
            {
                tableName = string.Format("[{0}].", table.DataBase);
            }
            tableName = string.Format("[{0}]", table.Name);
            var fields = new List<string>();
            var autoincreamentColumn = string.Empty;
            foreach (var item in table.Columns.Values)
            {
                if (item.IsAutoIncreament)
                {
                    autoincreamentColumn = item.Name;
                    continue;
                }
                fields.Add(item.Name);
            }
            while (pageIndex <= page)
            {
                var start = (pageIndex - 1) * limit;
                ArrayList entities = null;
                if (start + limit > list.Count)
                {
                    entities = list.GetRange(start, list.Count - start);
                }
                else
                {
                    entities = list.GetRange(start, limit);
                }
                var values = new List<string>();
                var index = 0;
                var sqlParameters = new Dictionary<string, object>();
                foreach (var entity in entities)
                {
                    var value = new List<string>();
                    if (!autoIncreament && keySetter != null)
                    {
                        keySetter(entity, ++maxIndex);
                    }
                    foreach (var key in getters.Keys)
                    {
                        if (autoincreamentColumn == key)
                        {
                            continue;
                        }
                        value.Add(string.Format("@{0}{1}", key, index));
                        var valueParam = getters.Get(key)(entity);
                        var dateValue = valueParam as DateTime?;
                        if (dateValue != null)
                        {
                            if (dateValue.Value.Date == dateValue.Value)
                            {
                                valueParam = dateValue.Value.ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                valueParam = dateValue.Value.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                        sqlParameters.Add(key + index, valueParam);
                    }
                    index++;
                    values.Add(string.Format("({0})", string.Join(",", value)));
                }
                insertStart = string.Format(insertStart, tableName, string.Join(",", fields), string.Join(",", values));
                count += _sqlExecutor.ExecuteNonQuery(insertStart, sqlParameters);
                pageIndex++;
            }

            #endregion
            if (!autoIncreament)
            {
                _sqlExecutor.ExecuteNonQuery(string.Format("update {0} set [Count]={1} where Name='{2}'", ConfigManager.SequenceTable, maxIndex, table.Name), new Dictionary<string, object>());
            }
            return count;
        }
        #endregion

        int IEntityOperator.UpdateValues(SchemaModel.Column keyColumn, SchemaModel.Table table, Dictionary<string, object> values)
        {
            var keyValue = values.Get(keyColumn.Name);
            if (keyValue == null)
            {
                throw new InvalidOperationException("字典未传入主键");
            }
            var updateSql = "UPDATE {0} SET {1} WHERE {2}";
            var tableName = _sqlBuilder.GetTableName(table);
            var sqlParameters = new Dictionary<string, object>();
            var setts = new List<string>();
            var alias = string.Empty;
            foreach (var key in values.Keys)
            {
                if (key == keyColumn.Name)
                {
                    continue;
                }
                alias = ParserUtils.GenerateAlias(key);
                var set = string.Format("[{0}] = @{1}", key, alias);
                sqlParameters.Add(alias, values.Get(key));
                setts.Add(set);
            }
            alias = ParserUtils.GenerateAlias(keyColumn.Name);
            var condition = string.Format("[{0}] = @{1}", keyColumn.Name, alias);
            sqlParameters.Add(alias, keyValue);
            updateSql = string.Format(updateSql, tableName, string.Join(",", setts), condition);
            return _sqlExecutor.ExecuteNonQuery(updateSql, sqlParameters);
        }

        int IEntityOperator.Delete(SchemaModel.Column keyColumn, SchemaModel.Table table, params int[] ids)
        {
            if (ids.Length <= 0) return 0;
            var deleteSql = "DELETE FROM {0} WHERE [{1}] IN ({2})";
            var tableName = _sqlBuilder.GetTableName(table);
            deleteSql = string.Format(deleteSql, tableName, keyColumn.Name, string.Join(",", ids));
            return _sqlExecutor.ExecuteNonQuery(deleteSql, new Dictionary<string, object>());
        }
    }
}
