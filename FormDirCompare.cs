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
            // 初始化组件
            InitializeComponent();
            // 设置连接
            this.connection = connection;
            // 设置机器码
            this.machineCode = machineCode;
            // 设置比较A
            this.compareA = compareA;
            // 设置比较B
            this.compareB = compareB;
            // 获取比较A的目录
            this.compareADir = compareA.Split("*")[1];
            // 获取比较A的机器码
            this.compareAMc = compareA.Split("*")[0];
            // 获取比较B的目录
            this.compareBDir = compareB.Split("*")[1];
            // 获取比较B的机器码
            this.compareBMc = compareB.Split("*")[0];
        }

        private void FormDirCompare_Load(object sender, EventArgs e)
        {
            this.Text = "文件夹比较：" + compareA + " VS " + compareB;
            this.richTextBox1.Text = compareA;
            this.richTextBox2.Text = compareB;
            this.richTextBox1.ReadOnly = true;
            this.richTextBox2.ReadOnly = true;

            // 设置richTextBox1为透明色，并设置前景色为黑色，边框样式为无
            richTextBox1.ForeColor = Color.Black;
            richTextBox1.BorderStyle = BorderStyle.None;

            // 设置richTextBox2为透明色，并设置前景色为黑色，边框样式为无
            richTextBox2.ForeColor = Color.Black;
            richTextBox2.BorderStyle = BorderStyle.None;

            // 如果compareAMc与machineCode的前两个字符相等，则在按钮1的文本后添加"（本机）”
            if (compareAMc.Equals(machineCode.Substring(0, 2)))
                this.button1.Text = this.button1.Text + "（本机）";

            // 如果compareBMc与machineCode的前两个字符相等，则在按钮2的文本后添加"（本机）”
            if (compareBMc.Equals(machineCode.Substring(0, 2)))
                this.button2.Text = this.button2.Text + "（本机）";

            // 设置dataGridView1、dataGridView2和dataGridView3为只读状态
            dataGridView1.ReadOnly = true;
            dataGridView2.ReadOnly = true;
            dataGridView3.ReadOnly = true;

            // 设置dataGridView1、dataGridView2和dataGridView3的列自动调整为填充整个列
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // 刷新显示
            RefreshGridShow();
        }

        private void RefreshGridShow()
        {
            // 查询两个机器中相同MD5值的文件，按照文件长度从大到小排序
            String sqlCommand1 = "select a.Md5,a.machine||a.Filename amfile, b.machine||b.Filename bmfile, a.Length from "
                            + "(select * from files where machine = '" + compareAMc + "' and Filename  like '" + compareADir + "%') a,"
                            + "(select * from files  where machine = '" + compareBMc + "' and Filename  like '" + compareBDir + "%') b "
                            + "where a.Md5 = b.Md5 order by a.Length desc";
            ShowFileListRecords(sqlCommand1, dataGridView1);

            // 查询机器A中相同MD5值的文件与机器B中相同MD5值的文件对比，列出机器A中不存在于机器B中的文件，并按照文件长度从大到小排序

            String sqlCommand2 = "select a.Md5,a.machine||a.Filename amfile, '' bmfile, a.Length from files a  "
                + "where machine = '" + compareAMc + "' and Filename  like  '" + compareADir + "%' and Md5 in ("
                + "select Md5 from files where machine = '" + compareAMc + "' and Filename  like '" + compareADir + "%' "
                + "EXCEPT "
                + "select Md5 from files where machine = '" + compareBMc + "' and Filename  like '" + compareBDir + "%') "
                + "order by Length desc";
            ShowFileListRecords(sqlCommand2, dataGridView2);

            // 查询机器B中相同MD5值的文件与机器A中相同MD5值的文件对比，列出机器B中不存在于机器A中
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
                      // 使用SQLiteCommand创建一个新的命令对象
            using (var command = new SQLiteCommand())
            {
                // 设置命令所在的连接对象
                command.Connection = connection;
                // 设置命令文本
                command.CommandText = sqlcommand;

                // 使用ExecuteReader方法执行命令，并创建一个数据读取器
                using (var reader = command.ExecuteReader())
                {
                    // 创建一个名为"files"的数据集
                    var dataset = new DataSet("files");
                    // 创建一个名为"files"的数据表
                    var table = new DataTable("files");
                    // 向数据表中添加列：md5、amfile、bmfile、length
                    table.Columns.Add("md5", typeof(string));
                    table.Columns.Add("amfile", typeof(string));
                    table.Columns.Add("bmfile", typeof(string));
                    table.Columns.Add("length", typeof(Int64));
                    // 将数据表添加到数据集中
                    dataset.Tables.Add(table);

                    // 循环读取数据源中的每一行
                    while (reader.Read())
                    {
                        // 创建一个新的数据行
                        var row = table.NewRow();
                        // 将读取到的数据填充到数据行中
                        row["md5"] = reader.GetString(0);
                        row["amfile"] = reader.GetString(1);
                        row["bmfile"] = reader.GetString(2);
                        row["length"] = reader.GetInt64(3);
                        // 将填充好的数据行添加到数据表中
                        table.Rows.Add(row);
                    }

                    // 批准数据集中的所有更改
                    dataset.AcceptChanges();

                    // 将数据表设置为dgv的DataSource
                    dgv.DataSource = dataset.Tables[0];
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {


            // 设置当前光标为等待光标
            Cursor.Current = Cursors.WaitCursor;
            // 调用 DeleteDir 方法删除指定目录
            DeleteDir(compareAMc, compareADir, compareBMc, compareBDir);
            // 将光标设置为默认箭头光标
            Cursor.Current = Cursors.Default;

        }

        private void DeleteDir(string deleteMc, string deleteDir, string otherMc, string otherDir)
        {
            bool deleteAll = false;

            if (rbAll.Checked)
            {
                deleteAll = true;
            }

            if (deleteAll && deleteDir.Length < 4) 
            {
                MessageBox.Show("无法删除整个磁盘！", "重要提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!deleteMc.Equals(machineCode.Substring(0, 2)))
            {
                MessageBox.Show("不是本机目录，无法删除！", "重要提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
