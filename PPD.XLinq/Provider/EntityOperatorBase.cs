using PPD.XLinq.SchemaModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider
{
    public interface IEntityOperator
    {
        int InsertEntities(ArrayList list);
        int UpdateValues(Column keyColumn, Table table, Dictionary<string,object> values);

        int Delete(Column keyColumn, Table table, params int[] ids);
    }
}
