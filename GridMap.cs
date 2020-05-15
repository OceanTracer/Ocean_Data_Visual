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
    public partial class GridMap : Form
    {
        public GridMap()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }
        public ChromiumWebBrowser wb;

        private void GridMap_Load(object sender, EventArgs e)
        {
            if (CefSharp.Cef.IsInitialized == false) 
                WbInit();
            string sURL = Application.StartupPath + @"/BaiduMap/region.html";
            wb = new ChromiumWebBrowser(sURL);
            panel2.Controls.Add(wb);
            wb.Dock = DockStyle.Fill;
            timer1.Interval = 2000;
            timer1.Start();

            string MM = "";
            if (UserStatus.status == 1)
                MM = 用户主页.MAXMONTH;
            else if (UserStatus.status == 0)
                MM = 管理员页面.MAXMONTH;
            DateTime d1 = Convert.ToDateTime(MM + "-28");

            dateTimePicker1.MaxDate = d1;

        }
        void WbInit()
        {
            Cef.EnableHighDPISupport();     //适应屏幕缩放
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSettings setting = new CefSettings();

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

        public int lon_max;
        public int lon_min;
        public int lat_max;
        public int lat_min;
        private void button2_Click(object sender, EventArgs e)
        {
            string script = @"(function()
    					{
	    					var linksArray = new Array();
	    					for (var i = 0; i < 4; i++)
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
                    if (list[0] == null)
                    {
                        MessageBox.Show("请先在地图中点击选择");
                        return;
                    }
                    else
                    {
                        lon_max = Convert.ToInt32(Convert.ToDouble(list[0].ToString()));
                        lon_min = Convert.ToInt32(Convert.ToDouble(list[1].ToString()));
                        lat_max = Convert.ToInt32(Convert.ToDouble(list[2].ToString()));
                        lat_min = Convert.ToInt32(Convert.ToDouble(list[3].ToString()));
                    }
                }
            });
            task.Wait();
            //由于框选的起始点不同，所以无法确定
            if (lat_max - lat_min >= 1||(lat_max==0&&lat_min==0))
            {
                textBox1.Text = lat_max.ToString();
                textBox2.Text = lat_min.ToString();
            }
            else if (lat_min - lat_max >= 1)
            {
                textBox1.Text = lat_min.ToString();
                textBox2.Text = lat_max.ToString();
            }
            else
            {
                MessageBox.Show("框选的范围有误");
            }

            if (lon_max - lon_min >= 1|| (lon_max == 0 && lon_min == 0))
            {
                textBox3.Text = lon_min.ToString();
                textBox4.Text = lon_max.ToString();
            }
            else if (lon_min - lon_max >= 1)
            {
                textBox3.Text = lon_max.ToString();
                textBox4.Text = lon_min.ToString();
            }
            else
            {
                MessageBox.Show("框选的范围有误");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button2.Enabled = true;
            timer1.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string script = "var pStart = new BMap.Point(" + textBox3.Text + "," + textBox1.Text + "); var pEnd = new BMap.Point(" + textBox4.Text + "," + textBox2.Text + ");var rectangle = new BMap.Polygon ([new BMap.Point(pStart.lng, pStart.lat), new BMap.Point(pEnd.lng, pStart.lat),new BMap.Point(pEnd.lng, pEnd.lat),new BMap.Point(pStart.lng, pEnd.lat)],{ strokeColor: \"blue\", strokeWeight: 2, strokeOpacity: 0.5});map.addOverlay(rectangle);";
            Console.WriteLine(script);
            wb.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success == false)
                {
                    MessageBox.Show("定位失败");
                }
            });
         }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text.Trim()==""|| textBox2.Text.Trim() == ""|| textBox3.Text.Trim() == ""|| textBox4.Text.Trim() == "")
            {
                MessageBox.Show("输入范围为空");
                label5.Visible = false;
            }
            else if(Convert.ToInt32(textBox1.Text)>=90|| Convert.ToInt32(textBox2.Text) >= 90|| Convert.ToInt32(textBox1.Text) < -90|| Convert.ToInt32(textBox2.Text) < -90)
            {
                MessageBox.Show("输入范围不合理");
                label5.Visible = false;
            }
            else if (Convert.ToInt32(textBox3.Text) >= 180 || Convert.ToInt32(textBox4.Text) >= 180 || Convert.ToInt32(textBox3.Text) < -180 || Convert.ToInt32(textBox4.Text) < -180)
            {
                MessageBox.Show("输入范围不合理");
                label5.Visible = false;
            }
            else if (textBox3.Text == textBox4.Text || textBox1.Text == textBox2.Text)
            {
                MessageBox.Show("输入范围不合理");
                label5.Visible = false;
            }
            else
            {
                cover.lat_max = Convert.ToDouble(textBox1.Text)+0.025;
                cover.lat_min = Convert.ToDouble(textBox2.Text)+0.025;
                cover.lon_max = Convert.ToDouble(textBox4.Text)+0.025;
                cover.lon_min = Convert.ToDouble(textBox3.Text)+0.025;
                cover.time = dateTimePicker1.Text;
                GridShow form = new GridShow();
                form.Owner = this;
                Hide();
                form.ShowDialog();
            }
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            label5.Visible = true;
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
            else if (e.KeyChar == '.')
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
            else if (e.KeyChar == ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '-' && ((TextBox)sender).Text.Length > 1)
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
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
            else if (e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '-' && ((TextBox)sender).Text.Length > 1)
            {
                e.Handled = true;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
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
            else if (e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == '-' && ((TextBox)sender).Text.Length > 1)
            {
                e.Handled = true;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Owner.Show();
            this.Dispose();
        }
    }
}
