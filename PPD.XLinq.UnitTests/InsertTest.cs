using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPD.XLinq.UnitTests.Model;
using System.Linq;
using System.Diagnostics;
using Xinchen.DbUtils;
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
            for (int i = 0; i < 200000; i++)
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
            for (int i = 0; i < 200000; i++)
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
        public void InsertBigTableBigData()
        {
            var watch = Stopwatch.StartNew();
            var dbUser = db.Set<LargeUser>();
            for (int i = 0; i < 200000; i++)
            {
                dbUser.Add(new LargeUser()
                {
                    BankAccount = "zzzz",
                    BankBranchName = "xxxx",
                    BankCity = 1,
                    BankOtherType = "aaaaaaaa",
                    BankType = 11,
                    BorrowCredit = 11,
                    CanCreateListDate = DateTime.Now.Date,
                    CreationDate = DateTime.Now,
                    DefaultRisk = 1,
                    Dispark = 1,
                    DisplayName = "xxxx",
                    Email = "aaa",
                    ExecuteAdminID = 1,
                    flag_UsertoCEmail = 1,
                    inserttime = DateTime.Now,
                    ISactive = true,
                    IsDelete = 0,
                    IsInpour = true,
                    IsTemp = true,
                    LastLoginDate = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    LendCredit = 1,
                    LendRisk = true,
                    Level = 1,
                    materialScore = 1,
                    Password = "xxx",
                    Password0 = "xxxx",
                    Password09 = "xxxx",
                    Password11 = "xxxx",
                    Password111 = "xxxx",
                    Password12 = "xxxx",
                    Password122 = "xxxx",
                    Password13 = "xxxx",
                    Password133 = "xxxx",
                    Password14 = "xxxx",
                    Password144 = "xxxx",
                    Password15 = "xxxx",
                    Password155 = "xxxx",
                    Password16 = "xxxx",
                    Password166 = "xxxx",
                    Password17 = "xxxx",
                    Password177 = "xxxx",
                    Password18 = "xxxx",
                    Password188 = "xxxx",
                    Password19 = "xxxx",
                    Password199 = "xxxx",
                    Password2 = "xxxx",
                    Password21 = "xxxx",
                    Password3 = "xxxx",
                    Password32 = "xxxx",
                    Password4 = "xxxx",
                    Password43 = "xxxx",
                    Password5 = "xxxx",
                    Password54 = "xxxx",
                    Password6 = "xxxx",
                    Password65 = "xxxx",
                    Password7 = "xxxx",
                    Password76 = "xxxx",
                    Password8 = "xxxx",
                    Password87 = "xxxx",
                    Password9 = "xxxx",
                    Password98 = "xxxx",
                    Picture = "xxxx",
                    Role = 1,
                    SpaceID = 1,
                    updatetime = DateTime.Now,
                    UserName = "xxx",
                    Verifystatus = 1
                });
            }
            db.SaveChanges();
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
        [TestMethod]
        public void InsertIdentityBigTableBigData()
        {
            var watch = Stopwatch.StartNew();
            var dbUser = db.Set<LargeUser>();
            for (int i = 0; i < 200000; i++)
            {
                dbUser.Add(new LargeUser()
                {
                    BankAccount = "zzzz",
                    BankBranchName = "xxxx",
                    BankCity = 1,
                    BankOtherType = "aaaaaaaa",
                    BankType = 11,
                    BorrowCredit = 11,
                    CanCreateListDate = DateTime.Now.Date,
                    CreationDate = DateTime.Now,
                    DefaultRisk = 1,
                    Dispark = 1,
                    DisplayName = "xxxx",
                    Email = "aaa",
                    ExecuteAdminID = 1,
                    flag_UsertoCEmail = 1,
                    inserttime = DateTime.Now,
                    ISactive = true,
                    IsDelete = 0,
                    IsInpour = true,
                    IsTemp = true,
                    LastLoginDate = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    LendCredit = 1,
                    LendRisk = true,
                    Level = 1,
                    materialScore = 1,
                    Password = "xxx",
                    Password0 = "xxxx",
                    Password09 = "xxxx",
                    Password11 = "xxxx",
                    Password111 = "xxxx",
                    Password12 = "xxxx",
                    Password122 = "xxxx",
                    Password13 = "xxxx",
                    Password133 = "xxxx",
                    Password14 = "xxxx",
                    Password144 = "xxxx",
                    Password15 = "xxxx",
                    Password155 = "xxxx",
                    Password16 = "xxxx",
                    Password166 = "xxxx",
                    Password17 = "xxxx",
                    Password177 = "xxxx",
                    Password18 = "xxxx",
                    Password188 = "xxxx",
                    Password19 = "xxxx",
                    Password199 = "xxxx",
                    Password2 = "xxxx",
                    Password21 = "xxxx",
                    Password3 = "xxxx",
                    Password32 = "xxxx",
                    Password4 = "xxxx",
                    Password43 = "xxxx",
                    Password5 = "xxxx",
                    Password54 = "xxxx",
                    Password6 = "xxxx",
                    Password65 = "xxxx",
                    Password7 = "xxxx",
                    Password76 = "xxxx",
                    Password8 = "xxxx",
                    Password87 = "xxxx",
                    Password9 = "xxxx",
                    Password98 = "xxxx",
                    Picture = "xxxx",
                    Role = 1,
                    SpaceID = 1,
                    updatetime = DateTime.Now,
                    UserName = "xxx",
                    Verifystatus = 1
                });
            }
            db.SaveChanges();
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
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
            var user =  db.Set<User>().FirstOrDefault();
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
            var user =  db.Set<User>().FirstOrDefault(x => x.Id == 17);
            user.IsEnabled = false;
            user.LastLoginDate = DateTime.Now.Date;
            user.Password = "password1";
            user.Username = "username1";
            user = db.Set<User>().FirstOrDefault(x => x.Id == 18);
            user.Password = "password2";
            user.Username = "username2";
            db.SaveChanges();
        }
        [TestMethod]
        public void QueryDelete()
        {
            var user = db.Set<User>().FirstOrDefault(x => x.Id == 11);
            db.Set<User>().Remove(user);
            user = db.Set<User>().FirstOrDefault(x => x.Id == 12);
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
