using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xinchen.DbEntity;

namespace Xinchen.XLinq
{
    public class QueryProvider : IQueryProvider
    {
        private string _columns;
        private Expression _expression;
        private string _sql;
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            _expression = expression;
            return new QueryEntitySet<TElement>(_context,this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            _expression = expression;
            Type elementType = null;
            foreach (Type arg in expression.Type.GetGenericArguments())
            {
                Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                if (ienum.IsAssignableFrom(expression.Type))
                {
                    elementType = ienum;
                }
            }
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(QueryEntitySet<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public QueryProvider(EntityContext context)
        {
            this._context = context;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return default(TResult);
        }

        //void VisitMethodCallExpression(MethodCallExpression exp)
        //{
        //    foreach (var argExp in exp.Arguments)
        //    {
        //        switch (argExp.NodeType)
        //        {
        //            case ExpressionType.Call:
        //                var methodExp = argExp as MethodCallExpression;
        //                switch (methodExp.Method.Name)
        //                {
        //                    case "Select":
        //                        var selectVisitor = new SelectExpressionVisitor(((UnaryExpression)methodExp.Arguments[1]).Operand);
        //                        selectVisitor.Visit();
        //                        _columns = string.Join(",", selectVisitor.Columns);
        //                        break;
        //                    case "Where":
        //                        var whereVisitor = new WhereExpressionVisitor(((UnaryExpression)methodExp.Arguments[1]).Operand);
        //                        whereVisitor.Visit();
        //                        //_columns = string.Join(",", selectVisitor.Columns);
        //                        break;
        //                }
        //                VisitMethodCallExpression(argExp as MethodCallExpression);
        //                break;
        //            case ExpressionType.Constant:
        //                break;
        //            case ExpressionType.Quote:

        //                break;
        //        }
        //    }
        //}

        public object Execute(Expression expression)
        {
            _expression = expression;
            Translate(expression);
            //MethodCallExpression expMethodCall = (MethodCallExpression)expression;
            //VisitMethodCallExpression(expMethodCall);
            return null;
        }

        private string Translate(Expression expression)
        {
            if (string.IsNullOrEmpty(_sql))
            {
                var visitor = new SqlExpressionVisitor(expression);
                visitor.Visit();
                _sql = visitor.CommandText;
                Parameters = visitor.Parameters;
            }
            return _sql;
        }

        public override string ToString()
        {
            return Translate(_expression);
        }

        public Dictionary<string,object> Parameters { get;private set; }

        private EntityContext _context;
    }
}
