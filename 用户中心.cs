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
using System.Runtime.InteropServices;
using DnsClient.Protocol;
using CCWin.SkinControl;
using System.IO;

namespace Data_Visual
{
    public partial class 用户中心 : Form
    {
        public 用户中心()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            fill_info(登录界面.mail);
            InfoGroupBox.BringToFront();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        string mysql;
        DataSet mydataset = new DataSet();
        private void fill_info(string mail)
        {
            try
            {
                mysql = "select uname, sex, desire, describe, portrait, u_status, experience from user_info where umail='" + mail+ "'";
                SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
                mydataset.Clear();
                myadapter.Fill(mydataset, "info");
                labelUname.Text = labelUname2.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][0]);
                labelSex.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][1]);
                labelDesire.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][2]);
                labelDesc.Text = Convert.ToString(mydataset.Tables["info"].Rows[0][3]);
                labelMail.Text = mail;
                string level = mydataset.Tables["info"].Rows[0]["u_status"].ToString();
                string exp = mydataset.Tables["info"].Rows[0]["experience"].ToString();
                labelLevel.Text = "Lv." + level + "     Exp." + exp;
                if (!mydataset.Tables["info"].Rows[0]["portrait"].IsNull())
                {//若用户上传过头像，则显示用户头像；否则显示系统默认头像
                    byte[] bytes = new byte[0];
                    bytes = (byte[])mydataset.Tables["info"].Rows[0][4];
                    MemoryStream mystream = new MemoryStream(bytes);
                    //用指定的数据流来创建一个image图片
                    Image portrait = Image.FromStream(mystream, true);
                    ovalShape1.BackgroundImage = portrait;
                }
            }
            catch (Exception)
            {
                ;//MessageBox.Show("请先完善个人信息！");
                //return;
            }
        }




        private void InfoLabel_Click(object sender, EventArgs e)
        {
            fill_info(登录界面.mail);
            NoticeBox.SendToBack();
            InfoGroupBox.BringToFront();
            /*InfoGroupBox.Show();
            RecordGroupBox.Hide();
            CVGroupBox.Hide();
            EditGroupBox.Hide();*/
        }


        private void HomeLabel_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
            //求职者主页 f_see = new 求职者主页();
            //f_see.ShowDialog();
        }


        private void LogoutButton_Click(object sender, EventArgs e)
        {
            Close();
            welcome f_wel = new welcome();
            f_wel.ShowDialog();
            
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            InfoGroupBox.SendToBack();
            EditGroupBox.BringToFront();
            radioButtonMan.Checked = true;
            textBoxMail.Text = 登录界面.mail;
            textBoxName.Text = labelUname2.Text;
            textBoxDesire.Text = labelDesire.Text;
            textBoxDescribe.Text = labelDesc.Text;
            if (labelSex.Text == "男")
                radioButtonMan.Checked = true;
            if (labelSex.Text == "女")
                radioButtonWoman.Checked = true;
            /*InfoGroupBox.Hide();
            CVGroupBox.Hide();
            RecordGroupBox.Hide();
            EditGroupBox.Show();*/
        }

        private void shapeContainer1_Load(object sender, EventArgs e)
        {

        }
        TimeSpan sign_intervel;
        private void 用户中心_Load(object sender, EventArgs e)
        {
            fill_info(登录界面.mail);
            string sql = "select last_sign from user_info where umail='" + 登录界面.mail + "'";
            SqlDataAdapter myadapter = new SqlDataAdapter(sql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "last_signtime");
            if(mydataset.Tables["last_signtime"].Rows[0][0].ToString()!="")
            {
                DateTime dt;
                dt = Convert.ToDateTime(mydataset.Tables["last_signtime"].Rows[0][0]);//数据库中存的时间
                                                                                      // MessageBox.Show((DateTime.Now - dt).ToString());
                                                                                      //MessageBox.Show((Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")) -Convert.ToDateTime( dt.ToString("yyyy-MM-dd"))).ToString());
                DateTime dt1 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));//只留下日期
                DateTime dt2 = Convert.ToDateTime(dt.ToString("yyyy-MM-dd"));
                sign_intervel = dt1 - dt2;
                if (sign_intervel.TotalDays >= 1)  //注意测试的时候不要出现负数时间！
                {
                    button1.Text = "每日签到";
                    button1.Enabled = true;
                }
                else
                {
                    button1.Text = "已签到";
                    button1.Enabled = false;
                }
            }
        }

        private void MyFav_Click(object sender, EventArgs e)
        {
            if (登录界面.mail == "")
                MessageBox.Show("未登录！");
            else
            {
                我的收藏 f1 = new 我的收藏();
                f1.Owner = this;
                Hide();
                f1.ShowDialog();
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string sex="";
            if (radioButtonMan.Checked == true)
                sex = "男";
            if (radioButtonWoman.Checked == true)
                sex = "女";
            mysql = "update user_info set uname='" + textBoxName.Text + "', sex='" + sex + "' , desire='" + textBoxDesire.Text + "' , describe='" + textBoxDescribe.Text + "' where umail='" + 登录界面.mail+"'";
                SqlCommand mycmd = new SqlCommand(mysql, myconn);
                myconn.Open();
                try
                {
                    mycmd.ExecuteNonQuery();
                    MessageBox.Show("修改成功！", "提示");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            myconn.Close();

            /*EditGroupBox.Hide();
            InfoGroupBox.Show();*/
            EditGroupBox.SendToBack();
            NoticeBox.SendToBack();
            InfoGroupBox.BringToFront();
            fill_info(登录界面.mail);
        }

        private List<string[]> GetNotice(string umail)
        {
            List<string[]> noticeList = new List<string[]>();
            //按时间倒序查询此用户收到的通知
            string sql = "select notice_content, notice_time from notice where umail='" + umail + "'order by notice_time desc";
            string ntc_content, ntc_time;
            SqlDataAdapter myadapter = new SqlDataAdapter(sql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "notice");
            for (int i = 0; i < mydataset.Tables["notice"].Rows.Count; i++)
            {
                ntc_content = mydataset.Tables["notice"].Rows[i][0].ToString();
                ntc_time = mydataset.Tables["notice"].Rows[i][1].ToString();
                string[] ntc = new string[] { ntc_content, ntc_time };
                noticeList.Add(ntc);
            }
            return noticeList;
        }

        private void RecordLabel_Click(object sender, EventArgs e)
        {
            NoticeBox.BringToFront();

            listView1.Clear();
            listView1.Columns.Add("通知内容", 400);
            listView1.Columns.Add("通知时间", 150);
            List<string[]> history= GetNotice(登录界面.mail);
            for (int i = 0; i < history.Count; i++)
            {
                ListViewItem It = new ListViewItem();
                It.Text = history[i][0];
                It.SubItems.Add(history[i][1]);
                listView1.Items.Add(It);
            }
            this.listView1.View = System.Windows.Forms.View.Details;
        }

        private void runlabel_Click(object sender, EventArgs e)
        {
            if (登录界面.mail == "")
                MessageBox.Show("未登录！");
            else
            {
                知识闯关 f1 = new 知识闯关();
                f1.Owner = this;
                Hide();
                f1.ShowDialog();
                fill_info(登录界面.mail);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int exp_bonus = sign_intervel.TotalDays == 1 ? 8 : 6;   //连续签到+8，否则+6
            string mycmd = "update user_info set last_sign=getdate(),experience=experience+" + exp_bonus.ToString() + " where umail='" + 登录界面.mail + "'";
            SqlCommand sqlCommand = new SqlCommand(mycmd, myconn);
            Console.WriteLine(mycmd);
            try
            {
                myconn.Open();
                {
                    sqlCommand.ExecuteNonQuery();
                    if (exp_bonus == 6)
                        MessageBox.Show("签到成功！经验+" + exp_bonus.ToString() + ".", "Ocean");
                    else
                        MessageBox.Show("连续签到！经验+" + exp_bonus.ToString() + ".", "Ocean");
                    button1.Text = "已签到";
                    button1.Enabled = false;                  
                }
                myconn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                myconn.Close();
            }
            fill_info(登录界面.mail);
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        private void ovalShape1_Click(object sender, EventArgs e)
        {
            /*选择头像并在本地显示*/
            OpenFileDialog opdia = new OpenFileDialog();
            opdia.Title = "上传头像";
            opdia.Filter = "图片|*.jpg|*.png|*.bmp";
            if (opdia.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = opdia.FileName;
            Image portrait = Image.FromFile(filename);
            ovalShape1.BackgroundImage = portrait;
            /*上传数据库*/
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
            fs.Close();
            try
            {
                myconn.Open();
                string cmdText = "update user_info set portrait=@imgfile where umail='" + 登录界面.mail + "'";
                SqlCommand cmd = new SqlCommand(cmdText, myconn);
                SqlParameter para = new SqlParameter("@imgfile", SqlDbType.Image);
                para.Value = bytes;
                cmd.Parameters.Add(para);
                int res = cmd.ExecuteNonQuery();
                if (res > 0)
                    MessageBox.Show("上传头像成功！", "Ocean");
                myconn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void label6_Click(object sender, EventArgs e)
        {
            社区交流 f1 = new 社区交流();
            f1.Owner = this;
            Hide();
            f1.ShowDialog();
            fill_info(登录界面.mail);
        }
    }
}
