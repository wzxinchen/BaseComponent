using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Xinchen.DynamicObject;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.Controls
{
    public class ControlHelper
    {
        public static void ShowErrorMessage(object exception)
        {
            var form = Form.ActiveForm;
            if (form != null)
            {
                Invoke(form, () =>
                {
                    MessageBox.Show(form, exception.ToString());
                });
            }
            else
            {
                MessageBox.Show(exception.ToString());
            }
            Environment.Exit(0);
        }
        public static void CatchException()
        {
            //设置应用程序处理异常方式：ThreadException处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += (s, e) =>
            {
                ShowErrorMessage(e.Exception);
            };
            //处理非UI线程异常
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                ShowErrorMessage(e.ExceptionObject);
            };
        }
        public static void Invoke(Control form, Action action)
        {
            form.Invoke(action);
        }

        /// <summary>
        /// 初始化表格的列
        /// </summary>
        /// <param name="gridView">要初始化的DataGridView</param>
        /// <param name="autoSizeMode">行模式</param>
        /// <param name="dataSource">数据源</param>
        public static void InitColumns(DataGridView gridView, DataGridViewAutoSizeColumnsMode autoSizeMode = DataGridViewAutoSizeColumnsMode.Fill)
        {
            gridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            var value = gridView.DataSource;
            gridView.AutoSizeColumnsMode = autoSizeMode;
            gridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            gridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridView.AllowUserToAddRows = false;
            gridView.AllowUserToDeleteRows = false;
            if (!(value is IList)) return;
            var type = value.GetType().GetGenericArguments().FirstOrDefault();
            if (type != null)
            {
                var pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => ExpressionReflector.IsEntityPropertyType(x.PropertyType));
                var displayType = typeof(DataGridColumnAttribute);
                foreach (var item in pis)
                {
                    var attrs = item.GetCustomAttributes(displayType, false);
                    if (attrs.Length <= 0) continue;
                    var dn = attrs[0] as DataGridColumnAttribute;
                    DataGridViewColumn column;
                    if (gridView.AutoGenerateColumns)
                    {
                        column = gridView.Columns[item.Name];
                        if (column == null) continue;
                        if (dn == null) continue;
                        column.HeaderText = dn.DisplayName;
                        column.Visible = dn.Visible;
                        if (dn.Width <= 0 || dn.Width == -1) continue;
                        try
                        {
                            column.Width = dn.Width;
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                            column = new DataGridViewTextBoxColumn();
                            column.HeaderText = dn.DisplayName;
                            column.Visible = dn.Visible;
                            column.Width = dn.Width;
                            gridView.Columns.Add(column);
                    }
                }
            }
        }

        public static void Show(Control control, string msg)
        {
            Invoke(control, () => MessageBox.Show(control, msg));
        }
    }
}
