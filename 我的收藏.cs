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
    public partial class 我的收藏 : Form
    {
        public 我的收藏()
        {
            InitializeComponent();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }
        //全局变量
        //int N = 12; //最多只能收藏N个 可能又bug，在前面收藏的地方应该有限制
        int[] num;

        private void 我的收藏_Load(object sender, EventArgs e)
        {
            if (登录界面.mail == "")
                MessageBox.Show("未登录！");
            else
            {
                num = new int[account.N];
                int count_all;
                SqlConnection myconn = new SqlConnection(@"Data Source=. ; Initial Catalog=OT_user ; Integrated Security=true");
                string mysql = "select umail,collect_num from collect where umail='" + 登录界面.mail + "'";
                DataSet mydataset = new DataSet();
                SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
                myadapter.Fill(mydataset, "_email");
                count_all = mydataset.Tables["_email"].Rows.Count;
                for (int i = 0; i < count_all; i++)
                {
                    num[i] = Convert.ToInt32(mydataset.Tables["_email"].Rows[i][1]);
                    imageList1.Images.Add(Image.FromFile(@"pic_all\" + num[i].ToString() + ".jpg"));
                }
                for (int j = 0; j < count_all; j++)
                {
                    PictureBox pic = new PictureBox();
                    pic.Size = new Size(160, 160);
                    pic.Image = this.imageList1.Images[j];
                    pic.SizeMode = PictureBoxSizeMode.StretchImage;
                    if(j<6)
                    {
                        flowLayoutPanel1.Controls.Add(pic);
                    }
                    else
                        flowLayoutPanel2.Controls.Add(pic);
                    pic.Tag = j;
                    pic.MouseClick += new MouseEventHandler(pictureBox1_Click);
                }
                foreach (Control c in flowLayoutPanel1.Controls)
                    c.Margin = new Padding(10, 10, 10, 10);
                foreach (Control c in flowLayoutPanel2.Controls)
                    c.Margin = new Padding(10, 10, 10, 10);
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            pic.Cursor = Cursors.Hand;
            account.click_num = num[Convert.ToInt32(pic.Tag)];
            //MessageBox.Show(num[Convert.ToInt32(pic.Tag)].ToString());
            收藏具体内容 f1 = new 收藏具体内容();
            f1.Owner = this;
            f1.ShowDialog();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage2;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }
    }
}
