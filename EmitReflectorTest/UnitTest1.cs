using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Xinchen.DynamicObject;
using System.Linq;
using Xinchen.DynamicObject.EmitReflection;
namespace EmitReflectorTest
{
    [TestClass]
    public class UnitTest1
    {
        public int MyProperty { get; set; }
        public int MyProperty1 { get; set; }
        [TestMethod]
        public void TestEmit()
        {
            var models = new List<Model>();
            for (int i = 0; i < 10000; i++)
            {
                models.Add(new Model()
                {
                    Id = 1,
                    Id1 = 1,
                    Name = "1",
                    P1 = DateTime.Now,
                    P2 = DateTime.Now,
                    P3 = 1,
                    P4 = 1,
                    P5 = 1,
                    P6 = 1
                });
            }
            var type = typeof(Model).GetEmitType();
            var properties = type.GetProperties();
            foreach (var model in models)
            {
                foreach (var property in properties)
                {
                    property.GetValue(model);
                }
            }
        }

        [TestMethod]
        public void TestDelegate()
        {
            var models = new List<Model>();
            for (int i = 0; i < 10000000; i++)
            {
                models.Add(new Model()
                {
                    Id = 1,
                    Id1 = 1,
                    Name = "1",
                    P1 = DateTime.Now,
                    P2 = DateTime.Now,
                    P3 = 1,
                    P4 = 1,
                    P5 = 1,
                    P6 = 1
                });
            }
            var properties = typeof(Model).GetProperties();
            foreach (var model in models)
            {
                foreach (var property in properties)
                {
                    var getters = ExpressionReflector.GetValue(model, property.Name);
                }
            }
        }

        public class Model
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int? Id1 { get; set; }
            public DateTime P1 { get; set; }
            public DateTime? P2 { get; set; }
            public long P3 { get; set; }
            public long? P4 { get; set; }
            public short P5 { get; set; }
            public short? P6 { get; set; }
        }

        public class A
        {
            public static Dictionary<string, int> SwitchInfo;
        }

        public object GetValue(object obj1, string text1)
        {
            int num2;
            UnitTest1.Model model = (UnitTest1.Model)obj1;
            if (A.SwitchInfo == null)
            {
                Dictionary<string, int> dictionary = new Dictionary<string, int>();
                int num = 0;
                dictionary.Add("Id", num);
                num++;
                dictionary.Add("Name", num);
                num++;
                A.SwitchInfo = dictionary;
            }
            if (!A.SwitchInfo.TryGetValue(text1, out num2))
            {
                return null;
            }
            switch (num2)
            {
                case 1:
                    return model.Name;
            }
            return model.Id;
        }
    }
}
