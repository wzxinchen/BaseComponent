using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace Xinchen.DbUtils
{
    public interface IUnit<T> 
        where T : class,new()
    {
        IQueryable<T> Query { get; }
        int GetSequenceId();
        T Add(T item);
        T AddAndSaveChanges(T item);
        IEnumerable<T> Add(IEnumerable<T> items);
        bool Any(Expression<Func<T, bool>> filter);
        //bool Any(Action<T> filter);
        ////T Get(Action<T> where);
        T Get(Expression<Func<T, bool>> filter);
        //T Get(Action<T> filter);
        //System.Collections.Generic.IDictionary<TKey, T> GetDictionary<TKey>(Func<T, TKey> keySelector);
        //System.Collections.Generic.IDictionary<TKey, T> GetDictionary<TKey>(string sql, Func<T, TKey> keySelector, FilterLinked filterLinked, Sort sort);
        //System.Collections.Generic.IDictionary<TKey, TModel> GetDictionary<TModel, TKey>(string sql, Func<TModel, TKey> keySelector);
        //System.Collections.Generic.IDictionary<TKey, TModel> GetDictionary<TModel, TKey>(string sql, Func<TModel, TKey> keySelector, FilterLinked filterLinked, Sort sort);
        //System.Collections.Generic.IList<T> GetList();
        IList<T> GetList(Expression<Func<T, bool>> filter);
        //System.Collections.Generic.IList<T> GetList(Action<T> filter);
        //System.Collections.Generic.IList<T> GetList(string sql);
        //System.Collections.Generic.IList<T> GetList(string sql, FilterLinked filterLinked, Sort sort);
        //System.Collections.Generic.IList<TModel> GetList<TModel>(string sql);
        //System.Collections.Generic.IList<TModel> GetList<TModel>(string sql, FilterLinked filterLinked, Sort sort);
        // int GetSequencesId();
        //PageResult<TModel> Page<TModel>(int page, int rows, string sql);
        PageResult<TResult> Page<TResult>(int page, int rows, Func<IQueryable<T>, IQueryable<TResult>> filter);
        //PageResult<TModel> Page<TModel>(int page, int rows, string sql, FilterLinked filterLinked, Sort sort);
        //void Remove(params T[] items);
        void Remove(Expression<Func<T, bool>> filter);
        //void RemoveAll();
        //void Update(Action<T> update, Action<T> where);
        void Update(Expression<Func<T, bool>> where, Expression<Func<T, T>> update);
        void Update(T t);
    }
}
