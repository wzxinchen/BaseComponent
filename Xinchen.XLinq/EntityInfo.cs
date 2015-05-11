using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xinchen.DynamicObject;

namespace Xinchen.XLinq
{
    /// <summary>
    /// 实体信息类
    /// </summary>
    public class EntityInfo
    {
        public EntityInfo()
        {
            ForeignKeys = new HashSet<PropertyInfo>();
            Properties = new HashSet<PropertyInfo>();
        }

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// 该实体所有的外键
        /// </summary>
        public HashSet<PropertyInfo> ForeignKeys { get; private set; }

        /// <summary>
        /// 实体的表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 该实体
        /// </summary>
        public HashSet<PropertyInfo> Properties { get; private set; }
    }
}
