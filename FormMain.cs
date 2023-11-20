using CsvHelper;
using FindDup4Disk;
using Microsoft.Web.WebView2.Core;
using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace WinFormsApp1
{

    public partial class FormMain : Form
    {
        private bool userSendEndCommand;
        private TreeNode _selectedNode;
        string[] md5LocalMachine;
        public static string dbfilename = "C:\\daba.db";
        string machineCode;
        const string connectionString = "Data Source=:memory:;Version=3;";
        static SQLiteConnection connection = new SQLiteConnection(connectionString);
        Dictionary<string, long> driveUsedSpace = new Dictionary<string, long> { };

        string AccessKey = "XXXX";
        string SecretKey = "xxxx";
        string Bucket = "dabasyj";
        //private byte[] formIconBytes;

        public FormMain()
        {
            InitializeComponent();
            InitializeAsync();
            /*
            string filePath = "app48.ico"; // 替换为你的.ico文件路径  

            try
            {
                formIconBytes = File.ReadAllBytes(filePath);
            }
            catch (IOException e)
            {
            }

            this.Icon = new Icon(new MemoryStream(formIconBytes));
            */
        }

        async void InitializeAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);
        }


        public void button1_Click(object sender, EventArgs e)
        {
            // 获取当前选中节点  
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null || string.IsNullOrEmpty(selectedNode.Text) || !selectedNode.Text.EndsWith("\\"))
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.ShowNewFolderButton = true;
                dialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //Console.WriteLine("您已选择: " + dialog.SelectedPath);
                    //MessageBox.Show("选择的磁盘：：" + dialog.SelectedPath, "请选择要操作的磁盘", MessageBoxButtons.OK);


                    MessageBox.Show("磁盘Md5扫描需要比较长的时间，请耐心等待！", "提醒", MessageBoxButtons.OK);

                    //计算目录大小
                    long dirSpaceSize = 100;
                    if (dialog.SelectedPath.EndsWith("\\"))
                    {
                        //MessageBox.Show("选择了整个磁盘", "请选择要操作的磁盘", MessageBoxButtons.OK);
                        dirSpaceSize = driveUsedSpace[dialog.SelectedPath];
                    }
                    else
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(dialog.SelectedPath);
                        if (dirInfo.Exists)
                        {
                            dirSpaceSize = GetDirectorySize(dialog.SelectedPath);
                            //Console.WriteLine("目录大小为: " + dirSpaceSize + " 字节");
                            //MessageBox.Show("目录大小为: " + dirSpaceSize + " 字节", "请选择要操作的磁盘", MessageBoxButtons.OK);
                        }
                    }
                    // 创建新实例  
                    FormScanMd5 form4 = new FormScanMd5(connection, machineCode, dialog.SelectedPath, dirSpaceSize);


                    // 显示新表单 
                    form4.ShowDialog();
                }
                else
                {
                    MessageBox.Show("请选择磁盘！", "请选择要操作的磁盘", MessageBoxButtons.OK);


                }
                return;
            }
            _selectedNode = treeView1.SelectedNode;


            MessageBox.Show("磁盘Md5扫描需要比较长的时间，请耐心等待！", "提醒", MessageBoxButtons.OK);
            // 创建新实例  
            FormScanMd5 form3 = new FormScanMd5(connection, machineCode, selectedNode.Text, driveUsedSpace[selectedNode.Text]);

            // 设置新窗口的属性为模态窗口  
            //form3.ModalResult = DialogResult.OK;

            // 显示新表单  
            //form3.Show();
            form3.ShowDialog();

        }


        private static List<dynamic> ReadCsv(List<dynamic> records)
        {
            try
            {
                using (var reader = new StreamReader("C:\\FileMd5Maps.csv"))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    records = csv.GetRecords<dynamic>().ToList();
                }
            }
            catch (Exception ex)
            {

            }

            return records;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // 设置ListView的属性
            this.listView1.View = View.Details;
            //this.listView1.Dock = DockStyle.Fill;

            this.listView1.Columns.Add("文件名", 220); //一步添加
            this.listView1.Columns.Add("MD5", 220); //一步添加

            this.listView1.Columns.Add("大小", 120); //一步添加



            this.listView2.View = View.Details;
            //this.listView1.Dock = DockStyle.Fill;

            this.listView2.Columns.Add("MD5", 220); //一步添加
            this.listView2.Columns.Add("大小", 120); //一步添加

            this.listView2.Columns.Add("文件名", 2200); //一步添加

            // 添加根节点  
            TreeNode rootNode = new TreeNode("本电脑");
            treeView1.Nodes.Add(rootNode);

            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Removable ||
                    drive.DriveType == DriveType.Fixed ||
                    drive.DriveType == DriveType.Network ||
                    drive.DriveType == DriveType.CDRom)
                {
                    //MessageBox.Show(drive.Name, "处理", MessageBoxButtons.YesNo);
                    // 添加子节点  
                    TreeNode childNode = new TreeNode(drive.Name);
                    rootNode.Nodes.Add(childNode);
                    long totalSize = drive.TotalSize; // 总大小，以字节为单位  
                    long freeSpace = drive.TotalFreeSpace; // 剩余空间，以字节为单位  
                    long usedSpace = (totalSize - freeSpace); // 已用空间
                    driveUsedSpace.Add(drive.Name, usedSpace);

                }
            }

            treeView1.ExpandAll();

            //读取本机机器码
            md5LocalMachine = GenerateMachineCode();
            this.label1.Text = md5LocalMachine[0];
            this.label2.Text = md5LocalMachine[1];
            this.label1.Text = md5LocalMachine[2];
            this.label2.Text = md5LocalMachine[3];

            this.label2.Text = md5LocalMachine[4];

            machineCode = md5LocalMachine[4];
            TestWriteOnCDisk();

            if (File.Exists(dbfilename))
            {
                //检查文件权限
                if (File.GetAttributes(dbfilename) == FileAttributes.ReadOnly)
                {
                    //Console.WriteLine($"文件 {dbfilename} 是只读的。");
                    MessageBox.Show($"文件 {dbfilename} 是只读的，请确保文件可读写，然后重新运行本软件。", "无法保存扫描结果！", MessageBoxButtons.OK);
                }
                //db文件存在，读入内存

                connection.Open();
                Db2Mem();
                CheckMachineCodeExists();

                ShowMachineList();

                ShowFileListRecords("SELECT * FROM files order by length desc");

            }
            else
            {
                if (MessageBox.Show($"未找到{dbfilename},确定要重新初始化吗？若在其他机器上运行过本程序，请将此文件拷贝到本机再重新运行本程序！ ", "重要提醒", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        File.Copy("daba.db", dbfilename);
                        //初始化db
                        connection.Open();
                        Db2Mem();

                        StoreMachineCode2Db();
                        ShowMachineList();

                        ShowFileListRecords("SELECT * FROM files order by length desc");
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"{dbfilename}初始化失败！请检查是否有C盘操作权限！ ", "重要提醒", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            //开启一个异步线程进行逻辑处理
            new Task(new Action(() =>
            {
                Thread.Sleep(3000); // 休眠3秒
                string similarMachineCode = CheckSimilarMachineCode();
                if (similarMachineCode.Length > 0)
                {
                    if (MessageBox.Show($"机器码{similarMachineCode}和本机硬件很相似，请确认是否要和本机合并？ ", "重要提醒", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        MergeMachineCode(similarMachineCode);
                        Mem2Db();

                        ShowMachineList();

                        ShowFileListRecords("SELECT * FROM files order by length desc");
                    }
                }
            })).Start();

        }

        private void CheckMachineCodeExists()
        {
            string sql = "SELECT * FROM machines where MachineCode = '" + md5LocalMachine[4] + "'"; // 选择你的表中的第一条记录  
            bool machineCodeExits = true;
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // 读取记录，这里假设你的表有字段A和字段B  
                            machineCode = reader.GetString(5); // 根据你的字段在表中的位置更换这里的索引  
                                                               //MessageBox.Show("machine code exits:" + machineCode, "处理", MessageBoxButtons.YesNo);
                        }
                    }
                    else
                    {
                        machineCodeExits = false;
                    }
                }
            }
            if (!machineCodeExits)
            {
                StoreMachineCode2Db();

            }
        }

        private void MergeMachineCode(string similarMachineCode)
        {
            string sqlMerge = "update files set machine = '" + machineCode.Substring(0, 2) + "' where  machine = '" + similarMachineCode.Substring(0, 2) + "'";
            SQLiteCommand command = new SQLiteCommand(sqlMerge, connection);
            command.ExecuteNonQuery();

            sqlMerge = "delete from machines where MachineCode = '" + similarMachineCode + "'";
            command = new SQLiteCommand(sqlMerge, connection);
            command.ExecuteNonQuery();


        }

        private string CheckSimilarMachineCode()
        {
            //检查当前机器码是否有相似机器
            string sqlCheckSimilar = "SELECT * FROM machines where MachineCode != '" + md5LocalMachine[4] + "'";
            string similarMachineCode = "";
            using (SQLiteCommand command = new SQLiteCommand(sqlCheckSimilar, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // 读取记录，这里假设你的表有字段A和字段B  
                            string cpuSn = reader.GetString(1); // 根据你的字段在表中的位置更换这里的索引  
                            string biosSn = reader.GetString(2); // 根据你的字段在表中的位置更换这里的索引  
                            string hdSn = reader.GetString(3); // 根据你的字段在表中的位置更换这里的索引  
                            string netSn = reader.GetString(4); // 根据你的字段在表中的位置更换这里的索引  
                            int count = 0;
                            if (cpuSn.Equals(md5LocalMachine[0]))
                                count++;
                            if (biosSn.Equals(md5LocalMachine[1]))
                                count++;
                            if (hdSn.Equals(md5LocalMachine[2]))
                                count++;
                            if (netSn.Equals(md5LocalMachine[3]))
                                count++;
                            if (count > 1)
                            {
                                similarMachineCode = reader.GetString(5);
                                break;
                            }

                        }
                    }
                    return similarMachineCode;
                }
            }
        }

        private void TestWriteOnCDisk()
        {
            // 指定文件路径和名称  
            string filePath = @"C:\FindDup4Disk.TestWriteOnCDisk.txt";

            try
            {
                // 检查文件是否存在  
                if (!File.Exists(filePath))
                {
                    // 创建新文件并写入内容  
                    using (StreamWriter writer = File.CreateText(filePath))
                    {
                        writer.WriteLine("这是一个新文件");
                    }

                    Console.WriteLine("文件已创建");
                }
                else
                {
                    Console.WriteLine("文件已存在");
                }

                // 删除文件  
                File.Delete(filePath);
            }
            catch (Exception ex)
            {

                MessageBox.Show("初始化失败！请检查是否有C盘操作权限！ ", "重要提醒", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        private void ShowMachineList()
        {
            using (var command = new SQLiteCommand())
            {
                command.Connection = connection;
                command.CommandText = "SELECT * FROM machines";

                using (var reader = command.ExecuteReader())
                {
                    var dataset = new DataSet("machines");
                    var table = new DataTable("machines");
                    table.Columns.Add("machinecode", typeof(string));
                    table.Columns.Add("cpusn", typeof(string));
                    table.Columns.Add("biossn", typeof(string));
                    table.Columns.Add("hdsn", typeof(string));
                    table.Columns.Add("netsn", typeof(string));
                    dataset.Tables.Add(table);

                    while (reader.Read())
                    {
                        var row = table.NewRow();
                        row["machinecode"] = reader.GetString(5);
                        row["cpusn"] = reader.GetString(1);
                        row["biossn"] = reader.GetString(2);
                        row["hdsn"] = reader.GetString(3);
                        row["netsn"] = reader.GetString(4);
                        table.Rows.Add(row);
                    }

                    dataset.AcceptChanges();

                    dataGridView1.DataSource = dataset.Tables[0];  // Assuming the DataGridView control is named dataGridView1  
                }
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

                    dataGridView2.DataSource = dataset.Tables[0];  // Assuming the DataGridView control is named dataGridView1  
                }
            }
        }

        private void StoreMachineCode2Db()
        {
            //生成本机机器码记录

            string sql = "CREATE TABLE IF NOT EXISTS machines (id INTEGER PRIMARY KEY AUTOINCREMENT, CPUSN TEXT, BIOSSN TEXT,HDSN TEXT, NETSN TEXT, MachineCode TEXT)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();




            command = new SQLiteCommand(connection);
            // 定义 SQL 查询，并指定参数占位符  
            sql = "INSERT INTO machines (CPUSN , BIOSSN ,HDSN , NETSN , MachineCode) VALUES (@cpusn , @biossn ,@hdsn , @netsn , @machinecode)";
            command.CommandText = sql;

            // 创建 SQLiteParameter 对象并设置参数值  
            SQLiteParameter snParam1 = new SQLiteParameter("@cpusn", md5LocalMachine[0]);
            command.Parameters.Add(snParam1);
            SQLiteParameter snParam2 = new SQLiteParameter("@biossn", md5LocalMachine[1]);
            command.Parameters.Add(snParam2);
            SQLiteParameter snParam3 = new SQLiteParameter("@hdsn", md5LocalMachine[2]);
            command.Parameters.Add(snParam3);
            SQLiteParameter snParam4 = new SQLiteParameter("@netsn", md5LocalMachine[3]);
            command.Parameters.Add(snParam4);
            SQLiteParameter snParam5 = new SQLiteParameter("@machinecode", md5LocalMachine[4]);
            command.Parameters.Add(snParam5);

            command.ExecuteNonQuery();
            return;
        }


        public void button3_Click(object sender, EventArgs e)
        {
            userSendEndCommand = true;
            dataGridView2.Visible = false;
            //listView1.Visible = false;
            listView2.Visible = false;

            // 创建新实例  
            FormDup form2 = new FormDup(connection, machineCode, "D:\\");

            // 显示新表单  
            form2.Show();

            /*
            string sql = "alter table files  ADD COLUMN machine TEXT";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();

            sql = "update files  set machine = 'a7'";
            command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
            MessageBox.Show("处理完成", "处理", MessageBoxButtons.YesNo);

            */

        }

        private static void Db2Mem()
        {
            if (File.Exists(dbfilename))
            {
                //db文件存在，读入内存
                SQLiteConnection cnnIn = new SQLiteConnection("Data Source=" + dbfilename + ";Version=3;");

                cnnIn.Open();


                cnnIn.BackupDatabase(connection, "main", "main", -1, null, -1);
                cnnIn.Close();



            }
        }

        private static void Mem2Db()
        {
            try
            {
                //读入内存，写入db
                SQLiteConnection cnnIn = new SQLiteConnection("Data Source=" + dbfilename + ";Version=3;");


                cnnIn.Open();


                connection.BackupDatabase(cnnIn, "main", "main", -1, null, -1);

                cnnIn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"文件{dbfilename}写入错误！请退出本程序并检查确保文件存在而且不是只读属性，再重新运行本程序！", "重要错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void button3_Click_bak(object sender, EventArgs e)
        {
            userSendEndCommand = true;
            // 获取当前选中节点  
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null || string.IsNullOrEmpty(selectedNode.Text))
            {
                MessageBox.Show("请选择磁盘！", "请选择要操作的磁盘", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("选择的磁盘：：" + selectedNode.Text, "请选择要操作的磁盘", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            var records = new List<dynamic>();

            //读取csv文件，已处理的

            records = ReadCsv(records);

            records.Sort(((a, b) => a.Md5.CompareTo(b.Md5)));


            string connectionString = "Data Source=:memory:;Version=3;";
            SQLiteConnection cnnIn = new SQLiteConnection("Data Source=" + dbfilename + ";Version=3;");
            SQLiteConnection connection = new SQLiteConnection(connectionString);

            connection.Open();
            cnnIn.Open();


            string sql = "CREATE TABLE IF NOT EXISTS files (id INTEGER PRIMARY KEY AUTOINCREMENT, Filename TEXT, Md5 TEXT, Length BIGINT)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();

            sql = "DELETE FROM files";
            command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();

            sql = "DELETE FROM sqlite_sequence WHERE name = 'files'";
            command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();

            foreach (var record in records)
            {
                command = new SQLiteCommand(connection);
                // 定义 SQL 查询，并指定参数占位符  
                sql = "INSERT INTO files (Filename, Md5, Length) VALUES (@filename, @md5, @length)";
                command.CommandText = sql;
                // 创建 SQLiteParameter 对象并设置参数值  
                SQLiteParameter nameParam = new SQLiteParameter("@filename", record.Filename);
                command.Parameters.Add(nameParam);
                SQLiteParameter md5Param = new SQLiteParameter("@md5", record.Md5);
                command.Parameters.Add(md5Param);
                SQLiteParameter lengthParam = new SQLiteParameter("@length", record.Length);
                command.Parameters.Add(lengthParam);

                command.ExecuteNonQuery();
            }

            sql = "CREATE TABLE IF NOT EXISTS machines (id INTEGER PRIMARY KEY AUTOINCREMENT, CPUSN TEXT, BIOSSN TEXT,HDSN TEXT, NETSN TEXT, MachineCode TEXT)";
            command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();






            command = new SQLiteCommand(connection);
            // 定义 SQL 查询，并指定参数占位符  
            sql = "INSERT INTO machines (CPUSN , BIOSSN ,HDSN , NETSN , MachineCode) VALUES (@cpusn , @biossn ,@hdsn , @netsn , @machinecode)";
            command.CommandText = sql;

            // 创建 SQLiteParameter 对象并设置参数值  
            SQLiteParameter snParam1 = new SQLiteParameter("@cpusn", md5LocalMachine[0]);
            command.Parameters.Add(snParam1);
            SQLiteParameter snParam2 = new SQLiteParameter("@biossn", md5LocalMachine[1]);
            command.Parameters.Add(snParam2);
            SQLiteParameter snParam3 = new SQLiteParameter("@hdsn", md5LocalMachine[2]);
            command.Parameters.Add(snParam3);
            SQLiteParameter snParam4 = new SQLiteParameter("@netsn", md5LocalMachine[3]);
            command.Parameters.Add(snParam4);
            SQLiteParameter snParam5 = new SQLiteParameter("@machinecode", md5LocalMachine[4]);
            command.Parameters.Add(snParam5);

            command.ExecuteNonQuery();




            connection.BackupDatabase(cnnIn, "main", "main", -1, null, -1);
            cnnIn.Close();

            connection.Close();
            MessageBox.Show("Done！", "请选择要操作的磁盘", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private string[] GenerateMachineCode()
        {
            string[] strings = new string[5];

            //读取本机硬件信息
            string sCPUSerialNumber = "";
            //优先取更有意义的机器名
            sCPUSerialNumber = Environment.MachineName;
            if (sCPUSerialNumber == "")
            {
                try
                {
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("Select * From Win32_Processor");

                    foreach (ManagementObject mo in searcher.Get())
                    {
                        sCPUSerialNumber = mo["ProcessorId"].ToString().Trim();
                        break;
                    }
                    //MessageBox.Show("sCPUSerialNumber！" + sCPUSerialNumber, "请选择要操作的磁盘", MessageBoxButtons.OK);




                }
                catch (Exception ex)
                {
                }
            }




            string sBIOSSerialNumber = "";
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("Select * From Win32_BIOS");

                foreach (ManagementObject mo in searcher.Get())
                {
                    sBIOSSerialNumber = mo.GetPropertyValue("SerialNumber").ToString().Trim();
                    break;
                }


            }
            catch (Exception ex)
            {
            }

            string sHardDiskSerialNumber = "";
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");

                foreach (ManagementObject mo in searcher.Get())
                {
                    sHardDiskSerialNumber = mo["SerialNumber"].ToString().Trim(); ;
                    break;
                }




            }
            catch (Exception ex)
            {
            }

            string sNetCardMACAddress = "";
            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE ((MACAddress Is Not NULL) AND (Manufacturer <> 'Microsoft'))");

                foreach (ManagementObject mo in searcher.Get())
                {
                    sNetCardMACAddress = mo["MACAddress"].ToString().Trim(); ;
                    break;
                }




            }
            catch (Exception ex)
            {
            }

            //生成本机MD5码：
            string md5LocalMachine = MD5.ComputeMD5Hash((sCPUSerialNumber + sBIOSSerialNumber + sHardDiskSerialNumber + sNetCardMACAddress).Replace('.', '-').Replace(':', '_'));

            strings[0] = sCPUSerialNumber;
            strings[1] = sBIOSSerialNumber;
            strings[2] = sHardDiskSerialNumber;
            strings[3] = sNetCardMACAddress;
            strings[4] = md5LocalMachine;

            return strings;
        }

        private void MD5TIMETEST(object sender, EventArgs e)
        {

            this.label1.Text = "start:::";

            //开启一个异步线程进行逻辑处理
            new Task(new Action(() =>
            {
                // 创建 Stopwatch 实例  
                Stopwatch stopwatch = new Stopwatch();

                // 开始测量时间  
                stopwatch.Start();

                string md5 = getMD5ByMD5CryptoService("D:\\VITS_fast_finetune\\whisper_model\\large-v2.pt");

                //string md5 = MD5.HashFile("D:\\VITS_fast_finetune\\whisper_model\\large-v2.pt", "md5");
                // 停止测量时间  
                stopwatch.Stop();

                // 获取总时间（以毫秒为单位）  
                double milliseconds = stopwatch.Elapsed.TotalMilliseconds;

                this.label1.Text = md5 + ":::" + milliseconds;

                // 开始测量时间  
                stopwatch.Start();


                //string md52 = getMD5ByMD5CryptoService("D:\\VITS_fast_finetune\\whisper_model\\large-v2.pt");
                string md52 = MD5.HashFile("D:\\VITS_fast_finetune\\whisper_model\\large-v2.pt", "md5");
                // 停止测量时间  
                stopwatch.Stop();

                // 获取总时间（以毫秒为单位）  
                double milliseconds2 = stopwatch.Elapsed.TotalMilliseconds;
                milliseconds2 = milliseconds2 - milliseconds;

                this.label1.Text = md52 + ":::" + milliseconds2;

                MessageBox.Show(md5 + ":::" + md52, "处理", MessageBoxButtons.YesNo);
                MessageBox.Show(milliseconds + ":::" + milliseconds2, "处理", MessageBoxButtons.YesNo);
            })).Start();



        }
        private void buttestick(object sender, EventArgs e)
        {
            //开启一个异步线程进行逻辑处理
            new Task(new Action(() =>
            {
                var records = new List<dynamic>();

                //读取csv文件，已处理的

                records = ReadCsv(records);

                records.Sort(((a, b) => a.Filename.CompareTo(b.Filename)));


                Dictionary<string, string> dict = new Dictionary<string, string> { };
                Dictionary<string, string> dupDict = new Dictionary<string, string> { };
                List<string> dicts = new List<string>();
                int i = 0;
                DirectoryInfo folder = new DirectoryInfo("E:\\CaoCao");


                foreach (System.IO.FileInfo file in folder.GetFiles("*.*", SearchOption.AllDirectories))
                {

                    // 处理每个文件
                    // 处理每个文件
                    //文件小于2M，不处理
                    if (file.Length / 1000 / 1000 < 2) continue;

                    if (records.Find(a => (a.Filename.Equals(file.ToString()) || a.Length.Equals(file.Length.ToString()))) != null)
                    {
                        MessageBox.Show("file 已处理: " + file.ToString(), "处理", MessageBoxButtons.YesNo);
                        continue;
                    }


                    //string md5 = MD5.HashFile(file.ToString(), "md5");
                    string md5 = getMD5ByMD5CryptoService(file.ToString());

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

                    this.listView1.Items.Add(lvi);


                    dynamic record = new ExpandoObject();
                    record.Filename = file.ToString();
                    record.Md5 = md5;
                    record.Length = file.Length;
                    records.Add(record);


                    i++;
                }

                //if (i > 5) return;
                //SaveCsv(records);

                foreach (KeyValuePair<string, string> item in dupDict)
                {

                    ListViewItem lvi = new ListViewItem(item.Key);



                    lvi.SubItems.Add(item.Value);

                    this.listView2.Items.Add(lvi);
                }
                MessageBox.Show("处理完成", "处理", MessageBoxButtons.YesNo);
            })).Start();
        }



        public static string getMD5ByMD5CryptoService(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException(string.Format("<{0}>, 不存在", path));
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();
            byte[] buffer = md5Provider.ComputeHash(fs);
            string resule = BitConverter.ToString(buffer);
            resule = resule.Replace("-", "");
            md5Provider.Clear();
            fs.Close();
            return resule;
        }

        public void button5_Click(object sender, EventArgs e)
        {
            // 获取当前选中节点  
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null || string.IsNullOrEmpty(selectedNode.Text) || !selectedNode.Text.EndsWith("\\"))
            {
                MessageBox.Show("请从右边列表中选择要操作的磁盘！", "请选择要操作的磁盘", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                //MessageBox.Show("选择的磁盘：：" + selectedNode.Text, "请选择要操作的磁盘", MessageBoxButtons.OK);
            }
            //MessageBox.Show("磁盘查重需要一定时间，请耐心等待！", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            // 创建新实例  
            FormDup form2 = new FormDup(connection, machineCode, selectedNode.Text);

            // 显示新表单  
            //form2.Show();
            form2.ShowDialog();

        }

        private void treeView1_Leave(object sender, EventArgs e)
        {
            // 确保选中的节点在失去焦点时仍然保持选中状态  
            //treeView1.SelectedNode = treeView1.Focused;
            // 在失去焦点事件中记录选中的节点  
            _selectedNode = treeView1.SelectedNode;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //MessageBox.Show("选择的磁盘：：" + e.Node.Text, "请选择要操作的磁盘", MessageBoxButtons.OK);
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 检查是否在同一行的Field1字段中  
            if (e.RowIndex >= 0 && e.ColumnIndex == 0 && dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null) // 根据你的需要改变这个索引  
            {
                // 获取当前单元格的值  
                string value = "";

                try
                {
                    value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                }
                catch (Exception ex)
                {

                }

                // 检查值是否为"特定值"  
                if (value == machineCode)
                {
                    // 设置单元格颜色为红色  
                    e.CellStyle.BackColor = Color.Red;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Mem2Db();
            connection.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            dataGridView2.Visible = true;

            if (e.RowIndex > -1)

            {
                string mc = dataGridView1.Rows[e.RowIndex].Cells["machinecode"].Value.ToString().Substring(0, 2);
                //MessageBox.Show("选择的磁盘：：" + mc, "请选择要操作的磁盘", MessageBoxButtons.OK);

                ShowFileListRecords("SELECT * FROM files where machine ='" + mc + "' order by length desc");

            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {

            // 需要恢复选中状态时，例如在按钮点击事件中  
            if (_selectedNode != null)
            {
                treeView1.SelectedNode = _selectedNode;
                treeView1.Focus();
            }
        }

        public void button2_Click(object sender, EventArgs e)
        {
            // 创建新实例  
            FormTips form4 = new FormTips();

            // 显示新表单  
            form4.Show();
            //form4.ShowDialog();
        }

        public long GetDirectorySize(string path)
        {
            long size = 0;
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (System.IO.FileInfo file in dir.GetFiles())
            {
                size += file.Length;
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                size += GetDirectorySize(subDir.FullName);
            }

            return size;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {

            Mac mac = new Mac(AccessKey, SecretKey);
            //自定义凭证有效期（示例2小时，expires单位为秒，为上传凭证的有效时间）
            PutPolicy putPolicy = new PutPolicy();
            string filePath = dbfilename;
            string key = "daba.db";

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"{dbfilename}不存在！", "文件不存在", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show($"{dbfilename}将覆盖云端文件，确认这是最新文件并覆盖云端吗？", "重要提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return;
            }


            // 设置当前光标为等待光标
            Cursor.Current = Cursors.WaitCursor;


            putPolicy.Scope = Bucket + ":" + key;
            putPolicy.SetExpires(3600);

            putPolicy.DeleteAfterDays = 10;
            string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            Config config = new Config();
            config.Zone = Zone.ZONE_CN_North;
            config.UseHttps = true;
            config.UseCdnDomains = true;
            config.ChunkSize = ChunkUnit.U512K;
            FormUploader target = new FormUploader(config);
            HttpResult result = target.UploadFile(filePath, key, token, null);
            //MessageBox.Show("form upload result: " + result.ToString());
            if (result.Code == (int)HttpCode.OK)
            {
                MessageBox.Show("云端上传成功！");
            }
            else
            {
                MessageBox.Show("云端上传失败！");
            }


            // 将光标设置为默认箭头光标
            Cursor.Current = Cursors.Default;

        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show($"将从云端下载文件并覆盖本地{dbfilename}，确认操作吗？", "重要提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return;
            }

            // 设置当前光标为等待光标
            Cursor.Current = Cursors.WaitCursor;


            Mac mac = new Mac(AccessKey, SecretKey);
            string domain = "http://s3ladakyt.hb-bkt.clouddn.com";
            string key = "daba.db";
            string privateUrl = DownloadManager.CreatePrivateUrl(mac, domain, key, 3600);
            HttpResult result = DownloadManager.Download(privateUrl, dbfilename);
            //MessageBox.Show("form download result: " + result.ToString());
            if (result.Code == (int)HttpCode.OK)
            {
                Db2Mem();
                CheckMachineCodeExists();

                ShowMachineList();

                ShowFileListRecords("SELECT * FROM files order by length desc");
                MessageBox.Show("下载成功！");
            }
            else
            {
                MessageBox.Show("下载失败！");
            }

            // 将光标设置为默认箭头光标
            Cursor.Current = Cursors.Default;
        }

        private void webView21_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if ((webView21 == null) || (webView21.CoreWebView2 == null))
                Console.WriteLine("not ready");
            //webView21.NavigateToString(File.ReadAllText("index.html"));
            else
            {
                webView21.CoreWebView2.AddHostObjectToScript("webBrowserObj", new ScriptCallbackObject());

                //虚拟映射
                webView21.CoreWebView2.SetVirtualHostNameToFolderMapping("html.sam", "./dist", CoreWebView2HostResourceAccessKind.Allow);
                //导航
                webView21.CoreWebView2.Navigate("https://html.sam/index.html");
                webView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("var webBrowserObj= window.chrome.webview.hostObjects.webBrowserObj;");

                //MessageBox.Show("done");
            }

            //webView21.NavigateToString(File.ReadAllText("index.html"));
        }

    }
}