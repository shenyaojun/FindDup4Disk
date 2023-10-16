namespace WinFormsApp1
{
    partial class Form1
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
            button1 = new Button();
            listView1 = new ListView();
            listView2 = new ListView();
            button2 = new Button();
            label1 = new Label();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            label2 = new Label();
            treeView1 = new TreeView();
            dataGridView1 = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(318, 58);
            button1.Name = "button1";
            button1.Size = new Size(106, 29);
            button1.TabIndex = 0;
            button1.Text = "MD5单磁盘";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // listView1
            // 
            listView1.Location = new Point(90, 121);
            listView1.Name = "listView1";
            listView1.Size = new Size(1032, 221);
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
            // button2
            // 
            button2.Location = new Point(590, 57);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 3;
            button2.Text = "start";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(104, 630);
            label1.Name = "label1";
            label1.Size = new Size(53, 20);
            label1.TabIndex = 4;
            label1.Text = "label1";
            // 
            // button3
            // 
            button3.Location = new Point(770, 57);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 5;
            button3.Text = "停止";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(191, 58);
            button4.Name = "button4";
            button4.Size = new Size(94, 29);
            button4.TabIndex = 6;
            button4.Text = "button4";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new Point(430, 57);
            button5.Name = "button5";
            button5.Size = new Size(124, 29);
            button5.TabIndex = 7;
            button5.Text = "单磁盘重复排查";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(104, 664);
            label2.Name = "label2";
            label2.Size = new Size(53, 20);
            label2.TabIndex = 8;
            label2.Text = "label2";
            // 
            // treeView1
            // 
            treeView1.Location = new Point(1141, 121);
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
            dataGridView1.Location = new Point(90, 107);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(1032, 248);
            dataGridView1.TabIndex = 10;
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1415, 742);
            Controls.Add(dataGridView1);
            Controls.Add(treeView1);
            Controls.Add(label2);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(listView2);
            Controls.Add(listView1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private ListView listView1;
        private ListView listView2;
        private Button button2;
        private Label label1;
        private Button button3;
        private Button button4;
        private Button button5;
        private Label label2;
        private TreeView treeView1;
        private DataGridView dataGridView1;
    }
}