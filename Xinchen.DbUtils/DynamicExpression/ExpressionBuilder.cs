using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DbUtils;

namespace Xinchen.DbUtils.DynamicExpression
{
    public class ExpressionBuilder<TParameter>
    {
        ParameterExpression _parameterExpression;
        Type _parameterType;
        public ExpressionBuilder()
        {
            _parameterType = typeof(TParameter);
            _parameterExpression = Expression.Parameter(_parameterType, _parameterType.Name);
        }


        /// <summary>
        /// 构造一个相等的二元逻辑表达式
        /// </summary>
        /// <param name="key">属性名，用于访问TParameter</param>
        /// <param name="value">该属性名等于什么值</param>
        /// <returns></returns>
        public Expression Equal(string key, object value)
        {
            var propertyExp = Expression.Property(_parameterExpression, key);
            var valueExp = Expression.Constant(value);
            return Expression.Equal(propertyExp, valueExp);
        }
        /// <summary>
        /// 构造一个二元逻辑表达式
        /// </summary>
        /// <param name="key">属性名，用于访问TParameter</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public BinaryExpression BuildBinaryExpression(string key, object value, Operation operation)
        {
            Expression propertyExp = Expression.Property(_parameterExpression, key);
            if (propertyExp.Type.IsGenericType && propertyExp.Type.GetGenericTypeDefinition() == ExpressionConsts.NullableType)
            {
                propertyExp = Expression.Convert(propertyExp, value.GetType());
            }
            var valueExp = Expression.Constant(value);
            switch (operation)
            {
                case Operation.Equal:
                    return Expression.Equal(propertyExp, valueExp);
                case Operation.GreaterThan:
                    return Expression.GreaterThan(propertyExp, valueExp);
                case Operation.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(propertyExp, valueExp);
                case Operation.LessThan:
                    return Expression.LessThan(propertyExp, valueExp);
                case Operation.LessThanOrEqual:
                    return Expression.LessThanOrEqual(propertyExp, valueExp);
                case Operation.NotEqual:
                    return Expression.NotEqual(propertyExp, valueExp);
                default:
                    throw new Exception(operation.ToString());
            }
        }

        public Expression And(params Expression[] expressions)
        {
            if (!expressions.Any())
            {
                return null;
            }
            if (expressions.Count() == 1)
            {
                return expressions[0];
            }

            Expression body = null;
            foreach (var expression in expressions)
            {
                if (body == null)
                {
                    body = expression;
                }
                else
                {
                    body = Expression.AndAlso(body, expression);
                }
            }

            return body;
        }

        public Expression<Func<TParameter, bool>> Build(Expression bodyExp)
        {
            return Expression.Lambda<Func<TParameter, bool>>(bodyExp, _parameterExpression);
        }

        public Expression<Func<TParameter, bool>> Build(IList<SqlFilter> filters)
        {
            var expressions = new List<Expression>();
            foreach (var filter in filters)
            {
                switch (filter.Operation)
                {
                    case Operation.Equal:
                    case Operation.GreaterThan:
                    case Operation.GreaterThanOrEqual:
                    case Operation.LessThan:
                    case Operation.LessThanOrEqual:
                    case Operation.NotEqual:
                        expressions.Add(BuildBinaryExpression(filter.Name, filter.Value, filter.Operation));
                        break;
                    case Operation.Like:
                    case Operation.List:
                        expressions.Add(BuildContains(filter.Name, filter.Value));
                        break;
                    default:
                        throw new Exception();
                }
            }

            var body = And(expressions.ToArray());
            return Build(body);
        }

        public Expression BuildContains(string propertyName, object value)
        {
            var propertyExp = Expression.Property(_parameterExpression, propertyName);
            var valueExp = Expression.Constant(value);
            if (value is string)
            {
                return Expression.Call(propertyExp, ExpressionConsts.StringContains, valueExp);
            }
            return Expression.Call(null, ExpressionConsts.IEnumerableListContains.MakeGenericMethod(ExpressionConsts.Int32Type), valueExp, propertyExp);
        }

        public IQueryable<TParameter> Build(IQueryable<TParameter> source, IList<Sort> sorts)
        {
            bool first = true;
            foreach (var sort in sorts)
            {
                MemberExpression propertyExp = (MemberExpression)Expression.Property(_parameterExpression, sort.Field);
                var propertyType = ((PropertyInfo)propertyExp.Member).PropertyType;
                MethodInfo sortMethod = null;
                switch (sort.SortOrder)
                {
                    case SortOrder.ASC:
                        if (first)
                            sortMethod = ExpressionConsts.OrderByMethod.MakeGenericMethod(_parameterType, propertyType);
                        else
                            sortMethod = ExpressionConsts.ThenByMethod.MakeGenericMethod(_parameterType, propertyType);
                        break;
                    case SortOrder.DESCENDING:
                        if (first)
                            sortMethod = ExpressionConsts.OrderByDescendingMethod.MakeGenericMethod(_parameterType, propertyType);
                        else
                            sortMethod = ExpressionConsts.ThenByDescendingMethod.MakeGenericMethod(_parameterType, propertyType);
                        break;
                    default:
                        throw new Exception(sort.SortOrder.ToString());
                }
                first = false;
                var lambda = Expression.Lambda(propertyExp, _parameterExpression);
                source = source.Provider.CreateQuery<TParameter>(Expression.Call(null, sortMethod, source.Expression, Expression.Quote(lambda)));
            }
            return source;
        }
    }
}
