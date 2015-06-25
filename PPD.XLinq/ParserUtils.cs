using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PPD.XLinq
{
    public class ParserUtils
    {
        static int _tableNum = 0;
        static object _tableLocker = new object();
        static Type _compilerGeneratedAttribute = typeof(CompilerGeneratedAttribute);
        /// <summary>
        /// 根据名称生成随机别名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GenerateAlias(string name)
        {
            lock (_tableLocker)
            {
                _tableNum++;
                return name + _tableNum;
            }
        }

        public static bool IsAnonymousType(Type type)
        {
            bool hasCompilerGeneratedAttribute = type.GetCustomAttributes(_compilerGeneratedAttribute, false).Count() > 0;
            bool nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            bool isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

            return isAnonymousType;
        }
    }
}
