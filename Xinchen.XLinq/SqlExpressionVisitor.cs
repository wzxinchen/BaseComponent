using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xinchen.DbUtils;
using Xinchen.DynamicObject;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.XLinq
{
    public class SqlExpressionVisitor : ExpressionVisitor
    {
        Type _tableAttrType = typeof(TableAttribute);
        private Expression expression;
        EntityManager _entityManager;
        public string CommandText { get; private set; }
        public Dictionary<string, object> Parameters { get; private set; }
        private Dictionary<string, Expression> _expressionDict = new Dictionary<string, Expression>();

        public SqlExpressionVisitor(Expression expression)
        {
            // TODO: Complete member initialization
            this.expression = expression;
            _entityManager = new EntityManager();
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "Select":
                case "Where":
                case "OrderBy":
                    if (_expressionDict.ContainsKey(node.Method.Name))
                    {
                        _expressionDict[node.Method.Name] = ((UnaryExpression)node.Arguments[1]).Operand;
                    }
                    else
                    {
                        _expressionDict.Add(node.Method.Name, ((UnaryExpression)node.Arguments[1]).Operand);
                    }
                    //selectVisitor = new SelectExpressionVisitor(((UnaryExpression)node.Arguments[1]).Operand);
                    //selectVisitor.Visit();
                    //_columns = string.Join(",", selectVisitor.Columns);
                    break;
                case "get_Item":
                case "FirstOrDefault":
                case "LastOrDefault":
                case "ToString":
                case "ToDateTime":
                    return base.VisitMethodCall(node);
                case "Join":
                    {

                    }
                    break;
                default:
                    throw new NotImplementedException("指定Linq方法暂未支持：" + node.Method.Name);
                //case "Where":
                //    whereVisitor = new WhereExpressionVisitor(selectVisitor.Alias, ((UnaryExpression)node.Arguments[1]).Operand);
                //    whereVisitor.Visit();
                //    break;
            }
            return base.VisitMethodCall(node);
        }

        internal void Visit()
        {
            Visit(expression);
            //获取表名
            Type elementType = null;
            MethodCallExpression call = expression as MethodCallExpression;
            while (true)
            {
                var exp = call.Arguments[0];
                if (exp is MethodCallExpression)
                {
                    call = exp as MethodCallExpression;
                    continue;
                }
                if (exp is ConstantExpression)
                {
                    var constExp = exp as ConstantExpression;
                    elementType = constExp.Type.GetGenericArguments()[0];
                    break;
                }
                throw new Exception("获取表达式树表名的时候出错");
            }
            var entityInfo = _entityManager.GetEntity(elementType);
            //var tableAttr = (TableAttribute)elementType.GetCustomAttributes(_tableAttrType, true).FirstOrDefault();
            //string tableName = elementType.Name + "s";
            //if (tableAttr != null)
            //{
            //    tableName = tableAttr.TableName;
            //}
            StringBuilder sb = new StringBuilder("select ");
            Dictionary<string, string> aliaTableNameMap = new Dictionary<string, string>();
            //先执行Select，必须执行
            if (_expressionDict.ContainsKey("Select"))
            {
                selectorVisitor = new SelectExpressionVisitor(entityInfo, _expressionDict["Select"]);
                selectorVisitor.Visit();
                _columns = string.Join(",", selectorVisitor.Columns);
                _alias = selectorVisitor.Alias;
                aliaTableNameMap = selectorVisitor.TableInfos;
                selectorVisitor.ClearResult();
            }
            else
            {
                var type = expression.Type.GetGenericArguments()[0];
                var pis = ExpressionReflector.GetProperties(type).Values;
                _alias = pis.Select(x => x.Name).ToList();
                _columns = string.Join(",", _alias.Select(x => "[" + x + "]"));
                aliaTableNameMap.Add(entityInfo.TableName, entityInfo.TableName);
            }
            //再执行Where，可以没有
            //Parameters = new Dictionary<string, object>();
            //if (_expressionDict.ContainsKey("Where"))
            //{
            //    var whereVisitor = new WhereExpressionVisitor(aliaTableNameMap, _expressionDict["Where"]);
            //    whereVisitor.Visit();

            //    condition = whereVisitor.Condition;
            //    Parameters = whereVisitor.Parameters;
            //}
            ////先解析为查询
            StringBuilder childSelect = new StringBuilder("select ");
            childSelect.Append(string.Join(",", _columns));
            //childSelect.Append(" from " + tableName);
            //是否需要包装成子查询
            if (selectorVisitor != null && selectorVisitor.NeedWrapSelect)
            {
                //拼接主查询
                sb.Append(string.Join(" , ", _alias));
                sb.AppendFormat(" from ({0}) t ", childSelect.ToString());
            }
            else
            {
                sb = childSelect;
            }
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" where ");
                sb.Append(condition);
            }
            CommandText = sb.ToString();
        }

        private string _columns { get; set; }
        private List<string> _alias;
        private string condition;
        private SelectExpressionVisitor selectorVisitor;
    }
}
