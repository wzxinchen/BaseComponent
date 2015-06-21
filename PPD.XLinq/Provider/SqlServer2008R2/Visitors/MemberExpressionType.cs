using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider.SqlServer2008R2.Visitors
{
    public enum MemberExpressionType
    {
        None,
        /// <summary>
        /// 该成员表达式是对列进行的访问
        /// </summary>
        Column,

        /// <summary>
        /// 自定义的对象
        /// </summary>
        Object,
        Token
    }
}
