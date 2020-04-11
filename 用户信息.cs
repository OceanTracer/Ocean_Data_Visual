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
    public partial class 用户信息 : Form
    {
        public 用户信息()
        {
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ;Initial Catalog=OT_user ; Integrated Security=true");
        string mysql;
        DataSet mydataset = new DataSet();

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }

        private void 用户信息_Load(object sender, EventArgs e)
        {
            mysql = "select umail, uname, sex, desire, u_status, describe from user_info";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "info");
            ListViewInit();

            for (int i = 0; i < mydataset.Tables["info"].Rows.Count; i++)
            {
                ListViewItem lt = new ListViewItem();
                lt.Text = mydataset.Tables["info"].Rows[i][0].ToString();
                for (int j = 0; j < 5; j++)
                    lt.SubItems.Add(mydataset.Tables["info"].Rows[i][j].ToString());
                listView1.Items.Add(lt);
            }
            this.listView1.View = System.Windows.Forms.View.Details;
        }

        void ListViewInit()
        {
            listView1.Columns.Add("用户邮箱", 80);
            listView1.Columns.Add("用户名", 70);
            listView1.Columns.Add("性别", 70);
            listView1.Columns.Add("兴趣", 70);
            listView1.Columns.Add("身份", 70);
            listView1.Columns.Add("描述", 200);
        }


        /// <summary>
        /// 根据用户名查找用户邮箱；需要在listView1已被填充后调用
        /// </summary>
        /// <param name="uname">用户名</param>
        private string SearchUser(string uname)
        {
            ListViewItem res = listView1.FindItemWithText(uname, true, 0, true);
            string umail = res.SubItems[0].Text;
            return umail;
        }

        /// <summary>根据用户邮箱封禁用户</summary>
        /// <param name="umail">用户邮箱</param>
        private void DisableUser(string umail)
        {
            string sql = "update user_info set enabled='N' where umail='" + umail + "'";
            SqlCommand mycmd = new SqlCommand(sql, myconn);
            myconn.Open();
            try
            {
                mycmd.ExecuteNonQuery();
                MessageBox.Show("已禁用用户"+umail, "OceanTracer");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }

        /// <summary>根据用户邮箱解封用户</summary>
        /// <param name="umail">用户邮箱</param>
        private void EnableUser(string umail)
        {
            string sql = "update user_info set enabled='Y' where umail='" + umail + "'";
            SqlCommand mycmd = new SqlCommand(sql, myconn);
            myconn.Open();
            try
            {
                mycmd.ExecuteNonQuery();
                MessageBox.Show("已启用用户" + umail, "OceanTracer");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }

        
    }
}
