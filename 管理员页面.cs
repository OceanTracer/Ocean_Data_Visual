using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Data_Visual
{
    public partial class 管理员页面 : Form
    {
        public 管理员页面()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
            welcome f15 = new welcome();
            f15.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            科普管理 f1=new 科普管理();
            f1.Owner = this;
            //Hide();
            f1.ShowDialog();
        }

        private void label2_Click(object sender, EventArgs e)
        {           
            Option f2 = new Option();
            f2.Owner = this;
            //Hide();
            f2.ShowDialog();
        }

        private void label3_Click(object sender, EventArgs e)
        {               
            用户信息 f3 = new 用户信息();
            f3.Owner = this;
            //Hide();
            f3.ShowDialog();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        MongoClient client = new MongoClient("mongodb://admin:password@47.101.201.58:14285/?authSource=admin&authMechanism=SCRAM-SHA-256&readPreference=primary&appname=MongoDB%20Compass&ssl=false"); // mongoDB连接
        public static string MAXMONTH = "";
        public static string MAXYEAR = "";

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

        private void 管理员页面_Load(object sender, EventArgs e)
        {

        }
        int tick_count = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            tick_count++;
            if(tick_count ==2)
            {
                SectionGet();
                label5.Visible = false;
                timer1.Stop();
                timer1.Dispose();
            }
        }
        void SectionGet()
        {
            UserStatus.status = 0;
            var database = client.GetDatabase("SST_res");
            var months = database.ListCollectionNames();
            List<string> mons_list = months.ToList();
            mons_list.Sort();
            MAXMONTH = mons_list[mons_list.Count - 5];

            List<double> nino_list = new List<double>();
            string[] month_label = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sept", "Oct", "Nov", "Dec" };
            string ctname = "Nino4";
            var collection = database.GetCollection<BsonDocument>(ctname);
            var filterBuilder = Builders<BsonDocument>.Filter;

            var filter = filterBuilder.Empty;
            var result = collection.Find<BsonDocument>(filter).ToList();
            var item = result[result.Count - 1];

            MAXYEAR = item.GetValue("Year").ToString();
            for (int i = 0; i < 12; i++)
                nino_list.Add(Convert.ToDouble(item.GetValue(month_label[i])));

            ///检查最新一年的数据是否已满
            int count = 0;
            if (nino_list != null)
            {
                for (int i = 0; i < 12; i++)
                {
                    if (Convert.ToDouble(nino_list[i]) < 0)
                        count++;
                }

                if (count > 0) // 没满就回退一年
                    MAXYEAR = (Convert.ToInt32(MAXYEAR) - 1).ToString();
            }
        }

        private void 管理员页面_Shown(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            社区交流 f4 = new 社区交流();
            f4.Owner = this;
            Hide();
            f4.ShowDialog();
        }

    }
}
