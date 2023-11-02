using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace FindDup4Disk
{
    public partial class FormDirCompare : Form
    {
        SQLiteConnection connection;
        bool isRunning = false;
        string machineCode;
        string compareA;
        string compareB;
        string compareAMc;
        string compareBMc;
        string compareADir;
        string compareBDir;

        public FormDirCompare(SQLiteConnection connection, string machineCode, string compareA, string compareB)
        {
            InitializeComponent();
            this.connection = connection;
            this.machineCode = machineCode;
            this.compareA = compareA;
            this.compareB = compareB;
            this.compareADir = compareA.Split("*")[1];
            this.compareAMc = compareA.Split("*")[0];
            this.compareBDir = compareB.Split("*")[1];
            this.compareBMc = compareB.Split("*")[0];
        }

        private void FormDirCompare_Load(object sender, EventArgs e)
        {

            this.Text = "文件夹比较：" + compareA + " VS " + compareB;
            this.richTextBox1.Text = compareA;
            this.richTextBox2.Text = compareB;
            this.richTextBox1.ReadOnly = true;
            this.richTextBox2.ReadOnly = true;

            //richTextBox1.BackColor = Color.Transparent;
            richTextBox1.ForeColor = Color.Black;
            richTextBox1.BorderStyle = BorderStyle.None;
            //richTextBox2.BackColor = System.Drawing.Color.Transparent;
            richTextBox2.ForeColor = Color.Black;
            richTextBox2.BorderStyle = BorderStyle.None;

            if (compareAMc.Equals(machineCode.Substring(0, 2)))
                this.button1.Text = this.button1.Text + "（本机）";

            if (compareBMc.Equals(machineCode.Substring(0, 2)))
                this.button2.Text = this.button2.Text + "（本机）";

            dataGridView1.ReadOnly = true;
            dataGridView2.ReadOnly = true;
            dataGridView3.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            RefreshGridShow();
        }

        private void RefreshGridShow()
        {
            String sqlCommand1 = "select a.Md5,a.machine||a.Filename amfile, b.machine||b.Filename bmfile, a.Length from "
                            + "(select * from files where machine = '" + compareAMc + "' and Filename  like '" + compareADir + "%') a,"
                            + "(select * from files  where machine = '" + compareBMc + "' and Filename  like '" + compareBDir + "%') b "
                            + "where a.Md5 = b.Md5 order by a.Length desc";
            ShowFileListRecords(sqlCommand1, dataGridView1);
            String sqlCommand2 = "select a.Md5,a.machine||a.Filename amfile, '' bmfile, a.Length from files a  "
                + "where machine = '" + compareAMc + "' and Filename  like  '" + compareADir + "%' and Md5 in ("
                + "select Md5 from files where machine = '" + compareAMc + "' and Filename  like '" + compareADir + "%' "
                + "EXCEPT "
                + "select Md5 from files where machine = '" + compareBMc + "' and Filename  like '" + compareBDir + "%') "
                + "order by Length desc";
            ShowFileListRecords(sqlCommand2, dataGridView2);
            String sqlCommand3 = "select a.Md5,'' amfile, a.machine||a.Filename bmfile, a.Length from files a  "
                + "where machine = '" + compareBMc + "' and Filename  like  '" + compareBDir + "%' and Md5 in ("
                + "select Md5 from files where machine = '" + compareBMc + "' and Filename  like '" + compareBDir + "%' "
                + "EXCEPT "
                + "select Md5 from files where machine = '" + compareAMc + "' and Filename  like '" + compareADir + "%') "
                + "order by Length desc";
            ShowFileListRecords(sqlCommand3, dataGridView3);
        }

        private void ShowFileListRecords(string sqlcommand, DataGridView dgv)
        {
            using (var command = new SQLiteCommand())
            {
                command.Connection = connection;
                command.CommandText = sqlcommand;

                using (var reader = command.ExecuteReader())
                {
                    var dataset = new DataSet("files");
                    var table = new DataTable("files");
                    table.Columns.Add("md5", typeof(string));
                    table.Columns.Add("amfile", typeof(string));
                    table.Columns.Add("bmfile", typeof(string));
                    table.Columns.Add("length", typeof(Int64));
                    dataset.Tables.Add(table);

                    while (reader.Read())
                    {
                        var row = table.NewRow();
                        row["md5"] = reader.GetString(0);
                        row["amfile"] = reader.GetString(1);
                        row["bmfile"] = reader.GetString(2);
                        row["length"] = reader.GetInt64(3);
                        table.Rows.Add(row);
                    }

                    dataset.AcceptChanges();

                    dgv.DataSource = dataset.Tables[0];
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;
            DeleteDir(compareAMc, compareADir, compareBMc, compareBDir);
            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;
        }

        private void DeleteDir(string deleteMc, string deleteDir, string otherMc, string otherDir)
        {
            bool deleteAll = false;

            if (rbAll.Checked)
            {
                deleteAll = true;
            }

            if (!deleteMc.Equals(machineCode.Substring(0, 2)))
            {
                MessageBox.Show("不是本机目录，无法删除！", "重要提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                return;
            }
            if (deleteDir != null)
            {
                string message = "符合规则的文件将被从硬盘直接删除，此操作不可撤回，确定吗？" + deleteDir;
                if (MessageBox.Show(message, "重要提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
                {
                    return;
                }
                string sqlSelect = "SELECT Filename FROM files where machine = '" + deleteMc + "' and Filename like '" + deleteDir + "%'";
                if (!deleteAll)
                    sqlSelect = sqlSelect + $" and Md5 in (select Md5 from files where machine = '{otherMc}' and Filename like '{otherDir}%')"; // 选择你的表中的第一条记录  

                using (SQLiteCommand commandSelect = new SQLiteCommand(sqlSelect, connection))
                {
                    using (SQLiteDataReader reader = commandSelect.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                // 读取记录，这里假设你的表有字段A和字段B  
                                string fileNameDelete = reader.GetString(0); // 根据你的字段在表中的位置更换这里的索引  

                                //文件系统删除各个文件
                                try
                                {
                                    if (File.Exists(fileNameDelete))
                                    {
                                        File.Delete(fileNameDelete);
                                        Console.WriteLine("文件已删除");
                                    }
                                    else
                                    {
                                        Console.WriteLine("文件不存在");
                                    }
                                }
                                catch (Exception ex1)
                                {
                                    MessageBox.Show("文件删除失败！请手工操作！", "重要提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    RefreshGridShow();
                                    return;
                                }
                            }
                        }
                    }
                }



                //数据库删除
                string sql = "delete FROM files where machine = '" + deleteMc + "' and Filename like '" + deleteDir + "%'";
                if (!deleteAll)
                    sql = sql + $" and Md5 in (select Md5 from files where machine = '{otherMc}' and Filename like '{otherDir}%')";
                SQLiteCommand command = new SQLiteCommand(sql, connection);
                command.ExecuteNonQuery();
                if (deleteAll)
                {
                    //删除整个目录
                    try
                    {
                        if (Directory.Exists(deleteDir))
                        {
                            Directory.Delete(deleteDir, true); // 第二个参数为true表示删除目录及其所有子目录和文件  
                            Console.WriteLine($"{deleteDir} 已被删除。");
                        }
                        else
                        {
                            Console.WriteLine($"{deleteDir} 不存在。");
                        }

                        MessageBox.Show("目录清除成功！", "重要提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("文件已经删除，但删除目录失败！请检查一下相关目录。", "重要提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
                else
                {
                    MessageBox.Show("目录清除成功！", "重要提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                RefreshGridShow();

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;
            DeleteDir(compareBMc, compareBDir, compareAMc, compareADir);
            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;
        }

    }
}
