using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    public class JoinExpressionVisitor : ExpressionVisitorBase
    {
        static string[] _joinMethodNames = new string[] { "Join", "GroupJoin" };
        public Expression SelectExpression { get; private set; }
        public Dictionary<string, Join> Joins { get; private set; }
        public JoinExpressionVisitor()
        {
            Joins = new Dictionary<string, Join>();
            Result = new List<string>();
        }
        protected override System.Linq.Expressions.Expression VisitMethodCall(System.Linq.Expressions.MethodCallExpression node)
        {
            var leftTable = node.Arguments[0];
            if (leftTable.NodeType == ExpressionType.Call)
            {
                var leftCallExp = leftTable as MethodCallExpression;
                if (leftCallExp.Method.Name == "NoLock")
                {
                    leftTable = leftCallExp.Arguments[0];
                }
            }
            JoinExpressionVisitor visitor = null;
            var joinName = "<>" + Path.GetRandomFileName();
            if (leftTable.NodeType == ExpressionType.Call)
            {
                var leftMethodCallExp = (MethodCallExpression)leftTable;
                visitor = new JoinExpressionVisitor();
                visitor.Visit(leftTable);
                if (node.Method.Name == "SelectMany")
                {
                    SelectExpression = node.Arguments[2];
                    joinName = ((ParameterExpression)((LambdaExpression)((UnaryExpression)node.Arguments[2]).Operand).Parameters[1]).Name;
                    var lastJoin = visitor.Joins.LastOrDefault();
                    // var lockTables = visitor.Result as List<string>;

                    Joins.Add(joinName, lastJoin.Value);
                    foreach (var item in visitor.Joins)
                    {
                        if (item.Key == lastJoin.Key)
                        {
                            continue;
                        }
                        Joins.Add(item.Key, item.Value);
                    }
                    return node;
                }
                foreach (var item in visitor.Joins)
                {
                    Joins.Add(item.Key, item.Value);
                }
            }
            SelectExpression = node.Arguments[4];
            //if (leftTable.NodeType == ExpressionType.Call &&
            //    (((MethodCallExpression)leftTable).Method.Name == "SelectMany" ||
            //    _joinMethodNames.Contains(((MethodCallExpression)leftTable).Method.Name)))
            //{
            //    visitor = new JoinExpressionVisitor();
            //    visitor.Visit(leftTable);
            //    foreach (var item in visitor.Joins)
            //    {
            //        Joins.Add(item.Key, item.Value);
            //    }
            //    //((List<Join>)Result).AddRange((List<Join>)visitor.Result);
            //    if (node.Method.Name == "SelectMany")
            //    {
            //        return node;
            //    }
            //}

            var rightTable = node.Arguments[1];
            //var nolockRightTable = false;
            if (rightTable.NodeType == ExpressionType.Call)
            {
                var rightCallExp = (MethodCallExpression)rightTable;
                if (rightCallExp.Method.Name != "NoLock")
                {
                    throw new Exception();
                }
                //nolockRightTable = true;
                rightTable = rightCallExp.Arguments[0];
            }
            if (rightTable.NodeType != ExpressionType.Constant)
            {
                throw new Exception();
            }
            var leftColumnExp = node.Arguments[2];
            var rightColumnExp = node.Arguments[3];
            var memVisitor = new MemberExpressionVisitor(Joins);
            memVisitor.Visit(leftColumnExp);
            if (memVisitor.Type != MemberExpressionType.Column)
            {
                throw new Exception();
            }
            var join = new Join();
            join.Left = ((Token)memVisitor.Result).Column;
            memVisitor = new MemberExpressionVisitor(Joins);
            memVisitor.Visit(rightColumnExp);
            join.Right = ((Token)memVisitor.Result).Column;
            //if (nolockRightTable)
            //{
            //    ((List<string>)Result).Add(join.Right.Table.Alias);
            //}
            switch (node.Method.Name)
            {
                case "Join":
                    join.JoinType = JoinType.Inner;
                    break;
                case "GroupJoin":
                    join.JoinType = JoinType.Left;
                    break;
                default:
                    throw new Exception();
            }
            var resultExp = node.Arguments[4];
            var resultCallExp = (LambdaExpression)((LambdaExpression)((UnaryExpression)resultExp).Operand);
            Joins.Add(joinName, join);
            //Joins.Add(resultCallExp.ReturnType, join);
            return node;
        }
    }
}
