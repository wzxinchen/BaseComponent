using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using System.Windows.Forms;
using Xinchen.Controls;
using Xinchen.DbEntity;
using Xinchen.ScheduledTask.Winform.Model;
using Xinchen.ScheduledTask.Winform.Model.Enum;
using Xinchen.ScheduledTask.Winform.Model.FormModel;

namespace Xinchen.ScheduledTask.Winform
{
    public partial class AddForm : Form
    {
        public Task Task { get; private set; }
        DataContext db = new DataContext();
        private Assembly ass;
        public AddForm()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            InitializeComponent();
        }

        private void txtDll_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                txtDll.Text = openFileDialog1.FileName;
                try
                {
                    ass = Assembly.LoadFile(txtDll.Text);
                    Dictionary<string, string> typeDict = new Dictionary<string, string>();
                    var types = ass.GetTypes().Where(x => x.IsPublic && !x.Assembly.GlobalAssemblyCache);
                    foreach (var item in types)
                    {
                        typeDict.Add(item.Name, item.Name);
                    }
                    comboType.DisplayMember = "Value";
                    comboType.ValueMember = "Key";
                    comboType.DataSource = new BindingSource(types, null);
                }
                catch (BadImageFormatException)
                {
                    MessageBox.Show(this, "选择的文件不是一个有效的程序集文件");
                    txtDll.Clear();
                }
            }
        }

        Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("resources")) return null;
            return Assembly.LoadFile(Path.GetDirectoryName(txtDll.Text) + "/" + args.Name.Split(',')[0] + ".dll");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addTaskModelResult = FormValidate.Validate<AddTaskModel>(this);
            if (!addTaskModelResult.Success)
            {
                MessageBox.Show(this, addTaskModelResult.Message);
                return;
            }
            var addTaskModel = addTaskModelResult.Model;
            if (radInterval.Checked)
            {
                addTaskModel.RunType = (int)RunType.Interval;
            }
            else
            {
                addTaskModel.RunType = (int)RunType.Timing;
            }
            addTaskModel.Hour = dateTimePicker1.Value.Hour;
            addTaskModel.Minute = dateTimePicker1.Value.Minute;
            if (!File.Exists(addTaskModel.Dll))
            {
                MessageBox.Show(this, "程序集文件不存在");
                return;
            }
            var type = ass.GetTypes().FirstOrDefault(x => x.FullName.EndsWith(addTaskModel.Type));
            if (type == null)
            {
                MessageBox.Show(this, "在选定的文件中未找到指定的类型名称，请确认大小写无误");
                return;
            }
            MethodInfo methodInfo = null;
            try
            {
                methodInfo = type.GetMethod(addTaskModel.Method);
                if (methodInfo == null)
                {
                    MessageBox.Show(this, "在选定的文件中指定的类型中未找到指定的方法，请确认大小写无误、类名无重复，如果类名有重复，请指定全名");
                    return;
                }
            }
            catch (AmbiguousMatchException)
            {
                MessageBox.Show(this, "在选定的文件中指定的类型中未找到指定的方法，请确认大小写无误、类名无重复，如果类名有重复，请指定全名");
                return;
            }
            if (methodInfo.IsStatic)
            {
                MessageBox.Show(this, "指定方法不能为静态方法");
                return;
            }
            if (methodInfo.GetGenericArguments().Length > 0)
            {
                MessageBox.Show(this, "指定方法不能为泛型方法");
                return;
            }
            if (!methodInfo.IsPublic)
            {
                MessageBox.Show(this, "指定方法非公开，请修改为公开");
                return;
            }
            if (methodInfo.GetParameters().Length > 0)
            {
                MessageBox.Show(this, "指定方法不能包含参数");
                return;
            }
            if (db.Tasks.GetList(x => x.Name = addTaskModelResult.Model.Name).Any())
            {
                MessageBox.Show(this, "名称重复");
                return;
            }
            Task = db.Tasks.Create(new Model.Task()
            {
                Dll = addTaskModel.Dll,
                Hour = addTaskModel.Hour,
                Interval = addTaskModel.Interval,
                Method = addTaskModel.Method,
                Minute = addTaskModel.Minute,
                Name = addTaskModel.Name,
                RunType = addTaskModel.RunType,
                Type = addTaskModel.Type
            });
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void AddForm_Load(object sender, EventArgs e)
        {

        }
    }
}
