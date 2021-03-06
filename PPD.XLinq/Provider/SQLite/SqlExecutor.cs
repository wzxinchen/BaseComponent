﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Xinchen.Utils;
namespace PPD.XLinq.Provider.SQLite
{
    public class SqlExecutor : SqlExecutorBase
    {
        protected override DbConnection CreateConnection()
        {
            if (DbConnection != null)
            {
                switch (DbConnection.State)
                {
                    case ConnectionState.Closed:
                        DbConnection.Open();
                        break;
                    case ConnectionState.Open:
                        break;
                    default:
                        throw new Exception();
                }
                return DbConnection;
            }
            return base.CreateConnection();
        }

        protected override void Close()
        {
            if (Transaction.Current != null)
            {
                Transaction.Current.TransactionCompleted -= Current_TransactionCompleted;
                Transaction.Current.TransactionCompleted += Current_TransactionCompleted;
            }
            else
            {
                DbConnection.Dispose();
            }
        }

        void Current_TransactionCompleted(object sender, TransactionEventArgs e)
        {
            DbConnection.Dispose();
        }

        protected override void SetParameter(DbParameter parameter, string parameterName, object value, DbType dbType)
        {
            parameter.ParameterName = parameterName;
            if (value != null)
            {
                parameter.DbType = dbType;
                var dateValue = value as DateTime?;
                if (dateValue != null)
                {
                    if (dateValue.Value.Date == dateValue.Value)
                    {
                        value = dateValue.Value.ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        value = dateValue.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    parameter.DbType = DbType.String;
                }
                parameter.Value = value;
            }
            else
            {
                parameter.Value = DBNull.Value;
            }
        }
    }
}
