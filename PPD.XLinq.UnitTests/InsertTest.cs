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
            var user = db.Set<User>().FirstOrDefault();
        }
    }
}
