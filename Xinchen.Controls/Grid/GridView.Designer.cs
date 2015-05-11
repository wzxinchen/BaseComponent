namespace Xinchen.Controls.Grid
{
    partial class GridView
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuCols = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripToolbar = new System.Windows.Forms.ToolStrip();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnRemove = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripPaging = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.lblTotal = new System.Windows.Forms.ToolStripLabel();
            this.lblEveryPage = new System.Windows.Forms.ToolStripLabel();
            this.txtPageSize = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.txtPageIndex = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.lblPageCount = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.btnFirst = new System.Windows.Forms.ToolStripButton();
            this.btnPrev = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.btnLast = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStripToolbar.SuspendLayout();
            this.toolStripPaging.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCols,
            this.menuFilter});
            this.contextMenuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 48);
            // 
            // menuCols
            // 
            this.menuCols.Name = "menuCols";
            this.menuCols.Size = new System.Drawing.Size(100, 22);
            this.menuCols.Text = "列";
            // 
            // menuFilter
            // 
            this.menuFilter.CheckOnClick = true;
            this.menuFilter.Name = "menuFilter";
            this.menuFilter.Size = new System.Drawing.Size(100, 22);
            this.menuFilter.Text = "过滤";
            this.menuFilter.Click += new System.EventHandler(this.menuFilter_Click);
            // 
            // toolStripToolbar
            // 
            this.toolStripToolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAdd,
            this.btnRemove,
            this.btnRefresh});
            this.toolStripToolbar.Location = new System.Drawing.Point(5, 5);
            this.toolStripToolbar.Name = "toolStripToolbar";
            this.toolStripToolbar.Size = new System.Drawing.Size(623, 25);
            this.toolStripToolbar.TabIndex = 5;
            this.toolStripToolbar.Text = "toolStrip1";
            // 
            // btnAdd
            // 
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(36, 22);
            this.btnAdd.Text = "添加";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(36, 22);
            this.btnRemove.Text = "删除";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(36, 22);
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // toolStripPaging
            // 
            this.toolStripPaging.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStripPaging.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel4,
            this.lblTotal,
            this.lblEveryPage,
            this.txtPageSize,
            this.toolStripLabel2,
            this.txtPageIndex,
            this.toolStripLabel3,
            this.toolStripLabel5,
            this.lblPageCount,
            this.toolStripLabel6,
            this.btnFirst,
            this.btnPrev,
            this.btnNext,
            this.btnLast});
            this.toolStripPaging.Location = new System.Drawing.Point(5, 368);
            this.toolStripPaging.Name = "toolStripPaging";
            this.toolStripPaging.Size = new System.Drawing.Size(623, 25);
            this.toolStripPaging.TabIndex = 7;
            this.toolStripPaging.Text = "toolStrip2";
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(44, 22);
            this.toolStripLabel4.Text = "总共：";
            // 
            // lblTotal
            // 
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(15, 22);
            this.lblTotal.Text = "0";
            // 
            // lblEveryPage
            // 
            this.lblEveryPage.Name = "lblEveryPage";
            this.lblEveryPage.Size = new System.Drawing.Size(44, 22);
            this.lblEveryPage.Text = "每页：";
            // 
            // txtPageSize
            // 
            this.txtPageSize.Name = "txtPageSize";
            this.txtPageSize.Size = new System.Drawing.Size(20, 25);
            this.txtPageSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPageSize_KeyPress);
            this.txtPageSize.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPageSize_KeyUp);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(20, 22);
            this.toolStripLabel2.Text = "第";
            // 
            // txtPageIndex
            // 
            this.txtPageIndex.Name = "txtPageIndex";
            this.txtPageIndex.Size = new System.Drawing.Size(20, 25);
            this.txtPageIndex.Text = "1";
            this.txtPageIndex.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPageIndex_KeyPress);
            this.txtPageIndex.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPageIndex_KeyUp);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(20, 22);
            this.toolStripLabel3.Text = "页";
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(20, 22);
            this.toolStripLabel5.Text = "共";
            // 
            // lblPageCount
            // 
            this.lblPageCount.Name = "lblPageCount";
            this.lblPageCount.Size = new System.Drawing.Size(15, 22);
            this.lblPageCount.Text = "0";
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(20, 22);
            this.toolStripLabel6.Text = "页";
            // 
            // btnFirst
            // 
            this.btnFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(36, 22);
            this.btnFirst.Text = "首页";
            // 
            // btnPrev
            // 
            this.btnPrev.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(48, 22);
            this.btnPrev.Text = "上一页";
            // 
            // btnNext
            // 
            this.btnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(48, 22);
            this.btnNext.Text = "下一页";
            // 
            // btnLast
            // 
            this.btnLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(36, 22);
            this.btnLast.Text = "末页";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(5, 33);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(620, 332);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // GridView
            // 
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.toolStripPaging);
            this.Controls.Add(this.toolStripToolbar);
            this.Name = "GridView";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(633, 398);
            this.Load += new System.EventHandler(this.DataGridViewEx_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStripToolbar.ResumeLayout(false);
            this.toolStripToolbar.PerformLayout();
            this.toolStripPaging.ResumeLayout(false);
            this.toolStripPaging.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuCols;
        private System.Windows.Forms.ToolStripMenuItem menuFilter;
        private System.Windows.Forms.ToolStrip toolStripToolbar;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnRemove;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStrip toolStripPaging;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripLabel lblTotal;
        private System.Windows.Forms.ToolStripLabel lblEveryPage;
        private System.Windows.Forms.ToolStripTextBox txtPageSize;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox txtPageIndex;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripLabel lblPageCount;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripButton btnFirst;
        private System.Windows.Forms.ToolStripButton btnPrev;
        private System.Windows.Forms.ToolStripButton btnNext;
        private System.Windows.Forms.ToolStripButton btnLast;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}
