using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.Utils
{
    public class TypeHelper
    {
        public static Type GetUnderlyingType(Type type)
        {
            if (!type.IsGenericType)
            {
                return type;
            }
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static bool IsNullableType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsGenericType)
            {
                return false;
            }
            if (type.GetGenericTypeDefinition() == ReflectorConsts.NullableType)
            {
                return true;
            }
            return false;
        }

        public static bool IsCompilerGenerated(Type type)
        {
            return type.GetCustomAttributes(ReflectorConsts.CompilerGeneratedAttributeType,false).Any();
        }
    }
}
