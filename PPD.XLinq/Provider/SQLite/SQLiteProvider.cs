using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xinchen.Utils;

namespace PPD.XLinq.Provider.SQLite
{
    public class SQLiteProvider : ProviderBase
    {
        public override IEntityOperator CreateEntityOperator()
        {
            return new EntityOperator(this);
        }

        internal override SqlExecutorBase CreateSqlExecutor()
        {
            return new SqlExecutor();
        }
    }
}
