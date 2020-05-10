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
using System.Runtime.InteropServices;

namespace Data_Visual
{
    public partial class 用户中心 : Form
    {
        public 用户中心()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            fill_info(登录界面.mail);
            InfoGroupBox.BringToFront();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        string mysql;
        DataSet mydataset = new DataSet();

        private void fill_info(string mail)
        {
            try
            {
                mysql = "select uname, sex, desire, describe from user_info where umail='" + mail+ "'";
                SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
                mydataset.Clear();
                myadapter.Fill(mydataset, "info");
                labelUname.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][0]);
                labelSex.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][1]);
                labelDesire.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][2]);
                labelDesc.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][3]);
                labelMail.Text = mail;
            }
            catch (Exception)
            {
                ;//MessageBox.Show("请先完善个人信息！");
                //return;
            }
        }




        private void InfoLabel_Click(object sender, EventArgs e)
        {
            fill_info(登录界面.mail);
            NoticeBox.SendToBack();
            InfoGroupBox.BringToFront();
            /*InfoGroupBox.Show();
            RecordGroupBox.Hide();
            CVGroupBox.Hide();
            EditGroupBox.Hide();*/
        }


        private void HomeLabel_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
            //求职者主页 f_see = new 求职者主页();
            //f_see.ShowDialog();
        }


        private void LogoutButton_Click(object sender, EventArgs e)
        {
            Close();
            welcome f_wel = new welcome();
            f_wel.ShowDialog();
            
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            InfoGroupBox.SendToBack();
            EditGroupBox.BringToFront();
            radioButtonMan.Checked = true;
            textBoxMail.Text = 登录界面.mail;
            textBoxDesire.Text = labelDesire.Text;
            textBoxDescribe.Text = labelDesc.Text;
            if (labelSex.Text == "男")
                radioButtonMan.Checked = true;
            if (labelSex.Text == "女")
                radioButtonWoman.Checked = true;
            /*InfoGroupBox.Hide();
            CVGroupBox.Hide();
            RecordGroupBox.Hide();
            EditGroupBox.Show();*/
        }

        private void shapeContainer1_Load(object sender, EventArgs e)
        {

        }

        private void 用户中心_Load(object sender, EventArgs e)
        {
            fill_info(登录界面.mail);
            
        }

        private void MyFav_Click(object sender, EventArgs e)
        {
            if (登录界面.mail == "")
                MessageBox.Show("未登录！");
            else
            {
                我的收藏 f1 = new 我的收藏();
                f1.Owner = this;
                Hide();
                f1.ShowDialog();
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void labelName_Click(object sender, EventArgs e)
        {

        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string sex="";
            if (radioButtonMan.Checked == true)
                sex = "男";
            if (radioButtonWoman.Checked == true)
                sex = "女";
            mysql = "update user_info set sex='" + sex + "' , desire='" + textBoxDesire.Text + "' , describe='" + textBoxDescribe.Text + "' where umail='" + 登录界面.mail+"'";
                SqlCommand mycmd = new SqlCommand(mysql, myconn);
                myconn.Open();
                try
                {
                    mycmd.ExecuteNonQuery();
                    MessageBox.Show("修改成功！", "提示");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            myconn.Close();

            /*EditGroupBox.Hide();
            InfoGroupBox.Show();*/
            EditGroupBox.SendToBack();
            NoticeBox.SendToBack();
            InfoGroupBox.BringToFront();
            fill_info(登录界面.mail);
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

        private void RecordLabel_Click(object sender, EventArgs e)
        {
            NoticeBox.BringToFront();

            listView1.Clear();
            listView1.Columns.Add("通知内容", 400);
            listView1.Columns.Add("通知时间", 150);
            List<string[]> history= GetNotice(登录界面.mail);
            for (int i = 0; i < history.Count; i++)
            {
                ListViewItem It = new ListViewItem();
                It.Text = history[i][0];
                It.SubItems.Add(history[i][1]);
                listView1.Items.Add(It);
            }
            this.listView1.View = System.Windows.Forms.View.Details;
        }

        private void runlabel_Click(object sender, EventArgs e)
        {
            if (登录界面.mail == "")
                MessageBox.Show("未登录！");
            else
            {
                知识闯关 f1 = new 知识闯关();
                f1.Owner = this;
                Hide();
                f1.ShowDialog();
            }
        }
    }
}
