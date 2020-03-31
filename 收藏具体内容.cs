using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Data_Visual
{
    public partial class 收藏具体内容 : Form
    {
        public 收藏具体内容()
        {
            InitializeComponent();
        }

        private void 收藏具体内容_Load(object sender, EventArgs e)
        {
            int numb = account.click_num;
            this.pictureBox1.Image = Image.FromFile(@"pic_all\" + numb.ToString() + ".jpg");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = @"pic_all\" + numb.ToString() + ".txt";
            richTextBox1.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);
        }

        private void richTextBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
