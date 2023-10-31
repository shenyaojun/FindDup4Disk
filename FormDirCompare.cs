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

            dataGridView1.ReadOnly = true;
            dataGridView2.ReadOnly = true;
            dataGridView3.ReadOnly = true;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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
    }
}
