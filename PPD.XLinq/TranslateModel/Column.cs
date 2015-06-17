using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.TranslateModel
{
    public class Column
    {
        public Table Table { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 调用者在linq中指定的别名，若未指定别名则为空
        /// </summary>
        public string Alias { get; set; }
        public Type DataType { get; set; }

        public string Converter { get; set; }
        public Column()
        {
            ConverterParameters = new List<object>();
        }
        public List<object> ConverterParameters { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}.{1} AS {2}", Table.Name, Name, Alias);
        }

    }
}
