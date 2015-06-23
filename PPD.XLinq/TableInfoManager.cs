using PPD.XLinq.Attributes;
using PPD.XLinq.SchemaModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xinchen.Utils;

namespace PPD.XLinq
{
    public class TableInfoManager
    {
        static Type _columnAttrType = typeof(ColumnAttribute);
        static Type _tableAttrType = typeof(PPD.XLinq.Attributes.TableAttribute);
        static Type _keyAttrType = typeof(KeyAttribute);
        static Type _dataBaseGeneratedAttrType = typeof(DatabaseGeneratedAttribute);
        static Dictionary<Type, Table> _tableTypeMap = new Dictionary<Type, Table>();
        /// <summary>
        /// 指定类型是否是一个表的类型
        /// </summary>
        /// <returns></returns>
        public static bool IsEntity(Type type)
        {
            return DataContext.IsEntity(type);
        }

        /// <summary>
        /// 将给定的表结构信息转换成完整表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string GetTableName(Table table)
        {
            var tableName = string.Empty;
            if (!string.IsNullOrWhiteSpace(table.DataBase))
            {
                tableName = string.Format("[{0}].dbo.", table.DataBase);
            }
            tableName = string.Format("{0}[{1}]", tableName, table.Name);
            return tableName;
        }
        /// <summary>
        /// 将给定的表结构信息转换成完整表名
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static string GetTableName(TranslateModel.Table table)
        {
            var tableName = string.Empty;
            if (!string.IsNullOrWhiteSpace(table.DataBase))
            {
                tableName = string.Format("[{0}].dbo.", table.DataBase);
            }
            tableName = string.Format("{0}[{1}]", tableName, table.Name);
            return tableName;
        }

        /// <summary>
        /// 根据给定的类型分析表名、数据库名
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static Table GetTable(Type entityType)
        {
            var table = _tableTypeMap.Get(entityType);
            if (table != null)
            {
                return table;
            }
            lock (_tableTypeMap)
            {
                table = _tableTypeMap.Get(entityType);
                if (table != null)
                {
                    return table;
                }
                if (!IsEntity(entityType))
                {
                    throw new Exception("不是实体类型");
                }
                var tableAttr = (PPD.XLinq.Attributes.TableAttribute)entityType.GetCustomAttributes(_tableAttrType, true).FirstOrDefault();
                table = new Table();
                string tableName, dbName = null;
                if (tableAttr == null)
                {
                    tableName = StringHelper.ToPlural(entityType.Name);
                }
                else
                {
                    tableName = tableAttr.Name;
                    dbName = tableAttr.DataBaseName;
                }
                table.DataBase = dbName;
                table.Name = tableName;
                table.Type = entityType;
                GetColumns(table);
                _tableTypeMap.Add(entityType, table);
                return table;
            }
        }

        static void GetColumns(Table table)
        {
            var pis = table.Type.GetProperties();
            var hasKey = false;
            foreach (var property in pis)
            {
                var columnAttr = (ColumnAttribute)property.GetCustomAttributes(_columnAttrType, false).FirstOrDefault();
                var keyAttr = (KeyAttribute)property.GetCustomAttributes(_keyAttrType, true).FirstOrDefault();
                var genAttr = (DatabaseGeneratedAttribute)property.GetCustomAttributes(_dataBaseGeneratedAttrType, true).FirstOrDefault();
                Column column = new Column();
                column.Table = table;
                column.PropertyInfo = property;
                column.Name = property.Name;
                column.IsKey = keyAttr != null;
                if (column.IsKey)
                {
                    hasKey = true;
                }
                if (column.IsKey)
                {
                    column.IsAutoIncreament = true;
                }
                if (genAttr != null)
                {
                    column.IsAutoIncreament = genAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
                    column.IsKey = true;
                    hasKey = true;
                }
                if (columnAttr != null)
                {
                    column.Name = columnAttr.Name;
                }
                table.Columns.Add(column.PropertyInfo.Name, column);
            }
            if (!hasKey)
            {
                var column = table.Columns.Get("Id");
                if (column != null)
                {
                    column.IsKey = true;
                    column.IsAutoIncreament = true;
                }
            }
        }
    }
}
