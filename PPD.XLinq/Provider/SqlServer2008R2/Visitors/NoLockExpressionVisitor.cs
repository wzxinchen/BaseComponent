using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    public class NoLockExpressionVisitor:ExpressionVisitorBase
    {
        protected override System.Linq.Expressions.Expression VisitConstant(System.Linq.Expressions.ConstantExpression node)
        {
            var table = TableInfoManager.GetTable(node.Type.GetGenericArguments()[0]);
            Result = table.Name;
            return base.VisitConstant(node);
        }
    }
}
