using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SQLite.Parser
{
    public class WhereExpressionVisitor : ExpressionVisitorBase
    {
        Dictionary<string, Join> _joins;
        public WhereExpressionVisitor(TranslateContext context)
            : base(context)
        {
            this._joins = context.Joins;
        }
        protected override Expression VisitBinary(BinaryExpression node)
        {
            BinaryExpressionVisitor visitor = new BinaryExpressionVisitor(Context);
            visitor.Visit(node);
            Token = visitor.Token;
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            var visitor = new MethodCallExpressionVisitor(Context);
            visitor.Visit(node);
            Token = visitor.Token;
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    if (Token.IsBool())
                    {
                        Token = Token.Create(!((bool)Token.Object));
                    }
                    else if (Token.Type == TokenType.Column)
                    {
                        Token = Token.Create(new Condition()
                        {
                            CompareType = CompareType.Not,
                            Left = Token
                        });
                    }
                    else
                    {
                        throw new Exception();
                    }




                    //if (Result is bool)
                    //{
                    //    Type = MemberExpressionType.Object;
                    //    Result = !((bool)Result);
                    //}
                    //else if (visitor.Type == MemberExpressionType.Column)
                    //{
                    //    Type = MemberExpressionType.Column;
                    //    var token = (Token)Result;
                    //    switch (token.Type)
                    //    {
                    //        case TokenType.Column:
                    //            Result = Token.Create(new Condition()
                    //            {
                    //                CompareType = CompareType.Not,
                    //                Left = token
                    //            });
                    //            break;
                    //        default:
                    //            throw new Exception();
                    //    }
                    //}
                    break;
                default:
                    throw new Exception();
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var visitor = new MethodCallExpressionVisitor(Context);
            visitor.Visit(node);
            Token = visitor.Token;
            return node;
        }

        class BinaryExpressionVisitor : ExpressionVisitorBase
        {
            Dictionary<string, Join> _joins;
            public BinaryExpressionVisitor(TranslateContext context)
                : base(context)
            {
                this._joins = context.Joins;
            }
            public override Expression Visit(Expression node)
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Constant:
                        Token = Token.Create((((ConstantExpression)node)).Value);
                        return node;
                    case ExpressionType.OrElse:
                    case ExpressionType.AndAlso:
                        Token = ParseLogicBinary((BinaryExpression)node);
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
                        Token = ParseMathBinary((BinaryExpression)node);
                        return node;
                    case ExpressionType.Not:
                        Token = ParseNotExpression((UnaryExpression)node);
                        return node;
                    case ExpressionType.Call:
                        Token = ParseMethodCallExpression((MethodCallExpression)node);
                        return node;
                    default:
                        throw new Exception(node.NodeType.ToString());
                }
            }

            private Token ParseMethodCallExpression(MethodCallExpression methodCallExpression)
            {
                var visitor = new MethodCallExpressionVisitor(Context);
                visitor.Visit(methodCallExpression);
                return visitor.Token;
            }

            Token ParseNotExpression(UnaryExpression unaryExpression)
            {
                var operand = unaryExpression.Operand;
                if (operand is MemberExpression)
                {
                    var visitor = new MemberExpressionVisitor(Context);
                    visitor.Visit(operand);
                    switch (visitor.Token.Type)
                    {
                        case TokenType.Object:
                            if (visitor.Token.IsBool())
                            {
                                return Token.Create(!((bool)visitor.Token.Object));
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                        case TokenType.Column:
                            if (operand.Type == typeof(bool) || operand.Type == typeof(bool?))
                            {
                                return Token.Create(new Condition()
                                {
                                    CompareType = CompareType.Equal,
                                    Left = Token.Create(1),
                                    Right = Token.Create(1)
                                });
                            }
                            return Token.Create(new Condition()
                            {
                                Left = visitor.Token,
                                CompareType = CompareType.Not
                            });
                        default:
                            throw new Exception();
                    }
                }
                else if (operand is MethodCallExpression)
                {
                    var visitor = new MethodCallExpressionVisitor(Context);
                    visitor.Visit(operand);
                    var token = visitor.Token;
                    switch (token.Type)
                    {
                        case TokenType.Column:
                            return Token.Create(new Condition()
                            {
                                Left = token,
                                CompareType = CompareType.Not
                            });
                        case TokenType.Condition:
                            return Token.Create(new Condition()
                            {
                                Left = Token.Create(token.Condition),
                                CompareType = CompareType.Not
                            });
                        case TokenType.Object:
                            return Token.Create(!((bool)token.Object));
                        default:
                            throw new Exception();
                    }
                }
                else
                {
                    throw new Exception("不支持");
                }
                throw new Exception();
            }

            Token ParseMathBinary(BinaryExpression node)
            {
                ExpressionVisitorBase leftVisitor, rightVisitor;
                if (node.Left is BinaryExpression)
                {
                    leftVisitor = new BinaryExpressionVisitor(Context);
                }
                else
                {
                    leftVisitor = new MemberExpressionVisitor(Context);
                }
                leftVisitor.Visit(node.Left);
                var leftResult = leftVisitor.Token;
                if (node.Right is BinaryExpression)
                {
                    rightVisitor = new BinaryExpressionVisitor(Context);
                }
                else
                {
                    rightVisitor = new MemberExpressionVisitor(Context);
                }
                rightVisitor.Visit(node.Right);
                var rightResult = rightVisitor.Token;
                if (leftResult.Type == TokenType.Object && rightResult.Type == TokenType.Object)
                {
                    if (leftResult.Object == null && rightResult.Object == null)
                    {
                        return Token.Create(true);
                    }
                    if (leftResult == null || rightResult == null)
                    {
                        return Token.Create(false);
                    }
                    if (leftResult.Type == TokenType.Object)
                    {
                        if (leftResult.Object is string || leftResult.Object is bool || leftResult.Object is bool?)
                        {
                            return Token.Create(leftResult.Equals(rightResult));
                        }
                        #region 比大小
                        if (leftResult.Object is DateTime || leftResult.Object is DateTime?)
                        {
                            var left = Convert.ToDateTime(leftResult.Object);
                            var right = Convert.ToDateTime(rightResult.Object);
                            switch (node.NodeType)
                            {
                                case ExpressionType.LessThan:
                                    return Token.Create(left < right);
                                case ExpressionType.LessThanOrEqual:
                                    return Token.Create(left <= right);
                                case ExpressionType.Equal:
                                    return Token.Create(left = right);
                                case ExpressionType.GreaterThan:
                                    return Token.Create(left > right);
                                case ExpressionType.GreaterThanOrEqual:
                                    return Token.Create(left >= right);
                                default:
                                    throw new Exception();
                            }
                        }
                        else if (leftResult.Object is int || leftResult.Object is int?)
                        {
                            var left = Convert.ToInt32(leftResult.Object);
                            var right = Convert.ToInt32(rightResult.Object);
                            switch (node.NodeType)
                            {
                                case ExpressionType.LessThan:
                                    return Token.Create(left < right);
                                case ExpressionType.LessThanOrEqual:
                                    return Token.Create(left <= right);
                                case ExpressionType.Equal:
                                    return Token.Create(left = right);
                                case ExpressionType.GreaterThan:
                                    return Token.Create(left > right);
                                case ExpressionType.GreaterThanOrEqual:
                                    return Token.Create(left >= right);
                                default:
                                    throw new Exception();
                            }
                        }
                        else if (leftResult.Object is short || leftResult.Object is short?)
                        {
                            var left = Convert.ToInt16(leftResult.Object);
                            var right = Convert.ToInt16(rightResult.Object);
                            switch (node.NodeType)
                            {
                                case ExpressionType.LessThan:
                                    return Token.Create(left < right);
                                case ExpressionType.LessThanOrEqual:
                                    return Token.Create(left <= right);
                                case ExpressionType.Equal:
                                    return Token.Create(left = right);
                                case ExpressionType.GreaterThan:
                                    return Token.Create(left > right);
                                case ExpressionType.GreaterThanOrEqual:
                                    return Token.Create(left >= right);
                                default:
                                    throw new Exception();
                            }
                        }
                        else if (leftResult.Object is long || leftResult.Object is long?)
                        {
                            var left = Convert.ToInt64(leftResult.Object);
                            var right = Convert.ToInt64(rightResult.Object);
                            switch (node.NodeType)
                            {
                                case ExpressionType.LessThan:
                                    return Token.Create(left < right);
                                case ExpressionType.LessThanOrEqual:
                                    return Token.Create(left <= right);
                                case ExpressionType.Equal:
                                    return Token.Create(left = right);
                                case ExpressionType.GreaterThan:
                                    return Token.Create(left > right);
                                case ExpressionType.GreaterThanOrEqual:
                                    return Token.Create(left >= right);
                                default:
                                    throw new Exception();
                            }
                        }
                        else
                        {
                            return Token.Create(leftResult.Object == rightResult.Object);
                        }
                        #endregion
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (leftResult.Type == TokenType.Column && rightResult.Type == TokenType.Object)
                {
                    var condition = new Condition();
                    condition.Left = leftResult;
                    condition.CompareType = SelectMathCompareType(node.NodeType);
                    condition.Right = Token.Create(rightResult.Object);
                    return Token.Create(condition);
                }
                else if (leftResult.Type == TokenType.Condition && rightResult.Type == TokenType.Object)
                {
                    var condition = new Condition();
                    condition.Left = leftResult;
                    condition.CompareType = SelectMathCompareType(node.NodeType);
                    condition.Right = Token.Create(rightResult.Object);
                    return Token.Create(condition);
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
                    case ExpressionType.NotEqual:
                        return CompareType.NotEqual;
                    default:
                        throw new Exception();
                }
            }

            /// <summary>
            /// 处理And Or的逻辑表达式
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            Token ParseLogicBinary(BinaryExpression node)
            {
                var leftResult = VisitBinaryNode(node.Left);
                if (leftResult.IsNull())
                {
                    return leftResult;
                }
                if (leftResult.IsBool())
                {
                    Token = leftResult;
                    var r = (bool)leftResult.Object;
                    //左支结果为布尔
                    if (r)
                    {
                        //左支结果为真
                        if (node.NodeType == ExpressionType.OrElse)
                        {
                            //逻辑关系为或，整个表达式丢弃
                            return Token.CreateNull();
                        }
                        //逻辑关系为与，左支可丢弃，可计算右支
                    }
                    else
                    {
                        //左支结果为假
                        if (node.NodeType == ExpressionType.AndAlso)
                        {
                            //逻辑关系为与，整个表达式为假
                            return Token.CreateNull();
                        }
                        //逻辑关系为或，左支可丢弃，可计算右支
                    }
                }
                var rightResult = VisitBinaryNode(node.Right);
                if (leftResult.IsBool())
                {
                    if (rightResult.IsNull())
                    {
                        return leftResult;
                    }
                    else if (rightResult.IsBool())
                    {
                        switch (node.NodeType)
                        {
                            case ExpressionType.AndAlso:
                                return Token.Create((bool)rightResult.Object && (bool)leftResult.Object);
                            case ExpressionType.OrElse:
                                return Token.Create((bool)rightResult.Object || (bool)leftResult.Object);
                            default:
                                throw new Exception();
                        }
                    }
                    else if (rightResult.Type == TokenType.Condition || rightResult.Type == TokenType.Column)
                    {
                        var lr = (bool)leftResult.Object;
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
                        return Token.CreateNull();
                    }
                    else
                    {
                        throw new Exception();
                    }
                    //else if (rightResult is Token || rightResult is Condition)
                    //{
                    //    var lr = (bool)leftResult;
                    //    switch (node.NodeType)
                    //    {
                    //        case ExpressionType.AndAlso:
                    //            if (lr)
                    //            {
                    //                return rightResult;
                    //            }
                    //            break;
                    //        case ExpressionType.OrElse:
                    //            if (!lr)
                    //            {
                    //                return rightResult;
                    //            }
                    //            break;
                    //    }
                    //    return null;
                    //}
                    //else
                    //{
                    //    throw new Exception();
                    //}
                }//else if (leftResult is Condition)
                else if (leftResult.Type == TokenType.Condition)
                {
                    var left = (Condition)leftResult.Condition;
                    if (rightResult == null)
                    {
                        return Token.Create(left);
                    }
                    else if (rightResult.IsBool())
                    {
                        var rr = (bool)rightResult.Object;
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
                    else if (rightResult.Type == TokenType.Condition)
                    {
                        var condition = new Condition();
                        condition.Left = Token.Create(left);
                        condition.Right = rightResult;
                        condition.CompareType = SelectLogicCompareType(node.NodeType);
                        return Token.Create(condition);
                    }
                    else if (rightResult.Type == TokenType.Column)
                    {
                        var condition = new Condition();
                        condition.Left = Token.Create(left);
                        condition.Right = rightResult;
                        condition.CompareType = SelectLogicCompareType(node.NodeType);
                        return Token.Create(condition);
                    }
                    else
                    {
                        var leftToken = (Token)leftResult;
                        var rightToken = Token.Create(rightResult);
                        var condition = new Condition();
                        condition.Left = leftToken;
                        condition.Right = rightToken;
                        condition.CompareType = SelectLogicCompareType(node.NodeType);
                        return Token.Create(condition);
                    }
                }
                //else if (leftResult is Token)
                //{
                //    if (rightResult == null)
                //    {
                //        return leftResult;
                //    }
                //    else if (rightResult is bool)
                //    {
                //        var rr = (bool)rightResult;
                //        switch (node.NodeType)
                //        {
                //            case ExpressionType.AndAlso:
                //                if (rr)
                //                {
                //                    return leftResult;
                //                }
                //                break;
                //            case ExpressionType.OrElse:
                //                if (!rr)
                //                {
                //                    return leftResult;
                //                }
                //                break;
                //            default:
                //                throw new Exception();
                //        }
                //    }
                //    else if (rightResult is Token)
                //    {
                //        var leftToken = leftResult as Token;
                //        var rightToken = rightResult as Token;
                //        var condition = new Condition();
                //        condition.Left = leftToken;
                //        condition.Right = rightToken;
                //        condition.CompareType = SelectLogicCompareType(node.NodeType);
                //        return Token.Create(condition);
                //    }
                //    else if (rightResult is Condition)
                //    {
                //        var leftToken = leftResult as Token;
                //        var rightToken = rightResult as Condition;
                //        var condition = new Condition();
                //        condition.Left = leftToken;
                //        condition.Right = Token.Create(rightToken);
                //        condition.CompareType = SelectLogicCompareType(node.NodeType);
                //        return Token.Create(condition);
                //    }
                //    else
                //    {
                //        var leftToken = leftResult as Token;
                //        var rightToken = Token.Create(rightResult);
                //        var condition = new Condition();
                //        condition.Left = leftToken;
                //        condition.Right = rightToken;
                //        condition.CompareType = SelectLogicCompareType(node.NodeType);
                //        return Token.Create(condition);
                //    }
                //}
                //else if (leftResult is Condition)
                //{
                //    var left = (Condition)leftResult;
                //    if (rightResult == null)
                //    {
                //        return Token.Create(left);
                //    }
                //    else if (rightResult is bool)
                //    {
                //        var rr = (bool)rightResult;
                //        switch (node.NodeType)
                //        {
                //            case ExpressionType.AndAlso:
                //                if (rr)
                //                {
                //                    return Token.Create(left);
                //                }
                //                break;
                //            case ExpressionType.OrElse:
                //                if (!rr)
                //                {
                //                    return Token.Create(left);
                //                }
                //                break;
                //            default:
                //                throw new Exception();
                //        }
                //    }
                //    else if (rightResult is Token)
                //    {
                //        var right = (Token)rightResult;
                //        var condition = new Condition();
                //        condition.Left = Token.Create(left);
                //        condition.Right = right;
                //        condition.CompareType = SelectLogicCompareType(node.NodeType);
                //        return Token.Create(condition);
                //    }
                //    else if (rightResult is Condition)
                //    {
                //        var right = (Condition)rightResult;
                //        var condition = new Condition();
                //        condition.Left = Token.Create(left);
                //        condition.Right = Token.Create(right);
                //        condition.CompareType = SelectLogicCompareType(node.NodeType);
                //        return Token.Create(condition);
                //    }
                //    else
                //    {
                //        var leftToken = leftResult as Token;
                //        var rightToken = Token.Create(rightResult);
                //        var condition = new Condition();
                //        condition.Left = leftToken;
                //        condition.Right = rightToken;
                //        condition.CompareType = SelectLogicCompareType(node.NodeType);
                //        return Token.Create(condition);
                //    }
                //}
                //else
                //{
                //    if (rightResult == null)
                //    {
                //        throw new Exception();
                //    }
                //    else if (rightResult is bool)
                //    {
                //        throw new Exception();
                //    }
                //    else if (rightResult is Token)
                //    {
                //        var leftToken = Token.Create(leftResult);
                //        var rightToken = rightResult as Token;
                //        var condition = new Condition();
                //        condition.Left = leftToken;
                //        condition.Right = rightToken;
                //        condition.CompareType = SelectLogicCompareType(node.NodeType);
                //        return Token.Create(condition);
                //    }
                //    else
                //    {
                //        throw new Exception();
                //    }
                //}
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
                BinaryExpressionVisitor visitor = new BinaryExpressionVisitor(Context);
                visitor.Visit(node.Left);
                Token = visitor.Token;
                return node;
            }
            Token VisitBinaryNode(Expression node)
            {
                BinaryExpressionVisitor visitor = new BinaryExpressionVisitor(Context);
                visitor.Visit(node);
                return visitor.Token;
            }
        }
    }
}
