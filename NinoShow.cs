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
using nino_plot;
using System.Threading;
using System.Runtime.InteropServices;//API
using Microsoft.Office.Interop.Excel;//Excel
using ExcelApplication = Microsoft.Office.Interop.Excel.Application;
using System.Reflection;
using System.IO;

namespace Data_Visual
{
    public partial class NinoShow : Form
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
        public NinoShow()
        {
            InitializeComponent();
        }

        private void NinoShow_Load(object sender, EventArgs e)
        {
            label2.Text = nino.ref_start + "—" + nino.ref_final;
            label3.Text = nino.aim_start + "—" + nino.aim_final;
            DataGetnShow();
        }

        List<double> aim_12 = new List<double>();
        List<double> aim_3 = new List<double>();
        List<double> aim_4 = new List<double>();
        List<double> ref_12 = new List<double>();
        List<double> ref_3 = new List<double>();
        List<double> ref_4 = new List<double>();
        List<double> temp = new List<double>();
        int aim_sec;
        int ref_sec;
        public delegate void UpdateUI();//委托用于更新UI
        Thread startload;//线程用于matlab窗体处理
        IntPtr figure1,figure2,figure3,figure4;//图像句柄

        void DataGetnShow()
        {
            MongoClient client = new MongoClient("mongodb://admin:password@47.101.201.58:27017/?authSource=admin&authMechanism=SCRAM-SHA-256&readPreference=primary&appname=MongoDB%20Compass&ssl=false"); // mongoDB连接

            var database = client.GetDatabase("SST_res"); //数据库名称
            int this_year;
            string ctname;
            string[] months = new string[] {"Jan","Feb","Mar","Apr","May","June","July","Aug","Sept","Oct","Nov","Dec" };
            ref_sec = Convert.ToInt32(nino.ref_final) - Convert.ToInt32(nino.ref_start) + 1;
            aim_sec = Convert.ToInt32(nino.aim_final) - Convert.ToInt32(nino.aim_start) + 1;
            ListViewInit();

            //Nino1+2
            //读参考并计算
            this_year = Convert.ToInt32(nino.ref_start);
            ctname = "Nino1+2";
            var collection = database.GetCollection<BsonDocument>(ctname);
            for (int i=0;i<ref_sec;i++)
            {
                var filterBuilder = Builders<BsonDocument>.Filter;
                var filter = filterBuilder.Eq("Year", this_year.ToString());
                var result = collection.Find<BsonDocument>(filter).First();
                if (i == 0)
                    for (int j=0;j<12;j++)
                    {
                        ref_12.Add(Convert.ToDouble(result.GetValue(months[j]).ToString()));
                    }
                else
                    for (int j = 0; j < 12; j++)
                    {
                        ref_12[j]+= (Convert.ToDouble(result.GetValue(months[j]).ToString()));
                    }
                this_year++;
            }
            for (int j = 0; j < 12; j++)//求平均
            {
                ref_12[j] /= ref_sec;
            }
            //读目标并作差(距平）
            this_year = Convert.ToInt32(nino.aim_start);
            for (int i=0;i<aim_sec;i++)
            {
                
                ListViewItem lt = new ListViewItem();
                //将数据库数据转变成ListView类型的一行数据
                lt.Text = this_year.ToString();

                var filterBuilder = Builders<BsonDocument>.Filter;
                var filter = filterBuilder.Eq("Year", this_year.ToString());
                var result = collection.Find<BsonDocument>(filter).First();
                for (int j = 0; j < 12; j++)
                {
                    aim_12.Add((Convert.ToDouble(result.GetValue(months[j]).ToString())- ref_12[j]));
                    lt.SubItems.Add(aim_12[i*12+j].ToString());
                }
                listView1.Items.Add(lt);
                this_year++;
            }
            this.listView1.View = System.Windows.Forms.View.Details;

            //Nino3
            //读参考并计算
            this_year = Convert.ToInt32(nino.ref_start);
            ctname = "Nino3";
            collection = database.GetCollection<BsonDocument>(ctname);
            for (int i = 0; i < ref_sec; i++)
            {
                var filterBuilder = Builders<BsonDocument>.Filter;
                var filter = filterBuilder.Eq("Year", this_year.ToString());
                var result = collection.Find<BsonDocument>(filter).First();
                if (i == 0)
                    for (int j = 0; j < 12; j++)
                    {
                        ref_3.Add(Convert.ToDouble(result.GetValue(months[j]).ToString()));
                    }
                else
                    for (int j = 0; j < 12; j++)
                    {
                        ref_3[j] += (Convert.ToDouble(result.GetValue(months[j]).ToString()));
                    }
                this_year++;
            }
            for (int j = 0; j < 12; j++)//求平均
            {
                ref_3[j] /= ref_sec;
            }
            //读目标并作差(距平）
            this_year = Convert.ToInt32(nino.aim_start);
            for (int i = 0; i < aim_sec; i++)
            {

                ListViewItem lt = new ListViewItem();
                //将数据库数据转变成ListView类型的一行数据
                lt.Text = this_year.ToString();

                var filterBuilder = Builders<BsonDocument>.Filter;
                var filter = filterBuilder.Eq("Year", this_year.ToString());
                var result = collection.Find<BsonDocument>(filter).First();
                for (int j = 0; j < 12; j++)
                {
                    aim_3.Add((Convert.ToDouble(result.GetValue(months[j]).ToString()) - ref_3[j]));
                    lt.SubItems.Add(aim_3[i * 12 + j].ToString());
                }
                listView2.Items.Add(lt);
                this_year++;
            }
            this.listView2.View = System.Windows.Forms.View.Details;

            //Nino4
            //读参考并计算
            this_year = Convert.ToInt32(nino.ref_start);
            ctname = "Nino4";
            collection = database.GetCollection<BsonDocument>(ctname);
            for (int i = 0; i < ref_sec; i++)
            {
                var filterBuilder = Builders<BsonDocument>.Filter;
                var filter = filterBuilder.Eq("Year", this_year.ToString());
                var result = collection.Find<BsonDocument>(filter).First();
                if (i == 0)
                    for (int j = 0; j < 12; j++)
                    {
                        ref_4.Add(Convert.ToDouble(result.GetValue(months[j]).ToString()));
                    }
                else
                    for (int j = 0; j < 12; j++)
                    {
                        ref_4[j] += (Convert.ToDouble(result.GetValue(months[j]).ToString()));
                    }
                this_year++;
            }
            for (int j = 0; j < 12; j++)//求平均
            {
                ref_4[j] /= ref_sec;
            }
            //读目标并作差(距平）
            this_year = Convert.ToInt32(nino.aim_start);
            for (int i = 0; i < aim_sec; i++)
            {

                ListViewItem lt = new ListViewItem();
                //将数据库数据转变成ListView类型的一行数据
                lt.Text = this_year.ToString();

                var filterBuilder = Builders<BsonDocument>.Filter;
                var filter = filterBuilder.Eq("Year", this_year.ToString());
                var result = collection.Find<BsonDocument>(filter).First();
                for (int j = 0; j < 12; j++)
                {
                    aim_4.Add((Convert.ToDouble(result.GetValue(months[j]).ToString()) - ref_4[j]));
                    lt.SubItems.Add(aim_4[i * 12 + j].ToString());
                }
                listView3.Items.Add(lt);
                this_year++;
            }
            this.listView3.View = System.Windows.Forms.View.Details;
        }
        void ListViewInit()
        {
            listView1.Columns.Add("时间(Year-Month)", 80);
            listView1.Columns.Add("Jan.", 70);
            listView1.Columns.Add("Feb.", 70);
            listView1.Columns.Add("Mar.", 70);
            listView1.Columns.Add("Apr.", 70);
            listView1.Columns.Add("May.", 70);
            listView1.Columns.Add("Jun.", 70);
            listView1.Columns.Add("Jul.", 70);
            listView1.Columns.Add("Aug.", 70);
            listView1.Columns.Add("Sept.", 70);
            listView1.Columns.Add("Oct.", 70);
            listView1.Columns.Add("Nov.", 70);
            listView1.Columns.Add("Dec.", 70);

            listView2.Columns.Add("时间(Year-Month)", 80);
            listView2.Columns.Add("Jan.", 70);
            listView2.Columns.Add("Feb.", 70);
            listView2.Columns.Add("Mar.", 70);
            listView2.Columns.Add("Apr.", 70);
            listView2.Columns.Add("May.", 70);
            listView2.Columns.Add("Jun.", 70);
            listView2.Columns.Add("Jul.", 70);
            listView2.Columns.Add("Aug.", 70);
            listView2.Columns.Add("Sept.", 70);
            listView2.Columns.Add("Oct.", 70);
            listView2.Columns.Add("Nov.", 70);
            listView2.Columns.Add("Dec.", 70);

            listView3.Columns.Add("时间(Year-Month)", 80);
            listView3.Columns.Add("Jan.", 70);
            listView3.Columns.Add("Feb.", 70);
            listView3.Columns.Add("Mar.", 70);
            listView3.Columns.Add("Apr.", 70);
            listView3.Columns.Add("May.", 70);
            listView3.Columns.Add("Jun.", 70);
            listView3.Columns.Add("Jul.", 70);
            listView3.Columns.Add("Aug.", 70);
            listView3.Columns.Add("Sept.", 70);
            listView3.Columns.Add("Oct.", 70);
            listView3.Columns.Add("Nov.", 70);
            listView3.Columns.Add("Dec.", 70);
        }

        NinoPlotClass plot = new NinoPlotClass();
        private void button1_Click(object sender, EventArgs e)
        {
            DataFigure();
            label7.Visible = true;
        }
        void DataFigure()
        {
            MWNumericArray aim12_m = new MWNumericArray(MWArrayComplexity.Real, aim_sec*12, 1);
            MWNumericArray aim3_m = new MWNumericArray(MWArrayComplexity.Real, aim_sec * 12, 1);
            MWNumericArray aim4_m = new MWNumericArray(MWArrayComplexity.Real, aim_sec * 12, 1);
            for(int i=0;i<aim_sec*12;i++)
            {
                aim12_m[i + 1] = aim_12[i];
                aim3_m[i + 1] = aim_3[i];
                aim4_m[i + 1] = aim_4[i];
            }
            if (aim_sec <= 3)
                plot.nino_plot(aim12_m, aim3_m, aim4_m, Convert.ToInt32(nino.aim_start), Convert.ToInt32(nino.aim_final), 1);
            else
                plot.nino_plot(aim12_m, aim3_m, aim4_m, Convert.ToInt32(nino.aim_start), Convert.ToInt32(nino.aim_final), 0);
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            label7.Visible = true;
        }
        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        private void button3_Click(object sender, EventArgs e)
        {
            string saveFileName = "";
            saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Excel 文件(*.xls)|*.xls";
            saveFileDialog1.RestoreDirectory = true;
            saveFileName = saveFileDialog1.FileName;

            saveFileDialog1.FileName = "r_" + label2.Text + "_a_" + label3.Text + "_" + tabControl1.SelectedTab.Text;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if(this.tabControl1.SelectedTab.Controls[0].Text=="listView1")
                    WriteListViewToExcel(listView1, "LOG");
                if (this.tabControl1.SelectedTab.Controls[0].Text == "listView2")
                    WriteListViewToExcel(listView2, "LOG");
                if (this.tabControl1.SelectedTab.Controls[0].Text == "listView3")
                    WriteListViewToExcel(listView3, "LOG");
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
            this.Close();
            this.Owner.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PanelInit();
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
            while (figure1 == IntPtr.Zero|| figure2 == IntPtr.Zero|| figure3 == IntPtr.Zero|| figure4 == IntPtr.Zero)
            {
                //查找matlab的Figure 1窗体
                figure1 = FindWindow("SunAwtFrame", "Figure 1");
                figure2 = FindWindow("SunAwtFrame", "Figure 2");
                figure3 = FindWindow("SunAwtFrame", "Figure 3");
                figure4 = FindWindow("SunAwtFrame", "Figure 4");
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
                //设置matlab图像窗体的父窗体为panel
                SetParent(figure1, panel2.Handle);
                //获取窗体原来的风格
                var style = GetWindowLong(figure1, GWL_STYLE);
                //设置新风格，去掉标题,不能通过边框改变尺寸
                SetWindowLong(figure1, GWL_STYLE, style & ~WS_CAPTION & ~WS_THICKFRAME);
                //移动到panel里合适的位置并重绘
                MoveWindow(figure1, 0, 0, panel2.Width, panel2.Height, true);

                //设置matlab图像窗体的父窗体为panel
                SetParent(figure2, panel3.Handle);
                //获取窗体原来的风格
                style = GetWindowLong(figure2, GWL_STYLE);
                //设置新风格，去掉标题,不能通过边框改变尺寸
                SetWindowLong(figure2, GWL_STYLE, style & ~WS_CAPTION & ~WS_THICKFRAME);
                //移动到panel里合适的位置并重绘
                MoveWindow(figure1, 0, 0, panel3.Width, panel3.Height, true);

                //设置matlab图像窗体的父窗体为panel
                SetParent(figure3, panel4.Handle);
                //获取窗体原来的风格
                style = GetWindowLong(figure3, GWL_STYLE);
                //设置新风格，去掉标题,不能通过边框改变尺寸
                SetWindowLong(figure3, GWL_STYLE, style & ~WS_CAPTION & ~WS_THICKFRAME);
                //移动到panel里合适的位置并重绘
                MoveWindow(figure3, 0, 0, panel4.Width, panel4.Height, true);

                //设置matlab图像窗体的父窗体为panel
                SetParent(figure4, panel5.Handle);
                //获取窗体原来的风格
                style = GetWindowLong(figure4, GWL_STYLE);
                //设置新风格，去掉标题,不能通过边框改变尺寸
                SetWindowLong(figure4, GWL_STYLE, style & ~WS_CAPTION & ~WS_THICKFRAME);
                //移动到panel里合适的位置并重绘
                MoveWindow(figure4, 0, 0, panel5.Width, panel5.Height, true);
            };
            panel2.Invoke(update);
            panel3.Invoke(update);
            panel4.Invoke(update);
            panel5.Invoke(update);
            //再移动一次，防止显示错误
            Thread.Sleep(100);
            MoveWindow(figure1, 0, 0, panel2.Width, panel2.Height, true);
            MoveWindow(figure2, 0, 0, panel3.Width, panel3.Height, true);
            MoveWindow(figure3, 0, 0, panel4.Width, panel4.Height, true);
            MoveWindow(figure4, 0, 0, panel5.Width, panel5.Height, true);
        }
        void PanelInit()
        {
            panel2.Location= new System.Drawing.Point(45, 215);
            panel2.Size = new System.Drawing.Size(1161, 278);

            panel3.Location = new System.Drawing.Point(45, 493);
            panel3.Size = new System.Drawing.Size(1161, 278);

            panel4.Location = new System.Drawing.Point(45, 771);
            panel4.Size = new System.Drawing.Size(1161, 278);

            panel5.Location = new System.Drawing.Point(45, 1049);
            panel5.Size = new System.Drawing.Size(1161, 278);
        }
    }
}
