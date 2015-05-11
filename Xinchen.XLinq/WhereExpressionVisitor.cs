using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Xinchen.DynamicObject;

namespace Xinchen.XLinq
{
    public class WhereExpressionVisitor : ExpressionVisitor
    {
        private System.Linq.Expressions.Expression expression;
        private Stack<string> _conditionStack = new Stack<string>();
        //private Stack<PropertyInfo> _propertyStack = new Stack<PropertyInfo>();
        private Dictionary<string, object> _list = new Dictionary<string, object>();
        private Stack<object> _constStack = new Stack<object>();
        private Stack<string> _paramNameStack = new Stack<string>();
        private Stack<object> _tempConst = new Stack<object>();
        private Stack<MemberInfo> _memberStack = new Stack<MemberInfo>();
        public Stack<string> ParamNameStack
        {
            get { return _paramNameStack; }
        }
        /// <summary>
        /// Key为表名，Value为别名
        /// </summary>
        Dictionary<string, string> _aliaTableNameMap;
        string _condition;
        private string field;
        public WhereExpressionVisitor(string firstTableName,Dictionary<string, string> aliaTableNameMap, Expression expression)
        {
            this.expression = expression;
            this._aliaTableNameMap = aliaTableNameMap;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not && node.Operand is MemberExpression)
            {
                return base.VisitUnary(node);
            }
            var exp = base.VisitUnary(node);
            if (node.NodeType == ExpressionType.Not)
            {
                string condition = _conditionStack.Pop();
                _conditionStack.Push("(NOT " + condition + ")");
            }
            else if (node.NodeType == ExpressionType.Convert)
            {
                if (node.Operand is ConstantExpression)
                {
                    _constStack.Push((node.Operand as ConstantExpression).Value);
                }
            }
            else
            {
                throw new Exception("未支持一元操作符：" + node.NodeType.ToString());
            }
            return exp;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            _memberStack.Push(node.Method);
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            string left, right, op, condition;
            Match match;
            Visit(node.Left);
            bool singleBool = false;
            if (node.Left.NodeType == ExpressionType.MemberAccess)
            {
                var leftExp = node.Left as MemberExpression;
                if (ExpressionReflector.GetNullableOrSelfType(leftExp.Type).FullName == "System.Boolean" && leftExp.Type.Assembly.GlobalAssemblyCache)
                {
                    //x.IsEnabled这种情况
                    if (node.Right.NodeType == ExpressionType.Constant)
                    {
                        //x.IsEnabled=true

                    }
                    else if (_conditionStack.Count > 0)
                    {
                        //x.IsEnabled && ......
                        _list.Add(_conditionStack.Peek(), true);//.Push("[" + leftExp.Member.Name + "]");
                        singleBool = true;
                    }
                }
            }
            if (node.Left.NodeType == ExpressionType.Not)
            {
                var unaryExp = node.Left as UnaryExpression;
                if (unaryExp.Operand.NodeType == ExpressionType.MemberAccess)
                {
                    var leftExp = unaryExp.Operand as MemberExpression;
                    if (leftExp.Type.FullName == "System.Boolean" && leftExp.Type.Assembly.GlobalAssemblyCache)
                    {
                        //x.IsEnabled这种情况
                        if (node.Right.NodeType == ExpressionType.Constant)
                        {
                            //x.IsEnabled=true

                        }
                        else if (_conditionStack.Count > 0)
                        {
                            //x.IsEnabled && ......
                            _list.Add(_conditionStack.Peek(), false);//.Push("[" + leftExp.Member.Name + "]");
                            singleBool = true;
                        }
                        else
                        {
                            bool value = (bool)_constStack.Pop();
                            if (value)
                            {
                                _conditionStack.Push("1=1");
                            }
                            else
                            {
                                _conditionStack.Push("1=2");
                            }
                        }
                    }
                }
            }
            //此时已获取到了值，一般情况下访问右支就是为了找值，所以直接组合
            if (singleBool)
            {
                right = _conditionStack.Pop();
                left = _conditionStack.Pop();
                op = "=";
                condition = string.Format("({0} {1} {2})", left, op, right);
                _conditionStack.Push(condition);
            }
            if (node.NodeType == ExpressionType.ArrayIndex)
            {
                //处理数组访问，此时right存的是index，所以跳过Right访问
                var constExp = node.Right as ConstantExpression;
                var index = Convert.ToInt32(constExp.Value);
                var arrayObject = _constStack.Pop() as Array;
                object arrayValue = arrayObject.GetValue(index);
                while (_memberStack.Count > 0)
                {
                    var member = _memberStack.Pop();
                    if (member.MemberType == MemberTypes.Field)
                    {
                        arrayValue = ((FieldInfo)member).GetValue(arrayValue);
                    }
                    else if (member.MemberType == MemberTypes.Property)
                    {
                        arrayValue = ((PropertyInfo)member).GetValue(arrayValue, null);
                    }
                    else
                    {
                        throw new NotSupportedException("未知的成员类型：" + member.MemberType);
                    }
                }
                _list.Add(_conditionStack.Peek(), arrayValue);
                return node;
            }
            else
            {
                if (node.Right is ConstantExpression)
                {
                    var constExp = node.Right as ConstantExpression;
                    _list.Add(_conditionStack.Peek(), constExp.Value);
                }
                else
                {
                    Visit(node.Right);
                }
            }
            if (_constStack.Count > 0)
            {
                //常量计算
                object c1 = _constStack.Pop();
                object c2 = _constStack.Pop();
                bool c3 = false;
                switch (node.NodeType)
                {
                    case ExpressionType.Equal:
                        c3 = c1 == c2;
                        break;
                    case ExpressionType.GreaterThan:
                        if (c1 == null && c2 == null)
                        {
                            c3 = true;
                        }
                        else
                        {
                            c3 = false;
                        }
                        break;
                }
                if (c3)
                {
                    _conditionStack.Push("1=1");
                }
                else
                {
                    _conditionStack.Push("1=2");
                }
            }
            else
            {
                right = _conditionStack.Pop();
                left = _conditionStack.Pop();
                op = string.Empty;
                switch (node.NodeType)
                {
                    case ExpressionType.AndAlso:
                        op = "and";
                        break;
                    case ExpressionType.OrElse:
                        op = "or";
                        break;
                    case ExpressionType.Equal:
                        op = "=";
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        op = ">=";
                        break;
                    case ExpressionType.GreaterThan:
                        op = ">";
                        break;
                    case ExpressionType.LessThan:
                        op = "<";
                        break;
                    case ExpressionType.LessThanOrEqual:
                        op = "<=";
                        break;
                    case ExpressionType.ArrayIndex:
                        op = "=";
                        break;
                    default:
                        throw new NotImplementedException("未实现逻辑关系:" + node.NodeType);
                }
                var lastParameter = _list.LastOrDefault();
                if (lastParameter.Value == null)
                {
                    right = "null";
                    _list.Remove(lastParameter.Key);
                }
                condition = string.Format("({0} {1} {2})", left, op, right);
                match = Regex.Match(condition, @"(.*)? = null");
                if (match.Success || (match = Regex.Match(condition, "null = (.*)")).Success)
                {
                    _conditionStack.Push(match.Groups[1].Value + " is null)");
                }
                else
                {
                    _conditionStack.Push(condition);
                }
            }
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is ParameterExpression)
            {
                //到达根节点，并且一定是左枝
                if (ExpressionUtil.IsEntityMember(node.Member))
                {
                    //x.User.Book.Id
                    string tableName = ExpressionUtil.GetEntityTableName(ExpressionUtil.GetMemberType(node.Member));
                    if (string.IsNullOrWhiteSpace(tableName))
                    {
                        throw new Exception("解析出错");
                    }
                    string alia = _aliaTableNameMap[tableName];
                    field = _memberStack.Pop().Name;
                    _conditionStack.Push("[" + alia + "].[" + field + "]");
                    _conditionStack.Push("@" + field);
                }
                else
                {
                    //按照属性处理
                    field = node.Member.Name;
                    if (_memberStack.Count > 0)
                    {
                        //有属性要转换为sql语句
                        var propertyInfo = (PropertyInfo)_memberStack.Pop();
                        if (propertyInfo.PropertyType.FullName == "System.DateTime" && propertyInfo.PropertyType.Assembly.GlobalAssemblyCache)
                        {
                            //该属性为日期时间型，可进行转换
                            _conditionStack.Push("CONVERT(NVARCHAR(20),[" + field + "],101)");
                        }
                        else
                        {
                            throw new NotImplementedException("不允许使用当前属性取值：" + propertyInfo.PropertyType.Name + "，" + propertyInfo.Name);
                        }
                    }
                    else if (_memberStack.Count > 0)
                    {
                        //有方法要转换为sql语句
                        var method = _memberStack.Pop();
                        if (method.Name == "ToDateTime" && method.DeclaringType.Assembly.GlobalAssemblyCache)
                        {
                            _conditionStack.Push("CONVERT(NVARCHAR(20),[" + field + "],101)");
                        }
                        else
                        {
                            throw new NotSupportedException("不支持指定方法的转换：" + method.Name);
                        }
                    }
                    else
                    {
                        _conditionStack.Push("[" + field + "]");
                    }
                    _conditionStack.Push("@" + field);
                }
            }
            else if (node.Expression == null)
            {
                //一定是静态成员，例如：x=>DateTime.Now中的Now，直接取出值
                object value = null;
                if (node.Member.MemberType == MemberTypes.Field)
                {
                    var fieldInfo = node.Member as FieldInfo;
                    value = fieldInfo.GetValue(null);
                }
                else
                {
                    var propertyInfo = node.Member as PropertyInfo;
                    value = propertyInfo.GetValue(null, null);
                }
                while (_memberStack.Count > 0)
                {
                    var member = _memberStack.Pop();
                    if (member.MemberType == MemberTypes.Field)
                    {
                        value = ((FieldInfo)member).GetValue(value);
                    }
                    else if (member.MemberType == MemberTypes.Property)
                    {
                        value = ((PropertyInfo)member).GetValue(value, null);
                    }
                    else
                    {
                        throw new NotSupportedException("未知的成员类型：" + member.MemberType);
                    }
                }
                //if (node.Member.MemberType == MemberTypes.Field)
                //{
                //    var fieldInfo = node.Member as FieldInfo;
                //    _list.Add(fieldInfo.GetValue(null));
                //}
                //else
                //{
                //    var propertyInfo = node.Member as PropertyInfo;
                //    _list.Add(propertyInfo.GetValue(null, null));
                //}
                _list.Add(_conditionStack.Peek(), value);
            }
            else if (node.Expression is ConstantExpression)
            {
                //引用的局部变量
                //var constExp = node.Expression as ConstantExpression;
                //var field = constExp.Value.GetType().GetField(node.Member.Name);
                //var rootValue = field.GetValue(constExp.Value);
                //while (_propertyStack.Count > 0)
                //{
                //    var property = _propertyStack.Pop();
                //    rootValue = property.GetValue(rootValue, null);
                //}
                //_paramStack.Push(rootValue);
                ObjectValueExpressionVisitor visitor = new ObjectValueExpressionVisitor();
                visitor.Visit(node);
                if (visitor.Value is Array || visitor.Value is IEnumerable)
                {
                    var rootValue = visitor.Value;
                    var list = rootValue as IList;
                    object result = rootValue;
                    while (_memberStack.Count > 0)
                    {
                        var memberInfo = _memberStack.Pop();
                        if (memberInfo.MemberType == MemberTypes.Method)
                        {
                            var method = memberInfo as MethodInfo;
                            IEnumerable<object> args = null;
                            //if (method.Arguments[0] is ConstantExpression)
                            //{
                            //    args = mce.Arguments.Select(x => ((ConstantExpression)x).Value);
                            //    result = mce.Method.Invoke(rootValue, args.ToArray());
                            //}
                            //else
                            //{
                            //    visitor = new ObjectValueExpressionVisitor();
                            //    visitor.Visit(mce.Arguments[0]);
                            //    var extensionArgs = mce.Arguments.Skip(1).Select(x => ((ConstantExpression)x).Value).ToList();
                            //    extensionArgs.Insert(0, visitor.Value);
                            //    result = mce.Method.Invoke(null, extensionArgs.ToArray());
                            //}
                        }
                    }
                    if (_memberStack.Count > 0)
                    {
                        //MethodCallExpression mce = _memberStack.Pop();
                        //IEnumerable<object> args = null;
                        //if (mce.Arguments[0] is ConstantExpression)
                        //{
                        //    args = mce.Arguments.Select(x => ((ConstantExpression)x).Value);
                        //    result = mce.Method.Invoke(rootValue, args.ToArray());
                        //}
                        //else
                        //{
                        //    visitor = new ObjectValueExpressionVisitor();
                        //    visitor.Visit(mce.Arguments[0]);
                        //    var extensionArgs = mce.Arguments.Skip(1).Select(x => ((ConstantExpression)x).Value).ToList();
                        //    extensionArgs.Insert(0, visitor.Value);
                        //    result = mce.Method.Invoke(null, extensionArgs.ToArray());
                        //}
                    }

                    _list.Add(_conditionStack.Peek(), result);
                    //_constStack.Push(rootValue);
                }
                else
                {
                    _constStack.Push(visitor.Value);
                }
                _paramNameStack.Push(node.Member.Name);
            }
            else if (node.Expression is MemberExpression)
            {
                if (!ExpressionReflector.IsEntityPropertyType(ExpressionUtil.GetMemberType(((MemberExpression)node.Expression).Member)))
                {
                    //x=>x.User.Book.Id
                    _memberStack.Push(node.Member);
                }
                else
                {
                    //var a=false;x=>a这种情况
                }
            }
            else
            {
                _memberStack.Push(node.Member);
            }
            return base.VisitMember(node);
        }

        //protected override Expression VisitConstant(ConstantExpression node)
        //{
        //    //引用的局部变量
        //    object rootValue = node.Value;
        //    if (rootValue is Array)
        //    {
        //        _constStack.Push(rootValue);
        //    }
        //    else if (_paramNameStack.Count > 0)
        //    {
        //        string memnerName = _paramNameStack.Pop();
        //        var field = node.Value.GetType().GetField(memnerName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public);//.GetField(node.Member.Name);
        //        rootValue = field.GetValue(rootValue);
        //        if (rootValue is Array)
        //        {
        //            _constStack.Push(rootValue);
        //            return base.VisitConstant(node);
        //        }
        //        else if (rootValue is IEnumerable)
        //        {
        //            //var list = rootValue as IList;
        //            //if (_methodStack.Count > 0)
        //            //{
        //            //    MethodCallExpression mce = _methodStack.Peek();
        //            //    IEnumerable<object> args = null;
        //            //    object result = null;
        //            //    if (mce.Arguments[0] is ConstantExpression)
        //            //    {
        //            //        args = mce.Arguments.Select(x => ((ConstantExpression)x).Value);
        //            //        result = mce.Method.Invoke(rootValue, args.ToArray());
        //            //    }
        //            //    else
        //            //    {
        //            //        ObjectValueExpressionVisitor visitor = new ObjectValueExpressionVisitor();
        //            //        visitor.Visit(mce.Arguments[0]);
        //            //        var extensionArgs = mce.Arguments.Skip(1).Select(x => ((ConstantExpression)x).Value).ToList();
        //            //        extensionArgs.Insert(0, visitor.Value);
        //            //        result = mce.Method.Invoke(null, extensionArgs.ToArray());
        //            //    }
        //            //    _list.Add(_conditionStack.Peek(), result);
        //            //    return base.VisitConstant(node);
        //            //}
        //            //_constStack.Push(rootValue);
        //            //return base.VisitConstant(node);
        //        }
        //        else
        //        {
        //            while (_propertyStack.Count > 0)
        //            {
        //                var property = _propertyStack.Pop();
        //                rootValue = property.GetValue(rootValue, null);
        //            }
        //        }
        //    }
        //    //if (_conditionStack.Count() <= 0)
        //    //{
        //    //    //直接使用了一个常量表达式，例如var a=true;x=>a==false或x=>a，这种情况计算出结果
        //    //    //bool value = (bool)rootValue;
        //    //    //_constStack.Push(value);

        //    //}
        //    //else
        //    //{
        //    //    //_list.Add(_conditionStack.Peek(), rootValue);
        //    //}
        //    return base.VisitConstant(node);
        //}

        internal void Visit()
        {
            Visit(expression);
            if (_conditionStack.Count <= 0)
            {
                var value = (bool)_constStack.Pop();
                if (value)
                {
                    _conditionStack.Push(string.Empty);
                }
                else
                {
                    _conditionStack.Push("1=2");
                }
            }
        }

        public string Condition
        {
            get
            {
                return _conditionStack.Pop();
            }
        }

        public Dictionary<string, object> Parameters
        {
            get
            {
                return _list;
            }
        }
    }
}
