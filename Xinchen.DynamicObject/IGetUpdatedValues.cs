using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.DynamicObject
{
    public interface IGetUpdatedValues
    {
        Dictionary<string, object> GetUpdatedValues();
    }
}
