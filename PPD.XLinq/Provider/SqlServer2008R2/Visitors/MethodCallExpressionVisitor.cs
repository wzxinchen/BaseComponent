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
    /// 对访问成员方法的表达式进行分析
    /// </summary>
    public class MethodCallExpressionVisitor : ExpressionVisitorBase
    {
        Dictionary<string, Join> _joins;
        /// <summary>
        /// 参数可传null，该参数主要用于创建该表达式中可能出现的对Entity的访问
        /// </summary>
        public MethodCallExpressionVisitor(Dictionary<string, Join> joins)
        {
            this._joins = joins;
        }
        public override Expression Visit(Expression node)
        {
            Type = MemberExpressionType.Object;
            return base.Visit(node);
        }

        object ParseArgument(Expression argExp)
        {
            if (argExp.NodeType == ExpressionType.Convert || argExp.NodeType == ExpressionType.ConvertChecked)
            {
                argExp = ((UnaryExpression)argExp).Operand;
            }
            if (argExp.NodeType == ExpressionType.MemberAccess || argExp.NodeType == ExpressionType.Call)
            {
                var visitor = new MemberExpressionVisitor(_joins);
                visitor.Visit(argExp);
                if (visitor.Type == MemberExpressionType.Object)
                {
                }
                else if (visitor.Type == MemberExpressionType.Column)
                {
                    Type = MemberExpressionType.Column;
                }
                else
                {
                    throw new Exception();
                }
                return visitor.Result;
            }
            else if (argExp.NodeType == ExpressionType.Constant)
            {
                return ((ConstantExpression)argExp).Value;
            }
            else
            {
                throw new Exception();
            }
        }



        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var argsExp = node.Arguments;
            var args = new List<object>();
            Type = MemberExpressionType.Object;
            foreach (var argExp in argsExp)
            {
                args.Add(ParseArgument(argExp));
            }
            if (node.Object == null)
            {
                //静态方法调用var col = (Token)body.Result;
                var method = node.Method;
                if (Type == MemberExpressionType.Column)
                {
                    string converter = "{0}";
                    Token token = null;
                    switch (method.Name)
                    {
                        case "ToDateTime":
                            if (method.DeclaringType != typeof(Convert) || argsExp.Count != 1)
                            {
                                throw new Exception("不支持");
                            }
                            token = (Token)args[0];
                            converter = token.Column.Converter;
                            if (string.IsNullOrWhiteSpace(converter))
                            {
                                converter = "{0}";
                            }
                            switch (Type)
                            {
                                case MemberExpressionType.Column:
                                    converter = string.Format(converter, "CONVERT(DATETIME,{0},211)");
                                    token.Column.Converter = converter;
                                    break;
                                default:
                                    throw new Exception();
                            }
                            Result = token;
                            break;
                        case "Contains":
                            token = (Token)args[1];
                            var list = args[0] as IList<int>;
                            if (method.DeclaringType != typeof(Enumerable))
                            {
                                throw new Exception("不支持");
                            }
                            converter = string.Format(converter, "{0} in (" + string.Join(",", list) + ")");
                            token.Column.Converter = converter;
                            Result = token;
                            break;
                        default:
                            throw new Exception();
                    }
                }
                else if (Type == MemberExpressionType.Object)
                {
                    Result = node.Method.Invoke(null, args.ToArray());
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                var body = new MemberExpressionVisitor(_joins);
                body.Visit(node.Object);
                if (body.Type == MemberExpressionType.Column && Type == MemberExpressionType.Object)
                {
                    var token = (Token)body.Result;
                    string converter = token.Column.Converter;
                    if (string.IsNullOrWhiteSpace(converter))
                    {
                        converter = "{0}";
                    }
                    var method = node.Method;
                    switch (method.Name)
                    {
                        case "ToDateTime":
                            if (method.DeclaringType != typeof(Convert) || argsExp.Count != 1)
                            {
                                throw new Exception("不支持");
                            }
                            converter = string.Format(converter, "CONVERT(DATETIME,{0},211)");
                            break;
                        case "Substring":
                            if (method.DeclaringType != typeof(string))
                            {
                                throw new Exception("不支持");
                            }
                            if (argsExp.Count == 1)
                            {
                                //SubString(startIndex)
                                PropertyFieldExpressionVisitor visitor = new PropertyFieldExpressionVisitor(_joins);
                                visitor.Visit(argsExp[0]);
                                if (visitor.Type != MemberExpressionType.Object)
                                {
                                    throw new Exception("不支持");
                                }
                                converter = string.Format(converter, "SUBSTRING({0}," + (Convert.ToInt32(visitor.Result) + 1) + ",LEN({0})+1-" + Convert.ToInt32(visitor.Result) + ")");
                            }
                            else if (argsExp.Count == 2)
                            {
                                //SubString(startIndex,length)
                                PropertyFieldExpressionVisitor startVisitor = new PropertyFieldExpressionVisitor(_joins);
                                startVisitor.Visit(argsExp[0]);
                                if (startVisitor.Type != MemberExpressionType.Object)
                                {
                                    throw new Exception("不支持");
                                }
                                PropertyFieldExpressionVisitor lenVistior = new PropertyFieldExpressionVisitor(_joins);
                                lenVistior.Visit(argsExp[1]);
                                if (lenVistior.Type != MemberExpressionType.Object)
                                {
                                    throw new Exception("不支持");
                                }
                                converter = string.Format(converter, "SUBSTRING({0}," + (Convert.ToInt32(startVisitor.Result) + 1) + "," + Convert.ToInt32(lenVistior.Result) + ")");
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "Contains":
                            if (method.DeclaringType == typeof(string))
                            {
                                converter = string.Format(converter, "CHARINDEX(@param1,{0})>0");
                                token.Column.ConverterParameters.AddRange(args);
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "StartsWith":
                            if (method.DeclaringType == typeof(string))
                            {
                                converter = string.Format(converter, "CHARINDEX(@param1,{0})=1");
                                token.Column.ConverterParameters.AddRange(args);
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddDays":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(DAY,@param1,{0})");
                                token.Column.ConverterParameters.AddRange(args);
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddHours":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(HOUR,@param1,{0})");
                                token.Column.ConverterParameters.AddRange(args);
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddMilliseconds":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(MILLISECOND,@param1,{0})");
                                token.Column.ConverterParameters.AddRange(args);
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddMinutes":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(MINUTE,@param1,{0})");
                                token.Column.ConverterParameters.AddRange(args);
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddMonths":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(MONTH,@param1,{0})");
                                token.Column.ConverterParameters.AddRange(args);
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddSeconds":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(SECOND,@param1,{0})");
                                token.Column.ConverterParameters.AddRange(args);
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddYears":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(YEAR,@param1,{0})");
                                token.Column.ConverterParameters.AddRange(args);
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                    token.Column.Converter = converter;
                    Result = token;
                    Type = MemberExpressionType.Column;
                }
                else if (body.Type == MemberExpressionType.Object && Type == MemberExpressionType.Object)
                {
                    Result = node.Method.Invoke(body.Result, args.ToArray());
                }
                else if (body.Type == MemberExpressionType.Object && Type == MemberExpressionType.Column)
                {
                    var col = (Token)args[0];
                    string converter = col.Column.Converter;
                    if (string.IsNullOrWhiteSpace(converter))
                    {
                        converter = "{0}";
                    }
                    var method = node.Method;
                    switch (method.Name)
                    {
                        case "Contains":
                            if (method.DeclaringType == typeof(string))
                            {
                                converter = string.Format(converter, "CHARINDEX({0},@param1)>0");
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "StartsWith":
                            if (method.DeclaringType == typeof(string))
                            {
                                converter = string.Format(converter, "CHARINDEX(@param1,{0})>0");
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddDays":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(DAY,{0},@param1)");
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddHours":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(HOUR,{0},@param1)");
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddMilliseconds":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(MILLISECOND,{0},@param1)");
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddMinutes":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(MINUTE,{0},@param1)");
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddMonths":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(MONTH,{0},@param1)");
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddSeconds":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(SECOND,{0},@param1)");
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        case "AddYears":
                            if (method.DeclaringType == typeof(DateTime))
                            {
                                converter = string.Format(converter, "DATEADD(YEAR,{0},@param1)");
                            }
                            else
                            {
                                throw new Exception("不支持");
                            }
                            break;
                        default:
                            throw new Exception("不支持");
                    }
                    col.Column.Converter = converter;
                    col.Column.ConverterParameters.Add(body.Result);
                    Result = col;
                }
                else
                {
                    throw new Exception();
                }
            }
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Type = MemberExpressionType.Column;

            return base.VisitParameter(node);
        }
    }
}
