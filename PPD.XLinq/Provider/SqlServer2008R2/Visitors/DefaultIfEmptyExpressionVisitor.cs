using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    public class DefaultIfEmptyExpressionVisitor : ExpressionVisitor
    {
        public Type Type { get; private set; }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            Type = node.Type;
            return base.VisitParameter(node);
        }
    }
}
