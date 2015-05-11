namespace Xinchen.DbEntity
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    public class DbHelper : IDisposable
    {
        private IDbCommand _cmd;
        private IDbDataAdapter _dda;
        private DataSet _ds;
        private static DbHelper _instance;
        private object _lock = new object();
        private static object _newLock = new object();
        private static Dictionary<Type, DbType> _typeMapper = new Dictionary<Type, DbType>();
        protected DbConnection Conn = null;
        private string connString;

        public string ConnectionString
        {
            get { return connString; }
        }
        protected System.Data.Common.DbProviderFactory DbProviderFactory;
        private string providerName;

        public string ProviderName
        {
            get { return providerName; }
            private set { providerName = value; }
        }

        static DbHelper()
        {
            TypeMapper.Add(typeof(string), DbType.String);
            TypeMapper.Add(typeof(DateTime), DbType.DateTime);
            TypeMapper.Add(typeof(int), DbType.Int32);
            TypeMapper.Add(typeof(short), DbType.Int16);
            TypeMapper.Add(typeof(long), DbType.Int64);
            TypeMapper.Add(typeof(int?), DbType.Int32);
            TypeMapper.Add(typeof(DateTime?), DbType.DateTime);
            TypeMapper.Add(typeof(short?), DbType.Int16);
            TypeMapper.Add(typeof(long?), DbType.Int64);
            TypeMapper.Add(typeof(decimal), DbType.Decimal);
            TypeMapper.Add(typeof(decimal?), DbType.Decimal);
            TypeMapper.Add(typeof(bool), DbType.Boolean);
            TypeMapper.Add(typeof(bool?), DbType.Boolean);
        }

        private DbHelper(string connectionString, string providerName)
        {
            this.connString = connectionString;
            this.ProviderName = providerName;
            this.DbProviderFactory = DbProviderFactories.GetFactory(providerName);
        }

        public void Close()
        {
            if (this._ds != null)
            {
                try
                {
                    this._ds.Dispose();
                    this._ds = null;
                }
                catch
                {
                }
            }
            if (this.Conn != null)
            {
                try
                {
                    this.Conn.Dispose();
                    this.Conn = null;
                }
                catch
                {
                }
            }
            if (this._cmd != null)
            {
                try
                {
                    this._cmd.Dispose();
                    this._cmd = null;
                }
                catch
                {
                }
            }
        }

        public IDbDataParameter CreateParameter(DbType dbtype, string name, object value)
        {
            IDbDataParameter parameter = this.DbProviderFactory.CreateParameter();
            parameter.DbType = dbtype;
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        public IDbDataParameter CreateParameter(string name, DbType dbtype, object value)
        {
            IDbDataParameter parameter = this.DbProviderFactory.CreateParameter();
            parameter.DbType = dbtype;
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        public void Dispose()
        {
            this.Close();
        }

        public int ExecuteCount(string sql, IEnumerable<IDbDataParameter> dbDataParameter)
        {
            return this.ExecuteCount(sql, dbDataParameter.ToArray<IDbDataParameter>());
        }

        public int ExecuteCount(string sql, params IDbDataParameter[] dbParams)
        {
            IDbDataParameter item = this.CreateParameter(DbType.Int32, "@count", 1);
            item.Direction = ParameterDirection.Output;
            List<IDbDataParameter> dbDataParameter = dbParams.ToList<IDbDataParameter>();
            dbDataParameter.Add(item);
            this.ExecuteUpdate(sql + ";select @count=@@ROWCOUNT", dbDataParameter);
            return (int)item.Value;
        }

        public IDataReader ExecuteReader(string sql, params IDbDataParameter[] dbDataParameters)
        {
            lock (this._lock)
            {
                this._cmd = this.DbProviderFactory.CreateCommand();
                this._cmd.CommandText = sql;
                this.OpenConn();
                this._cmd.Connection = this.Conn;
                using (this.Conn)
                {
                    using (_cmd)
                    {
                        return _cmd.ExecuteReader();
                    }
                }
            }
        }

        public DataSet ExecuteQuery(string sql)
        {
            DataSet set;
            lock (this._lock)
            {
                this._cmd = this.DbProviderFactory.CreateCommand();
                this._cmd.CommandText = sql;
                this._dda = this.DbProviderFactory.CreateDataAdapter();
                this._dda.SelectCommand = this._cmd;
                this.OpenConn();
                this._cmd.Connection = this.Conn;
                using (this.Conn)
                {
                    this._ds = new DataSet();
                    try
                    {
                        this._dda.Fill(this._ds);
                        set = this._ds;
                    }
                    finally
                    {
                        this._cmd.Parameters.Clear();
                        this.Close();
                    }
                }
            }
            return set;
        }

        public DataSet ExecuteQuery(string sql, IEnumerable<IDbDataParameter> dbDataParameter)
        {
            DataSet set;
            lock (this._lock)
            {
                this._cmd = this.DbProviderFactory.CreateCommand();
                this._cmd.CommandText = sql;
                foreach (IDbDataParameter parameter in from sp in dbDataParameter
                                                       where sp != null
                                                       select sp)
                {
                    this._cmd.Parameters.Add(parameter);
                }
                this._dda = this.DbProviderFactory.CreateDataAdapter();
                this._dda.SelectCommand = this._cmd;
                this.OpenConn();
                this._cmd.Connection = this.Conn;
                using (this.Conn)
                {
                    this._ds = new DataSet();
                    try
                    {
                        this._dda.Fill(this._ds);
                        set = this._ds;
                    }
                    finally
                    {
                        this._cmd.Parameters.Clear();
                        this.Close();
                    }
                }
            }
            return set;
        }

        public DataSet ExecuteQuery(string sql, params IDbDataParameter[] dbParams)
        {
            return this.ExecuteQuery(sql, dbParams.ToList<IDbDataParameter>());
        }

        public object ExecuteScalar(string sql)
        {
            lock (this._lock)
            {
                this._cmd = this.DbProviderFactory.CreateCommand();
                this._cmd.CommandText = sql;
                this.OpenConn();
                this._cmd.Connection = this.Conn;
                using (this.Conn)
                {
                    try
                    {
                        return this._cmd.ExecuteScalar();
                    }
                    finally
                    {
                        this._cmd.Parameters.Clear();
                        this.Close();
                    }
                }
            }
        }

        public object ExecuteScalar(string sql, IEnumerable<IDbDataParameter> dbDataParameter)
        {
            lock (this._lock)
            {
                this._cmd = this.DbProviderFactory.CreateCommand();
                this._cmd.CommandText = sql;
                foreach (IDbDataParameter parameter in from sp in dbDataParameter
                                                       where sp != null
                                                       select sp)
                {
                    this._cmd.Parameters.Add(parameter);
                }
                this._dda = this.DbProviderFactory.CreateDataAdapter();
                this._dda.SelectCommand = this._cmd;
                this.OpenConn();
                this._cmd.Connection = this.Conn;
                using (this.Conn)
                {
                    try
                    {
                        return this._cmd.ExecuteScalar();
                    }
                    finally
                    {
                        this._cmd.Parameters.Clear();
                        this.Close();
                    }
                }
            }
        }

        public object ExecuteScalar(string sql, params IDbDataParameter[] dbParams)
        {
            return this.ExecuteScalar(sql, dbParams.ToList<IDbDataParameter>());
        }

        public int ExecuteScalarCount(string sql, params IDbDataParameter[] dbParams)
        {
            return Convert.ToInt32(this.ExecuteScalar(sql, dbParams.ToList<IDbDataParameter>()));
        }

        public int ExecuteUpdate(string sql)
        {
            lock (this._lock)
            {
                this._cmd = this.DbProviderFactory.CreateCommand();
                this._cmd.CommandText = sql;
                this.OpenConn();
                this._cmd.Connection = this.Conn;
                int num = 0;
                using (this.Conn)
                {
                    try
                    {
                        num = this._cmd.ExecuteNonQuery();
                    }
                    finally
                    {
                        this._cmd.Parameters.Clear();
                    }
                }
                this.Close();
                return num;
            }
        }

        public virtual int ExecuteUpdate(string sql, IEnumerable<IDbDataParameter> dbDataParameter)
        {
            lock (this._lock)
            {
                this._cmd = this.DbProviderFactory.CreateCommand();
                this._cmd.CommandText = sql;
                this.OpenConn();
                this._cmd.Connection = this.Conn;
                int num = 0;
                using (this.Conn)
                {
                    foreach (IDbDataParameter parameter in from sp in dbDataParameter
                                                           where sp != null
                                                           select sp)
                    {
                        this._cmd.Parameters.Add(parameter);
                    }
                    try
                    {
                        num = this._cmd.ExecuteNonQuery();
                    }
                    finally
                    {
                        this._cmd.Parameters.Clear();
                    }
                }
                this.Close();
                return num;
            }
        }

        public int ExecuteUpdate(string sql, params IDbDataParameter[] dbParams)
        {
            return this.ExecuteUpdate(sql, dbParams.ToList<IDbDataParameter>());
        }

        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <returns></returns>
        public DataSet ExecutePager(string sql, string sort, int page, int pageSize, out int recordCount, params IDbDataParameter[] parameters)
        {
            DataSet ds = new DataSet();
            string countSql = "select count(1) from(" + sql + ") ttt;";
            recordCount = ExecuteCount(countSql, parameters);
            string[] strArray = new string[] { "select * from (SELECT ROW_NUMBER() OVER ( ORDER BY ", sort, " ) indexer ,* from (", sql, ") t) tt where tt.indexer>", ((page - 1) * pageSize).ToString(), " and tt.indexer<=", (page * pageSize).ToString() };
            string str = string.Concat(strArray);
            return ExecuteQuery(str, parameters);
        }

        public static List<Column> GetColumns(string server, string dataBaseName, string tableName)
        {
            string selectCommandText = "SELECT  * ,\r\n                        ISNULL(( SELECT COUNT(1)\r\n                                 FROM   SYSCOLUMNS ,\r\n                                        SYSOBJECTS ,\r\n                                        SYSINDEXES ,\r\n                                        SYSINDEXKEYS\r\n                                 WHERE  SYSCOLUMNS.id = sc.id\r\n                                        AND SYSOBJECTS.xtype = 'PK'\r\n                                        AND SYSOBJECTS.parent_obj = SYSCOLUMNS.id\r\n                                        AND SYSINDEXES.id = SYSCOLUMNS.id\r\n                                        AND SYSOBJECTS.name = SYSINDEXES.name\r\n                                        AND SYSINDEXKEYS.id = SYSCOLUMNS.id\r\n                                        AND SYSINDEXKEYS.indid = SYSINDEXES.indid\r\n                                        AND SYSCOLUMNS.colid = SYSINDEXKEYS.colid\r\n                                        AND syscolumns.NAME = sc.name\r\n                               ), 0) iskey\r\n                FROM    syscolumns sc\r\n                WHERE   ( id = ( SELECT id\r\n                                 FROM   sysobjects\r\n                                 WHERE  ( id = OBJECT_ID('" + tableName + "') )\r\n                               ) )ORDER BY colid";
            SqlConnection selectConnection = new SqlConnection("data source=" + server + ";Initial Catalog=" + dataBaseName + ";Integrated Security=True");
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(selectCommandText, selectConnection);
            adapter.Fill(dataSet, "table");
            List<Column> list = new List<Column>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Column item = new Column
                {
                    IsNullable = Convert.ToBoolean(row["isnullable"])
                };
                switch (Convert.ToInt32(row["xtype"]))
                {
                    case 0x68:
                        item.DataType = typeof(bool);
                        item.DataTypeString = "bool";
                        break;

                    case 0x6a:
                        item.DataType = typeof(decimal);
                        item.DataTypeString = "decimal";
                        break;

                    case 0xa7:
                    case 0xe7:
                    case 0x23:
                        item.DataType = typeof(string);
                        item.DataTypeString = "string";
                        break;

                    case 0x38:
                        item.DataType = typeof(int);
                        item.DataTypeString = "int";
                        break;

                    case 0x3d:
                        item.DataType = typeof(DateTime);
                        item.DataTypeString = "DateTime";
                        break;

                    default:
                        throw new NotImplementedException("不支持指定类型：" + row["xtype"].ToString());
                }
                item.Length = Convert.ToInt32(row["length"]);
                item.Name = row["name"].ToString();
                item.IsAutoIncrement = Convert.ToBoolean(row["colstat"]);
                item.IsKey = Convert.ToBoolean(row["iskey"]);
                list.Add(item);
            }
            adapter.Dispose();
            dataSet.Dispose();
            selectConnection.Dispose();
            return list;
        }

        public static List<string> GetDataBases(string server)
        {
            SqlConnection selectConnection = new SqlConnection("data source=" + server + ";Integrated Security=True");
            string selectCommandText = "select name from sysdatabases where dbid>=5 order by dbid desc";
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(selectCommandText, selectConnection);
            adapter.Fill(dataSet, "database");
            List<string> list = new List<string>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                list.Add(row["name"].ToString());
            }
            adapter.Dispose();
            dataSet.Dispose();
            selectConnection.Dispose();
            return list;
        }

        public static DbHelper GetInstance(string connectionStringName)
        {
            if (_instance == null)
            {
                lock (_newLock)
                {
                    if (_instance == null)
                    {
                        ConnectionStringSettings settings;
                        string input = null;
                        string providerName = null;
                        List<string> paths = new List<string>();
                        //if (!string.IsNullOrEmpty(connectionStringName))
                        //{
                        settings = ConfigurationManager.ConnectionStrings[connectionStringName];
                        if (settings == null)
                        {
                            throw new KeyNotFoundException("未找到配置节点：" + connectionStringName);
                        }
                        input = settings.ConnectionString;
                        providerName = settings.ProviderName;
                        if (string.IsNullOrEmpty(providerName))
                        {
                            providerName = "System.Data.SqlClient";
                        }
                        else if (providerName == "System.Data.EntityClient")
                        {
                            providerName = "System.Data.SqlClient";
                            //throw new Exception(input);
                            //input = Regex.Match(input, "(data source.*?;)M").Groups[1].Value;
                        }
                        //}
                        //else if (ConfigurationManager.ConnectionStrings.Count > 1)
                        //{
                        //    paths.Add("连接字符串可能有两个以上的设置");
                        //    foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
                        //    {
                        //        paths.Add(item.Name);
                        //    }
                        //    settings = ConfigurationManager.ConnectionStrings[1];
                        //    input = settings.ConnectionString;
                        //    providerName = settings.ProviderName;
                        //}
                        //else if (ConfigurationManager.AppSettings.Count > 2)
                        //{
                        //    paths.Add("连接字符串可能有三个以上的设置");
                        //    providerName = ConfigurationManager.AppSettings["ProviderName"];
                        //    input = ConfigurationManager.AppSettings["ConnectionString"];
                        //}
                        //else
                        //{
                        //    if (ConfigurationManager.AppSettings.Count <= 0)
                        //    {
                        //        throw new KeyNotFoundException("未给定ConnectionStringName，未找到ConnectionString设置，未找到AppSetting设置，实例化EntityContext失败");
                        //    }
                        //    providerName = "System.Data.SqlClient";
                        //    input = ConfigurationManager.AppSettings["ConnectionString"];
                        //}
                        //if (string.IsNullOrEmpty(input))
                        //{
                        //    throw new KeyNotFoundException("未找到ConnectionString：" + string.Join(",", paths));
                        //}
                        //if (string.IsNullOrEmpty(providerName))
                        //{
                        //    throw new KeyNotFoundException("未找到providerName" + string.Join(",", paths));
                        //}
                        //if (string.IsNullOrEmpty(providerName))
                        //{
                        //    providerName = "System.Data.SqlClient";
                        //}
                        //if (providerName == "System.Data.EntityClient")
                        //{
                        //    providerName = "System.Data.SqlClient";
                        //    //throw new Exception(input);
                        //    //input = Regex.Match(input, "(data source.*?;)M").Groups[1].Value;
                        //}
                        _instance = new DbHelper(input, providerName);
                    }
                }
            }
            return _instance;
        }

        public static List<string> GetTables(string server, string database)
        {
            SqlConnection selectConnection = new SqlConnection("data source=" + server + ";Initial Catalog=" + database + ";Integrated Security=True");
            string selectCommandText = "select name from sysobjects where type='" + 'U' + "' and name<>'sysdiagrams'";
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(selectCommandText, selectConnection);
            adapter.Fill(dataSet, "table");
            List<string> list = new List<string>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                list.Add(row["name"].ToString());
            }
            adapter.Dispose();
            dataSet.Dispose();
            selectConnection.Dispose();
            return list;
        }

        private void OpenConn()
        {
            if (this.Conn == null)
            {
                this.Conn = this.DbProviderFactory.CreateConnection();
                this.Conn.ConnectionString = this.connString;
                this.Conn.Open();
            }
            else if (this.Conn.State != ConnectionState.Open)
            {
                this.Conn.Close();
                this.Conn.ConnectionString = this.connString;
                this.Conn.Open();
            }
        }

        public static Dictionary<Type, DbType> TypeMapper
        {
            get
            {
                return _typeMapper;
            }
        }
    }
}

