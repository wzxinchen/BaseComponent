namespace Xinchen.ScheduledTask.Winform
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.gridView1 = new Xinchen.Controls.Grid.GridView();
            this.SuspendLayout();
            // 
            // gridView1
            // 
            this.gridView1.AddType = null;
            this.gridView1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.gridView1.DataSource = null;
            this.gridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridView1.EditType = null;
            this.gridView1.ExtraToolStripButtons = null;
            this.gridView1.Icon = null;
            this.gridView1.Location = new System.Drawing.Point(0, 0);
            this.gridView1.Name = "gridView1";
            this.gridView1.Padding = new System.Windows.Forms.Padding(5);
            this.gridView1.PageSize = 20;
            this.gridView1.Radius = 5;
            this.gridView1.Size = new System.Drawing.Size(787, 429);
            this.gridView1.TabIndex = 0;
            this.gridView1.Total = 0;
            this.gridView1.OnRefresh += new System.Action<object, Xinchen.Controls.Grid.GridViewRefreshEventArgs>(this.gridView1_OnRefresh);
            this.gridView1.Add += new System.EventHandler(this.gridView1_Add);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 429);
            this.Controls.Add(this.gridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "计划任务管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.Grid.GridView gridView1;
    }
}

