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
    public class SelectExpressionVisitor : ExpressionVisitor
    {
        IReadOnlyCollection<MemberInfo> _aliaMembers;
        IReadOnlyCollection<Expression> _columnParams;
        private Type _elementType;
        private Dictionary<string, Join> _Joins;
        public List<Column> Columns { get; private set; }

        public SelectExpressionVisitor(Type ElementType, Dictionary<string, Join> Joins)
        {
            // TODO: Complete member initialization
            this._elementType = ElementType;
            this._Joins = Joins;
            Columns = new List<Column>();
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
            {
                var properties = _elementType.GetProperties();
                var firstProperty = properties.FirstOrDefault();
                if (firstProperty == null)
                {
                    throw new Exception("实体类没有字段");
                }
                var tableSechma = TableInfoManager.GetTable(firstProperty.DeclaringType);
                var tableInfo = new Table()
                {
                    DataBase = tableSechma.DataBase,
                    Name = tableSechma.Name,
                    Type = tableSechma.Type
                };
                foreach (var property in properties)
                {
                    var column = new Column();
                    column.Name = property.Name;
                    column.DataType = property.PropertyType;
                    column.Table = tableInfo;
                    Columns.Add(column);
                }
                return node;
            }
            return base.Visit(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var visitor = new MemberExpressionVisitor(_Joins);
                var arg = node.Arguments[i];
                var member = node.Members[i];
                visitor.Visit(arg);
                visitor.SelectedColumn.Alias = member.Name;
                Columns.Add(visitor.SelectedColumn);
            }
            return base.VisitNew(node);
        }

        class MemberExpressionVisitor : ExpressionVisitor
        {
            public Column SelectedColumn { get; private set; }
            public object Value { get; set; }
            public MemberExpressionType MemberExpressionType { get; private set; }
            Stack<MemberInfo> _memberInfoStack = new Stack<MemberInfo>();
            private MemberInfo _tableMember;
            private Dictionary<string, Join> _Joins;

            public MemberExpressionVisitor(Dictionary<string, Join> _Joins)
            {
                // TODO: Complete member initialization
                this._Joins = _Joins;
                SelectedColumn = new Column();
            }
            public object GetValue(MemberInfo memberInfo, object obj)
            {
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    return ((FieldInfo)memberInfo).GetValue(obj);
                }
                return ((PropertyInfo)memberInfo).GetValue(obj);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                _memberInfoStack.Push(node.Member);
                if (node.Expression == null)
                {
                    //静态成员调用
                    throw new NotSupportedException("Select子句中不允许调用方法");
                }
                else if (node.Expression.NodeType == ExpressionType.Constant)
                {
                    //实例成员调用
                    MemberExpressionType = MemberExpressionType.Object;
                    return node;
                }
                return base.VisitMember(node);
            }

            SchemaModel.Table GetTable()
            {
                _tableMember = _memberInfoStack.Pop();
                var tableType = ((PropertyInfo)_tableMember).PropertyType;
                return GetTable(tableType);
            }

            SchemaModel.Table GetTable(Type tableType)
            {
                var tableInfo = TableInfoManager.GetTable(tableType);
                if (ParserUtils.IsAnonymousType(tableType))
                {
                    _tableMember = _memberInfoStack.Pop();
                    tableType = ((PropertyInfo)_tableMember).PropertyType;
                    return GetTable(tableType);
                }
                return tableInfo;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                throw new NotSupportedException("Select子句中不允许直接写常量");
            }

            string GetConverter()
            {
                if (_memberInfoStack.Count > 0)
                {
                    var propertyInfo = (PropertyInfo)_memberInfoStack.Pop();

                    if (propertyInfo.DeclaringType.FullName.StartsWith("System.Nullable") && propertyInfo.DeclaringType.Assembly.GlobalAssemblyCache)
                    {
                        //可空属性的Value访问，忽略不管
                        if (_memberInfoStack.Count <= 0)
                        {
                            return null;
                        }
                        propertyInfo = (PropertyInfo)_memberInfoStack.Pop();
                    }
                    if (propertyInfo.DeclaringType != typeof(DateTime) || propertyInfo.Name != "Date")
                    {
                        throw new Exception("除x=>x.Time.Date以外，不支持其他的");
                    }
                    return "CONVERT(DATE,{0},211)";
                }
                return null;
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
                MemberExpressionType = MemberExpressionType.Column;
                if (TableInfoManager.IsEntity(node.Type))
                {
                    //弹出第一个参数，一般是列
                    var _memberInfo = _memberInfoStack.Pop();
                    SelectedColumn.DataType = ((PropertyInfo)_memberInfo).PropertyType;
                    SelectedColumn.Name = _memberInfo.Name;
                    var table = GetTable(node.Type);
                    var tableAlias = node.Name;
                    if (_Joins != null)
                    {
                        if (_Joins.ContainsKey(tableAlias))
                        {
                            tableAlias = _Joins[tableAlias].Right.Table.Alias;
                        }
                    }
                    else
                    {
                        tableAlias = table.Name;
                    }
                    SelectedColumn.Table = CreateTable(tableAlias, table.DataBase, table.Name, table.Type);
                    SelectedColumn.Converter = GetConverter();
                }
                else
                {
                    var tableInfo = GetTable();
                    var columnMember = _memberInfoStack.Pop();
                    var columnType = ((PropertyInfo)columnMember).PropertyType;
                    var tableAlias = _tableMember.Name;
                    if (_Joins != null)
                    {
                        if (_Joins.ContainsKey(tableAlias))
                        {
                            tableAlias = _Joins[tableAlias].Right.Table.Alias;
                        }
                    }
                    else
                    {
                        tableAlias = tableInfo.Name;
                    }
                    var table = CreateTable(tableAlias, tableInfo.DataBase, tableInfo.Name, tableInfo.Type);
                    SelectedColumn = new Column()
                    {
                        DataType = columnType,
                        Name = columnMember.Name,
                        Table = table
                    };
                    SelectedColumn.Converter = GetConverter();
                }
                return node;
            }
        }
    }
}
