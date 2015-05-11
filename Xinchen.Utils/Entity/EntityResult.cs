using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.Utils.Entity
{
    public class EntityResult<TModel>
    {
        public string Message { get; set; }
        public TModel Model { get; set; }
        public bool Success { get; set; }
    }
}
