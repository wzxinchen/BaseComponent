using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DynamicObject;

namespace PPD.XLinq.Provider
{
    public class BuilderFactory
    {
        static Type _type;
        public static SqlBuilderBase CreateSqlBuilder()
        {
            if (string.IsNullOrWhiteSpace(ConfigManager.SqlBuilder))
            {
                return new SqlBuilder();
            }
            if (_type == null)
            {
                var assInfos = ConfigManager.SqlBuilder.Split(',');
                _type = Assembly.Load(assInfos[1]).GetTypes().FirstOrDefault(x => x.FullName == assInfos[0]);
            }
            return (SqlBuilderBase)ExpressionReflector.CreateInstance(_type);
        }
    }
}
