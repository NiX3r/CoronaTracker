using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoronaTracker.SubForms
{
    public partial class ReportBug : Form
    {
        public ReportBug()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Checks if all parameters are wrote
            if(!textBox1.Text.Equals("") && !richTextBox1.Text.Equals("") && listBox1.SelectedItem != null)
            {
                // TODO
            }

        }
    }
}
