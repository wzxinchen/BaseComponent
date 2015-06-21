using PPD.XLinq.Provider.SqlServer2008R2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xinchen.Utils;

namespace PPD.XLinq.Provider
{
    public abstract class ProviderBase
    {
        public abstract ParserBase CreateParser();

        internal SqlExecutor CreateSqlExecutor()
        {
            return new SqlExecutor();
        }

        internal abstract EntityAddBase CreateEntityAdder();
    }
}
