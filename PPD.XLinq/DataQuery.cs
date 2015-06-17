using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public class DataQuery<T> : IQueryable<T>, IOrderedQueryable<T>
    {
        private QueryProvider provider;
        private Expression _expression;
        public DataQuery(QueryProvider provider)
        {
            // TODO: Complete member initialization
            this.provider = provider;
            _expression = Expression.Constant(this);
        }
        public DataQuery(QueryProvider provider, Expression expression)
        {
            // TODO: Complete member initialization
            this.provider = provider;
            _expression = expression;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (IEnumerator)provider.Execute(Expression);
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return _expression; }
        }

        public IQueryProvider Provider
        {
            get { return provider; }
        }
    }
}
