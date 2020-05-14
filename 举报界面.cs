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
    public partial class 举报界面 : Form
    {
        public 举报界面()
        {
            InitializeComponent();
        }
        public string reason = "";

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(richTextBox1.Text=="")
            {
                MessageBox.Show("请填写举报原因！", "Ocean");
                return;
            }
            DialogResult dr = MessageBox.Show("您确定要举报该内容吗？您的举报会被记录在案！", "确定举报", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                reason = richTextBox1.Text;
                Close();
                /*sql="update collect set isComplaint='Y',Com_content='"+richTextBox1.Text.ToString()+"'" +
                  " where umail='" + 登录界面.mail + "' and collect_num="+account.click_num.ToString()+"";*/
            }
        }
    }
}
