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
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user ; Integrated Security=true");
        string mysql;
        DataSet mydataset = new DataSet();
        private void ovalShape1_Click(object sender, EventArgs e)
        {
           /* 精准查询 f5 = new 精准查询();
            f5.Owner = this;
            Hide();
            f5.ShowDialog();*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /* 推荐 f1 = new 推荐();
             f1.Owner = this;
             Hide();
             f1.ShowDialog();*/
        }

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

        private void 用户主页_Load(object sender, EventArgs e)
        {
            List<string[]> notice = GetNotice(登录界面.mail);
            if(notice.Count != 0)
                label1.Text = "Attention : " + notice[0][0];
            else
                label1.Text = "No Attention ";
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
