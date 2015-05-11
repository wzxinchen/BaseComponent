using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xinchen.DbEntity;

namespace Xinchen.XLinq
{
    public class QueryEntitySet<TEntity> : EntitySet<TEntity>, IQueryable<TEntity>, IOrderedQueryable<TEntity>
    {
        Expression _expression;
        QueryProvider _provider;
        private Dictionary<Type, object> _entitySets;
        public Dictionary<string, object> Parameters
        {
            get
            {
                return ((QueryProvider)Provider).Parameters;
            }
        }
        public QueryEntitySet(EntityContext context)
            : this(context, new QueryProvider(context), null)
        {

        }
        public QueryEntitySet(EntityContext context, QueryProvider provider, Expression expression)
            : base(context)
        {
            if (expression == null)
            {
                _expression = Expression.Constant(this);
            }
            else
            {
                _expression = expression;
            }
            _provider = provider;

        }
        public IEnumerator<TEntity> GetEnumerator()
        {
            return ((IEnumerable<TEntity>)Provider.Execute(Expression)).GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Provider.Execute(Expression)).GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(TEntity); }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public IQueryProvider Provider
        {
            get { return _provider; }
        }

        public override string ToString()
        {
            return Provider.ToString();
        }
    }
}
