using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xinchen.DynamicObject;
using Xinchen.Utils;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.XLinq
{
    /// <summary>
    /// 表达式树转sql时的工具集合
    /// </summary>
    public class ExpressionUtil
    {
        static Type _tableAttr = typeof(TableAttribute);
        /// <summary>
        /// 获取此成员的类型
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static Type GetMemberType(MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
            }
            return null;
        }

        /// <summary>
        /// 是否是实体成员
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static bool IsEntityMember(MemberInfo memberInfo)
        {
            return !ExpressionReflector.IsEntityPropertyType(GetMemberType(memberInfo));
        }

        /// <summary>
        /// 从指定的类型的特性中获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string GetEntityTableName(Type type)
        {
            var tableAttr = AttributeHelper.GetAttribute<TableAttribute>(type);
            return tableAttr == null ? null : tableAttr.TableName;
        }

        /// <summary>
        /// 获取随机表别名
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomTableAlias()
        {
            return "table" + Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// 该类型是否可以作为实体属性的类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static bool IsEntityPropertyType(Type type)
        {
            return ExpressionReflector.IsEntityPropertyType(type);
        }
    }
}
