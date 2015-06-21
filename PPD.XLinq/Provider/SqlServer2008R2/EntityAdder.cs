using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DynamicObject;
using Xinchen.Utils;
namespace PPD.XLinq.Provider.SqlServer2008R2
{
    internal class EntityAdder : EntityAddBase
    {
        internal override int InsertEntities(ArrayList list)
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
            SqlExecutor executor = null;
            var getters = ExpressionReflector.GetGetters(type);
            var setters = ExpressionReflector.GetSetters(type);
            Action<object, object> keySetter = null;
            if (keyColumn != null)
            {
                keySetter = setters.Get(keyColumn.PropertyInfo.Name);
            }
            if (!autoIncreament)
            {
                executor = new SqlExecutor();
                var obj = executor.ExecuteScalar(string.Format("select isnull(max(Count),0) from {0} where Name='{1}'", DataContext.SequenceTable, table.Name), new Dictionary<string, object>());
                executor.ExecuteNonQuery(string.Format("insert into {0}(Name,Count) values('{1}',{2})", DataContext.SequenceTable, table.Name, 0), new Dictionary<string, object>());
                maxIndex = Convert.ToInt32(obj);
            }
            if (list.Count <= 10)
            {
                if(executor==null)
                {
                    executor = new SqlExecutor();
                }
                var insertStart = "insert into {0}({1}) values{2}";
                var tableName = string.Empty;
                if (!string.IsNullOrWhiteSpace(table.DataBase))
                {
                    tableName = string.Format("[{0}]", table.DataBase);
                }
                tableName = string.Format("dbo.[{0}]", table.Name);
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
                var values = new List<string>();
                var index = 0;
                var sqlParameters = new Dictionary<string, object>();
                foreach (var entity in list)
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
                        sqlParameters.Add(key + index, getters.Get(key)(entity));
                    }
                    index++;
                    values.Add(string.Format("({0})", string.Join(",", value)));
                }
                insertStart = string.Format(insertStart, tableName, string.Join(",", fields), string.Join(",", values));
                count = executor.ExecuteNonQuery(insertStart, sqlParameters);
            }
            else
            {
                var sqlBulkCopy = new SqlBulkCopy(DataContext.ConnectionString);
                sqlBulkCopy.DestinationTableName = "dbo.[" + table.Name + "]";
                sqlBulkCopy.BatchSize = list.Count / 10;
                var dataTable = new DataTable();
                foreach (var column in table.Columns.Values)
                {
                    dataTable.Columns.Add(column.Name);
                }
                foreach (var item in list)
                {
                    var row = dataTable.NewRow();
                    if (!autoIncreament && keySetter != null)
                    {
                        keySetter(item, ++maxIndex);
                    }
                    foreach (var key in getters.Keys)
                    {
                        row[columns.Get(key).Name] = getters.Get(key)(item);
                    }
                    dataTable.Rows.Add(row);
                }
                sqlBulkCopy.WriteToServer(dataTable);
                sqlBulkCopy.Close();
                count = list.Count;
            }
            if (!autoIncreament)
            {
                executor.ExecuteNonQuery(string.Format("update {0} set [Count]={1} where Name='{2}'", DataContext.SequenceTable, maxIndex, table.Name), new Dictionary<string, object>());
            }
            return count;
        }
    }
}
