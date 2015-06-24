using PPD.XLinq.SchemaModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider
{
    internal abstract class EntityOperatorBase
    {
        internal abstract int InsertEntities(ArrayList list);
        internal abstract int UpdateValues(Column keyColumn, Table table, Dictionary<string,object> values);

        internal abstract int Delete(Column keyColumn, Table table, params int[] ids);
    }
}
