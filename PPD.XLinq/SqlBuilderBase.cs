using PPD.XLinq.Provider;
using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public abstract class SqlBuilderBase
    {
        public SqlBuilderBase()
        {
        }
        public abstract ParseResult BuildSql(BuilderContext context);

        public abstract string GetTableName(SchemaModel.Table table);
        public abstract string GetTableName(Table leftTable);
        public abstract string ParserConverter(Column column);
    }
}
