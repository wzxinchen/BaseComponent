using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Xinchen.ScheduledTask.Winform.Model.Enum;
using Xinchen.ScheduledTask.Winform.Model.FormModel;
using Xinchen.Utils;

namespace Xinchen.ScheduledTask.Winform
{
    public partial class MainForm : Form
    {
        DataContext db = new DataContext();
        Type _baseJobType = typeof(BaseJob<>);
        AddForm _addForm = new AddForm();
        IScheduler scheduler;
        BindingList<TaskModel> _taskModels = new BindingList<TaskModel>();
        public MainForm()
        {
            InitializeComponent();
            gridView1.DataSource = _taskModels;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            gridView1.RefreshData();
            scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            //try
            //{
            //    Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter { Level = Common.Logging.LogLevel.Info };

            //    // Grab the Scheduler instance from the Factory 
            //    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

            //    // and start it off
            //    scheduler.Start();

            //    // define the job and tie it to our HelloJob class
            //    IJobDetail job = JobBuilder.Create<HelloJob>()
            //        .WithIdentity("job1", "group1")
            //        .Build();

            //    // Trigger the job to run now, and then repeat every 10 seconds
            //    ITrigger trigger = TriggerBuilder.Create()
            //        .WithIdentity("trigger1", "group1")
            //        .StartNow().WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute())
            //        .WithSimpleSchedule(x => x
            //            .WithIntervalInSeconds(10).w
            //            .RepeatForever())
            //        .Build();

            //    // Tell quartz to schedule the job using our trigger
            //    scheduler.ScheduleJob(job, trigger);

            //    // some sleep to show what's happening
            //    Thread.Sleep(TimeSpan.FromSeconds(60));

            //    // and last shut down the scheduler when you are ready to close your program
            //    scheduler.Shutdown();
            //}
            //catch (SchedulerException se)
            //{
            //    Console.WriteLine(se);
            //}

            //Console.WriteLine("Press any key to close the application");
            //Console.ReadKey();
        }

        private void gridView1_Add(object sender, EventArgs e)
        {
            _addForm.ShowDialog(this);
            gridView1.RefreshData();
        }

        private void gridView1_OnRefresh(object arg1, Controls.Grid.GridViewRefreshEventArgs arg2)
        {
            var list = db.Tasks.GetList();
            ControlHelper.Invoke(this, () =>
            {
                foreach (var item in list)
                {
                    TaskModel tm = _taskModels.FirstOrDefault(x => x.Name == item.Name);// new TaskModel();
                    if (tm == null)
                    {
                        tm = new TaskModel();
                        tm.Hour = item.Hour;
                        tm.Interval = item.Interval;
                        tm.Method = item.Method;
                        tm.Minute = item.Minute;
                        tm.Name = item.Name;
                        tm.RunType = item.RunType;
                        tm.Type = item.Type;
                        tm.Id = item.Id;
                        tm.Status = (int)Status.WaitRun;
                        tm.Dll = item.Dll;
                        _taskModels.Add(tm);
                    }
                }
                StartTask(_taskModels);
            });
        }

        private void StartTask(IEnumerable<TaskModel> list)
        {
            foreach (var item in list)
            {
                if (item.Status != (int)Status.WaitRun) continue;
                var ass = Assembly.LoadFile(item.Dll);
                var type = ass.GetTypes().FirstOrDefault(x => x.FullName.EndsWith(item.Type));
                type = _baseJobType.MakeGenericType(type);
                var data = new JobDataMap();
                data.Put("taskObject", item);
                var jobDetail = JobBuilder.Create(type).UsingJobData(data)
                    .WithIdentity(item.Name)
                    .Build();
                ITrigger trigger;
                if (item.RunType == (int)RunType.Interval)
                {
                    //隔多少秒执行
                    trigger = TriggerBuilder.Create()
                        .WithIdentity(item.Name)
                        .StartNow()
                        .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(item.Interval)
                            .RepeatForever())
                        .Build();
                }
                else
                {
                    //指定时间执行
                    trigger = TriggerBuilder.Create()
                        .WithIdentity(item.Name)
                        .StartNow()
                        .WithSchedule(
                        CronScheduleBuilder.DailyAtHourAndMinute(item.Hour, item.Minute)
                        )
                        .Build();
                }
                scheduler.ScheduleJob(jobDetail, trigger);
                item.Status = (int)Status.Runing;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            scheduler.Shutdown(false);
        }
    }
}
