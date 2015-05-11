using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest2
    {
        EducationGameEntities db = new EducationGameEntities();
        [TestMethod]
        public void TestMethod1()
        {
            var query = from c in db.Departments
                        join d in db.DependenceStageses on c.Id equals d.Id
                        select c;
            string s = query.ToString();
        }
    }
}
