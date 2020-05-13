using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;

namespace Data_Visual
{
    public partial class 举报界面 : Form
    {
        public 举报界面()
        {
            InitializeComponent();
        }

        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        DataSet mydataset = new DataSet();
        string sql;


        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("您确定要举报改科普的内容吗？您的举报都会被记录在案！", "确定举报", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                sql="update collect set isComplaint='Y',Com_content='"+richTextBox1.Text.ToString()+"'" +
                  " where umail='" + 登录界面.mail + "' and collect_num="+account.click_num.ToString()+"";
                SqlCommand mycmd = new SqlCommand(sql, myconn);
                myconn.Open();
                try
                {
                    mycmd.ExecuteNonQuery();
                    MessageBox.Show("举报成功！", "提示");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                myconn.Close();
            }
            else
            {
            }
        }

    }
}
