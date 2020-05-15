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
    public partial class 帖子详情 : Form
    {
        int post_id; string post_umail;
        public 帖子详情(int id)
        {
            InitializeComponent();
            post_id = id;
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        string sql;
        DataSet mydataset = new DataSet();
        int lastseq;
        private void 帖子详情_Load(object sender, EventArgs e)
        {
            FetchPost(post_id);
            flowLayoutPanel1.Controls.Clear();
            FetchReplies(post_id);
        }

        int status;
        void get_status()
        {
            string mysql = "select u_status from user_info where umail='" + 登录界面.mail + "'";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "status");
            status = Convert.ToInt32(mydataset.Tables["status"].Rows[0][0]);

        }

        private void FetchPost(int post_id)
        {
            sql = @"SELECT post_title,posts.umail,uname,post_content,post_time,post_repcnt
                    FROM posts,user_info
                    WHERE post_deleted='N' and post_id=" + post_id.ToString() + "and posts.umail=user_info.umail";
            SqlDataAdapter myadapter = new SqlDataAdapter(sql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "post");
            labelTitle.Text = mydataset.Tables["post"].Rows[0]["post_title"].ToString();
            labelPoster.Text = "发帖人：" + mydataset.Tables["post"].Rows[0]["uname"].ToString();
            labelTime.Text = mydataset.Tables["post"].Rows[0]["post_time"].ToString();
            labelRepcnt.Text = "回复量：" + mydataset.Tables["post"].Rows[0]["post_repcnt"].ToString();
            richTextBox1.Text = mydataset.Tables["post"].Rows[0]["post_content"].ToString();
            post_umail = mydataset.Tables["post"].Rows[0]["umail"].ToString();
        }

        private void FetchReplies(int post_id)
        {
            sql = @"SELECT rep_content,replies.umail,uname,rep_time,rep_id
                    FROM replies,user_info
                    WHERE rep_deleted='N' and post_id=" + post_id.ToString() + @" and replies.umail=user_info.umail
                    ORDER BY rep_time";
            SqlDataAdapter myadapter = new SqlDataAdapter(sql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "replies");
            string rep_content, umail, uname, rep_time, rep_id;
            for (int i = 0; i < mydataset.Tables["replies"].Rows.Count; i++)
            {
                rep_content = mydataset.Tables["replies"].Rows[i]["rep_content"].ToString();
                umail = mydataset.Tables["replies"].Rows[i]["umail"].ToString();
                uname = mydataset.Tables["replies"].Rows[i]["uname"].ToString();
                rep_time = mydataset.Tables["replies"].Rows[i]["rep_time"].ToString();
                rep_id = mydataset.Tables["replies"].Rows[i]["rep_id"].ToString();
                AddReplyPanel(rep_content, uname, rep_time, umail, rep_id, i+1);
            }
            lastseq = mydataset.Tables["replies"].Rows.Count;
        }

        private void AddReplyPanel(string rep_content, string uname, string rep_time, string umail, string rep_id, int seq)
        {
            Panel panel = new Panel();
            Label labelContent = new Label();
            Label labelTime = new Label();
            Label labelName = new Label();
            Label labelSeq = new Label();
            Label labelReport = new Label();

            panel.BorderStyle = BorderStyle.Fixed3D;
            panel.Size = new Size(flowLayoutPanel1.Width-27, 58);
            panel.Controls.Add(labelContent);
            panel.Controls.Add(labelTime);
            panel.Controls.Add(labelName);
            panel.Controls.Add(labelSeq);
            panel.Controls.Add(labelReport);
            panel.Font = new Font("张海山锐线体简", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            
            labelContent.Font = new Font("张海山锐线体简", 10.8F);
            labelContent.Location = new Point(13, 7);
            labelName.Location = new Point(13, 36);
            labelTime.Location = new Point(73, 36);
            labelSeq.Font = new Font("张海山锐线体简", 9F, FontStyle.Bold);
            labelSeq.Location = new Point(panel.Width - 39, 7);
            labelReport.Font=new Font("张海山锐线体简", 9F, FontStyle.Italic | FontStyle.Underline, GraphicsUnit.Point, ((byte)(134)));
            labelReport.ForeColor = SystemColors.HotTrack;
            labelReport.Cursor = Cursors.Hand;
            labelReport.Text = "举报";
            labelReport.Tag = new string[] { umail, rep_id};   //用于举报时标识被举报人和rep_id
            labelReport.Click += ReplyReport_Click;
            labelReport.Location = new Point(panel.Width - 51, 34);
            //labelTime.Location = new Point(panel.Width - 121, 34);

            labelContent.AutoSize = labelName.AutoSize = labelSeq.AutoSize = labelTime.AutoSize = true;
            labelContent.Text = rep_content;
            labelTime.Text = rep_time;
            labelName.Text = uname;
            labelSeq.Text = "#" + seq.ToString();

            flowLayoutPanel1.Controls.Add(panel);
        }

        private void ReplyReport_Click(object sender, EventArgs e)
        {
            get_status();
            if (status < 7 && status>0)
            {
                MessageBox.Show("您的等级不足7级，请继续加油！");
                return;
            }
            Label l = (Label)sender;
            string[] para = (string[])l.Tag;
            string rep_umail = para[0], rep_id = para[1], reason;
            举报界面 fr = new 举报界面();
            fr.ShowDialog();
            reason = fr.reason;
            if (reason == "")
                return;
            sql = "insert into report values('" + 登录界面.mail + "','" + rep_umail +
                "','reply'," + rep_id + ",'" + reason + "',GETDATE(),'N')";
            SqlCommand mycmd = new SqlCommand(sql, myconn);
            myconn.Open();
            try
            {
                int res = mycmd.ExecuteNonQuery();
                if (res > 0)
                    MessageBox.Show("举报成功！", "Ocean");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();

        }

        private void PostReport_Click(object sender, EventArgs e)
        {
            get_status();
            if (status < 7 && status>0)
            {
                MessageBox.Show("您的等级不足7级，请继续加油！");
                return;
            }
            举报界面 fr = new 举报界面();
            fr.ShowDialog();
            string reason = fr.reason;
            if (reason == "")
                return;
            sql = "insert into report values('" + 登录界面.mail + "','" + post_umail +
                "','post'," + post_id.ToString() + ",'" + reason + "',GETDATE(),'N')";
            SqlCommand mycmd = new SqlCommand(sql, myconn);
            myconn.Open();
            try
            {
                int res = mycmd.ExecuteNonQuery();
                if (res > 0)
                    MessageBox.Show("举报成功！", "Ocean");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            get_status();
            if (status>0 && status <5)
            {
                MessageBox.Show("您的等级不足5级，请继续加油！");
                return;
            }
            if (richTextBox2.Text == "")
            {
                MessageBox.Show("请输入回复内容！", "Ocean");
                return;
            }
            sql = "insert into replies VALUES('" + 登录界面.mail + "'," + post_id.ToString() + ",'" + richTextBox2.Text.Replace("'", "''") + "',GETDATE(),'N')";
            //使用.Replace("'", "''")以防文字中带有单引号'，引起SQL字符串截断
            SqlCommand mycmd = new SqlCommand(sql, myconn);
            myconn.Open();
            try
            {
                int res = mycmd.ExecuteNonQuery();
                if (res > 0)
                {
                    //MessageBox.Show("回复成功", "Ocean");
                    lastseq += 1;
                    AddReplyPanel(richTextBox2.Text, 登录界面.uname, DateTime.Now.ToString(), 登录界面.mail, "1", lastseq);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
            Dispose();
        }
    }
}
