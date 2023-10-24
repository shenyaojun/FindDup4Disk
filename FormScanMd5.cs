using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using WinFormsApp1;

namespace FindDup4Disk
{
    public partial class FormScanMd5 : Form
    {
        SQLiteConnection connection;
        bool userSendEndCommand = false;
        bool userScanningEnd = false;
        string machineCode;
        string disk4Scan;
        long usedSpace;
        long scannedSpace;
        int progressValue;
        // 创建一个ManualResetEvent对象  
        ManualResetEvent startSignal = new ManualResetEvent(false);

        public FormScanMd5(SQLiteConnection connection, string machineCode, string disk4Scan, long usedSpace)
        {
            InitializeComponent();
            this.connection = connection;
            this.machineCode = machineCode;
            this.disk4Scan = disk4Scan;
            this.usedSpace = usedSpace;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.Text = "文件Md5扫描中，" + disk4Scan;

            // 设置ListView的属性
            this.listView2.View = View.Details;
            //this.listView1.Dock = DockStyle.Fill;

            this.listView2.Columns.Add("文件名", 800); //一步添加
            this.listView2.Columns.Add("MD5", 220); //一步添加

            this.listView2.Columns.Add("大小", 120); //一步添加



            // 设置ProgressBar的最大值和最小值  
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;

            scannedSpace = 0;
            // 更新ProgressBar的值  
            progressBar1.Value = 0;
            progressValue = 0;

            StartMd5Scanning();
        }

        private void StartMd5Scanning()
        {
            //开启一个异步线程进行逻辑处理
            new Task(new Action(() =>
            {
                userSendEndCommand = false;
                this.label1.Text = "start.....";

                var records = new List<dynamic>();

                //读取db文件，已处理的

                records = ReadDbAsRecord("SELECT * FROM files where machine = '" + machineCode.Substring(0, 2) + "' and Filename like '" + disk4Scan + "%' ");

                records.Sort(((a, b) => a.Filename.CompareTo(b.Filename)));

                Dictionary<string, string> dict = new Dictionary<string, string> { };
                Dictionary<string, string> dupDict = new Dictionary<string, string> { };
                List<string> dicts = new List<string>();
                int i = 0;

                //String[] dicts = {};
                DirectoryInfo TheFolder = new DirectoryInfo(disk4Scan);
                dicts.Add(disk4Scan);
                foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                {
                    Console.WriteLine(NextFolder.FullName);
                    if (NextFolder.FullName.Contains(".Trashes")) continue;
                    if (NextFolder.FullName.Contains("System Volume")) continue;
                    dicts.Add(disk4Scan + NextFolder.Name);
                }

                foreach (string directory in dicts)
                {
                    try
                    {
                        //MessageBox.Show(directory, "处理", MessageBoxButtons.YesNo);
                        DirectoryInfo folder = new DirectoryInfo(directory);
                        int searchOption = (int)SearchOption.AllDirectories;
                        if (directory == disk4Scan)
                            searchOption = (int)SearchOption.TopDirectoryOnly;
                        foreach (FileInfo file in folder.GetFiles("*.*", (SearchOption)searchOption))

                        {
                            //用户点击了停止命令
                            if (userSendEndCommand)
                            {
                                SaveCsv(records);
                                this.label1.Text = "用户要求暂停";
                                Mem2Db();
                                try
                                {

                                    startSignal.WaitOne(); // 等待信号，直到第一个线程调用Set()方法  
                                }
                                catch (Exception ex)
                                {

                                }
                                //return;
                            }
                            // 处理每个文件
                            this.label1.Text = file.ToString();
                            scannedSpace = scannedSpace + file.Length;

                            // 更新ProgressBar的值  
                            int progressValueNew = (int)(scannedSpace / (double)usedSpace * 100);
                            progressValueNew = progressValueNew * 3;
                            if (progressValueNew > progressValue && progressValueNew < 70) {
                                progressValue = progressValueNew;
                            }
                            else
                            {
                                progressValueNew = progressValueNew / 2;
                                if (progressValueNew > progressValue && progressValueNew < 95)
                                {
                                    progressValue = progressValueNew;
                                }
                                else
                                {
                                    progressValueNew = progressValueNew * 2 / 3;
                                    if (progressValueNew > progressValue && progressValueNew <= 100)
                                    {
                                        progressValue = progressValueNew;
                                    }
                                }
                            }

                            progressBar1.Value = progressValue;
                            this.label3.Text = progressValue.ToString() + "%";

                            if (file.Length / 1000 / 1000 < 2) continue;
                            if (file.ToString().EndsWith(".dll")) continue;
                            if (file.ToString().EndsWith(".exe")) continue;
                            if (file.ToString().EndsWith(".jar")) continue;

                            if (records.Find(a => (a.Filename.Equals(file.ToString()) || a.Length.Equals(file.Length.ToString()))) != null)
                            {
                                //MessageBox.Show("file 已处理: " + file.ToString(), "处理", MessageBoxButtons.YesNo);
                                continue;
                            }

                            //string md5 = MD5.HashFile(file.ToString(), "md5");
                            string md5 = FormMain.getMD5ByMD5CryptoService(file.ToString());

                            if (dict.ContainsValue(md5))
                            {
                                if (!dupDict.ContainsKey(md5))
                                {
                                    dupDict.Add(md5, file.ToString());

                                }
                                else
                                {
                                    String old = dupDict.GetValueOrDefault(md5) + "";
                                    dupDict.Remove(md5);
                                    dupDict.Add(md5, file.ToString() + "; " + old);
                                }



                            }
                            dict.Add(file.ToString(), md5);

                            ListViewItem lvi = new ListViewItem(file.ToString());



                            lvi.SubItems.Add(md5);
                            lvi.SubItems.Add(file.Length / 1000 / 1000 + "M");

                            this.listView2.Items.Add(lvi);

                            dynamic record = new ExpandoObject();
                            record.Filename = file.ToString();
                            record.Md5 = md5;
                            record.Length = file.Length;
                            record.machine = machineCode.Substring(0, 2);
                            records.Add(record);

                            //save to db table:files
                            SQLiteCommand command = new SQLiteCommand(connection);
                            // 定义 SQL 查询，并指定参数占位符  
                            string sql = "INSERT INTO files (Filename, Md5, Length, machine) VALUES (@filename, @md5, @length, @machine)";
                            command.CommandText = sql;
                            // 创建 SQLiteParameter 对象并设置参数值  
                            SQLiteParameter nameParam = new SQLiteParameter("@filename", record.Filename);
                            command.Parameters.Add(nameParam);
                            SQLiteParameter md5Param = new SQLiteParameter("@md5", record.Md5);
                            command.Parameters.Add(md5Param);
                            SQLiteParameter lengthParam = new SQLiteParameter("@length", record.Length);
                            command.Parameters.Add(lengthParam);
                            SQLiteParameter mcParam = new SQLiteParameter("@machine", record.machine);
                            command.Parameters.Add(mcParam);

                            command.ExecuteNonQuery();

                            

                            i++;
                        }

                        //if (i > 5) return;

                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show("error", "提示", MessageBoxButtons.YesNo); 
                    }


                }

                SaveCsv(records);
                Mem2Db();
                userScanningEnd = true;
                this.label1.Text = "磁盘扫描已完成！";
                this.button1.Text = "扫描结束";
                progressValue = 100;
                progressBar1.Value = progressValue;
                this.label3.Text = progressValue.ToString() + "%";

            })).Start();
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
        private void SaveCsv(List<dynamic> records, string filename = "C:\\FileMd5Maps.csv")
        {
            using (var writer = new StreamWriter(filename))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }
        private void Mem2Db()
        {
            try
            {
                //读入内存，写入db
                SQLiteConnection cnnIn = new SQLiteConnection("Data Source=" + FormMain.dbfilename + ";Version=3;");


                cnnIn.Open();


                connection.BackupDatabase(cnnIn, "main", "main", -1, null, -1);

                cnnIn.Close();
            }
            catch (Exception ex)
            {
            }



        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (userScanningEnd)
            {
                this.Close();
                return;
            }

            if (!userSendEndCommand)
            {

                userSendEndCommand = true;
                this.button1.Text = "继续";
            }
            else
            {

                startSignal.Set(); // 触发信号，允许第二个线程开始执行 
                userSendEndCommand = false;
                this.button1.Text = "暂停";
            }

        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!userScanningEnd)
            {
                if (!userSendEndCommand)
                {
                    MessageBox.Show("磁盘扫描正在进行中，请先暂停！", "提示", MessageBoxButtons.OK);

                    e.Cancel = true;

                }
            }


        }
    }
}
