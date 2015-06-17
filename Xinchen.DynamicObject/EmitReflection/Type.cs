using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.DynamicObject.EmitReflection
{
    public class Type
    {
        private System.Type type;

        public Type(System.Type type)
        {
            // TODO: Complete member initialization
            this.type = type;
        }


        public IEnumerable<PropertyInfo> GetProperties()
        {
            var properties = type.GetProperties();
            foreach (var item in properties)
            {
                yield return new PropertyInfo(item);
            }
        }
    }
}
