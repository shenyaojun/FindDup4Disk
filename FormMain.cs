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
            string filePath = "app48.ico"; // �滻Ϊ���.ico�ļ�·��  

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
            // ��ȡ��ǰѡ�нڵ�  
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null || string.IsNullOrEmpty(selectedNode.Text) || !selectedNode.Text.EndsWith("\\"))
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.ShowNewFolderButton = true;
                dialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //Console.WriteLine("����ѡ��: " + dialog.SelectedPath);
                    //MessageBox.Show("ѡ��Ĵ��̣���" + dialog.SelectedPath, "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);


                    MessageBox.Show("����Md5ɨ����Ҫ�Ƚϳ���ʱ�䣬�����ĵȴ���", "����", MessageBoxButtons.OK);

                    //����Ŀ¼��С
                    long dirSpaceSize = 100;
                    if (dialog.SelectedPath.EndsWith("\\"))
                    {
                        //MessageBox.Show("ѡ������������", "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);
                        dirSpaceSize = driveUsedSpace[dialog.SelectedPath];
                    }
                    else
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(dialog.SelectedPath);
                        if (dirInfo.Exists)
                        {
                            dirSpaceSize = GetDirectorySize(dialog.SelectedPath);
                            //Console.WriteLine("Ŀ¼��СΪ: " + dirSpaceSize + " �ֽ�");
                            //MessageBox.Show("Ŀ¼��СΪ: " + dirSpaceSize + " �ֽ�", "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);
                        }
                    }
                    // ������ʵ��  
                    FormScanMd5 form4 = new FormScanMd5(connection, machineCode, dialog.SelectedPath, dirSpaceSize);


                    // ��ʾ�±� 
                    form4.ShowDialog();
                }
                else
                {
                    MessageBox.Show("��ѡ����̣�", "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);


                }
                return;
            }
            _selectedNode = treeView1.SelectedNode;


            MessageBox.Show("����Md5ɨ����Ҫ�Ƚϳ���ʱ�䣬�����ĵȴ���", "����", MessageBoxButtons.OK);
            // ������ʵ��  
            FormScanMd5 form3 = new FormScanMd5(connection, machineCode, selectedNode.Text, driveUsedSpace[selectedNode.Text]);

            // �����´��ڵ�����Ϊģ̬����  
            //form3.ModalResult = DialogResult.OK;

            // ��ʾ�±�  
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

            // ����ListView������
            this.listView1.View = View.Details;
            //this.listView1.Dock = DockStyle.Fill;

            this.listView1.Columns.Add("�ļ���", 220); //һ�����
            this.listView1.Columns.Add("MD5", 220); //һ�����

            this.listView1.Columns.Add("��С", 120); //һ�����



            this.listView2.View = View.Details;
            //this.listView1.Dock = DockStyle.Fill;

            this.listView2.Columns.Add("MD5", 220); //һ�����
            this.listView2.Columns.Add("��С", 120); //һ�����

            this.listView2.Columns.Add("�ļ���", 2200); //һ�����

            // ��Ӹ��ڵ�  
            TreeNode rootNode = new TreeNode("������");
            treeView1.Nodes.Add(rootNode);

            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Removable ||
                    drive.DriveType == DriveType.Fixed ||
                    drive.DriveType == DriveType.Network ||
                    drive.DriveType == DriveType.CDRom)
                {
                    //MessageBox.Show(drive.Name, "����", MessageBoxButtons.YesNo);
                    // ����ӽڵ�  
                    TreeNode childNode = new TreeNode(drive.Name);
                    rootNode.Nodes.Add(childNode);
                    long totalSize = drive.TotalSize; // �ܴ�С�����ֽ�Ϊ��λ  
                    long freeSpace = drive.TotalFreeSpace; // ʣ��ռ䣬���ֽ�Ϊ��λ  
                    long usedSpace = (totalSize - freeSpace); // ���ÿռ�
                    driveUsedSpace.Add(drive.Name, usedSpace);

                }
            }

            treeView1.ExpandAll();

            //��ȡ����������
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
                //����ļ�Ȩ��
                if (File.GetAttributes(dbfilename) == FileAttributes.ReadOnly)
                {
                    //Console.WriteLine($"�ļ� {dbfilename} ��ֻ���ġ�");
                    MessageBox.Show($"�ļ� {dbfilename} ��ֻ���ģ���ȷ���ļ��ɶ�д��Ȼ���������б������", "�޷�����ɨ������", MessageBoxButtons.OK);
                }
                //db�ļ����ڣ������ڴ�

                connection.Open();
                Db2Mem();
                CheckMachineCodeExists();

                ShowMachineList();

                ShowFileListRecords("SELECT * FROM files order by length desc");

            }
            else
            {
                if (MessageBox.Show($"δ�ҵ�{dbfilename},ȷ��Ҫ���³�ʼ���������������������й��������뽫���ļ��������������������б����� ", "��Ҫ����", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        File.Copy("daba.db", dbfilename);
                        //��ʼ��db
                        connection.Open();
                        Db2Mem();

                        StoreMachineCode2Db();
                        ShowMachineList();

                        ShowFileListRecords("SELECT * FROM files order by length desc");
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"{dbfilename}��ʼ��ʧ�ܣ������Ƿ���C�̲���Ȩ�ޣ� ", "��Ҫ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            //����һ���첽�߳̽����߼�����
            new Task(new Action(() =>
            {
                Thread.Sleep(3000); // ����3��
                string similarMachineCode = CheckSimilarMachineCode();
                if (similarMachineCode.Length > 0)
                {
                    if (MessageBox.Show($"������{similarMachineCode}�ͱ���Ӳ�������ƣ���ȷ���Ƿ�Ҫ�ͱ����ϲ��� ", "��Ҫ����", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
            string sql = "SELECT * FROM machines where MachineCode = '" + md5LocalMachine[4] + "'"; // ѡ����ı��еĵ�һ����¼  
            bool machineCodeExits = true;
            using (SQLiteCommand command = new SQLiteCommand(sql, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            // ��ȡ��¼�����������ı����ֶ�A���ֶ�B  
                            machineCode = reader.GetString(5); // ��������ֶ��ڱ��е�λ�ø������������  
                                                               //MessageBox.Show("machine code exits:" + machineCode, "����", MessageBoxButtons.YesNo);
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
            //��鵱ǰ�������Ƿ������ƻ���
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
                            // ��ȡ��¼�����������ı����ֶ�A���ֶ�B  
                            string cpuSn = reader.GetString(1); // ��������ֶ��ڱ��е�λ�ø������������  
                            string biosSn = reader.GetString(2); // ��������ֶ��ڱ��е�λ�ø������������  
                            string hdSn = reader.GetString(3); // ��������ֶ��ڱ��е�λ�ø������������  
                            string netSn = reader.GetString(4); // ��������ֶ��ڱ��е�λ�ø������������  
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
            // ָ���ļ�·��������  
            string filePath = @"C:\FindDup4Disk.TestWriteOnCDisk.txt";

            try
            {
                // ����ļ��Ƿ����  
                if (!File.Exists(filePath))
                {
                    // �������ļ���д������  
                    using (StreamWriter writer = File.CreateText(filePath))
                    {
                        writer.WriteLine("����һ�����ļ�");
                    }

                    Console.WriteLine("�ļ��Ѵ���");
                }
                else
                {
                    Console.WriteLine("�ļ��Ѵ���");
                }

                // ɾ���ļ�  
                File.Delete(filePath);
            }
            catch (Exception ex)
            {

                MessageBox.Show("��ʼ��ʧ�ܣ������Ƿ���C�̲���Ȩ�ޣ� ", "��Ҫ����", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
            //���ɱ����������¼

            string sql = "CREATE TABLE IF NOT EXISTS machines (id INTEGER PRIMARY KEY AUTOINCREMENT, CPUSN TEXT, BIOSSN TEXT,HDSN TEXT, NETSN TEXT, MachineCode TEXT)";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();




            command = new SQLiteCommand(connection);
            // ���� SQL ��ѯ����ָ������ռλ��  
            sql = "INSERT INTO machines (CPUSN , BIOSSN ,HDSN , NETSN , MachineCode) VALUES (@cpusn , @biossn ,@hdsn , @netsn , @machinecode)";
            command.CommandText = sql;

            // ���� SQLiteParameter �������ò���ֵ  
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

            // ������ʵ��  
            FormDup form2 = new FormDup(connection, machineCode, "D:\\");

            // ��ʾ�±�  
            form2.Show();

            /*
            string sql = "alter table files  ADD COLUMN machine TEXT";
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();

            sql = "update files  set machine = 'a7'";
            command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
            MessageBox.Show("�������", "����", MessageBoxButtons.YesNo);

            */

        }

        private static void Db2Mem()
        {
            if (File.Exists(dbfilename))
            {
                //db�ļ����ڣ������ڴ�
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
                //�����ڴ棬д��db
                SQLiteConnection cnnIn = new SQLiteConnection("Data Source=" + dbfilename + ";Version=3;");


                cnnIn.Open();


                connection.BackupDatabase(cnnIn, "main", "main", -1, null, -1);

                cnnIn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�ļ�{dbfilename}д��������˳������򲢼��ȷ���ļ����ڶ��Ҳ���ֻ�����ԣ����������б�����", "��Ҫ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void button3_Click_bak(object sender, EventArgs e)
        {
            userSendEndCommand = true;
            // ��ȡ��ǰѡ�нڵ�  
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null || string.IsNullOrEmpty(selectedNode.Text))
            {
                MessageBox.Show("��ѡ����̣�", "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("ѡ��Ĵ��̣���" + selectedNode.Text, "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            var records = new List<dynamic>();

            //��ȡcsv�ļ����Ѵ����

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
                // ���� SQL ��ѯ����ָ������ռλ��  
                sql = "INSERT INTO files (Filename, Md5, Length) VALUES (@filename, @md5, @length)";
                command.CommandText = sql;
                // ���� SQLiteParameter �������ò���ֵ  
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
            // ���� SQL ��ѯ����ָ������ռλ��  
            sql = "INSERT INTO machines (CPUSN , BIOSSN ,HDSN , NETSN , MachineCode) VALUES (@cpusn , @biossn ,@hdsn , @netsn , @machinecode)";
            command.CommandText = sql;

            // ���� SQLiteParameter �������ò���ֵ  
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
            MessageBox.Show("Done��", "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private string[] GenerateMachineCode()
        {
            string[] strings = new string[5];

            //��ȡ����Ӳ����Ϣ
            string sCPUSerialNumber = "";
            //����ȡ��������Ļ�����
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
                    //MessageBox.Show("sCPUSerialNumber��" + sCPUSerialNumber, "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);




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

            //���ɱ���MD5�룺
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

            //����һ���첽�߳̽����߼�����
            new Task(new Action(() =>
            {
                // ���� Stopwatch ʵ��  
                Stopwatch stopwatch = new Stopwatch();

                // ��ʼ����ʱ��  
                stopwatch.Start();

                string md5 = getMD5ByMD5CryptoService("D:\\VITS_fast_finetune\\whisper_model\\large-v2.pt");

                //string md5 = MD5.HashFile("D:\\VITS_fast_finetune\\whisper_model\\large-v2.pt", "md5");
                // ֹͣ����ʱ��  
                stopwatch.Stop();

                // ��ȡ��ʱ�䣨�Ժ���Ϊ��λ��  
                double milliseconds = stopwatch.Elapsed.TotalMilliseconds;

                this.label1.Text = md5 + ":::" + milliseconds;

                // ��ʼ����ʱ��  
                stopwatch.Start();


                //string md52 = getMD5ByMD5CryptoService("D:\\VITS_fast_finetune\\whisper_model\\large-v2.pt");
                string md52 = MD5.HashFile("D:\\VITS_fast_finetune\\whisper_model\\large-v2.pt", "md5");
                // ֹͣ����ʱ��  
                stopwatch.Stop();

                // ��ȡ��ʱ�䣨�Ժ���Ϊ��λ��  
                double milliseconds2 = stopwatch.Elapsed.TotalMilliseconds;
                milliseconds2 = milliseconds2 - milliseconds;

                this.label1.Text = md52 + ":::" + milliseconds2;

                MessageBox.Show(md5 + ":::" + md52, "����", MessageBoxButtons.YesNo);
                MessageBox.Show(milliseconds + ":::" + milliseconds2, "����", MessageBoxButtons.YesNo);
            })).Start();



        }
        private void buttestick(object sender, EventArgs e)
        {
            //����һ���첽�߳̽����߼�����
            new Task(new Action(() =>
            {
                var records = new List<dynamic>();

                //��ȡcsv�ļ����Ѵ����

                records = ReadCsv(records);

                records.Sort(((a, b) => a.Filename.CompareTo(b.Filename)));


                Dictionary<string, string> dict = new Dictionary<string, string> { };
                Dictionary<string, string> dupDict = new Dictionary<string, string> { };
                List<string> dicts = new List<string>();
                int i = 0;
                DirectoryInfo folder = new DirectoryInfo("E:\\CaoCao");


                foreach (System.IO.FileInfo file in folder.GetFiles("*.*", SearchOption.AllDirectories))
                {

                    // ����ÿ���ļ�
                    // ����ÿ���ļ�
                    //�ļ�С��2M��������
                    if (file.Length / 1000 / 1000 < 2) continue;

                    if (records.Find(a => (a.Filename.Equals(file.ToString()) || a.Length.Equals(file.Length.ToString()))) != null)
                    {
                        MessageBox.Show("file �Ѵ���: " + file.ToString(), "����", MessageBoxButtons.YesNo);
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
                MessageBox.Show("�������", "����", MessageBoxButtons.YesNo);
            })).Start();
        }



        public static string getMD5ByMD5CryptoService(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException(string.Format("<{0}>, ������", path));
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
            // ��ȡ��ǰѡ�нڵ�  
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null || string.IsNullOrEmpty(selectedNode.Text) || !selectedNode.Text.EndsWith("\\"))
            {
                MessageBox.Show("����ұ��б���ѡ��Ҫ�����Ĵ��̣�", "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                //MessageBox.Show("ѡ��Ĵ��̣���" + selectedNode.Text, "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);
            }
            //MessageBox.Show("���̲�����Ҫһ��ʱ�䣬�����ĵȴ���", "����", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            // ������ʵ��  
            FormDup form2 = new FormDup(connection, machineCode, selectedNode.Text);

            // ��ʾ�±�  
            //form2.Show();
            form2.ShowDialog();

        }

        private void treeView1_Leave(object sender, EventArgs e)
        {
            // ȷ��ѡ�еĽڵ���ʧȥ����ʱ��Ȼ����ѡ��״̬  
            //treeView1.SelectedNode = treeView1.Focused;
            // ��ʧȥ�����¼��м�¼ѡ�еĽڵ�  
            _selectedNode = treeView1.SelectedNode;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //MessageBox.Show("ѡ��Ĵ��̣���" + e.Node.Text, "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // ����Ƿ���ͬһ�е�Field1�ֶ���  
            if (e.RowIndex >= 0 && e.ColumnIndex == 0 && dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null) // ���������Ҫ�ı��������  
            {
                // ��ȡ��ǰ��Ԫ���ֵ  
                string value = "";

                try
                {
                    value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                }
                catch (Exception ex)
                {

                }

                // ���ֵ�Ƿ�Ϊ"�ض�ֵ"  
                if (value == machineCode)
                {
                    // ���õ�Ԫ����ɫΪ��ɫ  
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
                //MessageBox.Show("ѡ��Ĵ��̣���" + mc, "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);

                ShowFileListRecords("SELECT * FROM files where machine ='" + mc + "' order by length desc");

            }
        }

        private void Form1_Activated(object sender, EventArgs e)
        {

            // ��Ҫ�ָ�ѡ��״̬ʱ�������ڰ�ť����¼���  
            if (_selectedNode != null)
            {
                treeView1.SelectedNode = _selectedNode;
                treeView1.Focus();
            }
        }

        public void button2_Click(object sender, EventArgs e)
        {
            // ������ʵ��  
            FormTips form4 = new FormTips();

            // ��ʾ�±�  
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
            //�Զ���ƾ֤��Ч�ڣ�ʾ��2Сʱ��expires��λΪ�룬Ϊ�ϴ�ƾ֤����Чʱ�䣩
            PutPolicy putPolicy = new PutPolicy();
            string filePath = dbfilename;
            string key = "daba.db";

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"{dbfilename}�����ڣ�", "�ļ�������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show($"{dbfilename}�������ƶ��ļ���ȷ�����������ļ��������ƶ���", "��Ҫ����", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return;
            }


            // ���õ�ǰ���Ϊ�ȴ����
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
                MessageBox.Show("�ƶ��ϴ��ɹ���");
            }
            else
            {
                MessageBox.Show("�ƶ��ϴ�ʧ�ܣ�");
            }


            // ���������ΪĬ�ϼ�ͷ���
            Cursor.Current = Cursors.Default;

        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show($"�����ƶ������ļ������Ǳ���{dbfilename}��ȷ�ϲ�����", "��Ҫ����", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != DialogResult.OK)
            {
                return;
            }

            // ���õ�ǰ���Ϊ�ȴ����
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
                MessageBox.Show("���سɹ���");
            }
            else
            {
                MessageBox.Show("����ʧ�ܣ�");
            }

            // ���������ΪĬ�ϼ�ͷ���
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

                //����ӳ��
                webView21.CoreWebView2.SetVirtualHostNameToFolderMapping("html.sam", "./dist", CoreWebView2HostResourceAccessKind.Allow);
                //����
                webView21.CoreWebView2.Navigate("https://html.sam/index.html");
                webView21.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync("var webBrowserObj= window.chrome.webview.hostObjects.webBrowserObj;");

                //MessageBox.Show("done");
            }

            //webView21.NavigateToString(File.ReadAllText("index.html"));
        }

    }
}