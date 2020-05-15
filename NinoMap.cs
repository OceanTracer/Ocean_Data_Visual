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
    public partial class NinoMap : Form
    {
        public NinoMap()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Convert.ToInt32(dateTimePicker1.Text)> Convert.ToInt32(dateTimePicker2.Text)|| Convert.ToInt32(dateTimePicker4.Text) > Convert.ToInt32(dateTimePicker3.Text))
            {
                MessageBox.Show("时间区间选择有误");
                label5.Visible = false;
            }
            else
            {
                nino.ref_start = dateTimePicker1.Text;
                nino.ref_final = dateTimePicker2.Text;
                nino.aim_start = dateTimePicker4.Text;
                nino.aim_final = dateTimePicker3.Text;
                NinoShow form = new NinoShow();
                form.Owner = this;
                Hide();
                form.ShowDialog();
            }
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            label5.Visible = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Close();
            Owner.Show();
            Dispose();
        }

        private void NinoMap_Load(object sender, EventArgs e)
        {
            string MY = "";
            if (UserStatus.status == 1)
                MY = 用户主页.MAXYEAR;
            else if (UserStatus.status == 0)
                MY = 管理员页面.MAXYEAR;

            if (MY != null)
            {
                DateTime d1 = Convert.ToDateTime(MY + "-12-31");

                dateTimePicker2.MaxDate = d1;
                dateTimePicker3.MaxDate = d1;
            }
        }
    }
}
