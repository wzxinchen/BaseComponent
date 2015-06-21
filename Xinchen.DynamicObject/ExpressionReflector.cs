using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Xinchen.DynamicObject
{
    /// <summary>
    /// 通过表达式树实现类似于反射的功能
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ExpressionReflector<TEntity>
    {
        private static Dictionary<Type, Dictionary<string, Func<TEntity, object>>> _entityGetters;
        private static Dictionary<Type, Dictionary<string, Action<TEntity, object>>> _entitySetters;
        private Type _entityType;
        private static Dictionary<Type, Dictionary<string, Func<object, object>>> _objectGetters;
        private static Dictionary<Type, Dictionary<string, Action<object, object>>> _objectSetters;
        private static Type _objectType;
        private PropertyInfo[] _properties;

        static ExpressionReflector()
        {
            ExpressionReflector<TEntity>._entitySetters = new Dictionary<Type, Dictionary<string, Action<TEntity, object>>>();
            ExpressionReflector<TEntity>._entityGetters = new Dictionary<Type, Dictionary<string, Func<TEntity, object>>>();
            ExpressionReflector<TEntity>._objectGetters = new Dictionary<Type, Dictionary<string, Func<object, object>>>();
            ExpressionReflector<TEntity>._objectSetters = new Dictionary<Type, Dictionary<string, Action<object, object>>>();
            ExpressionReflector<TEntity>._objectType = typeof(object);
        }

        public ExpressionReflector()
        {
            this._entityType = typeof(TEntity);
            this._properties = GetProperties(this._entityType);
        }

        private Dictionary<string, Func<TEntity, object>> GetGetters()
        {
            Dictionary<string, Func<TEntity, object>> dictionary = null;
            if (!_entityGetters.TryGetValue(_entityType, out dictionary))
            {
                lock (_entityGetters)
                {
                    if (!_entityGetters.TryGetValue(_entityType, out dictionary))
                    {
                        dictionary = new Dictionary<string, Func<TEntity, object>>();
                        foreach (PropertyInfo info in this._properties)
                        {
                            ParameterExpression expression = Expression.Parameter(this._entityType);
                            Func<TEntity, object> func = Expression.Lambda<Func<TEntity, object>>(Expression.Convert(Expression.Property(expression, info), _objectType), new ParameterExpression[] { expression }).Compile();
                            dictionary.Add(info.Name, func);
                        }
                        _entityGetters.Add(this._entityType, dictionary);
                    }
                }
            }
            return dictionary;
        }

        private static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance);
        }

        public Dictionary<string, object> GetPropertyValues(TEntity entity)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            Dictionary<string, Func<TEntity, object>> getters = this.GetGetters();
            foreach (KeyValuePair<string, Func<TEntity, object>> pair in getters)
            {
                dictionary.Add(pair.Key, pair.Value(entity));
            }
            return dictionary;
        }

        private Dictionary<string, Action<TEntity, object>> GetSetters()
        {
            Dictionary<string, Action<TEntity, object>> dictionary = null;
            if (!_entitySetters.TryGetValue(_entityType, out dictionary))
            {
                lock (_entitySetters)
                {
                    if (!_entitySetters.TryGetValue(_entityType, out dictionary))
                    {
                        dictionary = new Dictionary<string, Action<TEntity, object>>();
                        foreach (PropertyInfo info in this._properties)
                        {
                            var setMethod = info.GetSetMethod();
                            if (setMethod == null)
                            {
                                continue;
                            }
                            ParameterExpression instance = Expression.Parameter(this._entityType);
                            ParameterExpression expression = Expression.Parameter(_objectType);
                            UnaryExpression expression3 = Expression.Convert(expression, info.PropertyType);
                            Action<TEntity, object> action = Expression.Lambda<Action<TEntity, object>>(Expression.Call(instance, setMethod, new Expression[] { expression3 }), new ParameterExpression[] { instance, expression }).Compile();
                            dictionary.Add(info.Name, action);
                        }
                        _entitySetters.Add(this._entityType, dictionary);
                    }
                }
            }
            return dictionary;
        }

        public object GetValue(TEntity entity, string propertyName)
        {
            var getters = GetGetters();
            Func<TEntity, object> getter = null;
            if (!getters.TryGetValue(propertyName, out getter))
            {
                throw new Exception("Getter未初始化完整");
            }
            return getter(entity);
        }

        public void SetValue(TEntity entity, string propertyName, object value)
        {
            var setters = GetSetters();
            Action<TEntity, object> setter = null;
            if (!setters.TryGetValue(propertyName, out setter))
            {
                throw new Exception("Setter未初始化完整");
            }
            setter(entity, value);
        }

        public PropertyInfo[] Properties
        {
            get
            {
                return this._properties;
            }
        }

        public Dictionary<string, Func<TEntity, object>> GetPropertyGetters()
        {
            return GetGetters();
        }
        public Dictionary<string, Action<TEntity, object>> GetPropertySetters()
        {
            return GetSetters();
        }
    }

    /// <summary>
    /// 通过表达式树实现类似于反射的功能
    /// </summary>
    public class ExpressionReflector
    {
        private static IDictionary<Type, Dictionary<string, Func<object, object>>> _objectGetters = new Dictionary<Type, Dictionary<string, Func<object, object>>>();
        private static IDictionary<Type, Dictionary<string, Action<object, object>>> _objectSetters = new Dictionary<Type, Dictionary<string, Action<object, object>>>();
        //private static IDictionary<Type, Dictionary<string, Action<object, object[]>>> _objectMethods = new Dictionary<Type, Dictionary<string, Action<object, object[]>>>();
        private static Dictionary<Type, Func<object[], object>> _objectConstructors = new Dictionary<Type, Func<object[], object>>();

        public static Dictionary<string, Func<object, object>> GetGetters(Type entityType)
        {
            Dictionary<string, Func<object, object>> dictionary = null;
            if (!_objectGetters.TryGetValue(entityType, out dictionary))
            {
                lock (_objectGetters)
                {
                    if (!_objectGetters.TryGetValue(entityType, out dictionary))
                    {
                        dictionary = new Dictionary<string, Func<object, object>>();
                        foreach (PropertyInfo info in ExpressionReflectorCore.GetProperties(entityType).Values)
                        {
                            ParameterExpression expression = Expression.Parameter(ExpressionReflectorCore.ObjectType);
                            Expression entityExp = Expression.Convert(expression, entityType);
                            Func<object, object> func = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Property(entityExp, info), ExpressionReflectorCore.ObjectType), new ParameterExpression[] { expression }).Compile();
                            dictionary.Add(info.Name, func);
                        }
                        _objectGetters.Add(entityType, dictionary);
                    }
                }
            }
            return dictionary;
        }

        public static Dictionary<string, object> GetPropertyValues(object entity)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            Dictionary<string, Func<object, object>> getters = GetGetters(entity.GetType());
            foreach (KeyValuePair<string, Func<object, object>> pair in getters)
            {
                dictionary.Add(pair.Key, pair.Value(entity));
            }
            return dictionary;
        }

        public static object CreateInstance(Type type, params object[] parameters)
        {
            Func<object[], object> ctorDelegate = null;
            if (!_objectConstructors.TryGetValue(type, out ctorDelegate))
            {
                lock (_objectConstructors)
                {
                    if (!_objectConstructors.TryGetValue(type, out ctorDelegate))
                    {
                        var ctorInfo = type.GetConstructors()[0];
                        var parameterExp = Expression.Parameter(typeof(object[]));
                        List<Expression> expList = new List<Expression>();
                        var parameterList = ctorInfo.GetParameters();
                        for (int i = 0; i < parameterList.Length; i++)
                        {
                            var paramObj = Expression.ArrayIndex(parameterExp, Expression.Constant(i));
                            var expObj = Expression.Convert(paramObj, parameterList[i].ParameterType);
                            expList.Add(expObj);
                        }
                        var newExp = Expression.New(ctorInfo, expList.ToArray());
                        ctorDelegate = Expression.Lambda<Func<object[], object>>(newExp, parameterExp).Compile();
                        _objectConstructors.Add(type, ctorDelegate);
                    }
                }
            }
            return ctorDelegate(parameters);
        }

        public static Dictionary<string, Action<object, object>> GetSetters(Type entityType)
        {
            Dictionary<string, Action<object, object>> dictionary = null;
            if (!_objectSetters.TryGetValue(entityType, out dictionary))
            {
                lock (_objectSetters)
                {
                    if (!_objectSetters.TryGetValue(entityType, out dictionary))
                    {
                        dictionary = new Dictionary<string, Action<object, object>>();
                        foreach (PropertyInfo info in ExpressionReflectorCore.GetProperties(entityType).Values)
                        {
                            var setMethod = info.GetSetMethod();
                            if (setMethod == null)
                            {
                                continue;
                            }
                            ParameterExpression instance = Expression.Parameter(ExpressionReflectorCore.ObjectType);
                            Expression instanceObj = Expression.Convert(instance, entityType);
                            ParameterExpression valueExp = Expression.Parameter(ExpressionReflectorCore.ObjectType);
                            UnaryExpression valueObjExp = Expression.Convert(valueExp, info.PropertyType);
                            Action<object, object> action = Expression.Lambda<Action<object, object>>(Expression.Call(instanceObj, setMethod, new Expression[] { valueObjExp }), new ParameterExpression[] { instance, valueExp }).Compile();
                            dictionary.Add(info.Name, action);
                        }
                        _objectSetters.Add(entityType, dictionary);
                    }
                }
            }
            return dictionary;
        }

        public static object GetValue(object entity, string propertyName)
        {
            var getters = GetGetters(entity.GetType());
            Func<object, object> getter = null;
            if (!getters.TryGetValue(propertyName, out getter))
            {
                throw new Exception("Getter未初始化完整");
            }
            return getter(entity);
        }

        public static void SetValue(object entity, string propertyName, object value)
        {
            var setters = GetSetters(entity.GetType());
            Action<object, object> setter = null;
            if (!setters.TryGetValue(propertyName, out setter))
            {
                throw new Exception("Setter未初始化完整");
            }
            setter(entity, value);
        }

        public static IDictionary<string, PropertyInfo> GetProperties(Type type)
        {
            return ExpressionReflectorCore.GetProperties(type);
        }

        /// <summary>
        /// 只编译出委托，不进行任何缓存
        /// </summary>
        /// <param name="proxyObject"></param>
        /// <param name="methodName"></param>
        /// <param name="argTypes"></param>
        public static Action<object, object[]> GetMethodDelegate(object proxyObject, string methodName, params Type[] argTypes)
        {
            var proxyType = proxyObject.GetType();
            var method = proxyType.GetMethod(methodName, argTypes);
            if (method == null)
            {
                throw new ArgumentException("指定方法未找到");
            }
            ParameterExpression expression = Expression.Parameter(ExpressionReflectorCore.ObjectType);
            UnaryExpression expression2 = Expression.Convert(expression, proxyType);
            ParameterExpression array = Expression.Parameter(typeof(object[]));
            List<Expression> list = new List<Expression>();
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo info2 = parameters[i];
                UnaryExpression item = Expression.Convert(Expression.ArrayIndex(array, Expression.Constant(i)), parameters[i].ParameterType);
                list.Add(item);
            }
            Expression instance = method.IsStatic ? null : Expression.Convert(expression, method.ReflectedType);
            return Expression.Lambda<Action<object, object[]>>(Expression.Call(instance, method, list.ToArray()), new ParameterExpression[] { expression, array }).Compile();
        }

        public static Type GetNullableOrSelfType(Type type)
        {
            Type result = Nullable.GetUnderlyingType(type);
            if (result == null)
            {
                return type;
            }
            return result;
        }

        public static bool IsEntityPropertyType(Type type)
        {
            return ExpressionReflectorCore.EntityPropertyTypes.Contains(type);
        }
    }
}
