using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;

namespace Xinchen.DbUtils
{
    /// <summary>
    /// BLL基础功能类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public abstract class BaseBLL<T> where T : class, new()
    //{
    //    protected BaseBLL()
    //    {
    //    }

    //    public virtual T Add(T item)
    //    {
    //        return this.DAL.Add(item);
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
    //        return this.DAL.Any();
    //    }

    //    public bool ExecuteTransaction(System.Func<TransactionScope, BaseBLL<T>, bool> trans)
    //    {
    //        bool flag = false;
    //        using (TransactionScope scope = new TransactionScope())
    //        {
    //            flag = trans(scope, (BaseBLL<T>)this);
    //            if (flag)
    //            {
    //                scope.Complete();
    //            }
    //        }
    //        return flag;
    //    }

    //    public T Get(Action<T> where)
    //    {
    //        return this.DAL.Get(where);
    //    }

    //    public T Get(int id)
    //    {
    //        return this.DAL.Get(id);
    //    }

    //    public IDictionary<TKey, T> GetDictionary<TKey>(Func<T, TKey> keySelector)
    //    {
    //        return this.DAL.GetDictionary<TKey>(keySelector);
    //    }

    //    public IList<T> GetList()
    //    {
    //        return this.DAL.GetList();
    //    }

    //    public IList<T> GetList(Action<T> filter)
    //    {
    //        return this.DAL.GetList(filter);
    //    }

    //    public int GetScalarCount(Action<T> where)
    //    {
    //        return this.DAL.GetScaclrCount(where);
    //    }

    //    public int GetSequencesId()
    //    {
    //        return this.DAL.GetSequencesId();
    //    }

    //    public virtual void Remove(params T[] items)
    //    {
    //        this.DAL.Remove(items);
    //    }

    //    public virtual void Remove(Action<T> where)
    //    {
    //        this.DAL.Remove(where);
    //    }

    //    public virtual void Remove(params int[] ids)
    //    {
    //        this.DAL.Remove(ids);
    //    }

    //    public virtual void RemoveAll()
    //    {
    //        this.DAL.RemoveAll();
    //    }

    //    public void Update(T t)
    //    {
    //        this.DAL.Update(t);
    //    }

    //    public void Update(Action<T> where, Action<T> update)
    //    {
    //        this.DAL.Update(update, where);
    //    }

    //    public abstract BaseDAL<T> DAL { get; }
    //}

}
