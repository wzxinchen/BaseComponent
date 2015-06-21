using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Parser
{
    public class NoLockExpressionVisitor:ExpressionVisitorBase
    {
        public NoLockExpressionVisitor(TranslateContext context):base(context)
        {

        }
        protected override System.Linq.Expressions.Expression VisitConstant(System.Linq.Expressions.ConstantExpression node)
        {
            var table = TableInfoManager.GetTable(node.Type.GetGenericArguments()[0]);
            ExtraObject = table.Name;
            return base.VisitConstant(node);
        }
    }
}
