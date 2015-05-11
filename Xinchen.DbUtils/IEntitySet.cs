namespace Xinchen.DbUtils
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.InteropServices;

    public interface IEntitySet<TEntity>
    {
        int Count(string sql, params IDbDataParameter[] dbParameters);
        TEntity Create(TEntity entity);
        //IDbDataParameter CreateParameter(string field, object value);
        TEntity CreateProxy();
        void Delete(TEntity group);
        void DeleteAll();
        void DeleteBy(Action<TEntity> where);
        void DeleteBy(string where);
        void DeleteBy(Dictionary<string, object> dict);
        void DeleteById(params int[] id);
        //bool Exists(string sql);
        bool Exists(string sql, params IDbDataParameter[] dbParameters);
        bool Exists(int id);
        //bool Exists(string where);
        bool Exists(Action<TEntity> where);
        //bool Exists(string where, params IDbDataParameter[] dbParameters);
        IList<TEntity> GetList();
        IList<TModel> GetList<TModel>(string sql);
        IList<TEntity> GetList(Action<TEntity> where);
        IList<TEntity> GetList(Dictionary<string, object> modifiedProperties);
        //IList<TEntity> GetList(string sql, params IDbDataParameter[] dbParameters);
        //IList<TModel> GetIList<TModel>(string sql, params IDbDataParameter[] dbParameters) where TModel : class, new();
        IList<TEntity> GetList(string sql, FilterLinked filterLinked = null, Sort sort = null);
        IList<TModel> GetList<TModel>(string sql, FilterLinked filterLinked = null, Sort sort = null);
        IDictionary<TKey, TModel> GetDictionary<TModel, TKey>(string sql, Func<TModel, TKey> keySelector, FilterLinked filterLinked = null, Sort sort = null);
        //IList<TEntity> GetList(string fields, string strWhere, string strOrder = null, params IDbDataParameter[] dbParameters);
        //IList<TEntity> GetList(string fields, string strWhere, string strOrder = null, string strGroup = null);
        //IList<TEntity> GetBy(string fields, Action<TEntity> where, string strOrder = null, string strGroup = null);
        TEntity GetSingle(int id);
        TEntity GetSingle(Action<TEntity> condition);
        IDictionary<TKey, TEntity> GetDictionary<TKey>(Func<TEntity, TKey> keySelector);
        IDictionary<TKey, TEntity> GetDictionaryBy<TKey>(string sql, Func<TEntity, TKey> keySelector, FilterLinked filterLinked = null, Sort sort = null);
        //Dictionary<TKey, TEntity> GetDictionaryBy<TKey>(string sql, Func<TEntity, TKey> keySelector, FilterLinked filterLinked = null, Sort sort = null);
        IDictionary<TKey, TModel> GetDictionaryBy<TModel, TKey>(string sql, Func<TModel, TKey> keySelector, FilterLinked filterLinked = null, Sort sort = null);
        //Dictionary<TKey, TEntity> GetDictionaryBy<TKey>(string fields, string strWhere, Func<TEntity, TKey> keySelector, string strOrder = null, string strGroup = null);
        TFieldType GetMax<TFieldType>(string field);
        int GetSequenceId();
        bool IsEmpty();
        PageResult<TModel> Page<TModel>(int page, int rows, string sql, FilterLinked filterLinked = null, Sort sort = null);
        int ScalarCount(Action<TEntity> where);
        void Update(TEntity entity);
        //void Update(string sql, params IDbDataParameter[] dbParameters);
        void Update(Action<TEntity> where, Action<TEntity> update);
    }
}
