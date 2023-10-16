using CsvHelper;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Management;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Text;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private bool userSendEndCommand;

        public Form1()
        {
            InitializeComponent();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //����һ���첽�߳̽����߼�����
            new Task(new Action(() =>
            {
                userSendEndCommand = false;
                this.label1.Text = "start.....";

                var records = new List<dynamic>();

                //��ȡcsv�ļ����Ѵ����

                records = ReadCsv(records);

                records.Sort(((a, b) => a.Filename.CompareTo(b.Filename)));

                Dictionary<string, string> dict = new Dictionary<string, string> { };
                Dictionary<string, string> dupDict = new Dictionary<string, string> { };
                List<string> dicts = new List<string>();
                int i = 0;

                //String[] dicts = {};
                DirectoryInfo TheFolder = new DirectoryInfo("L:\\");
                foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
                {
                    Console.WriteLine(NextFolder.FullName);
                    if (NextFolder.FullName.Contains(".Trashes")) continue;
                    if (NextFolder.FullName.Contains("System Volume")) continue;
                    dicts.Add("L:\\" + NextFolder.Name);
                }

                foreach (string directory in dicts)
                {
                    try
                    {
                        //MessageBox.Show(directory, "����", MessageBoxButtons.YesNo);
                        DirectoryInfo folder = new DirectoryInfo(directory);
                        foreach (FileInfo file in folder.GetFiles("*.*", SearchOption.AllDirectories))

                        {
                            //�û������ֹͣ����
                            if (userSendEndCommand)
                            {
                                SaveCsv(records);
                                this.label1.Text = "�û�Ҫ����ͣ";
                                return;
                            }
                            // ����ÿ���ļ�
                            this.label1.Text = file.ToString();
                            if (file.Length / 1000 / 1000 < 2) continue;
                            if (file.ToString().EndsWith(".dll")) continue;
                            if (file.ToString().EndsWith(".exe")) continue;
                            if (file.ToString().EndsWith(".jar")) continue;

                            if (records.Find(a => (a.Filename.Equals(file.ToString()) || a.Length.Equals(file.Length.ToString()))) != null)
                            {
                                //MessageBox.Show("file �Ѵ���: " + file.ToString(), "����", MessageBoxButtons.YesNo);
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

                    }
                    catch { MessageBox.Show("error", "��ʾ", MessageBoxButtons.YesNo); }


                }

                SaveCsv(records);

                foreach (KeyValuePair<string, string> item in dupDict)
                {

                    ListViewItem lvi = new ListViewItem(item.Key);



                    lvi.SubItems.Add(item.Value);

                    this.listView2.Items.Add(lvi);
                }
            })).Start();
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
                }
            }




        }

        private void button2_Click(object sender, EventArgs e)
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


                foreach (FileInfo file in folder.GetFiles("*.*", SearchOption.AllDirectories))
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
                SaveCsv(records);

                foreach (KeyValuePair<string, string> item in dupDict)
                {

                    ListViewItem lvi = new ListViewItem(item.Key);



                    lvi.SubItems.Add(item.Value);

                    this.listView2.Items.Add(lvi);
                }
                MessageBox.Show("�������", "����", MessageBoxButtons.YesNo);
            })).Start();
        }

        private static void SaveCsv(List<dynamic> records, string filename = "C:\\FileMd5Maps.csv")
        {
            using (var writer = new StreamWriter(filename))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(records);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            userSendEndCommand = true;
            // ��ȡ��ǰѡ�нڵ�  
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null || string.IsNullOrEmpty(selectedNode.Text))
            {
                MessageBox.Show("��ѡ����̣�", "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show("ѡ��Ĵ��̣���" + selectedNode.Text, "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);
            }

            var records = new List<dynamic>();

            //��ȡcsv�ļ����Ѵ����

            records = ReadCsv(records);

            records.Sort(((a, b) => a.Md5.CompareTo(b.Md5)));


            string connectionString = "Data Source=:memory:;Version=3;";
            SQLiteConnection cnnIn = new SQLiteConnection("Data Source=C:\\daba.db;Version=3;");
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




            string[] md5LocalMachine = GenerateMachineCode();
            this.label1.Text = md5LocalMachine[0];
            this.label2.Text = md5LocalMachine[1];
            this.label1.Text = md5LocalMachine[2];
            this.label2.Text = md5LocalMachine[3];

            this.label2.Text = md5LocalMachine[4];

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
            MessageBox.Show("Done��", "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);


        }

        private string[] GenerateMachineCode()
        {
            string[] strings = new string[5];

            //��ȡ����Ӳ����Ϣ
            string sCPUSerialNumber = "";
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

        private void button4_Click(object sender, EventArgs e)
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
        



        private static string getMD5ByMD5CryptoService(string path)
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

        private void button5_Click(object sender, EventArgs e)
        {
            // ��ȡ��ǰѡ�нڵ�  
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode == null || string.IsNullOrEmpty(selectedNode.Text))
            {
                MessageBox.Show("��ѡ����̣�", "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);
            }
            else
            {
                //MessageBox.Show("ѡ��Ĵ��̣���" + selectedNode.Text, "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);
            }

            //����һ���첽�߳̽����߼�����
            new Task(new Action(() =>
            {
                this.label1.Text = "start.....";


                var records = new List<dynamic>();

                //��ȡcsv�ļ����Ѵ����

                records = ReadCsv(records);

                records.Sort(((a, b) => a.Md5.CompareTo(b.Md5)));

                Dictionary<string, string> dupDict = new Dictionary<string, string> { };
                Dictionary<string, string> lengthDict = new Dictionary<string, string> { };

                String oldMd5 = "";
                String oldFilename = "";
                bool needSave = false;
                bool hasDup = false;

                foreach (var record in records)
                {
                    this.label1.Text = record.Filename;
                    //MessageBox.Show(record.Filename + ":::" , "����", MessageBoxButtons.YesNo);
                    string md5 = record.Md5;
                    try
                    {
                        lengthDict.Add(md5, record.Length);
                    }
                    catch (Exception ex)
                    {

                    }



                    if (md5 == oldMd5)
                    {
                        //�����ظ��ļ�
                        hasDup = true;
                        if (record.Filename.StartsWith(selectedNode.Text) || oldFilename.StartsWith(selectedNode.Text))
                        {
                            needSave = true;
                        }

                        if (!dupDict.ContainsKey(md5))
                        {
                            dupDict.Add(md5, record.Filename + "; " + oldFilename);

                        }
                        else
                        {
                            String old = dupDict.GetValueOrDefault(md5) + "";
                            dupDict.Remove(md5);
                            dupDict.Add(md5, record.Filename + "; " + old);
                        }

                    }
                    else
                    {
                        //������md5
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
                        needSave = false;
                        hasDup = false;

                    }

                }
                this.label1.Text = "end scanning.....";
                var recordsSave = new List<dynamic>();
                foreach (KeyValuePair<string, string> item in dupDict)
                {

                    ListViewItem lvi = new ListViewItem(item.Key);

                    string slength = lengthDict.GetValueOrDefault(item.Key);
                    lvi.SubItems.Add(slength);
                    lvi.SubItems.Add(item.Value);

                    this.listView2.Items.Add(lvi);
                    dynamic record = new ExpandoObject();
                    record.Md5 = item.Key;
                    record.Length = slength;
                    record.Filename = item.Value;
                    recordsSave.Add(record);

                }
                //this.listView2.Sort(this.listView2.Columns["��С"], ListSortDirection.Ascending);

                SaveCsv(recordsSave, "C:\\duplicatefiles" + selectedNode.Text.Substring(0, 1) + ".csv");
                this.label1.Text = "end.....";


            })).Start();
        }

        private void treeView1_Leave(object sender, EventArgs e)
        {
            // ȷ��ѡ�еĽڵ���ʧȥ����ʱ��Ȼ����ѡ��״̬  
            //treeView1.SelectedNode = treeView1.Focused;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //MessageBox.Show("ѡ��Ĵ��̣���" + e.Node.Text, "��ѡ��Ҫ�����Ĵ���", MessageBoxButtons.OK);
        }
    }
}