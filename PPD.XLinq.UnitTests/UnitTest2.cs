using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PPD.XLinq.UnitTests.Model;
using System.Linq;
namespace PPD.XLinq.UnitTests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            TestDataContext db = new TestDataContext();
            db.Set<TransferOrder>().ToList();
            db.Set<User>().ToList();
        }
    }
}
