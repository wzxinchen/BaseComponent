using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPD.XLinq
{
    public interface IOperateAddingEntities
    {
        ArrayList GetAddingEntities();
        void ClearAddingEntities();
    }
}
