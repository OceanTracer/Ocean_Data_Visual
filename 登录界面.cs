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
        SqlConnection myconn = new SqlConnection(@"Data Source=.; Initial Catalog=OT_user ; Integrated Security=true");
        string mysql;
        DataSet mydataset = new DataSet();
        private void label6_Click(object sender, EventArgs e)
        {
            Close();
            System.Environment.Exit(0);
        }
        public static int ID = 0, type = 0;
        //登录
        public int login(string email, string password, ref int type)
        {
            mysql = "select upsword,u_status from user_info where umail='" + email + "'";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            myadapter.Fill(mydataset, "_email");
            try
            {
                type = Convert.ToInt32(mydataset.Tables["_email"].Rows[0][1]);
                string pass = Convert.ToString(mydataset.Tables["_email"].Rows[0][0]);
                if (password == pass)
                    return 1;
                else
                    return 0;
            }
            catch (Exception)
            {
                return 0;
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
                if (c == 0)
                    MessageBox.Show("用户名或密码错误！请重试。","登录错误");
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

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                LoginButton_Click(sender, e);
        }
    }
}
