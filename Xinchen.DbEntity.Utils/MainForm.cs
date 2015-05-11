using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Xinchen.DbEntity.Utils
{
    public partial class MainForm : Form
    {
        private string server;
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            server = txtDataSource.Text.Trim();
            List<string> databases = DbHelper.GetDataBases(server);
            treeView1.Nodes.Clear();
            foreach (var database in databases)
            {
                treeView1.Nodes.Add(database);
            }
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                //点击的数据库节点
                TabPage page = tabControl1.TabPages[e.Node.Text];
                if (page == null)
                {
                    page = new TabPage(e.Node.Text);
                    page.Name = e.Node.Text;
                    page.BackColor = Color.White;
                    TableControl tc = new TableControl();
                    tc.DataBaseName = e.Node.Text;
                    tc.Server = server;
                    tc.Dock = DockStyle.Fill;
                    page.Controls.Add(tc);
                    tabControl1.TabPages.Add(page);
                }
                tabControl1.SelectTab(page);
                //e.Node.Nodes.Clear();
                //List<string> tables = DbHelper.GetTables(server, e.Node.Text);
                //foreach (var table in tables)
                //{
                //    e.Node.Nodes.Add(table);
                //}
                //e.Node.Expand();
            }
            else if (e.Node.Level == 1)
            {
                //点击的表

            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
