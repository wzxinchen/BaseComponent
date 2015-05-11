using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Xinchen.XLinq
{
    public class ObjectValueExpressionVisitor : ExpressionVisitor
    {
        MemberInfo _memberInfo;
        object _value;

        public object Value
        {
            get { return _value; }
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            _memberInfo = node.Member;
            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (_memberInfo.MemberType == MemberTypes.Field)
            {
                var fieldInfo = _memberInfo as FieldInfo;
                _value = fieldInfo.GetValue(node.Value);
            }
            else if(_memberInfo.MemberType== MemberTypes.Property)
            {
                var propertyInfo = _memberInfo as PropertyInfo;
                _value = propertyInfo.GetValue(node.Value, null);
            }
            return base.VisitConstant(node);
        }
    }
}
