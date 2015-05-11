using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Xinchen.ScheduledTask.Winform
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };

            ////Grab the Scheduler instance from the Factory 
            //IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            //// and start it off
            //scheduler.Start();

            //// define the job and tie it to our HelloJob class
            //IJobDetail job = JobBuilder.Create(typeof(HelloJob))
            //    .WithIdentity("job1", "group1")
            //    .Build();
            //// Trigger the job to run now, and then repeat every 10 seconds
            //ITrigger trigger = TriggerBuilder.Create()
            //    .WithIdentity("trigger1", "group1")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInSeconds(2)
            //        .RepeatForever())
            //    .Build();

            //// Tell quartz to schedule the job using our trigger
            //scheduler.ScheduleJob(job, trigger);

            //// some sleep to show what's happening
            //Thread.Sleep(TimeSpan.FromSeconds(60));

            //// and last shut down the scheduler when you are ready to close your program
            //scheduler.Shutdown();

            //Console.WriteLine("Press any key to close the application");
            //Console.ReadKey();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
