using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.Utils
{
    /// <summary>
    /// 提供简单的高性能的单例模式
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public static class ObjectCache<TObject>
    {
        public static TObject GetObject()
        {
            return GetObject(() => (TObject)Activator.CreateInstance(typeof(TObject), false));
        }
        public static TObject GetObject(Func<TObject> createInstanceAction)
        {
            if (Instance == null)
            {
                Instance = createInstanceAction();
            }
            return Instance;
        }

        public static object GetObjectFromCallContext(Type type)
        {
            return GetObjectFromCallContext(type, x => Activator.CreateInstance(x));
        }

        public static object GetObjectFromCallContext(Type type, Func<Type, object> createInstance)
        {
            object instance = CallContext.GetData(type.FullName);
            if (instance == null)
            {
                instance = createInstance(type);// Activator.CreateInstance(type);
                CallContext.SetData(type.FullName, instance);
            }
            return instance;
        }
        static TObject Instance { get; set; }
    }
}
