namespace FindDup4Disk
{
    partial class FormDup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDup));
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            dataGridView1 = new DataGridView();
            dataGridView2 = new DataGridView();
            button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(499, 65);
            button1.Name = "button1";
            button1.Size = new Size(84, 29);
            button1.TabIndex = 0;
            button1.Text = "开始";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(114, 588);
            label1.Name = "label1";
            label1.Size = new Size(48, 20);
            label1.TabIndex = 3;
            label1.Text = "Done";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(115, 618);
            label2.Name = "label2";
            label2.Size = new Size(13, 20);
            label2.TabIndex = 4;
            label2.Text = " ";
            // 
            // dataGridView1
            // 
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(79, 38);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.RowTemplate.Height = 29;
            dataGridView1.Size = new Size(919, 329);
            dataGridView1.TabIndex = 5;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // dataGridView2
            // 
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(79, 364);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.RowHeadersWidth = 51;
            dataGridView2.RowTemplate.Height = 29;
            dataGridView2.Size = new Size(919, 202);
            dataGridView2.TabIndex = 6;
            dataGridView2.CellMouseClick += dataGridView2_CellMouseClick;
            // 
            // button2
            // 
            button2.BackColor = Color.Khaki;
            button2.Font = new Font("宋体", 12F, FontStyle.Regular, GraphicsUnit.Point);
            button2.ForeColor = Color.Blue;
            button2.Location = new Point(775, 581);
            button2.Name = "button2";
            button2.Size = new Size(223, 76);
            button2.TabIndex = 7;
            button2.Text = "文件夹对比";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // FormDup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1089, 699);
            Controls.Add(button2);
            Controls.Add(dataGridView2);
            Controls.Add(dataGridView1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormDup";
            Text = "Form2";
            Load += Form2_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
        private Label label2;
        private DataGridView dataGridView1;
        private DataGridView dataGridView2;
        private Button button2;
    }
}