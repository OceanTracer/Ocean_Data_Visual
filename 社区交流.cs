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
using CCWin.SkinClass;

namespace Data_Visual
{
    public partial class 社区交流 : Form
    {
        public 社区交流()
        {
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        string sql;
        DataSet mydataset = new DataSet();
        int section=1, page = 1;
        const int PAGESIZE = 5;
        List<Panel> panels = new List<Panel>();
        private void 社区交流_Load(object sender, EventArgs e)
        {
            panels.AddRange(new Panel[] { panel1, panel2, panel3, panel4, panel5 });
            FetchPosts(section, page);
        }

        private void PanelInit()
        {
            foreach(Panel p in panels)
            {
                p.Enabled = true;
                p.Visible = true;
            }
        }

        private void FetchPosts(int section, int page)
        {
            SqlCommand mycmd = new SqlCommand("fetchPosts", myconn);   //利用数据库的存储过程实现
            mycmd.CommandType = CommandType.StoredProcedure;
            SqlParameter page_num = new SqlParameter("@page_num ", SqlDbType.Int);
            SqlParameter page_size = new SqlParameter("@page_size ", SqlDbType.Int);
            SqlParameter sec = new SqlParameter("@section ", SqlDbType.SmallInt);
            page_num.Value = page;
            page_size.Value = PAGESIZE;
            sec.Value = section;
            mycmd.Parameters.AddRange(new SqlParameter[] { page_num, page_size, sec });
            PanelInit();
            myconn.Open();
            try
            {
                SqlDataReader res = mycmd.ExecuteReader();
                int i = 0;
                while (res.Read())
                {
                    panels[i].Controls[3].Text = res["post_title"].ToString();
                    panels[i].Controls[2].Text = res["uname"].ToString();
                    panels[i].Controls[1].Text = res["post_time"].ToString();
                    panels[i].Controls[0].Text = "回复量：" + res["post_repcnt"].ToString();
                    panels[i].Tag = res["post_id"];     //用panel的Tag属性记录此帖子的id，以便查询帖子详情时使用
                    i++;
                }
                for (; i < 5; i++)
                {
                    panels[i].Visible = false;
                    panels[i].Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }

        private void panels_MouseDown(object sender, MouseEventArgs e)
        {
            Panel p = (Panel)sender;
            帖子详情 fpost = new 帖子详情(Convert.ToInt32(p.Tag));
            fpost.Owner = this;
            Hide();
            fpost.ShowDialog();
        }

        private void labelSection1_Click(object sender, EventArgs e)
        {
            section = 1;
            page = 1;
            FetchPosts(section, page);
        }

        private void labelSection2_Click(object sender, EventArgs e)
        {
            section = 2;
            page = 1;
            FetchPosts(section, page);
        }

        private void labelSection3_Click(object sender, EventArgs e)
        {
            section = 3;
            page = 1;
            FetchPosts(section, page);
        }

        private void labelSection4_Click(object sender, EventArgs e)
        {
            section = 4;
            page = 1;
            FetchPosts(section, page);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            sql = "select count(*) from posts where post_section=" + section.ToString();
            SqlCommand mycmd = new SqlCommand(sql, myconn);
            myconn.Open();
            try
            {
                int total = mycmd.ExecuteScalar().ConvertTo<int>();
                if (page * PAGESIZE >= total) 
                {
                    MessageBox.Show("已到达尾页！", "Ocean");
                    myconn.Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            myconn.Close();
            page += 1;
            try
            {
                FetchPosts(section, page);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                page -= 1;
            }
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            if (page == 1)
            {
                MessageBox.Show("已到达首页！","Ocean");
                return;
            }
            page -= 1;
            try
            {
                FetchPosts(section, page);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                page += 1;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }

        private void buttonPost_Click(object sender, EventArgs e)
        {
            发送帖子 fsend = new 发送帖子();
            fsend.Owner = this;
            fsend.ShowDialog();
        }

    }
}
