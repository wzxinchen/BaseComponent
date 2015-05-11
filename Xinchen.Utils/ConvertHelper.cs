using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.Utils
{
    public class ConvertHelper
    {
        public static int ToInt32(object o)
        {
            if (o == null) return 0;
            string str = o.ToString();
            int result = 0;
            int.TryParse(str, out result);
            return result;
        }

        public static T[] ToArray<T>(string idString)
        {
            if(string.IsNullOrEmpty(idString))
            {
                return new T[0];
            }
            string[] arr = idString.Split(',');
            List<T> list = new List<T>();
            Type type = typeof(T);
            foreach (var item in arr)
            {
                try
                {
                    list.Add((T)Convert.ChangeType(item, type));
                }
                catch (InvalidCastException)
                {

                }
                catch (FormatException)
                {

                }
            }
            return list.ToArray();
        }
    }
}
