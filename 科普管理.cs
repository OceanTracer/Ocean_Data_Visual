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
        SqlConnection myconn = new SqlConnection(@"Data Source=. ; Initial Catalog=OT_user ; Integrated Security=true");
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
            string kp_no = comboBox1.Text;
            string pic_name = "pic_all\\" + kp_no + ".jpg";
            string txt_name = "pic_all\\" + kp_no + ".txt";
            label3.Text = pic_name;
            if(inopen==1)
            {
                pictureBox1.Image.Save(pic_name);
            }
            TXT_write(txt_name);
            GetKPNum();
            MessageBox.Show("保存成功！");
            inopen = 0;
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
            string pic_name = "pic_all\\" + (kp_lastnum + 1).ToString()+".jpg";
            string txt_name = "pic_all\\" + (kp_lastnum + 1).ToString() + ".txt";
            //SaveFileDialog sfd = new SaveFileDialog();
            if (pictureBox2.Image!=null)
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
                pictureBox2.Image=null;
                richTextBox2.Text="";
                GetKPNum();
            }
            else
            {
                MessageBox.Show("No picture!");
            }
        }

        private void button8_Click(object sender, EventArgs e)
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
            MessageBox.Show("删除成功");
            GetKPNum();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
