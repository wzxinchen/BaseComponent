using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xinchen.DynamicObject;

namespace Xinchen.XLinq
{
    public class SelectExpressionVisitor : ExpressionVisitor
    {
        Expression _expression;
        private EntityInfo _masterEntityInfo;
        string _masterTableName;
        private EntityManager _entityManager;
        //List<string> _fields = new List<string>();
        //List<string> _alias = new List<string>();
        public ICollection<PropertyInfo> PropertyInfos { get; private set; }
        public bool NeedWrapSelect { get; private set; }
        /// <summary>
        /// Dictionary《别名,KeyValuePair《表名,列名》》
        /// </summary>
        private Dictionary<string, KeyValuePair<string, string>> _columnInfos;

        /// <summary>
        /// Key为表名，Value为别名
        /// </summary>
        private Dictionary<string, string> _tableInfos;

        /// <summary>
        /// Key为表名，Value为别名
        /// </summary>
        public Dictionary<string, string> TableInfos
        {
            get
            {
                //if (_aliaTableNameMap == null)
                //{
                //    _aliaTableNameMap = new Dictionary<string, string>();
                //    foreach (var column in _columnInfos.Keys)
                //    {
                //        string columnWrap = "[" + column + "]";
                //        string tableName = _columnInfos[column].Key;
                //        if (_aliaTableNameMap.ContainsKey(tableName))
                //        {
                //            continue;
                //        }
                //        _aliaTableNameMap.Add(tableName, columnWrap);
                //    }
                //}
                return _tableInfos;
            }
        }

        private Dictionary<KeyValuePair<string, string>, string> _columnMethods { get; set; }
        /// <summary>
        /// 已包装过[]
        /// </summary>
        public List<string> Alias
        {
            get { return _columnInfos.Keys.ToList(); }
        }

        private List<string> _columns = new List<string>();

        /// <summary>
        /// 已包装过[]
        /// </summary>
        public List<string> Columns
        {
            get
            {
                if (_columns.Count <= 0)
                {
                    foreach (var _column in _columnInfos.Keys)
                    {
                        var columnInfo = _columnInfos[_column];
                        //表名.字段名
                        var column = "[" + columnInfo.Key + "].[" + columnInfo.Value + "]";
                        //查看是否有方法需要转换
                        if (_columnMethods.ContainsKey(columnInfo))
                        {
                            _columns.Add(string.Format(_columnMethods[columnInfo], column) + " as [" + _column + "]");
                        }
                        else
                        {
                            _columns.Add(column + " as [" + _column + "]");
                        }
                    }
                }
                return _columns;
            }
        }

        public void ClearResult()
        {
            _columnMethods.Clear();
            _columnInfos.Clear();
        }

        public SelectExpressionVisitor(EntityInfo masterEntityInfo, Expression expression)
        {
            _masterEntityInfo = masterEntityInfo;
            _masterTableName = masterEntityInfo.TableName;
            _columnMethods = new Dictionary<KeyValuePair<string, string>, string>();
            _expression = expression;
            _columnInfos = new Dictionary<string, KeyValuePair<string, string>>();
            _tableInfos = new Dictionary<string, string>();
            _tableInfos.Add(masterEntityInfo.TableName, masterEntityInfo.TableName);
            _entityManager = new EntityManager();
        }

        Stack<PropertyInfo> _properties = new Stack<PropertyInfo>();

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ParameterExpression)
            {
                //访问到了根节点了
                if (_properties.Count <= 0)
                {
                    _columnInfos.Add(node.Member.Name, new KeyValuePair<string, string>(_masterTableName, node.Member.Name));
                }
                else
                {
                    Type type = node.Type;
                    if (type.IsGenericType)
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }
                    if (!(type.FullName == "System.DateTime" && type.Assembly.GlobalAssemblyCache))
                    {
                        throw new ArgumentException("Select子句中多个属性访问的表达式时，若属性名为Date，则只能支持访问DateTime类型的实体，例如x.Time.Date");
                    }

                    NeedWrapSelect = true;
                    _columnInfos.Add(node.Member.Name, new KeyValuePair<string, string>(_masterTableName, node.Member.Name));
                    _columnMethods.Add(new KeyValuePair<string, string>(_masterTableName, node.Member.Name), "CONVERT(NVARCHAR(20),{0},101)");
                    if (!_tableInfos.ContainsKey(_masterTableName))
                    {
                        _tableInfos.Add(_masterTableName, ExpressionUtil.GenerateRandomTableAlias());
                    }
                    _properties.Clear();
                }
            }
            else
            {
                //不是根节点，取出Member属性用于计算值
                PropertyInfo propertyInfo = (PropertyInfo)node.Member;
                if (propertyInfo.Name == "Value")
                {
                    propertyInfo = (PropertyInfo)((MemberExpression)node.Expression).Member;
                    //跳过可空类型
                    if (!propertyInfo.PropertyType.IsGenericType)
                    {
                        throw new ArgumentException("在Select子句中，若使用了多个属性访问的表达式并且属性名为Value，则该表达式只允许为访问Nullable<T>的表达式，例如如果实体的Time为DateTime?，则可以写为x.Time.Value");
                    }
                    else
                    {
                        return base.VisitMember(node);
                    }
                }
                NeedWrapSelect = true;
                _properties.Push(propertyInfo);
            }
            return base.VisitMember(node);
        }

        /// <summary>
        /// 从指定的参数列表中查找对应的name的表达式的父表达式
        /// </summary>
        /// <param name="args"></param>
        /// <param name="name"></param>
        /// <returns>可能是MemberExpression和ParameterExpression</returns>
        Expression FindArgument(IEnumerable<Expression> args, MemberInfo memberInfo)
        {
            string name = memberInfo.Name;
            Type type = ExpressionUtil.GetMemberType(memberInfo);
            MemberExpression temp = null;
            foreach (Expression _arg in args)
            {
                //Expression target = null;
                if (_arg is ParameterExpression)
                {
                    throw new Exception("Select子句中的匿名类不允许存在直接使用参数的表达式，例如x=>new {x}");
                }
                var arg = (MemberExpression)_arg;
                if (arg.Member.Name == name && ExpressionUtil.GetMemberType(arg.Member) == type)
                {
                    return arg.Expression;
                }
                temp = (MemberExpression)arg.Expression;
                while (true)
                {
                    if(temp.Expression is ParameterExpression)
                    {
                        break;
                    }
                    temp = (MemberExpression)temp.Expression;
                    if (temp.Member.Name == name && ExpressionUtil.GetMemberType(temp.Member) == type)
                    {
                        return temp.Expression;
                    }
                }
            }
            return null;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            //此处一般是在Select方法中new了匿名对象，例如users.Select(x=>new {x.Id})
            foreach (System.Reflection.MemberInfo memberInfo in node.Members)
            {
                //检查当前成员的类型是否是实体类型，例如x=>new {x.User.Book.Id}
                //这种情况一般是外键访问，需要考虑与主表之间的关系

                if (ExpressionUtil.IsEntityMember(memberInfo))
                {
                    //当前成员的类型是一个实体类型，需要考虑与主表的关系
                    var type = ExpressionUtil.GetMemberType(memberInfo);
                    var entityInfo = _entityManager.GetEntity(type);
                    var parentExpression= FindArgument(node.Arguments, memberInfo);
                    if(parentExpression is ParameterExpression)
                    {
                        //该成员由参数直接调用，可直接考虑与主表的关系

                    }
                    string tableName = entityInfo.TableName;
                    //if (string.IsNullOrWhiteSpace(tableName))
                    //{
                    //    throw new Exception("Select子句中选择的类型没有标TableAttribute特性或者特性中指定了空表名");
                    //}

                    var pis = entityInfo.Properties;
                    //_alias.AddRange(pis.Select(x => x.Name));
                    foreach (var propertyInfo in pis)
                    {
                        _columnInfos.Add(propertyInfo.Name, new KeyValuePair<string, string>(tableName, propertyInfo.Name));
                    }
                    if (!_tableInfos.ContainsKey(tableName))
                    {
                        _tableInfos.Add(tableName, ExpressionUtil.GenerateRandomTableAlias());
                    }
                }
                else
                {
                    _columnInfos.Add(memberInfo.Name, new KeyValuePair<string, string>(_masterTableName, memberInfo.Name));
                }
            }
            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.Body is NewExpression)
            {
                //此处一般是在Select方法中new了匿名对象，例如users.Select(x=>new {x.Id})
                return base.VisitLambda<T>(node);
            }

            //此处一般是在Select方法中直接选择了参数，例如users.Select(x=>x)，这种情况下直接把x的所有列转换出来
            var properties = ExpressionReflector.GetProperties(node.Body.Type).Values;
            foreach (var item in properties)
            {

                if (!ExpressionUtil.IsEntityPropertyType(item.PropertyType))
                {
                    continue;
                    ////是否.Net自带的String、DateTime，如果不是则跳过
                    //if (!((item.PropertyType.FullName == "System.String" || item.PropertyType.FullName == "System.DateTime") && item.PropertyType.Assembly.GlobalAssemblyCache))
                    //{
                    //    continue;
                    //}
                    ////是否可空，如果不是则跳过
                    //var type = Nullable.GetUnderlyingType(item.PropertyType);
                    //if (type == null)
                    //{
                    //    continue;
                    //}
                }
                _columnInfos.Add(item.Name, new KeyValuePair<string, string>(_masterTableName, item.Name));
            }
            return node;
        }

        public void Visit()
        {
            _columns.Clear();
            base.Visit(_expression);
        }
    }
}
