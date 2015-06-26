using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.Utils;
namespace PPD.XLinq.Provider.SQLite
{
    public class SqlBuilder : SqlBuilderBase
    {
        protected override void BuildSelectSql()
        {
            if (_context.NoLockTables.Any())
            {
                throw new Exception("暂不支持NOLock");
            }
            var columns = _context.Columns;
            var conditions = _context.Conditions;
            var joins = _context.Joins;
            var fromBuilder = new StringBuilder("FROM ");
            var selectBuilder = new StringBuilder("SELECT ");
            if (_context.Distinct)
            {
                selectBuilder.Append(" DISTINCT ");
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

            if (_context.Pager)
            {
                sqlBuilder.AppendFormat(" LIMIT {0},{1}", _context.Skip, _context.Take);
            }
            else if (_context.Take > 0)
            {
                selectBuilder.Append(" LIMIT " + _context.Take + " ");
            }
            var sql = sqlBuilder.ToString();
            _result.CommandText = sql;
        }

        public override string GetTableName(Table table)
        {
            var tableName = string.Empty;
            tableName = string.Format("{0}[{1}]", tableName, table.Name);
            return tableName;
        }


        public override string GetTableName(SchemaModel.Table table)
        {
            var tableName = string.Empty;
            tableName = string.Format("{0}[{1}]", tableName, table.Name);
            return tableName;
        }

        string FormatConverter(bool isColumnCaller, string rawConverter, string converter, string num)
        {
            if (isColumnCaller)
            {
                converter = string.Format(rawConverter, string.Format(converter, "{0}", num));
            }
            else
            {
                converter = string.Format(rawConverter, string.Format(converter, num, "{0}"));
            }
            return converter;
        }

        string FormatDateConverter(ColumnConverter columnConverter, string converter, string unit, string value)
        {
            if (columnConverter.IsInstanceColumn)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    converter = string.Format(converter, string.Format("DATE({0},{1} ' {2}')", "{0}", value, unit));
                }
                else
                {
                    converter = string.Format(converter, string.Format("DATE({0},{1} || ' {2}')", "{0}", value, unit));
                }
            }
            else
            {
                converter = string.Format(converter, string.Format("DATE('{0}',{1} || ' {2}')", value, "{0}", unit));
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
                            converter = string.Format(converter, "DATE({0})");
                            continue;
                        }
                        else if (memberInfo.DeclaringType == ReflectorConsts.TimeSpanType)
                        {
                            var unit = 1;
                            switch (memberInfo.Name)
                            {
                                case "TotalDays":
                                    unit = 1;
                                    break;
                                case "TotalHours":
                                    unit = 12;
                                    break;
                                case "TotalMilliseconds":
                                    unit = 12 * 60 * 60 * 1000;
                                    break;
                                case "TotalMinutes":
                                    unit = 12 * 60;
                                    break;
                                case "TotalSeconds":
                                    unit = 12 * 60 * 60;
                                    break;
                                default:
                                    throw new Exception("不支持");
                            }
                            converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "((JULIANDAY({0}) - JULIANDAY({1}))*" + unit + ")", paramName);
                            //converter = FormatConverter(columnConverter.IsInstanceColumn, converter, "DATEDIFF(" + unit + ",{1},{0})", paramName);
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
                                            converter = string.Format(converter, "SUBSTR({0}," + (Convert.ToInt32(columnConverter.Parameters[0]) + 1) + ")");
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
                                            converter = string.Format(converter, "SUBSTR({0}," + (Convert.ToInt32(columnConverter.Parameters[0]) + 1) + "," + columnConverter.Parameters[1] + ")");
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
                            var num = args[0].ToString();
                            switch (memberInfo.Name)
                            {
                                case "AddDays":
                                    converter = FormatDateConverter(columnConverter, converter, "DAYS", num);
                                    break;
                                case "AddHours":
                                    converter = FormatDateConverter(columnConverter, converter, "HOURS", num);
                                    break;
                                case "AddYears":
                                    converter = FormatDateConverter(columnConverter, converter, "YEARS", num);
                                    break;
                                case "AddMonths":
                                    converter = FormatDateConverter(columnConverter, converter, "MONTHS", num);
                                    break;
                                case "AddSeconds":
                                    converter = FormatDateConverter(columnConverter, converter, "SECONDS", num);
                                    break;
                                case "AddMilliseconds":
                                    converter = FormatDateConverter(columnConverter, converter, string.Format("0.{0} SECONDS", num), string.Empty);
                                    break;
                                case "AddMinutes":
                                    converter = FormatDateConverter(columnConverter, converter, "MINUTES", num);
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
