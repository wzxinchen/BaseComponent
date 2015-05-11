using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xinchen.ScheduledTask.Winform.Model.Enum;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.ScheduledTask.Winform.Model.FormModel
{
    public class TaskModel:INotifyPropertyChanged
    {
        int _id, _interval, _runType, _hour, _minute, _status;
        string _name, _type, _method;
        [DataGridColumn(Visible=false)]
        public string Dll { get; set; }
        [DataGridColumn("编号")]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                ChangeProperty("Id");
            }
        }
        [DataGridColumn("名字")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                ChangeProperty("Name");
            }
        }
        [DataGridColumn("类名")]
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
                ChangeProperty("Type");
            }
        }
        [DataGridColumn("方法")]
        public string Method
        {
            get
            {
                return _method;
            }
            set
            {
                _method = value;
                ChangeProperty("Method");
            }
        }
        [DataGridColumn("间隔")]
        public int Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                _interval = value;
                ChangeProperty("Interval");
            }
        }
        [DataGridColumn("运行类型", ValueType = typeof(RunType))]
        public int RunType
        {
            get
            {
                return _runType;
            }
            set
            {
                _runType = value;
                ChangeProperty("RunType");
            }
        }
        [DataGridColumn("小时")]
        public int Hour
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;
                ChangeProperty("Hour");
            }
        }
        [DataGridColumn("分钟")]
        public int Minute
        {
            get
            {
                return _minute;
            }
            set
            {
                _minute = value;
                ChangeProperty("Minute");
            }
        }
        [DataGridColumn("状态", ValueType = typeof(Status))]
        public int Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                ChangeProperty("Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeProperty(string name)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
