using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    public class GroupJoinExpressionVisitor:ExpressionVisitorBase
    {
        protected override System.Linq.Expressions.Expression VisitMethodCall(System.Linq.Expressions.MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }
    }
}
