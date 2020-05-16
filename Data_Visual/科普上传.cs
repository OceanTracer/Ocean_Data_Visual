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
    public partial class 科普上传 : Form
    {
        public 科普上传()
        {
            InitializeComponent();
        }

        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        DataSet mydataset = new DataSet();

        string imagefile="", textfile="";
        int FileAll;

        void countAll()//用动态数组将id与collect_num对应起来！
        {
            string sql = "select collect_num from collect_info";
            DataSet mydataset2 = new DataSet();
            SqlDataAdapter myadapter = new SqlDataAdapter(sql, myconn);
            mydataset2.Clear();
            myadapter.Fill(mydataset2, "count");
            FileAll = Convert.ToInt32(mydataset2.Tables["count"].Rows.Count);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            OpenFileDialog opdia = new OpenFileDialog();
            opdia.Title = "请选择图像";
            opdia.Filter = "图像文件|*.png;*.jpg;*.bmp";
            if (opdia.ShowDialog() == DialogResult.Cancel)
                return;
            imagefile = opdia.FileName;
            FileStream fs = new FileStream(imagefile, FileMode.Open, FileAccess.Read);
            if (fs.Length > 1024 * 1024)
            {
                MessageBox.Show("您选择的图片过大！请选择小于1M的图片。", "Ocean");
                fs.Dispose();
                return;
            }
            fs.Dispose();
            label1.Visible = false;
            label1.Enabled = false;
            skinPictureBox1.Load(imagefile);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opdia = new OpenFileDialog();
            opdia.Title = "请选择文本";
            opdia.Filter = "文本|*.txt";
            if (opdia.ShowDialog() == DialogResult.Cancel)
                return;
            textfile = opdia.FileName;
            label2.Visible = false;
            label2.Enabled = false;
            richTextBox1.LoadFile(textfile, RichTextBoxStreamType.PlainText);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            label2.Visible = false;
            label2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(imagefile=="" || richTextBox1.Text == "")
            {
                MessageBox.Show("请先选择图像与本文", "Ocean");
                return;
            }
            countAll();
            FileStream fs = new FileStream(imagefile, FileMode.Open, FileAccess.Read);
            Byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            try
            {
                myconn.Open();
                string cmdText = "insert into collect_info values('"+(FileAll+1).ToString()+"','"+ richTextBox1.Text +"', @imgfile,'"+ 登录界面.mail + "','N')";
                //string cmdText = "insert into collect_info values('" + richTextBox1.Text + "', @imgfile, null)";
                SqlCommand cmd = new SqlCommand(cmdText, myconn);
                SqlParameter para = new SqlParameter("@imgfile", SqlDbType.Image);
                para.Value = bytes;
                cmd.Parameters.Add(para);

                int res = cmd.ExecuteNonQuery();
                if (res > 0)
                    MessageBox.Show("上传成功！已移交管理员审核。", "Ocean");
                myconn.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            button1.Enabled = false;
        }


    }
}
