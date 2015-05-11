namespace Xinchen.ApplicationBase.UI
{
    using Ext.Net;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using Xinchen.ApplicationBase;
    using Xinchen.DbUtils;
    using Xinchen.DynamicObject;
    using Xinchen.ExtNetBase;
    using Xinchen.PrivilegeManagement;
    using Xinchen.PrivilegeManagement.DTO;
    using Xinchen.PrivilegeManagement.ViewModel;
    using Xinchen.Utils;
    using Xinchen.Utils.Entity;

    public abstract class PageBase : Page
    {
        protected Xinchen.ApplicationBase.Privilege privilege;

        private ResourceManager resourceManager = null;

        protected bool IsFirstLoad
        {
            get
            {
                return !X.IsAjaxRequest && !IsPostBack;
            }
        }

        protected ResourceManager ResourceManager
        {
            get { return resourceManager; }
        }

        protected IEnumerable<Node> ConvertToNodes<T>(IEnumerable<T> list, Func<T, string> getNodeId, Func<T, string> getNodeText, Func<T, bool> isLeaf = null)
        {
            var nodes = new List<Node>();
            foreach (var item in list)
            {
                Node node = new Node();
                node.NodeID = getNodeId(item);
                node.Text = getNodeText(item);
                node.Leaf = isLeaf != null ? isLeaf(item) : true;
                nodes.Add(node);
            }
            return nodes;
        }

        protected void OpenTopModalWindow(string url, string title, int width, int height)
        {
            X.AddScript("var topWin=parent.App._topWin;");
            X.AddScript("topWin.getLoader().load({url:'" + ResolveClientUrl(url) + "'});");
            X.AddScript("topWin.setWidth(" + width + ");");
            X.AddScript("topWin.setHeight(" + height + ");");
            X.AddScript("topWin.setTitle('" + title + "');");
            X.AddScript("topWin.show();");

        }

        HtmlForm _mainForm;

        protected HtmlForm MainForm
        {
            get
            {
                if (_mainForm == null)
                {
                    foreach (Control control in Controls)
                    {
                        if (_mainForm == null)
                        {
                            _mainForm = control as HtmlForm;
                            if (_mainForm != null) break;
                        }
                    }
                    if (_mainForm == null)
                    {
                        throw new Exception("请至少插入一个表单");
                    }
                }
                return _mainForm;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            foreach (Control control in Controls)
            {
                if (ResourceManager == null)
                {
                    resourceManager = control as ResourceManager;
                    if (resourceManager != null) break;
                }
            }
            if (ResourceManager == null)
            {
                resourceManager = new ResourceManager();
                Controls.Add(ResourceManager);
            }
            this.privilege = new Xinchen.ApplicationBase.Privilege(new PrivilegeBase());
            this.privilege.PrivilegeBase.Setup();
            this.privilege.PrivilegeBase.CheckLoginStatus();
            if (!this.privilege.PrivilegeBase.CheckPrivilege(this.Privilege))
            {
                base.Response.Redirect("~/privilegeerror.aspx", true);
            }
        }

        protected PageBase()
        {
        }

        /// <summary>
        /// 在浏览器端弹出提示框
        /// </summary>
        /// <param name="msg"></param>
        public void Alert(string msg)
        {
            X.Msg.Alert("提示", msg).Show();
        }

        /// <summary>
        /// 弹出消息提示，并转到指定url
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        public void Alert(string msg, string url)
        {
            X.Msg.Alert("提示", msg, new JFunction("location.href='" + HttpUtility.UrlEncode(this.Page.ResolveClientUrl(url)) + "'")).Show();
        }

        /// <summary>
        /// 弹出消息提示，关闭已打开的顶层窗口，并执行自定义操作
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="handler"></param>
        public void AlertCloseWindow(string msg, string handler = null)
        {
            X.Msg.Alert("提示", msg, "parent.Ext.WindowMgr.getActive().hide();" + handler).Show();
        }

        /// <summary>
        /// 弹出消息提示，关闭已打开的顶层窗口，刷新父窗口中的GridPanel
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="gridID"></param>
        public void AlertCloseWindowRefreshGrid(string msg, string gridID)
        {
            this.AlertCloseWindow(msg, "parent.Ext.getCmp('" + gridID + "').getStore().load();");
        }

        /// <summary>
        /// 弹出消息提示并刷新父窗口中指定的GridPanel
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="gridID"></param>
        public void AlertRefreshGrid(string msg, string gridID)
        {
            X.Msg.Alert("提示", msg, "parent.Ext.getCmp('" + gridID + "').getStore().load();").Show();
        }

        protected bool CatchException(Action action, Action<ApplicationException> onError = null)
        {
            try
            {
                action();
                return true;
            }
            catch (ApplicationException exception)
            {
                if (onError != null)
                {
                    onError(exception);
                }
                else
                {
                    this.Alert(exception.Message);
                }
            }
            return false;
        }

        public void ChangeVerifyImage(Ext.Net.Image imgVerify)
        {
            string imageUrl = imgVerify.ImageUrl;
            imgVerify.ImageUrl = base.Request.Url.LocalPath + "?action=verifyImage&" + DateTime.Now.Ticks.ToString();
        }

        public EntityResult<TModel> GetModelFromPost<TModel>() where TModel : class, new()
        {
            Type objectType = typeof(TModel);
            var propertySetters = ExpressionReflector.GetSetters(objectType);
            EntityResult<TModel> result = new EntityResult<TModel>();
            var values = ExpressionReflector.GetProperties(objectType);
            NameValueCollection form = HttpContext.Current.Request.Form;
            TModel local = Activator.CreateInstance<TModel>();
            foreach (PropertyInfo info in values.Values)
            {
                object obj2 = null;
                string str = info.Name.ToLower();
                Type underlyingType = Nullable.GetUnderlyingType(info.PropertyType);
                ValidationAttribute[] attributes = AttributeHelper.GetAttributes<ValidationAttribute>(info);
                if (form.AllKeys.Contains<string>(str, StringComparer.InvariantCultureIgnoreCase))
                {
                    string str2 = form[str];
                    try
                    {
                        if (underlyingType != null)
                        {
                            obj2 = Convert.ChangeType(str2, underlyingType);
                        }
                        else
                        {
                            obj2 = Convert.ChangeType(str2, info.PropertyType);
                        }
                    }
                    catch (FormatException)
                    {
                    }
                }
                else if (underlyingType == null)
                {
                    //不是可空类型，必须要有一个值，所以应该提示一个错误
                    var requiredAttr = attributes.FirstOrDefault(x => x is RequiredAttribute);
                    if (requiredAttr == null || string.IsNullOrWhiteSpace(requiredAttr.ErrorMessage))
                    {
                        result.Message = "表单项 " + info.Name + " 验证失败";
                    }
                    else
                    {
                        result.Message = requiredAttr.ErrorMessage;
                    }
                    return result;
                }
                foreach (ValidationAttribute attribute in attributes)
                {
                    if (!((attribute == null) || attribute.IsValid(obj2)))
                    {
                        result.Message = string.IsNullOrEmpty(attribute.ErrorMessage) ? ("表单项 " + info.Name + " 验证失败") : attribute.ErrorMessage;
                        return result;
                    }
                }
                if ((obj2 == null) && (underlyingType == null))
                {
                    try
                    {
                        obj2 = Activator.CreateInstance(info.PropertyType);
                    }
                    catch (MissingMethodException)
                    {
                        obj2 = null;
                    }
                }
                propertySetters[info.Name](local, obj2);
            }
            result.Model = local;
            result.Success = true;
            return result;
        }

        public MemoryStream GetVerifyImage(string code, int width = 60, int height = 0x16, float fontSize = 14f)
        {
            Bitmap image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.White);
            Random random = new Random();
            for (int i = 0; i < 12; i++)
            {
                int num2 = random.Next(image.Width);
                int num3 = random.Next(image.Width);
                int num4 = random.Next(image.Height);
                int num5 = random.Next(image.Height);
                graphics.DrawLine(new Pen(Color.LightGray), num2, num4, num3, num5);
            }
            Font font = new Font("Arial", fontSize, FontStyle.Italic | FontStyle.Bold);
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.Gray, 1.2f, true);
            graphics.DrawString(code, font, brush, (float)0f, (float)0f);
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Gif);
            image.Dispose();
            graphics.Dispose();
            return stream;
        }

        protected bool HasPrivilege(int privilegeId)
        {
            return this.privilege.PrivilegeBase.CheckPrivilege(privilegeId);
        }

        public void Info(string msg)
        {
            X.MessageBox.Info("提示", msg, AnchorPoint.Bottom, UI.Info).Show();
        }

        protected string Json(object o)
        {
            return JsonConvert.SerializeObject(o);
        }



        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.WriteVerifyImage();
        }

        public void ShowError(string msg)
        {
            base.Response.Redirect("~/error.aspx?message=" + HttpUtility.UrlEncode(msg), true);
        }

        /// <summary>
        /// 将Ext.Net的参数中的过滤参数转换成条件链表
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected FilterLinked ConvertToFilterLinked(StoreReadDataEventArgs e, Func<FilterCondition, object> converter = null)
        {
            return FilterConverter.ConvertToFilterLinked(e, converter);
        }

        /// <summary>
        /// 注意只会转换第一个排序规则
        /// </summary>
        /// <param name="sorters"></param>
        /// <returns></returns>
        protected Sort ConvertToSort(DataSorter[] sorters, Func<string, string> fieldMap = null)
        {
            return FilterConverter.ConvertToSorter(sorters, fieldMap);
        }

        //protected List<DynamicSqlParam> ToSqlParams(StoreReadDataEventArgs e, IDynamicParamConverter converter, bool autoGenerateSort = true)
        //{

        //    return FilterConverter.ToSqlParams(e, converter, autoGenerateSort);
        //}

        private void WriteVerifyImage()
        {
            if (!X.IsAjaxRequest && ("verifyImage".Equals(base.Request["action"], StringComparison.OrdinalIgnoreCase)))
            {
                string randomString = StringHelper.GetRandomString(4);
                this.Session["VerifyCode"] = randomString.ToLower();
                MemoryStream stream = this.GetVerifyImage(randomString, 60, 0x16, 14f);
                using (stream)
                {
                    base.Response.ContentType = "image/Gif";
                    base.Response.BinaryWrite(stream.ToArray());
                }
                base.Response.End();
            }
        }

        protected abstract int Privilege { get; }

        protected UserSessionModel userInfo
        {
            get
            {
                return this.privilege.UserInfo;
            }
        }
    }
}

