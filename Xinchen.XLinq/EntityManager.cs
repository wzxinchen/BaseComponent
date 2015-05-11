using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.DynamicObject;
using Xinchen.Utils;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.XLinq
{
    /// <summary>
    /// 实体信息管理类
    /// </summary>
    public class EntityManager
    {
        static Dictionary<Type, EntityInfo> _entityInfos = new Dictionary<Type, EntityInfo>();
        static object _entityLocker = new object();
        /// <summary>
        /// 通过实体类型获取实体信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public EntityInfo GetEntity(Type type)
        {
            EntityInfo entityInfo = null;

            if (!_entityInfos.TryGetValue(type, out entityInfo))
            {
                lock (_entityLocker)
                {
                    entityInfo = new EntityInfo();
                    entityInfo.TableName = ExpressionUtil.GetEntityTableName(type);
                    if(string.IsNullOrWhiteSpace(entityInfo.TableName))
                    {
                        throw new Exception("实体必须标明TableAttribute");
                    }
                    entityInfo.EntityType = type;
                   
                    var pis = ExpressionReflector.GetProperties(type).Values;
                    foreach (var property in pis)
                    {
                        var foreignKeyAttr = AttributeHelper.GetAttribute<ForeignKeyAttribute>(property);
                        if (foreignKeyAttr != null)
                        {
                            entityInfo.ForeignKeys.Add(property);
                        }
                        entityInfo.Properties.Add(property);
                    }
                    
                    _entityInfos.Add(type, entityInfo);
                }
            }

            return entityInfo;
        }
    }
}
