using PPD.XLinq.TranslateModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.Parser
{
    /// <summary>
    /// 对访问成员方法的表达式进行分析
    /// </summary>
    public class MethodCallExpressionVisitor : ExpressionVisitorBase
    {
        Dictionary<string, Join> _joins;
        bool _isColumn = false;

        public MethodCallExpressionVisitor(TranslateContext context)
            : base(context)
        {
            // TODO: Complete member initialization
            this._joins = context.Joins;
        }

        Token ParseArgument(Expression argExp)
        {
            if (argExp.NodeType == ExpressionType.Convert || argExp.NodeType == ExpressionType.ConvertChecked)
            {
                argExp = ((UnaryExpression)argExp).Operand;
            }
            if (argExp.NodeType == ExpressionType.MemberAccess || argExp.NodeType == ExpressionType.Call)
            {
                var visitor = new MemberExpressionVisitor(Context);
                visitor.Visit(argExp);
                if (visitor.Token.Type == TokenType.Column)
                {
                    _isColumn = true;
                }
                return visitor.Token;
                //if (visitor.Token.Type == TokenType.Object)
                //{
                //}
                //else if (visitor.Token.Type == TokenType.Column)
                //{
                //    Type = MemberExpressionType.Column;
                //}
                //else
                //{
                //    throw new Exception();
                //}
                //return visitor.Result;
            }
            else if (argExp.NodeType == ExpressionType.Constant)
            {
                return Token.Create(((ConstantExpression)argExp).Value);
            }
            else
            {
                throw new Exception();
            }
        }



        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var argsExp = node.Arguments;
            var args = new List<Token>();
            foreach (var argExp in argsExp)
            {
                args.Add(ParseArgument(argExp));
            }
            if (node.Object == null)
            {
                //静态方法调用var col = (Token)body.Result;
                var method = node.Method;
                if (_isColumn)
                {
                    //string converter = "{0}";
                    var parameters = new List<object>();
                    switch (method.Name)
                    {
                        case "ToDateTime":
                            //if (method.DeclaringType != typeof(Convert) || argsExp.Count != 1)
                            //{
                            //    throw new Exception("不支持");
                            //}
                            Token = (Token)args[0];
                            parameters.AddRange(args.Skip(1));
                            //converter = Token.Column.Converter;
                            //if (string.IsNullOrWhiteSpace(converter))
                            //{
                            //    converter = "{0}";
                            //}
                            //switch (Token.Type)
                            //{
                            //    case TokenType.Column:
                            //        converter = string.Format(converter, "CONVERT(DATETIME,{0},211)");
                            //        Token.Column.Converter = converter;
                            //        break;
                            //    default:
                            //        throw new Exception();
                            //}
                            break;
                        case "Contains":
                            Token = (Token)args[1];
                            //var list = ((Token)args[0]).Object as IList<int>;
                            parameters.Add(args[0].Object);
                            parameters.AddRange(args.Skip(2).Where(x => x.Type == TokenType.Object).Select(x => x.Object));
                            //if (method.DeclaringType != typeof(Enumerable))
                            //{
                            //    throw new Exception("不支持");
                            //}
                            //converter = string.Format(converter, "{0} in (" + string.Join(",", list) + ")");
                            //Token.Column.Converter = converter;
                            break;
                        default:
                            throw new Exception();
                    }
                    var converter = new ColumnConverter(method, parameters);
                    Token.Column.Converters.Push(converter);
                }
                else
                {
                    Token = Token.Create(node.Method.Invoke(null, args.Select(x => x.Object).ToArray()));
                }
            }
            else
            {
                var body = new MemberExpressionVisitor(Context);
                body.Visit(node.Object);
                if (body.Token.Type == TokenType.Column && !_isColumn)
                {
                    //实例对象是列，参数不是列
                    Token = body.Token;
                    //string converter = Token.Column.Converter;
                    //if (string.IsNullOrWhiteSpace(converter))
                    //{
                    //    converter = "{0}";
                    //}
                    var method = node.Method;
                    var argObjects = args.Select(x => x.Object);
                    //Token.Column.ConverterParameters.AddRange(argObjects);
                    Token.Column.Converters.Push(new ColumnConverter(method, argObjects.ToList(), true));
                    //switch (method.Name)
                    //{
                    //    case "ToDateTime":
                    //        if (method.DeclaringType != typeof(Convert) || argsExp.Count != 1)
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        converter = string.Format(converter, "CONVERT(DATETIME,{0},211)");
                    //        break;
                    //    case "Substring":
                    //        if (method.DeclaringType != typeof(string))
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        if (argsExp.Count == 1)
                    //        {
                    //            //SubString(startIndex)
                    //            ExpressionVisitorBase visitor = new PropertyFieldExpressionVisitor(Context);
                    //            visitor.Visit(argsExp[0]);
                    //            if (visitor.Token.Type != TokenType.Object)
                    //            {
                    //                throw new Exception("不支持");
                    //            }
                    //            converter = string.Format(converter, "SUBSTRING({0}," + (Convert.ToInt32(argObjects[0]) + 1) + ",LEN({0})+1-" + Convert.ToInt32(visitor.Token.Object) + ")");
                    //        }
                    //        else if (argsExp.Count == 2)
                    //        {
                    //            //SubString(startIndex,length)
                    //            ExpressionVisitorBase startVisitor = new PropertyFieldExpressionVisitor(Context);
                    //            startVisitor.Visit(argsExp[0]);
                    //            if (startVisitor.Token.Type != TokenType.Object)
                    //            {
                    //                throw new Exception("不支持");
                    //            }
                    //            ExpressionVisitorBase lenVistior = new PropertyFieldExpressionVisitor(Context);
                    //            lenVistior.Visit(argsExp[1]);
                    //            if (lenVistior.Token.Type != TokenType.Object)
                    //            {
                    //                throw new Exception("不支持");
                    //            }
                    //            converter = string.Format(converter, "SUBSTRING({0}," + (Convert.ToInt32(startVisitor.Token.Object) + 1) + "," + Convert.ToInt32(lenVistior.Token.Object) + ")");
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "Contains":
                    //        if (method.DeclaringType == typeof(string))
                    //        {
                    //            converter = string.Format(converter, "CHARINDEX(@param1,{0})>0");
                    //            Token.Column.ConverterParameters.AddRange(argObjects);
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "StartsWith":
                    //        if (method.DeclaringType == typeof(string))
                    //        {
                    //            converter = string.Format(converter, "CHARINDEX(@param1,{0})=1");
                    //            Token.Column.ConverterParameters.AddRange(argObjects);
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddDays":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(DAY,@param1,{0})");
                    //            Token.Column.ConverterParameters.AddRange(argObjects);
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddHours":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(HOUR,@param1,{0})");
                    //            Token.Column.ConverterParameters.AddRange(argObjects);
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddMilliseconds":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(MILLISECOND,@param1,{0})");
                    //            Token.Column.ConverterParameters.AddRange(argObjects);
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddMinutes":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(MINUTE,@param1,{0})");
                    //            Token.Column.ConverterParameters.AddRange(argObjects);
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddMonths":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(MONTH,@param1,{0})");
                    //            Token.Column.ConverterParameters.AddRange(argObjects);
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddSeconds":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(SECOND,@param1,{0})");
                    //            Token.Column.ConverterParameters.AddRange(argObjects);
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddYears":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(YEAR,@param1,{0})");
                    //            Token.Column.ConverterParameters.AddRange(argObjects);
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    default:
                    //        throw new Exception();
                    //}
                    //Token.Column.Converter = converter;
                }
                else if (body.Token.Type == TokenType.Object && !_isColumn)
                {
                    Token = Token.Create(node.Method.Invoke(body.Token.Object, args.Select(x => x.Object).ToArray()));
                }
                else if (body.Token.Type == TokenType.Object && _isColumn)
                {
                    Token = args[0];
                    var parameters = new List<object>();
                    parameters.Add(body.Token.Object);
                    parameters.AddRange(args.Skip(1));
                    Token.Column.Converters.Push(new ColumnConverter(node.Method, parameters, false));
                    //string converter = Token.Column.Converter;
                    //if (string.IsNullOrWhiteSpace(converter))
                    //{
                    //    converter = "{0}";
                    //}
                    //var method = node.Method;
                    //switch (method.Name)
                    //{
                    //    case "Contains":
                    //        if (method.DeclaringType == typeof(string))
                    //        {
                    //            converter = string.Format(converter, "CHARINDEX({0},@param1)>0");
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "StartsWith":
                    //        if (method.DeclaringType == typeof(string))
                    //        {
                    //            converter = string.Format(converter, "CHARINDEX(@param1,{0})>0");
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddDays":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(DAY,{0},@param1)");
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddHours":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(HOUR,{0},@param1)");
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddMilliseconds":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(MILLISECOND,{0},@param1)");
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddMinutes":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(MINUTE,{0},@param1)");
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddMonths":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(MONTH,{0},@param1)");
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddSeconds":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(SECOND,{0},@param1)");
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    case "AddYears":
                    //        if (method.DeclaringType == typeof(DateTime))
                    //        {
                    //            converter = string.Format(converter, "DATEADD(YEAR,{0},@param1)");
                    //        }
                    //        else
                    //        {
                    //            throw new Exception("不支持");
                    //        }
                    //        break;
                    //    default:
                    //        throw new Exception("不支持");
                    //}
                    //Token.Column.Converter = converter;
                    //Token.Column.ConverterParameters.Add(body.Token.Object);
                }
                else
                {
                    throw new Exception();
                }
            }
            return node;
        }
    }
}
