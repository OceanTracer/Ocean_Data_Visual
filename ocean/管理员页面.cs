using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ocean
{
    public partial class 管理员页面 : Form
    {
        public 管理员页面()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
            welcome f15 = new welcome();
            f15.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {/* 
            Hide();
            发布招聘 f1=new 发布招聘();
            f1.Owner = this;
            f1.ShowDialog();*/
        }

        private void label2_Click(object sender, EventArgs e)
        {
          /*   Hide();
            简历筛选 f2=new 简历筛选();
            f2.Owner = this;
            f2.ShowDialog();*/
        }

        private void label3_Click(object sender, EventArgs e)
        {
            /*    Hide();
               修改信息 f3 = new 修改信息();
               f3.Owner = this;
               f3.ShowDialog();*/
        }
    }
}
