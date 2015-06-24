using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.DbUtils.DynamicExpression
{
    public class ExpressionConsts
    {
        public static readonly Type IEnumerableType = typeof(IEnumerable<>);
        public static readonly Type EnumerableType = typeof(Enumerable);
        public static readonly Type QueryableType = typeof(Queryable);
        public static readonly MethodInfo StringContains = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
        public static readonly MethodInfo IEnumerableListContains = EnumerableType.GetMethods().FirstOrDefault(x => x.Name == "Contains" && x.GetParameters().Length == 2);

        public static readonly Type Int32Type = typeof(Int32);
        public static readonly Type NullableType = typeof(Nullable<>);
        public static readonly Type StringType = typeof(string);

        public static readonly Type DateTimeType = typeof(DateTime);
        public static readonly Type DateTimeNullableType = typeof(DateTime?);
        public static readonly Type TimeSpanType = typeof(TimeSpan);
    }
}
