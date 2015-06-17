using PPD.XLinq.Attributes;
using PPD.XLinq.SchemaModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public class TableInfoManager
    {
        static Type _columnAttrType = typeof(ColumnAttribute);
        static Type _tableAttrType = typeof(PPD.XLinq.Attributes.TableAttribute);
        static Dictionary<Type, Table> _tableTypeMap = new Dictionary<Type, Table>();

        /// <summary>
        /// 指定类型是否是一个表的类型
        /// </summary>
        /// <returns></returns>
        public static bool IsEntity(Type type)
        {
            return DataContext.Tables.Contains(type);
        }

        /// <summary>
        /// 根据给定的类型分析表名、数据库名
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static Table GetTable(Type entityType)
        {
            if (_tableTypeMap.ContainsKey(entityType))
            {
                return _tableTypeMap[entityType];
            }
            lock (_tableTypeMap)
            {
                if (_tableTypeMap.ContainsKey(entityType))
                {
                    return _tableTypeMap[entityType];
                }
                var tableAttr = (PPD.XLinq.Attributes.TableAttribute)entityType.GetCustomAttributes(_tableAttrType, true).FirstOrDefault();
                Table table = new Table();
                string tableName, dbName = null;
                if (tableAttr == null)
                {
                    tableName = entityType.Name;
                }
                else
                {
                    tableName = tableAttr.Name;
                    dbName = tableAttr.DataBaseName;
                }
                table.DataBase = dbName;
                table.Name = tableName;
                table.Type = entityType;
                table.Columns.AddRange(GetColumns(table));
                _tableTypeMap.Add(entityType, table);
                return table;
            }
        }

        static List<Column> GetColumns(Table table)
        {
            var columns = new List<Column>();
            var pis = table.Type.GetProperties();
            foreach (var property in pis)
            {
                var columnAttr = (ColumnAttribute)property.GetCustomAttributes(_columnAttrType, false).FirstOrDefault();
                Column column = new Column();
                column.Table = table;
                column.PropertyInfo = property;
                column.Name = property.Name;
                if(columnAttr!=null)
                {
                    column.Name = columnAttr.Name;
                }
                columns.Add(column);
            }
            return columns;
        }
    }
}
