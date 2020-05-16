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
    public partial class 发送帖子 : Form
    {
        public 发送帖子()
        {
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        string sql;
        private void buttonLaunch_Click(object sender, EventArgs e)
        {
            if (richTextBoxTitle.Text == "")
            {
                MessageBox.Show("请输入标题！", "Ocean");
                return;
            }
            //使用.Replace("'", "''")以防文字中带有单引号'，引起SQL字符串截断
            sql = "insert into posts values('" + 登录界面.mail + "','" + richTextBoxTitle.Text.Replace("'", "''") + "','" +
                richTextBoxContent.Text.Replace("'", "''") + "',GETDATE(),0," + (cbSection.SelectedIndex + 1).ToString() + ",'N')";
            SqlCommand mycmd = new SqlCommand(sql, myconn);
            try
            {
                myconn.Open();
                int res = mycmd.ExecuteNonQuery();
                if (res > 0)
                    MessageBox.Show("发送成功！经验+5.", "Ocean");
                myconn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
