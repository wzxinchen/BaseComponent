using PPD.XLinq.Provider.SqlServer2008R2.Visitors;
using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    internal class SqlExpressionParser : ExpressionVisitor
    {
        //SqlServer2008R2Provider provider = null;
        bool _distinct = false;
        bool _count = false;
        Dictionary<string, Expression> _aggregationExpressions;
        Dictionary<string, Column> _aggregationColumns;

        public Dictionary<string, Column> AggregationColumns
        {
            get { return _aggregationColumns; }
        }

        public bool Distinct
        {
            get { return _distinct; }
        }
        public Type ElementType { get; set; }
        public ParseResult Result { get; private set; }
        public List<Column> Columns
        {
            get;
            private set;
        }

        public List<string> NoLockTables { get; private set; }

        public IList<object> Conditions { get; private set; }
        List<Expression> _nolockExpressions;
        Expression _selectExpression;
        Expression _sumExpression;
        Expression _averageExpression;
        List<KeyValuePair<string, Expression>> _expression = new List<KeyValuePair<string, Expression>>();
        List<Expression> _whereExpressions = new List<Expression>();
        List<Expression> _joinExpressions = new List<Expression>();
        public Dictionary<string, Join> Joins
        {
            get;
            private set;
        }
        public SqlExpressionParser()
        {
        }

        internal void Parse(Expression expression)
        {
            _distinct = false;
            _aggregationExpressions = new Dictionary<string, Expression>();
            Conditions = new List<object>();
            _nolockExpressions = new List<Expression>();
            NoLockTables = new List<string>();
            _aggregationColumns = new Dictionary<string, Column>();
            Visit(expression);
            if (_aggregationColumns.Count > 1)
            {
                throw new Exception();
            }
            #region 解析Where子句
            foreach (MethodCallExpression node in _whereExpressions)
            {
                var unary = node.Arguments[1] as UnaryExpression;
                var operand = unary.Operand as LambdaExpression;
                var body = operand.Body;
                if (body is ConstantExpression)
                {
                    var constExp = body as ConstantExpression;
                    if ((bool)constExp.Value == false)
                    {
                        Conditions.Add(false);
                    }
                }
                else
                {
                    VisitWhereExpression(body);
                }
            }
            #endregion

            #region 解析Join子句
            foreach (MethodCallExpression node in _joinExpressions)
            {
                VisitJoinExpression(node);
                break;
            }
            #endregion

            #region 解析Select子句
            if (_selectExpression != null)
            {
                var unary = _selectExpression as UnaryExpression;
                var lambdaExp = unary.Operand as LambdaExpression;
                var newExp = lambdaExp.Body as NewExpression;
                if (newExp == null)
                {
                    throw new NotSupportedException("Select子句中只能使用new表达式");
                }
                VisitSelectExpression(newExp);
            }
            else
            {
                VisitSelectExpression(null);
            }
            #endregion

            #region 解析Lock子句
            foreach (var nolockExpression in _nolockExpressions)
            {
                VisitNoLockExpression(nolockExpression);
            }
            #endregion

            //#region 解析Sum子句
            //if (_sumExpression != null)
            //{
            //    VisitSumExpression(_sumExpression);
            //}
            //#endregion

            //if (_averageExpression != null)
            //{
            //    VisitAverageExpression(_averageExpression);
            //}

            foreach (var aggreationExpression in _aggregationExpressions)
            {
                VisitAggreationExpression(aggreationExpression);
            }
        }

        private void VisitAggreationExpression(KeyValuePair<string, Expression> aggreationExpression)
        {
            if (aggreationExpression.Value != null)
            {
                var visitor = new MemberExpressionVisitor(Joins);
                visitor.Visit(aggreationExpression.Value);
                if (visitor.Type != MemberExpressionType.Column)
                {
                    throw new Exception("只能针对列进行聚合操作");
                }
                _aggregationColumns.Add(aggreationExpression.Key, ((Token)visitor.Result).Column);
            }
            else
            {
                _aggregationColumns.Add(aggreationExpression.Key, null);
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            while (true)
            {
                switch (node.Method.Name)
                {
                    case "Select":
                        _selectExpression = node.Arguments[1];
                        break;
                    case "Where":
                        _whereExpressions.Add(node);
                        break;
                    case "Join":
                    case "GroupJoin":
                    case "SelectMany":
                        _joinExpressions.Add(node);
                        break;
                    case "DefaultIfEmpty":

                        break;
                    case "Distinct":
                        _distinct = true;
                        break;
                    case "NoLock":
                        _nolockExpressions.Add(node);
                        break;
                    case "Count":
                    case "LongCount":
                    case "Sum":
                    case "Average":
                        Expression exp = null;
                        if (node.Arguments.Count >= 2)
                        {
                            if (node.Method.Name == "Count")
                            {
                                throw new Exception("不支持调用Count方法的当前重载");
                            }
                            exp = node.Arguments[1];
                        }
                        _aggregationExpressions.Add(node.Method.Name, exp);
                        break;
                    default:
                        throw new NotSupportedException("未支持的方法：" + node.Method.Name);
                }
                var call = node.Arguments[0];
                if (call.NodeType == ExpressionType.Call)
                {
                    node = (MethodCallExpression)call;
                    continue;
                }
                break;
            }
            return node;
        }

        private void VisitJoinExpression(MethodCallExpression node)
        {
            var visitor = new JoinExpressionVisitor();
            visitor.Visit(node);
            Joins = visitor.Joins;
            if (_selectExpression == null)
                _selectExpression = visitor.SelectExpression;
        }

        private void VisitWhereExpression(Expression binaryExpression)
        {
            var visitor = new WhereExpressionVisitor(Joins);
            visitor.Visit(binaryExpression);
            Conditions.Add(visitor.Result);
        }



        private void VisitSelectExpression(NewExpression node)
        {
            var visitor = new SelectExpressionVisitor(ElementType, Joins);
            visitor.Visit(node);
            Columns = visitor.Columns;
        }

        private void VisitNoLockExpression(Expression node)
        {
            var visitor = new NoLockExpressionVisitor();
            visitor.Visit(node);
            NoLockTables.Add(visitor.Result.ToString());
        }
    }
}
