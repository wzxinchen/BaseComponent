using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.Controls.Grid
{
    public class GridViewRefreshEventArgs : EventArgs
    {
        public GridViewRefreshEventArgs(int page, int pageSize, int pageCount)
        {
            //this.SortOrder = sortOrder;
            //this.Filters = fitlers;
            this.Page = page;
            this.PageSize = pageSize;
            this.PageCount = pageCount;
        }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int Total { get; set; }
        //public SortOrder SortOrder { get; set; }

        //public DynamicFilter[] Filters { get; set; }
    }
}
