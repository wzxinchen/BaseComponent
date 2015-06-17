namespace Xinchen.ExtNetBase.Mvc
{
    using Ext.Net;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Web.UI;
    using Xinchen.ModelBase;
    using Xinchen.Utils;
    using Xinchen.Utils.DataAnnotations;

    public class GridPanelEx : GridPanel
    {
        private Window _editWindow;
        private int _pageSize = 50;
        private Store _store;

        public event ComponentDirectEvent.DirectEventHandler Delete;

        public string ReadUrl { get; set; }
        //public string CommandUrl { get; set; }
        //public event SyncStoreBase.AjaxReadDataEventHandler ReadData;
        public string OnBeforeLoad;

        public GridPanelEx()
        {
            this.EnableAdd = true;
            this.EnableEdit = true;
            this.EnableRemove = true;
        }

        private void btnAdd_DirectClick(object sender, DirectEventArgs e)
        {
            if (this.EditorConfig == null)
            {
                throw new SettingsPropertyNotFoundException("EditorConfig未设置");
            }
            if (!this.EnableAdd)
            {
                Ext.Net.X.Msg.Alert("错误", "无法添加记录").Show();
            }
            else if (this.EditorConfig.Mode != EditorMode.Inline)
            {
                WindowConfig addWindow = this.EditorConfig.AddWindow;
                string str = this.Page.ResolveClientUrl(addWindow.Url ?? "");
                foreach (Parameter parameter in this.EditorConfig.ExtraParams)
                {
                    str = str.Replace("{" + parameter.Name + "}", e.ExtraParams[parameter.Name]);
                }
                this._editWindow.Height = addWindow.Height;
                this._editWindow.Width = addWindow.Width;
                this._editWindow.Title = addWindow.Title;
                _editWindow.Icon = Ext.Net.Icon.ApplicationAdd;
                this._editWindow.Loader.Url = str;
                foreach (Parameter parameter in this.EditorConfig.AddWindow.ExtraParams)
                {
                    this._editWindow.Loader.Url = this._editWindow.Loader.Url.Replace("{" + parameter.Name + "}", e.ExtraParams[parameter.Name]);
                }
                this._editWindow.Show();
                this._editWindow.Loader.LoadContent();
            }
        }

        private void btnRemove_DirectClick(object sender, DirectEventArgs e)
        {
            if ((this.Delete != null) && this.EnableRemove)
            {
                this.Delete(sender, e);
            }
        }

        private void CheckEditorConfig()
        {
            if (this.EditorConfig == null)
            {
                throw new SettingsPropertyNotFoundException("EditorConfig属性未设置");
            }
            if (this.EditorConfig.AddWindow == null)
            {
                throw new SettingsPropertyNotFoundException("未设置添加窗口样式");
            }
            if (this.EditorConfig.EditWindow == null)
            {
                throw new SettingsPropertyNotFoundException("未设置编辑窗口样式");
            }
        }

        public DataSorterCollection Sorters { get; set; }

        //private void Command_Event(object sender, DirectEventArgs e)
        //{
        //    string str = e.ExtraParams["command"];
        //    string str2 = e.ExtraParams["id"];
        //    if (str == "edit")
        //    {
        //        if (!this.EnableEdit)
        //        {
        //            Ext.Net.X.Msg.Alert("错误", "可能由于权限不足，无法编辑").Show();
        //        }
        //        else
        //        {
        //            this.CheckEditorConfig();
        //            _editWindow.Icon = Ext.Net.Icon.ApplicationEdit;
        //            this._editWindow.Title = this.EditorConfig.EditWindow.Title;
        //            this._editWindow.Width = this.EditorConfig.EditWindow.Width;
        //            this._editWindow.Height = this.EditorConfig.EditWindow.Height;
        //            this._editWindow.Loader.Url = this.EditorConfig.EditWindow.Url;
        //            this._editWindow.Loader.Url = this.ReplaceExtraParams(this._editWindow.Loader.Url, this.EditorConfig.ExtraParams, e.ExtraParams);
        //            this._editWindow.Loader.Url = this.ReplaceExtraParams(this._editWindow.Loader.Url, this.EditorConfig.EditWindow.ExtraParams, e.ExtraParams);
        //            this._editWindow.Show();
        //            this._editWindow.Loader.LoadContent();
        //        }
        //    }
        //    else if ((str == "delete") && (this.Delete != null))
        //    {
        //        this.Delete(sender, e);
        //    }
        //}

        private string GetFormat(DataGridColumnAttribute displayAttr, string defaultFormat)
        {
            if (!((displayAttr == null) || string.IsNullOrEmpty(displayAttr.Format)))
            {
                return displayAttr.Format;
            }
            return defaultFormat;
        }

        private void InitGridCommand()
        {
            if ((this.EditorConfig != null) && (this.EditorConfig.Mode == EditorMode.Window))
            {
                Window window = new Window
                {
                    AutoShow = false,
                    Hidden = true,
                    CloseAction = CloseAction.Hide,
                    Closable = true,
                    ConstrainHeader = true,
                    Constrain = true,
                    Modal = true,
                    BodyPadding = 10
                };
                ComponentLoader loader = new ComponentLoader
                {
                    DisableCaching = true,
                    AutoLoad = false,
                    Mode = LoadMode.Frame
                };
                window.Loader = loader;
                this._editWindow = window;
                this._editWindow.Loader.LoadMask.ShowMask = true;
                this._editWindow.Loader.LoadMask.Msg = "正在加载";
                this._editWindow.ID = "editWindow";
                _editWindow.Resizable = false;
                _editWindow.Icon = Ext.Net.Icon.UserAdd;
                this.Controls.Add(this._editWindow);
                CommandColumn item = (CommandColumn)this.ColumnModel.Columns.FirstOrDefault<ColumnBase>(x => (x is CommandColumn));
                GridCommand command = null;
                if (this.EnableEdit)
                {
                    command = new GridCommand
                    {
                        Text = "编辑",
                        Icon = Ext.Net.Icon.ApplicationEdit,
                        CommandName = "edit"
                    };
                }
                GridCommand commandRemove = null;
                if (this.EnableRemove)
                {
                    commandRemove = new GridCommand
                    {
                        Text = "删除",
                        Icon = Ext.Net.Icon.ApplicationDelete,
                        CommandName = "delete"
                    };
                }
                if (item == null)
                {
                    item = new CommandColumn();
                    item.DirectEvents.Command.Confirmation.ConfirmRequest = true;
                    item.DirectEvents.Command.Confirmation.Message = "确认删除？";
                    item.DirectEvents.Command.Confirmation.Title = "提示";
                    item.DirectEvents.Command.Confirmation.BeforeConfirm = "if(command=='delete')return true;return false;";
                    item.Width = 150;
                    if (this.EnableEdit)
                    {
                        item.Commands.Add(command);
                    }
                    if (this.EnableRemove)
                    {
                        item.Commands.Add(commandRemove);
                    }
                    this.ColumnModel.Columns.Insert(0, item);
                    this.InitGridCommand(item);
                    if (EditorConfig.EditWindow != null)
                    {
                        var replaceParamsScripts = new List<string>();
                        var scriptTpl = "url=url.replace('{0}',{1});";
                        foreach (var paras in EditorConfig.EditWindow.ExtraParams)
                        {
                            var script = string.Format(scriptTpl, "@" + paras.Name, paras.Value);
                            replaceParamsScripts.Add(script);
                        }
                        item.Listeners.Command.Handler = "if(command==\"edit\"){var url='" + EditorConfig.EditWindow.Url + "';" + string.Join(";", replaceParamsScripts) + "var editWindow=App." + _editWindow.ID + ";editWindow.show();editWindow.loader.load({url:url});editWindow.setTitle(\"" + EditorConfig.EditWindow.Title + "\");editWindow.setWidth(" + EditorConfig.EditWindow.Width + ");editWindow.setHeight(" + EditorConfig.EditWindow.Height + ");editWindow.center();}else{Ext.Msg.confirm(\"提示\",\"确认删除吗？\",function(r){if(r==\"yes\"){               }})}";
                        //item.DirectEvents.Command.ExtraParams.AddRange(this.EditorConfig.EditWindow.ExtraParams);
                    }
                }
                else
                {
                    GridCommand commandEdit = (GridCommand)item.Commands.FirstOrDefault<GridCommandBase>(x => (((GridCommand)x).CommandName == "edit"));
                    if (commandEdit == null)
                    {
                        commandEdit = command;
                        this.InitGridCommand(item);
                        if (EnableRemove)
                            item.Commands.Insert(0, commandRemove);
                        if (EnableEdit)
                            item.Commands.Insert(0, commandEdit);
                        if (EditorConfig.EditWindow != null)
                            item.DirectEvents.Command.ExtraParams.AddRange(this.EditorConfig.EditWindow.ExtraParams);
                    }
                }
            }
        }

        private void InitGridCommand(CommandColumn commandColumn)
        {
            commandColumn.ID = "_cmdCol";
            commandColumn.Sortable = false;
            commandColumn.Listeners.Command.Handler = "";//.DirectEvents.Command.EventMask.Msg = "正在加载";
            //commandColumn.DirectEvents.Command.EventMask.ShowMask = true;
            //commandColumn.DirectEvents.Command.Url = CommandUrl;//.Event += new ComponentDirectEvent.DirectEventHandler(this.Command_Event);
            ParameterCollection extraParams = commandColumn.DirectEvents.Command.ExtraParams;
            if (this.EditorConfig != null)
            {
                Parameter parameters = new Parameter
                {
                    Name = "command",
                    Value = "command",
                    Mode = ParameterMode.Raw
                };
                this.EditorConfig.ExtraParams.Add(parameters);
                Parameter parameter3 = new Parameter
                {
                    Name = "id",
                    Value = "record.data.Id",
                    Mode = ParameterMode.Raw
                };
                this.EditorConfig.ExtraParams.Add(parameter3);
                foreach (Parameter parameter in this.EditorConfig.ExtraParams)
                {
                    Parameter param1 = parameter;
                    if (extraParams.All<Parameter>(x => x.Name != param1.Name))
                    {
                        extraParams.Add(parameter);
                    }
                }
            }
        }

        private void InitModel()
        {
            if (this.ItemType != null)
            {
                Model item = new Model();
                PropertyInfo[] infoArray2 = this.ItemType.GetProperties();
                GridFilters filters = new GridFilters();
                filters.MenuFilterText = "搜索";
                filters.ID = "filters" + ID;
                for (int i = 0; i < infoArray2.Length; i++)
                {
                    Func<ColumnBase, bool> predicate = null;
                    PropertyInfo property = infoArray2[i];
                    ModelField field = new ModelField
                    {
                        Name = property.Name
                    };
                    #region 映射字段类型
                    string fullName = property.PropertyType.FullName;
                    if (fullName.Contains("System.Int"))
                    {
                        var enumDataTypeAttr = AttributeHelper.GetAttribute<EnumDataTypeAttribute>(property);
                        if (enumDataTypeAttr == null)
                        {
                            field.Type = ModelFieldType.Int;
                        }
                    }
                    else if (fullName.Contains("System.DateTime"))
                    {
                        field.Type = ModelFieldType.Date;
                    }
                    else if (fullName.Contains("System.Single"))
                    {
                        field.Type = ModelFieldType.Float;
                    }
                    else if (fullName.Contains("System.Boolean"))
                    {
                        field.Type = ModelFieldType.Boolean;
                    }
                    else if (fullName.Contains("System.String"))
                    {
                        field.Type = ModelFieldType.String;
                    }
                    else if (!property.PropertyType.IsPrimitive)
                    {
                        field.Type = ModelFieldType.Object;
                    }
                    else
                    {
                        field.Type = ModelFieldType.Auto;
                    }
                    #endregion
                    item.Fields.Add(field);

                    if (predicate == null)
                    {
                        predicate = x => x.DataIndex == property.Name;
                    }
                    ColumnBase column = this.ColumnModel.Columns.FirstOrDefault<ColumnBase>(predicate);
                    if (column == null)
                    {
                        ColumnBase base2;
                        DataGridColumnAttribute displayAttr = AttributeHelper.GetAttribute<DataGridColumnAttribute>(property);
                        FilterAttribute filterAttr = AttributeHelper.GetAttribute<FilterAttribute>(property);
                        bool simpleFilterable = filterAttr != null && filterAttr.Enabled && filterAttr.FilterType == null;
                        GridFilter filter = null;
                        #region 映射列类型
                        if (fullName.Contains("System.Int")
                                            || fullName.Contains("System.Single")
                                            || fullName.Contains("System.Decimal")
                                            || fullName.Contains("System.Double"))
                        {
                            NumberColumn column1 = new NumberColumn
                            {
                                Format = this.GetFormat(displayAttr, "0")
                            };
                            base2 = column1;
                            if (simpleFilterable && filterAttr.FilterType == null)
                            {
                                filter = new NumericFilter();
                            }
                        }
                        else if (fullName.Contains("System.DateTime"))
                        {
                            DateColumn column2 = new DateColumn
                            {
                                Format = this.GetFormat(displayAttr, "Y-m-d")
                            };
                            base2 = column2;
                            if (simpleFilterable)
                            {
                                filter = new DateFilter()
                                {
                                    AfterText = "在这之后",
                                    BeforeText = "在这之前",
                                    OnText = "在这天"
                                };
                            }
                        }
                        else if (fullName.Contains("System.Boolean"))
                        {
                            string[] strArray = this.GetFormat(displayAttr, "是|否").Split(new char[] { '|' });
                            BooleanColumn column4 = new BooleanColumn
                            {
                                TrueText = strArray[0],
                                FalseText = strArray[1]
                            };
                            base2 = column4;
                            if (simpleFilterable)
                            {
                                filter = new BooleanFilter()
                                {
                                    NoText = strArray[1],
                                    YesText = strArray[0]
                                };
                            }
                        }
                        else if (fullName.Contains("System.String"))
                        {
                            base2 = new Column();
                            if (simpleFilterable)
                            {
                                filter = new StringFilter();
                            }
                        }
                        else if (!property.PropertyType.IsPrimitive)
                        {
                            base2 = new Column();
                            if (simpleFilterable)
                            {
                                filter = new StringFilter();
                            }
                        }
                        else
                        {
                            base2 = new Column();
                            if (simpleFilterable)
                            {
                                filter = new StringFilter();
                            }
                        }
                        #endregion
                        #region 生成外键过滤器
                        if (filterAttr != null && filterAttr.Enabled && filterAttr.FilterType != null)
                        {
                            Store store = new Store();
                            store.ID = "filterStore" + property.Name;
                            if (filterAttr.FilterType.IsEnum)
                            {
                                var valueType = filterAttr.FilterType;
                                var values = Enum.GetValues(valueType);
                                var names = Enum.GetNames(valueType);
                                List<KeyValueModel> enumDict = new List<KeyValueModel>();
                                for (int it = 0; it < values.Length; it++)
                                {
                                    var fieldInfo = valueType.GetField(names[it]);
                                    var descAttr = AttributeHelper.GetAttribute<DescriptionAttribute>(fieldInfo);
                                    if (descAttr == null)
                                    {
                                        throw new Exception("枚举必须要有Description");
                                    }
                                    int key = (int)values.GetValue(it);
                                    enumDict.Add(new KeyValueModel()
                                    {
                                        Id = key,
                                        Name = descAttr.Description
                                    });
                                }
                                Model model = new Model();
                                model.Fields.Add(new ModelField("Id", ModelFieldType.Int));
                                model.Fields.Add(new ModelField("Name", ModelFieldType.String));
                                store.Model.Add(model);
                                store.DataSource = enumDict;
                                store.DataBind();
                                filter = new ListFilter()
                                {
                                    StoreID = store.ID,
                                    Single = true,
                                    IDField = "Id",
                                    LabelField = "Name"
                                };
                            }
                            else
                            {
                                var filterObject = Activator.CreateInstance(filterAttr.FilterType);
                                var filterGen = filterObject as ForeignFilterBase;
                                if (filterGen == null)
                                {
                                    throw new ArgumentException("FilterAttribute中的FilterType的类型必须为ForeignFilterBase的子类或枚举类型");
                                }
                                store.Model.Add(filterGen.GetModel());
                                store.DataSource = filterGen.GetData();
                                store.DataBind();
                                filter = new ListFilter()
                                {
                                    StoreID = store.ID,
                                    Single = true,
                                    IDField = filterGen.IdField,
                                    LabelField = filterGen.LabelField
                                };
                            }
                            Bin.Add(store);
                        }
                        #endregion
                        base2.Text = (displayAttr == null) ? property.Name : displayAttr.DisplayName;
                        base2.DataIndex = property.Name;
                        if (displayAttr != null && displayAttr.Width != -1)
                            base2.Width = displayAttr.Width;
                        if (displayAttr != null && displayAttr.ValueType != null)
                        {
                            Type valueType = displayAttr.ValueType;
                            var values = Enum.GetValues(valueType);
                            var names = Enum.GetNames(valueType);
                            Dictionary<int, string> enumDict = new Dictionary<int, string>();
                            List<string> valueList = new List<string>();
                            for (int it = 0; it < values.Length; it++)
                            {
                                var fieldInfo = valueType.GetField(names[it]);
                                var descAttr = AttributeHelper.GetAttribute<DescriptionAttribute>(fieldInfo);
                                int key = (int)values.GetValue(it);
                                enumDict.Add(key, descAttr.Description);
                                valueList.Add("values[\"" + names[it] + "\"]=\"" + (descAttr != null ? descAttr.Description : names[it]) + "\";");
                            }

                            base2.Renderer.Fn = "function(){var values=Array();" + string.Join(string.Empty, valueList) + "return values[arguments[0]];}";
                        }
                        if (filter != null)
                        {
                            filter.DataIndex = property.Name;
                            filters.Filters.Add(filter);
                        }
                        column = base2;
                        this.ColumnModel.Columns.Add(base2);
                    }
                    column.TabIndex = (short)i;
                }
                Features.Add(filters);
                if (this.ColumnModel.Columns.Count >= 10)
                {
                    this.AutoScroll = true;
                }
                var list = ColumnModel.Columns.OrderBy(x => x.TabIndex).ToList();
                ColumnModel.Columns.Clear();
                ColumnModel.Columns.AddRange(list);
                this._store.Model.Add(item);
            }
        }

        private void InitStore()
        {
            this._store = new Store();
            _store.ID = ID + "Store";
            this._store.RemoteFilter = true;
            this._store.RemoteGroup = true;
            this._store.RemotePaging = true;
            this._store.RemoteSort = true;
            this._store.Proxy.Clear();
            if (Sorters != null)
            {
                _store.Sorters.AddRange(Sorters);
            }
            var readProxy = new AjaxProxy()
            {
                Url = ReadUrl
            };
            readProxy.ActionMethods.Read = HttpMethod.POST;
            readProxy.Reader.Add(new JsonReader()
            {
                Root = "data",
                TotalProperty = "tota"
            });
            this._store.Proxy.Add(readProxy);
            //this._store.ReadData += new SyncStoreBase.AjaxReadDataEventHandler(this.store_ReadData);
            if (!string.IsNullOrEmpty(OnBeforeLoad))
            {
                _store.On("beforeload", OnBeforeLoad);
            }
            this._store.PageSize = this.PageSize;
            this.Store.Add(this._store);
        }

        protected override void OnInit(EventArgs e)
        {
            this.InitStore();
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GridView item = View.FirstOrDefault();
            if (item == null)
            {
                item = new GridView();
                item.EnableTextSelection = true;
                this.View.Add(item);
            }
            item.ID = this.ID + "View";
            item.LoadingText = "正在加载";
            item.LoadMask = true;
            PagingToolbar toolbar = new PagingToolbar();
            Ext.Net.Button btnAdd = null;
            Ext.Net.Button button2 = null;
            if (this.EnableAdd)
            {
                if (EditorConfig == null || EditorConfig.AddWindow == null)
                {
                    throw new Exception("EditorConfig的属性有误");
                }
                btnAdd = new Ext.Net.Button
                {
                    Text = "添加"
                };
                btnAdd.Handler = string.Format("App.editWindow.setWidth({0});App.editWindow.setHeight({1});App.editWindow.setTitle('{2}');App.editWindow.show();App.editWindow.loader.load({{url:'{3}'}})", EditorConfig.AddWindow.Width, EditorConfig.AddWindow.Height, EditorConfig.AddWindow.Title, EditorConfig.AddWindow.Url);
                toolbar.Items.Add(btnAdd);
            }
            if (this.EnableRemove)
            {
                button2 = new Ext.Net.Button
                {
                    Text = "删除"
                };
                button2.DirectEvents.Click.EventMask.Msg = "正在删除";
                button2.DirectEvents.Click.EventMask.ShowMask = true;
                button2.DirectEvents.Click.Confirmation.ConfirmRequest = true;
                button2.DirectEvents.Click.Confirmation.Message = "确认删除？";
                button2.DirectEvents.Click.Confirmation.Title = "提示";
                Parameter parameters = new Parameter
                {
                    Name = "ids",
                    Value = "getSelectionIds()",
                    Mode = ParameterMode.Raw
                };
                button2.DirectEvents.Click.ExtraParams.Add(parameters);
            }
            if (this.EditorConfig != null)
            {
                if (this.EnableAdd)
                {
                    btnAdd.DirectEvents.Click.ExtraParams.AddRange(this.EditorConfig.ExtraParams);
                    btnAdd.DirectEvents.Click.ExtraParams.AddRange(this.EditorConfig.AddWindow.ExtraParams);
                }
                if (this.EnableRemove)
                {
                    button2.DirectEvents.Click.ExtraParams.AddRange(this.EditorConfig.ExtraParams);
                    button2.DirectEvents.Click.ExtraParams.AddRange(this.EditorConfig.AddWindow.ExtraParams);
                }
            }
            if (this.EnableAdd)
            {
            }
            if (this.EnableRemove)
            {
                button2.DirectClick += new ComponentDirectEvent.DirectEventHandler(this.btnRemove_DirectClick);
                toolbar.Items.Add(button2);
            }
            if (BottomBar.Count > 0)
            {
                toolbar.Items.AddRange(BottomBar[0].Items);
                BottomBar.Clear();
            }
            this.BottomBar.Add(toolbar);
            this.SelectionModel.Add(new CheckboxSelectionModel());
            this.InitModel();
            this.InitGridCommand();
            if (!Ext.Net.X.IsAjaxRequest)
            {
                this.AddScript("function getSelectionIds(){var sm=Ext.getCmp(\"" + this.ID + "\").getSelectionModel();var ids=Array();var selections=sm.getSelection();for(var i=0;i<selections.length;i++)ids.push(selections[i].data.Id);return ids.join(',');}");
            }
        }

        private string ReplaceExtraParams(string url, IEnumerable<Parameter> extraParams, ParameterCollection valueParams)
        {
            return extraParams.Aggregate<Parameter, string>(url, (current, extraParam) => current.Replace("{" + extraParam.Name + "}", valueParams[extraParam.Name]));
        }

        //private void store_ReadData(object sender, StoreReadDataEventArgs e)
        //{
        //    if (this.ReadData == null)
        //    {
        //        Ext.Net.X.Msg.Alert("错误", "OnReadData事件未注册").Show();
        //    }
        //    else
        //    {
        //        this.ReadData(sender, e);
        //    }
        //}

        public GridPanelEditorConfig EditorConfig { get; set; }

        public bool EnableAdd { get; set; }

        public bool EnableEdit { get; set; }

        public bool EnableRemove { get; set; }

        public Type ItemType { get; set; }

        public int PageSize
        {
            get
            {
                return this._pageSize;
            }
            set
            {
                this._pageSize = value;
            }
        }
    }
}

