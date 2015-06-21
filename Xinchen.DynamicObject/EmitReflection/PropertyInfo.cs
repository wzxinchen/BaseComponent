using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xinchen.Utils;

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

        public IPropertyAccessor GetPropertyAccessor()
        {
            IPropertyAccessor accessor = null;
            if (!_propertyAccessor.TryGetValue(item.DeclaringType, out accessor))
            {
                AssemblyBuilder assBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("EmitReflection"), AssemblyBuilderAccess.Run);
                var moduleBuilder = assBuilder.DefineDynamicModule("EmitReflection"/*, "EmitReflection.dll"*/);
                var typeBuilder = moduleBuilder.DefineType(item.PropertyType.FullName + "<>PropertyAccessor", TypeAttributes.Public);
                typeBuilder.AddInterfaceImplementation(typeof(IPropertyAccessor));
                var stringType = typeof(string);
                var equalMethod = stringType.GetMethod("Equals", new System.Type[] { stringType });
                var methodBuilder = typeBuilder.DefineMethod("GetValue", MethodAttributes.Public | MethodAttributes.Virtual, typeof(object), new System.Type[] { typeof(object), typeof(string) });
                var dictTypeBuilder = moduleBuilder.DefineType(item.PropertyType.FullName + "<>Type");
                var dictFieldBuilder = dictTypeBuilder.DefineField("SwitchInfo", typeof(Dictionary<string, int>), FieldAttributes.Public | FieldAttributes.Static);
                dictTypeBuilder.CreateType();

                var ilBuilder = methodBuilder.GetILGenerator();
                ilBuilder.Emit(OpCodes.Nop);
                ilBuilder.DeclareLocal(typeof(Dictionary<string, int>));
                ilBuilder.DeclareLocal(typeof(int));//add索引
                ilBuilder.DeclareLocal(typeof(int));//get索引
                ilBuilder.DeclareLocal(item.DeclaringType);//对象索引
                ilBuilder.Emit(OpCodes.Ldarg_1);
                ilBuilder.Emit(OpCodes.Castclass, item.DeclaringType);
                ilBuilder.Emit(OpCodes.Stloc_3);
                ilBuilder.Emit(OpCodes.Ldsfld, dictFieldBuilder);
                var switchLabel = ilBuilder.DefineLabel();
                var endLabel = ilBuilder.DefineLabel();
                ilBuilder.Emit(OpCodes.Brtrue, switchLabel);
                var addMethod = typeof(Dictionary<string, int>).GetMethod("Add", new System.Type[] { typeof(string), typeof(int) });
                var tryGetValueMethod = typeof(Dictionary<string, int>).GetMethod("TryGetValue", new System.Type[] { typeof(string), typeof(int).MakeByRefType() });
                ilBuilder.Emit(OpCodes.Nop);
                ilBuilder.Emit(OpCodes.Newobj, typeof(Dictionary<string, int>).GetConstructor(System.Type.EmptyTypes));
                ilBuilder.Emit(OpCodes.Stloc_0);//字典
                ilBuilder.Emit(OpCodes.Ldc_I4_0);
                ilBuilder.Emit(OpCodes.Stloc_1);
                var labels = new Dictionary<Label, System.Reflection.PropertyInfo>();
                foreach (var property in item.PropertyType.GetProperties())
                {
                    ilBuilder.Emit(OpCodes.Ldloc_0);
                    ilBuilder.Emit(OpCodes.Ldstr, property.Name);
                    ilBuilder.Emit(OpCodes.Ldloc_1);
                    ilBuilder.EmitCall(OpCodes.Callvirt, addMethod, null);
                    ilBuilder.Emit(OpCodes.Nop);
                    ilBuilder.Emit(OpCodes.Ldloc_1);
                    ilBuilder.Emit(OpCodes.Ldc_I4_1);
                    ilBuilder.Emit(OpCodes.Add);
                    ilBuilder.Emit(OpCodes.Stloc_1);
                    labels.Add(ilBuilder.DefineLabel(), property);
                }
                ilBuilder.Emit(OpCodes.Ldloc_0);
                ilBuilder.Emit(OpCodes.Stsfld, dictFieldBuilder);
                ilBuilder.Emit(OpCodes.Nop);
                ilBuilder.MarkLabel(switchLabel);
                ilBuilder.Emit(OpCodes.Ldsfld, dictFieldBuilder);
                ilBuilder.Emit(OpCodes.Ldarg_2);
                ilBuilder.Emit(OpCodes.Ldloca, 2);
                ilBuilder.EmitCall(OpCodes.Callvirt, tryGetValueMethod, null);
                ilBuilder.Emit(OpCodes.Brfalse, endLabel);
                ilBuilder.Emit(OpCodes.Ldloc_2);
                ilBuilder.Emit(OpCodes.Switch, labels.Keys.ToArray());
                foreach (var label in labels)
                {
                    ilBuilder.MarkLabel(label.Key);
                    ilBuilder.Emit(OpCodes.Ldloc_3);
                    ilBuilder.EmitCall(OpCodes.Callvirt, label.Value.GetGetMethod(), System.Type.EmptyTypes);
                    if (label.Value.PropertyType.IsValueType)
                    {
                        ilBuilder.Emit(OpCodes.Box, label.Value.PropertyType);
                    }
                    ilBuilder.Emit(OpCodes.Ret);
                }
                ilBuilder.MarkLabel(endLabel);
                ilBuilder.Emit(OpCodes.Nop);
                ilBuilder.Emit(OpCodes.Ldnull);
                ilBuilder.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(methodBuilder, typeof(IPropertyAccessor).GetMethod("GetValue"));
                var type = typeBuilder.CreateType();
                accessor = (IPropertyAccessor)Activator.CreateInstance(type);
                _propertyAccessor.Add(item.DeclaringType, accessor);
                // assBuilder.Save("EmitReflection.dll");
            }
            return accessor;
        }

        static Dictionary<System.Type, IPropertyAccessor> _propertyAccessor = new Dictionary<System.Type, IPropertyAccessor>();

        public object GetValue(object obj)
        {
            var getter = GetPropertyAccessor();
            return getter.GetValue(obj, this.item.Name);
        }
    }
}
