﻿using CsvHelper;
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

        public FormDup(SQLiteConnection connection, string machineCode, string disk4Scan)
        {
            InitializeComponent();
            this.connection = connection;
            this.machineCode = machineCode;
            this.disk4Scan = disk4Scan;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            this.Text = "文件查重中，" + disk4Scan;
            StartScanning();

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
            new Task(new Action(() =>
            {
                this.label1.Text = "start.....";


                var records = new List<dynamic>();

                //读取csv文件，已处理的

                //records = ReadDbAsRecord("SELECT * FROM files where Md5 in (select Md5 from files group by Md5 having count(1)> 1) order by Md5");
                //优化SQL
                records = ReadDbAsRecord("SELECT * FROM files where Md5 in (SELECT Md5 FROM files where Md5 in (select Md5 from files group by Md5 having count(1)> 1) and Filename like '"+ disk4Scan + "%' and machine = '"+ machineCode.Substring(0, 2) + "') order by Length desc");

                
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
                    MessageBox.Show( "未发现重复文件，请确保查重之前已经进行MD5磁盘扫描！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                foreach (var record in records)
                {
                    this.label1.Text = record.Filename;
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
                            this.label2.Text = record.machine.Substring(0, 2) + "." + record.Filename;
                            dupDict.Add(md5, record.machine.Substring(0, 2) + "." + record.Filename + "; " + oldMachine.Substring(0, 2) + "." + oldFilename);

                        }
                        else
                        {
                            this.label2.Text = record.machine.Substring(0, 2) + "*" + record.Filename;
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
                this.label1.Text = "end scanning.....";
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
                this.label1.Text = "end.....";


            })).Start();
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

        private void dataGridView2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip menu = new ContextMenuStrip();

                menu.Items.Add("Copy", null, CopyToClipboard);
                menu.Items.Add("Paste", null, PasteFromClipboard);

                menu.Show(dataGridView2, e.Location);
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
    }
}
