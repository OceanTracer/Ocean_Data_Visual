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

namespace Data_Visual
{
    public partial class 知识闯关 : Form
    {
        public 知识闯关()
        {
            InitializeComponent();
        }
        SqlConnection myconn = new SqlConnection(@"Data Source=" + sql_source.dt_source + " ; Initial Catalog=OT_user;User ID=sa;Password=Cptbtptp123");
        string mysql;
        DataSet mydataset = new DataSet();
        static int N = 10;//闯关题目数量N<count_all
        int count_all = 15;//总共的题目数量
        int[] num = new int[N];//随机数数组，load的时候初始化
        int i=0;//遍历序号
        int right_num = 0;//回答题目的正确数量

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            mysql = "select contents, A, B, C, D from question where num='" + num[0] + "'";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "ques");
            contentlabel.Text= Convert.ToString(mydataset.Tables["ques"].Rows[0][0]);
            radioButton1.Text= Convert.ToString(mydataset.Tables["ques"].Rows[0][1]);
            radioButton2.Text = Convert.ToString(mydataset.Tables["ques"].Rows[0][2]);
            radioButton3.Text = Convert.ToString(mydataset.Tables["ques"].Rows[0][3]);
            radioButton4.Text = Convert.ToString(mydataset.Tables["ques"].Rows[0][4]);
            button2.Enabled = false;
            button4.Enabled = false;
        }
        public int[] MakeRandom(int MaxValue, int Number)
        {
            Random random = new Random();
            int[] itemIds = new int[Number];

            int[] temp = new int[MaxValue];
            for (int i = 0; i < MaxValue; i++)
                temp[i] = i+1;

            for (int i = 0; i < Number; i++)
            {
                int r = random.Next(0, MaxValue-1); //0-14代表1-15
                itemIds[i] = temp[r];
                for (int j = r; j < MaxValue - 1; j++)
                {
                    temp[j] = temp[j + 1];
                }
                MaxValue--;
            }
            return itemIds;
        }

        private void 知识闯关_Load(object sender, EventArgs e)
        {
            anslable.Visible = false;
            num = MakeRandom(count_all, N);
            mysql = "select num from question";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "ques1");
            count_all = mydataset.Tables["ques1"].Rows.Count;
            //MessageBox.Show(count_all.ToString());
            panel1.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = (i + 1).ToString();
            anslable.Visible = false;
            mysql = "select contents, A, B, C, D from question where num='" + num[i] + "'";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "ques");
            contentlabel.Text = Convert.ToString(mydataset.Tables["ques"].Rows[0][0]);
            radioButton1.Text = Convert.ToString(mydataset.Tables["ques"].Rows[0][1]);
            radioButton2.Text = Convert.ToString(mydataset.Tables["ques"].Rows[0][2]);
            radioButton3.Text = Convert.ToString(mydataset.Tables["ques"].Rows[0][3]);
            radioButton4.Text = Convert.ToString(mydataset.Tables["ques"].Rows[0][4]);
            button4.Enabled = false;
            button5.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            anslable.Visible = true;
            char ans;
            mysql = "select right_ans from question where num='" + num[i] + "'";
            SqlDataAdapter myadapter = new SqlDataAdapter(mysql, myconn);
            mydataset.Clear();
            myadapter.Fill(mydataset, "ans1");
            i = i + 1;
            if (radioButton1.Checked)
                ans = 'A';
            else if (radioButton2.Checked)
                ans = 'B';
            else if (radioButton3.Checked)
                ans = 'C';
            else
                ans = 'D';
            if (ans == Convert.ToChar(mydataset.Tables["ans1"].Rows[0][0]))
            {
                anslable.Text = "回答正确！";
                right_num = right_num + 1;
            }
            else
            {
                anslable.Text="回答错误！正确答案是"+ Convert.ToChar(mydataset.Tables["ans1"].Rows[0][0])+ "";
            }

            if (i == N)
            {
                MessageBox.Show("恭喜回答完毕！共回答对" + right_num.ToString() + "道题目，经验+" + right_num.ToString() + ".");
                button5.Enabled = false;
                i = 0;
                UpdateExp();
            }
            else
            {
                button4.Enabled = true;
            }
            button5.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(button4.Enabled==false & button5.Enabled==false)
            {
                Close();
                Owner.Show();
                Dispose();
            }
            else
            {
                MessageBox.Show("共回答对" + right_num.ToString() + "道题目，经验+" + right_num.ToString() + ".");
                UpdateExp();
                Close();
                Owner.Show();
                Dispose();
            }
        }

        private void UpdateExp()
        {
            mysql = "update user_info set experience=experience+" + right_num.ToString() + "where umail='" + 登录界面.mail + "'";
            SqlCommand mycmd = new SqlCommand(mysql, myconn);
            myconn.Open();
            try
            {
                mycmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            myconn.Close();
        }
    }
}
