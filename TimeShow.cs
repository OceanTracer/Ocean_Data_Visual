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
using time_series_plot;
using series_acf_pacf;
using series_anormal;
using series_spectrum;
using my_sarima;
using System.Threading;
using System.Runtime.InteropServices;//API
using Microsoft.Office.Interop.Excel;//Excel
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;
using System.Reflection;
using System.IO;

namespace Data_Visual
{
    public partial class TimeShow : Form
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

        public TimeShow()
        {
            InitializeComponent();
        }

        List<double> band = new List<double>();//SST列表
        List<string> time = new List<string>();//time列表
        int section;
        public delegate void UpdateUI();//委托用于更新UI
        Thread startload;//线程用于matlab窗体处理
        IntPtr figure1;//图像句柄

        /// <summary>
        /// 图像句柄调用
        /// </summary>


        private void TimeShow_Load(object sender, EventArgs e)
        {
            label2.Text = onepoint.lon.ToString();
            label3.Text = onepoint.lat.ToString();
            label5.Text = onepoint.start + "—" + onepoint.final;
        }
        void DataGetnShow()
        {
            MongoClient client = new MongoClient("mongodb://admin:password@47.101.201.58:14285/?authSource=admin&authMechanism=SCRAM-SHA-256&readPreference=primary&appname=MongoDB%20Compass&ssl=false"); // mongoDB连接

            var database = client.GetDatabase("SST_res"); //数据库名称

            DateTime s = Convert.ToDateTime(onepoint.start);
            DateTime f = Convert.ToDateTime(onepoint.final);
            section = (f.Year - s.Year) * 12 + f.Month - s.Month + 1;
            DateTime this_year=s;
            string ctname;

            //加入listview
            listView1.Columns.Add("时间(Year-Month)",80);
            listView1.Columns.Add("温度(K)",80);
            listView1.Columns.Add("温度(°C)",80);

            for (int i=0;i<section;i++)
            {
                if (this_year.Month < 10)
                    ctname = this_year.Year.ToString() + "-0" + this_year.Month.ToString();
                else
                    ctname = this_year.Year.ToString() + "-" + this_year.Month.ToString();
                time.Add(ctname);

                var collection = database.GetCollection<BsonDocument>(ctname);

                var filterBuilder = Builders<BsonDocument>.Filter;
                var filter = filterBuilder.Eq("Lon", onepoint.lon) & filterBuilder.Eq("Lat", onepoint.lat);
                var result = collection.Find<BsonDocument>(filter).First();
                band.Add(Convert.ToDouble(result.GetValue("Band").ToString()));

                ListViewItem lt = new ListViewItem();
                //将数据库数据转变成ListView类型的一行数据
                lt.Text = ctname;
                lt.SubItems.Add(band[i].ToString());
                lt.SubItems.Add((band[i]-272.15).ToString());
                //将lt数据添加到listView1控件中
                listView1.Items.Add(lt);

                this_year=this_year.AddMonths(1);
            }
            this.listView1.View = System.Windows.Forms.View.Details;


        }
        AnormalPlot plot_1 = new AnormalPlot();
        SpectrumPlot plot_2 = new SpectrumPlot();
        AcfPlot plot_3 = new AcfPlot();

        private void button1_Click(object sender, EventArgs e)
        {

            FigureClose();
            AnormalPlot();
            ListViewItem res = listView2.FindItemWithText("Std", true, 0, false);
            if (res == null)
            {
                it = new ListViewItem();
                it.Text = "Std";
                it.SubItems.Add(Std.ToString());
                listView2.Items.Add(it);
            }
        }
        double Std = -1;
        void AnormalPlot()
        {
            MWNumericArray band_m = new MWNumericArray(MWArrayComplexity.Real, 1, section);
            MWCellArray time_m = new MWCellArray(section);
            MWArray result;
            for (int i = 0; i < section; i++)
            {
                band_m[i + 1] = band[i];
                time_m[i + 1] = time[i];
            }
            if (section <= 36)
                result = plot_1.series_anormal(band_m, time_m, 1);
            else
                result = plot_1.series_anormal(band_m, time_m, 0);
            var temp = result.ToString();
            Std = Convert.ToDouble(temp);
        }

        void SpectrumPlot()
        {
            MWNumericArray band_m = new MWNumericArray(MWArrayComplexity.Real, 1, section);
            MWCellArray time_m = new MWCellArray(section);
            for (int i = 0; i < section; i++)
            {
                band_m[i + 1] = band[i];
                time_m[i + 1] = time[i];
            }
            MWArray result;
            if (section <= 36)
                result = plot_2.series_spectrum(band_m, time_m, 1);
            else
                result = plot_2.series_spectrum(band_m, time_m, 0);
        }
        List<double> pq = new List<double>();
        void AcfPlot()
        {
            FigureClose();
            MWNumericArray band_m = new MWNumericArray(MWArrayComplexity.Real, 1, section);
            MWArray result;
            for (int i = 0; i < section; i++)
            {
                band_m[i + 1] = band[i];
            }
            result = plot_3.series_acf_pacf(band_m);
            var temp = result.ToArray();
            foreach( var it in temp)
            {
                pq.Add(Convert.ToDouble(it));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
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
                    MessageBox.Show( "matlab资源加载时间过长！");
                    return;
                }
            }
            //跨线程，用委托方式执行
            UpdateUI update = delegate
            {
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
            startload.Abort();

            while (startload.ThreadState != ThreadState.Aborted)
            {
                //当调用Abort方法后，如果thread线程的状态不为Aborted，主线程就一直在这里做循环，直到thread线程的状态变为Aborted为止
                Thread.Sleep(100);
            }
        }
        void FigureClose()
        {
            if(startload != null)
                startload.Abort();
            if (figure1 != IntPtr.Zero && IsWindow(figure1))
            {
                SendMessage(figure1, WM_CLOSE, 0, 0);  // 调用了 发送消息 发送关闭窗口的消息
                MessageBox.Show("我应该关了");
            }
            else
            {
                figure1 = IntPtr.Zero;
                MessageBox.Show("没找到这个窗口");
            }
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

            saveFileDialog1.FileName = label2.Text + "_" + label3.Text + "_" + "SST";

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

        private void pictureBox2_Click(object sender, EventArgs e)
        {

            this.Close();
            this.Owner.Show();
        }

        private void TimeShow_Shown(object sender, EventArgs e)
        {
            DataGetnShow();
            StaticsGet();
        }
        ListViewItem it;
        void StaticsGet()
        {
            listView2.Columns.Add("统计量", 80);
            listView2.Columns.Add("数值", 80);

            double band_max = band.Max();
            double band_min = band.Min();
            double band_avg = band.Average();
            it = new ListViewItem();
            it.Text = "Max";
            it.SubItems.Add((band_max-272.13).ToString());
            listView2.Items.Add(it);

            it = new ListViewItem();
            it.Text = "Min";
            it.SubItems.Add((band_min-272.13).ToString());
            listView2.Items.Add(it);

            it = new ListViewItem();
            it.Text = "Mean";
            it.SubItems.Add((band_avg-272.13).ToString());
            listView2.Items.Add(it);
            this.listView2.View = System.Windows.Forms.View.Details;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            FigureClose();
            SpectrumPlot();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FigureClose();
            AcfPlot();
            ListViewItem res = listView2.FindItemWithText("p", true, 0, false);
            if (res == null)
            {
                it = new ListViewItem();
                it.Text = "p";
                it.SubItems.Add(pq[0].ToString());
                listView2.Items.Add(it);

                it = new ListViewItem();
                it.Text = "q";
                it.SubItems.Add(pq[1].ToString());
                listView2.Items.Add(it);
            }
            button6.Enabled = true;
        }
        MySarima sarima = new MySarima();
        private void button6_Click(object sender, EventArgs e)
        {
            MWNumericArray band_m = new MWNumericArray(MWArrayComplexity.Real, 1, section);
            MWCellArray time_m = new MWCellArray(section);
            for (int i = 0; i < section; i++)
            {
                band_m[i + 1] = band[i];
                time_m[i + 1] = time[i];
            }
            MWArray result;
            if (section <= 48)
                MessageBox.Show("选取时间区间过短，无法进行SARIMA预测");
            else
                result = sarima.my_sarima(band_m, time_m, pq[0],pq[1]);
        }
    }
}
