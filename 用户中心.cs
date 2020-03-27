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
            fill_info(登录界面.ID);
            InfoGroupBox.BringToFront();
        }
        SqlConnection myconn = new SqlConnection("database=InternJi;data source=LAPTOP-KBCJUDIO;integrated security=true");
        string mysql;
        DataSet mydataset = new DataSet();

        private void fill_info(int ID)
        {
            try
            {
                mysql = "select seeker_name,seeker_sex,seeker_tel,seeker_status,seeker_degree,seeker_school,seeker_major,seeker_religion,seeker_describe from seeker where seeker_ID=" + ID;
                SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
                mydataset.Clear();
                myadapter.Fill(mydataset, "info");
                labelUname.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][0]);
                labelName.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][0]);
                labelSex.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][1]);
                labelTel.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][2]);
                labelStatus.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][3]);
                labelDegree.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][4]);
                labelSchool.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][5]);
                labelMajor.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][6]);
                labelReligion.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][7]);
                labelDesc.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][8]);
            }
            catch (Exception)
            {
                ;//MessageBox.Show("请先完善个人信息！");
                //return;
            }
        }


        private void SaveButton_Click(object sender, EventArgs e)
        {
            mysql = "update seeker set seeker_name='" + textBoxName.Text + "' , seeker_sex='" + comboBoxSex.SelectedItem + "' , seeker_tel='" + textBoxTel.Text + "' , seeker_status='" + comboBoxStatus.SelectedItem + "' , seeker_degree='" + comboBoxDegree.SelectedItem + "' , seeker_school='" + comboBoxSchool.SelectedItem + "' , seeker_major='" + comboBoxMajor.SelectedItem + "', seeker_religion='" + comboBoxReligion.SelectedItem + "', seeker_describe='" + textBoxDesc.Text + "' where seeker_ID=" + 登录界面.ID;
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
            fill_info(登录界面.ID);
           
            /*EditGroupBox.Hide();
            InfoGroupBox.Show();*/
            InfoGroupBox.BringToFront();
        }

        private void InfoLabel_Click(object sender, EventArgs e)
        {
            fill_info(登录界面.ID);
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
            /*InfoGroupBox.Hide();
            CVGroupBox.Hide();
            RecordGroupBox.Hide();
            EditGroupBox.Show();*/
        }
    }
}
