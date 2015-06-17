using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    /// <summary>
    /// 对访问成员方法或属性的表达式进行分析
    /// </summary>
    public class MemberExpressionVisitor : ExpressionVisitorBase
    {
        Dictionary<string, Join> _joins;
        /// <summary>
        /// 参数可传null，该参数主要用于创建该表达式中可能出现的对Entity的访问
        /// </summary>
        public MemberExpressionVisitor(Dictionary<string, Join> joins)
        {
            this._joins = joins;
        }
        public override System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression node)
        {
            ExpressionVisitorBase visitor = null;
            if (node.NodeType == System.Linq.Expressions.ExpressionType.Call)
            {
                visitor = new MethodCallExpressionVisitor(_joins);
            }
            else
            {
                visitor = new PropertyFieldExpressionVisitor(_joins);
            }
            visitor.Visit(node);
            Result = visitor.Result;
            Type = visitor.Type;
            return node;
        }
    }
}
