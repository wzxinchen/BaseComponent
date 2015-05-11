namespace Xinchen.PrivilegeManagement.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IEntityRepository<TEntity>:IDisposable
    {
        TEntity Add(TEntity entity);
        bool Exist(Expression<Func<TEntity,bool>> condition);
        TEntity Get(Expression<Func<TEntity, bool>> condition);
        //TEntity Get(int id);
        //IList<TEntity> GetList(Action<TEntity> condition);
        IList<TEntity> GetList(Func<IQueryable<TEntity>,IQueryable<TEntity>> condition);
       // int GetUniqueId();
       // bool IsEmpty();
        //void Remove(TEntity condition);
        void Remove(Expression<Func<TEntity,bool>> condition);
        void Update(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TEntity>> update);
        void Update(TEntity update);

        int SaveChanges();
    }
}

