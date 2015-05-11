using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.DbEntity
{
    public class EntityException:ApplicationException
    {

        public EntityException(string message):base(message)
        {
        }
    }
}
