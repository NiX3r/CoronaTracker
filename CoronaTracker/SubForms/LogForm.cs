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
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();

            Log = new writeLog(writeLogMethod);

        }

        /// <summary>
        /// Create delegete to write logs
        /// </summary>
        public delegate void writeLog(string log);
        public writeLog Log;
        void writeLogMethod(string log) 
        { 
            richTextBox1.Text += log; 
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret(); 
            if (richTextBox1.Text.Length >= 2147483000) 
                richTextBox1.Clear(); 
        }

    }
}
