using PPD.XLinq.Provider;
using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.Utils;
namespace PPD.XLinq
{
    public abstract class SqlBuilderBase
    {
        protected ParseResult _result;
        protected BuilderContext _context;
        protected string tableName;
        protected abstract void BuildSelectSql();
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

        public ParseResult BuildSql(BuilderContext context)
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
        protected string FormatColumn(Column column, bool genColumnAlias = true)
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

        protected string FormatSelectString(List<Column> columns)
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
                return (string.Join(",", fields));
            }
        }

        protected string FormatSortColumns()
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

        protected string BuildWhere(IList<Token> conditions)
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
        protected string BuildCondition(Condition condition)
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

        protected string BuildSql(Column column)
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
        protected string BuildSql(Token token)
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
        protected string SelectOperation(CompareType compareType)
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

        protected string GetTableAlias(string columnName)
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

        public SqlBuilderBase()
        {
        }
        protected string GenJoinType(JoinType joinType)
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

        public abstract string GetTableName(SchemaModel.Table table);
        public abstract string GetTableName(Table leftTable);
        public abstract string ParserConverter(Column column);
    }
}
