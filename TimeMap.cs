using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace Data_Visual
{
    public partial class TimeMap : Form
    {
        public TimeMap()
        {
            InitializeComponent();
        }
        public ChromiumWebBrowser wb;

        private void TimeMap_Load(object sender, EventArgs e)
        {
             if (CefSharp.Cef.IsInitialized == false)
                  WbInit();
             string sURL = Application.StartupPath + @"/BaiduMap/point.html";
             wb = new ChromiumWebBrowser(sURL);
             panel1.Controls.Add(wb);
             wb.Dock = DockStyle.Fill;
             timer1.Interval = 2000;
             timer1.Start();
        }
        void WbInit()
        {
            Cef.EnableHighDPISupport();     //适应屏幕缩放
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            var setting = new CefSettings();
            setting.Locale = "zh-CN";
            // 缓存路径
            setting.CachePath = "/BrowserCache";
            // 浏览器引擎的语言
            setting.AcceptLanguageList = "zh-CN,zh;q=0.8";
            setting.LocalesDirPath = "/localeDir";
            // 日志文件
            setting.LogFile = "/LogData";
            setting.PersistSessionCookies = true;
            setting.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (Khtml, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
            setting.UserDataPath = "/userData";

            setting.MultiThreadedMessageLoop = true;
            setting.CefCommandLineArgs.Add("--disable-web-security", "");
            setting.CefCommandLineArgs.Add("--user-data-dir", "C:\\MyChromeDevUserData");

            CefSharp.Cef.Initialize(setting);
            
        }

        public int lon_temp;
        public int lat_temp;
        private void button1_Click(object sender, EventArgs e)
        {
            const string script = @"(function()
    					{
	    					var linksArray = new Array();
	    					for (var i = 0; i < 2; i++)
	    					{
	    						linksArray[i] = llinfo[i+1];
	    					}
	    					return linksArray;
    					})();";

            Task task = wb.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    var list = (List<object>)response.Result;
                    if(list[0]==null)
                    {
                        MessageBox.Show("请先在地图中点击选择");
                    }
                    else
                    {
                        lon_temp = Convert.ToInt32(Convert.ToDouble(list[0])) ;
                        lat_temp = Convert.ToInt32(Convert.ToDouble(list[1]));
                    }
                }
            });
            task.Wait();
            textBox1.Text = lon_temp.ToString();
            textBox2.Text = lat_temp.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "")
            {
                MessageBox.Show("输入的经纬度有误");
                label5.Visible = false;
            }
            else if (Convert.ToInt32(textBox1.Text) > 180 || Convert.ToInt32(textBox1.Text) < -180|| Convert.ToInt32(textBox2.Text) > 90|| Convert.ToInt32(textBox2.Text) < -90)
            {
                MessageBox.Show("输入的经纬度有误");
                label5.Visible = false;
            }
            else if(DateTime.Compare(Convert.ToDateTime(dateTimePicker1.Text), Convert.ToDateTime(dateTimePicker2.Text))>0 )
            {
                MessageBox.Show("输入的日期有误");
                label5.Visible = false;
            }
            else
            {
                onepoint.lon = Convert.ToDouble(textBox1.Text)+0.025;
                onepoint.lat = Convert.ToDouble(textBox2.Text)+0.025;
                onepoint.start = dateTimePicker1.Text.ToString();
                onepoint.final = dateTimePicker2.Text.ToString();
                TimeShow form = new TimeShow();
                form.Owner = this;
                Hide();
                form.ShowDialog();
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是负号，退格且不能转为小数，则屏蔽输入
            if (!(e.KeyChar == '-' || e.KeyChar == '\b' || float.TryParse(((TextBox)sender).Text + e.KeyChar.ToString(), out float f)))
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '-' && ((TextBox)sender).Text.Length > 1)
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //如果输入的不是负号，退格且不能转为小数，则屏蔽输入
            if (!(e.KeyChar == '-' || e.KeyChar == '\b' || float.TryParse(((TextBox)sender).Text + e.KeyChar.ToString(), out float f)))
            {
                e.Handled = true;
            }
            else if (e.KeyChar==',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '-' && ((TextBox)sender).Text.Length > 1)
            {
                e.Handled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button1.Enabled = true;
            timer1.Stop();
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            label5.Visible = true;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            //this.Dispose(); //Dispose()方法在这里会让Cef每次都耗费同样的时间加载；不使用Dispose()似乎可以节省二次加载时间(不是很确定)
            this.Owner.Show();
        }
    }
}
