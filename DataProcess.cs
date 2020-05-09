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

        MongoClient client = new MongoClient("mongodb://admin:password@47.101.201.58:14285/?authSource=admin&authMechanism=SCRAM-SHA-256&readPreference=primary&appname=MongoDB%20Compass&ssl=false"); // mongoDB连接

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "" || textBox3.Text.Trim() == "")
            {
                MessageBox.Show("请完整输入数据");
            }
            else if (Convert.ToInt32(textBox1.Text) >= 180 || Convert.ToInt32(textBox1.Text) < -180 || Convert.ToInt32(textBox2.Text) >= 90 || Convert.ToInt32(textBox2.Text) < -90)
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
                var filter = filterBuilder.Eq("Lon", Math.Round((Convert.ToDouble(textBox1.Text) + 0.025),3)) & filterBuilder.Eq("Lat", Math.Round((Convert.ToDouble(textBox2.Text) + 0.025), 3) );
                var list = collection.Find<SST>(filter).ToList();
                if (list.Count == 0)
                {
                    MessageBox.Show("未查询到该条记录，请检查您的输入");
                }
                else
                {
                    DialogResult dr = MessageBox.Show("将更新\nLon = "+textBox1.Text+"\nLat = "+textBox2.Text+"\nTime = "+dateTimePicker1.Text+"处的SST为\n"+textBox3.Text, "更新确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (dr == DialogResult.OK)
                    {
                        var update = Builders<SST>.Update.Set("Band", Convert.ToDouble(textBox3.Text));
                        var result = collection.UpdateOne(filter, update);
                        MessageBox.Show("修改成功");
                    }
                }
            }

        }
        ///// <summary>
        ///// SST单独修改的类
        ///// </summary>
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
            //dt.Clear();
            dt.Rows.Clear();
            dt.Columns.Clear();
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

                    //只能按顺序插入数据
                    DateTime d_toinsert = Convert.ToDateTime(dt2ctname + "-28");
                    DateTime d_nowmax = Convert.ToDateTime(month_list[month_list.Count-1].month + "-28");

                    if(d_nowmax.Month!= 12 && (d_toinsert.Year-d_nowmax.Year)> 0 ) 
                    {
                        MessageBox.Show("插入数据与原有月份不连续，请重新选择文件");
                        return;
                    }
                    else if (d_nowmax.Month == 12 && (d_toinsert.Year - d_nowmax.Year) >1)
                    {
                        MessageBox.Show("插入数据与原有月份不连续，请重新选择文件");
                        return;
                    }
                    else if ( (d_toinsert.Year - d_nowmax.Year) == 0 && (d_toinsert.Month - d_nowmax.Month) > 1)
                    {
                        MessageBox.Show("插入数据与原有月份不连续，请重新选择文件");
                        return;
                    }
                    object[,] data = GetExcelRangeData(strpath, "A2", 0);
                    for (int i = 0; i < data.GetLength(1); i++)
                        dt.Columns.Add(i.ToString(), typeof(object));
                    //MessageBox.Show("读好了");
                    //DataRow dr = dt.NewRow();
                    for (int i = 0; i < data.GetLength(0); i++)
                    {
                        object[] dr = new object[data.GetLength(1)];
                        for (int j = 0; j < data.GetLength(1); j++)
                        {
                            dr[j] = data[i + 1, j + 1];
                        }
                        dt.Rows.Add(dr);
                        Console.WriteLine(i.ToString());
                    }
                    //MessageBox.Show("读好了");
                    BindingSource bs = new BindingSource();
                    bs.DataSource = dt;
                    dataGridView1.DataSource = bs;
                    //dataGridView1.DataSource = dt;
                    dataGridView1.Columns[0].HeaderCell.Value = "Lon";
                    dataGridView1.Columns[1].HeaderCell.Value = "Lat";
                    dataGridView1.Columns[2].HeaderCell.Value = "SST(K)";
                    dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                    label8.Text = data.GetLength(0).ToString();
                    label6.Text = filename;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// 读入EXCEL文件
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="stCell"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        /// 
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

       /// <summary>
       /// SST数据入库的类
       /// </summary>
        public class SST_single
        {
            public double Lon { get; set; }
            public double Lat { get; set; }
            public double Band { get; set; }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var database = client.GetDatabase("SST_res"); //数据库名称
            database.DropCollection(dt2ctname);
            var collection = database.GetCollection<BsonDocument>(dt2ctname);
            var count = 0;
            var batch = new List<BsonDocument>();
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    count++;

                    SST_single sST = new SST_single();
                    sST.Lon = Math.Round(Convert.ToDouble(dr[0].ToString()), 3);
                    sST.Lat = Math.Round(Convert.ToDouble(dr[1].ToString()), 3);
                    sST.Band = Convert.ToSingle(dr[2].ToString());

                    var sstdocument = new BsonDocument(sST.ToBsonDocument());

                    batch.Add(new BsonDocument(sstdocument));

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
                Month_show();
            }
            else
            {
                MessageBox.Show("DataTable为空，请先导入数据");
            }
        }

        //手动清除内存
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

        /// <summary>
        /// NINO数据入库类
        /// </summary>
        public class NINO_single
        {
            public string Year { get; set; }
            public float Jan { get; set; }
            public float Feb { get; set; }
            public float Mar { get; set; }
            public float Apr { get; set; }
            public float May { get; set; }
            public float June { get; set; }
            public float July { get; set; }
            public float Aug { get; set; }
            public float Sept { get; set; }
            public float Oct { get; set; }
            public float Nov { get; set; }
            public float Dec { get; set; }
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
                    //dt.Rows.Clear();
                    //dt.Columns.Clear();
                    strpath = System.Windows.Forms.Application.StartupPath + "\\nino_cr\\" + file[idx];
                    filename = strpath.Substring(strpath.LastIndexOf("\\") + 1);//去掉了路径
                    dt2ctname = filename.Substring(0, filename.LastIndexOf("."));//去掉后缀名
                    object[,] data = GetExcelRangeData(strpath, "A2", 1);
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

                            NINO_single nino= new NINO_single();
                            nino.Year = dr[0].ToString();
                            nino.Jan = Convert.ToSingle(dr[1].ToString());
                            nino.Feb = Convert.ToSingle(dr[2].ToString());
                            nino.Mar = Convert.ToSingle(dr[3].ToString());
                            nino.Apr = Convert.ToSingle(dr[4].ToString());
                            nino.May = Convert.ToSingle(dr[5].ToString());
                            nino.June = Convert.ToSingle(dr[6].ToString());
                            nino.July = Convert.ToSingle(dr[7].ToString());
                            nino.Aug = Convert.ToSingle(dr[8].ToString());
                            nino.Sept = Convert.ToSingle(dr[9].ToString());
                            nino.Oct = Convert.ToSingle(dr[10].ToString());
                            nino.Nov = Convert.ToSingle(dr[11].ToString());
                            nino.Dec = Convert.ToSingle(dr[12].ToString());

                            var sstdocument = new BsonDocument(nino.ToBsonDocument());

                            batch.Add(new BsonDocument(sstdocument));
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
                MessageBox.Show("更新成功！查询区间将在重新登录后生效");
            }
            catch (Exception ex)
            {
                label10.Visible = false;
                MessageBox.Show("请先完成数据获取");
            }
        }


        private void button8_MouseDown(object sender, MouseEventArgs e)
        {
            label10.Visible = true;
        }
        public class month_single
        {
            public string month { get; set; }
        }

        List<month_single> month_list = new List<month_single>(); //月份列表
        List<string> mons_list = new List<string>();
        private void DataProcess_Load(object sender, EventArgs e)
        {
            Month_show();
            dateTimePicker1.MaxDate = Convert.ToDateTime(管理员页面.MAXMONTH);
        }

        void Month_show()
        {
            month_list.Clear();
            mons_list.Clear();
            var database = client.GetDatabase("SST_res");
            var months = database.ListCollectionNames();
            mons_list = months.ToList();
            mons_list.Sort();
            for (int i = 0; i < mons_list.Count - 3; i++) // 最后三个是NINO的表 不要取进来
            {
                month_single ms = new month_single();
                ms.month = mons_list[i];
                month_list.Add(ms);
            }
            BindingList<month_single> bd_list = new BindingList<month_single>(month_list);
            dataGridView2.DataSource = bd_list;
            dataGridView2.DefaultCellStyle.ForeColor = Color.Black;
        }

        private void singlesst_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage1;
        }

        private void nino_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage3;
        }

        private void allsst_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage2;
        }

        private void button6_MouseDown(object sender, MouseEventArgs e)
        {
            label9.Visible = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var database = client.GetDatabase("SST_res"); //数据库名称
            string ctname = dataGridView2.Rows[dataGridView2.Rows.Count - 2].Cells[0].Value.ToString();
            DialogResult dr = MessageBox.Show("将删除\t" + ctname + "\t的记录", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                database.DropCollection(ctname);
                MessageBox.Show("删除成功");
                Month_show();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
