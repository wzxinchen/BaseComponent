using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Parser
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
        public MemberExpressionVisitor(TranslateContext context):base(context)
        {
            this._joins = context.Joins;
        }
        public override System.Linq.Expressions.Expression Visit(System.Linq.Expressions.Expression node)
        {
            ExpressionVisitorBase visitor = null;
            if (node.NodeType == System.Linq.Expressions.ExpressionType.Call)
            {
                visitor = new MethodCallExpressionVisitor(Context);
            }
            else
            {
                visitor = new PropertyFieldExpressionVisitor(Context);
            }
            visitor.Visit(node);
            Token = visitor.Token;
            return node;
        }
    }
}
