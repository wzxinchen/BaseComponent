using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Xinchen.DbEntity
{
    public class Util
    {
        public static bool HasRow(DataSet ds)
        {
            return ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
        }

        public static DataRow GetFirstRow(DataSet ds)
        {
            return ds.Tables[0].Rows[0];
        }
    }
}
