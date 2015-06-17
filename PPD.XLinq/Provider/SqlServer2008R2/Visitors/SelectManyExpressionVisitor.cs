using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    public class SelectManyExpressionVisitor:ExpressionVisitor
    {
        public List<Column> Columns { get; private set; }
        IReadOnlyCollection<Expression> _columnParams;
        private Dictionary<string, TranslateModel.Join> Joins;
        private Type _lastJoinType;
        private Join _lastJoin;

        public SelectManyExpressionVisitor(Dictionary<string, TranslateModel.Join> Joins)
        {
            // TODO: Complete member initialization
            this.Joins = Joins;
            Columns = new List<Column>();
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var selectExp = node.Arguments[2];
            var defaultIfEmptyCallExp = node.Arguments[1];
            //var visitor=new DefaultIfEmptyExpressionVisitor();
            //visitor.Visit(defaultIfEmptyCallExp);
            //_lastJoinType = visitor.Type;
            //_lastJoin = Joins[_lastJoinType];
            Visit(selectExp);
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var arg = node.Arguments[i];
            }
            return base.VisitNew(node);
        }
    }
}
