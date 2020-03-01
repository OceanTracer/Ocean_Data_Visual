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
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(Convert.ToInt32(dateTimePicker1.Text)> Convert.ToInt32(dateTimePicker2.Text)|| Convert.ToInt32(dateTimePicker4.Text) > Convert.ToInt32(dateTimePicker3.Text))
            {
                MessageBox.Show("时间区间选择有误");
            }
            else
            {
                nino.ref_start = dateTimePicker1.Text;
                nino.ref_final = dateTimePicker2.Text;
                nino.aim_start = dateTimePicker4.Text;
                nino.aim_final = dateTimePicker3.Text;
                NinoShow form = new NinoShow();
                form.ShowDialog();
            }
        }
    }
}
