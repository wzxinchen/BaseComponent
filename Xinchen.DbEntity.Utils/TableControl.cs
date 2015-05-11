using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xinchen.Utils;

namespace Xinchen.DbEntity.Utils
{
    public partial class TableControl : UserControl
    {
        private string _modelTemplate = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Templates/Model.txt");
        public string DataBaseName { get; set; }
        public string Server { get; set; }
        public TableControl()
        {
            InitializeComponent();
        }

        private void TableControl_Load(object sender, EventArgs e)
        {
            var list = DbHelper.GetTables(Server, DataBaseName);
            foreach (var VARIABLE in list)
            {
                listBox1.Items.Add(VARIABLE);
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var tables = listBox1.SelectedItems;
            if (tables.Count <= 0) return;
            if (tables.Count <= 1)
            {
                var columns = DbHelper.GetColumns(Server, DataBaseName, tables[0].ToString());
                string modelTemplate = TemplateHelper.ReplaceModelParams(_modelTemplate, txtNameSpace.Text.Trim(),
                    tables[0].ToString(), columns);
                txtResult.Text = modelTemplate;
            }
            else
            {
                folderBrowserDialog1.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    string folder = folderBrowserDialog1.SelectedPath + "/Model";
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                    Task.Factory.StartNew(() =>
                    {
                        foreach (string table in tables)
                        {
                            var columns = DbHelper.GetColumns(Server, DataBaseName, table);
                            string modelTemplate = TemplateHelper.ReplaceModelParams(_modelTemplate, txtNameSpace.Text.Trim(),
                    table, columns);
                            string file = folder + "/" + StringHelper.ToSingular(table) + ".cs";
                            File.WriteAllText(file, modelTemplate);
                            ControlHelper.Invoke(this, () =>
                            {
                                txtResult.Text = txtResult.Text + file + "生成成功" + Environment.NewLine;
                            });
                        }
                    });
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnGenerate.Enabled = listBox1.SelectedItem != null;
        }

        private void btnGenerateBySql_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = DbHelper.GetInstance().ExecuteQuery(txtSql.Text.Trim());
                var cols = ds.Tables[0].Columns;
                txtModelCode.Text = TemplateHelper.ReplaceModelParams(cols);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void txtSql_TextChanged(object sender, EventArgs e)
        {
            btnGenerateBySql.Enabled = !string.IsNullOrEmpty(txtSql.Text.Trim());
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtModelCode.Text.Trim());
        }
    }
}
