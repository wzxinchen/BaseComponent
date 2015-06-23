using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPD.XLinq.UnitTests.Model;
using System.Linq;
namespace PPD.XLinq.UnitTests
{
    [TestClass]
    public class InsertTest
    {
        TestDataContext db = new TestDataContext();
        [TestMethod]
        public void InsertBigData()
        {
            var dbUser = db.Set<User>();
            for (int i = 0; i < 50000; i++)
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
            for (int i = 0; i < 20000; i++)
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
        public void QueryUpdate()
        {
            var user = db.QueryEnableProxy(() => db.Set<User>().FirstOrDefault());
            user.IsEnabled = false;
            user.LastLoginDate = DateTime.Now.Date;
            user.Password = "xxxxxx";
            user.Username = "uuuuuu";
            db.SaveChanges();
        }
        [TestMethod]
        public void QueryNotUpdate()
        {
            var user = db.Set<User>().FirstOrDefault();
            user.IsEnabled = false;
            user.LastLoginDate = DateTime.Now.Date;
            user.Password = "xxxx1111xx";
            user.Username = "uuuuuu";
            db.SaveChanges();
        }
        [TestMethod]
        public void QueryUpdateMultiEntity()
        {
            var user = db.QueryEnableProxy(() => db.Set<User>().FirstOrDefault(x => x.Id == 1));
            user.IsEnabled = false;
            user.LastLoginDate = DateTime.Now.Date;
            user.Password = "password1";
            user.Username = "username1";
            user = db.QueryEnableProxy(() => db.Set<User>().FirstOrDefault(x => x.Id == 2));
            user.Password = "password2";
            user.Username = "username2";
            db.SaveChanges();
        }
        [TestMethod]
        public void QueryDelete()
        {
            var user = db.Set<User>().FirstOrDefault(x => x.Id == 3);
            db.Set<User>().Remove(user);
            user = db.Set<User>().FirstOrDefault(x => x.Id == 4);
            db.Set<User>().Remove(user);
            db.SaveChanges();
        }

        [TestMethod]
        public void WhereDelete()
        {
            db.Set<User>().Where(x => x.Id == 2).Delete();
        }
        [TestMethod]
        public void DirectDelete()
        {
            var ids = new int[] { 5, 6 };
            db.Set<User>().Delete(x => ids.Contains(x.Id));
        }
        [TestMethod]
        public void WhereUpdate()
        {
            var ids = new int[] { 7, 8 };
            db.Set<User>().Where(x => ids.Contains(x.Id)).Update(x => new User()
            {
                LastLoginDate = DateTime.Now,
                Password = "password78",
                Username = "username78"
            });
        }
    }
}
