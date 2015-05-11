using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Xinchen.DynamicObject;
using Xinchen.Utils;
using Xinchen.Utils.Entity;

namespace Xinchen.Controls
{
    public class FormValidate
    {
        static Dictionary<Type, string> _controlPrefixMap;
        static Type textBoxType = typeof(TextBox);
        static Type comboBoxType = typeof(ComboBox);
        static Type numericType = typeof(NumericUpDown);
        static FormValidate()
        {
            _controlPrefixMap = new Dictionary<Type, string>();
            _controlPrefixMap.Add(textBoxType, "txt");
            _controlPrefixMap.Add(comboBoxType, "combo");
            _controlPrefixMap.Add(numericType, "num");
        }

        public FormValidate(Dictionary<Type, string> controlPrefixMap)
        {
        }

        static string GetControlPrefix<TControl>()
        {
            Type type = typeof(TControl);
            if (_controlPrefixMap.ContainsKey(type))
            {
                return _controlPrefixMap[type];
            }
            throw new KeyNotFoundException("未找到匹配的控件类型与前缀的映射");
        }

        public static Utils.Entity.EntityResult<TModel> Validate<TModel>(Form form)
            where TModel : class,new()
        {
            var controls = form.GetControls();
            EntityResult<TModel> er = new EntityResult<TModel>();
            var pis = ExpressionReflector.GetProperties(typeof(TModel));
            TModel model = new TModel();
            foreach (Control item in controls.Values)
            {
                if (item is TextBox)
                {
                    var prefix = GetControlPrefix<TextBox>();
                    var propertyPair = pis.Where(x => item.Name == prefix + x.Value.Name).FirstOrDefault();
                    if (string.IsNullOrEmpty(propertyPair.Key))
                    {
                        continue;
                    }
                    var propertyInfo = propertyPair.Value;
                    Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                    if (propertyType == null)
                    {
                        propertyType = propertyInfo.PropertyType;
                    }
                    object value = item.Text;
                    value = Convert.ChangeType(value, propertyType);
                    var attrs = AttributeHelper.GetAttributes<ValidationAttribute>(propertyInfo);
                    foreach (var attr in attrs)
                    {
                        if (attr != null && !attr.IsValid(value))
                        {
                            er.Message = string.IsNullOrEmpty(attr.ErrorMessage) ? ("表单项" + item.Name + "验证失败") : attr.ErrorMessage;
                            return er;
                        }
                    }
                    ExpressionReflector.SetValue(model, propertyPair.Key, value);
                }
                else if (item is NumericUpDown)
                {
                    var prefix = GetControlPrefix<NumericUpDown>();
                    var propertyPair = pis.Where(x => item.Name == prefix + x.Value.Name).FirstOrDefault();
                    if (string.IsNullOrEmpty(propertyPair.Key))
                    {
                        continue;
                    }
                    var propertyInfo = propertyPair.Value;
                    Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                    if (propertyType == null)
                    {
                        propertyType = propertyInfo.PropertyType;
                    }
                    object value = item.Text;
                    value = Convert.ChangeType(value, propertyType);
                    var attrs = AttributeHelper.GetAttributes<ValidationAttribute>(propertyInfo);
                    foreach (var attr in attrs)
                    {
                        if (attr != null && !attr.IsValid(value))
                        {
                            er.Message = string.IsNullOrEmpty(attr.ErrorMessage) ? ("表单项" + item.Name + "验证失败") : attr.ErrorMessage;
                            return er;
                        }
                    }
                    ExpressionReflector.SetValue(model, propertyPair.Key, value);
                }
                else if (item is ComboBox)
                {
                    var prefix = GetControlPrefix<ComboBox>();
                    var propertyPair = pis.Where(x => item.Name == prefix + x.Value.Name).FirstOrDefault();
                    if (string.IsNullOrEmpty(propertyPair.Key))
                    {
                        continue;
                    }
                    var propertyInfo = propertyPair.Value;
                    Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                    if (propertyType == null)
                    {
                        propertyType = propertyInfo.PropertyType;
                    }
                    object value = item.Text;
                    value = Convert.ChangeType(value, propertyType);
                    var attrs = AttributeHelper.GetAttributes<ValidationAttribute>(propertyInfo);
                    foreach (var attr in attrs)
                    {
                        if (attr != null && !attr.IsValid(value))
                        {
                            er.Message = string.IsNullOrEmpty(attr.ErrorMessage) ? ("表单项" + item.Name + "验证失败") : attr.ErrorMessage;
                            return er;
                        }
                    }
                    ExpressionReflector.SetValue(model, propertyPair.Key, value);
                }
            }
            er.Model = model;
            er.Success = true;
            return er;
        }
    }
}
