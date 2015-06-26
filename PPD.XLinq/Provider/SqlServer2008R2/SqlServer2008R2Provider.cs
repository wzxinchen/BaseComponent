using PPD.XLinq.Provider.SqlServer2008R2;
using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DynamicObject;

namespace PPD.XLinq.Provider.SqlServer2008R2
{
    public class SqlServer2008R2Provider : ProviderBase
    {
        public SqlServer2008R2Provider()
        {

        }

        public override IEntityOperator CreateEntityOperator()
        {
            return new EntityOperator();
        }
    }
}
