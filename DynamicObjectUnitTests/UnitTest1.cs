using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xinchen.DynamicObject;

namespace DynamicObjectUnitTests
{
    [TestClass]
    public class UnitTest1:Model
    {
        [TestMethod]
        public void TestCreateProxy()
        {
            var model = DynamicProxy.CreateDynamicProxy(new Model());
            model.Id = 1;

        }
    }

    public class Model
    {
        public virtual int Id { get; set; }
    }
}
