using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using MathWorks.MATLAB.NET.Arrays;
using nc2xls;
using Microsoft.Office.Interop.Excel;//Excel
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace Data_Visual
{
    public delegate void ChangeDateSec(bool topmost);
    public partial class DataProcess : Form
    {
        public DataProcess()
        {
            InitializeComponent();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是负号，退格且不能转为小数，则屏蔽输入
            if (!(e.KeyChar == '-' || e.KeyChar == '\b' || float.TryParse(((System.Windows.Forms.TextBox)sender).Text + e.KeyChar.ToString(), out float f)))
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '-' && ((System.Windows.Forms.TextBox)sender).Text.Length > 1)
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是负号，退格且不能转为小数，则屏蔽输入
            if (!(e.KeyChar == '-' || e.KeyChar == '\b' || float.TryParse(((System.Windows.Forms.TextBox)sender).Text + e.KeyChar.ToString(), out float f)))
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '-' && ((System.Windows.Forms.TextBox)sender).Text.Length > 1)
            {
                e.Handled = true;
            }
        }

        MongoClient client = new MongoClient("mongodb://localhost");
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "" || textBox3.Text.Trim() == "")
            {
                MessageBox.Show("请完整输入数据");
            }
            else if (Convert.ToInt32(textBox1.Text) > 180 || Convert.ToInt32(textBox1.Text) < -180 || Convert.ToInt32(textBox2.Text) > 90 || Convert.ToInt32(textBox2.Text) < -90)
            {
                MessageBox.Show("输入的经纬度有误");
            }
            else if (Convert.ToDouble(textBox3.Text) < 272.15 || Convert.ToDouble(textBox3.Text) > 322.15)
            {
                MessageBox.Show("输入的温度疑似有误，请检查");
            }
            else
            {
                var database = client.GetDatabase("SST_res"); //数据库名称
                string ctname = dateTimePicker1.Text;
                var collection = database.GetCollection<SST>(ctname);
                var filterBuilder = Builders<SST>.Filter;
                var filter = filterBuilder.Eq("Lon", Convert.ToDouble(textBox1.Text) + 0.025) & filterBuilder.Eq("Lat", Convert.ToDouble(textBox2.Text) + 0.025);
                var list = collection.Find<SST>(filter).ToList();
                if (list.Count == 0)
                {
                    MessageBox.Show("未查询到该条记录，将进行插入");
                    SST s = new SST();
                    s.Lon = Convert.ToDouble(textBox1.Text) + 0.025;
                    s.Lat = Convert.ToDouble(textBox2.Text) + 0.025;
                    s.Band = Convert.ToDouble(textBox3.Text);
                    collection.InsertOne(s);
                    MessageBox.Show("插入成功");
                }
                else
                {
                    MessageBox.Show("查询到已有记录，将进行修改");
                    var update = Builders<SST>.Update.Set("Band", Convert.ToDouble(textBox3.Text));
                    var result = collection.UpdateOne(filter, update);
                    MessageBox.Show("修改成功");
                }
            }

        }

        public class SST
        {
            public ObjectId _id { get; set; }
            public double Lon { get; set; }
            public double Lat { get; set; }
            public double Band { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "")
            {
                MessageBox.Show("请完整输入数据");
            }
            else if (Convert.ToInt32(textBox1.Text) > 180 || Convert.ToInt32(textBox1.Text) < -180 || Convert.ToInt32(textBox2.Text) > 90 || Convert.ToInt32(textBox2.Text) < -90)
            {
                MessageBox.Show("输入的经纬度有误");
            }
            else
            {
                var database = client.GetDatabase("SST_res"); //数据库名称
                string ctname = dateTimePicker1.Text;
                var collection = database.GetCollection<SST>(ctname);
                var filterBuilder = Builders<SST>.Filter;
                var filter = filterBuilder.Eq("Lon", Convert.ToDouble(textBox1.Text) + 0.025) & filterBuilder.Eq("Lat", Convert.ToDouble(textBox2.Text) + 0.025);
                var list = collection.Find<SST>(filter).ToList();
                if (list.Count == 0)
                {
                    MessageBox.Show("未查询到该条记录，无法删除");
                }
                else
                {
                    if (MessageBox.Show("将删除Lon=" + textBox1.Text + " Lat=" + textBox2.Text + "处的数据，请确认", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        var result = collection.DeleteOne(filter);
                        MessageBox.Show("删除成功");
                    }
                }
            }
        }

        System.Data.DataTable dt = new System.Data.DataTable();
        string dt2ctname;
        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "EXCEL FILE|*.xlsx;*.xls";
            string strpath;
            string filename;
            if (file.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    strpath = file.FileName;
                    filename = strpath.Substring(strpath.LastIndexOf("\\") + 1);//去掉了路径
                    dt2ctname = filename.Substring(0, filename.LastIndexOf("."));//去掉后缀名
                    object[,] data = GetExcelRangeData(strpath, "A2", 0);
                    for (int i = 0; i < data.GetLength(1); i++)
                        dt.Columns.Add(i.ToString(), typeof(object));

                    for (int i = 0; i < data.GetLength(0); i++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < data.GetLength(1); j++)
                        {
                            dr[j.ToString()] = data[i + 1, j + 1];
                        }
                        dt.Rows.Add(dr);
                    }
                    //MessageBox.Show(data[1, 2].ToString());
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns[0].HeaderCell.Value = "Lon";
                    dataGridView1.Columns[1].HeaderCell.Value = "Lat";
                    dataGridView1.Columns[2].HeaderCell.Value = "SST(K)";
                    label6.Text = data.GetLength(0).ToString();
                    label8.Text = filename;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public static object[,] GetExcelRangeData(string excelPath, string stCell, int option)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook workBook = null;
            object oMissiong = Missing.Value;
            try
            {
                workBook = app.Workbooks.Open(excelPath, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong,
                    oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);
                if (workBook == null)
                    return null;


                Worksheet workSheet = (Worksheet)workBook.Worksheets.Item[1];
                int rowCount = workSheet.UsedRange.Cells.Rows.Count;
                string edCell;
                if (option == 0)
                    edCell = "C" + rowCount.ToString();
                else
                    edCell = "M" + rowCount.ToString();
                //使用下述语句可以从头读取到最后，按需使用
                //var maxN = workSheet.Range[startCell].End[XlDirection.xlDown].Row;
                return workSheet.Range[stCell + ":" + edCell].Value2;
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
                //COM组件方式调用完记得释放资源
                if (workBook != null)
                {
                    workBook.Close(false, oMissiong, oMissiong);
                    Marshal.ReleaseComObject(workBook);
                    app.Workbooks.Close();
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            var database = client.GetDatabase("SST_res"); //数据库名称
            var collection = database.GetCollection<BsonDocument>(dt2ctname);
            var count = 0;
            var batch = new List<BsonDocument>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    count++;
                    var dictionary = dr.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col.ColumnName]);
                    batch.Add(new BsonDocument(dictionary));

                    //分批次写入，防止内存溢出
                    if (batch.Count == 5000)
                    {
                        //UpdateUI(() => { lblStatus.Text = "已导入 " + count; });
                        collection.InsertManyAsync(batch.AsEnumerable());
                        batch.Clear();
                        ClearMemory();
                    }
                }
                if (batch.Count > 0)
                {
                    //UpdateUI(() => { lblStatus.Text = "已导入 " + count; });
                    collection.InsertManyAsync(batch.AsEnumerable());
                    batch.Clear();
                    ClearMemory();
                }
                MessageBox.Show("导入成功");
            }
            else
            {
                MessageBox.Show("DataTable为空，请先导入数据");
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);

        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        List<string> strpath = new List<string>();//time列表
        private void button5_Click(object sender, EventArgs e)
        {
            strpath.Clear();
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true;
            file.Filter = "NetCDF 文件(*.nc)|*.nc";

            if (file.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string this_file in file.FileNames)
                    {
                        //选择了shape文件后
                        string pathName = System.IO.Path.GetDirectoryName(this_file);
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(this_file);
                        strpath.Add(pathName + '\\' + fileName + ".nc");
                        //Console.WriteLine(pathName + '\\' + fileName+".nc");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            string foldPath = "";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foldPath = dialog.SelectedPath + @"\";
            }
            Console.WriteLine(foldPath);
            Nc2xlsClass cvt = new Nc2xlsClass();
            MWCellArray file_ist = new MWCellArray(strpath.ToArray().Length);
            for (int i = 0; i < strpath.ToArray().Length; i++)
                file_ist[i + 1] = strpath[i];

            string first_file;
            MWArray temp = cvt.nc2xls(file_ist, foldPath);
            MWCharArray arr = (MWCharArray)temp;
            first_file = arr.ToString();

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
            psi.Arguments = "/e,/select," + foldPath + first_file;
            System.Diagnostics.Process.Start(psi);
            label9.Visible = false;
        }

        private void button6_KeyDown(object sender, KeyEventArgs e)
        {
            label9.Visible = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Owner.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string path = System.Windows.Forms.Application.StartupPath + @"\nino_cr\crawler.exe";
            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(path);
            info.WorkingDirectory = Path.GetDirectoryName(path); System.Diagnostics.Process.Start(info);
            //System.Diagnostics.Process.Start(System.Windows.Forms.Application.StartupPath + @"\nino_cr\crawler.exe");
            button8.Enabled = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string strpath;
            string filename;
            string[] file = new string[] { "Nino1+2.xls", "Nino3.xls", "Nino4.xls" };
            //三种nino数据

            try
            {
                for (int idx = 0; idx < 3; idx++)
                {
                    dt.Clear();
                    dt.Columns.Clear();
                    strpath = System.Windows.Forms.Application.StartupPath + "\\nino_cr\\" + file[idx];
                    filename = strpath.Substring(strpath.LastIndexOf("\\") + 1);//去掉了路径
                    dt2ctname = filename.Substring(0, filename.LastIndexOf("."));//去掉后缀名
                    object[,] data = GetExcelRangeData(strpath, "A1", 1);
                    for (int i = 0; i < data.GetLength(1); i++)
                        dt.Columns.Add(i.ToString(), typeof(object));

                    for (int i = 0; i < data.GetLength(0); i++)
                    {
                        DataRow dr = dt.NewRow();
                        for (int j = 0; j < data.GetLength(1); j++)
                        {
                            dr[j.ToString()] = data[i + 1, j + 1];
                        }
                        dt.Rows.Add(dr);
                    }
                    //dataGridView2.DataSource = dt;

                    var database = client.GetDatabase("SST_res"); //数据库名称
                    var collection = database.GetCollection<BsonDocument>(dt2ctname);
                    database.DropCollection(dt2ctname);
                    var count = 0;
                    var batch = new List<BsonDocument>();
                    if (dt != null)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            count++;
                            var dictionary = dr.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col.ColumnName]);
                            batch.Add(new BsonDocument(dictionary));

                            //分批次写入，防止内存溢出
                            if (batch.Count == 5000)
                            {
                                //UpdateUI(() => { lblStatus.Text = "已导入 " + count; });
                                collection.InsertManyAsync(batch.AsEnumerable());
                                batch.Clear();
                                ClearMemory();
                            }
                        }
                        if (batch.Count > 0)
                        {
                            //UpdateUI(() => { lblStatus.Text = "已导入 " + count; });
                            collection.InsertManyAsync(batch.AsEnumerable());
                            batch.Clear();
                            ClearMemory();
                        }
                    }
                }
                label10.Visible = false;
                MessageBox.Show("更新成功！");
            }
            catch (Exception ex)
            {
                label10.Visible = false;
                MessageBox.Show("请先完成数据获取");
            }
        }

        //public event ChangeDateSec ChangeSec;
        private void button9_Click(object sender, EventArgs e)
        {
            if (dt != null)
            {
                int count=0;
                var array = dt.Rows[dt.Rows.Count - 1].ItemArray;

                foreach (var item in array)
                {
                    if (Convert.ToDouble(item) < 0)
                        count++;
                }

                if (count <= 13)
                    MessageBox.Show("数据未足一年，无法更新上限至：" + dt.Rows[dt.Rows.Count - 1][0].ToString());
                else
                {
                    nino.MAX_YEAR = dt.Rows[dt.Rows.Count - 1][0].ToString();
                    MessageBox.Show("时间上限更新至"+ nino.MAX_YEAR);
                }

            }
        }

        private void button8_MouseDown(object sender, MouseEventArgs e)
        {
            label10.Visible = true;
        }
    }
}
