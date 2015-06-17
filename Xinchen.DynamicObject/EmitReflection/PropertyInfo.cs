using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.DynamicObject.EmitReflection
{
    public class PropertyInfo
    {
        private System.Reflection.PropertyInfo item;


        public PropertyInfo(System.Reflection.PropertyInfo item)
        {
            // TODO: Complete member initialization
            this.item = item;
        }


        public object GetValue(object obj)
        {
            AssemblyName DemoName = new AssemblyName("EmitAssembly");
            AssemblyBuilderAccess assemblyBuilderAccess = AssemblyBuilderAccess.RunAndSave;
            AssemblyBuilder dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(DemoName,
                assemblyBuilderAccess);
            var moduleBuilder = dynamicAssembly.DefineDynamicModule("EmitModule", "test.dll");
            var typeBuilder = moduleBuilder.DefineType("EmitType");
            //typeBuilder.AddInterfaceImplementation(typeof(IPropertyAccessor));
            var getValueMethodBuilder = typeBuilder.DefineMethod("GetValue", MethodAttributes.Public, typeof(object), new System.Type[] { typeof(object), typeof(string) });
            var getValueIlGen = getValueMethodBuilder.GetILGenerator();
            //var defaultCase = getValueIlGen.DefineLabel();

            getValueIlGen.DeclareLocal(item.DeclaringType);
            getValueIlGen.DeclareLocal(typeof(object));
            getValueIlGen.DeclareLocal(typeof(string));
            getValueIlGen.Emit(OpCodes.Nop);
            getValueIlGen.Emit(OpCodes.Ldarg_1);
            getValueIlGen.Emit(OpCodes.Castclass, item.DeclaringType);
            getValueIlGen.Emit(OpCodes.Stloc_0);
            getValueIlGen.Emit(OpCodes.Ldarg_2);
            getValueIlGen.Emit(OpCodes.Stloc_2);
            getValueIlGen.Emit(OpCodes.Ldloc_2);
            getValueIlGen.Emit(OpCodes.Ldstr, item.Name);
            getValueIlGen.EmitCall(OpCodes.Callvirt, typeof(string).GetMethod("Equals", new System.Type[] { typeof(string) }), new System.Type[] { typeof(string) });
            var testCase = getValueIlGen.DefineLabel();
            getValueIlGen.Emit(OpCodes.Brtrue_S, testCase);
            getValueIlGen.MarkLabel(testCase);
            getValueIlGen.Emit(OpCodes.Ldloc_0);
            getValueIlGen.EmitCall(OpCodes.Callvirt, item.GetGetMethod(),System.Type.EmptyTypes);
            getValueIlGen.Emit(OpCodes.Ret);
            getValueIlGen.Emit(OpCodes.Ldloc_2);
            getValueIlGen.Emit(OpCodes.Ldstr, "Name");
            getValueIlGen.EmitCall(OpCodes.Callvirt, typeof(string).GetMethod("Equals", new System.Type[] { typeof(string) }), new System.Type[] { typeof(string) });
            var testCase1 = getValueIlGen.DefineLabel();
            getValueIlGen.Emit(OpCodes.Brtrue_S, testCase1);
            getValueIlGen.MarkLabel(testCase1); 
            getValueIlGen.Emit(OpCodes.Ldloc_0);
            getValueIlGen.EmitCall(OpCodes.Callvirt, item.DeclaringType.GetProperty("Name").GetGetMethod(), System.Type.EmptyTypes);
            getValueIlGen.Emit(OpCodes.Ret);
            //getValueIlGen.Emit(OpCodes.Ldarg_0);
            //getValueIlGen.EmitCall(OpCodes.Callvirt, item.GetGetMethod(), new System.Type[] { typeof(object) });
            //getValueIlGen.Emit(OpCodes.Ret);
            //var propertyBuilder = typeBuilder.DefineProperty("Test", PropertyAttributes.SpecialName, value.GetType(), null);
            //var targetObjectType = obj.GetType();
            //var targetObjectProperties = targetObjectType.GetProperties();
            //Label defaultCase = getValueIlGen.DefineLabel();
            ////Label endOfMethod = getValueIlGen.DefineLabel();
            //var labelDict = targetObjectProperties.Select(x => new { PropertyInfo = x, Label = getValueIlGen.DefineLabel() }).ToDictionary(x => x.PropertyInfo);
            //var labels = labelDict.Select(x => x.Value.Label).ToArray();
            ////getValueIlGen.Emit(OpCodes.Ldarg_1);
            //getValueIlGen.Emit(OpCodes.Switch, labels);
            ////getValueIlGen.Emit(OpCodes.Br_S, defaultCase);
            //foreach (var targetObjectProperty in targetObjectProperties)
            //{
            //    var label = getValueIlGen.DefineLabel();
            //    getValueIlGen.MarkLabel(labelDict[targetObjectProperty].Label);
            //    getValueIlGen.Emit(OpCodes.Ldarg_1);
            //    getValueIlGen.Emit(OpCodes.Ldstr, targetObjectProperty.Name);
            //    getValueIlGen.EmitCall(OpCodes.Callvirt, typeof(string).GetMethod("Equals", new System.Type[] { typeof(string) }), new System.Type[] { typeof(string) });
            //    //getValueIlGen.Emit(OpCodes.Brtrue_S, label);
            //    //getValueIlGen.MarkLabel(label);
            //    //getValueIlGen.EmitCall(OpCodes.Callvirt, labelDict[targetObjectProperty].PropertyInfo.GetGetMethod(), new System.Type[] { typeof(object), typeof(string) });
            //    getValueIlGen.Emit(OpCodes.Ret);
            //    //getValueIlGen.Emit(OpCodes.Br_S, endOfMethod);

            //}
            //getValueIlGen.MarkLabel(defaultCase);
            typeBuilder.CreateType();
            dynamicAssembly.Save("test.dll");
            return null;
        }
    }
}
