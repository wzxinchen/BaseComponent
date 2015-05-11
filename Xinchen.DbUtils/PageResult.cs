namespace Xinchen.DbUtils
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class PageResult<T>
    {
        public PageResult()
        {
            Data = new List<T>();
        }
        public IList<T> Data { get; set; }

        public int Page { get; set; }

        public int PageCount { get; set; }

        public int RecordCount { get; set; }
    }
}
