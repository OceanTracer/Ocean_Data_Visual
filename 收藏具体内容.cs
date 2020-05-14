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
using System.IO;

namespace Data_Visual
{
    public partial class 收藏具体内容 : Form
    {
        public 收藏具体内容()
        {
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        Image img;
        private void fetchCollect(int id)
        {
            byte[] bytes = new byte[0];
            string sql = @"select collect_txt, collect_pic from collect_info 
                    where collect_num=" + id.ToString();
            SqlCommand cmd = new SqlCommand(sql, myconn);
            try
            {
                myconn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                richTextBox1.Text = sdr["collect_txt"].ToString();
                bytes = (byte[])sdr["collect_pic"];
                sdr.Close();
                myconn.Close();
                MemoryStream mystream = new MemoryStream(bytes);
                //用指定的数据流来创建一个image图片
                img = Image.FromStream(mystream, true);
                pictureBox1.Image = img;
                mystream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }


        private void 收藏具体内容_Load(object sender, EventArgs e)
        {
            int numb = account.click_num;
            fetchCollect(numb);
        }

        private void richTextBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
