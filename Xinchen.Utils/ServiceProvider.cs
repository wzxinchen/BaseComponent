using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.Utils
{
    public class ServiceProvider
    {
        static Dictionary<Type, object> _serviceDict = new Dictionary<Type, object>();
        public static T GetService<T>()
        {
            var type = typeof(T);
            object service = null;
            if (!_serviceDict.TryGetValue(type, out service))
            {
                service = Activator.CreateInstance(type, false);
                _serviceDict.Add(type, service);
            }
            return (T)service;
        }
    }
}
