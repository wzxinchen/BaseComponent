using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.Utils
{
    /// <summary>
    /// 提供简单的但不是线程安全的高性能的单例模式
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public static class ObjectCache<TObject>
    {
        public static TObject GetObject()
        {
            if (Instance == null)
            {
                Instance = (TObject)Activator.CreateInstance(typeof(TObject), false);
            }
            return Instance;
        }
        public static TObject GetObject(Func<TObject> createInstanceAction)
        {
            if (Instance == null)
            {
                Instance = createInstanceAction();
            }
            return Instance;
        }
        static TObject Instance { get; set; }
    }
}
