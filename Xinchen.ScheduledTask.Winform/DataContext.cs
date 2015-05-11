using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.DbEntity;

namespace Xinchen.ScheduledTask.Winform
{
    public class DataContext : EntityContext
    {
        public DataContext()
            : base("ConnectionString")
        {
            Tasks = new EntitySet<Model.Task>();
        }

        public EntitySet<Model.Task> Tasks { get; private set; }
    }
}
