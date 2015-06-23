using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DbUtils;

namespace PPD.XLinq
{
    public static class Extension
    {
        private static Expression GetSourceExpression<TSource>(IEnumerable<TSource> source)
        {
            IQueryable<TSource> queryable = source as IQueryable<TSource>;
            if (queryable != null)
            {
                return queryable.Expression;
            }
            return Expression.Constant(source, typeof(IEnumerable<TSource>));
        }

        public static IQueryable<TSource> NoLock<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.CreateQuery<TSource>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression }));
        }

        public static PageResult<TSource> Page<TSource>(this IQueryable<TSource> source, int page, int size)
        {
            var pageResult = new PageResult<TSource>();
            pageResult.RecordCount = source.Count();
            pageResult.Data = source.Skip((page - 1) * size).Take(size).ToList();
            return pageResult;
        }
        public static int Delete<TSource>(this IQueryable<TSource> source)
        {
            return source.Provider.Execute<int>(Expression.Call(null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TSource) }), new Expression[] { source.Expression }));
        }
        public static int Delete<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return source.Provider.Execute<int>((Expression)Expression.Call((Expression)null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource)), new Expression[2]
      {
        source.Expression,
        Expression.Quote(predicate)
      }));
        }
        public static int Update<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource>> predicate)
        {
            return source.Provider.Execute<int>((Expression)Expression.Call((Expression)null, ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TSource)), new Expression[2]
      {
        source.Expression,
        Expression.Quote(predicate)
      }));
        }
    }
}
