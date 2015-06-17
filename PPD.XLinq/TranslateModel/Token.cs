using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.TranslateModel
{
    public class Token
    {
        private Token()
        {

        }

        public static Token Create(Column column)
        {
            return new Token()
            {
                Column = column,
                Type = TokenType.Column
            };
        }
        public static Token Create(object obj)
        {
            return new Token()
            {
                Object = obj,
                Type = TokenType.Object
            };
        }
        public static Token Create(Condition obj)
        {
            return new Token()
            {
                Condition = obj,
                Type = TokenType.Condition
            };
        }
        public TokenType Type { get; private set; }
        public Column Column { get; private set; }
        public object Object { get; private set; }
        public Condition Condition { get; private set; }
    }

    public enum TokenType
    {
        Column, Object,Condition
    }
}
