using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.DbUtils
{
    public class ExpressionHelper
    {
        public static Type GetMemberType(MemberExpression expression)
        {
            var memberInfo = expression.Member;
            switch (memberInfo.MemberType)
            {
                case System.Reflection.MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                default:
                    throw new Exception("未支持的成员类型：" + memberInfo.MemberType.ToString());
            }
        }
    }
}
