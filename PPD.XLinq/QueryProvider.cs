using PPD.XLinq.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public class QueryProvider : IQueryProvider
    {
        Expression _expression;
        Type _elementType;
        public QueryProvider(Type elementType)
        {
            _elementType = elementType;
        }
        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            _expression = expression;
            return new DataQuery<TElement>(this, expression);
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            var provider = ProviderFactory.CreateProvider(DataContext.DataBase);
            var parser = provider.CreateParser();
            parser.ElementType = _elementType;
            parser.Parse(expression);
            provider.CreateSqlExecutor().ExecuteDataSet(parser.Result.CommandText, parser.Result.Parameters);
            Type type = typeof(TResult);
            if (type.IsGenericType)
            {
                var elementType = type.GetGenericArguments()[0];
                return (TResult)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            }
            else
            {
                return default(TResult);
            }
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
