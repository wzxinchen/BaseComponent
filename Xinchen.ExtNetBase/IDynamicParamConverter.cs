namespace Xinchen.ExtNetBase
{
    using Ext.Net;
    using System;
    using System.Collections.Generic;
    using Xinchen.DbUtils;

    public interface IDynamicParamConverter
    {
        Dictionary<string, Func<FilterCondition, DynamicSqlParam[]>> Converters { get; }

        Dictionary<string, string> FieldMap { get; }
    }
}

