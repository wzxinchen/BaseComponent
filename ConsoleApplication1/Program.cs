using PPD.XLinq;
using PPD.XLinq.Provider;
using PPD.XLinq.Provider.SqlServer2008R2;
using PPD.XLinq.UnitTests.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigManager.DbFactoryName = "System.Data.SQLite";
            ConfigManager.DataBase = "SQLite";
            //TestDataContext db = new TestDataContext();
            //db.Set<User>();
            //var provider = ProviderFactory.CreateProvider(ConfigManager.DataBaseType);
            //var op = provider.CreateEntityOperator();
            //var list = new ArrayList();
            var scope = new TransactionScope();
            //list.Add(new User() { });
            //op.InsertEntities(list);
            var _sqlExecutor = new SqlExecutor();
            var obj = _sqlExecutor.ExecuteScalar(string.Format("select max(Count) from Seqs where Name='User'"), new Dictionary<string, object>());
            obj = _sqlExecutor.ExecuteNonQuery(string.Format("insert into Seqs(Name,Count) values('User',1)"), new Dictionary<string, object>());
            scope.Complete();
            scope.Dispose();
            Console.ReadKey();
        }
    }
    public enum T
    {
        A = 1, B = 1
    }
}
