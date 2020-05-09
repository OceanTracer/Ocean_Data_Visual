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
    public partial class 用户信息 : Form
    {
        public 用户信息()
        {
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        //SqlConnection myconn = new SqlConnection(@"Data Source=.\SQLEXPRESS ;Initial Catalog=OT_user ; Integrated Security=true");
        string mysql;
        DataSet mydataset = new DataSet();

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }

        private void 用户信息_Load(object sender, EventArgs e)
        {
            ListViewInit();
        }

        void ListViewInit()
        {
            listView1.Clear();

            listView1.Columns.Add("用户邮箱", 120);
            listView1.Columns.Add("用户名", 70);
            listView1.Columns.Add("性别", 70);
            listView1.Columns.Add("兴趣", 70);
            listView1.Columns.Add("权限", 70);
            listView1.Columns.Add("账号状态", 70);
            listView1.Columns.Add("描述", 250);

            mysql = "select umail, uname, sex, desire, u_status, enabled, describe from user_info";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "info");

            for (int i = 0; i < mydataset.Tables["info"].Rows.Count; i++)
            {
                ListViewItem lt = new ListViewItem();
                string status_temp;
                string enable_temp;
                lt.Text = mydataset.Tables["info"].Rows[i][0].ToString();
                for (int j = 1; j < 7; j++)
                {
                    //账号身份判断
                    if (j == 4)
                    {
                        status_temp = "用户";
                        if (mydataset.Tables["info"].Rows[i][j].ToString() == "0")
                            status_temp = "管理员";
                        lt.SubItems.Add(status_temp);
                    }
                    //账号封禁状态判断
                    else if (j == 5)
                    {
                        enable_temp = "可用";
                        if (mydataset.Tables["info"].Rows[i][j].ToString() == "N")
                            enable_temp = "禁用";
                        lt.SubItems.Add(enable_temp);
                    }

                    else
                        lt.SubItems.Add(mydataset.Tables["info"].Rows[i][j].ToString());
                }
                listView1.Items.Add(lt);
            }
            this.listView1.View = System.Windows.Forms.View.Details;
        }

        void ListViewUpdate()
        {
            //针对用户可用/禁用更新
            mysql = "select umail, uname, sex, desire, u_status, enabled, describe from user_info";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "info");

            string enable_temp = "可用";
            for (int i = 0; i<listView1.Items.Count; i++)
            {
                enable_temp = "可用";
                //MessageBox.Show(mydataset.Tables["info"].Rows[i][5].ToString());
                if (mydataset.Tables["info"].Rows[i][5].ToString() == "N")
                {
                    enable_temp = "禁用";
                }
                listView1.Items[i].SubItems[5].Text = enable_temp;
            }
            
        }

        /// <summary>
        /// 根据用户名查找用户邮箱；需要在listView1已被填充后调用
        /// </summary>
        /// <param name="uname">用户名</param>
        private List<string> SearchUser(string uname)
        {
            int index = 0;
            List<string> umail = new List<string>();
            while (true)
            {
                ListViewItem res = listView1.FindItemWithText(uname, true, index, false);

                if (res == null && index == 0)
                {
                    umail[0] = "";
                    break;//未查找到
                }
                else if (res == null && index != 0)
                {
                    break;//查找完毕
                }
                else
                {
                    umail.Add(res.SubItems[0].Text);
                    index = res.Index + 1;
                }
             }

            return umail;
                
        }

        /// <summary>根据用户邮箱封禁用户</summary>
        /// <param name="umail">用户邮箱</param>
        private void DisableUser(string umail)
        {
            string sql = "update user_info set enabled='N' where umail='" + umail + "'";
            SqlCommand mycmd = new SqlCommand(sql, myconn);
            myconn.Open();
            try
            {
                mycmd.ExecuteNonQuery();
                MessageBox.Show("已禁用用户"+umail, "OceanTracer");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }

        /// <summary>根据用户邮箱解封用户</summary>
        /// <param name="umail">用户邮箱</param>
        private void EnableUser(string umail)
        {
            string sql = "update user_info set enabled='Y' where umail='" + umail + "'";
            SqlCommand mycmd = new SqlCommand(sql, myconn);
            myconn.Open();
            try
            {
                mycmd.ExecuteNonQuery();
                MessageBox.Show("已启用用户" + umail, "OceanTracer");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }

        /// <summary>统计科普收藏量</summary>
        /// 本函数将<科普ID,收藏人数>的键值对储存在了一个字典中并返回，可以利用字典中的数据实现各种展示；
        /// 实际使用时直接用DataSet的SQL查询结果也可以，字典只是将前后端分离的中转站（实际上也有将查询结果转换为合理类型储存、避免多次查询的作用）
        private Dictionary<int, int> CollectCount()
        {
            Dictionary<int, int> collects = new Dictionary<int, int>();
            string sql = @"select collect_num, count(*) as 'count' from collect group by collect_num UNION
            select collect_num, 0 as 'count' from collect_info where collect_num not in(select collect_num from collect)";
            SqlDataAdapter myadapter = new SqlDataAdapter(sql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "count");
            if (mydataset.Tables["count"].Rows.Count != 0)
            {
                for (int i = 0; i < mydataset.Tables["count"].Rows.Count; i++)
                {
                    int collect_num = Convert.ToInt32(mydataset.Tables["count"].Rows[i][0]);
                    int count = Convert.ToInt32(mydataset.Tables["count"].Rows[i][1]);
                    collects[collect_num] = count;
                }
            }
            return collects;
            /*eg: how to access the data in a Dictionary*/
            //foreach(KeyValuePair<int, int> clct in collects)
            //{
            //    Console.Write("科普ID："+clct.Key+" 收藏数量："+clct.Value);
            //}
        }

        /// <summary>统计不同兴趣数量</summary>
        /// 本函数将<兴趣,人数>的键值对储存在了一个字典中并返回，可以利用字典中的数据实现各种展示；
        /// 与上一个函数不同的是，因为desire属性在数据库中合并储存，因此必须手动分割计数，不能直接用DataSet的结果
        private Dictionary<string, int> DesireCount()
        {
            Dictionary<string, int> desires = new Dictionary<string, int>();
            string sql = "select desire from user_info";
            SqlDataAdapter myadapter = new SqlDataAdapter(sql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "dcount");
            for (int i = 0; i < mydataset.Tables["dcount"].Rows.Count; i++)
            {
                string des = mydataset.Tables["dcount"].Rows[i][0].ToString();
                string[] splits = des.Split(',');
                foreach (string s in splits)
                    if (!desires.ContainsKey(s))
                        desires[s] = 1;
                    else
                        desires[s]++;
            }
            return desires;
            /*eg: how to access the data in a Dictionary*/
            //foreach(KeyValuePair<string, int> dsr in collects)
            //{
            //    Console.Write("兴趣："+dsr.Key+" 人数："+dsr.Value);
            //}
        }


        /// <summary>向指定用户发送通知</summary>
        /// <param name="umail">用户邮箱</param>
        /// <param name="notice_content">通知内容，最大长度400(中文字符长度为2)</param>
        private void SendNotice(string umail, string notice_content)
        {
            string sql = "insert into notice values('" + umail + "','" + notice_content + "',GETDATE())";
            SqlCommand mycmd = new SqlCommand(sql, myconn);
            myconn.Open();
            try
            {
                mycmd.ExecuteNonQuery();
                MessageBox.Show("通知发送成功", "OceanTracer");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }

        /// <summary>向所有用户群发通知</summary>
        /// <param name="notice_content">通知内容，最大长度400(中文字符长度为2)</param>
        private void SendGroupNotice(string notice_content)
        {
            SqlCommand mycmd = new SqlCommand("groupNotice", myconn);   //利用数据库的存储过程实现
            mycmd.CommandType = CommandType.StoredProcedure;
            SqlParameter content = new SqlParameter("@notice_content ", SqlDbType.VarChar, 400);
            mycmd.Parameters.Add(content);
            content.Value = notice_content;
            myconn.Open();
            try
            {
                mycmd.ExecuteNonQuery();
                MessageBox.Show("通知群发成功", "OceanTracer");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }

        ///<summary>用户查询收到的通知</summary>
        ///<param name="umail">用户邮箱</param>
        ///本函数将[通知内容,通知时间]的二元组按通知时间倒序储存在了一个List中并返回，可以用List中的数据实现各种展示；
        ///实际使用时也可以直接用DataSet的查询结果输出
        ///先将该函数移植至用户中心  管理员位置改为查询通知历史记录功能
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
                string[] ntc = new string[] { ntc_content, ntc_time};
                noticeList.Add(ntc);
            }
            return noticeList;
            /*eg: how to access the data in a List<string[]>*/
            //for(int i=0;i<noticeList.Count;i++)
            //    Console.Write("通知内容：" + noticeList[i][0] + " 时间：" + noticeList[i][1]);
        }

        private List<string[]> GetNoticeHistory()
        {
            List<string[]> noticeList = new List<string[]>();
            //按时间倒序查询此用户收到的通知
            string sql = "select umail, notice_content, notice_time from notice";
            string umail, ntc_content, ntc_time;
            SqlDataAdapter myadapter = new SqlDataAdapter(sql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "notice");
            for (int i = 0; i < mydataset.Tables["notice"].Rows.Count; i++)
            {
                umail = mydataset.Tables["notice"].Rows[i][0].ToString();
                ntc_content = mydataset.Tables["notice"].Rows[i][1].ToString();
                ntc_time = mydataset.Tables["notice"].Rows[i][2].ToString();
                string[] ntc = new string[] { umail, ntc_content, ntc_time };
                noticeList.Add(ntc);
            }
            return noticeList;
            /*eg: how to access the data in a List<string[]>*/
            //for(int i=0;i<noticeList.Count;i++)
            //    Console.Write("通知内容：" + noticeList[i][0] + " 时间：" + noticeList[i][1]);
        }

        private void 定义_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage1;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage2;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage3;
            Collect_statistics();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage4;
            Desire_statistics();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage5;
            Notice_Update();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            label6.Visible = false;
            label7.Text = "";
            List<string> result = new List<string>();
            if (textBox1.Text.Trim() == "")
                MessageBox.Show("用户名不能为空");
            else 
                result = SearchUser(textBox1.Text);
            if (result[0] == "")
                MessageBox.Show("未查找到该用户");
            else
            {
                label6.Visible = true;
                for (int i = 0; i < result.Count; i++)
                    label7.Text = label7.Text + result[i] + "\n";
            }
            ListViewInit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string umail = textBox2.Text;
            ListViewItem res = listView1.FindItemWithText(umail, true, 0, false);
            if (umail == "")
                MessageBox.Show("用户邮箱不能为空");
            else if (res == null)
                MessageBox.Show("未找到该用户");
            else if (res.SubItems[4].Text == "管理员")
                MessageBox.Show("无法封禁管理员账户");
            else
                DisableUser(umail);
            ListViewUpdate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string umail = textBox2.Text;
            ListViewItem res = listView1.FindItemWithText(umail, true, 0, false);
            if (umail == "")
                MessageBox.Show("用户邮箱不能为空");
            else if (res == null)
                MessageBox.Show("未找到该用户");
            else if (res.SubItems[4].Text == "管理员")
                MessageBox.Show("无法解禁管理员账户");
            else
                EnableUser(umail);
            ListViewUpdate();
        }

        private void Collect_statistics()
        {
            listView2.Clear();
            
            Dictionary<int, int> collects = CollectCount();

            listView2.Columns.Add("科普序号", 140);
            listView2.Columns.Add("收藏人数", 140);
            for ( int i = 1; i <= collects.Count; i++)
            {
                ListViewItem It = new ListViewItem();
                It.Text = i.ToString();
                It.SubItems.Add(collects[i].ToString());
                listView2.Items.Add(It);
            }
            this.listView2.View = System.Windows.Forms.View.Details;
        }

        private void Desire_statistics()
        {
            listView3.Clear();

            Dictionary<string, int> desires = DesireCount();

            listView3.Columns.Add("兴趣", 140);
            listView3.Columns.Add("感兴趣人数", 140);
            foreach (KeyValuePair<string, int> dsr in desires)
            {
                if (dsr.Key == "")
                    continue;
                ListViewItem It = new ListViewItem();
                It.Text = dsr.Key.ToString();
                It.SubItems.Add(dsr.Value.ToString());
                listView3.Items.Add(It);
            }
            this.listView3.View = System.Windows.Forms.View.Details;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string umail = textBox3.Text;
            string content = richTextBox1.Text;
            ListViewItem res = listView1.FindItemWithText(umail, true, 0, false);
            if (umail == "")
                MessageBox.Show("用户邮箱不能为空");
            else if (content == "")
                MessageBox.Show("通知内容不能为空");
            else if (res == null)
                MessageBox.Show("未找到该用户");
            else if (res.SubItems[4].Text == "管理员")
                MessageBox.Show("无法通知管理员");
            else
            {
                DialogResult dr = MessageBox.Show("您确定要通知以下内容 \n" + richTextBox1.Text + "\n吗？", "通知确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    SendNotice(umail, content);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string broadcast = richTextBox1.Text; 
            if(broadcast == "")
                MessageBox.Show("广播内容不能为空");
            else
            {
                DialogResult dr = MessageBox.Show("您确定要广播以下内容 \n" + richTextBox1.Text + "\n吗？", "广播确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr == DialogResult.OK)
                {
                    SendGroupNotice(broadcast);
                }
            }
        }

        private void Notice_Update()
        {
            listView4.Clear();
            listView4.Columns.Add("用户邮箱", 120);
            listView4.Columns.Add("通知内容", 400);
            listView4.Columns.Add("通知时间", 200);

            List<string[]> history = GetNoticeHistory();
            for (int i = 0; i < history.Count; i++)
            {
                ListViewItem It = new ListViewItem();
                It.Text = history[i][0];
                It.SubItems.Add(history[i][1]);
                It.SubItems.Add(history[i][2]);
                listView4.Items.Add(It);
            }
            this.listView4.View = System.Windows.Forms.View.Details;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label12.Visible = true;
            listView4.Visible = true;
            Notice_Update();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
