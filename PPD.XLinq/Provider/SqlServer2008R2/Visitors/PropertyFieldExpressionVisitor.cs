using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    /// <summary>
    /// 对访问成员属性的表达式进行分析
    /// </summary>
    public class PropertyFieldExpressionVisitor : ExpressionVisitorBase
    {
        Dictionary<string, Join> _joins;
        /// <summary>
        /// 参数可传null，该参数主要用于创建该表达式中可能出现的对Entity的访问
        /// </summary>
        /// <param name="joins"></param>
        public PropertyFieldExpressionVisitor(Dictionary<string, Join> joins)
        {
            this._joins = joins;
        }

        public override Expression Visit(Expression node)
        {
            Type = MemberExpressionType.None;
            var sepcNode = node;
            if (sepcNode.NodeType == ExpressionType.Quote)
            {
                sepcNode = ((UnaryExpression)sepcNode).Operand;
            }
            if (sepcNode.NodeType == ExpressionType.Lambda)
            {
                return base.Visit(((LambdaExpression)sepcNode).Body);
            }
            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor(_joins);
            visitor.Visit(node);
            Type = visitor.Type;
            if (Type == MemberExpressionType.Column)
            {
                var token = (Token)visitor.Result;
                var column = token.Column;
                column.Converter = GetConverter(column.Converter);
                Result = Token.Create(column);
            }
            else
            {
                Result = visitor.Result;
                while (_memberInfos.Count > 0)
                {
                    Result = GetValue((MemberExpression)_memberInfos.Pop(), Result);
                }
            }
            return node;
        }

        public object GetValue(MemberExpression memberExpression, object obj)
        {
            switch (memberExpression.NodeType)
            {
                case ExpressionType.MemberAccess:

                    if (memberExpression.Member.MemberType == MemberTypes.Field)
                    {
                        return ((FieldInfo)memberExpression.Member).GetValue(obj);
                    }
                    return ((PropertyInfo)memberExpression.Member).GetValue(obj);
            }
            throw new Exception();
        }

        Stack<Expression> _memberInfos = new Stack<Expression>();
        private MemberInfo _tableMember;
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == null)
            {
                //静态属性访问，已到根节点
                var value = GetValue(node, null);
                while (_memberInfos.Count > 0)
                {
                    value = GetValue((MemberExpression)_memberInfos.Pop(), value);
                }
                Result = value;
                Type = MemberExpressionType.Object;
                return node;
            }
            if (node.Member.DeclaringType == typeof(TimeSpan))
            {
                MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor(_joins);
                visitor.Visit(node);
                if (node.Expression.NodeType == ExpressionType.Subtract)
                {
                    var binaryExp = (BinaryExpression)node.Expression;
                    var left = binaryExp.Left;
                    var right = binaryExp.Right;
                    var leftVisitor = new MemberExpressionVisitor(_joins);
                    leftVisitor.Visit(left);
                    var rightVisitor = new MemberExpressionVisitor(_joins);
                    rightVisitor.Visit(right);
                    if (leftVisitor.Type == MemberExpressionType.Column && rightVisitor.Type == MemberExpressionType.Object)
                    {
                        Token token = null;
                        var leftToken = (Token)leftVisitor.Result;
                        var rightToken = (Token)rightVisitor.Result;
                        if (leftToken.Type == TokenType.Column && rightToken.Type == TokenType.Object)
                        {
                            var column = new Column()
                            {
                                Converter = "DATEDIFF(DAY,@param1,{0})"
                            };

                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
                return node;
            }
            _memberInfos.Push(node);
            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var value = node.Value;
            while (_memberInfos.Count > 0)
            {
                value = GetValue((MemberExpression)_memberInfos.Pop(), value);
            }
            Result = value;
            Type = MemberExpressionType.Object;
            return base.VisitConstant(node);
        }

        SchemaModel.Table GetTable()
        {
            _tableMember = ((MemberExpression)_memberInfos.Pop()).Member;
            var tableType = ((PropertyInfo)_tableMember).PropertyType;
            return GetTable(tableType);
        }

        SchemaModel.Table GetTable(Type tableType)
        {
            var tableInfo = TableInfoManager.GetTable(tableType);
            if (ParserUtils.IsAnonymousType(tableType))
            {
                _tableMember = ((MemberExpression)_memberInfos.Pop()).Member;
                tableType = ((PropertyInfo)_tableMember).PropertyType;
                return GetTable(tableType);
            }
            return tableInfo;
        }

        string GetConverter(string converter)
        {
            if (_memberInfos.Count > 0 && string.IsNullOrWhiteSpace(converter))
            {
                converter = "{0}";
            }
            while (_memberInfos.Count > 0)
            {
                var exp = _memberInfos.Pop();
                switch (exp.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        var memberInfo = ((MemberExpression)exp).Member;
                        if (memberInfo.MemberType == MemberTypes.Field)
                        {
                            throw new Exception();
                        }
                        else if (memberInfo.MemberType == MemberTypes.Property)
                        {
                            var propertyInfo = (PropertyInfo)memberInfo;

                            if (propertyInfo.DeclaringType.IsGenericType && propertyInfo.DeclaringType.GetGenericTypeDefinition() == typeof(Nullable<>) && propertyInfo.Name == "Value")
                            {
                                //可空属性的Value访问，忽略不管
                                continue;
                            }

                            if ((propertyInfo.DeclaringType != typeof(DateTime) && propertyInfo.DeclaringType != typeof(DateTime?)) || propertyInfo.Name != "Date")
                            {
                                throw new Exception("除x=>x.Time.Date以外，不支持其他的");
                            }
                            converter = string.Format(converter, "CONVERT(DATE,{0},211)");
                        }
                        else
                        {
                            throw new Exception();
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }
            return converter;
        }

        Table CreateTable(string alias, string db, string name, Type type)
        {
            return new Table()
            {
                Alias = alias,
                DataBase = db,
                Name = name,
                Type = type
            };
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            var column = new Column();
            Type = MemberExpressionType.Column;
            if (TableInfoManager.IsEntity(node.Type))
            {
                //弹出第一个参数，一般是列
                var _memberInfo = ((MemberExpression)_memberInfos.Pop()).Member;
                column.DataType = ((PropertyInfo)_memberInfo).PropertyType;
                column.Name = _memberInfo.Name;
                var table = GetTable(node.Type);
                var tableAlias = node.Name;
                if (_joins != null)
                {
                    if (_joins.ContainsKey(tableAlias))
                    {
                        tableAlias = _joins[tableAlias].Right.Table.Alias;
                    }
                }
                else
                {
                    tableAlias = table.Name;
                }
                column.Table = CreateTable(tableAlias, table.DataBase, table.Name, table.Type);
                column.Converter = GetConverter(null);
            }
            else
            {
                var tableInfo = GetTable();
                var columnMember = ((MemberExpression)_memberInfos.Pop()).Member;
                var columnType = ((PropertyInfo)columnMember).PropertyType;
                var tableAlias = _tableMember.Name;
                if (_joins != null)
                {
                    if (_joins.ContainsKey(tableAlias))
                    {
                        tableAlias = _joins[tableAlias].Right.Table.Alias;
                    }
                }
                else
                {
                    tableAlias = tableInfo.Name;
                }
                var table = CreateTable(tableAlias, tableInfo.DataBase, tableInfo.Name, tableInfo.Type);
                column = new Column()
                {
                    DataType = columnType,
                    Name = columnMember.Name,
                    Table = table
                };
                column.Converter = GetConverter(null);
            }
            Result = Token.Create(column);
            return node;
        }
    }
}
