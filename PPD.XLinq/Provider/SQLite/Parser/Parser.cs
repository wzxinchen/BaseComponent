using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xinchen.Utils;
namespace PPD.XLinq.Provider.SQLite.Parser
{
    public class Parser : ParserBase
    {
        SqlExpressionParser parser = new SqlExpressionParser();
        Expression _expression;
        public override void Parse(System.Linq.Expressions.Expression expression)
        {
            _expression = expression;
            parser.ElementType = ElementType;
            parser.Parse(expression);
            var provider = ProviderFactory.CreateProvider(ConfigManager.DataBaseType);
            var builderFactory = provider.CreateSqlBuilderFactory();
            var builder = builderFactory.CreateSqlBuilder();
            BuilderContext context = new BuilderContext();
            var sqlType = SqlType.Select;
            if (parser.IsCallAny)
            {
                context.Take = 1;
            }
            if (parser.IsDelete)
            {
                sqlType = SqlType.Delete;
            }
            else if (parser.IsUpdate)
            {
                sqlType = SqlType.Update;
            }
            context.SqlType = sqlType;
            context.Pager = parser.Skip != -1 && parser.Take != -1;
            context.SortColumns = parser.SortColumns;
            context.Joins = parser.Joins;
            context.UpdateResult = parser.UpdateResult;
            context.Skip = parser.Skip;
            context.Take = parser.Take;
            context.AggregationColumns = parser.AggregationColumns;
            context.Columns = parser.Columns;
            context.NoLockTables = parser.NoLockTables;
            context.Conditions = parser.Conditions;
            context.ElementType = parser.ElementType;
            Result = builder.BuildSql(context);
        }

    }
}
