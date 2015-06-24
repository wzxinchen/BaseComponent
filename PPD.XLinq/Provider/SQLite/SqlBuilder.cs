using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DbUtils.DynamicExpression;
using Xinchen.Utils;
namespace PPD.XLinq.Provider.SQLite
{
    public class SqlBuilder : SqlBuilderBase
    {
        private ParseResult _result;
        private BuilderContext _context;
        private string tableName;
        string GenJoinType(JoinType joinType)
        {
            if (joinType == JoinType.Inner)
            {
                return (" INNER JOIN ");
            }
            else if (joinType == JoinType.Left)
            {
                return (" Left JOIN ");
            }
            else
            {
                throw new NotSupportedException("未支持的Join类型：" + joinType);
            }
        }



        string GetTableAlias(string columnName)
        {
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                return tableName;
            }

            foreach (var join in _context.Joins.Values)
            {
                if (join.Left.Name == columnName)
                {
                    return join.Left.Table.Alias;
                }
                else if (join.Right.Name == columnName)
                {
                    return join.Right.Table.Alias;
                }
                var tableInfo = TableInfoManager.GetTable(join.Left.Table.Type);
                foreach (var column in tableInfo.Columns.Values)
                {
                    if (column.Name == columnName)
                    {
                        return join.Left.Table.Name == tableInfo.Name ? join.Left.Table.Alias : join.Right.Table.Alias;
                    }
                }
                tableInfo = TableInfoManager.GetTable(join.Right.Table.Type);
                foreach (var column in tableInfo.Columns.Values)
                {
                    if (column.Name == columnName)
                    {
                        return join.Left.Table.Name == tableInfo.Name ? join.Left.Table.Alias : join.Right.Table.Alias;
                    }
                }
            }
            throw new Exception();
        }

        string SelectOperation(CompareType compareType)
        {
            switch (compareType)
            {
                case CompareType.And:
                    return "AND";
                case CompareType.Equal:
                    return "=";
                case CompareType.GreaterThan:
                    return ">";
                case CompareType.GreaterThanOrEqual:
                    return ">=";
                case CompareType.LessThan:
                    return "<";
                case CompareType.LessThanOrEqual:
                    return "<=";
                case CompareType.Or:
                    return "OR";
                case CompareType.Add:
                    return "+";
                case CompareType.Substarct:
                    return "-";
                case CompareType.Multiply:
                    return "*";
                case CompareType.Divide:
                    return "/";
                case CompareType.NotEqual:
                    return "<>";
                default:
                    throw new Exception();
            }
        }


        string BuildSql(Token token)
        {
            switch (token.Type)
            {
                case TokenType.Condition:
                    return BuildCondition(token.Condition);
                case TokenType.Column:
                    return BuildSql(token.Column);
                case TokenType.Object:
                    if (token.Object == null)
                    {
                        return null;
                    }
                    if (token.IsBool())
                    {
                        if (!token.GetBool())
                        {
                            return "1=2";
                        }
                        return null;
                    }
                    var paramName = ParserUtils.GenerateAlias("param");
                    _result.Parameters.Add(paramName, token.Object);
                    return "@" + paramName;
                default:
                    throw new Exception();
            }
            throw new Exception();
        }

        string BuildCondition(Condition condition)
        {
            var left = condition.Left;
            var result = string.Empty;
            switch (condition.CompareType)
            {
                case CompareType.Not:
                    return string.Format("NOT ({0})", BuildSql(left));
                default:
                    string leftStr = BuildSql(left);
                    string rightStr = BuildSql(condition.Right);
                    if (leftStr == null)
                    {
                        return string.Format("({0} IS NULL)", rightStr);
                    }
                    else if (rightStr == null)
                    {
                        return string.Format("({0} IS NULL)", leftStr);
                    }
                    return string.Format("({0} {1} {2})", leftStr, SelectOperation(condition.CompareType), rightStr);

            }
        }

        private string BuildSql(Column column)
        {
            var col = string.Empty;
            //if (string.IsNullOrWhiteSpace(column.Table.DataBase))
            //{
            col = string.Format("[{0}].[{1}]", GetTableAlias(column.Name), column.Name);
            //}
            //else
            //{
            //    col = string.Format("[{0}].dbo.[{1}].[{2}]", column.Table.DataBase, GetTableAlias(column.Name), column.Name);
            //}
            var convert = string.Empty;
            if (column.Converters.Any())
            {
                convert = ParserConverter(column);
                col = string.Format(convert, col);
            }
            return col;
        }

        string FormatColumn(Column column, bool genColumnAlias = true)
        {
            var tblAlias = GetTableAlias(column.Name);
            string col = string.Format("[{0}].[{1}]", tblAlias, column.Name);
            var converter = ParserConverter(column);
            if (!string.IsNullOrWhiteSpace(converter))
            {
                col = string.Format(converter, string.Format("[{0}].[{1}]", tblAlias, column.Name));
            }
            if (!genColumnAlias)
            {
                return col;
            }
            return string.Format("{0} [{1}]", col, column.Alias ?? column.MemberInfo.Name);
        }

        string FormatSelectString(List<Column> columns)
        {
            if (_context.AggregationColumns.Count >= 1)
            {
                _context.SortColumns.Clear();
                var aggrColumn = _context.AggregationColumns.FirstOrDefault();
                var columnString = string.Empty;
                if (aggrColumn.Value != null)
                {
                    columnString = FormatColumn(aggrColumn.Value, false);
                }
                switch (aggrColumn.Key)
                {
                    case "Count":
                        if (aggrColumn.Value == null)
                        {
                            return ("Count(1)");
                        }
                        return string.Format("Count({0})", columnString);
                    case "Sum":
                        return string.Format("Sum({0})", columnString);
                    case "Average":
                        return string.Format("AVG({0})", columnString);
                    default:
                        throw new Exception(aggrColumn.Key);
                }
            }
            else
            {
                List<string> fields = new List<string>();
                foreach (var column in columns)
                {
                    string col = FormatColumn(column);
                    fields.Add(col);
                }
                if (_context.Pager)
                {
                    fields.Add(string.Format("ROW_NUMBER() OVER({0}) #index", FormatSortColumns()));
                }
                return (string.Join(",", fields));
            }
        }

        string FormatSortColumns()
        {
            var sortBuilder = new StringBuilder();
            if (_context.SortColumns.Any())
            {
                sortBuilder.Append("ORDER BY ");
                var sorts = new List<string>();
                foreach (var sortColumn in _context.SortColumns)
                {
                    var col = string.Format("[{0}].[{1}]", GetTableAlias(sortColumn.Value.Name), sortColumn.Value.Alias ?? sortColumn.Value.Name);
                    var converter = ParserConverter(sortColumn.Value);
                    if (!string.IsNullOrWhiteSpace(converter))
                    {
                        col = string.Format(converter, col);
                    }
                    sorts.Add(col);
                }
                sortBuilder.Append(string.Join(",", sorts));
            }
            return sortBuilder.ToString();
        }

        string BuildWhere(IList<Token> conditions)
        {
            var whereBuilder = new StringBuilder();
            if (conditions.Any())
            {
                var filters = new List<string>();
                foreach (var condition in conditions)
                {
                    var filter = BuildSql(condition);
                    if (string.IsNullOrWhiteSpace(filter))
                    {
                        continue;
                    }
                    filters.Add(filter);
                }
                if (filters.Any())
                {
                    whereBuilder.Append("WHERE ");
                    whereBuilder.Append(string.Join(" AND ", filters));
                }
            }
            return whereBuilder.ToString();
        }

        void BuildSelectSql()
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

        void BuildDeleteSql()
        {
            var where = string.Empty;
            var table = TableInfoManager.GetTable(_context.ElementType);
            tableName = table.Name;
            var tableFullName = GetTableName(table);
            if (_context.Conditions.Any())
            {
                where = BuildWhere(_context.Conditions);
            }
            var sql = "DELETE FROM {0} {1}";
            sql = string.Format(sql, tableFullName, where);
            _result.CommandText = sql;
        }

        void BuildUpdateSql()
        {
            var where = string.Empty;
            var table = TableInfoManager.GetTable(_context.ElementType);
            var keyColumnName = string.Empty;
            var keyColumn = table.Columns.FirstOrDefault(x => x.Value.IsKey).Value;
            if (keyColumn != null)
            {
                keyColumnName = keyColumn.PropertyInfo.Name;
            }
            tableName = table.Name;
            var tableFullName = GetTableName(table);
            if (_context.Conditions.Any())
            {
                where = BuildWhere(_context.Conditions);
            }
            var setts = new List<string>();
            var alias = string.Empty;
            foreach (var key in _context.UpdateResult.Keys)
            {
                if (key == keyColumnName)
                {
                    continue;
                }
                alias = ParserUtils.GenerateAlias(key);
                var set = string.Format("[{0}] = @{1}", key, alias);
                _result.Parameters.Add(alias, _context.UpdateResult.Get(key));
                setts.Add(set);
            }
            var sql = "UPDATE {0} SET {1} {2}";
            sql = string.Format(sql, tableFullName, string.Join(",", setts), where);
            _result.CommandText = sql;
        }

        public override ParseResult BuildSql(BuilderContext context)
        {
            _context = context;
            _result = new ParseResult();
            switch (context.SqlType)
            {
                case SqlType.Delete:
                    BuildDeleteSql();
                    break;
                case SqlType.Update:
                    BuildUpdateSql();
                    break;
                case SqlType.Select:
                    BuildSelectSql();
                    break;
                default:
                    throw new Exception();
            }
            return _result;
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
                switch (memberInfo.MemberType)
                {
                    case MemberTypes.Property:
                        if (ExpressionConsts.NullableType.IsAssignableFrom(memberInfo.DeclaringType) && memberInfo.Name == "Value")
                        {
                            continue;
                        }
                        if (memberInfo.DeclaringType == ExpressionConsts.DateTimeNullableType || memberInfo.DeclaringType == ExpressionConsts.DateTimeType)
                        {
                            converter = string.Format(converter, "CONVERT(DATE,{0},211)");
                            continue;
                        }
                        throw new Exception("不支持");
                    case MemberTypes.Method:
                        var paramName = "@" + ParserUtils.GenerateAlias("param");
                        if (memberInfo.DeclaringType == ExpressionConsts.StringType)
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
                        else if (memberInfo.DeclaringType == ExpressionConsts.DateTimeType || memberInfo.DeclaringType == ExpressionConsts.DateTimeNullableType)
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
                        else if (memberInfo.DeclaringType == ExpressionConsts.EnumerableType)
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
