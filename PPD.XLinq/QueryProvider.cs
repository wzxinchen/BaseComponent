using PPD.XLinq.Provider;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.ObjectMapper;

namespace PPD.XLinq
{
    public class QueryProvider : IQueryProvider
    {
        Expression _expression;
        Type _elementType;
        DataContext _context;
        public QueryProvider(DataContext context,Type elementType)
        {
            _context = context;
            _elementType = elementType;
        }
        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            _expression = expression;
            return new DataQuery<TElement>(this, expression);
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            Type elementType = expression.Type.GetGenericArguments()[0];
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(DataQuery<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            var provider = ProviderFactory.CreateProvider(DataContext.DataBase);
            var parser = provider.CreateParser();
            parser.ElementType = _elementType;
            parser.Parse(expression);
            var executor = provider.CreateSqlExecutor();
            var ds = executor.ExecuteDataSet(parser.Result.CommandText, parser.Result.Parameters);
            Type type = typeof(TResult);
            if (expression.NodeType == ExpressionType.Call)
            {
                var method = ((MethodCallExpression)expression).Method;
                if (method.Name == "Any")
                {
                    object r = ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
                    return (TResult)r;
                }
            }
            //if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
            //{
            //    var typeArg = type.GetGenericArguments()[0];
            //    if (method.Name == "ToList")
            //    {
            //        return (TResult)Activator.CreateInstance(typeof(List<>).MakeGenericType(typeArg));
            //    }
            //    if (method.Name == "FirstOrDefault")
            //    {
            //        return default(TResult);
            //    }
            //    else if (method.Name == "First")
            //    {
            //        throw new ArgumentOutOfRangeException();
            //    }
            //}
            bool isValueType = false;
            if (type.IsGenericType)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    return (TResult)EntityMapper.Map(type.GetGenericArguments()[0], ds);
                }
                if (typeof(Nullable<>).IsAssignableFrom(type))
                {
                    isValueType = true;
                }
                if (!isValueType)
                {
                    throw new Exception();
                }
            }
            if (type.IsValueType || isValueType)
            {
                if (ds.Tables[0].Rows.Count <= 0)
                {
                    return default(TResult);
                }
                var result = ds.Tables[0].Rows[0][0];
                if (result == DBNull.Value)
                {
                    return default(TResult);
                }
                return (TResult)Convert.ChangeType(result, type);
            }

            if (TableInfoManager.IsEntity(type))
            {
                var results = EntityMapper.Map(type, ds);
                if (results.Count <= 0)
                {
                    return default(TResult);
                }
                return (TResult)results[0];
            }
            throw new Exception();
            //if (type.IsGenericType)
            //{
            //    var typeDef = type.GetGenericTypeDefinition();
            //    if (typeDef == typeof(Nullable<>) && type.GetGenericArguments()[0].IsValueType)
            //    {
            //        if (result == DBNull.Value)
            //        {
            //            return default(TResult);
            //        }
            //        return (TResult)Convert.ChangeType(result, type);
            //    }
            //    else if (typeDef == typeof(IEnumerable))
            //    {
            //        return (TResult)EntityMapper.Map(typeArg, ds);
            //    }
            //    else
            //    {
            //        throw new Exception();
            //    }
            //}
            throw new Exception();
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            var type = expression.Type.GetGenericArguments()[0];
            return ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(type).Invoke(this,new object[]{ expression});
        }
    }
}
