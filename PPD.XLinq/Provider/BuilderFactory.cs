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
    public class BuilderFactory
    {
        DatabaseTypes _databaseType;
        public BuilderFactory(DatabaseTypes databaseType)
        {
            _databaseType = databaseType;
        }
        static Type _type;
        static List<Type> _providerTypes;
        public SqlBuilderBase CreateSqlBuilder()
        {
            if (_providerTypes == null)
            {
                _providerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(SqlBuilderBase)).ToList();
            }
            var typeName = _databaseType.ToString() + ".SqlBuilder";
            var types = _providerTypes.Where(x => x.FullName.EndsWith(typeName));
            if (types.Count() > 1)
            {
                throw new NotSupportedException("找到了多个包含" + typeName + "的提供者类");
            }
            var type = types.FirstOrDefault();
            if (type == null)
            {
                throw new NotSupportedException("未找到提供者类：" + typeName);
            }
            return (SqlBuilderBase)ObjectCache<SqlBuilderBase>.GetObjectFromCallContext(type);
        }
    }
}
