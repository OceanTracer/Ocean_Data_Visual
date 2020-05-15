using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Office.Interop.Excel;//Excel
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;

namespace Data_Visual
{
    public partial class 科普管理 : Form
    {
        public 科普管理()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        string mysql;
        string sql;
        Image img;
        DataSet mydataset = new DataSet();
        DataSet mydataset1 = new DataSet();
        int kp_lastnum;
        int kp_nopass;//待审核的总数
        int inopen = 0;//判断保存修改那里是否修改了图片！
        string filename_img;//imgfile filename_img = file.FileName;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label3.Text = comboBox1.Text;
            try
            {
                fetchCollect(Convert.ToInt32(comboBox1.Text));
            }
            catch
            { }
        }

        private void 科普管理_Load(object sender, EventArgs e)
        {
            GetKPNum();
            NotPass();
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
            pictureBox1.Image = null;
            pictureBox4.Image = null;
            richTextBox1.Text = "";
            richTextBox3.Text = "";
        }

       void GetKPNum()
        {
            //已经通过审核的
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
        void NotPass()
        {
            string mysql1 = "select collect_num from collect_info where approved='N'";
            SqlDataAdapter myadapter1 = new SqlDataAdapter(mysql1, myconn);
            mydataset1.Clear();
            myadapter1.Fill(mydataset1, "info1");
            //MessageBox.Show(mydataset.Tables["info"].Rows[0][0].ToString());
            comboBox3.DisplayMember = "collect_num";
            comboBox3.DataSource = mydataset1.Tables["info1"];
            int last = mydataset1.Tables["info1"].Rows.Count;
            if(last!=0)
            { kp_nopass = Convert.ToInt32(mydataset1.Tables["info1"].Rows[last - 1][0].ToString()); }
            
        }

        private void fetchCollect(int id)
        {
            byte[] bytes = new byte[0];
            sql = @"select collect_txt, collect_pic from collect_info 
                    where collect_num=" + id.ToString();
            SqlCommand cmd = new SqlCommand(sql, myconn);
            try
            {
                myconn.Open();
                SqlDataReader sdr = cmd.ExecuteReader();
                sdr.Read();
                richTextBox1.Text = sdr["collect_txt"].ToString();
                richTextBox3.Text = sdr["collect_txt"].ToString();
                richTextBox4.Text = sdr["collect_txt"].ToString();
                bytes = (byte[])sdr["collect_pic"];
                sdr.Close();
                myconn.Close();
                MemoryStream mystream = new MemoryStream(bytes);
                //用指定的数据流来创建一个image图片
                img = Image.FromStream(mystream, true);
                pictureBox1.Image = img;
                pictureBox4.Image = img;
                pictureBox6.Image = img;
                mystream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog opdia = new OpenFileDialog();
            opdia.Title = "请选择图像";
            opdia.Filter = "图片|*.jpg|*.png|*.bmp";
            if (opdia.ShowDialog() == DialogResult.OK)
            {
                filename_img = opdia.FileName;
                pictureBox1.Load(filename_img);
                inopen = 1;
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
                int id = Convert.ToInt32(comboBox1.Text);
                if (inopen == 1)//如果==1，修改图片和文字
                {
                    FileStream fs = new FileStream(filename_img, FileMode.Open, FileAccess.Read);
                    Byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                    try
                    {
                        myconn.Open();
                        string cmdText = "update collect_info set collect_txt='" + richTextBox1.Text + "',collect_pic=@imgfile where collect_num="+id.ToString()+" ";
                        //string cmdText = "insert into collect_info values('" + richTextBox1.Text + "', @imgfile, null)";
                        SqlCommand cmd = new SqlCommand(cmdText, myconn);
                        SqlParameter para = new SqlParameter("@imgfile", SqlDbType.Image);
                        para.Value = bytes;
                        cmd.Parameters.Add(para);

                        int res = cmd.ExecuteNonQuery();
                        if (res > 0)
                            MessageBox.Show("修改成功！", "Ocean");
                        myconn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else //修改文字即可
                {
                    string cmdText = "update collect_info set collect_txt='" + richTextBox1.Text + "' where collect_num=" + id.ToString() + " ";
                    SqlCommand cmd = new SqlCommand(cmdText, myconn);
                    try
                    {
                        myconn.Open();
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("修改成功！", "Ocean");

                        }
                        myconn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                        return;
                    }
                }
                GetKPNum();
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
            Dispose();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = true;
            file.Filter = "图像文件|*.png;*.jpg";

            string filename;

            if (file.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    foreach (string this_file in file.FileNames)
                    {
                        filename_img = file.FileName;
                        filename = filename_img.Substring(filename_img.LastIndexOf("\\") + 1);//去掉了路径
                        pictureBox2.Image = Image.FromFile(filename_img);
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
                int id = kp_lastnum + 1; //****
                if (inopen == 1)
                {
                    FileStream fs = new FileStream(filename_img, FileMode.Open, FileAccess.Read);
                    Byte[] bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                    try
                    {
                        myconn.Open();
                        string cmdText = "insert into collect_info values('"+id.ToString()+"','" + richTextBox2.Text + "', @imgfile,'" + 登录界面.mail + "','Y')";
                        SqlCommand cmd = new SqlCommand(cmdText, myconn);
                        SqlParameter para = new SqlParameter("@imgfile", SqlDbType.Image);
                        para.Value = bytes;
                        cmd.Parameters.Add(para);

                        int res = cmd.ExecuteNonQuery();
                        if (res > 0)
                            MessageBox.Show("修改成功！", "Ocean");
                        myconn.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                GetKPNum();
                NotPass();
                inopen = 0;
                pictureBox2.Image = null;
                richTextBox2.Text = "";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("您确定要删除" + comboBox2.Text + "号科普的内容吗？", "删除确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                string to_delete = comboBox2.Text;
                mysql = "delete from collect_info where collect_num = '" + to_delete + "'";
                SqlCommand mycmd1 = new SqlCommand(mysql, myconn);

                myconn.Open();
                {
                    mycmd1.ExecuteNonQuery();//删除收藏
                }
                myconn.Close();

                int to_update = Convert.ToInt32(to_delete);
                for (to_update = Convert.ToInt32(to_delete) + 1; to_update <= kp_lastnum; to_update++)
                {
                    //更新删除文件后的序号
                    string mysql2 = "update collect_info set collect_num = '" + (to_update - 1).ToString() + "'  where collect_num =  '" + to_update.ToString() + "'";
                    SqlCommand updatecmd1 = new SqlCommand(mysql2, myconn);

                    string mysql3 = "update collect set collect_num = '" + (to_update - 1).ToString() + "' where collect_num =  '" + to_update.ToString() + "'";
                    SqlCommand updatecmd2 = new SqlCommand(mysql3, myconn);

                    myconn.Open();
                    {
                        updatecmd1.ExecuteNonQuery();
                        updatecmd2.ExecuteNonQuery();
                    }
                    myconn.Close();
                    //}
                }
                MessageBox.Show("删除成功");
                GetKPNum();
                NotPass();
                comboBox2.SelectedItem = null;
                pictureBox4.Image = null;
                richTextBox3.Text = "";
            }
            else
            {
                //
            }

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                fetchCollect(Convert.ToInt32(comboBox2.Text));
            }
            catch
            { }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = null;
            comboBox2.SelectedItem = null;
            comboBox3.SelectedItem = null;
            pictureBox1.Image = null;
            pictureBox4.Image = null;
            pictureBox6.Image = null;
            richTextBox1.Text = "";
            richTextBox3.Text = "";
            richTextBox4.Text = "";
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

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                fetchCollect(Convert.ToInt32(comboBox3.Text));
            }
            catch
            { }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage4;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            mysql = "update collect_info set approved= 'Y'  where collect_num =  '" + comboBox3.Text + "'";
            SqlCommand updatecmd1 = new SqlCommand(mysql, myconn);
            myconn.Open();
            {
                updatecmd1.ExecuteNonQuery();
            }
            myconn.Close();
            MessageBox.Show("审核成功！");
            GetKPNum();
            NotPass();
            comboBox3.SelectedItem = null;
            pictureBox6.Image = null;
            richTextBox4.Text = "";

        }

        private void label8_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage5;
        }

        public static object[,] GetExcelRangeData(string excelPath, string stCell)
        {
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            Workbook workBook = null;
            object oMissiong = Missing.Value;
            try
            {
                workBook = app.Workbooks.Open(excelPath, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong,
                    oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong, oMissiong);
                if (workBook == null)
                    return null;


                Worksheet workSheet = (Worksheet)workBook.Worksheets.Item[1];
                int rowCount = workSheet.UsedRange.Cells.Rows.Count;
                string edCell;
                edCell = "F" + rowCount.ToString();
                //使用下述语句可以从头读取到最后，按需使用
                //var maxN = workSheet.Range[startCell].End[XlDirection.xlDown].Row;
                return workSheet.Range[stCell + ":" + edCell].Value2;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                //COM组件方式调用完记得释放资源
                if (workBook != null)
                {
                    workBook.Close(false, oMissiong, oMissiong);
                    Marshal.ReleaseComObject(workBook);
                    app.Workbooks.Close();
                    app.Quit();
                    Marshal.ReleaseComObject(app);
                }
            }
        }

        string dt2ctname;
        int row_count;
        System.Data.DataTable dt = new System.Data.DataTable();
        private void button9_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            System.Data.DataTable dt2 = new System.Data.DataTable();
            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "EXCEL FILE|*.xlsx;*.xls";
            string strpath;
            string filename;
            if (file.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    strpath = file.FileName;
                    filename = strpath.Substring(strpath.LastIndexOf("\\") + 1);//去掉了路径
                    dt2ctname = filename.Substring(0, filename.LastIndexOf("."));//去掉后缀名
                    object[,] data = GetExcelRangeData(strpath, "A2");
                    for (int i = 0; i < data.GetLength(1); i++)
                        dt2.Columns.Add(i.ToString(), typeof(object));
                    //MessageBox.Show("读好了");
                    row_count = data.GetLength(0);
                    for (int i = 0; i < data.GetLength(0); i++)
                    {
                        object[] dr = new object[data.GetLength(1)];
                        for (int j = 0; j < data.GetLength(1); j++)
                        {
                            dr[j] = data[i + 1, j + 1];
                        }
                        dt2.Rows.Add(dr);
                        Console.WriteLine(i.ToString());
                    }
                    //MessageBox.Show("读好了");
                    BindingSource bs = new BindingSource();
                    bs.DataSource = dt2;
                    dataGridView1.DataSource = bs;
                    dataGridView1.Columns[0].HeaderCell.Value = "题干";
                    dataGridView1.Columns[1].HeaderCell.Value = "A";
                    dataGridView1.Columns[2].HeaderCell.Value = "B";
                    dataGridView1.Columns[3].HeaderCell.Value = "C";
                    dataGridView1.Columns[4].HeaderCell.Value = "D";
                    dataGridView1.Columns[5].HeaderCell.Value = "正确选项";
                    dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                    dt = dt2;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            int id;
            if (dt2ctname == null)
                return;
            try
            {
                if (dt != null)
                {
                    mysql = "select count(num) from question";                
                    SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
                    mydataset.Clear();
                    myadapter.Fill(mydataset, "question");
                    id = Convert.ToInt32(mydataset.Tables["question"].Rows[0][0]);
                    for(int i=0;i<row_count;i++)
                    {
                        string mysql = "insert into question values('" + (id + i + 1).ToString() + "','" + dataGridView1.Rows[i].Cells[0].Value.ToString() + "','" + dataGridView1.Rows[i].Cells[1].Value.ToString() + "','" + dataGridView1.Rows[i].Cells[2].Value.ToString() + "','" + dataGridView1.Rows[i].Cells[3].Value.ToString() + "','" + dataGridView1.Rows[i].Cells[4].Value.ToString() + "','" + dataGridView1.Rows[i].Cells[5].Value.ToString() + "')";
                        //Console.WriteLine(mysql);
                        SqlCommand updatecmd1 = new SqlCommand(mysql, myconn);
                        myconn.Open();
                        {
                            updatecmd1.ExecuteNonQuery();
                        }
                        myconn.Close();
                    }
                    MessageBox.Show("添加成功！");
                }
                else
                {
                    MessageBox.Show("DataTable为空，请先导入数据");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("格式错误！请重新导入");
            }
        }
    }
}