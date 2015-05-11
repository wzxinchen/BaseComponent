using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DbEntity;

namespace Xinchen.XLinq
{
    public class QueryContext : EntityContext
    {
        private System.Reflection.PropertyInfo[] _properties;
        private Dictionary<Type, object> _entitySets;
        public QueryContext(string conn)
            : base(conn)
        {
            _properties = this.GetType().GetProperties();
            _entitySets = new Dictionary<Type, object>();
            foreach (var property in _properties.Where(x => x.PropertyType.IsGenericType && x.PropertyType.FullName.StartsWith("Xinchen.XLinq.QueryEntitySet")))
            {
                object entitySet = Activator.CreateInstance(property.PropertyType, this);
                property.SetValue(this, entitySet, null);
                _entitySets.Add(property.PropertyType.GetGenericArguments()[0], entitySet);
            }
        }
    }
}
