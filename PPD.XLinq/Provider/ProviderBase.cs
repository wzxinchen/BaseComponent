using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider
{
    public abstract class ProviderBase
    {
        public abstract ParserBase CreateParser();

        internal SqlExecutor CreateSqlExecutor()
        {
            return new SqlExecutor();
        }
    }
}
