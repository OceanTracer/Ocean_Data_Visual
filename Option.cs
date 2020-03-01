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

        }
    }
}
