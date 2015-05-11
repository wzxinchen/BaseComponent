using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.ScheduledTask.Winform.Model
{
    [Table("Tasks")]
    public class Task
    {
        [Key, AutoIncrement]
        public virtual int Id { get; set; }
        public virtual string Dll { get; set; }
        public virtual string Method { get; set; }
        public virtual int Interval { get; set; }
        public virtual string Type { get; set; }
        public virtual int Hour { get; set; }
        public virtual int Minute { get; set; }
        public virtual int RunType { get; set; }
        public virtual string Name { get; set; }
    }
}
