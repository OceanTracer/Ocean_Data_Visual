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
    public partial class Option : Form
    {
        public Option()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TimeMap form = new TimeMap();
            form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GridMap form = new GridMap();
            form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NinoMap form = new NinoMap();
            form.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DataProcess form = new DataProcess();
            form.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            用户主页 form = new 用户主页();
            form.Owner = this;
            Hide();
            form.ShowDialog();
        }

        private void Option_Load(object sender, EventArgs e)
        {
            if (登录界面.type == 1)
                button4.Visible = false;
            if (登录界面.type == 0)
                button4.Visible = true;
        }
    }
}
