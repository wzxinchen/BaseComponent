using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xinchen.ScheduledTask.Winform.Model;
using Xinchen.ScheduledTask.Winform.Model.Enum;
using Xinchen.ScheduledTask.Winform.Model.FormModel;

namespace Xinchen.ScheduledTask.Winform
{
    public class BaseJob<T> : IJob
    {
        Type _type;
        MethodInfo _methodInfo;
        TaskModel _task;
        object _obj;
        public BaseJob()
        {
            _type = typeof(T);
            _obj = Activator.CreateInstance<T>();
        }
        public void Execute(IJobExecutionContext context)
        {
            if (_methodInfo == null)
            {
                _task = (TaskModel)context.JobDetail.JobDataMap["taskObject"];
                _methodInfo = _type.GetMethod(_task.Method);
            }
            try
            {
                _methodInfo.Invoke(_obj, new object[0]);

            }
            catch (Exception e)
            {
                _task.Status = (int)Status.Error;
            }
        }
    }
}
