using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPD.XLinq.UnitTests.Model;
using System.Linq;
using System.Text;
using System.Diagnostics;
using PPD.XLinq;
using Xinchen.Utils;
using System.Collections.Generic;
namespace PPD.XLinq.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        TestDataContext db = ObjectCache<TestDataContext>.GetObject();
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
            var list = db.Set<User>().Where(x => x.Password == "06ea391fdd9abd6ac43cc858dcc7c4ce" && x.Username == "awfer1").ToList();

            //string aa = "呵呵";
            //var a=aa.Length;
            //TestDataContext db = new TestDataContext();
            //var date = DateTime.Now;
            //UnitTest1 t = new UnitTest1();
            //db.Set<User>().Select(x => new
            //{
            //    Reg = x.RegTime.Date,
            //    x.RegTime.Date,
            //    x.Id,
            //    x.Age
            //}).Where(x => x.Reg.Date == DateTime.Now.Date && x.Id == 1 && x.Age == 1).ToList();
            //db.Set<User>().Select(x => x).Where(x => x.RegTime.Date == DateTime.Now.Date && x.Id == 1 && x.Age == 1).ToList();
            //var query = from user in db.Set<User>() select user;
        }
        [TestMethod]
        public void SelectOrderBigData()
        {
            var list = db.Set<TransferOrder>().OrderBy(x => x.Id).ThenBy(x => x.ToUserId).ToList();
        }
        [TestMethod]
        public void SelectOrderBigData1()
        {
            var list = db.Set<TransferOrder>().Select(x => new { x.Id, x.ToUserId, x.ToUsername }).Where(x => x.Id == 1).OrderBy(x => x.Id).ThenBy(x => x.ToUserId).ToList();
        }



        [TestMethod]
        public void ToDictionary()
        {
            var list = db.Set<User>().Where(x => x.LastLoginDate.Value.Date == DateTime.Now.Date).Select(x => new { x.Username, x.Id }).ToDictionary(x => x.Id);
        }



        [TestMethod]
        public void Select中取Date()
        {
            var query = (from user in db.Set<User>()
                         where user.Password == "xxxx" && user.Id == 1
                         select new
                         {
                             Id = user.Id,
                             user.Password,
                             user.LastLoginDate.Value.Date
                         });
            query.ToList();
        }

        [TestMethod]
        public void Where方法调用()
        {
            var query = from user in db.Set<User>()
                        where (user.Password.Substring(0, 1) == "aaaa" ||
                        user.LastLoginDate.Value.Date == Convert.ToDateTime(DateTime.Now.ToString()).Date.Date &&
                        user.Password == "xxxx") && user.Id == 1
                        select new
                        {
                            Id = user.Id,
                            user.Password,
                            user.LastLoginDate.Value.Date
                        };
            query.ToList();
        }
        [TestMethod]
        public void Where方法调用1()
        {
            var query = from user in db.Set<User>()
                        where (user.LastLoginDate.Value.Date == Convert.ToDateTime("2015-1-1").Date.Date && user.Password == "xxxx") && user.Id == 1
                        select new
                        {
                            Id = user.Id,
                            user.Password,
                            user.LastLoginDate.Value.Date
                        };
            query.ToList();
        }

        [TestMethod]
        public void SimpleAliasSelect()
        {
            var query = from user in db.Set<User>()
                        where user.Password == "xxx"
                        select new
                        {
                            Id = user.Id,
                            user.Password,
                            user.LastLoginDate
                        };
            query.ToList();
        }
        [TestMethod]
        public void SimpleNoAliasSelect()
        {
            var query = from user in db.Set<User>()
                        where user.Password == "xxx"
                        select new
                        {
                            UserId = user.Id,
                            user.Password
                        };
            query.ToList();
        }
        [TestMethod]
        public void Select不取Date()
        {
            var query = from user in db.Set<User>()
                        where user.Password == "xxx"
                        select new
                        {
                            Id = user.Id,
                            user.Password,
                            user.LastLoginDate
                        };
            query.ToList();
        }
        [TestMethod]
        public void 没有Select()
        {
            var query = db.Set<User>().Where(x => x.Password == "").ToList();
            query.ToList();
        }
        [TestMethod]
        public void WhereEqualNull()
        {
            var query = db.Set<User>().Where(x => x.LastLoginDate == null).ToList();
        }

        [TestMethod]
        public void 复杂条件查询()
        {
            var query = db.Set<User>().Where(x => x.Password == "xx" && (x.Id == 111 || x.Username == "xxxxx") && x.LastLoginDate == null).ToList();
        }

        [TestMethod]
        public void 复杂条件查询1()
        {
            var query = (from x in db.Set<User>() where x.Password == "xx" && (x.Id == 111 || x.Username == "xxxxx") && x.LastLoginDate == null select x).ToList();
        }


        [TestMethod]
        public void 直接ToList()
        {
            db.Set<User>().ToList();
        }
        [TestMethod]
        public void 直接ToList1()
        {
            db.Set<User>().Select(x => new { x.LastLoginDate }).ToList();
        }

        [TestMethod]
        public void 多重Where()
        {
            var query = db.Set<User>().Where(x => x.Id == 1);
            query = query.Where(x => x.Username == "xxx");
            query.ToList();
        }

        [TestMethod]
        public void JoinNoWhere()
        {
            var query = from user in db.Set<User>()
                        join order in db.Set<TransferOrder>() on user.Id equals order.ToUserId
                        join flow in db.Set<TransferWorkFlow>() on order.ToUserId equals flow.UploadUserId
                        select new
                        {
                            UserId = user.Id,
                            flow.UploadFileName,
                            order.ToUsername
                        };
            query.ToList();
        }
        [TestMethod]
        public void JoinWhere()
        {
            var query = from user in db.Set<User>()
                        join order in db.Set<TransferOrder>() on user.Id equals order.ToUserId
                        join flow in db.Set<TransferWorkFlow>() on order.ToUserId equals flow.UploadUserId
                        where flow.UploadUserId == 1
                        select new
                        {
                            UserId = user.Id,
                            flow.UploadFileName,
                            order.ToUsername
                        };
            query.ToList();
        }
        [TestMethod]
        public void JoinWhereDate()
        {
            var query = from user in db.Set<User>()
                        join order in db.Set<TransferOrder>() on user.Id equals order.ToUserId
                        join flow in db.Set<TransferWorkFlow>() on order.ToUserId equals flow.UploadUserId
                        where flow.UploadUserId == 1
                        select new
                        {
                            UserId = user.Id,
                            flow.UploadFileName,
                            order.ToUsername,
                            user.LastLoginDate.Value.Date
                        };
            query.ToList();
        }

        [TestMethod]
        public void ColumnStartsWithString()
        {
            db.Set<User>().Where(x => x.Password.StartsWith("xxxxx")).ToList();
        }

        [TestMethod]
        public void StringStartsWithColumn()
        {
            db.Set<User>().Where(x => "xxxxx".StartsWith(x.Password)).ToList();
        }

        [TestMethod]
        public void StringStartsWithString()
        {
            db.Set<User>().Where(x => "xxxxx".StartsWith("xxxxxxxxxxxx")).ToList();
        }

        [TestMethod]
        public void ColumnContainsString()
        {
            db.Set<User>().Where(x => x.Password.Contains("xxxxxxxxxxxx")).ToList();
        }
        [TestMethod]
        public void ColumnNotContainsString()
        {
            db.Set<User>().Where(x => !x.Password.Contains("xxxxxxxxxxxx")).ToList();
        }

        [TestMethod]
        public void StringContainsColumn()
        {
            db.Set<User>().Where(x => "xxxxxx".Contains(x.Password)).ToList();
        }

        [TestMethod]
        public void StringNotContainsColumn()
        {
            db.Set<User>().Where(x => !"xxxxxx".Contains(x.Password)).ToList();
        }
        [TestMethod]
        public void ListContainsColumn()
        {
            var list = new int[] { 1, 2 };
            db.Set<User>().Where(x => list.Contains(x.Id)).ToList();
        }
        [TestMethod]
        public void ListNotContainsColumn()
        {
            var list = new int[] { 1, 2 };
            db.Set<User>().Where(x => !list.Contains(x.Id)).ToList();
        }
        [TestMethod]
        public void ListContainsNumber()
        {
            var list = new int[] { 1, 2 };
            db.Set<User>().Where(x => list.Contains(1)).ToList();
        }
        [TestMethod]
        public void ListNotContainsNumber()
        {
            var list = new int[] { 1, 2 };
            db.Set<User>().Where(x => !list.Contains(1)).ToList();
        }
        [TestMethod]
        public void Or()
        {
            var list = new int[] { 1, 2 };
            db.Set<User>().Where(x => list == null || list.Contains(x.Id)).ToList();
        }
        [TestMethod]
        public void OrNull()
        {
            var list = new int[] { 1, 2 };
            list = null;
            db.Set<User>().Where(x => list == null || list.Contains(x.Id)).ToList();
        }
        [TestMethod]
        public void AndNull()
        {
            var list = new object();
            list = null;
            var a = db.Set<User>().Where(x => list == null && x.Password == "xxxx").ToList();
        }

        [TestMethod]
        public void LeftJoinNoWhere()
        {
            var query = from user in db.Set<User>()
                        join order in db.Set<TransferOrder>() on user.Id equals order.ToUserId into us
                        from u in us.DefaultIfEmpty()
                        join flow in db.Set<TransferWorkFlow>() on u.ToUserId equals flow.UploadUserId
                        join user1 in db.Set<User>() on user.Password equals user1.Username into test
                        from t in test.DefaultIfEmpty()
                        select new
                        {
                            t.Username,
                            u.ToUserId,
                            UserId = user.Id,
                            user.LastLoginDate.Value.Date,
                            flow.UploadFileName
                        };
            query.ToList();
        }

        [TestMethod]
        public void LeftSelfJoin()
        {
            var query1 = from user in db.Set<User>()
                         join user1 in db.Set<User>() on user.Password equals user1.Username into test
                         from t in test.DefaultIfEmpty()
                         select new
                         {
                             t.Username,
                             UserId = user.Id,
                             user.LastLoginDate.Value.Date
                         };
            query1.ToList();
        }
        [TestMethod]
        public void SelfJoin()
        {
            var query1 = from user in db.Set<User>()
                         join user1 in db.Set<User>() on user.Password equals user1.Username
                         select new
                         {
                             user1.Username,
                             UserId = user.Id,
                             user.LastLoginDate.Value.Date
                         };
            query1.ToList();
        }
        [TestMethod]
        public void LeftJoinWhere()
        {
            var query = from user in db.Set<User>()
                        join order in db.Set<TransferOrder>() on user.Id equals order.ToUserId into us
                        from u in us.DefaultIfEmpty()
                        join flow in db.Set<TransferWorkFlow>() on u.ToUserId equals flow.UploadUserId
                        join user1 in db.Set<User>() on user.Password equals user1.Username into test
                        from t in test.DefaultIfEmpty()
                        where flow.UploadUserId == 1 && t.Username == "xxxx"
                        select new
                        {
                            t.Username,
                            UserId = user.Id,
                            user.LastLoginDate.Value.Date,
                            flow.UploadFileName
                        };
            query.ToList();
        }
        [TestMethod]
        public void Distinct()
        {
            var query = (from user in db.Set<User>()
                         join order in db.Set<TransferOrder>() on user.Id equals order.ToUserId into us
                         from u in us.DefaultIfEmpty()
                         join flow in db.Set<TransferWorkFlow>() on u.ToUserId equals flow.UploadUserId
                         join user1 in db.Set<User>() on user.Password equals user1.Username into test
                         from t in test.DefaultIfEmpty()
                         where flow.UploadUserId == 1 && t.Username == "xxxx"
                         select new
                         {
                             t.Username,
                             UserId = user.Id,
                             user.LastLoginDate.Value.Date,
                             flow.UploadFileName
                         }).Distinct();
            query.ToList();
        }


        [TestMethod]
        public void NoLock()
        {
            var query = (from user in db.Set<User>().NoLock()
                         join order in db.Set<TransferOrder>().NoLock() on user.Id equals order.ToUserId into us
                         from u in us.DefaultIfEmpty()
                         join flow in db.Set<TransferWorkFlow>().NoLock() on u.ToUserId equals flow.UploadUserId
                         join user1 in db.Set<User>().NoLock() on user.Password equals user1.Username into test
                         from t in test.DefaultIfEmpty()
                         where flow.UploadUserId == 1 && t.Username == "xxxx"
                         select new
                         {
                             t.Username,
                             UserId = user.Id,
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
            var query = from user in db.Set<User>() where t.Test1() select user;
            query.ToList();
        }
        [TestMethod]
        public void Count()
        {
            var t = new UnitTest1();
            var query = from user in db.Set<User>() where t.Test1() select user;
            query.Count();
        }
        [TestMethod]
        public void Count1()
        {
            var t = new UnitTest1();
            var query = from user in db.Set<User>() where t.Test1() select user;
            query.Count(x => x.Password == string.Empty);
        }
        [TestMethod]
        public void Sum()
        {
            var t = new UnitTest1();
            var query = from user in db.Set<User>() where t.Test1() select user;
            query.Sum(x => x.Id);
        }
        [TestMethod]
        public void Avg()
        {
            var t = new UnitTest1();
            var query = from user in db.Set<User>() where t.Test1() select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void DateAddDay()
        {
            var query = from user in db.Set<User>() where user.LastLoginDate.Value.AddDays(1) <= DateTime.Now.Date select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void DateAddHour()
        {
            var query = from user in db.Set<User>() where user.LastLoginDate.Value.AddHours(1) <= DateTime.Now.Date select user;
            query.Average(x => x.Id);
        }

        [TestMethod]
        public void DateAddMilliseconds()
        {
            var query = from user in db.Set<User>() where user.LastLoginDate.Value.AddMilliseconds(1) <= DateTime.Now.Date select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void DateAddMINUTE()
        {
            var query = from user in db.Set<User>() where user.LastLoginDate.Value.AddMinutes(1) <= DateTime.Now.Date select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void DateAddMonth()
        {
            var query = from user in db.Set<User>() where user.LastLoginDate.Value.AddMonths(1) <= DateTime.Now.Date select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void DateAddSeconds()
        {
            var query = from user in db.Set<User>() where user.LastLoginDate.Value.AddSeconds(1) <= DateTime.Now.Date select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void DateAddYears()
        {
            var query = from user in db.Set<User>() where user.LastLoginDate.Value.AddYears(1) <= DateTime.Now.Date select user;
            query.Average(x => x.Id);
        }

        [TestMethod]
        public void DateAddColumnYears()
        {
            var query = from user in db.Set<User>() where DateTime.Now.AddDays(user.Id) <= DateTime.Now.Date select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void OrderBy()
        {
            var query = from user in db.Set<User>() where DateTime.Now.AddDays(user.Id) <= DateTime.Now.Date orderby user.Id descending select user;
            query.ToList();
        }

        [TestMethod]
        public void ColumnDateDiffObject()
        {
            var query = from user in db.Set<User>() where (user.LastLoginDate.Value - DateTime.Now).TotalDays + DateTime.Now.Day > 5 select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void ColumnDateDiffHourObject()
        {
            var query = from user in db.Set<User>() where (user.LastLoginDate.Value - DateTime.Now).TotalHours + DateTime.Now.Day > 5 select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void ColumnDateDiffMillisecondsObject()
        {
            var query = from user in db.Set<User>() where (user.LastLoginDate.Value - DateTime.Now).TotalMilliseconds + DateTime.Now.Day > 5 select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void ColumnDateDiffMinutesObject()
        {
            var query = from user in db.Set<User>() where (user.LastLoginDate.Value - DateTime.Now).TotalMinutes + DateTime.Now.Day > 5 select user;
            query.Average(x => x.Id);
        }
        [TestMethod]
        public void ColumnDateDiffSecondsObject()
        {
            var query = from user in db.Set<User>() where (user.LastLoginDate.Value - DateTime.Now).TotalSeconds + DateTime.Now.Day > 5 select user;
            query.Average(x => x.Id);
        }


        [TestMethod]
        public void ForceConvert()
        {
            var query = from user in db.Set<User>() where ((DateTime)user.LastLoginDate).AddDays(1) <= DateTime.Now.Date select user;
            query.Average(x => x.Id);
        }

        [TestMethod]
        public void InsertBigData()
        {
            var dbUser = db.Set<User>();
            for (int i = 0; i < 20000; i++)
            {
                dbUser.Add(new User()
                {
                    LastLoginDate = DateTime.Now,
                    Password = "111",
                    Username = "zzzz"
                });
            }
            db.SaveChanges();
        }
        [TestMethod]
        public void InsertIdentityBigData()
        {
            var dbUser = db.Set<TransferOrder>();
            for (int i = 0; i < 2000000; i++)
            {
                dbUser.Add(new TransferOrder()
                {
                    ToUserId = 1,
                    ToUsername = "zz"
                });
            }
            db.SaveChanges();
        }
        [TestMethod]
        public void InsertSmallData()
        {
            var dbUser = db.Set<User>();
            for (int i = 0; i < 5; i++)
            {
                dbUser.Add(new User()
                {
                    LastLoginDate = DateTime.Now,
                    Password = "111",
                    Username = "zzzz"
                });
            }
            db.SaveChanges();
        }
        [TestMethod]
        public void InsertSmallIdentityData()
        {
            var dbUser = db.Set<TransferOrder>();
            for (int i = 0; i < 5; i++)
            {
                dbUser.Add(new TransferOrder()
                {
                    ToUserId = 1,
                    ToUsername = "zz"
                });
            }
            db.SaveChanges();
        }

        [TestMethod]
        public void Take()
        {
            db.Set<User>().Take(1).ToList();
        }
        [TestMethod]
        public void PageBySkipTake()
        {
            db.Set<User>().OrderBy(x => x.Id).Skip(1).Take(1).ToList();
        }
        [TestMethod]
        public void PageBySkipTake1()
        {
            db.Set<User>().OrderBy(x => x.LastLoginDate.Value.Date).Skip(1).Take(1).ToList();
        }
        [TestMethod]
        public void PageByPage()
        {
            db.Set<User>().OrderBy(x => x.LastLoginDate.Value.Date).Page(1, 1);
        }

        [TestMethod]
        public void Page()
        {
            var query = (from user in db.Set<User>().NoLock()
                         join order in db.Set<TransferOrder>().NoLock() on user.Id equals order.ToUserId into us
                         from u in us.DefaultIfEmpty()
                         join flow in db.Set<TransferWorkFlow>().NoLock() on u.ToUserId equals flow.UploadUserId
                         join user1 in db.Set<User>().NoLock() on user.Password equals user1.Username into test
                         from t in test.DefaultIfEmpty()
                         where flow.UploadUserId == 1 && t.Username == "xxxx"
                         orderby t.Username descending
                         select new
                         {
                             t.Username,
                             UserId = user.Id,
                             user.LastLoginDate.Value.Date,
                             flow.UploadFileName
                         }).Distinct();
            query.Page(1, 1);
        }

        [TestMethod]
        public void FirstOrDefault()
        {
            db.Set<User>().FirstOrDefault(x => x.LastLoginDate == DateTime.Now.Date);
            db.Set<User>().First(x => x.LastLoginDate == DateTime.Now.Date);
        }

    }
}
