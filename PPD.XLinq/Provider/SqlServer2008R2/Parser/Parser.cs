using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Parser
{
    public class Parser : ParserBase
    {
        SqlExpressionParser parser = new SqlExpressionParser();
        Expression _expression;
        private string tableName;
        public override void Parse(System.Linq.Expressions.Expression expression)
        {
            _expression = expression;
            parser.ElementType = ElementType;
            parser.Parse(expression);
            BuildSql();
        }


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

            foreach (var join in parser.Joins.Values)
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
                    Result.Parameters.Add(paramName, token.Object);
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
            if (!string.IsNullOrWhiteSpace(column.Converter))
            {
                col = string.Format(column.Converter, col);
                var matches = Regex.Matches(col, @"@param\d+");
                for (int i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    Result.Parameters.Add(match.Value, column.ConverterParameters[i]);
                }
            }
            return col;
        }

        //string BuildCondition(object obj)
        //{
        //    if (obj == null)
        //    {
        //        return string.Empty;
        //    }
        //    if (obj is Token)
        //    {
        //        var result = BuildSql(obj as Token);
        //        return result;
        //    }
        //    else if (obj is Condition)
        //    {
        //        return BuildCondition(obj as Condition);
        //    }
        //    else if (obj is Column)
        //    {
        //        var result = BuildSql(obj as Column);
        //        return result;
        //    }
        //    else if (obj is bool)
        //    {
        //        var result = Convert.ToBoolean(obj);
        //        if (!result)
        //        {
        //            return "1=2";
        //        }
        //        return string.Empty;
        //    }
        //    throw new Exception();
        //}

        string FormatColumn(Column column, bool genColumnAlias = true)
        {
            var tblAlias = GetTableAlias(column.Name);
            string col = string.Format("[{0}].[{1}]", tblAlias, column.Name);
            if (!string.IsNullOrWhiteSpace(column.Converter))
            {
                col = string.Format(column.Converter, string.Format("[{0}].[{1}]", tblAlias, column.Name));
            }
            if (!genColumnAlias)
            {
                return col;
            }
            return string.Format("{0} [{1}]", col, column.Alias ?? column.MemberInfo.Name);
        }

        string FormatSelectString(List<Column> columns)
        {
            if (parser.AggregationColumns.Count >= 1)
            {
                parser.SortColumns.Clear();
                var aggrColumn = parser.AggregationColumns.FirstOrDefault();
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
                if (parser.Take != -1 && parser.Skip != -1)
                {
                    fields.Add(string.Format("ROW_NUMBER() OVER({0}) #index", FormatSortColumns()));
                }
                return (string.Join(",", fields));
            }
        }

        string FormatSortColumns()
        {
            var sortBuilder = new StringBuilder();
            if (parser.SortColumns.Any())
            {
                sortBuilder.Append("ORDER BY ");
                var sorts = new List<string>();
                foreach (var sortColumn in parser.SortColumns)
                {
                    var col = string.Format("[{0}].[{1}]", GetTableAlias(sortColumn.Value.Name), sortColumn.Value.Alias ?? sortColumn.Value.Name);
                    if (!string.IsNullOrWhiteSpace(sortColumn.Value.Converter))
                    {
                        col = string.Format(sortColumn.Value.Converter, col);
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
            var columns = parser.Columns;
            var conditions = parser.Conditions;
            var joins = parser.Joins;
            var fromBuilder = new StringBuilder("FROM ");
            var selectBuilder = new StringBuilder("SELECT ");
            if (parser.Distinct)
            {
                selectBuilder.Append(" DISTINCT ");
            }

            if (parser.IsCallAny)
            {
                selectBuilder.Append(" TOP 1 ");
            }
            else if (parser.Take != -1 && parser.Skip == -1)
            {
                selectBuilder.Append(" TOP " + parser.Take + " ");
            }
            var whereBuilder = new StringBuilder();
            var sortBuilder = new StringBuilder();
            var sqlBuilder = new StringBuilder();
            if (joins != null && joins.Count > 0)
            {
                var firstJoin = joins.Values.FirstOrDefault();
                var leftColumn = firstJoin.Left;
                var leftTable = leftColumn.Table;
                fromBuilder.Append(TableInfoManager.GetTableName(leftTable));
                //if (!string.IsNullOrWhiteSpace(leftTable.DataBase))
                //{
                //    fromBuilder.AppendFormat("[{0}].dbo.", leftTable.DataBase);
                //}
                //fromBuilder.AppendFormat("[{0}] ", leftTable.Name);
                fromBuilder.AppendFormat(" [{0}]", leftTable.Alias);
                if (parser.NoLockTables.Contains(leftTable.Name))
                {
                    fromBuilder.Append(" WITH (NOLOCK) ");
                }
                fromBuilder.Append(GenJoinType(firstJoin.JoinType));
                var rightColumn = firstJoin.Right;
                var rightTable = rightColumn.Table;
                fromBuilder.Append(TableInfoManager.GetTableName(rightTable));
                //if (!string.IsNullOrWhiteSpace(rightTable.DataBase))
                //{
                //    fromBuilder.AppendFormat("[{0}].dbo.", rightTable.DataBase);
                //}
                //fromBuilder.AppendFormat("[{0}] ", rightTable.Name);
                fromBuilder.AppendFormat(" [{0}]", rightTable.Alias);
                if (parser.NoLockTables.Contains(rightTable.Name))
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
                    fromBuilder.Append(TableInfoManager.GetTableName(rightTable));
                    //if (!string.IsNullOrWhiteSpace(rightTable.DataBase))
                    //{
                    //    fromBuilder.AppendFormat("[{0}].dbo.", rightTable.DataBase);
                    //}
                    //fromBuilder.AppendFormat("[{0}] ", rightTable.Name);
                    fromBuilder.AppendFormat(" [{0}]", rightTable.Alias);
                    if (parser.NoLockTables.Contains(rightTable.Name))
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

                if (parser.Skip == -1 || parser.Take == -1)
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
                fromBuilder.Append(TableInfoManager.GetTableName(table));
                //if (!string.IsNullOrWhiteSpace(table.DataBase))
                //{
                //    fromBuilder.AppendFormat("[{0}].dbo.", table.DataBase);
                //}
                tableName = ParserUtils.GenerateAlias(table.Name);
                fromBuilder.AppendFormat(" [{0}]", tableName);
                if (parser.NoLockTables.Contains(table.Name))
                {
                    fromBuilder.Append(" WITH (NOLOCK) ");
                }

                selectBuilder.Append(FormatSelectString(columns));

                if (conditions.Any())
                {
                    whereBuilder.Append(BuildWhere(conditions));
                }

                if (parser.Skip == -1 || parser.Take == -1)
                {
                    sortBuilder.Append(FormatSortColumns());
                }

                sqlBuilder.Clear();
                sqlBuilder.AppendFormat("{0} {1} {2} {3}", selectBuilder.ToString(), fromBuilder.ToString(), whereBuilder.ToString(), sortBuilder.ToString());
            }

            var sql = sqlBuilder.ToString();
            if (parser.Take != -1 && parser.Skip != -1)
            {
                sqlBuilder.Clear();
                var fields = new List<string>();
                foreach (var item in parser.Columns)
                {
                    fields.Add(string.Format("[_indexTable].[{0}]", item.Alias ?? item.MemberInfo.Name));
                }
                sqlBuilder.AppendFormat("SELECT {0} FROM ({1}) _indexTable where [_indexTable].[#index] BETWEEN {2} AND {3}", string.Join(",", fields), sql, parser.Skip, parser.Take);
                sql = sqlBuilder.ToString();
            }
            Result.CommandText = sql;
        }

        void BuildDeleteSql()
        {
            var where = string.Empty;
            var table = TableInfoManager.GetTable(ElementType);
            tableName = table.Name;
            var tableFullName = TableInfoManager.GetTableName(table);
            if (parser.Conditions.Any())
            {
                where = BuildWhere(parser.Conditions);
            }
            var sql = "DELETE FROM {0} {1}";
            sql = string.Format(sql, tableFullName, where);
            Result.CommandText = sql;
        }

        void BuildUpdateSql()
        {

        }

        internal void BuildSql()
        {
            Result = new ParseResult();
            if (parser.IsDelete)
            {
                BuildDeleteSql();
                return;
            }
            if (parser.IsUpdate)
            {
                BuildUpdateSql();
                return;
            }
            BuildSelectSql();
        }
    }
}
