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

namespace ocean
{
    public partial class 注册成功 : Form
    {
        public 注册成功()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection("database=InternJi;data source=LAPTOP-KBCJUDIO;integrated security=true");
        string mysql;
        public static string intent;
        DataSet mydataset = new DataSet();
        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            intent = "";
            foreach (Control c in groupBox1.Controls)
            {
                if (c is CheckBox)
                {
                    CheckBox r = c as CheckBox;
                    if (r.Checked)
                        intent += r.Text + ",";
                }
            }
            mysql = "update seeker set seeker_desire='" + intent + "' where seeker_ID=(select ID from userinfo where user_mail='" + 注册界面.user_email + "')";
            SqlCommand mycmd = new SqlCommand(mysql, myconn);
            myconn.Open();
            {
                try
                {
                    mycmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }
            myconn.Close();
            Hide();
            登录界面 f_log = new 登录界面();
            f_log.Owner = this.Owner;
            f_log.ShowDialog();
        }
    }
}
