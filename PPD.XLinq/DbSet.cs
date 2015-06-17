using PPD.XLinq.Provider;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public class DbSet<T> : IQueryable<T>, IOrderedQueryable<T>
    {
        DataQuery<T> _dataQuery;
        public DbSet(QueryProvider provider)
        {
            _dataQuery = new DataQuery<T>(provider);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _dataQuery.GetEnumerator();//.Provider.Execute<T>(Expression);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dataQuery.GetEnumerator();
        }

        public Type ElementType
        {
            get { return _dataQuery.ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return _dataQuery.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _dataQuery.Provider; }
        }
    }
}
