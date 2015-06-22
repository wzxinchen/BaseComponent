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
            if(!type.IsGenericType)
            {
                return type;
            }
            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }
}
