using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DynamicObject;
using Xinchen.Utils;

namespace PPD.XLinq.Provider
{
    public class ProviderFactory
    {
        static Type _type = null;
        //public static BuilderFactory CreateSqlBuilderFactory()
        //{
        //    if (!string.IsNullOrWhiteSpace(ConfigManager.SqlBuilder))
        //    {
        //        if (_type == null)
        //        {
        //            var assInfos = ConfigManager.SqlBuilder.Split(',');
        //            _type = Assembly.Load(assInfos[1]).GetTypes().FirstOrDefault(x => x.FullName == assInfos[0]);
        //        }
        //        return (BuilderFactory)ExpressionReflector.CreateInstance(_type);
        //    }
        //    return new BuilderFactory();
        //}
        static Dictionary<string, Type> _providerTypes;

        internal static ProviderBase CreateProvider()
        {
            return CreateProvider(ConfigManager.DataBaseType);
        }
        public static ProviderBase CreateProvider(DatabaseTypes databaseType)
        {
            if (_type == null)
            {
                if (_providerTypes == null)
                {
                    _providerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(ProviderBase)).ToDictionary(x => x.Name);
                }
                var typeName = string.Format("Provider.{0}.{0}Provider", databaseType.ToString());
                var types = _providerTypes.Values.Where(x => x.FullName.EndsWith(typeName));
                if (types.Count() > 1)
                {
                    throw new NotSupportedException("找到了多个包含" + typeName + "的提供者类");
                }
                _type = types.FirstOrDefault();
                if (_type == null)
                {
                    throw new NotSupportedException("未找到提供者类：" + typeName);
                }
            }
            return (ProviderBase)ObjectCache<ProviderBase>.GetObjectFromCallContext(_type);
        }
    }
}
