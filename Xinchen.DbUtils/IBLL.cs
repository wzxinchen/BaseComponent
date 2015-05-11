using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Xinchen.DbUtils
{
    public interface IBLL<T>
    {
       // void Add(params T[] items);

        //bool IsEmpty();

        //T Get(int id);
        T Get(Func<IQueryable<T>, IQueryable<T>> filter);
        IList<T> GetList(Func<IQueryable<T>, IQueryable<T>> filter);

        //IDictionary<TKey, T> GetDictionary<TKey>(Func<T, TKey> keySelector);

        //IList<T> GetList();
        //IList<T> GetList(params int[] ids);
        //IList<T> GetList(Expression<Func<T, bool>> filter);
        //PageResult<T> Page<TKey>(int page, int pageSize, Expression<Func<T, bool>> filter, Expression<Func<T, TKey>> order);

        //int GetSequencesId();

        //void Remove(params T[] items);

        //void Remove(params int[] ids);

        // void RemoveAll();

        //void Update(T t);

       // IUnit<T> GetUnit();
    }
}
