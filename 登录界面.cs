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
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        string mysql;
        DataSet mydataset = new DataSet();
        private void label6_Click(object sender, EventArgs e)
        {
            Close();
            System.Environment.Exit(0);
        }
        public static int type = 0;
        public static string mail = "", uname = "";

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
                    return 2;   //已被封禁
            }
            catch (Exception)
            {
                return 3;
            }
            /*核对登录密码*/
            mysql = "select upsword,u_status,uname from user_info where umail='" + email + "'";
            mail = email;
            try
            {
                SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
                myadapter.Fill(mydataset, "_email");
                string pass = Convert.ToString(mydataset.Tables["_email"].Rows[0][0]);
                type = Convert.ToInt32(mydataset.Tables["_email"].Rows[0][1]);  //用户类型
                uname = Convert.ToString(mydataset.Tables["_email"].Rows[0][2]);
                if (password == pass)
                    return 0;   //成功登录
                else
                    return 1;   //密码错误
            }
            catch (Exception)
            {
                return 3;
            }
            
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
                    if (type == 0)
                    {
                        管理员页面 f_adm = new 管理员页面();
                        f_adm.Owner = this.Owner;
                        Hide();
                        f_adm.ShowDialog();
                    }
                    else
                    {
                        用户主页 f_see = new 用户主页();
                        f_see.Owner = this.Owner;
                        Hide();
                        f_see.ShowDialog();
                    }
                }
            }
        }

        private void 登录界面_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                LoginButton_Click(sender, e);
        }
    }
}
