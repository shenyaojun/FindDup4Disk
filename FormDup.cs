using CsvHelper;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FindDup4Disk
{
    public partial class FormDup : Form
    {
        SQLiteConnection connection;
        bool isRunning = false;
        string machineCode;
        string disk4Scan;

        string[] dirCompare = new string[2];
        int dirCompareIndex = 0;

        public FormDup(SQLiteConnection connection, string machineCode, string disk4Scan)
        {
            InitializeComponent();

            //取消跨线程访问（此为不安全操作)**
            //Control.CheckForIllegalCrossThreadCalls = false;

            this.connection = connection;
            this.machineCode = machineCode;
            this.disk4Scan = disk4Scan;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            this.Text = "文件查重中，" + disk4Scan;
            dataGridView1.ReadOnly = true;
            dataGridView2.ReadOnly = true;
            button2.Visible = false;

            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;
            StartScanning();
            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                isRunning = false;
                this.button1.Text = "开始";
            }
            else
            {
                isRunning = true;
                this.button1.Text = "停止";
                StartScanning();
            }

        }

        //重写过程，利用DB SQL优化性能
        private void StartScanning()
        {
            //开启一个异步线程进行逻辑处理
            //Task.Run(() =>
            //{
            //this.label1.Text = "start.....";


            var records = new List<dynamic>();

            //读取csv文件，已处理的

            //records = ReadDbAsRecord("SELECT * FROM files where Md5 in (select Md5 from files group by Md5 having count(1)> 1) order by Md5");
            //优化SQL
            records = ReadDbAsRecord("SELECT * FROM files where Md5 in (SELECT Md5 FROM files where Md5 in (select Md5 from files group by Md5 having count(1)> 1) and Filename like '" + disk4Scan + "%' and machine = '" + machineCode.Substring(0, 2) + "') order by Length desc");


            //records.Sort(((a, b) => a.Md5.CompareTo(b.Md5)));

            Dictionary<string, string> dupDict = new Dictionary<string, string> { };
            Dictionary<string, string> lengthDict = new Dictionary<string, string> { };

            String oldMd5 = "";
            String oldFilename = "";
            String oldMachine = "";
            bool needSave = false;
            bool hasDup = false;
            if (records.Count == 0)
            {
                MessageBox.Show("未发现重复文件，请确保查重之前已经进行MD5磁盘扫描！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            foreach (var record in records)
            {
                //this.label1.Text = record.Filename;
                //MessageBox.Show(record.Filename + ":::" , "处理", MessageBoxButtons.YesNo);
                string md5 = record.Md5;
                try
                {
                    lengthDict.Add(md5, record.Length.ToString());
                }
                catch (Exception ex)
                {

                }


                if (md5 == oldMd5)
                {
                    //发现重复文件

                    hasDup = true;
                    if ((record.Filename.StartsWith(disk4Scan) && record.machine == machineCode.Substring(0, 2)) || (oldFilename.StartsWith(disk4Scan) && oldMachine == machineCode))
                    {
                        needSave = true;
                    }

                    if (!dupDict.ContainsKey(md5))
                    {
                        //this.label2.Text = record.machine.Substring(0, 2) + "." + record.Filename;
                        dupDict.Add(md5, record.machine.Substring(0, 2) + "." + record.Filename + "; " + oldMachine.Substring(0, 2) + "." + oldFilename);

                    }
                    else
                    {
                        //this.label2.Text = record.machine.Substring(0, 2) + "*" + record.Filename;
                        String old = dupDict.GetValueOrDefault(md5) + "";
                        dupDict.Remove(md5);
                        dupDict.Add(md5, record.machine.Substring(0, 2) + "." + record.Filename + "; " + old);
                    }

                }
                else
                {
                    //发现新md5
                    if (!needSave && hasDup)
                    {
                        try
                        {
                            dupDict.Remove(oldMd5);
                        }
                        catch (Exception e)
                        {

                        }

                    }
                    oldFilename = record.Filename;
                    oldMd5 = record.Md5;
                    oldMachine = record.machine;
                    needSave = false;
                    hasDup = false;

                }

            }
            //this.label1.Text = "end scanning.....";
            var recordsSave = new List<dynamic>();

            var dataset = new DataSet("dupfiles");
            var table = new DataTable("dupfiles");
            table.Columns.Add("MD5", typeof(string));
            table.Columns.Add("大小", typeof(Int64));
            table.Columns.Add("文件", typeof(string));
            dataset.Tables.Add(table);

            foreach (KeyValuePair<string, string> item in dupDict)
            {


                string slength = lengthDict.GetValueOrDefault(item.Key);

                dynamic record = new ExpandoObject();
                record.Md5 = item.Key;
                record.Length = slength;
                record.Filename = item.Value;
                recordsSave.Add(record);
                var row = table.NewRow();
                row["MD5"] = item.Key;
                row["大小"] = slength;
                row["文件"] = item.Value;
                table.Rows.Add(row);


            }

            dataset.AcceptChanges();

            dataGridView1.DataSource = dataset.Tables[0];  // Assuming the DataGridView control is named dataGridView1

            //this.listView2.Sort(this.listView2.Columns["大小"], ListSortDirection.Ascending);

            SaveCsv(recordsSave, "C:\\duplicatefiles" + disk4Scan.Substring(0, 1) + ".csv");
            //this.label1.Text = "end.....";


            //});
        }


        private void SaveCsv(List<dynamic> records, string filename = "C:\\FileMd5Maps.csv")
        {
            using (var writer = new StreamWriter(filename))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }
        private List<dynamic> ReadDbAsRecord(string query)
        {
            List<dynamic> dataList = new List<dynamic>();

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dynamic data = new ExpandoObject();
                        var dictionary = (IDictionary<string, object>)data;

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dictionary[reader.GetName(i)] = reader.GetValue(i);
                        }

                        dataList.Add(data);
                    }
                }
            }
            return dataList;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            dataGridView2.Visible = true;

            if (e.RowIndex > -1)

            {
                string md5 = dataGridView1.Rows[e.RowIndex].Cells["MD5"].Value.ToString();
                //MessageBox.Show("选择的磁盘：：" + mc, "请选择要操作的磁盘", MessageBoxButtons.OK);

                ShowFileListRecords("SELECT * FROM files where Md5 ='" + md5 + "' order by machine, Filename ");

            }
        }

        private void ShowFileListRecords(string sqlcommand)
        {
            using (var command = new SQLiteCommand())
            {
                command.Connection = connection;
                command.CommandText = sqlcommand;

                using (var reader = command.ExecuteReader())
                {
                    var dataset = new DataSet("files");
                    var table = new DataTable("files");
                    table.Columns.Add("mc", typeof(string));
                    table.Columns.Add("filename", typeof(string));
                    table.Columns.Add("md5", typeof(string));
                    table.Columns.Add("length", typeof(Int64));
                    dataset.Tables.Add(table);

                    while (reader.Read())
                    {
                        var row = table.NewRow();
                        row["mc"] = reader.GetString(4);
                        row["filename"] = reader.GetString(1);
                        row["md5"] = reader.GetString(2);
                        row["length"] = reader.GetInt64(3);
                        table.Rows.Add(row);
                    }

                    dataset.AcceptChanges();

                    dataGridView2.DataSource = dataset.Tables[0];
                }
            }
        }


        private void CopyToClipboard(object sender, EventArgs e)
        {
            // 将选定的行数据复制到剪贴板  
            if (dataGridView2.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                DataGridViewCell cell = dataGridView2.SelectedCells[0];
                Clipboard.SetText(cell.Value.ToString());
            }
        }
        private void PasteFromClipboard(object sender, EventArgs e)
        {
            // 从剪贴板中粘贴数据到选定的单元格  
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                if (dataGridView2.GetCellCount(DataGridViewElementStates.Selected) > 0)
                {
                    DataGridViewCell cell = dataGridView2.SelectedCells[0];
                    cell.Value = text;
                }
            }
        }
        private void DeleteFile(object sender, EventArgs e)
        {
            string fileName = sender.ToString();
            if (fileName != null)
            {
                fileName = fileName.Split('：')[1];
                MessageBox.Show(fileName, "message",MessageBoxButtons.OK);
            }
        }
        private void AddDirCompare(object sender, EventArgs e)
        {

            if (dirCompareIndex < 2)
            {
                dirCompare[dirCompareIndex] = sender.ToString();
            }
            else
            {
                dirCompare[dirCompareIndex % 2] = sender.ToString();
            }

            dirCompareIndex++;

            this.label1.Text = "①" + dirCompare[0];
            this.label1.ForeColor = Color.Red;

            if (dirCompareIndex > 1)
            {
                button2.Visible = true;
                this.label2.Text = "②" + dirCompare[1];
                this.label2.ForeColor = Color.Red;
            }


        }

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {

                ContextMenuStrip menu = new ContextMenuStrip();
                dataGridView2.ClearSelection();

                //dataGridView2.CurrentCell = null;

                dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;

                menu.Items.Add("Copy", null, CopyToClipboard);
                //menu.Items.Add("Paste", null, PasteFromClipboard);

                if (dataGridView2.Rows[e.RowIndex].Cells[1].Value == null || dataGridView2.Rows[e.RowIndex].Cells[0] == null)
                    return;
                string[] dirArray = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString().Split('\\');
                string mc = dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();

                if (machineCode.StartsWith(mc))
                {
                    menu.Items.Add("删除：" + dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString(), null, DeleteFile);
                }


                string dir = mc + "*";
                int len = 0;
                // 使用 foreach 循环遍历数组  
                foreach (string s in dirArray)
                {
                    len++;
                    if (dirArray.Length > len)
                    {
                        dir = dir + s + "\\";
                        menu.Items.Add("文件夹对比：" + dir, null, AddDirCompare);
                    }

                    if (len > 5) break;

                }



                menu.Show(Cursor.Position);

                // 输出列的名称  
                //MessageBox.Show("选中的列名: " + dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), "处理", MessageBoxButtons.YesNo);

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //检查确定两个文件夹都已经选择好
            if (dirCompareIndex < 1)
            {
                MessageBox.Show("请选择好两个文件夹！本功能只对两个文件夹进行比较！ ", "缺少文件夹", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dirCompare[0] == dirCompare[1])
            {
                MessageBox.Show("两个文件夹相同，无法进行比较！ ", "缺少文件夹", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string compareA = dirCompare[0].Split("：")[1];
            string compareB = dirCompare[1].Split("：")[1];

            if (compareA.StartsWith(compareB) || compareB.StartsWith(compareA))
            {
                MessageBox.Show("两个文件夹为包含关系，无法进行比较！ ", "缺少文件夹", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //MessageBox.Show(compareA, "缺少文件夹", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //MessageBox.Show(compareB, "缺少文件夹", MessageBoxButtons.OK, MessageBoxIcon.Error);
            // 创建新实例  
            FormDirCompare formDC = new FormDirCompare(connection, machineCode, compareA, compareB);

            // 显示新表单  
            //form2.Show();
            formDC.ShowDialog();
        }
    }
}
