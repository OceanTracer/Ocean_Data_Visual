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
    public partial class 注册成功 : Form
    {
        public 注册成功()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + "; Initial Catalog=OT_user ; Integrated Security=true");
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
            mysql = "update user_info set desire='" + intent + "' where umail='" + 注册界面.user_email + "'";
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
