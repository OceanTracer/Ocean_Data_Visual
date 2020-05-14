using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data_Visual
{
    public partial class 用户主页 : Form
    {
        public 用户主页()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        DataSet mydataset = new DataSet();

        private void ExitButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                welcome fwel = new welcome();
                fwel.ShowDialog();
                //this.Close();
                //this.Owner.Show();
            }
            catch (Exception) { }
        }

        private void labelSearch_Click(object sender, EventArgs e)
        {
            /*  精准查询 f5 = new 精准查询();
             f5.Owner = this;
             Hide();
             f5.ShowDialog();*/
            Data_Visual.Option form = new Data_Visual.Option();
            form.Owner = this;
            Hide();
            form.ShowDialog();
        }

        private void labelSelect_Click(object sender, EventArgs e)
        {
            科普界面new f1 = new 科普界面new();
            f1.Owner = this;
            Hide();
            f1.ShowDialog();
        }

        private void labelUserC_Click(object sender, EventArgs e)
        {
            用户中心 f_user = new 用户中心();
            f_user.Owner = this;
            Hide();
            f_user.ShowDialog();
        }

        private List<string[]> GetNotice(string umail)
        {
            List<string[]> noticeList = new List<string[]>();
            //按时间倒序查询此用户收到的通知
            string sql = "select notice_content, notice_time from notice where umail='" + umail + "'order by notice_time desc";
            string ntc_content, ntc_time;
            SqlDataAdapter myadapter = new SqlDataAdapter(sql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "notice");
            for (int i = 0; i < mydataset.Tables["notice"].Rows.Count; i++)
            {
                ntc_content = mydataset.Tables["notice"].Rows[i][0].ToString();
                ntc_time = mydataset.Tables["notice"].Rows[i][1].ToString();
                string[] ntc = new string[] { ntc_content, ntc_time };
                noticeList.Add(ntc);
            }
            return noticeList;
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

        private void 用户主页_Load(object sender, EventArgs e)
        {
            UserStatus.status = 1;
            
        }
        void SectionGet()
        {
            List<string[]> notice = GetNotice(登录界面.mail);
            if (notice.Count != 0)
                label1.Text = "Attention : " + notice[0][0];
            else
                label1.Text = "No Attention ";

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
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void 用户主页_Shown(object sender, EventArgs e)
        {
            timer1.Interval = 500;
            timer1.Start();
        }
        int tick_count = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            tick_count++;
            if (tick_count == 1)
            {
                SectionGet();
                label5.Visible = false;
                timer1.Stop();
                timer1.Dispose();
            }
        }

    }
}
