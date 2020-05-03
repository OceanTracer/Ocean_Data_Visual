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
using Microsoft.Office.Interop.Word;
using MongoDB.Bson;
using MongoDB.Driver;
namespace Data_Visual
{
    public partial class 登录界面 : Form
    {
        public 登录界面()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        string mysql;
        DataSet mydataset = new DataSet();
        private void label6_Click(object sender, EventArgs e)
        {
            Close();
            System.Environment.Exit(0);
        }
        public static int  type = 0;
        public static string mail = "";
        public static string MAXMONTH = "";
        public static string MAXYEAR = "";
        //登录
        public int login(string email, string password, ref int type)
        {
            /*检查用户是否被禁用*/
            mysql = "select enabled from user_info where umail='" + email + "'";
            try
            {
                SqlDataAdapter myadapter0 = new SqlDataAdapter(mysql, myconn);
                myadapter0.Fill(mydataset, "checkEnabled");
                string enabled = mydataset.Tables["checkEnabled"].Rows[0][0].ToString();
                if (enabled == "N")
                    return 2;
            }
            catch (Exception)
            {
                return 3;
            }
            /*核对登录密码*/
            mysql = "select upsword,u_status from user_info where umail='" + email + "'";
            mail = email;
            try
            {
                SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
                myadapter.Fill(mydataset, "_email");
                type = Convert.ToInt32(mydataset.Tables["_email"].Rows[0][1]);
                string pass = Convert.ToString(mydataset.Tables["_email"].Rows[0][0]);
                if (password == pass)
                    return 0;
                else
                    return 1;
            }
            catch (Exception)
            {
                return 3;
            }
            
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            管理员页面 f14 = new 管理员页面();
            f14.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            管理员页面 f14 = new 管理员页面();
            f14.ShowDialog();
            Close();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "") 
                MessageBox.Show("用户名或密码不能为空！");
            else
            {
                string a = textBox1.Text;
                string b = textBox2.Text;
                int c = login(a, b,ref type);
                if (c == 1)
                    MessageBox.Show("用户名或密码错误！请重试。", "登录错误");
                else if (c == 2)
                    MessageBox.Show("您的账号已被禁用！", "登录错误");
                else if (c == 3)
                    MessageBox.Show("未知错误！请重试。", "登录错误");
                else
                {
                    Hide();
                    if (type == 0)
                    {
                        管理员页面 f_adm = new 管理员页面();
                        f_adm.Owner = this.Owner;
                        f_adm.ShowDialog();
                    }
                    if (type == 1)
                    {
                        用户主页 f_see = new 用户主页();
                        f_see.Owner = this.Owner;
                        f_see.ShowDialog();
                    }
                }
            }
        }
        MongoClient client = new MongoClient("mongodb://localhost"); // mongoDB连接
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
        private void 登录界面_Load(object sender, EventArgs e)
        {
            var database = client.GetDatabase("SST_res");
            var months = database.ListCollectionNames();
            List<string> mons_list = months.ToList();
            mons_list.Sort();
            MAXMONTH = mons_list[mons_list.Count - 4];

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

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                LoginButton_Click(sender, e);
        }
    }
}
