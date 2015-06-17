using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xinchen.DynamicObject.EmitReflection;
namespace EmitReflectorTest
{
    [TestClass]
    public class UnitTest1:IPropertyAccessor
    {
        public int MyProperty { get; set; }
        public int MyProperty1 { get; set; }
        public object GetValue(object obj,string propertyName)
        {
            UnitTest1 ut = (UnitTest1)obj;
            switch(propertyName)
            {
                case "MyProperty":
                    return ut.MyProperty;
                case "MyProperty1":
                    return ut.MyProperty1;
                default:
                    return string.Empty;
            }
        }
        [TestMethod]
        public void TestMethod1()
        {
            Model model = new Model();
            var type = model.GetEmitType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                property.GetValue(model);
                break;
            }
        }

        public class Model
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public void SetValue(object target, object value, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
