namespace Xinchen.DbUtils
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// DAL基础功能类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public abstract class BaseDAL<T> : Xinchen.DbUtils.IDAL<T> where T : class, new()
    //{
    //    protected BaseDAL()
    //    {
    //    }

    //    public virtual T Add(T item)
    //    {
    //        return this.entitySet.Create(item);
    //    }

    //    public virtual void Add(T[] items)
    //    {
    //        foreach (T local in items)
    //        {
    //            this.Add(local);
    //        }
    //    }

    //    public bool Any()
    //    {
    //        return !this.entitySet.IsEmpty();
    //    }

    //    public T Get(int id)
    //    {
    //        return this.entitySet.GetSingle(id);
    //    }

    //    public T Get(Action<T> where)
    //    {
    //        return this.entitySet.GetList(where).FirstOrDefault<T>();
    //    }

    //    public IDictionary<TKey, T> GetDictionary<TKey>(Func<T, TKey> keySelector)
    //    {
    //        return this.entitySet.GetDictionary<TKey>(keySelector);
    //    }

    //    public IDictionary<TKey, T> GetDictionary<TKey>(string sql, Func<T, TKey> keySelector, FilterLinked filterLinked = null, Sort sort = null)
    //    {
    //        return this.entitySet.GetDictionaryBy<TKey>(sql, keySelector, filterLinked,sort);
    //    }

    //    public IDictionary<TKey, TModel> GetDictionary<TModel, TKey>(string sql, Func<TModel, TKey> keySelector, FilterLinked filterLinked = null, Sort sort = null) where TModel : class, new()
    //    {
    //        return this.entitySet.GetDictionary<TModel, TKey>(sql, keySelector, filterLinked,sort);
    //    }

    //    public IList<T> GetList()
    //    {
    //        return this.entitySet.GetList();
    //    }

    //    public IList<T> GetList(Action<T> filter)
    //    {
    //        return this.entitySet.GetList(filter);
    //    }

    //    //public IList<T> GetList(string sql, params IDbDataParameter[] dbParams)
    //    //{
    //    //    return this.entitySet.GetList(sql, dbParams);
    //    //}

    //    public IList<T> GetList(string sql,FilterLinked filterLinked=null,Sort sort=null)
    //    {
    //        return this.entitySet.GetList(sql,filterLinked,sort);
    //    }

    //    public IList<TModel> GetList<TModel>(string sql, FilterLinked filterLinked = null, Sort sort = null)
    //    {
    //        return this.entitySet.GetList<TModel>(sql, filterLinked,sort);
    //    }

    //    internal int GetScaclrCount(Action<T> where)
    //    {
    //        return this.entitySet.ScalarCount(where);
    //    }

    //    public int GetSequencesId()
    //    {
    //        return this.entitySet.GetSequenceId();
    //    }
    //    public PageResult<TModel> Page<TModel>(int page, int rows, string sql, FilterLinked filterLinked = null, Sort sort = null) where TModel : class, new()
    //    {
    //        return this.entitySet.Page<TModel>(page, rows, sql, filterLinked,sort);
    //    }
    //    //public PageResult<TModel> Page<TModel>(int page, int rows, string sql, DynamicSqlSorter defaultSorter, params DynamicSqlParam[] sqlParams) where TModel : class, new()
    //    //{
    //    //    List<DynamicSqlParam> sqlParas = new List<DynamicSqlParam>();
    //    //    sqlParas.AddRange(sqlParams);
    //    //    if (!sqlParams.Any(x => x.Type == DynamicSqlParamType.Sort))
    //    //    {
    //    //        sqlParas.Add(defaultSorter);
    //    //    }
    //    //    return this.entitySet.Page<TModel>(page, rows, sql, sqlParas.ToArray());
    //    //}

    //    //public PageResult<T> Page(int page, int rows, string sort, string sql, params IDbDataParameter[] dbParameters)
    //    //{
    //    //    int num = this.entitySet.Count(sql, dbParameters);
    //    //    PageResult<T> result = new PageResult<T>
    //    //    {
    //    //        RecordCount = num,
    //    //        Page = page,
    //    //        PageCount = (int)Math.Ceiling((double)(((double)num) / ((double)rows)))
    //    //    };
    //    //    string str = Regex.Replace(sql, "^select?", (MatchEvaluator)(x => ("SELECT ROW_NUMBER() OVER(ORDER BY " + sort + ") _index,")));
    //    //    object[] objArray = new object[] { "select * from (", str, ") pagerTmp where pagerTmp between ", ((page - 1) * rows).ToString(), " and ", page * rows };
    //    //    str = string.Concat(objArray);
    //    //    result.Data = this.entitySet.GetList(str, dbParameters);
    //    //    return result;
    //    //}

    //    public virtual void Remove(params T[] items)
    //    {
    //        foreach (T local in items)
    //        {
    //            this.entitySet.Delete(local);
    //        }
    //    }

    //    public virtual void Remove(Action<T> where)
    //    {
    //        this.entitySet.DeleteBy(where);
    //    }

    //    public virtual void Remove(params int[] ids)
    //    {
    //        this.entitySet.DeleteById(ids);
    //    }

    //    public virtual void RemoveAll()
    //    {
    //        this.entitySet.DeleteAll();
    //    }

    //    public void Update(T t)
    //    {
    //        this.entitySet.Update(t);
    //    }

    //    public void Update(Action<T> update, Action<T> where)
    //    {
    //        this.entitySet.Update(where, update);
    //    }

    //    protected abstract IEntitySet<T> entitySet { get; }
    //}
}
