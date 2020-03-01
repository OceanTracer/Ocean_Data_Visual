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
using System.Threading;
using System.Runtime.InteropServices;//API

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
        [DllImport("user32")]
        private static extern int GetWindowLong(System.IntPtr hwnd, int nIndex);
        [DllImport("user32")]
        private static extern int SetWindowLong(System.IntPtr hwnd, int index, int newLong);
        #endregion

        public TimeShow()
        {
            InitializeComponent();
        }

        List<double> band = new List<double>();//SST列表
        List<string> time = new List<string>();//SST列表
        int section;
        public delegate void UpdateUI();//委托用于更新UI
        Thread startload;//线程用于matlab窗体处理
        IntPtr figure1;//图像句柄

        private void TimeShow_Load(object sender, EventArgs e)
        {
            label2.Text = onepoint.lon.ToString();
            label3.Text = onepoint.lat.ToString();
            label5.Text = onepoint.start + "—" + onepoint.final;
            DataGetnShow();
        }
        void DataGetnShow()
        {
            var client = new MongoClient("mongodb://localhost");
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
        TimePlotClass plot = new TimePlotClass();
        private void button1_Click(object sender, EventArgs e)
        {
            DataFigure();
        }
        void DataFigure()
        {
            MWNumericArray band_m = new MWNumericArray(MWArrayComplexity.Real, 1, section);
            MWCellArray time_m = new MWCellArray(section);
            for(int i=0;i<section;i++)
            {
                band_m[i + 1] = band[i];
                time_m[i + 1] = time[i];
            }
            if (section <= 36)
                plot.time_series_plot(band_m, time_m, 1);
            else
                plot.time_series_plot(band_m, time_m, 0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            startload = new Thread(new ThreadStart(startload_run));
            //运行线程方法
            startload.Start();
            label7.Visible = false;
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
                    label7.Text = "matlab资源加载时间过长！";
                    return;
                }
            }
            //跨线程，用委托方式执行
            UpdateUI update = delegate
            {
                //隐藏标签
                label7.Visible = false;
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
            label7.Visible = true;
        }
    }
}
