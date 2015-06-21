using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    public class WhereExpressionVisitor : ExpressionVisitorBase
    {
        Dictionary<string, Join> _joins;
        public WhereExpressionVisitor(Dictionary<string, Join> joins)
        {
            this._joins = joins;
        }
        protected override Expression VisitBinary(BinaryExpression node)
        {
            BinaryExpressionVisitor visitor = new BinaryExpressionVisitor(_joins);
            visitor.Visit(node);
            Result = visitor.Result;
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var visitor = new MethodCallExpressionVisitor(_joins);
            visitor.Visit(node);
            Result = visitor.Result;
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    if (Result is bool)
                    {
                        Type = MemberExpressionType.Object;
                        Result = !((bool)Result);
                    }
                    else if (visitor.Type == MemberExpressionType.Column)
                    {
                        Type = MemberExpressionType.Column;
                        var token = (Token)Result;
                        switch (token.Type)
                        {
                            case TokenType.Column:
                                Result = Token.Create(new Condition()
                                {
                                    CompareType = CompareType.Not,
                                    Left = token
                                });
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    break;
                default:
                    throw new Exception();
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var visitor = new MethodCallExpressionVisitor(_joins);
            visitor.Visit(node);
            Result = visitor.Result;
            return node;
        }

        class BinaryExpressionVisitor : ExpressionVisitorBase
        {
            Dictionary<string, Join> _joins;
            public BinaryExpressionVisitor(Dictionary<string, Join> joins)
            {
                this._joins = joins;
            }
            public override Expression Visit(Expression node)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Constant:
                        Result = (((ConstantExpression)node)).Value;
                        return node;
                    case ExpressionType.OrElse:
                    case ExpressionType.AndAlso:
                        Result = ParseLogicBinary((BinaryExpression)node);
                        return node;
                    case ExpressionType.Equal:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.LessThan:
                    case ExpressionType.NotEqual:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Divide:
                        Result = ParseMathBinary((BinaryExpression)node);
                        return node;
                    case ExpressionType.Not:
                        Result = ParseNotExpression((UnaryExpression)node);
                        return node;
                    case ExpressionType.Call:
                        Result = ParseMethodCallExpression((MethodCallExpression)node);
                        return node;
                    default:
                        throw new Exception(node.NodeType.ToString());
                }
            }

            private object ParseMethodCallExpression(MethodCallExpression methodCallExpression)
            {
                var visitor = new MethodCallExpressionVisitor(_joins);
                visitor.Visit(methodCallExpression);
                return visitor.Result;
            }

            private object ParseNotExpression(UnaryExpression unaryExpression)
            {
                var operand = unaryExpression.Operand;
                if (operand is MemberExpression)
                {
                    var visitor = new MemberExpressionVisitor(_joins);
                    visitor.Visit(operand);
                    if (visitor.Type == MemberExpressionType.Object)
                    {
                        return !((bool)visitor.Result);
                    }
                    else if (visitor.Type == MemberExpressionType.Column)
                    {
                        throw new Exception("不支持");
                    }
                }
                else if (operand is MethodCallExpression)
                {
                    var visitor = new MethodCallExpressionVisitor(_joins);
                    visitor.Visit(operand);
                }
                else
                {
                    throw new Exception("不支持");
                }
                throw new Exception();
            }

            object ParseMathBinary(BinaryExpression node)
            {
                ExpressionVisitorBase leftVisitor, rightVisitor;
                if (node.Left is BinaryExpression)
                {
                    leftVisitor = new BinaryExpressionVisitor(_joins);
                }
                else
                {
                    leftVisitor = new MemberExpressionVisitor(_joins);
                }
                leftVisitor.Visit(node.Left);
                var leftResult = leftVisitor.Result;
                if (node.Right is BinaryExpression)
                {
                    rightVisitor = new BinaryExpressionVisitor(_joins);
                }
                else
                {
                    rightVisitor = new MemberExpressionVisitor(_joins);
                }
                rightVisitor.Visit(node.Right);
                var rightResult = rightVisitor.Result;
                if (leftVisitor.Type == MemberExpressionType.Object && rightVisitor.Type == MemberExpressionType.Object)
                {
                    if (leftResult == null && rightResult == null)
                    {
                        return true;
                    }
                    if (leftResult == null || rightResult == null)
                    {
                        return false;
                    }
                    if (leftResult is string || leftResult is bool || leftResult is bool?)
                    {
                        return leftResult.Equals(rightResult);
                    }
                    #region 比大小
                    if (leftResult is DateTime || leftResult is DateTime?)
                    {
                        var left = Convert.ToDateTime(leftResult);
                        var right = Convert.ToDateTime(rightResult);
                        switch (node.NodeType)
                        {
                            case ExpressionType.LessThan:
                                return left < right;
                            case ExpressionType.LessThanOrEqual:
                                return left <= right;
                            case ExpressionType.Equal:
                                return left = right;
                            case ExpressionType.GreaterThan:
                                return left > right;
                            case ExpressionType.GreaterThanOrEqual:
                                return left >= right;
                            default:
                                throw new Exception();
                        }
                    }
                    else if (leftResult is int || leftResult is int?)
                    {
                        var left = Convert.ToInt32(leftResult);
                        var right = Convert.ToInt32(rightResult);
                        switch (node.NodeType)
                        {
                            case ExpressionType.LessThan:
                                return left < right;
                            case ExpressionType.LessThanOrEqual:
                                return left <= right;
                            case ExpressionType.Equal:
                                return left = right;
                            case ExpressionType.GreaterThan:
                                return left > right;
                            case ExpressionType.GreaterThanOrEqual:
                                return left >= right;
                            default:
                                throw new Exception();
                        }
                    }
                    else if (leftResult is short || leftResult is short?)
                    {
                        var left = Convert.ToInt16(leftResult);
                        var right = Convert.ToInt16(rightResult);
                        switch (node.NodeType)
                        {
                            case ExpressionType.LessThan:
                                return left < right;
                            case ExpressionType.LessThanOrEqual:
                                return left <= right;
                            case ExpressionType.Equal:
                                return left = right;
                            case ExpressionType.GreaterThan:
                                return left > right;
                            case ExpressionType.GreaterThanOrEqual:
                                return left >= right;
                            default:
                                throw new Exception();
                        }
                    }
                    else if (leftResult is long || leftResult is long?)
                    {
                        var left = Convert.ToInt64(leftResult);
                        var right = Convert.ToInt64(rightResult);
                        switch (node.NodeType)
                        {
                            case ExpressionType.LessThan:
                                return left < right;
                            case ExpressionType.LessThanOrEqual:
                                return left <= right;
                            case ExpressionType.Equal:
                                return left = right;
                            case ExpressionType.GreaterThan:
                                return left > right;
                            case ExpressionType.GreaterThanOrEqual:
                                return left >= right;
                            default:
                                throw new Exception();
                        }
                    }
                    #endregion
                }
                else if (leftVisitor.Type == MemberExpressionType.Column && rightVisitor.Type == MemberExpressionType.Object)
                {
                    var token = leftVisitor.Result as Token;
                    var condition = new Condition();
                    condition.Left = token;
                    condition.CompareType = SelectMathCompareType(node.NodeType);
                    condition.Right = Token.Create(rightVisitor.Result);
                    return condition;
                }
                else
                {
                    throw new Exception();
                }
                throw new Exception();
            }

            private CompareType SelectMathCompareType(ExpressionType expressionType)
            {
                switch (expressionType)
                {
                    case ExpressionType.LessThan:
                        return CompareType.LessThan;
                    case ExpressionType.LessThanOrEqual:
                        return CompareType.LessThanOrEqual;
                    case ExpressionType.Equal:
                        return CompareType.Equal;
                    case ExpressionType.GreaterThan:
                        return CompareType.GreaterThan;
                    case ExpressionType.GreaterThanOrEqual:
                        return CompareType.GreaterThanOrEqual;
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                        return CompareType.Add;
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        return CompareType.Substarct;
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Multiply:
                        return CompareType.Multiply;
                    case ExpressionType.Divide:
                        return CompareType.Divide;
                    default:
                        throw new Exception();
                }
            }

            /// <summary>
            /// 处理And Or的逻辑表达式
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            object ParseLogicBinary(BinaryExpression node)
            {
                var leftResult = VisitBinaryNode(node.Left);
                if (leftResult == null)
                {
                    return null;
                }
                if (leftResult is bool)
                {
                    Result = leftResult;
                    var r = (bool)leftResult;
                    //左支结果为布尔
                    if (r)
                    {
                        //左支结果为真
                        if (node.NodeType == ExpressionType.OrElse)
                        {
                            //逻辑关系为或，整个表达式丢弃
                            return null;
                        }
                        //逻辑关系为与，左支可丢弃，可计算右支
                    }
                    else
                    {
                        //左支结果为假
                        if (node.NodeType == ExpressionType.AndAlso)
                        {
                            //逻辑关系为与，整个表达式为假
                            return null;
                        }
                        //逻辑关系为或，左支可丢弃，可计算右支
                    }
                }
                var rightResult = VisitBinaryNode(node.Right);
                if (leftResult is bool)
                {
                    if (rightResult == null)
                    {
                        return leftResult;
                    }
                    else if (rightResult is bool)
                    {
                        switch (node.NodeType)
                        {
                            case ExpressionType.AndAlso:
                                return (bool)rightResult && (bool)leftResult;
                            case ExpressionType.OrElse:
                                return (bool)rightResult || (bool)leftResult;
                            default:
                                throw new Exception();
                        }
                    }
                    else if (rightResult is Token || rightResult is Condition)
                    {
                        var lr = (bool)leftResult;
                        switch (node.NodeType)
                        {
                            case ExpressionType.AndAlso:
                                if (lr)
                                {
                                    return rightResult;
                                }
                                break;
                            case ExpressionType.OrElse:
                                if (!lr)
                                {
                                    return rightResult;
                                }
                                break;
                        }
                        return null;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (leftResult is Token)
                {
                    if (rightResult == null)
                    {
                        return leftResult;
                    }
                    else if (rightResult is bool)
                    {
                        var rr = (bool)rightResult;
                        switch (node.NodeType)
                        {
                            case ExpressionType.AndAlso:
                                if (rr)
                                {
                                    return leftResult;
                                }
                                break;
                            case ExpressionType.OrElse:
                                if (!rr)
                                {
                                    return leftResult;
                                }
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    else if (rightResult is Token)
                    {
                        var leftToken = leftResult as Token;
                        var rightToken = rightResult as Token;
                        var condition = new Condition();
                        condition.Left = leftToken;
                        condition.Right = rightToken;
                        condition.CompareType = SelectLogicCompareType(node.NodeType);
                        return Token.Create(condition);
                    }
                    else if (rightResult is Condition)
                    {
                        var leftToken = leftResult as Token;
                        var rightToken = rightResult as Condition;
                        var condition = new Condition();
                        condition.Left = leftToken;
                        condition.Right = Token.Create(rightToken);
                        condition.CompareType = SelectLogicCompareType(node.NodeType);
                        return Token.Create(condition);
                    }
                    else
                    {
                        var leftToken = leftResult as Token;
                        var rightToken = Token.Create(rightResult);
                        var condition = new Condition();
                        condition.Left = leftToken;
                        condition.Right = rightToken;
                        condition.CompareType = SelectLogicCompareType(node.NodeType);
                        return Token.Create(condition);
                    }
                }
                else if (leftResult is Condition)
                {
                    var left = (Condition)leftResult;
                    if (rightResult == null)
                    {
                        return Token.Create(left);
                    }
                    else if (rightResult is bool)
                    {
                        var rr = (bool)rightResult;
                        switch (node.NodeType)
                        {
                            case ExpressionType.AndAlso:
                                if (rr)
                                {
                                    return Token.Create(left);
                                }
                                break;
                            case ExpressionType.OrElse:
                                if (!rr)
                                {
                                    return Token.Create(left);
                                }
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    else if (rightResult is Token)
                    {
                        var right = (Token)rightResult;
                        var condition = new Condition();
                        condition.Left = Token.Create(left);
                        condition.Right = right;
                        condition.CompareType = SelectLogicCompareType(node.NodeType);
                        return Token.Create(condition);
                    }
                    else if (rightResult is Condition)
                    {
                        var right = (Condition)rightResult;
                        var condition = new Condition();
                        condition.Left = Token.Create(left);
                        condition.Right = Token.Create(right);
                        condition.CompareType = SelectLogicCompareType(node.NodeType);
                        return Token.Create(condition);
                    }
                    else
                    {
                        var leftToken = leftResult as Token;
                        var rightToken = Token.Create(rightResult);
                        var condition = new Condition();
                        condition.Left = leftToken;
                        condition.Right = rightToken;
                        condition.CompareType = SelectLogicCompareType(node.NodeType);
                        return Token.Create(condition);
                    }
                }
                else
                {
                    if (rightResult == null)
                    {
                        throw new Exception();
                    }
                    else if (rightResult is bool)
                    {
                        throw new Exception();
                    }
                    else if (rightResult is Token)
                    {
                        var leftToken = Token.Create(leftResult);
                        var rightToken = rightResult as Token;
                        var condition = new Condition();
                        condition.Left = leftToken;
                        condition.Right = rightToken;
                        condition.CompareType = SelectLogicCompareType(node.NodeType);
                        return Token.Create(condition);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                throw new Exception();
            }

            private CompareType SelectLogicCompareType(ExpressionType expressionType)
            {
                switch (expressionType)
                {
                    case ExpressionType.AndAlso:
                        return CompareType.And;
                    case ExpressionType.OrElse:
                        return CompareType.Or;
                    default:
                        throw new Exception();
                }
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                BinaryExpressionVisitor visitor = new BinaryExpressionVisitor(_joins);
                visitor.Visit(node.Left);
                Result = visitor.Result;
                return base.VisitBinary(node);
            }
            object VisitBinaryNode(Expression node)
            {
                BinaryExpressionVisitor visitor = new BinaryExpressionVisitor(_joins);
                visitor.Visit(node);
                return visitor.Result;
            }
        }
    }
}
