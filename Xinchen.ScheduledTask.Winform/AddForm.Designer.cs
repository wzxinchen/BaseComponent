namespace Xinchen.ScheduledTask.Winform
{
    partial class AddForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddForm));
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDll = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMethod = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.radInterval = new System.Windows.Forms.RadioButton();
            this.radTiming = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.btnAdd = new System.Windows.Forms.Button();
            this.numInterval = new System.Windows.Forms.NumericUpDown();
            this.comboType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "名字";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(90, 40);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(202, 21);
            this.txtName.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(308, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "程序集";
            // 
            // txtDll
            // 
            this.txtDll.Location = new System.Drawing.Point(365, 40);
            this.txtDll.Name = "txtDll";
            this.txtDll.ReadOnly = true;
            this.txtDll.Size = new System.Drawing.Size(202, 21);
            this.txtDll.TabIndex = 2;
            this.txtDll.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtDll_MouseDoubleClick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "程序集文件|*.dll;*.exe";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "类型名";
            // 
            // txtMethod
            // 
            this.txtMethod.Location = new System.Drawing.Point(90, 121);
            this.txtMethod.Name = "txtMethod";
            this.txtMethod.Size = new System.Drawing.Size(477, 21);
            this.txtMethod.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "方法名";
            // 
            // radInterval
            // 
            this.radInterval.AutoSize = true;
            this.radInterval.Checked = true;
            this.radInterval.Location = new System.Drawing.Point(106, 165);
            this.radInterval.Name = "radInterval";
            this.radInterval.Size = new System.Drawing.Size(83, 16);
            this.radInterval.TabIndex = 5;
            this.radInterval.TabStop = true;
            this.radInterval.Text = "每隔多少秒";
            this.radInterval.UseVisualStyleBackColor = true;
            // 
            // radTiming
            // 
            this.radTiming.AutoSize = true;
            this.radTiming.Location = new System.Drawing.Point(195, 166);
            this.radTiming.Name = "radTiming";
            this.radTiming.Size = new System.Drawing.Size(71, 16);
            this.radTiming.TabIndex = 6;
            this.radTiming.Text = "指定时间";
            this.radTiming.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(33, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "运行类型";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(33, 201);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "隔多少秒";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(308, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 2;
            this.label7.Text = "指定时间";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "HH:mm:ss";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(365, 196);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.ShowUpDown = true;
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 21);
            this.dateTimePicker1.TabIndex = 8;
            // 
            // btnAdd
            // 
            this.btnAdd.AutoSize = true;
            this.btnAdd.Location = new System.Drawing.Point(261, 244);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "添加并启动";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // numInterval
            // 
            this.numInterval.Location = new System.Drawing.Point(90, 195);
            this.numInterval.Name = "numInterval";
            this.numInterval.Size = new System.Drawing.Size(200, 21);
            this.numInterval.TabIndex = 7;
            // 
            // comboType
            // 
            this.comboType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(90, 83);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(477, 20);
            this.comboType.TabIndex = 9;
            // 
            // AddForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 301);
            this.Controls.Add(this.comboType);
            this.Controls.Add(this.numInterval);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.radTiming);
            this.Controls.Add(this.radInterval);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDll);
            this.Controls.Add(this.txtMethod);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "AddForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "添加任务";
            this.Load += new System.EventHandler(this.AddForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDll;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMethod;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radInterval;
        private System.Windows.Forms.RadioButton radTiming;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.NumericUpDown numInterval;
        private System.Windows.Forms.ComboBox comboType;
    }
}