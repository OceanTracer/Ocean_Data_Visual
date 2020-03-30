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
    public partial class 科普界面_选择 : Form
    {
        public 科普界面_选择()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            科普界面new f1 = new 科普界面new();
            f1.Owner = this;
            Hide();
            f1.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            厄尔尼诺专题 f1 = new 厄尔尼诺专题();
            f1.Owner = this;
            Hide();
            f1.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }
    }
}
