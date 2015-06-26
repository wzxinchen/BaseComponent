using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xinchen.Utils;

namespace PPD.XLinq.Provider
{
    public class SqlExecutorBase
    {
        static DbProviderFactory _factory;
        static Dictionary<Type, DbType> _typeMapper = new Dictionary<Type, DbType>();
        private DbConnection _dbConnection;

        public DbConnection DbConnection
        {
            get { return _dbConnection; }
        }
        static SqlExecutorBase()
        {
            _typeMapper.Add(ReflectorConsts.DateTimeType, DbType.DateTime2);
            _typeMapper.Add(ReflectorConsts.Int32Type, DbType.Int32);
            _typeMapper.Add(typeof(short), DbType.Int16);
            _typeMapper.Add(typeof(long), DbType.Int64);
            _typeMapper.Add(typeof(string), DbType.String);
            _typeMapper.Add(typeof(DateTime?), DbType.DateTime2);
            _typeMapper.Add(typeof(int?), DbType.Int32);
            _typeMapper.Add(typeof(short?), DbType.Int16);
            _typeMapper.Add(typeof(long?), DbType.Int64);
            _typeMapper.Add(typeof(decimal), DbType.Currency);
            _typeMapper.Add(typeof(decimal?), DbType.Currency);
            _typeMapper.Add(typeof(double), DbType.Double);
            _typeMapper.Add(typeof(double?), DbType.Double);
            _typeMapper.Add(typeof(float), DbType.Single);
            _typeMapper.Add(typeof(float?), DbType.Single);
            _typeMapper.Add(typeof(bool), DbType.Boolean);
            _factory = DbProviderFactories.GetFactory(ConfigManager.DbFactoryName);
        }

        protected virtual DbConnection CreateConnection()
        {
            //if (dbConnection != null)
            //{
            //    switch (dbConnection.State)
            //    {
            //        case ConnectionState.Closed:
            //            dbConnection.Open();
            //            break;
            //        case ConnectionState.Open:
            //            break;
            //        default:
            //            throw new Exception();
            //    }
            //    return dbConnection;
            //}
            _dbConnection = _factory.CreateConnection();
            _dbConnection.ConnectionString = DataContext.ConnectionString;
            _dbConnection.Open();
            return _dbConnection;
        }

        protected virtual void Close()
        {
            if (_dbConnection != null)
            {
                _dbConnection.Dispose();
                _dbConnection = null;
            }
        }

        protected virtual void SetParameter(DbParameter parameter, string parameterName, object value, DbType dbType)
        {
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            parameter.DbType = dbType;
        }

        DbCommand CreateCommand(DbConnection conn, string cmdText, Dictionary<string, object> parameters)
        {
            var dbCommand = _factory.CreateCommand();
            dbCommand.Connection = conn;
            dbCommand.CommandText = cmdText;
            foreach (var parameterName in parameters.Keys)
            {
                var parameter = dbCommand.CreateParameter();
                var value = parameters.Get(parameterName);
                if (value != null)
                {
                    var propertyType = value.GetType();
                    DbType dbType;
                    if (propertyType.IsEnum)
                    {
                        dbType = DbType.Int32;
                    }
                    else
                    {
                        dbType = _typeMapper.Get(propertyType);
                    }
                    SetParameter(parameter, parameterName, value, dbType);
                }
                else
                {
                    parameter.ParameterName = parameterName;
                    parameter.Value = DBNull.Value;
                }
                dbCommand.Parameters.Add(parameter);
            }
            return dbCommand;
        }

        DbDataAdapter CreateDataAdapter(DbCommand cmd)
        {
            var dbDataAdapter = _factory.CreateDataAdapter();
            dbDataAdapter.SelectCommand = cmd;
            return dbDataAdapter;
        }

        public virtual object ExecuteScalar(string sql, Dictionary<string, object> parameters)
        {
            var conn = CreateConnection();
            try
            {
                var cmd = CreateCommand(conn, sql, parameters);
                return cmd.ExecuteScalar();
            }
            finally
            {
                Close();
            }
        }

        public DataSet ExecuteDataSet(string sql, Dictionary<string, object> parameters)
        {
            var conn = CreateConnection();
            try
            {
                var cmd = CreateCommand(conn, sql, parameters);
                using (cmd)
                {
                    var dda = CreateDataAdapter(cmd);
                    using (dda)
                    {
                        DataSet ds = new DataSet();
                        dda.Fill(ds);
                        return ds;
                    }
                }
            }
            finally
            {
                Close();
            }
        }
        public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters)
        {
            var conn = CreateConnection();
            try
            {
                var cmd = CreateCommand(conn, sql, parameters);
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                Close();
            }
        }
    }
}
