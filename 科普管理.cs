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
using System.Text;

namespace Data_Visual
{
    public partial class 科普管理 : Form
    {
        public 科普管理()
        {
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source="+sql_source.dt_source+" ; Initial Catalog=OT_user ; Integrated Security=true");
        string mysql;
        DataSet mydataset = new DataSet();
        int kp_lastnum;
        int inopen = 0;//判断保存修改那里是否修改了图片！
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label3.Text = comboBox1.Text;
            try
            { 
            FileStream pFileStream = new FileStream(@"pic_all\" + comboBox1.Text.ToString() + ".jpg", FileMode.Open, FileAccess.Read);
            pictureBox1.Image = Image.FromStream(pFileStream);
            pFileStream.Close();
            pFileStream.Dispose();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = @"pic_all\" + comboBox1.Text.ToString() + ".txt";
            richTextBox1.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);
            }
            catch
            { }
        }

        private void 科普管理_Load(object sender, EventArgs e)
        {
            GetKPNum();
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
            pictureBox1.Image = null;
            pictureBox4.Image = null;
            richTextBox1.Text = "";
            richTextBox3.Text = "";
        }

       void GetKPNum()
        {
            mysql = "select collect_num from collect_info";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "info");
            //MessageBox.Show(mydataset.Tables["info"].Rows[0][0].ToString());
            comboBox1.DisplayMember = "collect_num";
            comboBox1.DataSource = mydataset.Tables["info"];
            comboBox2.DisplayMember = "collect_num";
            comboBox2.DataSource = mydataset.Tables["info"];
            int last = mydataset.Tables["info"].Rows.Count;
            kp_lastnum = Convert.ToInt32(mydataset.Tables["info"].Rows[last - 1][0].ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true;
            file.Filter = "图像文件|*.png;*.jpg";

            string strpath;
            string filename;

            if (file.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string this_file in file.FileNames)
                    {
                        strpath = file.FileName;
                        filename = strpath.Substring(strpath.LastIndexOf("\\") + 1);//去掉了路径
                        pictureBox1.Image = Image.FromFile(strpath);
                        //Console.WriteLine(pathName + '\\' + fileName+".nc");
                        inopen = 1;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true;
            file.Filter = "文本文件|*.txt";

            string strpath;
            string filename;

            if (file.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    richTextBox1.Text = "";
                    foreach (string this_file in file.FileNames)
                    {
                        strpath = file.FileName;
                        filename = strpath.Substring(strpath.LastIndexOf("\\") + 1);//去掉了路径
                        TXT_read(strpath);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void TXT_read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                richTextBox1.Text += line.ToString();
            }
            sr.Close();
        }
        public void TXT_read2(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                richTextBox2.Text += line.ToString() + "\r\n"; 
            }
            sr.Close();
        }

        public void TXT_write(string path)
        {
            richTextBox1.SaveFile(path, RichTextBoxStreamType.PlainText);
        }
        public void TXT_write2(string path)
        {
            richTextBox2.SaveFile(path, RichTextBoxStreamType.PlainText);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("您确定要修改"+comboBox1.Text+"号科普的内容吗？", "修改确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                string kp_no = comboBox1.Text;
                string pic_name = "pic_all\\" + kp_no + ".jpg";
                string txt_name = "pic_all\\" + kp_no + ".txt";
                label3.Text = pic_name;
                if (inopen == 1)
                {
                    pictureBox1.Image.Save(pic_name);
                }
                TXT_write(txt_name);
                GetKPNum();
                MessageBox.Show("保存成功！");
                inopen = 0;
                comboBox1.SelectedItem = null;
                pictureBox1.Image = null;
                richTextBox1.Text = "";
            }
            else
            {       
                //
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true;
            file.Filter = "图像文件|*.png;*.jpg";

            string strpath;
            string filename;

            if (file.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string this_file in file.FileNames)
                    {
                        strpath = file.FileName;
                        filename = strpath.Substring(strpath.LastIndexOf("\\") + 1);//去掉了路径
                        pictureBox2.Image = Image.FromFile(strpath);
                        //Console.WriteLine(pathName + '\\' + fileName+".nc");
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true;
            file.Filter = "文本文件|*.txt";

            string strpath;
            string filename;

            if (file.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string this_file in file.FileNames)
                    {
                        strpath = file.FileName;
                        filename = strpath.Substring(strpath.LastIndexOf("\\") + 1);//去掉了路径
                        TXT_read2(strpath);
                       // richTextBox2.Text = TXT_read(strpath);
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("您确定要添加如下科普内容吗？", "添加确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                string pic_name = "pic_all\\" + (kp_lastnum + 1).ToString() + ".jpg";
                string txt_name = "pic_all\\" + (kp_lastnum + 1).ToString() + ".txt";
                //SaveFileDialog sfd = new SaveFileDialog();
                if (pictureBox2.Image != null)
                {
                    pictureBox2.Image.Save(pic_name);
                    richTextBox2.SaveFile(txt_name, RichTextBoxStreamType.PlainText);
                    mysql = "insert into collect_info VALUES('" + (kp_lastnum + 1).ToString() + "','" + pic_name + "','" + txt_name + "')";
                    SqlCommand mycmd = new SqlCommand(mysql, myconn);
                    myconn.Open();
                    {
                        mycmd.ExecuteNonQuery();
                    }
                    myconn.Close();
                    MessageBox.Show("添加成功");
                    pictureBox2.Image = null;
                    richTextBox2.Text = "";
                    GetKPNum();
                }
                else
                {
                    MessageBox.Show("没有上传图片!");
                }
            }
            else
            {
                //
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("您确定要删除" + comboBox2.Text + "号科普的内容吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                string to_delete = comboBox2.Text;
                mysql = "delete from collect_info where collect_num = '" + to_delete + "'";
                SqlCommand mycmd = new SqlCommand(mysql, myconn);
                myconn.Open();
                {
                    mycmd.ExecuteNonQuery();
                }
                myconn.Close();
                //删除文件夹文件！
                string pic_name = "pic_all\\" + to_delete + ".jpg";
                string txt_name = "pic_all\\" + to_delete + ".txt";
                File.Delete(pic_name);
                File.Delete(txt_name);

                //更新删除文件后的序号
                int to_update = Convert.ToInt32(to_delete);
                for (to_update = Convert.ToInt32(to_delete) + 1; to_update <= kp_lastnum; to_update++)
                {
                    string srcjpgFileName = @"pic_all\" + to_update.ToString() + ".jpg";
                    string destjpgFileName = @"pic_all\" + (to_update - 1).ToString() + ".jpg";
                    string srctxtFileName = @"pic_all\" + to_update.ToString() + ".txt";
                    string desttxtFileName = @"pic_all\" + (to_update - 1).ToString() + ".txt";
                    if (File.Exists(srcjpgFileName))
                    {
                        File.Move(srcjpgFileName, destjpgFileName);
                    }
                    if (File.Exists(srctxtFileName))
                    {
                        File.Move(srctxtFileName, desttxtFileName);
                    }
                    mysql = "update collect_info set collect_num = '" + (to_update - 1).ToString() + "', collect_pic = '" + destjpgFileName + "', collect_txt = '" + desttxtFileName + "' where collect_num =  '" + to_update.ToString() + "'";
                    SqlCommand updatecmd = new SqlCommand(mysql, myconn);
                    myconn.Open();
                    {
                        updatecmd.ExecuteNonQuery();
                    }
                    myconn.Close();
                }

                MessageBox.Show("删除成功");
                GetKPNum();
                comboBox2.SelectedItem = null;
                pictureBox4.Image = null;
                richTextBox3.Text = "";
            }
            else
            {
                //
            }
 
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FileStream pFileStream = new FileStream(@"pic_all\" + comboBox2.Text.ToString() + ".jpg", FileMode.Open, FileAccess.Read);
                pictureBox4.Image = Image.FromStream(pFileStream);
                pFileStream.Close();
                pFileStream.Dispose();
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.FileName = @"pic_all\" + comboBox2.Text.ToString() + ".txt";
                richTextBox3.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);
            }
            catch
            { }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
            pictureBox1.Image = null;
            pictureBox4.Image = null;
            richTextBox1.Text = "";
            richTextBox3.Text = "";
        }

        private void label4_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage1;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage3;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage2;
        }
    }
}