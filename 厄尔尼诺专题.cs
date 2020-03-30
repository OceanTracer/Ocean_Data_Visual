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
    public partial class 厄尔尼诺专题 : Form
    {
        public 厄尔尼诺专题()
        {
            InitializeComponent();
        }

        private void 厄尔尼诺专题_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage3;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage4;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage5;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }
    }
}
