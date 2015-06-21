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
    public class DbSet<T> : IQueryable<T>, IOrderedQueryable<T>, IOperateAddingEntities
    {
        DataQuery<T> _dataQuery;
        HashSet<T> _entitiesToAdd = new HashSet<T>();
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

        public T Add(T obj)
        {
            if (!_entitiesToAdd.Contains(obj))
            {
                _entitiesToAdd.Add(obj);
            }
            return obj;
        }

        ArrayList IOperateAddingEntities.GetAddingEntities()
        {
            var list = new ArrayList();
            foreach (var item in _entitiesToAdd)
            {
                list.Add(item);
            }
            return list;
        }

        public void ClearAddingEntities()
        {
            lock (_entitiesToAdd)
                _entitiesToAdd.Clear();
        }
    }
}
