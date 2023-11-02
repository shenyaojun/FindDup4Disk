namespace FindDup4Disk
{
    partial class FormDirCompare
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDirCompare));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            panel1 = new Panel();
            dataGridView1 = new DataGridView();
            tabPage2 = new TabPage();
            dataGridView2 = new DataGridView();
            tabPage3 = new TabPage();
            dataGridView3 = new DataGridView();
            button1 = new Button();
            button2 = new Button();
            rbDup = new RadioButton();
            rbAll = new RadioButton();
            richTextBox1 = new RichTextBox();
            richTextBox2 = new RichTextBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(28, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1159, 642);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(panel1);
            tabPage1.Controls.Add(dataGridView1);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1151, 609);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "AB共有";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Location = new Point(439, 615);
            panel1.Name = "panel1";
            panel1.Size = new Size(224, 72);
            panel1.TabIndex = 3;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(6, 0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(1171, 609);
            dataGridView1.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(dataGridView2);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1151, 609);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "A-B";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView2
            // 
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(6, 0);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.RowHeadersWidth = 51;
            dataGridView2.RowTemplate.Height = 29;
            dataGridView2.Size = new Size(1171, 609);
            dataGridView2.TabIndex = 1;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(dataGridView3);
            tabPage3.Location = new Point(4, 29);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1151, 609);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "B-A";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridView3
            // 
            dataGridView3.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView3.Location = new Point(3, -3);
            dataGridView3.Name = "dataGridView3";
            dataGridView3.RowHeadersWidth = 51;
            dataGridView3.RowTemplate.Height = 29;
            dataGridView3.Size = new Size(1174, 609);
            dataGridView3.TabIndex = 1;
            // 
            // button1
            // 
            button1.BackColor = Color.PeachPuff;
            button1.ForeColor = Color.DeepPink;
            button1.Location = new Point(32, 673);
            button1.Name = "button1";
            button1.Size = new Size(311, 55);
            button1.TabIndex = 1;
            button1.Text = "清除A目录";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.PeachPuff;
            button2.ForeColor = Color.DeepPink;
            button2.Location = new Point(872, 673);
            button2.Name = "button2";
            button2.Size = new Size(311, 55);
            button2.TabIndex = 2;
            button2.Text = "清除B目录";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // rbDup
            // 
            rbDup.AutoSize = true;
            rbDup.Checked = true;
            rbDup.Location = new Point(526, 673);
            rbDup.Name = "rbDup";
            rbDup.Size = new Size(117, 24);
            rbDup.TabIndex = 3;
            rbDup.TabStop = true;
            rbDup.Text = "只删除重复项";
            rbDup.UseVisualStyleBackColor = true;
            // 
            // rbAll
            // 
            rbAll.AutoSize = true;
            rbAll.Location = new Point(526, 706);
            rbAll.Name = "rbAll";
            rbAll.Size = new Size(117, 24);
            rbAll.TabIndex = 4;
            rbAll.Text = "删除整个目录";
            rbAll.UseVisualStyleBackColor = true;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(34, 743);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(310, 96);
            richTextBox1.TabIndex = 5;
            richTextBox1.Text = "";
            // 
            // richTextBox2
            // 
            richTextBox2.Location = new Point(872, 743);
            richTextBox2.Name = "richTextBox2";
            richTextBox2.Size = new Size(310, 96);
            richTextBox2.TabIndex = 6;
            richTextBox2.Text = "";
            // 
            // FormDirCompare
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1203, 854);
            Controls.Add(richTextBox2);
            Controls.Add(richTextBox1);
            Controls.Add(rbAll);
            Controls.Add(rbDup);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormDirCompare";
            Text = "FormDirCompare";
            Load += FormDirCompare_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private DataGridView dataGridView3;
        private Button button1;
        private Button button2;
        private Panel panel1;
        private RadioButton rbDup;
        private RadioButton rbAll;
        private RichTextBox richTextBox1;
        private RichTextBox richTextBox2;
    }
}