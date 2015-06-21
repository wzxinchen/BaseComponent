using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public class SqlExecutor
    {
        static DbProviderFactory _factory;
        static Dictionary<Type, DbType> _typeMapper = new Dictionary<Type, DbType>();
        static SqlExecutor()
        {
            _typeMapper.Add(typeof(DateTime), DbType.DateTime2);
            _typeMapper.Add(typeof(int), DbType.Int32);
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
            _factory = DbProviderFactories.GetFactory(DataContext.DbFactoryName);
        }

        DbConnection CreateConnection()
        {
            var dbConnection = _factory.CreateConnection();
            dbConnection.ConnectionString = DataContext.ConnectionString;
            dbConnection.Open();
            return dbConnection;
        }

        DbCommand CreateCommand(DbConnection conn, string cmdText, Dictionary<string, object> parameters)
        {
            var dbCommand = _factory.CreateCommand();
            dbCommand.Connection = conn;
            dbCommand.CommandText = cmdText;
            foreach (var parameterName in parameters.Keys)
            {
                var parameter = dbCommand.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = parameters[parameterName];
                parameter.DbType = _typeMapper[parameter.Value.GetType()];
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

        public object ExecuteScalar(string sql, Dictionary<string, object> parameters)
        {
            var conn = CreateConnection();
            using (conn)
            {
                var cmd = CreateCommand(conn, sql, parameters);
                return cmd.ExecuteScalar();
            }
        }

        public DataSet ExecuteDataSet(string sql, Dictionary<string, object> parameters)
        {
            var conn = CreateConnection();
            using (conn)
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
        }
        public int ExecuteNonQuery(string sql, Dictionary<string, object> parameters)
        {
            var conn = CreateConnection();
            using (conn)
            {
                var cmd = CreateCommand(conn, sql, parameters);
                return cmd.ExecuteNonQuery();
            }
        }

    }
}
