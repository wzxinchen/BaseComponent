using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xinchen.Utils
{
    public class EnumHelper
    {
        public static List<T> GetList<T>(Type enumType,Func<int,string,T> typeSelector)
        {
            var values = Enum.GetValues(enumType);
            var names = Enum.GetNames(enumType);
            List<T> list = new List<T>();

            for (int it = 0; it < values.Length; it++)
            {
                var fieldInfo = enumType.GetField(names[it]);
                var descAttr = AttributeHelper.GetAttribute<DescriptionAttribute>(fieldInfo);
                int key=(int)values.GetValue(it);
                T t = typeSelector(key, descAttr == null ? names[it] : descAttr.Description);
                list.Add(t);
            }
            return list;
        }
        public static Dictionary<int,string> GetDictionary(Type enumType)
        {
            var values = Enum.GetValues(enumType);
            var names = Enum.GetNames(enumType);
            Dictionary<int, string> valueList = new Dictionary<int, string>();

            for (int it = 0; it < values.Length; it++)
            {
                var fieldInfo = enumType.GetField(names[it]);
                var descAttr = AttributeHelper.GetAttribute<DescriptionAttribute>(fieldInfo);
                if(descAttr==null)
                {
                    valueList.Add((int)values.GetValue(it), names[it]);
                }
                else
                {
                    valueList.Add((int)values.GetValue(it), descAttr.Description);
                }
            }

            return valueList;
        }
    }
}
