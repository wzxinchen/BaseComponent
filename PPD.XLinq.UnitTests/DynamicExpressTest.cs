using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPD.XLinq.UnitTests.Model;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xinchen.DbUtils;
using System.Collections;
using System.Reflection;
namespace PPD.XLinq.UnitTests
{
    [TestClass]
    public class DynamicExpressTest
    {
        TestDataContext db = new TestDataContext();
        [TestMethod]
        public void ListContainsColumn()
        {
            var builder = new ExpressionBuilder<User>();
            var condition = builder.BuildContains("Id", new int[] { 9, 10 });
            var where = builder.Build(condition);
            var results = db.Set<User>().Where(where).ToList();
            Console.WriteLine(results.Count + "条数据");
        }
        [TestMethod]
        public void StringContainsString()
        {
            var builder = new ExpressionBuilder<User>();
            var condition = builder.BuildContains("Password", "123");
            var where = builder.Build(condition);
            var results = db.Set<User>().Where(where).ToList();
            Console.WriteLine(results.Count + "条数据");
        }

        [TestMethod]
        public void BuildFilters()
        {
            var builder = new ExpressionBuilder<User>();
            var filters = new List<SqlFilter>();
            filters.Add(SqlFilter.Create("Id", Operation.Equal, 1));
            filters.Add(SqlFilter.Create("LastLoginDate", Operation.GreaterThan, DateTime.Now));
            filters.Add(SqlFilter.Create("Username", Operation.Like, "aaaa"));
            filters.Add(SqlFilter.Create("Id", Operation.List, new int[] { 1, 2, 3 }));
            filters.Add(SqlFilter.Create("Password", Operation.NotEqual, "1"));
            filters.Add(SqlFilter.Create("Status", Operation.List, new int[] { 1 }));
            var where = builder.Build(filters, new Dictionary<string, string>());
            var results = db.Set<User>().Where(where).ToList();
            Console.WriteLine(results.Count + "条数据");
        }
    }
}
