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
        SqlConnection myconn = new SqlConnection(@"Data Source=.\SQLEXPRESS ; Initial Catalog=OT_user ; Integrated Security=true");
        string mysql;
        DataSet mydataset = new DataSet();
        int kp_lastnum;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

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
                        pictureBox1.Image = Image.FromFile(filename);
                        //Console.WriteLine(pathName + '\\' + fileName+".nc");
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

        public void TXT_write(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(richTextBox1.Text);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string kp_no = comboBox1.Text;
            string pic_name = System.Windows.Forms.Application.StartupPath + "\\pic_all\\'" + kp_no + ".jpg";
            string txt_name = System.Windows.Forms.Application.StartupPath + "\\pic_all\\'" + kp_no + ".txt";

            if (File.Exists(pic_name))
            {
                File.Delete(pic_name);
            }

            if (File.Exists(txt_name))
            {
                File.Delete(txt_name);
            }

            pictureBox1.Image.Save(pic_name, System.Drawing.Imaging.ImageFormat.Jpeg);
            TXT_write(txt_name);
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
                        pictureBox2.Image = Image.FromFile(filename);
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
            file.Filter = "文本文件|*.png;*.jpg";

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
                        TXT_read(strpath);
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
            string pic_name = System.Windows.Forms.Application.StartupPath + "\\pic_all\\'" + (kp_lastnum + 1).ToString() + ".jpg";
            string txt_name = System.Windows.Forms.Application.StartupPath + "\\pic_all\\'" + (kp_lastnum + 1).ToString() + ".txt";
            mysql = "insert into collect_info VALUES('" + (kp_lastnum + 1).ToString() + "','" + pic_name + "','" + txt_name + "')";
            SqlCommand mycmd = new SqlCommand(mysql, myconn);
            myconn.Open();
            {
                mycmd.ExecuteNonQuery();
            }
            myconn.Close();
            MessageBox.Show("添加成功");
            GetKPNum();
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
            MessageBox.Show("删除成功");
            GetKPNum();
        }
    }
}
