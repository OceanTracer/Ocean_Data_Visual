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
    public partial class welcome : Form
    {
        public welcome()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Close();
            System.Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            注册界面 f_reg = new 注册界面();
            f_reg.Owner = this;
            f_reg.ShowDialog();
            //Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            登录界面 f_log = new 登录界面();
            f_log.Owner = this;
            f_log.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
