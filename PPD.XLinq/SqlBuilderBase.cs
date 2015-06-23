using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public abstract class SqlBuilderBase
    {
        public abstract ParseResult BuildSql(BuilderContext context);
    }
}
