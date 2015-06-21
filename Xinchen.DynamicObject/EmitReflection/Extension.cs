using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.DynamicObject.EmitReflection
{
    public static class Extension
    {
        public static Type GetEmitType(this object obj)
        {
            if(obj==null)
            {
                return null;
            }
            return new Type(obj.GetType());
        }
        public static Type GetEmitType(this System.Type obj)
        {
            if (obj == null)
            {
                return null;
            }
            return new Type(obj);
        }
    }
}
