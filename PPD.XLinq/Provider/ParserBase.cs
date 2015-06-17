using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider
{
    public abstract class ParserBase
    {
        public Type ElementType { get; set; }
        public ParseResult Result { get; protected set; }
        public abstract void Parse(Expression expression);
    }
}
