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
using CCWin.SkinClass;

namespace Data_Visual
{
    public partial class 科普界面new : Form
    {
        public 科普界面new()
        {
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        string sql;
        DataSet mydataset = new DataSet();
        int FileCount = 0;
        Image img;
        private void 科普界面new_Load(object sender, EventArgs e)
        {
            initializeCount();
            fetchCollect(1);
        }

        private void initializeCount()
        {
            sql = "Select count(*) from collect_info";
            SqlCommand cmd = new SqlCommand(sql, myconn);
            try
            {
                myconn.Open();
                FileCount = Convert.ToInt32(cmd.ExecuteScalar());
                myconn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void fetchCollect(int id)
        {
            byte[] bytes = new byte[0];
            sql = "select collect_txt, collect_pic from collect_info where collect_num=" + id.ToString();
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
                img = Image.FromStream(mystream,true);
                skinPictureBox1.Image = img;
                mystream.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            label1.Text = cur.ToString() + "/" + FileCount.ToString();
        }

        int cur = 1;
        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            cur = cur - 1;
            if (cur < 1)
                cur = FileCount;
            fetchCollect(cur);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            cur = cur + 1;
            if (cur > FileCount)
                cur = 1;
            fetchCollect(cur);
        }

        private void buttonCollect_Click(object sender, EventArgs e)
        {
            if (登录界面.mail== "")
                MessageBox.Show("未登录！");
            else
            {
                try
                {
                    SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
                    string mycmd = "insert into collect  VALUES('" + 登录界面.mail + "','" + cur + "',null)";
                    string mycmd1= "select collect_num from collect where umail='" + 登录界面.mail+"'";
                    //统计已经收藏个数
                    DataSet mydataset = new DataSet();
                    SqlDataAdapter myadapter = new SqlDataAdapter(mycmd1, myconn);
                    myadapter.Fill(mydataset, "_email");
                    int count_all = mydataset.Tables["_email"].Rows.Count;
                    if (count_all >= account.N)
                        MessageBox.Show("收藏已达上限！");
                    else
                    {   //收藏
                        SqlCommand sqlCommand = new SqlCommand(mycmd, myconn);
                        Console.WriteLine(mycmd);
                        myconn.Open();
                        {
                            sqlCommand.ExecuteNonQuery();
                        }
                        myconn.Close();
                        MessageBox.Show("收藏成功！", "Ocean");
                    }
 
                }
                catch
                {
                    MessageBox.Show("已收藏！");
                }
            }
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            厄尔尼诺专题 form = new 厄尔尼诺专题();
            form.Owner = this;
            Hide();
            form.ShowDialog();
        }

        private void richTextBox1_MouseLeave(object sender, EventArgs e)
        {
            skinPictureBox1.Focus();
        }

        private void buttonUpload_Click(object sender, EventArgs e)
        {
            科普上传 form = new 科普上传();
            form.Owner = this;
            form.ShowDialog();
            initializeCount();
        }
    }
}
