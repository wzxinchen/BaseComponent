namespace Xinchen.ExtNetBase
{
    using Ext.Net;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Web.UI;

    [ToolboxData("<{0}:UploadFilesField runat=server></{0}:UploadFilesField>"), DefaultProperty("Text")]
    public class UploadFilesField : FieldContainer
    {
        private string[] _files = new string[0];
        private TextField _hdn = new TextField();
        private Panel _panel;

        private void AddFiles()
        {
            for (int i = 0; i < this._files.Length; i++)
            {
                Panel panel = new Panel();
                string str = this._files[i];
                panel = new Panel
                {
                    Width = panel.Height = 100,
                    Margin = 5,
                    ID = "_imgPanel" + i.ToString()
                };
                panel.Title = "图片" + ((i + 1)).ToString();
                panel.Closable = true;
                panel.AutoDestroy = true;
                panel.Listeners.BeforeDestroy.Handler = "window.removeFile('" + str + "');";
                Image item = new Image
                {
                    ImageUrl = str,
                    Width = 100,
                    ID = "_img" + i.ToString(),
                    Height = 0x37
                };
                item.Listeners.DblClick.Handler = "window.open('" + str + "','_blank')";
                panel.Items.Add(item);
                this._panel.Add(panel);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            this._panel = new Panel();
            this._panel.ID = "_panelImages";
            this._panel.AutoScroll = true;
            this._panel.BodyPadding = 8;
            Toolbar item = new Toolbar();
            Ext.Net.Button component = new Ext.Net.Button
            {
                Text = "选择文件",
                ID = "_btnSelectFile"
            };
            item.Add(component);
            Ext.Net.Button button2 = new Ext.Net.Button
            {
                ID = "_btnRemoveFileFromAll",
                Text = "清空"
            };
            Ext.Net.Button button3 = new Ext.Net.Button
            {
                Text = "上传",
                ID = "_btnStartUpload",
                Handler = "window.startUpload();"
            };
            button2.Handler = "window.btnRemoveAll_click();";
            item.Add(button2);
            item.Add(button3);
            this._panel.TopBar.Add(item);
            this.Items.Add(this._panel);
            this._hdn.Style.Add("display", "none");
            this._hdn.Listeners.Change.Fn = "window.updateFileUrl";
            this._hdn.Value = string.Join(",", new string[] { this.Files });
            this.Items.Add(this._hdn);
            this.FieldLabel = "图片";
            this.Layout = "fit";
            this._panel.Layout = "column";
            if (!Ext.Net.X.IsAjaxRequest)
            {
                this.AddScript(Resource .uploader.Replace("{uploadUrl}", this.UploadUrl).Replace("{fieldName}", this._hdn.ID));
            }
            this.ID = null;
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Ext.Net.X.IsAjaxRequest)
            {
                this.AddFiles();
            }
        }

        public void SetValue(string p)
        {
            this.Files = p;
            this.AddScript(";window.updateFileUrl();");
        }

        public string Files
        {
            get
            {
                return string.Join(",", this._files);
            }
            set
            {
                this._hdn.Value = value;
                this._files = string.IsNullOrEmpty(value) ? new string[0] : value.Split(new char[] { ',' });
            }
        }

        public string Name
        {
            get
            {
                return this._hdn.Name;
            }
            set
            {
                this._hdn.Name = value;
                this._hdn.ID = "hdn" + value;
            }
        }

        public string UploadUrl { get; set; }
    }
}

