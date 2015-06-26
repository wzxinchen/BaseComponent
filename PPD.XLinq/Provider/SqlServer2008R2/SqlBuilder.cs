using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xinchen.Utils;
namespace PPD.XLinq.Provider.SqlServer2008R2
{
    public class SqlBuilder : SqlBuilderBase
    {

        protected override void BuildSelectSql()
        {
            var columns = _context.Columns;
            var conditions = _context.Conditions;
            var joins = _context.Joins;
            var fromBuilder = new StringBuilder("FROM ");
            var selectBuilder = new StringBuilder("SELECT ");
            if (_context.Distinct)
            {
                selectBuilder.Append(" DISTINCT ");
            }

            else if (_context.Take > 0)
            {
                selectBuilder.Append(" TOP " + _context.Take + " ");
            }
            var whereBuilder = new StringBuilder();
            var sortBuilder = new StringBuilder();
            var sqlBuilder = new StringBuilder();
            if (joins != null && joins.Count > 0)
            {
                var firstJoin = joins.Values.FirstOrDefault();
                var leftColumn = firstJoin.Left;
                var leftTable = leftColumn.Table;
                fromBuilder.Append(GetTableName(leftTable));
                fromBuilder.AppendFormat(" [{0}]", leftTable.Alias);
                if (_context.NoLockTables.Contains(leftTable.Name))
                {
                    fromBuilder.Append(" WITH (NOLOCK) ");
                }
                fromBuilder.Append(GenJoinType(firstJoin.JoinType));
                var rightColumn = firstJoin.Right;
                var rightTable = rightColumn.Table;
                fromBuilder.Append(GetTableName(rightTable));
                fromBuilder.AppendFormat(" [{0}]", rightTable.Alias);
                if (_context.NoLockTables.Contains(rightTable.Name))
                {
                    fromBuilder.Append(" WITH (NOLOCK) ");
                }

                fromBuilder.Append(" ON ");
                fromBuilder.AppendFormat("[{0}].[{1}]", leftTable.Alias, leftColumn.Name);
                fromBuilder.AppendFormat(" = ");
                fromBuilder.AppendFormat("[{0}].[{1}]" + Environment.NewLine, rightTable.Alias, rightColumn.Name);
                foreach (var join in joins.Values.Skip(1))
                {
                    leftColumn = join.Left;
                    leftTable = leftColumn.Table;
                    fromBuilder.Append(GenJoinType(join.JoinType));
                    rightColumn = join.Right;
                    rightTable = rightColumn.Table;
                    fromBuilder.Append(GetTableName(rightTable));
                    fromBuilder.AppendFormat(" [{0}]", rightTable.Alias);
                    if (_context.NoLockTables.Contains(rightTable.Name))
                    {
                        fromBuilder.Append(" WITH (NOLOCK) ");
                    }

                    fromBuilder.Append(" ON ");
                    fromBuilder.AppendFormat("[{0}].[{1}]", leftTable.Alias, leftColumn.Name);
                    fromBuilder.AppendFormat(" = ");
                    fromBuilder.AppendFormat("[{0}].[{1}]" + Environment.NewLine, rightTable.Alias, rightColumn.Name);
                }
                selectBuilder.Append(FormatSelectString(columns));

                if (conditions.Any())
                {
                    whereBuilder.Append(BuildWhere(conditions));
                }

                if (_context.Skip == -1 || _context.Take == -1)
                {
                    sortBuilder.Append(FormatSortColumns());
                }

                sqlBuilder.Clear();
                sqlBuilder.AppendFormat("{0} {1} {2} {3}", selectBuilder.ToString(), fromBuilder.ToString(), whereBuilder.ToString(), sortBuilder.ToString());
            }
            else
            {
                fromBuilder = new StringBuilder("FROM ");
                var table = columns.FirstOrDefault().Table;
                fromBuilder.Append(GetTableName(table));
                tableName = ParserUtils.GenerateAlias(table.Name);
                fromBuilder.AppendFormat(" [{0}]", tableName);
                if (_context.NoLockTables.Contains(table.Name))
                {
                    fromBuilder.Append(" WITH (NOLOCK) ");
                }

                selectBuilder.Append(FormatSelectString(columns));

                if (conditions.Any())
                {
                    whereBuilder.Append(BuildWhere(conditions));
                }

                if (_context.Take == -1 || _context.Skip == -1)
                {
                    sortBuilder.Append(FormatSortColumns());
                }

                sqlBuilder.Clear();
                sqlBuilder.AppendFormat("{0} {1} {2} {3}", selectBuilder.ToString(), fromBuilder.ToString(), whereBuilder.ToString(), sortBuilder.ToString());
            }

            var sql = sqlBuilder.ToString();
            if (_context.Pager)
            {
                sqlBuilder.Clear();
                var fields = new List<string>();
                foreach (var item in _context.Columns)
                {
                    fields.Add(string.Format("[_indexTable].[{0}]", item.Alias ?? item.MemberInfo.Name));
                }
                sqlBuilder.AppendFormat("SELECT {0} FROM ({1}) _indexTable where [_indexTable].[#index] BETWEEN {2} AND {3}", string.Join(",", fields), sql, _context.Skip, _context.Take);
                sql = sqlBuilder.ToString();
            }
            _result.CommandText = sql;
        }

        public override string GetTableName(Table table)
        {
            var tableName = string.Empty;
            if (!string.IsNullOrWhiteSpace(table.DataBase))
            {
                tableName = string.Format("[{0}].DBO.", table.DataBase);
            }
            tableName = string.Format("{0}[{1}]", tableName, table.Name);
            return tableName;
        }


        public override string GetTableName(SchemaModel.Table table)
        {
            var tableName = string.Empty;
            if (!string.IsNullOrWhiteSpace(table.DataBase))
            {
                tableName = string.Format("[{0}].DBO.", table.DataBase);
            }
            tableName = string.Format("{0}[{1}]", tableName, table.Name);
            return tableName;
        }

        string FormatConverter(bool isColumnCaller, string rawConverter, string converter, string param)
        {
            if (isColumnCaller)
            {
                converter = string.Format(rawConverter, string.Format(converter, param, "{0}"));
            }
            else
            {
                converter = string.Format(rawConverter, string.Format(converter, "{0}", param));
            }
            return converter;
        }

        public override string ParserConverter(Column column)
        {
            var converter = string.Empty;
            if (column.Converters.Any())
            {
                converter = "{0}";
            }
            while (column.Converters.Count > 0)
            {
                var columnConverter = column.Converters.Pop();
                var memberInfo = columnConverter.MemberInfo;
                var args = columnConverter.Parameters;
                var paramName = "@" + ParserUtils.GenerateAlias("param");
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Property:
                        if (TypeHelper.IsNullableType(memberInfo.DeclaringType) && memberInfo.Name == "Value")
                        {
                            continue;
                        }
                        if (memberInfo.DeclaringType == ReflectorConsts.DateTimeNullableType || memberInfo.DeclaringType == ReflectorConsts.DateTimeType)
                        {
                            switch (memberInfo.Name)
                            {
                                case "Date":
                                    converter = string.Format(converter, "CONVERT(DATE,{0},211)");
                                    break;
                                case "Value":

                                    break;
                                default:
                                    throw new Exception("不支持");
                            }
                            continue;
                        }
                        else if (memberInfo.DeclaringType == ReflectorConsts.TimeSpanType)
                        {
                            var unit = string.Empty;
                            switch (memberInfo.Name)
                            {
                                case "TotalDays":
                                    unit = "DAY";
                                    break;
                                case "TotalHours":
                                    unit = "HOUR";
                                    break;
                                case "TotalMilliseconds":
                                    unit = "MILLISECOND";
                                    break;
                                case "TotalMinutes":
                                    unit = "MINUTE";
                                    break;
                                case "TotalSeconds":
                                    unit = "SECOND";
                                    break;
                                default:
                                    throw new Exception("不支持");
                            }
                            converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "DATEDIFF(" + unit + ",{1},{0})", paramName);
                            _result.Parameters.Add(paramName, args[0]);
                            continue;
                        }
                        throw new Exception("不支持");
                    case MemberTypes.Method:
                        if (memberInfo.DeclaringType == ReflectorConsts.StringType)
                        {
                            switch (memberInfo.Name)
                            {
                                case "Contains":
                                    converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "CHARINDEX({0},{1})>0", paramName);
                                    _result.Parameters.Add(paramName, args[0]);
                                    break;
                                case "StartsWith":
                                    converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "CHARINDEX({0},{1})=1", paramName);
                                    _result.Parameters.Add(paramName, args[0]);
                                    break;
                                case "Substring":
                                    if (columnConverter.Parameters.Count == 1)
                                    {
                                        if (columnConverter.IsInstanceColumn)
                                        {
                                            converter = string.Format(converter, "SUBSTRING({0}," + (Convert.ToInt32(columnConverter.Parameters[0]) + 1) + ",LEN({0})+1-" + columnConverter.Parameters[0] + ")");
                                        }
                                        else
                                        {
                                            throw new Exception("不支持");
                                        }
                                    }
                                    else if (columnConverter.Parameters.Count == 2)
                                    {
                                        if (columnConverter.IsInstanceColumn)
                                        {
                                            converter = string.Format(converter, "SUBSTRING({0}," + (Convert.ToInt32(columnConverter.Parameters[0]) + 1) + "," + columnConverter.Parameters[1] + ")");
                                        }
                                        else
                                        {
                                            throw new Exception("不支持");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("不支持");
                                    }
                                    break;
                                default:
                                    throw new Exception("不支持");
                            }
                            continue;
                        }
                        else if (memberInfo.DeclaringType == ReflectorConsts.DateTimeType || memberInfo.DeclaringType == ReflectorConsts.DateTimeNullableType)
                        {
                            switch (memberInfo.Name)
                            {
                                case "AddDays":
                                    converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "DATEADD(DAY,{0},{1})", paramName);
                                    _result.Parameters.Add(paramName, args[0]);
                                    break;
                                case "AddHours":
                                    converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "DATEADD(HOUR,{0},{1})", paramName);
                                    _result.Parameters.Add(paramName, args[0]);
                                    break;
                                case "AddYears":
                                    converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "DATEADD(YEAR,{0},{1})", paramName);
                                    _result.Parameters.Add(paramName, args[0]);
                                    break;
                                case "AddMonths":
                                    converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "DATEADD(MONTH,{0},{1})", paramName);
                                    _result.Parameters.Add(paramName, args[0]);
                                    break;
                                case "AddSeconds":
                                    converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "DATEADD(SECOND,{0},{1})", paramName);
                                    _result.Parameters.Add(paramName, args[0]);
                                    break;
                                case "AddMilliseconds":
                                    converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "DATEADD(MILLISECOND,{0},{1})", paramName);
                                    _result.Parameters.Add(paramName, args[0]);
                                    break;
                                case "AddMinutes":
                                    converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "DATEADD(MINUTE,{0},{1})", paramName);
                                    _result.Parameters.Add(paramName, args[0]);
                                    break;
                                default:
                                    throw new Exception("不支持");
                            }
                            continue;
                        }
                        else if (memberInfo.DeclaringType == ReflectorConsts.EnumerableType)
                        {
                            switch (memberInfo.Name)
                            {
                                case "Contains":
                                    if (columnConverter.IsInstanceColumn)
                                    {
                                        throw new Exception("不支持");
                                    }
                                    converter = string.Format(converter, "{0} in (" + string.Join(",", (IEnumerable<int>)args[0]) + ")");
                                    break;
                                default:
                                    throw new Exception("不支持");
                            }
                            continue;
                        }
                        else
                        {
                            throw new Exception("不支持");
                        }
                    default:
                        throw new Exception();
                }
            }
            return converter;
        }
    }
}
