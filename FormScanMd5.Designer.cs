﻿namespace FindDup4Disk
{
    partial class FormScanMd5
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScanMd5));
            listView2 = new ListView();
            label1 = new Label();
            label2 = new Label();
            button1 = new Button();
            progressBar1 = new ProgressBar();
            label3 = new Label();
            SuspendLayout();
            // 
            // listView2
            // 
            listView2.Location = new Point(71, 51);
            listView2.Name = "listView2";
            listView2.Size = new Size(1160, 527);
            listView2.TabIndex = 1;
            listView2.UseCompatibleStateImageBehavior = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(71, 594);
            label1.Name = "label1";
            label1.Size = new Size(0, 20);
            label1.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(71, 614);
            label2.Name = "label2";
            label2.Size = new Size(0, 20);
            label2.TabIndex = 3;
            // 
            // button1
            // 
            button1.BackColor = SystemColors.GradientActiveCaption;
            button1.Location = new Point(366, 8);
            button1.Name = "button1";
            button1.Size = new Size(479, 37);
            button1.TabIndex = 4;
            button1.Text = "暂停";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(71, 628);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(1123, 29);
            progressBar1.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(1197, 634);
            label3.Name = "label3";
            label3.Size = new Size(31, 20);
            label3.TabIndex = 6;
            label3.Text = "0%";
            // 
            // FormScanMd5
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1308, 673);
            Controls.Add(label3);
            Controls.Add(progressBar1);
            Controls.Add(button1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(listView2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormScanMd5";
            Text = "Form3";
            FormClosing += Form3_FormClosing;
            Load += Form3_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListView listView2;
        private Label label1;
        private Label label2;
        private Button button1;
        private ProgressBar progressBar1;
        private Label label3;
    }
}