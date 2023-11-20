namespace WinFormsApp1
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            button1 = new Button();
            listView1 = new ListView();
            listView2 = new ListView();
            label1 = new Label();
            button5 = new Button();
            label2 = new Label();
            treeView1 = new TreeView();
            dataGridView1 = new DataGridView();
            dataGridView2 = new DataGridView();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = SystemColors.GradientActiveCaption;
            button1.Location = new Point(217, 50);
            button1.Name = "button1";
            button1.Size = new Size(245, 43);
            button1.TabIndex = 0;
            button1.Text = "MD5单磁盘扫描";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // listView1
            // 
            listView1.Location = new Point(90, 280);
            listView1.Name = "listView1";
            listView1.Size = new Size(1032, 302);
            listView1.TabIndex = 1;
            listView1.UseCompatibleStateImageBehavior = false;
            // 
            // listView2
            // 
            listView2.Location = new Point(90, 361);
            listView2.Name = "listView2";
            listView2.Size = new Size(1032, 221);
            listView2.TabIndex = 2;
            listView2.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(104, 718);
            label1.Name = "label1";
            label1.Size = new Size(53, 20);
            label1.TabIndex = 4;
            label1.Text = "label1";
            // 
            // button5
            // 
            button5.BackColor = SystemColors.GradientActiveCaption;
            button5.Location = new Point(482, 50);
            button5.Name = "button5";
            button5.Size = new Size(309, 44);
            button5.TabIndex = 7;
            button5.Text = "单磁盘重复排查";
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(104, 752);
            label2.Name = "label2";
            label2.Size = new Size(53, 20);
            label2.TabIndex = 8;
            label2.Text = "label2";
            // 
            // treeView1
            // 
            treeView1.Location = new Point(1141, 209);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(238, 461);
            treeView1.TabIndex = 9;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
            treeView1.Leave += treeView1_Leave;
            // 
            // dataGridView1
            // 
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(90, 163);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(1032, 176);
            dataGridView1.TabIndex = 10;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            // 
            // dataGridView2
            // 
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(90, 335);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.RowHeadersWidth = 51;
            dataGridView2.RowTemplate.Height = 29;
            dataGridView2.Size = new Size(1032, 349);
            dataGridView2.TabIndex = 11;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.GradientActiveCaption;
            button2.ForeColor = Color.MediumVioletRed;
            button2.Location = new Point(778, 70);
            button2.Name = "button2";
            button2.Size = new Size(326, 44);
            button2.TabIndex = 12;
            button2.Text = "去扫描另一台新机器";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.ForeColor = Color.MediumVioletRed;
            button3.Location = new Point(1153, 57);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 13;
            button3.Text = "上传云端";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click_1;
            // 
            // button4
            // 
            button4.ForeColor = Color.MediumVioletRed;
            button4.Location = new Point(1262, 58);
            button4.Name = "button4";
            button4.Size = new Size(94, 29);
            button4.TabIndex = 14;
            button4.Text = "下载云端";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // webView21
            // 
            webView21.AllowExternalDrop = true;
            webView21.BackColor = SystemColors.ControlLight;
            webView21.CreationProperties = null;
            webView21.DefaultBackgroundColor = Color.Transparent;
            webView21.Location = new Point(90, 13);
            webView21.Name = "webView21";
            webView21.Size = new Size(1032, 150);
            webView21.TabIndex = 15;
            webView21.ZoomFactor = 1D;
            webView21.CoreWebView2InitializationCompleted += webView21_CoreWebView2InitializationCompleted;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnablePreventFocusChange;
            BackColor = SystemColors.ControlLight;
            ClientSize = new Size(1415, 795);
            Controls.Add(webView21);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(dataGridView1);
            Controls.Add(treeView1);
            Controls.Add(label2);
            Controls.Add(button5);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(dataGridView2);
            Controls.Add(listView2);
            Controls.Add(listView1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormMain";
            Text = "文件查重";
            Activated += Form1_Activated;
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)webView21).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private ListView listView1;
        private ListView listView2;
        private Label label1;
        private Button button5;
        private Label label2;
        private TreeView treeView1;
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private Button button2;
        private Button button3;
        private Button button4;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
    }
}