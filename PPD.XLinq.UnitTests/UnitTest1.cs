using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPD.XLinq.UnitTests.Model;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PPD.XLinq;
namespace PPD.XLinq.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        TestDataContext db = new TestDataContext();
        public DateTime Test
        {
            get
            {
                return DateTime.Now.Date;
            }
        }
        [TestMethod]
        public void TestMethod1()
        {
            var list = db.Users.Where(x => x.Password == "06ea391fdd9abd6ac43cc858dcc7c4ce" && x.Username == "awfer1").ToList();

            //string aa = "呵呵";
            //var a=aa.Length;
            //TestDataContext db = new TestDataContext();
            //var date = DateTime.Now;
            //UnitTest1 t = new UnitTest1();
            //db.Users.Select(x => new
            //{
            //    Reg = x.RegTime.Date,
            //    x.RegTime.Date,
            //    x.Id,
            //    x.Age
            //}).Where(x => x.Reg.Date == DateTime.Now.Date && x.Id == 1 && x.Age == 1).ToList();
            //db.Users.Select(x => x).Where(x => x.RegTime.Date == DateTime.Now.Date && x.Id == 1 && x.Age == 1).ToList();
            //var query = from user in db.Users select user;
        }
        [TestMethod]
        public void TestMethod2()
        {
            var list = db.Users.Where(x => x.LastLoginDate.Value.Date == DateTime.Now.Date).Select(x => new { x.Username }).ToList();
        }
        [TestMethod]
        public void Select中取Date()
        {
            var a = true;
            var query = from user in db.Users
                        where user.Password == "xxxx" && user.UserId == 1
                        select new
                        {
                            Id = user.UserId,
                            user.Password,
                            user.LastLoginDate.Value.Date
                        };
            query.ToList();
        }

        [TestMethod]
        public void Where方法调用()
        {
            var query = from user in db.Users
                        where (user.Password.Substring(0, 1) == "aaaa" ||
                        user.LastLoginDate.Value.Date == Convert.ToDateTime(DateTime.Now.ToString()).Date.Date &&
                        user.Password == "xxxx") && user.UserId == 1
                        select new
                        {
                            Id = user.UserId,
                            user.Password,
                            user.LastLoginDate.Value.Date
                        };
            query.ToList();
        }
        [TestMethod]
        public void Where方法调用1()
        {
            var query = from user in db.Users
                        where (user.LastLoginDate.Value.Date == Convert.ToDateTime("2015-1-1").Date.Date && user.Password == "xxxx") && user.UserId == 1
                        select new
                        {
                            Id = user.UserId,
                            user.Password,
                            user.LastLoginDate.Value.Date
                        };
            query.ToList();
        }

        [TestMethod]
        public void SimpleAliasSelect()
        {
            var query = from user in db.Users
                        where user.Password == "xxx"
                        select new
                        {
                            Id = user.UserId,
                            user.Password,
                            user.LastLoginDate
                        };
            query.ToList();
        }
        [TestMethod]
        public void SimpleNoAliasSelect()
        {
            var query = from user in db.Users
                        where user.Password == "xxx"
                        select new
                        {
                            user.UserId,
                            user.Password
                        };
            query.ToList();
        }
        [TestMethod]
        public void Select不取Date()
        {
            var query = from user in db.Users
                        where user.Password == "xxx"
                        select new
                        {
                            Id = user.UserId,
                            user.Password,
                            user.LastLoginDate
                        };
            query.ToList();
        }
        [TestMethod]
        public void 没有Select()
        {
            var query = db.Users.Where(x => x.Password == "").ToList();
            query.ToList();
        }
        [TestMethod]
        public void WhereEqualNull()
        {
            var query = db.Users.Where(x => x.LastLoginDate == null).ToList();
        }

        [TestMethod]
        public void 复杂条件查询()
        {
            var query = db.Users.Where(x => x.Password == "xx" && (x.UserId == 111 || x.Username == "xxxxx") && x.LastLoginDate == null).ToList();
        }

        [TestMethod]
        public void 复杂条件查询1()
        {
            var query = (from x in db.Users where x.Password == "xx" && (x.UserId == 111 || x.Username == "xxxxx") && x.LastLoginDate == null select x).ToList();
        }


        [TestMethod]
        public void 直接ToList()
        {
            db.Users.ToList();
        }
        [TestMethod]
        public void 直接ToList1()
        {
            db.Users.Select(x => new { x.LastLoginDate }).ToList();
        }

        [TestMethod]
        public void 多重Where()
        {
            var query = db.Users.Where(x => x.UserId == 1);
            query = query.Where(x => x.Username == "xxx");
            query.ToList();
        }

        [TestMethod]
        public void JoinNoWhere()
        {
            var query = from user in db.Users
                        join order in db.TransferOrders on user.UserId equals order.ToUserId
                        join flow in db.TransferWorkFlows on order.ToUserId equals flow.UploadUserId
                        select new
                        {
                            user.UserId,
                            flow.UploadFileName,
                            order.ToUsername
                        };
            query.ToList();
        }
        [TestMethod]
        public void JoinWhere()
        {
            var query = from user in db.Users
                        join order in db.TransferOrders on user.UserId equals order.ToUserId
                        join flow in db.TransferWorkFlows on order.ToUserId equals flow.UploadUserId
                        where flow.UploadUserId == 1
                        select new
                        {
                            user.UserId,
                            flow.UploadFileName,
                            order.ToUsername
                        };
            query.ToList();
        }
        [TestMethod]
        public void JoinWhereDate()
        {
            var query = from user in db.Users
                        join order in db.TransferOrders on user.UserId equals order.ToUserId
                        join flow in db.TransferWorkFlows on order.ToUserId equals flow.UploadUserId
                        where flow.UploadUserId == 1
                        select new
                        {
                            user.UserId,
                            flow.UploadFileName,
                            order.ToUsername,
                            user.LastLoginDate.Value.Date
                        };
            query.ToList();
        }

        [TestMethod]
        public void ColumnStartsWithString()
        {
            db.Users.Where(x => x.Password.StartsWith("xxxxx")).ToList();
        }

        [TestMethod]
        public void StringStartsWithColumn()
        {
            db.Users.Where(x => "xxxxx".StartsWith(x.Password)).ToList();
        }

        [TestMethod]
        public void StringStartsWithString()
        {
            db.Users.Where(x => "xxxxx".StartsWith("xxxxxxxxxxxx")).ToList();
        }

        [TestMethod]
        public void ColumnContainsString()
        {
            db.Users.Where(x => x.Password.Contains("xxxxxxxxxxxx")).ToList();
        }
        [TestMethod]
        public void ColumnNotContainsString()
        {
            db.Users.Where(x => !x.Password.Contains("xxxxxxxxxxxx")).ToList();
        }

        [TestMethod]
        public void StringContainsColumn()
        {
            db.Users.Where(x => "xxxxxx".Contains(x.Password)).ToList();
        }

        [TestMethod]
        public void StringNotContainsColumn()
        {
            db.Users.Where(x => !"xxxxxx".Contains(x.Password)).ToList();
        }
        [TestMethod]
        public void ListContainsColumn()
        {
            var list = new int[] { 1, 2 };
            db.Users.Where(x => list.Contains(x.UserId)).ToList();
        }
        [TestMethod]
        public void ListNotContainsColumn()
        {
            var list = new int[] { 1, 2 };
            db.Users.Where(x => !list.Contains(x.UserId)).ToList();
        }
        [TestMethod]
        public void ListContainsNumber()
        {
            var list = new int[] { 1, 2 };
            db.Users.Where(x => list.Contains(1)).ToList();
        }
        [TestMethod]
        public void ListNotContainsNumber()
        {
            var list = new int[] { 1, 2 };
            db.Users.Where(x => !list.Contains(1)).ToList();
        }
        [TestMethod]
        public void Or()
        {
            var list = new int[] { 1, 2 };
            db.Users.Where(x => list == null || list.Contains(x.UserId)).ToList();
        }
        [TestMethod]
        public void OrNull()
        {
            var list = new int[] { 1, 2 };
            list = null;
            db.Users.Where(x => list == null || list.Contains(x.UserId)).ToList();
        }
        [TestMethod]
        public void AndNull()
        {
            var list = new object();
            list = null;
            db.Users.Where(x => list == null && x.Password == "xxxx").ToList();
        }

        [TestMethod]
        public void LeftJoinNoWhere()
        {
            var query = from user in db.Users
                        join order in db.TransferOrders on user.UserId equals order.ToUserId into us
                        from u in us.DefaultIfEmpty()
                        join flow in db.TransferWorkFlows on u.ToUserId equals flow.UploadUserId
                        join user1 in db.Users on user.Password equals user1.Username into test
                        from t in test.DefaultIfEmpty()
                        select new
                        {
                            t.Username,
                            u.ToUserId,
                            user.UserId,
                            user.LastLoginDate.Value.Date,
                            flow.UploadFileName
                        };
            query.ToList();
        }

        [TestMethod]
        public void LeftSelfJoin()
        {
            var query1 = from user in db.Users
                         join user1 in db.Users on user.Password equals user1.Username into test
                         from t in test.DefaultIfEmpty()
                         select new
                         {
                             t.Username,
                             user.UserId,
                             user.LastLoginDate.Value.Date
                         };
            query1.ToList();
        }
        [TestMethod]
        public void SelfJoin()
        {
            var query1 = from user in db.Users
                         join user1 in db.Users on user.Password equals user1.Username
                         select new
                         {
                             user1.Username,
                             user.UserId,
                             user.LastLoginDate.Value.Date
                         };
            query1.ToList();
        }
        [TestMethod]
        public void LeftJoinWhere()
        {
            var query = from user in db.Users
                        join order in db.TransferOrders on user.UserId equals order.ToUserId into us
                        from u in us.DefaultIfEmpty()
                        join flow in db.TransferWorkFlows on u.ToUserId equals flow.UploadUserId
                        join user1 in db.Users on user.Password equals user1.Username into test
                        from t in test.DefaultIfEmpty()
                        where flow.UploadUserId == 1 && t.Username == "xxxx"
                        select new
                        {
                            t.Username,
                            user.UserId,
                            user.LastLoginDate.Value.Date,
                            flow.UploadFileName
                        };
            query.ToList();
        }
        [TestMethod]
        public void Distinct()
        {
            var query = (from user in db.Users
                         join order in db.TransferOrders on user.UserId equals order.ToUserId into us
                         from u in us.DefaultIfEmpty()
                         join flow in db.TransferWorkFlows on u.ToUserId equals flow.UploadUserId
                         join user1 in db.Users on user.Password equals user1.Username into test
                         from t in test.DefaultIfEmpty()
                         where flow.UploadUserId == 1 && t.Username == "xxxx"
                         select new
                         {
                             t.Username,
                             user.UserId,
                             user.LastLoginDate.Value.Date,
                             flow.UploadFileName
                         }).Distinct();
            query.ToList();
        }
        [TestMethod]
        public void NoLock()
        {
            var query = (from user in db.Users.NoLock()
                         join order in db.TransferOrders.NoLock() on user.UserId equals order.ToUserId into us
                         from u in us.DefaultIfEmpty()
                         join flow in db.TransferWorkFlows.NoLock() on u.ToUserId equals flow.UploadUserId
                         join user1 in db.Users.NoLock() on user.Password equals user1.Username into test
                         from t in test.DefaultIfEmpty()
                         where flow.UploadUserId == 1 && t.Username == "xxxx"
                         select new
                         {
                             t.Username,
                             user.UserId,
                             user.LastLoginDate.Value.Date,
                             flow.UploadFileName
                         }).Distinct();
            query.ToList();
        }

        bool Test1()
        {
            return DateTime.Today.Day != 1;
        }

        [TestMethod]
        public void 复杂方法调用()
        {
            var t = new UnitTest1();
            var query = from user in db.Users where t.Test1() select user;
            query.ToList();
        }
        [TestMethod]
        public void Count()
        {
            var t = new UnitTest1();
            var query = from user in db.Users where t.Test1() select user;
            query.Count();
        }
        [TestMethod]
        public void Count1()
        {
            var t = new UnitTest1();
            var query = from user in db.Users where t.Test1() select user;
            query.Count(x => x.Password == string.Empty);
        }
        [TestMethod]
        public void Sum()
        {
            var t = new UnitTest1();
            var query = from user in db.Users where t.Test1() select user;
            query.Sum(x => x.UserId);
        }
        [TestMethod]
        public void Avg()
        {
            var t = new UnitTest1();
            var query = from user in db.Users where t.Test1() select user;
            query.Average(x => x.UserId);
        }
        [TestMethod]
        public void DateAddDay()
        {
            var query = from user in db.Users where user.LastLoginDate.Value.AddDays(1) <= DateTime.Now.Date select user;
            query.Average(x => x.UserId);
        }
        [TestMethod]
        public void DateAddHour()
        {
            var query = from user in db.Users where user.LastLoginDate.Value.AddHours(1) <= DateTime.Now.Date select user;
            query.Average(x => x.UserId);
        }

        [TestMethod]
        public void DateAddMilliseconds()
        {
            var query = from user in db.Users where user.LastLoginDate.Value.AddMilliseconds(1) <= DateTime.Now.Date select user;
            query.Average(x => x.UserId);
        }
        [TestMethod]
        public void DateAddMINUTE()
        {
            var query = from user in db.Users where user.LastLoginDate.Value.AddMinutes(1) <= DateTime.Now.Date select user;
            query.Average(x => x.UserId);
        }
        [TestMethod]
        public void DateAddMonth()
        {
            var query = from user in db.Users where user.LastLoginDate.Value.AddMonths(1) <= DateTime.Now.Date select user;
            query.Average(x => x.UserId);
        }
        [TestMethod]
        public void DateAddSeconds()
        {
            var query = from user in db.Users where user.LastLoginDate.Value.AddSeconds(1) <= DateTime.Now.Date select user;
            query.Average(x => x.UserId);
        }
        [TestMethod]
        public void DateAddYears()
        {
            var query = from user in db.Users where user.LastLoginDate.Value.AddYears(1) <= DateTime.Now.Date select user;
            query.Average(x => x.UserId);
        }

        [TestMethod]
        public void DateAddColumnYears()
        {
            var query = from user in db.Users where DateTime.Now.AddDays(user.UserId) <= DateTime.Now.Date select user;
            query.Average(x => x.UserId);
        }

        [TestMethod]
        public void ColumnDateDiffObject()
        {
            var query = from user in db.Users where (user.LastLoginDate.Value - DateTime.Now).TotalDays + DateTime.Now.Day > 5 select user;
            query.Average(x => x.UserId);
        }


        [TestMethod]
        public void ForceConvert()
        {
            var query = from user in db.Users where ((DateTime)user.LastLoginDate).AddDays(1) <= DateTime.Now.Date select user;
            query.Average(x => x.UserId);
        }
    }
}
