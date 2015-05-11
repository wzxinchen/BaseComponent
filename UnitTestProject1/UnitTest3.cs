using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xinchen.XLinq;
using Xinchen.DbEntity;
using System.Linq;
using System.Data;
using System.Collections.Generic;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest3
    {
        private LinqEntitySet<TriggerDetail> db = new LinqEntitySet<TriggerDetail>();
        private PrivilegeTestEntities efdb = new PrivilegeTestEntities();
        EntitySet<TriggerDetail> user = new EntitySet<TriggerDetail>("test");
        [TestMethod]
        public void TestMethod1()
        {
            var query = db.Select(x => new
            {
                x.TriggerTemplete.Log.CustomerSysNo,
                x.TriggerTemplete,
                x.TrackingCode,
                A = x.TempleteSysNo,
            });//.Where(x => x.TriggerTemplete.ContentSql == "sql");
            Execute(query.ToString(), db.Parameters);
        }
        [TestMethod]
        public void TestMethod2()
        {
            var query = db.Select(x => new
            {
                x.TriggerTemplete.ContentResult
            });//.Where(x => x.TriggerTemplete.ContentSql == "sql");
            Execute(query.ToString(), db.Parameters);
        }

        [TestMethod]
        public void TestEF()
        {
            //var query = from online in efdb.Onlines
            //            join user in efdb.Users on online.UserId equals user.Id into ps
            //            from user in ps.DefaultIfEmpty()
            //            select new
            //            {
            //                B = user.Username
            //            };
            var query = from user in efdb.Users
                        from online in user.Onlines
                        select new
                        {
                            user.Name,
                            online.Status
                        };
            string sql = query.ToString();

        }

        void Execute(string sql, Dictionary<string, object> parameters)
        {
            List<IDbDataParameter> dbParameters = new List<IDbDataParameter>();
            foreach (var item in parameters)
            {
                dbParameters.Add(user.CreateParameter(item.Key, item.Value));
            }
            DbHelper.GetInstance("test").ExecuteQuery(sql, dbParameters.ToArray());
        }
    }
}
