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
    public partial class 科普界面new : Form
    {
        public 科普界面new()
        {
            InitializeComponent();
        }

        int FileCount = 0;
        private void 科普界面new_Load(object sender, EventArgs e)
        {
            this.skinPictureBox1.Image = Image.FromFile(@"pic_all\1.jpg");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = @"pic_all\1.txt";
            richTextBox1.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);

            /////获取文件数目

            DirectoryInfo Dir = new DirectoryInfo(@"pic_all");
            foreach (FileInfo FI in Dir.GetFiles())
            {
                if (System.IO.Path.GetExtension(FI.Name) == ".txt")
                {
                    FileCount++;
                }
            }
        }

        int cur = 1;
        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            cur = cur - 1;
            if (cur < 1)
                cur = FileCount;
            this.skinPictureBox1.Image = Image.FromFile(@"pic_all\" + cur + ".jpg");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = @"pic_all\" + cur + ".txt";
            richTextBox1.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            cur = cur + 1;
            if (cur > FileCount)
                cur = 1;
            this.skinPictureBox1.Image = Image.FromFile(@"pic_all\" + cur + ".jpg");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = @"pic_all\" + cur + ".txt";
            richTextBox1.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);
            
        }

        private void buttonCollect_Click(object sender, EventArgs e)
        {
            if (登录界面.mail== "")
                MessageBox.Show("未登录！");
            else
            {
                try
                {
                    SqlConnection myconn = new SqlConnection(@"Data Source=.\SQLEXPRESS ; Initial Catalog=OT_user ; Integrated Security=true");
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
    }
}
