using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.Utils;
namespace PPD.XLinq.Provider.SqlServer2008R2.Parser
{
    /// <summary>
    /// 对访问成员属性的表达式进行分析
    /// </summary>
    public class PropertyFieldExpressionVisitor : ExpressionVisitorBase
    {
        Dictionary<string, Join> _joins;

        public PropertyFieldExpressionVisitor(TranslateContext context)
            : base(context)
        {
            // TODO: Complete member initialization
            this._joins = context.Joins;
            this._columns = context.Columns;
        }

        public override Expression Visit(Expression node)
        {
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
            MethodCallExpressionVisitor visitor = new MethodCallExpressionVisitor(Context);
            visitor.Visit(node);
            var type = visitor.Token.Type;
            if (type == TokenType.Column)
            {
                Token = visitor.Token;
                var column = Token.Column;
                column.Converter = GetConverter(column.Converter);
            }
            else if (type == TokenType.Condition)
            {
                throw new Exception();
            }
            else
            {
                var obj = visitor.Token.Object;
                while (_memberInfos.Count > 0)
                {
                    obj = GetValue((MemberExpression)_memberInfos.Pop(), obj);
                }
                Token = Token.Create(obj);
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
        private Dictionary<string, Column> _columns;
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
                Token = Token.Create(value);
                return node;
            }
            if (node.Member.DeclaringType == typeof(TimeSpan))
            {
                var unit = string.Empty;
                switch (node.Member.Name)
                {
                    case "TotalDays":
                        unit = "DAY";
                        break;
                    case "TotalHours":
                        unit = "HOUR";
                        break;
                    case "TotalMilliseconds":
                        unit = "MILLISECOND";
                        break;
                    case "TotalMinutes":
                        unit = "MINUTE";
                        break;
                    case "TotalSeconds":
                        unit = "SECOND";
                        break;
                    default:
                        throw new Exception();
                }
                if (node.Expression.NodeType == ExpressionType.Subtract)
                {
                    var binaryExp = (BinaryExpression)node.Expression;
                    var left = binaryExp.Left;
                    var right = binaryExp.Right;
                    var leftVisitor = new MemberExpressionVisitor(Context);
                    leftVisitor.Visit(left);
                    var rightVisitor = new MemberExpressionVisitor(Context);
                    rightVisitor.Visit(right);
                    if (leftVisitor.Token.Type == TokenType.Column && rightVisitor.Token.Type == TokenType.Object)
                    {
                        var rightObject = (DateTime)rightVisitor.Token.Object;
                        leftVisitor.Token.Column.Converter = "DATEDIFF(" + unit + ",@param1,{0})";
                        leftVisitor.Token.Column.ConverterParameters.Add(rightObject);
                        Token = leftVisitor.Token;
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
            Token = Token.Create(value);
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
            if (TableInfoManager.IsEntity(node.Type))
            {
                //弹出第一个参数，一般是列
                var table = GetTable(node.Type);
                var _memberInfo = ((MemberExpression)_memberInfos.Pop()).Member;
                column.DataType = ((PropertyInfo)_memberInfo).PropertyType;
                column.Name = table.Columns.Get(_memberInfo.Name).Name;
                column.MemberInfo = _memberInfo;
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
                SchemaModel.Table tableInfo = null;
                MemberInfo columnMember = null;
                var tableAlias = string.Empty;
                Table table = null;
                if (_memberInfos.Count > 1)
                {
                    tableInfo = GetTable();
                    tableAlias = _tableMember.Name;
                    columnMember = ((MemberExpression)_memberInfos.Pop()).Member;
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
                    table = CreateTable(tableAlias, tableInfo.DataBase, tableInfo.Name, tableInfo.Type);
                }
                else
                {
                    columnMember = ((MemberExpression)_memberInfos.Pop()).Member;
                    table = _columns.Get(columnMember.Name).Table;
                    tableAlias = table.Alias;
                    tableInfo = GetTable(table.Type);
                }
                var columnType = ((PropertyInfo)columnMember).PropertyType;
                column = new Column()
                {
                    DataType = columnType,
                    Name = tableInfo.Columns.Get(columnMember.Name).Name,
                    Table = table,
                    MemberInfo = columnMember
                };
                column.Converter = GetConverter(null);
            }
            Token = Token.Create(column);
            return node;
        }
    }
}
