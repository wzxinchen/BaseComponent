using PPD.XLinq.Provider.SqlServer2008R2.Visitors;
using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider
{
    public class SqlServer2008R2Provider : ProviderBase
    {
        public override ParserBase CreateParser()
        {
            return new Parser();
        }
    }
}
