namespace Xinchen.DbEntity
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using Xinchen.DynamicObject;
    using Xinchen.ObjectMapper;
    using Xinchen.Utils;
    using Xinchen.Utils.Entity;

    public class EntityContext
    {
        private Xinchen.DbEntity.DbHelper _helper;
        private Dictionary<Type, object> _entitySets;
        private System.Reflection.PropertyInfo[] _properties;

        public System.Reflection.PropertyInfo[] Properties
        {
            get { return _properties; }
        }
        public EntityContext(string connectionStringName)
        {
            _helper = Xinchen.DbEntity.DbHelper.GetInstance(connectionStringName);

            _properties = this.GetType().GetProperties();
            _entitySets = new Dictionary<Type, object>();
            foreach (var property in _properties.Where(x => x.PropertyType.IsGenericType && x.PropertyType.FullName.StartsWith("Xinchen.DbEntity.EntitySet")))
            {
                object entitySet = Activator.CreateInstance(property.PropertyType, connectionStringName);
                property.SetValue(this, entitySet, null);
                _entitySets.Add(property.PropertyType.GetGenericArguments()[0], entitySet);
            }

        }

        public EntitySet<T> EntitySet<T>()
        {
            return (EntitySet<T>)_entitySets[typeof(T)];
        }

        public IDbDataParameter CreateParameter<TValue>(string field, TValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            IDbDataParameter parameter = this._helper.CreateParameter(Xinchen.DbEntity.DbHelper.TypeMapper[value.GetType()], "@" + field, value);
            if (parameter.Value == null)
            {
                parameter.Value = DBNull.Value;
            }
            return parameter;
        }

        public T CreateProxy<T>()
        {
            return DynamicProxy.CreateDynamicProxy<T>();
        }

        public T CreateProxy<T>(T t)
        {
            return DynamicProxy.CreateDynamicProxy<T>(t);
        }

        public T GetModel<T>(string sql, params object[] parameters)
        {
            return GetModels<T>(sql, parameters).FirstOrDefault();
        }

        public IList<T> GetModels<T>(string sql, params object[] parameters)
        {
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            MatchCollection matches = Regex.Matches(sql, @"@([\w\d]*)");
            if (matches.Count != parameters.Length)
            {
                throw new ArgumentOutOfRangeException("传入的sql参数与sql语句中的参数数目不符");
            }
            int index = 1;
            foreach (var parameter in parameters)
            {
                dbParameters.Add(CreateParameter(matches[index].Groups[1].Value, parameter));
            }
            return Mapper.MapList<T>(_helper.ExecuteQuery(sql, dbParameters.ToArray()));
        }
        public System.Collections.Generic.IList<TModel> Page<TModel>(string sql, string sort, int page, int pageSize, out int recordCount, params object[] parameters)
        {
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            MatchCollection matches = Regex.Matches(sql, @"@([\w\d]*)");
            if (matches.Count != parameters.Length)
            {
                throw new ArgumentOutOfRangeException("传入的sql参数与sql语句中的参数数目不符");
            }
            int index = 0;
            foreach (var parameter in parameters)
            {
                dbParameters.Add(CreateParameter(matches[index++].Groups[1].Value, parameter));
            }
            return Mapper.MapList<TModel>(_helper.ExecutePager(sql, sort, page, pageSize, out recordCount, dbParameters.ToArray()));
        }

        public Dictionary<string, object> GetModifiedProperties(object o)
        {
            return DynamicProxy.GetModifiedProperties(o);
        }

        public Xinchen.DbEntity.DbHelper DbHelper
        {
            get
            {
                return this._helper;
            }
        }
    }
}

