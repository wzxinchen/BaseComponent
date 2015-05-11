using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Xinchen.ApplicationBase.UI;
using Xinchen.ModelBase;

namespace Xinchen.ApplicationUI.User
{
    public class Login : PageBase
    {
        protected Ext.Net.LinkButton btnChangeImage;
        protected Ext.Net.Button btnLogin;
        protected HtmlForm form1;
        protected Ext.Net.Image imgVerify;
        protected TextField txtPassword;
        protected TextField txtUsername;
        protected TextField txtVerifyCode;

        private static object __fileDependencies;
        private static bool __initialized;
        private static MethodInfo __PageInspector_SetTraceDataMethod = __PageInspector_LoadHelper("SetTraceData");


        public Login()
        {
            AppRelativeVirtualPath = "~/user/Login.aspx";
            if (!__initialized)
            {
                string[] virtualFileDependencies = new string[] { "~/user/Login.aspx" };
                __fileDependencies = GetWrappedFileDependencies(virtualFileDependencies);
                __initialized = true;
            }
            Server.ScriptTimeout = 0x1c9c380;
        }


        private LiteralControl __BuildControl__control10()
        {
            LiteralControl control = new LiteralControl("\r\n        ");
            object[] parameters = new object[5];
            parameters[0] = control;
            parameters[2] = 0x191;
            parameters[3] = 10;
            parameters[4] = true;
            this.__PageInspector_SetTraceData(parameters);
            return control;
        }


        private Window __BuildControl__control11()
        {
            Window window = new Window();
            window.ApplyStyleSheetSkin(this);
            window.Title=("用户登录");
            window.Closable=(false);
            window.Icon = Icon.Key;
            window.Width = new Unit(320.0, UnitType.Pixel);
            window.Height = new Unit(205.0, UnitType.Pixel);
            this.__BuildControl__control12(window.Items);
            this.__BuildControl__control18(window.Buttons);
            object[] parameters = new object[5];
            parameters[0] = window;
            parameters[2] = 0x19b;
            parameters[3] = 0x709;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return window;
        }


        private void __BuildControl__control12(ItemsCollection<AbstractComponent> __ctrl)
        {
            FormPanel panel = this.__BuildControl__control13();
            __ctrl.Add(panel);
        }


        private FormPanel __BuildControl__control13()
        {
            FormPanel panel = new FormPanel();
            panel.ApplyStyleSheetSkin(this);
            panel.BodyStyle=("padding:20px;");
            panel.Layout=("TableLayout");
            this.__BuildControl__control14(panel.LayoutConfig);
            this.__BuildControl__control16(panel.Items);
            object[] parameters = new object[5];
            parameters[0] = panel;
            parameters[2] = 0x21f;
            parameters[3] = 0x4ca;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return panel;
        }


        private void __BuildControl__control14(LayoutConfigCollection __ctrl)
        {
            TableLayoutConfig config = this.__BuildControl__control15();
            __ctrl.Add(config);
        }


        private TableLayoutConfig __BuildControl__control15()
        {
            TableLayoutConfig config = new TableLayoutConfig();
            config.Columns=(2);
            return config;
        }


        private void __BuildControl__control16(ItemsCollection<AbstractComponent> __ctrl)
        {
            TextField field = this.__BuildControltxtUsername();
            __ctrl.Add(field);
            TextField field2 = this.__BuildControltxtPassword();
            __ctrl.Add(field2);
            TextField field3 = this.__BuildControltxtVerifyCode();
            __ctrl.Add(field3);
            Ext.Net.Image image = this.__BuildControlimgVerify();
            __ctrl.Add(image);
            DisplayField field4 = this.__BuildControl__control17();
            __ctrl.Add(field4);
            Ext.Net.LinkButton button = this.__BuildControlbtnChangeImage();
            __ctrl.Add(button);
        }


        private DisplayField __BuildControl__control17()
        {
            DisplayField field = new DisplayField();
            field.ApplyStyleSheetSkin(this);
            field.ColSpan = 1;
            object[] parameters = new object[5];
            parameters[0] = field;
            parameters[2] = 0x5b3;
            parameters[3] = 0x40;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return field;
        }


        private void __BuildControl__control18(ItemsCollection<ButtonBase> __ctrl)
        {
            Ext.Net.Button button = this.__BuildControlbtnLogin();
            __ctrl.Add(button);
        }


        private void __BuildControl__control19(ButtonDirectEvents __ctrl)
        {
            this.__BuildControl__control20(__ctrl.Click);
        }


        private LiteralControl __BuildControl__control2()
        {
            LiteralControl control = new LiteralControl("\r\n\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n");
            object[] parameters = new object[5];
            parameters[0] = control;
            parameters[2] = 0x70;
            parameters[3] = 0x31;
            parameters[4] = true;
            this.__PageInspector_SetTraceData(parameters);
            return control;
        }


        private void __BuildControl__control20(ComponentDirectEvent __ctrl)
        {
            this.__BuildControl__control21(__ctrl.EventMask);
            __ctrl.Event -= btnLogin_DirectClick;
            __ctrl.Event += btnLogin_DirectClick;
        }


        private void __BuildControl__control21(EventMask __ctrl)
        {
            __ctrl.ShowMask=(true);
            __ctrl.Msg=("正在登录");
        }


        private LiteralControl __BuildControl__control22()
        {
            LiteralControl control = new LiteralControl("\r\n        ");
            object[] parameters = new object[5];
            parameters[0] = control;
            parameters[2] = 0x8a4;
            parameters[3] = 10;
            parameters[4] = true;
            this.__PageInspector_SetTraceData(parameters);
            return control;
        }


        private LiteralControl __BuildControl__control23()
        {
            LiteralControl control = new LiteralControl("\r\n</body>\r\n</html>");
            object[] parameters = new object[5];
            parameters[0] = control;
            parameters[2] = 0x8b5;
            parameters[3] = 0x12;
            parameters[4] = true;
            this.__PageInspector_SetTraceData(parameters);
            return control;
        }


        private HtmlHead __BuildControl__control3()
        {
            HtmlHead head = new HtmlHead("head");
            HtmlMeta meta = this.__BuildControl__control4();
            IParserAccessor accessor = head;
            accessor.AddParsedSubObject(meta);
            HtmlTitle title = this.__BuildControl__control5();
            accessor.AddParsedSubObject(title);
            object[] parameters = new object[5];
            parameters[0] = head;
            parameters[2] = 0xa1;
            parameters[3] = 130;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return head;
        }


        private HtmlMeta __BuildControl__control4()
        {
            HtmlMeta meta = new HtmlMeta();
            ((IAttributeAccessor)meta).SetAttribute("http-equiv", "Content-Type");
            meta.Content = "text/html; charset=utf-8";
            object[] parameters = new object[5];
            parameters[0] = meta;
            parameters[2] = 0xbc;
            parameters[3] = 0x45;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return meta;
        }


        private HtmlTitle __BuildControl__control5()
        {
            HtmlTitle title = new HtmlTitle();
            LiteralControl control = this.__BuildControl__control6();
            IParserAccessor accessor = title;
            accessor.AddParsedSubObject(control);
            object[] parameters = new object[5];
            parameters[0] = title;
            parameters[2] = 0x107;
            parameters[3] = 0x13;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return title;
        }


        private LiteralControl __BuildControl__control6()
        {
            LiteralControl control = new LiteralControl("用户登录");
            object[] parameters = new object[5];
            parameters[0] = control;
            parameters[2] = 270;
            parameters[3] = 4;
            parameters[4] = true;
            this.__PageInspector_SetTraceData(parameters);
            return control;
        }


        private LiteralControl __BuildControl__control7()
        {
            LiteralControl control = new LiteralControl("\r\n<body>\r\n    ");
            object[] parameters = new object[5];
            parameters[0] = control;
            parameters[2] = 0x123;
            parameters[3] = 14;
            parameters[4] = true;
            this.__PageInspector_SetTraceData(parameters);
            return control;
        }


        private ResourceManager __BuildControl__control8()
        {
            ResourceManager manager = new ResourceManager();
            manager.ApplyStyleSheetSkin(this);
            object[] parameters = new object[5];
            parameters[0] = manager;
            parameters[2] = 0x131;
            parameters[3] = 0x3a;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return manager;
        }


        private LiteralControl __BuildControl__control9()
        {
            LiteralControl control = new LiteralControl("\r\n    ");
            object[] parameters = new object[5];
            parameters[0] = control;
            parameters[2] = 0x16b;
            parameters[3] = 6;
            parameters[4] = true;
            this.__PageInspector_SetTraceData(parameters);
            return control;
        }


        private Ext.Net.LinkButton __BuildControlbtnChangeImage()
        {
            Ext.Net.LinkButton button = new Ext.Net.LinkButton();
            btnChangeImage = button;
            button.ApplyStyleSheetSkin(this);
            button.ColSpan=(1);
            button.TextAlign=(0);
            button.ID = "btnChangeImage";
            button.Text=("看不清？点击这里");
            button.DirectClick -= btnChangeImage_DirectClick;
            button.DirectClick += btnChangeImage_DirectClick;
            object[] parameters = new object[5];
            parameters[0] = button;
            parameters[2] = 0x60d;
            parameters[3] = 0x9c;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return button;
        }


        private Ext.Net.Button __BuildControlbtnLogin()
        {
            Ext.Net.Button button = new Ext.Net.Button();
            btnLogin = button;
            button.ApplyStyleSheetSkin(this);
            button.Text=("登录");
            button.ID = "btnLogin";
            this.__BuildControl__control19(button.DirectEvents);
            object[] parameters = new object[5];
            parameters[0] = button;
            parameters[2] = 0x728;
            parameters[3] = 0x14d;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return button;
        }


        private HtmlForm __BuildControlform1()
        {
            HtmlForm form = new HtmlForm();
            form1 = form;
            form.ID = "form1";
            LiteralControl control = this.__BuildControl__control10();
            IParserAccessor accessor = form;
            accessor.AddParsedSubObject(control);
            Window window = this.__BuildControl__control11();
            accessor.AddParsedSubObject(window);
            LiteralControl control2 = this.__BuildControl__control22();
            accessor.AddParsedSubObject(control2);
            object[] parameters = new object[5];
            parameters[0] = form;
            parameters[2] = 0x171;
            parameters[3] = 0x744;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return form;
        }


        private Ext.Net.Image __BuildControlimgVerify()
        {
            Ext.Net.Image image = new Ext.Net.Image();
            imgVerify = image;
            image.ApplyStyleSheetSkin(this);
            image.ColSpan=(1);
            image.Width = new Unit(150.0, UnitType.Pixel);
            image.ID = "imgVerify";
            image.ImageUrl=("login.aspx?action=verifyImage");
            image.MaxWidth=(100);
            image.MaxHeight=(0x16);
            object[] parameters = new object[5];
            parameters[0] = image;
            parameters[2] = 0x4eb;
            parameters[3] = 0xae;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return image;
        }


        private void BuildControlTree(Login __ctrl)
        {
            this.InitializeCulture();
            LiteralControl control = this.__BuildControl__control2();
            IParserAccessor accessor = __ctrl;
            accessor.AddParsedSubObject(control);
            HtmlHead head = this.__BuildControl__control3();
            accessor.AddParsedSubObject(head);
            LiteralControl control2 = this.__BuildControl__control7();
            accessor.AddParsedSubObject(control2);
            ResourceManager manager = this.__BuildControl__control8();
            accessor.AddParsedSubObject(manager);
            LiteralControl control3 = this.__BuildControl__control9();
            accessor.AddParsedSubObject(control3);
            HtmlForm form = this.__BuildControlform1();
            accessor.AddParsedSubObject(form);
            LiteralControl control4 = this.__BuildControl__control23();
            accessor.AddParsedSubObject(control4);
        }


        private TextField __BuildControltxtPassword()
        {
            TextField field = new TextField();
            txtPassword = field;
            field.ApplyStyleSheetSkin(this);
            field.FieldLabel=("密码");
            field.Name=("Password");
            field.ID = "txtPassword";
            field.InputType= InputType.Password;
            field.ColSpan=(2);
            object[] parameters = new object[5];
            parameters[0] = field;
            parameters[2] = 0x3c7;
            parameters[3] = 0x80;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return field;
        }


        private TextField __BuildControltxtUsername()
        {
            TextField field = new TextField();
            txtUsername = field;
            field.ApplyStyleSheetSkin(this);
            field.Name=("Username");
            field.FieldLabel=("用户名");
            field.ID = "txtUsername";
            field.ColSpan=(2);
            object[] parameters = new object[5];
            parameters[0] = field;
            parameters[2] = 0x341;
            parameters[3] = 0x6c;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return field;
        }


        private TextField __BuildControltxtVerifyCode()
        {
            TextField field = new TextField();
            txtVerifyCode = field;
            field.ApplyStyleSheetSkin(this);
            field.FieldLabel=("验证码");
            field.Name=("VerifyCode");
            field.ID = "txtVerifyCode";
            field.ColSpan=(1);
            object[] parameters = new object[5];
            parameters[0] = field;
            parameters[2] = 0x461;
            parameters[3] = 0x70;
            parameters[4] = false;
            this.__PageInspector_SetTraceData(parameters);
            return field;
        }

        private static MethodInfo __PageInspector_LoadHelper(string helperName)
        {
            Type type = Type.GetType("Microsoft.VisualStudio.Web.PageInspector.Runtime.WebForms.TraceHelpers, Microsoft.VisualStudio.Web.PageInspector.Tracing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            if (type != null)
            {
                return type.GetMethod(helperName);
            }
            return null;
        }

        private void __PageInspector_SetTraceData(object[] parameters)
        {
            if (__PageInspector_SetTraceDataMethod != null)
            {
                __PageInspector_SetTraceDataMethod.Invoke(null, parameters);
            }
        }


        protected override void FrameworkInitialize()
        {
            this.BuildControlTree(this);
            AddWrappedFileDependencies(__fileDependencies);
            Request.ValidateInput();
        }


        public override int GetTypeHashCode()
        {
            return -1636476646;
        }

        //protected global_asax ApplicationInstance
        //{
        //    get
        //    {
        //        return (global_asax) this.Context.ApplicationInstance;
        //    }
        //}

        //protected DefaultProfile Profile
        //{
        //    get
        //    {
        //        return (DefaultProfile) this.Context.Profile;
        //    }
        //}
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void btnLogin_DirectClick(object sender, DirectEventArgs e)
        {
            var mr = GetModelFromPost<LoginModel>();
            if (mr.Success)
            {
                try
                {
                    privilege.PrivilegeBase.Login(mr.Model.Username, mr.Model.Password);
                    Response.Redirect("~/index.aspx", true);
                }
                catch (ApplicationException ae)
                {
                    Alert(ae.Message);
                }
            }
            else
            {
                Alert(mr.Message);
            }
            ChangeVerifyImage(imgVerify);
        }

        protected void btnChangeImage_DirectClick(object sender, DirectEventArgs e)
        {
            ChangeVerifyImage(imgVerify);
        }

        protected override int Privilege
        {
            get { return -1; }
        }
    }
}
