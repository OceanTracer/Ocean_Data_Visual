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
            this.tabControl1.SelectedTab = this.tabPage1;
        }

        private void 定义_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage1;
        }

        private void 成因_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage2;
        }

        private void 表现_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage3;
        }

        private void 影响_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage4;
        }

        private void 实例_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage5;
        }

        private void Nino数据查询_Click(object sender, EventArgs e)
        {
            NinoMap form = new NinoMap();
            form.Owner = this;
            Hide();
            form.ShowDialog();
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
        }
    }
}
