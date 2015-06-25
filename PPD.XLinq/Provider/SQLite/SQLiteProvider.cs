using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SQLite
{
    public class SQLiteProvider : ProviderBase
    {
        public SQLiteProvider()
            : base(DatabaseTypes.SQLite)
        {

        }
        internal override EntityOperatorBase CreateEntityOperator()
        {
            return new EntityOperator(this);
        }
    }
}
