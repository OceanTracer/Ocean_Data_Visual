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
        SqlConnection myconn = new SqlConnection(@"Data Source=. ; Initial Catalog=OT_user ; Integrated Security=true");
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
            EditGroupBox.BringToFront();
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
                fill_info(登录界面.mail);

                /*EditGroupBox.Hide();
                InfoGroupBox.Show();*/
                InfoGroupBox.BringToFront();
        }
    }
}
