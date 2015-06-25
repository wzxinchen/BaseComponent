using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.Utils
{
    public class ReflectorConsts
    {
        public static readonly Type ObjectType = typeof(object);
        public static readonly Type ObjectArrayType = typeof(object[]);
        public static readonly Type BoolType = typeof(bool);
        public static readonly Type ConvertType = typeof(Convert);
        public static readonly Type IEnumerableType = typeof(IEnumerable<>);
        public static readonly Type EnumerableType = typeof(Enumerable);
        public static readonly Type QueryableType = typeof(Queryable);

        public static readonly Type Int32Type = typeof(Int32);
        public static readonly Type NullableType = typeof(Nullable<>);
        public static readonly Type StringType = typeof(string);

        public static readonly Type DateTimeType = typeof(DateTime);
        public static readonly Type DateTimeNullableType = typeof(DateTime?);
        public static readonly Type TimeSpanType = typeof(TimeSpan);

        public static readonly MethodInfo OrderByMethod = QueryableType.GetMethods().FirstOrDefault(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);
        public static readonly MethodInfo OrderByDescendingMethod = QueryableType.GetMethods().FirstOrDefault(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);
        public static readonly MethodInfo ThenByMethod = QueryableType.GetMethods().FirstOrDefault(x => x.Name == "ThenBy" && x.GetParameters().Length == 2);
        public static readonly MethodInfo ThenByDescendingMethod = QueryableType.GetMethods().FirstOrDefault(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 2);
        public static readonly MethodInfo StringContains = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        public static readonly MethodInfo IEnumerableListContains = EnumerableType.GetMethods().FirstOrDefault(x => x.Name == "Contains" && x.GetParameters().Length == 2);
        public static readonly MethodInfo ConvertToDateTimeMethod = ConvertType.GetMethod("ToDateTime", new Type[] { ObjectType });
        public static readonly MethodInfo ConvertToStringMethod = ConvertType.GetMethod("ToString", new Type[] { ObjectType });
        public static readonly MethodInfo ConvertToInt32Method = ConvertType.GetMethod("ToInt32", new Type[] { ObjectType });
        public static readonly MethodInfo ConvertToBoolMethod = ConvertType.GetMethod("ToBool", new Type[] { ObjectType });

    }
}
