using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    public class ExpressionVisitorBase : ExpressionVisitor
    {
        public MemberExpressionType Type { get; protected set; }
        public object Result { get; protected set; }
    }
}
