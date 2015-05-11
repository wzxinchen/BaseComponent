using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xinchen.DbEntity;
using Xinchen.PrivilegeManagement;
using Xinchen.Utils.DataAnnotations;
using Xinchen.Utils.Entity;

namespace UnitTestProject1
{
    public class TestT<T>
    {
        private static object _syncRoot = new object();
        private static TestT<T> _instance;
        public static TestT<T> GetInstance()
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new TestT<T>();
                    }
                }
            }

            return _instance;
        }

        private TestT()
        {
        }
    }
    public class T
    {
        public virtual DateTime AA { get; set; }
        public virtual string BB { get; set; }
    }
    [TestClass]
    public class UnitTest1 : T
    {
        private DateTime aa;
        private string bb;
        private Dictionary<string, object> dict = new Dictionary<string, object>();

        public override DateTime AA
        {
            get
            {
                return aa;
            }
            set
            {
                aa = value;
                dict.Remove("AA");
                dict.Add("AA", value);
                base.AA = value;
            }
        }

        public override string BB
        {
            get
            {
                return bb;
            }
            set
            {
                bb = value;
                dict.Remove("BB");
                dict.Add("BB", value);
            }
        }

        [TestMethod]
        public void EntityFramework()
        {
            //PrivilegeTestEntities db = new PrivilegeTestEntities();
            //var groups = new Group();
            //db.Groups.Add(groups);
            //db.SaveChanges();
        }



        [TestMethod]
        public void DbEntityCreate()
        {
            //DepartmentProvider.GetInstance().Add(new Xinchen.PrivilegeManagement.Model.Department()
            //{
            //    Id = 1,
            //    Description = "a",
            //    Name = "b"
            //});
            //var tt=(TestConfig)System.Configuration.ConfigurationManager.GetSection("privilegeProvider");

            //TestT<string>.GetInstance();
            //TestT<int>.GetInstance();
            EntitySet<Group> test = new EntitySet<Group>();
            //var group = new Group();
            var group=test.GetById(0);
            group.Name = "asdf";
            test.Update(group);
            group.Del = true;
            { }
            //var t=test.CreateProxy(group);
            //test.Create(new Group());
            //var ds=test.DbHelper.ExecuteQuery("select * from dbo.Groups");
            //{
            //}
            //var menu = new Group()
            //{
            //    Name = "asdf",
            //    Price = 1,
            //    Privileges = 1,
            //    Remain = 1,
            //    Time = DateTime.Now,
            //    Total = 1,
            //    UpdateTime = DateTime.Now,
            //    Description = "aaaa"
            //};
            //menu=test.Create(menu);
            //var group = test.GetById(1);
            //group.Del = true;
            ////group.DelTime = DateTime.Now;
            //test.Update(group);
            //test.DeleteBy(x =>
            //{
            //    x.Del = true;
            //});
            //menu.Name = "aa";
            //menu.Name = "bb";
            //for (int i = 0; i < 1000000; i++)
            //{
            //    test.Create(menu);
            //}
        }

        [TestMethod]
        public void DbEntitySelect()
        {
            EntitySet<Group> test = new EntitySet<Group>();
            var menu = new Group()
            {
                Name = "asdf",
                Price = 1,
                Privileges = 1,
                Remain = 1,
                Time = DateTime.Now,
                Total = 1,
                UpdateTime = DateTime.Now,
                Description = "aaaa"
            };
            var ts = test.GetAll();
            ts = null;
        }

        [TestMethod]
        public void 原生Create()
        {
            DbHelper helper = DbHelper.GetInstance();
            for (int i = 0; i < 1000000; i++)
            {
                var p1 = helper.CreateParameter("@Name", DbType.String, "asdf");
                var p2 = helper.CreateParameter("@Price", DbType.String, 1);
                var p3 = helper.CreateParameter("@Privileges", DbType.String, 1);
                var p4 = helper.CreateParameter("@Remain", DbType.String, 1);
                var p5 = helper.CreateParameter("@Time", DbType.String, DateTime.Now.ToString());
                var p6 = helper.CreateParameter("@Total", DbType.String, 1);
                var p7 = helper.CreateParameter("@UpdateTime", DbType.String, DateTime.Now.ToString());
                var p8 = helper.CreateParameter("@Description", DbType.String, "asdf");
                var p9 = helper.CreateParameter("@DelTime", DbType.DateTime, DBNull.Value); DbHelper.GetInstance().ExecuteUpdate("insert into groups (Name,Price,Privileges,Remain,Time,Total,UpdateTime,Description) values(@Name,@Price,@Privileges,@Remain,@Time,@Total,@UpdateTime,@Description)", p1, p2, p3, p4, p5, p6, p7, p8, p9);
            }

        }
        [TestMethod]
        public void 原生Select()
        {
            var helper = DbHelper.GetInstance();
            DataSet ds = helper.ExecuteQuery("select * from dbo.groups");
            List<Group> groups = new List<Group>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Group group = new Group();
                group.Description = row["Description"].ToString();
                group.Name = row["Name"].ToString();
                group.Price = Convert.ToDecimal(row["Price"]);
                group.Privileges = Convert.ToInt32(row["Privileges"]);
                group.Remain = Convert.ToInt32(row["Remain"]);
                group.Time = Convert.ToDateTime(row["Time"]);
                group.Total = Convert.ToInt32(row["Total"]);
                group.UpdateTime = Convert.ToDateTime(row["UpdateTime"]);
                groups.Add(group);
            }
            groups = null;
        }

        [TestMethod]
        public void Test()
        {
            new Dictionary<string, string>().Remove("a");
            var test = new EntitySet<Group>();
            //test.DeleteById(601522, 601520, 601521);
            var t = test.CreateProxy();
            test.GetBy("select top 10 * from groups order by id");
            //t = test.GetById(601523);
            //test.GetBy(x => x.Name = "asdf");
            //t.Name = "aa";
            //test.DeleteBy(x =>
            //{
            //    x.Name = t.Name;
            //});
            // test.DeleteByWhere(t);
        }

        //[TestMethod]
        //public void TestMethod1()
        //{
        //    UnitTest1 t = new UnitTest1();
        //    t.AA = "a";
        //    for (int i = 0; i < 1000000; i++)
        //    {
        //        var s = t.AA;
        //        //new UnitTest1();
        //    }
        //    ////var t = new UnitTest1();
        //    ////t.AA = null;
        //    //var t = DynamicProxyGenerator.CreateDynamicProxy<UnitTest1>(true);
        //    ////return;
        //    //t.AA = "111";
        //    //DynamicProxyGenerator.GetModifiedProperties(t);
        //}
        [Table("Groups")]
        public class Group
        {
            [Key]
            public virtual int Id { get; set; }
            public virtual string Name { get; set; }
            public virtual int Privileges { get; set; }
            public virtual DateTime Time { get; set; }
            public virtual decimal Price { get; set; }
            public virtual string Description { get; set; }
            public virtual DateTime UpdateTime { get; set; }
            public virtual int Remain { get; set; }
            public virtual int Total { get; set; }
            public virtual DateTime? DelTime { get; set; }
            public virtual bool Del { get; set; }
        }
    }
}
