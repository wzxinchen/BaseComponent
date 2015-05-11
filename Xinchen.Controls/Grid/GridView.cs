using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Xinchen.Utils;
using Xinchen.Utils.DataAnnotations;
using System.Threading.Tasks;

namespace Xinchen.Controls.Grid
{
    public partial class GridView : UserControl
    {
        //FilterForm filterForm = new FilterForm();
        #region 初始化列
        //public static void InitColumns(DataGridView gridView, DataGridViewAutoSizeColumnsMode autoSizeMode = DataGridViewAutoSizeColumnsMode.Fill)
        //{
        //    gridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
        //    var value = gridView.DataSource;
        //    gridView.AutoSizeColumnsMode = autoSizeMode;
        //    gridView.EditMode = DataGridViewEditMode.EditProgrammatically;
        //    gridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        //    gridView.AllowUserToAddRows = false;
        //    gridView.AllowUserToDeleteRows = false;
        //    if (value is IList)
        //    {
        //        var type = value.GetType().GetGenericArguments().FirstOrDefault();
        //        if (type != null)
        //        {
        //            var pis = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        //            var displayType = typeof(DataGridColumnDisplayAttribute);
        //            foreach (var item in pis)
        //            {
        //                var attrs = item.GetCustomAttributes(displayType, false);
        //                if (attrs.Length <= 0) continue;
        //                var dn = attrs[0] as DataGridColumnDisplayAttribute;
        //                var column = gridView.Columns[item.Name];
        //                if (column == null) continue;
        //                if (dn == null) continue;
        //                column.HeaderText = dn.DisplayName;
        //                column.Visible = dn.Visible;
        //                if (dn.Width <= 0) continue;
        //                try
        //                {
        //                    column.Width = dn.Width;
        //                }
        //                catch
        //                {
        //                }
        //            }
        //        }
        //    }
        //}
        //public void InitColumns(DataGridView gridView)
        //{
        //    ControlHelper.InitColumns(gridView);
        //    var value = gridView.DataSource;
        //    if (value is IList)
        //    {
        //        foreach (var item in _dataSourceObjectTypePropertyInfoTypes.Keys)
        //        {
        //            var attrs = _dataSourceObjectTypePropertyInfoTypes[item];// item.GetCustomAttributes(displayType, false);
        //            var dn = attrs.DataGridColumnDisplayAttribute;// as DataGridColumnDisplayAttribute;
        //            var column = gridView.Columns[item.Name];
        //            if (column == null) continue;
        //            column.HeaderText = dn.DisplayName;
        //            column.Visible = dn.Visible;
        //            if (item.PropertyType.FullName.Contains("System.Boolean"))
        //            {
        //                //column.CellTemplate=new DataGridViewTextBoxCell();
        //            }
        //            if (dn.Width <= 0) continue;
        //            try
        //            {
        //                column.Width = dn.Width;
        //            }
        //            catch (Exception)
        //            {
        //            }
        //        }
        //    }
        //}
        #endregion
        protected override void OnPaint(PaintEventArgs e)
        {
            int x = 0;
            int y = 0;
            int w = Width;
            int h = Height;
            int a = Radius;
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddArc(x, y, a, a, 180, 90);
            myPath.AddArc(w - a - 1, y, a, a, 270, 90);
            myPath.AddArc(w - a - 1, h - a - 1, a, a, 0, 90);
            myPath.AddArc(x, h - a - 1, a, a, 90, 90);
            myPath.CloseAllFigures();
            e.Graphics.DrawPath(SystemPens.ControlDarkDark, myPath);
        }

        private Type addType, editType;
        public event Action<object, GridViewRefreshEventArgs> OnRefresh;
        public event EventHandler Add;
        public event EventHandler Edit;
        public Icon Icon { get; set; }
        public event Func<object[], bool> OnRemove;
        //public event Func<object, bool> OnSaveEdit;
        //public event Func<object, bool> OnSaveAdd;
        bool _enableAdd = true, _enableEdit = true, _enableRemove = true, _enablePaging = true;
        public ToolStrip PagingToolStrip
        {
            get
            {
                return toolStripPaging;
            }
        }

        [DefaultValue(true)]
        public bool EnablePaging
        {
            get { return _enablePaging; }
            set
            {
                _enablePaging = value;
                base.Refresh();
            }
        }
        [DefaultValue(true)]
        public bool EnableAdd
        {
            get
            {
                return _enableAdd;
            }
            set
            {
                _enableAdd = value;
                base.OnLoad(new EventArgs());
            }
        }

        [DefaultValue(true)]
        public bool EnableEdit
        {
            get
            {
                return _enableEdit;
            }
            set
            {
                _enableEdit = value;
                base.Refresh();
            }
        }

        [DefaultValue(true)]
        public bool EnableRemove
        {
            get
            {
                return _enableRemove;
            }
            set
            {
                _enableRemove = value;
                base.Refresh();
            }
        }
        public DataGridView DataGridView
        {
            get
            {
                return dataGridView1;
            }
        }

        public List<T> GetSelectedItems<T>()
        {
            var list = (from DataGridViewRow item in dataGridView1.SelectedRows select (T)item.DataBoundItem).ToList();
            list.Reverse();
            return list;
        }

        #region 分页处理
        int _page = 1, _pageSize = 20, _total, _pageCount;
        public int PageCount
        {
            get { return _pageCount; }
            private set
            {
                _pageCount = value;
            }
        }

        public int Total
        {
            get { return _total; }
            set
            {
                _total = value;
                lblTotal.Text = value.ToString(CultureInfo.InvariantCulture);
                PageCount = (int)Math.Ceiling((float)Total / PageSize);
            }
        }

        void ToggleButton()
        {
            btnNext.Enabled = _page < PageCount;
            btnPrev.Enabled = _page > 1;
            lblPageCount.Text = PageCount.ToString(CultureInfo.InvariantCulture);
            lblTotal.Text = Total.ToString(CultureInfo.InvariantCulture);
            txtPageIndex.Text = Page.ToString(CultureInfo.InvariantCulture);
        }

        [DefaultValue(100)]
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                txtPageSize.Text = value.ToString(CultureInfo.InvariantCulture);
                PageCount = (int)Math.Ceiling((float)Total / PageSize);
            }
        }
        [DefaultValue(1)]
        public int Page
        {
            get
            {
                return _page;
            }
            set
            {
                _page = value;
            }
        }
        #endregion

        Dictionary<PropertyInfo, ColumnAttributes> initProperties(PropertyInfo[] ObjectPis)
        {
            Dictionary<PropertyInfo, ColumnAttributes> propertyInfoTypes = new Dictionary<PropertyInfo, ColumnAttributes>();
            foreach (var item in ObjectPis)
            {
                var ca = new ColumnAttributes
                {
                    DataGridColumnDisplayAttribute =
                        (DataGridColumnAttribute)
                            item.GetCustomAttributes(columnDisplayType, false).FirstOrDefault()
                };
                if (ca.DataGridColumnDisplayAttribute == null)
                {
                    ca.DataGridColumnDisplayAttribute = new DataGridColumnAttribute(item.Name);
                }
                ca.DisplayAttribute = (DisplayAttribute)item.GetCustomAttributes(displayType, false).FirstOrDefault();
                ca.RegularExpressionAttribute = (RegularExpressionAttribute)item.GetCustomAttributes(regExpType, false).FirstOrDefault();
                ca.RequiredAttribute = (RequiredAttribute)item.GetCustomAttributes(requireType, false).FirstOrDefault();
                ca.ValidationAttribute = (ValidationAttribute)item.GetCustomAttributes(validateType, false).FirstOrDefault();
                ca.KeyAttribute = (KeyAttribute)item.GetCustomAttributes(keyType, false).FirstOrDefault();
                ca.DefaultAttribute = (DefaultValueAttribute)item.GetCustomAttributes(defalutValueType, false).FirstOrDefault();
                //ca.DataSourceAttribute =
                //(DataSourceAttribute)item.GetCustomAttributes(datasourceAttribute, false).FirstOrDefault();
                propertyInfoTypes.Add(item, ca);
            }
            return propertyInfoTypes;
        }

        public object DataSource
        {
            get
            {
                return dataGridView1.DataSource;
            }
            set
            {
                if (value == null) return;
                dataGridView1.DataSource = value;
                Type type = value.GetType().GetGenericArguments()[0];
                addType = type;
                _dataSourceObjectPis = _addTypePis = type.GetProperties();
                _dataSourceObjectTypePropertyInfoTypes = _addTypePropertyInfoTypes = initProperties(_dataSourceObjectPis);
                //_pis = type.GetProperties().ToArray();
                //_propertyInfoTypes = new Dictionary<PropertyInfo, ColumnAttributes>();
                //foreach (var item in _pis)
                //{
                //    var ca = new ColumnAttributes
                //    {
                //        DataGridColumnDisplayAttribute =
                //            (DataGridColumnDisplayAttribute)
                //                item.GetCustomAttributes(columnDisplayType, false).FirstOrDefault()
                //    };
                //    if (ca.DataGridColumnDisplayAttribute == null)
                //    {
                //        ca.DataGridColumnDisplayAttribute = new DataGridColumnDisplayAttribute(item.Name);
                //    }
                //    ca.DisplayAttribute = (DisplayAttribute)item.GetCustomAttributes(displayType, false).FirstOrDefault();
                //    ca.RegularExpressionAttribute = (RegularExpressionAttribute)item.GetCustomAttributes(regExpType, false).FirstOrDefault();
                //    ca.RequiredAttribute = (RequiredAttribute)item.GetCustomAttributes(requireType, false).FirstOrDefault();
                //    ca.ValidationAttribute = (ValidationAttribute)item.GetCustomAttributes(validateType, false).FirstOrDefault();
                //    ca.KeyAttribute = (KeyAttribute)item.GetCustomAttributes(keyType, false).FirstOrDefault();
                //    ca.DefaultAttribute = (DefaultValueAttribute)item.GetCustomAttributes(defalutValueType, false).FirstOrDefault();
                //    ca.DataSourceAttribute =
                //        (DataSourceAttribute)item.GetCustomAttributes(datasourceAttribute, false).FirstOrDefault();
                //    _propertyInfoTypes.Add(item, ca);
                //}

                ControlHelper.InitColumns(DataGridView);
            }
        }
        public GridView()
        {
            InitializeComponent();
            DataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            //Load += DataGridViewEx_Load;
            //_filters = new List<DynamicFilter>();

        }
        Type displayType = typeof(DisplayAttribute),
            keyType = typeof(KeyAttribute),
            PasswordPropertyTextType = typeof(PasswordPropertyTextAttribute),
            validateType = typeof(ValidationAttribute),
            //datasourceAttribute = typeof(DataSourceAttribute),
                columnDisplayType = typeof(DataGridColumnAttribute),
                regExpType = typeof(RegularExpressionAttribute),
                requireType = typeof(RequiredAttribute),
                defalutValueType = typeof(DefaultValueAttribute);

        Control GetFilterEditorByType(PropertyInfo pi)
        {
            Control control;
            var type = pi.PropertyType;//, realType = ca.RealTypeAttribute == null ? null : ca.RealTypeAttribute.Type;
            if (type.FullName.Contains("System.Int32"))
            {
                control = new NumericUpDown();
            }
            else if (type.FullName.Contains("System.Int64"))
            {
                control = new NumericUpDown();
            }
            else if (type.FullName.Contains("System.DateTime"))
            {
                control = new DateTimePicker();
            }
            else if (type.FullName.Contains("System.String"))
            {
                var combo = new ComboBox();
                var dict = new Dictionary<string, string>();
                var items = DataSource as IList;
                if (items != null)
                    foreach (var value in from object item in items select pi.GetValue(item, null) into val where val != null select val.ToString() into value where !dict.ContainsKey(value) select value)
                    {
                        dict.Add(value, value);
                        combo.AutoCompleteCustomSource.Add(value);
                    }
                if (dict.Count <= 0) dict.Add("", "");
                combo.DataSource = new BindingSource(dict, null);
                combo.DisplayMember = "Key";
                combo.ValueMember = "Value";
                combo.AutoCompleteMode = AutoCompleteMode.Suggest;
                combo.AutoCompleteSource = AutoCompleteSource.CustomSource;
                control = combo;
            }
            else
            {
                control = new TextBox();
                var passwordAttr = ((PasswordPropertyTextAttribute[])type.GetCustomAttributes(PasswordPropertyTextType, false)).FirstOrDefault();
                if (passwordAttr != null && passwordAttr.Password)
                {
                    ((TextBox)control).PasswordChar = '*';
                }
            }
            return control;
        }

        //Control GetEditorByType(PropertyInfo pi)
        //{
        //    Control control;
        //    var type = pi.PropertyType;//, realType = ca.RealTypeAttribute == null ? null : ca.RealTypeAttribute.Type;
        //    if (type.FullName.Contains("System.Int32"))
        //    {
        //        control = new NumericUpDown()
        //        {
        //            Maximum = 999999999999999999
        //        };
        //    }
        //    else if (type.FullName.Contains("System.Int64"))
        //    {
        //        control = new NumericUpDown()
        //        {
        //            Maximum = 999999999999999999
        //        };
        //    }
        //    else if (type.FullName.Contains("System.DateTime"))
        //    {
        //        control = new DateTimePicker
        //        {
        //            Format = DateTimePickerFormat.Short
        //        };
        //    }
        //    else if (type.FullName.Contains("System.Boolean"))
        //    {
        //        var dict = new List<string>();
        //        var dcda = _dataSourceObjectTypePropertyInfoTypes[pi].DataGridColumnDisplayAttribute;
        //        var format = "是|否";
        //        if (dcda != null && !string.IsNullOrEmpty(dcda.Format))
        //        {
        //            format = dcda.Format;
        //        }
        //        var formats = format.Split('|');
        //        foreach (var item in formats)
        //        {
        //            dict.Add(item);
        //        }
        //        control = new RadioButtonGroup
        //        {
        //            DataSource = dict
        //        };
        //    }
        //    else
        //    {
        //        control = new TextBox();
        //        var passwordAttr = ((PasswordPropertyTextAttribute[])pi.GetCustomAttributes(PasswordPropertyTextType, false)).FirstOrDefault();
        //        if (passwordAttr != null && passwordAttr.Password)
        //        {
        //            ((TextBox)control).PasswordChar = '*';
        //        }
        //    }
        //    return control;
        //}

        void DataGridViewEx_Load(object sender, EventArgs e)
        {

        }

        //void editForm_Load(object sender, EventArgs e)
        //{
        //    if (_action == "add")
        //    {
        //        foreach (var item in _addTypePis)
        //        {
        //            var control = _editForm.Controls["ctrl" + item.Name];
        //            if (control == null) continue;
        //            var ca = _addTypePropertyInfoTypes[item];
        //            if (ca.DefaultAttribute != null)
        //            {
        //                control.Text = ca.DefaultAttribute.Value.ToString();
        //            }
        //            else
        //            {
        //                control.ResetText();
        //            }
        //        }
        //    }
        //    else if (_action == "edit")
        //    {
        //        var o = _editForm.EditingModel;
        //        if (o == null) return;
        //        foreach (var item in _dataSourceObjectPis)
        //        {
        //            var control = _editForm.Controls["ctrl" + item.Name];
        //            if (control == null) continue;
        //            control.Text = Convert.ToString(item.GetValue(o, null));
        //        }
        //    }
        //}

        //Dictionary<PropertyInfo, object> Parse()
        //{
        //    var dict = new Dictionary<PropertyInfo, object>();
        //    PropertyInfo[] pis = _action == "add" ? _addTypePis : _editTypePis;
        //    Dictionary<PropertyInfo, ColumnAttributes> propertyColumnMap = _action == "add" ? _addTypePropertyInfoTypes : _editTypePropertyInfoTypes;
        //    foreach (var item in pis)
        //    {
        //        if (item.Name == "Id")
        //        {
        //            dict.Add(item, _dataSourceObjectPis.Single(x => x.Name == "Id").GetValue(EditForm.EditingModel, null));
        //            continue;
        //        }
        //        var control = EditForm.Controls["ctrl" + item.Name];
        //        if (control == null) continue;
        //        var ca = propertyColumnMap[item];
        //        var valiTypes = item.GetCustomAttributes(validateType, false);
        //        foreach (var valiAttr in valiTypes.Cast<ValidationAttribute>().Where(valiAttr => !valiAttr.IsValid(control.Text)))
        //        {
        //            if (!string.IsNullOrEmpty(valiAttr.ErrorMessage)) throw new Exception(valiAttr.ErrorMessage);
        //            var displayAttr = ca.DisplayAttribute;
        //            if (displayAttr == null)
        //            {
        //                if (ca.DataGridColumnDisplayAttribute != null)
        //                {
        //                    valiAttr.ErrorMessage = ca.DataGridColumnDisplayAttribute.DisplayName + "验证失败";
        //                }
        //                else
        //                    valiAttr.ErrorMessage = item.Name + "验证失败";
        //            }
        //            else
        //            {
        //                valiAttr.ErrorMessage = displayAttr.Name + "验证失败";
        //            }
        //            throw new Exception(valiAttr.ErrorMessage);
        //        }
        //        dict.Add(item, control.Text);
        //    }
        //    return dict;
        //}

        //void btnSave_Click(object sender, EventArgs e)
        //{
        //    if (_action == "add")
        //    {
        //        var dict = Parse();
        //        var o = Activator.CreateInstance(AddType);
        //        foreach (var item in dict)
        //        {
        //            var value = Convert.ChangeType(item.Value, item.Key.PropertyType);
        //            item.Key.SetValue(o, value, null);
        //        }
        //        if (OnSaveAdd != null)
        //        {
        //            if (OnSaveAdd(o))
        //            {
        //                EditForm.DialogResult = DialogResult.OK;
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show(this, "没有注册OnSaveAdd事件");
        //        }
        //    }
        //    else if (_action == "edit")
        //    {
        //        var dict = Parse();
        //        var o = Activator.CreateInstance(EditType);
        //        foreach (var item in dict)
        //        {
        //            var value = Convert.ChangeType(item.Value, item.Key.PropertyType);
        //            item.Key.SetValue(o, value, null);
        //        }
        //        if (OnSaveEdit != null)
        //        {
        //            if (OnSaveEdit(o))
        //            {
        //                EditForm.DialogResult = DialogResult.OK;
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show(this, "没有注册OnSaveEdit事件");
        //        }
        //    }
        //}

        #region 分页相关
        private void txtPageIndex_KeyPress(object sender, KeyPressEventArgs e)
        {
            preventNum(e);
        }
        void preventNum(KeyPressEventArgs e)
        {
            // 在当前输入框中按下并弹起按键的事件
            var keyAsciiValue = Convert.ToInt32(e.KeyChar); // 获取当前按下的按键对应的ASCII值

            // 如果当前ASCII值范围不在48-57之间、也就是0-9对应的ASCII值之间
            // 或者ASCII的值不是8[退格键]或13[回车键]时
            if (!(((keyAsciiValue >= 48 && keyAsciiValue <= 57) || (keyAsciiValue == 8 || keyAsciiValue == 13))))
            {   //如果上面的验证结果是false
                e.Handled = true;
            }
        }

        private void txtPageSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            preventNum(e);
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            Page = 1;
            RefreshData();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (Page <= 0) return;
            Page--;
            RefreshData();
        }


        private void btnNext_Click(object sender, EventArgs e)
        {
            if (Page >= PageCount) return;
            Page++;
            RefreshData();
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            Page = PageCount;
            RefreshData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void txtPageSize_KeyUp(object sender, KeyEventArgs e)
        {
            PageSize = Convert.ToInt32(txtPageSize.Text.Trim());
            if (e.KeyCode == Keys.Enter)
            {
                RefreshData();
            }
        }

        private void txtPageIndex_KeyUp(object sender, KeyEventArgs e)
        {
            Page = Convert.ToInt32(txtPageIndex.Text.Trim());
            if (e.KeyCode == Keys.Enter)
            {
                RefreshData();
            }
        }
        #endregion
        BinaryFormatter bf = new BinaryFormatter();
        public void RefreshData()
        {
            Enabled = false;
            Task.Factory.StartNew(() =>
            {
                if (!_enablePaging)
                    _pageSize = 10000;
                var args = new GridViewRefreshEventArgs(_page, _pageSize, _pageCount);
                //ms.Seek(0, SeekOrigin.Begin);
                //soms.Seek(0, SeekOrigin.Begin);
                if (OnRefresh != null) OnRefresh(this, args);
                //ms.Dispose();
                //soms.Dispose();
                ControlHelper.Invoke(this, () =>
                {
                    Total = args.Total;
                    ToggleButton();
                    Enabled = true;
                });
            });
            //var ms = new MemoryStream();
            //bf.Serialize(ms, _filters.ToArray());
            //var so = new Xinchen.Data.SortOrder();
            //switch (dataGridView1.SortOrder)
            //{
            //    case System.Windows.Forms.SortOrder.Ascending:
            //        so.Field = dataGridView1.SortedColumn.Name;
            //        so.Direction = Direction.ASC;
            //        break;
            //    case System.Windows.Forms.SortOrder.Descending:
            //        so.Field = dataGridView1.SortedColumn.Name;
            //        so.Direction = Direction.DESC;
            //        break;
            //    default:
            //        so.Field = _dataSourceObjectPis.First().Name;
            //        so.Direction = Direction.DESC;
            //        break;
            //}
            //var soms = new MemoryStream();
            //bf.Serialize(soms, so);
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!EnableEdit) return;
            var item = dataGridView1.SelectedRows[0].DataBoundItem;
            if (Edit != null)
            {
                Edit(sender, e);
            }
            //var rows = dataGridView1.SelectedRows;
            //if (rows.Count <= 0) return;
            //if (OnEdit != null)
            //{
            //    if (OnEdit(rows[0].DataBoundItem))
            //    {
            //        RefreshData();
            //    }
            //}
            //else
            //{
            //    _action = "edit";
            //    if (EditForm != null)
            //    {
            //        if (EditForm.Edit(rows[0].DataBoundItem))
            //            RefreshData();
            //    }
            //}
        }

        private void dataGridView1_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            toolStripPaging.Refresh();
        }

        private PropertyInfo[] _dataSourceObjectPis, _addTypePis, _editTypePis;
        private Dictionary<PropertyInfo, ColumnAttributes> _dataSourceObjectTypePropertyInfoTypes, _addTypePropertyInfoTypes, _editTypePropertyInfoTypes;
        //private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    _headerColumn = dataGridView1.Columns[e.ColumnIndex];
        //    if (e.Button == System.Windows.Forms.MouseButtons.Right)
        //    {
        //        if (menuCols.DropDownItems.Count <= 0)
        //        {
        //            foreach (var item in _dataSourceObjectTypePropertyInfoTypes)
        //            {
        //                var columnDisplayAttr = item.Value.DataGridColumnDisplayAttribute;
        //                var menu = new ToolStripMenuItem();
        //                menu.Name = item.Key.Name;
        //                menu.Text = columnDisplayAttr == null ? item.Key.Name : columnDisplayAttr.DisplayName;
        //                if (columnDisplayAttr != null) menu.Checked = columnDisplayAttr.Visible;
        //                menuCols.DropDownItems.Add(menu);
        //            }
        //            menuCols.DropDownItemClicked += menuCols_DropDownItemClicked;
        //        }

        //        var columnFilter = (DynamicFilter)_headerColumn.Tag;
        //        menuFilter.Checked = columnFilter != null;
        //        var pi = _dataSourceObjectPis.First(x => x.Name == _headerColumn.Name);
        //        var list = (IList)DataSource;
        //        var menus = new List<string>();
        //        menuFilter.DropDownItems.Clear();
        //        var menuCustomer = new ToolStripMenuItem { Name = "menuCustomer", Text = "自定义" };
        //        var menuCancel = new ToolStripMenuItem { Name = "menuCancel", Text = "取消过滤" };
        //        menuFilter.DropDownItems.Add(menuCustomer);
        //        menuFilter.DropDownItems.Add(menuCancel);
        //        if (pi.PropertyType.FullName.Contains("System.Boolean"))
        //        {
        //            var ca = _dataSourceObjectTypePropertyInfoTypes[pi];
        //            var format = "勾选|未勾选";
        //            if (ca.DataGridColumnDisplayAttribute != null && !string.IsNullOrEmpty(ca.DataGridColumnDisplayAttribute.Format))
        //            {
        //                format = ca.DataGridColumnDisplayAttribute.Format;
        //            }
        //            var formats = format.Split('|');
        //            var menu = new ToolStripMenuItem { Text = formats[0], Tag = true };
        //            var menu1 = new ToolStripMenuItem { Text = formats[1], Tag = false };
        //            menuFilter.DropDownItems.Add(menu);
        //            menuFilter.DropDownItems.Add(menu1);
        //        }
        //        else
        //        {
        //            foreach (var value in from object item in list select pi.GetValue(item, null) into val where val != null select FormatDisplayValue(pi, val) into value where !menus.Contains(value) select value)
        //            {
        //                menus.Add(value);
        //            }
        //            if (menus.Count <= 100)
        //            {
        //                foreach (var item in menus)
        //                {
        //                    var menu = new ToolStripMenuItem();
        //                    menu.Text = item;
        //                    menu.Tag = item;
        //                    menuFilter.DropDownItems.Add(menu);
        //                }
        //            }
        //        }
        //        contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
        //    }
        //    else if (e.Button == MouseButtons.Left)
        //    {
        //        RefreshData();
        //    }
        //}

        //void menuCols_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        //{
        //    var menuItem = (ToolStripMenuItem)e.ClickedItem;
        //    if (menuItem.Checked)
        //    {
        //        var count = dataGridView1.Columns.Cast<DataGridViewColumn>().Count(item => item.Visible);
        //        if (count <= 1) return;
        //    }
        //    var field = e.ClickedItem.Name;
        //    var pi = _dataSourceObjectPis.First(x => x.Name == field);
        //    menuItem.Checked = !menuItem.Checked;
        //    _dataSourceObjectTypePropertyInfoTypes[pi].DataGridColumnDisplayAttribute.Visible = menuItem.Checked;
        //    var dataGridViewColumn = dataGridView1.Columns[field];
        //    if (dataGridViewColumn != null) dataGridViewColumn.Visible = menuItem.Checked;
        //}

        //void menuFilter_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        //{
        //    switch (e.ClickedItem.Name)
        //    {
        //        case "menuCustomer":
        //            var panel = (Panel)filterForm.Controls["panel1"];
        //            panel.Controls.Clear();
        //            var pi = _dataSourceObjectPis.First(x => x.Name == _headerColumn.Name);
        //            var editor = GetFilterEditorByType(pi);
        //            editor.Dock = DockStyle.Fill;
        //            panel.Controls.Add(editor);
        //            panel = (Panel)filterForm.Controls["panel2"];
        //            panel.Controls.Clear();
        //            editor = GetFilterEditorByType(pi);
        //            editor.Dock = DockStyle.Fill;
        //            panel.Controls.Add(editor);
        //            filterForm.PropertyInfo = pi;
        //            filterForm.Filter = (DynamicFilter)_headerColumn.Tag;
        //            if (filterForm.ShowDialog(this) == DialogResult.OK)
        //            {
        //                RemoveFilter();
        //                _headerColumn.Tag = filterForm.Filter;
        //                if (filterForm.Filter != null)
        //                {
        //                    _filters.Add(filterForm.Filter);
        //                    _headerColumn.HeaderText = _headerColumn.HeaderText + " *";
        //                }
        //                else
        //                {
        //                    _headerColumn.HeaderText = _headerColumn.HeaderText.Replace(" *", "");
        //                }
        //            }
        //            break;
        //        case "menuCancel":
        //            RemoveFilter();
        //            _headerColumn.Tag = null;
        //            _headerColumn.HeaderText = _headerColumn.HeaderText.Replace(" *", "");
        //            break;
        //        default:
        //            RemoveFilter();
        //            var fitler = new DynamicFilter
        //            {
        //                Name = _headerColumn.Name,
        //                Filters = { Left = new KeyValuePair<Oper, object>(Oper.Equal, e.ClickedItem.Tag) }
        //            };
        //            _headerColumn.Tag = fitler;
        //            _filters.Add(fitler);
        //            _headerColumn.HeaderText = _headerColumn.HeaderText + " *";
        //            break;
        //    }
        //    RefreshData();
        //}

        //void RemoveFilter()
        //{
        //    List<DynamicFilter> _filters = this._filters.Where(x => x.Name == _headerColumn.Name).ToList();
        //    for (var i = _filters.Count - 1; i >= 0; i--)
        //    {
        //        this._filters.Remove(this._filters[i]);
        //    }
        //}


        //DataGridViewEditForm _editForm;

        //public DataGridViewEditForm EditForm
        //{
        //    get { return _editForm; }
        //    set { _editForm = value; }
        //}

        //private List<DynamicFilter> _filters;

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (Add != null) Add(sender, e);
            //if (EditForm == null) return;
            //_action = "add";
            //if (EditForm.Add())
            //{
            //    RefreshData();
            //}
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var rows = dataGridView1.SelectedRows;
            if (rows.Count <= 0) return;
            if (OnRemove != null)
            {
                if (MessageBox.Show(this, "无法恢复，确认删除？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (OnRemove((from DataGridViewRow item in rows select item.DataBoundItem).ToArray()))
                    {
                        RefreshData();
                    }
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void menuFilter_Click(object sender, EventArgs e)
        {
            //if (_headerColumn != null)
            //{
            //}
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var field = dataGridView1.Columns[e.ColumnIndex].Name;
            var pi = _dataSourceObjectPis.FirstOrDefault(x => x.Name == field);
            if (pi == null) return;
            //dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex]...FormattedValueType = typeof (string);
            e.Value = FormatDisplayValue(pi, e.Value);
            //e.FormattingApplied = true;
        }

        string FormatDisplayValue(PropertyInfo pi, object value)
        {
            var ca = _dataSourceObjectTypePropertyInfoTypes[pi];
            var dcda = ca.DataGridColumnDisplayAttribute;
            string format = dcda == null ? "" : dcda.Format, emptyText = dcda == null ? "" : dcda.EmptyText;
            if (value == null) return string.IsNullOrEmpty(emptyText) ? "" : emptyText;
            if (pi.PropertyType.FullName.Contains("System.DateTime"))
            {
                return Convert.ToDateTime(value).ToString(format);
            }
            if (pi.PropertyType.FullName.Contains("System.Single"))
            {
                return Convert.ToSingle(value).ToString(format);
            }
            if (pi.PropertyType.FullName.Contains("System.Double"))
            {
                return Convert.ToDouble(value).ToString(format);
            }
            if (pi.PropertyType.FullName.Contains("System.Decimal"))
            {
                return Convert.ToDecimal(value).ToString(format);
            }
            if (pi.PropertyType.FullName.Contains("System.String"))
            {
                return Convert.ToString(value);
            }
            if (pi.PropertyType.FullName.Contains("System.Int32"))
            {
                if (dcda.ValueType != null)
                {
                    var names = Enum.GetNames(dcda.ValueType);
                    var nameList = new List<string>();
                    foreach (var item in names)
                    {
                        var field = dcda.ValueType.GetField(item);
                        var descAttr = AttributeHelper.GetAttribute<DescriptionAttribute>(field);
                        nameList.Add(descAttr == null ? item : descAttr.Description);
                    }
                    var values = Enum.GetValues(dcda.ValueType);
                    var valueList = new List<int>();
                    for (int i = 0; i < values.Length; i++)
                    {
                        valueList.Add((int)values.GetValue(i));
                    }
                    return nameList[valueList.IndexOf((int)value)];
                }
                return Convert.ToInt32(value).ToString(format);
            }
            if (pi.PropertyType.FullName.Contains("System.Int64"))
            {
                return Convert.ToInt64(value).ToString(format);
            }
            if (pi.PropertyType.FullName.Contains("System.Int16"))
            {
                return Convert.ToInt16(value).ToString(format);
            }
            if (pi.PropertyType.FullName.Contains("System.Boolean"))
            {
                bool b = Convert.ToBoolean(value);
                if (string.IsNullOrWhiteSpace(format)) return b.ToString();
                string[] formats = format.Split('|');
                return b ? formats.FirstOrDefault() : formats.LastOrDefault();
            }
            throw new NotImplementedException("未处理的类型：" + pi.PropertyType.FullName);
        }


        private ToolStripButton[] _extraToolStripButtons;

        public ToolStripButton[] ExtraToolStripButtons
        {
            get { return _extraToolStripButtons; }
            set
            {
                if (value != null)
                {
                    foreach (var extraToolStripButton in value)
                    {
                        toolStripToolbar.Items.Add(extraToolStripButton);
                    }
                }
                else if (_extraToolStripButtons != null)
                {
                    foreach (var extraToolStripButton in _extraToolStripButtons)
                    {
                        toolStripToolbar.Items.Remove(extraToolStripButton);
                    }
                }
                _extraToolStripButtons = value;
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
            e.Cancel = true;
        }

        public void TurnToPage(int p)
        {
            Page = p;
            RefreshData();
        }

        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    btnAdd.Visible = EnableAdd;
        //    btnRemove.Visible = EnableRemove;
        //    btnFirst.Visible = btnLast.Visible = btnNext.Visible = btnNext.Visible = btnPrev.Visible = lblPageCount.Visible = lblEveryPage.Visible = txtPageSize.Visible = txtPageIndex.Visible = toolStripLabel2.Visible = toolStripLabel3.Visible = toolStripLabel5.Visible = toolStripLabel6.Visible = EnablePaging;
        //    if (Icon != null)
        //        filterForm.Icon = Icon;
        //    if (_dataSourceObjectPis == null) return;
        //    if (!DesignMode)
        //        if (EditForm == null)
        //        {
        //            EditForm = new DataGridViewEditForm();
        //            _editForm.Load += editForm_Load;
        //            int padding = 20, count = 0, height = 0, controlWidth = 0, lblWidth = 100;
        //            for (var i = 0; i < _addTypePis.Length; i++)
        //            {
        //                var item = _addTypePis[i];
        //                var ca = _addTypePropertyInfoTypes[item];
        //                if (ca.KeyAttribute != null) continue;
        //                var lbl = new Label();
        //                height = lbl.Height;
        //                lbl.Width = lblWidth;
        //                var displayAttr = ca.DisplayAttribute;
        //                if (displayAttr != null)
        //                {
        //                    var gen = displayAttr.GetAutoGenerateField();
        //                    if (gen != null && !gen.Value)
        //                    {
        //                        continue;
        //                    }
        //                    lbl.Text = displayAttr.Name + "：";
        //                }
        //                else
        //                {
        //                    if (ca.DataGridColumnDisplayAttribute != null)
        //                    {
        //                        lbl.Text = ca.DataGridColumnDisplayAttribute.DisplayName + "：";
        //                    }
        //                    else
        //                    {
        //                        lbl.Text = item.Name + "：";
        //                    }
        //                }
        //                lbl.Top = padding + lbl.Height * count;
        //                if (count > 0)
        //                {
        //                    lbl.Top += 10 * count;
        //                }
        //                lbl.Left = padding;
        //                lbl.TextAlign = ContentAlignment.MiddleRight;
        //                EditForm.Controls.Add(lbl);
        //                Control control;
        //                if (ca.DataSourceAttribute != null)
        //                {
        //                    ComboBox comboBox = new ComboBox();
        //                    comboBox.DataSource = ca.DataSourceAttribute.SimpleData;
        //                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        //                    control = comboBox;
        //                }
        //                else
        //                {
        //                    control = GetEditorByType(item);
        //                }
        //                control.Width = 150;
        //                control.Top = padding + lbl.Height * count;
        //                if (count > 0)
        //                {
        //                    control.Top += 10 * count;
        //                }
        //                control.Left = lbl.Width + padding;
        //                if (ca.DefaultAttribute != null)
        //                {
        //                    control.Text = ca.DefaultAttribute.Value.ToString();
        //                }
        //                count++;
        //                controlWidth = control.Width;
        //                control.Name = "ctrl" + item.Name;
        //                EditForm.Controls.Add(control);
        //            }
        //            EditForm.Height = (count + 1) * (height + 10) + padding * 3;
        //            EditForm.Width = padding + padding + lblWidth + padding * 2 + controlWidth + padding;
        //            var btnSave = new Button();
        //            btnSave.Top = EditForm.Height - padding * 2 - btnSave.Height - 10;
        //            btnSave.Text = "保存";
        //            btnSave.Left = (EditForm.Width - btnSave.Width) / 2;
        //            btnSave.Click += btnSave_Click;
        //            EditForm.Controls.Add(btnSave);
        //        }
        //}

        private int _radius = 5;

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var rectangle = new Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                dataGridView1.RowHeadersWidth - 4,
                e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(CultureInfo.InvariantCulture),
                dataGridView1.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }
        [DefaultValue(10)]
        public int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                base.Refresh();
            }
        }

        public Type AddType
        {
            get { return addType; }
            set
            {
                if (value == null) return;
                addType = value;
                _addTypePis = addType.GetProperties();
                _addTypePropertyInfoTypes = initProperties(_addTypePis);
            }
        }

        public Type EditType
        {
            get
            {
                return editType;
            }
            set
            {
                if (value == null) return;
                editType = value;
                _editTypePis = editType.GetProperties();
                _editTypePropertyInfoTypes = initProperties(_editTypePis);
            }
        }
    }
}
