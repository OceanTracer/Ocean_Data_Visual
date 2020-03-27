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

namespace ocean
{
    public partial class 科普界面new : Form
    {
        public 科普界面new()
        {
            InitializeComponent();
        }

        int FileCount = 0;
        private void 科普界面new_Load(object sender, EventArgs e)
        {
            this.skinPictureBox1.Image = Image.FromFile(@"pic_all\01.jpg");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = @"pic_all\01.txt";
            richTextBox1.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);

            /////获取文件数目

            DirectoryInfo Dir = new DirectoryInfo(@"pic_all");
            foreach (FileInfo FI in Dir.GetFiles())
            {
                if (System.IO.Path.GetExtension(FI.Name) == ".txt")
                {
                    FileCount++;
                }
            }
        }

        int cur = 1;
        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            cur = cur - 1;
            if (cur < 1)
                cur = FileCount;
            this.skinPictureBox1.Image = Image.FromFile(@"pic_all\0" + cur + ".jpg");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = @"pic_all\0" + cur + ".txt";
            richTextBox1.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            cur = cur + 1;
            if (cur > FileCount)
                cur = 1;
            this.skinPictureBox1.Image = Image.FromFile(@"pic_all\0" + cur + ".jpg");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = @"pic_all\0" + cur + ".txt";
            richTextBox1.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.PlainText);
            
        }

        private void buttonCollect_Click(object sender, EventArgs e)
        {
            MessageBox.Show("收藏成功！", "Ocean");
        }

        private void buttonQuit_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }
    }
}
