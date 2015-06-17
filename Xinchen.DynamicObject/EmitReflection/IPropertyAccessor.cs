using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.DynamicObject.EmitReflection
{
    public interface IPropertyAccessor
    {
        void SetValue(object target, object value, string propertyName);
        object GetValue(object target, string propertyName);
    }
}
