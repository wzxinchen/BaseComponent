using PPD.XLinq.Provider;
using PPD.XLinq.Provider.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DynamicObject;
using Xinchen.ObjectMapper;

namespace PPD.XLinq
{
    public class QueryProvider : IQueryProvider
    {
        Expression _expression;
        Type _elementType;
        DataContext _context;
        public QueryProvider(DataContext context, Type elementType)
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
            var provider = ProviderFactory.CreateProvider(ConfigManager.DataBaseType);
            var parser = provider.CreateParser();
            //var provider = ProviderFactory.CreateProvider(ConfigManager.DataBase);
            //var parser = provider.CreateParser();
            parser.ElementType = _elementType;
            parser.Parse(expression);
            Type type = typeof(TResult);
            var executor = provider.CreateSqlExecutor();
            if (expression.NodeType == ExpressionType.Call && type.IsValueType)
            {
                var method = ((MethodCallExpression)expression).Method;
                object r;
                DataSet ds;
                switch (method.Name)
                {
                    case "Any":
                        ds = executor.ExecuteDataSet(parser.Result.CommandText, parser.Result.Parameters);
                        r = ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
                        return (TResult)r;
                    case "Delete":
                    case "Update":
                        r = executor.ExecuteNonQuery(parser.Result.CommandText, parser.Result.Parameters);
                        return (TResult)r;
                    case "Average":
                    case "Sum":
                    case "Count":
                        r = executor.ExecuteScalar(parser.Result.CommandText, parser.Result.Parameters);
                        if (r == DBNull.Value)
                        {
                            return default(TResult);
                        }
                        return (TResult)Convert.ChangeType(r, type);
                    default:
                        throw new Exception();
                }
            }
            else
            {
                var ds = executor.ExecuteDataSet(parser.Result.CommandText, parser.Result.Parameters);
                bool isValueType = false;
                if (type.IsGenericType)
                {
                    if (typeof(IEnumerable).IsAssignableFrom(type))
                    {
                        var list = EntityMapper.Map(type.GetGenericArguments()[0], ds);
                        if (_context.EnableProxy)
                        {
                            if (list.Count > 10)
                            {
                                return (TResult)list;
                            }
                            if (TableInfoManager.IsEntity(type))
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    list[i] = DynamicProxy.CreateDynamicProxy(list[i]);
                                }
                            }
                            var entityOp = _context.GetEntityOperator(type);
                            entityOp.AddEditing(list);
                        }
                        return (TResult)list;
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
                    var result = results[0];
                    if (_context.EnableProxy)
                    {
                        var entityOp = _context.GetEntityOperator(type);
                        result = DynamicProxy.CreateDynamicProxy(result);
                        entityOp.AddEditing(new ArrayList() { result });
                        return (TResult)result;
                    }
                    return (TResult)result;
                }
                throw new Exception();
            }

        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            var type = expression.Type.GetGenericArguments()[0];
            return ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(type).Invoke(this, new object[] { expression });
        }
    }
}
