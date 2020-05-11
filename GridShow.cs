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
using ll_grid_plot;
using System.Threading;
using System.Runtime.InteropServices;//API
using Microsoft.Office.Interop.Excel;//Excel
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;
using System.Reflection;
using System.IO;
using months_grid_plot;
using anormaly_grid_plot;

namespace Data_Visual
{
    public partial class GridShow : Form
    {
        #region //Windows API
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);//
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool BRePaint);
        const int GWL_STYLE = -16;
        const int WS_CAPTION = 0x00C00000;
        const int WS_THICKFRAME = 0x00040000;
        const int WS_SYSMENU = 0X00080000;
        const int WM_CLOSE = 0x0010;

        [DllImport("user32")]
        private static extern int GetWindowLong(System.IntPtr hwnd, int nIndex);
        [DllImport("user32")]
        private static extern int SetWindowLong(System.IntPtr hwnd, int index, int newLong);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll", EntryPoint = "IsWindow")]
        public static extern bool IsWindow(IntPtr hWnd);
        #endregion
        public GridShow()
        {
            InitializeComponent();
        }

        private void GridShow_Load(object sender, EventArgs e)
        {
            label2.Text = cover.lon_min.ToString() + "—" +cover.lon_max.ToString();
            label3.Text = cover.lat_min.ToString() + "—" + cover.lat_max.ToString();
            label5.Text = cover.time;
        }
        
        public delegate void UpdateUI();//委托用于更新UI
        Thread startload;//线程用于matlab窗体处理
        IntPtr figure1;//图像句柄
        MongoClient client = new MongoClient("mongodb://admin:password@47.101.201.58:14285/?authSource=admin&authMechanism=SCRAM-SHA-256&readPreference=primary&appname=MongoDB%20Compass&ssl=false"); // mongoDB连接
        List<double> band = new List<double>();//SST列表

        public MWNumericArray DataGet(string ctname)
        {
            int length = band.ToArray().Length;
            MWNumericArray band_temp = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            //List<double> band_temp = new List<double>();//SST列表
            
            var database = client.GetDatabase("SST_res"); //数据库名称
            var collection = database.GetCollection<BsonDocument>(ctname);

            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Gte("Lon", cover.lon_min) & filterBuilder.Gte("Lat", cover.lat_min) & filterBuilder.Lte("Lon", cover.lon_max) & filterBuilder.Lte("Lat", cover.lat_max);
            var result = collection.Find<BsonDocument>(filter).ToList();
            int i = 1;
            foreach (var item in result)
            {
                //band_temp.Add(Convert.ToDouble(item.GetValue("Band").ToString()));
                band_temp[i] = (Convert.ToDouble(item.GetValue("Band").ToString()));
                Console.WriteLine(i);
                i++;
            }

            return band_temp;
        }

        void DataGetnShow()
        {
            string ctname = cover.time;
            MongoClient client = new MongoClient("mongodb://admin:password@47.101.201.58:14285/?authSource=admin&authMechanism=SCRAM-SHA-256&readPreference=primary&appname=MongoDB%20Compass&ssl=false"); // mongoDB连接

            var database = client.GetDatabase("SST_res"); //数据库名称
            var collection = database.GetCollection<BsonDocument>(ctname);

            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Gte("Lon", cover.lon_min) & filterBuilder.Gte("Lat", cover.lat_min) & filterBuilder.Lte("Lon", cover.lon_max) & filterBuilder.Lte("Lat", cover.lat_max);
            var result = collection.Find<BsonDocument>(filter).ToList();
            //加入listview
            listView1.Columns.Add("经度(Lon)", 80);
            listView1.Columns.Add("纬度(Lat)", 80);
            listView1.Columns.Add("温度(K)", 80);
            listView1.Columns.Add("温度(°C)", 80);
            int i = 0;
            foreach (var item in result)
            {
                band.Add(Convert.ToDouble(item.GetValue("Band").ToString()));

                ListViewItem lt = new ListViewItem();
                //将数据库数据转变成ListView类型的一行数据
                lt.Text = Convert.ToDouble(item.GetValue("Lon").ToString()).ToString();
                lt.SubItems.Add(Convert.ToDouble(item.GetValue("Lat").ToString()).ToString());
                lt.SubItems.Add(band[i].ToString());
                lt.SubItems.Add((band[i] - 273.15).ToString());
                //将lt数据添加到listView1控件中
                listView1.Items.Add(lt);
                i++;
            }
            this.listView1.View = System.Windows.Forms.View.Details;
            label7.Visible = false;
            pictureBox2.Visible = false;
        }

        GridPlotClass plot_1 = new GridPlotClass();
        MonthsGridPlot plot_2 = new MonthsGridPlot();
        AnomalyGridPlot plot_3 = new AnomalyGridPlot();
        private void button1_Click(object sender, EventArgs e)
        {
            DataFigure();
        }
        void DataFigure()
        {
            int length = band.ToArray().Length;
            MWNumericArray band_m = new MWNumericArray(MWArrayComplexity.Real, length,1);
            for(int i=0;i<length;i++)
            {
                band_m[i + 1] = band[i];
            }
            plot_1.ll_grid_plot(cover.lon_max, cover.lon_min, cover.lat_max, cover.lat_min, band_m, 1, cover.time);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            figure1 = IntPtr.Zero;
            startload = new Thread(new ThreadStart(startload_run));
            //运行线程方法
            startload.Start();
        }
        void startload_run()
        {

            int count50ms = 0;
            //实例化matlab对象


            //循环查找figure1窗体
            while (figure1 == IntPtr.Zero)
            {
                //查找matlab的Figure 1窗体
                figure1 = FindWindow("SunAwtFrame", "Figure 1");
                //延时50ms
                Thread.Sleep(50);
                count50ms++;
                //20s超时设置
                if (count50ms >= 400)
                {
                    return;
                }
            }
            //跨线程，用委托方式执行
            UpdateUI update = delegate
            {
                //隐藏标签
                //设置matlab图像窗体的父窗体为panel
                SetParent(figure1, panel2.Handle);
                //获取窗体原来的风格
                var style = GetWindowLong(figure1, GWL_STYLE);
                //设置新风格，去掉标题,不能通过边框改变尺寸
                SetWindowLong(figure1, GWL_STYLE, style & ~WS_CAPTION & ~WS_THICKFRAME);
                //移动到panel里合适的位置并重绘
                MoveWindow(figure1, 0, 0, panel2.Width, panel2.Height, true);


            };
            panel2.Invoke(update);
            //再移动一次，防止显示错误
            Thread.Sleep(100);
            MoveWindow(figure1, 0, 0, panel2.Width, panel2.Height, true);
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
        }
        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        private void button3_Click(object sender, EventArgs e)
        {
            string saveFileName = "";
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel 文件(*.xls)|*.xls";
            saveFileDialog1.RestoreDirectory = true;
            saveFileName = saveFileDialog1.FileName;

            saveFileDialog1.FileName = label2.Text + "_" + label3.Text + "_" + label5.Text+"_SST";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                WriteListViewToExcel(listView1, "LOG");

            }
        }
        public void WriteListViewToExcel(ListView listView1, string sheet1)
        {
            string Sheetname = sheet1;
            ListView listView = listView1;
            if (listView.Items.Count < 1)
                return;
            try
            {
                ExcelApplication MyExcel = new ExcelApplication();

                MyExcel.Visible = true;   //display excel application；if value set 'false',please enable the ' finally' code below;
                if (MyExcel == null)
                {
                    return;
                }

                Workbooks MyWorkBooks = (Workbooks)MyExcel.Workbooks;

                Workbook MyWorkBook = (Workbook)MyWorkBooks.Add(Missing.Value);

                Worksheet MyWorkSheet = (Worksheet)MyWorkBook.Worksheets[1];


                Range MyRange = MyWorkSheet.get_Range("A1", "H1");
                MyRange = MyRange.get_Resize(1, listView.Columns.Count);
                object[] MyHeader = new object[listView.Columns.Count];
                for (int i = 0; i < listView.Columns.Count; i++)
                {
                    MyHeader.SetValue(listView.Columns[i].Text, i);
                }
                MyRange.Value2 = MyHeader;
                MyWorkSheet.Name = Sheetname;

                if (listView.Items.Count > 0)
                {
                    MyRange = MyWorkSheet.get_Range("A2", Missing.Value);
                    object[,] MyData = new Object[listView.Items.Count, listView.Columns.Count];
                    for (int j = 0; j < listView1.Items.Count; j++)
                    {
                        ListViewItem lvi = listView1.Items[j];
                        for (int k = 0; k < listView.Columns.Count; k++)
                        {

                            MyData[j, k] = lvi.SubItems[k].Text;
                        }

                    }
                    MyRange = MyRange.get_Resize(listView.Items.Count, listView.Columns.Count);
                    MyRange.Value2 = MyData;
                    MyRange.EntireColumn.AutoFit();
                }

                try
                {
                    object missing = System.Reflection.Missing.Value;
                    MyWorkBook.Saved = true;
                    MyWorkBook.SaveAs(saveFileDialog1.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, missing, missing, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing, missing);
                }
                catch (Exception e1)
                {
                    MessageBox.Show("Export Error,Maybe the file is opened by other application!\n" + e1.Message);
                }
                /*
                 finally
                     {
                          MyExcel.Quit();
                          System.GC.Collect();
                      }
                 */

                // MyExcel = null;

            }
            catch (Exception Err)
            {
                MessageBox.Show(Err.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (figure1 != IntPtr.Zero && IsWindow(figure1))
            {
                SendMessage(figure1, WM_CLOSE, 0, 0);  // 调用了 发送消息 发送关闭窗口的消息
                //MessageBox.Show("我应该关了");
            }
            else
            {
                figure1 = IntPtr.Zero;
                //MessageBox.Show("没找到这个窗口");
            }

            this.Close();
            this.Owner.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MonthsPlot();
        }

        void MonthsPlot()
        {
            int length = band.ToArray().Length;
            MWNumericArray band_1= new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_2 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_3 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_4 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWCellArray time_m = new MWCellArray(4);

            string this_time = cover.time;
            DateTime this_date = Convert.ToDateTime(this_time + "-28");
            string last1_time;
            string last2_time;
            string last3_time;
            
            if(Convert.ToDouble(this_date.AddMonths(-1).Month) >=10)
                 last1_time = this_date.AddMonths(-1).Year.ToString() + "-" + this_date.AddMonths(-1).Month.ToString();
            else
                last1_time = this_date.AddMonths(-1).Year.ToString() + "-0" + this_date.AddMonths(-1).Month.ToString();

            if (Convert.ToDouble(this_date.AddMonths(-2).Month) >= 10)
                last2_time = this_date.AddMonths(-2).Year.ToString() + "-" + this_date.AddMonths(-2).Month.ToString();
            else
                last2_time = this_date.AddMonths(-2).Year.ToString() + "-0" + this_date.AddMonths(-2).Month.ToString();

            last3_time = this_date.AddMonths(-3).Year.ToString() + "-0" + this_date.AddMonths(-3).Month.ToString();

            //MessageBox.Show(last1_time + last2_time + last3_time);

            band_2 = DataGet(last1_time);
            band_3 = DataGet(last2_time);
            band_4 = DataGet(last3_time);

            for (int i = 0; i < length; i++)
            {
                band_1[i + 1] = band[i];
            }
            time_m[1] = this_time;
            time_m[2] = last1_time;
            time_m[3] = last2_time;
            time_m[4] = last3_time;

            plot_2.months_grid_plot(cover.lon_max, cover.lon_min, cover.lat_max, cover.lat_min, band_1, band_2, band_3, band_4, 1, time_m);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            YearsPlot();
        }

        void YearsPlot()
        {
            int length = band.ToArray().Length;
            MWNumericArray band_1 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_2 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_3 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_4 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWCellArray time_m = new MWCellArray(4);

            string this_time = cover.time;
            DateTime this_date = Convert.ToDateTime(this_time + "-28");

            string last1_time;
            string last2_time;
            string last3_time;

            if (Convert.ToDouble(this_date.Month) >= 10)
            {
                last1_time = this_date.AddYears(-1).Year.ToString() + "-" + this_date.Month.ToString();
                last2_time = this_date.AddYears(-2).Year.ToString() + "-" + this_date.Month.ToString();
                last3_time = this_date.AddYears(-3).Year.ToString() + "-" + this_date.Month.ToString();
            }
            else
            {
                last1_time = this_date.AddYears(-1).Year.ToString() + "-0" + this_date.Month.ToString();
                last2_time = this_date.AddYears(-2).Year.ToString() + "-0" + this_date.Month.ToString();
                last3_time = this_date.AddYears(-3).Year.ToString() + "-0" + this_date.Month.ToString();
            }



            //MessageBox.Show(last1_time + last2_time + last3_time);

            band_2 = DataGet(last1_time);
            band_3 = DataGet(last2_time);
            band_4 = DataGet(last3_time);

            for (int i = 0; i < length; i++)
            {
                band_1[i + 1] = band[i];
            }
            time_m[1] = this_time;
            time_m[2] = last1_time;
            time_m[3] = last2_time;
            time_m[4] = last3_time;

            plot_2.months_grid_plot(cover.lon_max, cover.lon_min, cover.lat_max, cover.lat_min, band_1, band_2, band_3, band_4, 1, time_m);
        }

        /// <summary>
        /// 这俩我直接写到事件里面了
        /// </summary>
        private void button7_Click(object sender, EventArgs e)
        {
            int length = band.ToArray().Length;
            MWNumericArray band_1 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_2 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_3 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_4 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWCellArray time_m = new MWCellArray(4);

            string this_time = cover.time;
            DateTime this_date = Convert.ToDateTime(this_time + "-28");
            string last1_time;
            string last2_time;
            string last3_time;

            if (Convert.ToDouble(this_date.Month) >= 10)
            {
                last1_time = this_date.AddYears(-1).Year.ToString() + "-" + this_date.Month.ToString();
                last2_time = this_date.AddYears(-2).Year.ToString() + "-" + this_date.Month.ToString();
                last3_time = this_date.AddYears(-3).Year.ToString() + "-" + this_date.Month.ToString();
            }
            else
            {
                last1_time = this_date.AddYears(-1).Year.ToString() + "-0" + this_date.Month.ToString();
                last2_time = this_date.AddYears(-2).Year.ToString() + "-0" + this_date.Month.ToString();
                last3_time = this_date.AddYears(-3).Year.ToString() + "-0" + this_date.Month.ToString();
            }


            //MessageBox.Show(last1_time + last2_time + last3_time);

            band_2 = DataGet(last1_time);
            band_3 = DataGet(last2_time);
            band_4 = DataGet(last3_time);

            for (int i = 0; i < length; i++)
            {
                band_1[i + 1] = band[i];
            }
            time_m[1] = this_time;
            time_m[2] = last1_time;
            time_m[3] = last2_time;
            time_m[4] = last3_time;

            plot_3.anormaly_grid_plot(cover.lon_max, cover.lon_min, cover.lat_max, cover.lat_min, band_1, band_2, band_3, band_4, 1, time_m);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int length = band.ToArray().Length;
            MWNumericArray band_1 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_2 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_3 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWNumericArray band_4 = new MWNumericArray(MWArrayComplexity.Real, length, 1);
            MWCellArray time_m = new MWCellArray(4);

            string this_time = cover.time;
            DateTime this_date = Convert.ToDateTime(this_time + "-28");
            string last1_time;
            string last2_time;
            string last3_time;

            if (Convert.ToDouble(this_date.AddMonths(-1).Month) >= 10)
                last1_time = this_date.AddMonths(-1).Year.ToString() + "-" + this_date.AddMonths(-1).Month.ToString();
            else
                last1_time = this_date.AddMonths(-1).Year.ToString() + "-0" + this_date.AddMonths(-1).Month.ToString();

            if (Convert.ToDouble(this_date.AddMonths(-2).Month) >= 10)
                last2_time = this_date.AddMonths(-2).Year.ToString() + "-" + this_date.AddMonths(-2).Month.ToString();
            else
                last2_time = this_date.AddMonths(-2).Year.ToString() + "-0" + this_date.AddMonths(-2).Month.ToString();

            last3_time = this_date.AddMonths(-3).Year.ToString() + "-0" + this_date.AddMonths(-3).Month.ToString();

            //MessageBox.Show(last1_time + last2_time + last3_time);

            band_2 = DataGet(last1_time);
            band_3 = DataGet(last2_time);
            band_4 = DataGet(last3_time);

            for (int i = 0; i < length; i++)
            {
                band_1[i + 1] = band[i];
            }
            time_m[1] = this_time;
            time_m[2] = last1_time;
            time_m[3] = last2_time;
            time_m[4] = last3_time;

            plot_3.anormaly_grid_plot(cover.lon_max, cover.lon_min, cover.lat_max, cover.lat_min, band_1, band_2, band_3, band_4, 1, time_m);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            GridShowHelp form = new GridShowHelp();
            form.ShowDialog();
        }

        public class SST_single
        {
            public double Lon { get; set; }
            public double Lat { get; set; }
            public double Band { get; set; }
        }
        List<SST_single> error = new List<SST_single>();

        /// <summary>
        /// 这块需要用户给管理员发消息
        /// </summary>
        private void button9_Click(object sender, EventArgs e)
        {
            int count = 0;
            for( int i =0; i<band.Count; i++)
            {
                if (band[i] > 273.15 + 40 || band[i] < 273.15)
                {
                    listView1.Items[i].BackColor = Color.Red;
                    count++;
                    SST_single temp = new SST_single();
                    temp.Band = band[i];
                    temp.Lon = Convert.ToDouble(listView1.Items[i].SubItems[0].ToString());
                    temp.Lat = Convert.ToDouble(listView1.Items[i].SubItems[1].ToString());
                    error.Add(temp);
                }
            }
            MessageBox.Show("共检查到疑似错误记录共" + count.ToString() + "条");
        }

        private void GridShow_Shown(object sender, EventArgs e)
        {
            timer1.Interval = 100;
            timer1.Start();
        }
        int tick_count = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(tick_count == 1)
            {
                DataGetnShow();
                timer1.Stop();
                timer1.Dispose();
            }
            tick_count++;
        }
    }
}
