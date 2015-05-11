using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Xinchen.ScheduledTask.Winform
{
    public class HelloJob : IJob
    {
        long _id = 0;
        public HelloJob()
        {
        }
        public void Execute(IJobExecutionContext context)
        {
            if (_id == 0)
            {
                _id = DateTime.Now.ToFileTime();
            }
            Console.WriteLine(_id);
        }
    }
}
