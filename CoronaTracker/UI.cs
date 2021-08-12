using CoronaTracker.SubForms;
using CoronaTracker.Timers;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoronaTracker
{
    public partial class UI : Form
    {

        private Form currentChildForm = null;
        private Button currentButton = null;

        public UI()
        {
            InitializeComponent();

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Region = new Region(path);

            OpenChildForm(new HomeSubForm(), button6);

        }
        
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void UpperPanel_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        public void OpenChildForm(Form childForm, Button button)
        {

            // Setup child form
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.Dock = DockStyle.Fill;
            SubFormPanel.Controls.Add(childForm);
            SubFormPanel.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

            // Setup button visualation
            if(currentButton != null)
            {
                currentButton.TextAlign = ContentAlignment.MiddleCenter;
                currentButton.BackColor = Color.FromArgb(33, 33, 33);
            }
            currentButton = button;
            panel2.Location = currentButton.Location;
            currentButton.TextAlign = ContentAlignment.MiddleRight;
            currentButton.BackColor = Color.FromArgb(44, 44, 44);
            currentButton.TextAlign = ContentAlignment.MiddleRight;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ProgramVariables.LoginUI.EndApp();
            Application.Exit();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void UI_VisibleChanged(object sender, EventArgs e)
        {
            if(ProgramVariables.Fullname != null)
            {
                label4.Text = ProgramVariables.Fullname;
                pictureBox2.ImageLocation = ProgramVariables.ProfileURL;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            ProgramVariables.Fullname = null;
            ProgramVariables.ID = 0;
            ProgramVariables.ProfileURL = null;
            ProgramVariables.LoginUI.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Text.Equals("Home"))
                OpenChildForm(new HomeSubForm(), (Button)sender);
            else if (((Button)sender).Text.Equals("Countries"))
                OpenChildForm(new CountriesSubForm(), (Button)sender);
            else if (((Button)sender).Text.Equals("Vaccine"))
                OpenChildForm(new VaccineTypeSubForm(), (Button)sender);
            else if (((Button)sender).Text.Equals("Patient"))
                OpenChildForm(new PatientSubForm(), (Button)sender);
            else if (((Button)sender).Text.Equals("Dashboard"))
                OpenChildForm(new DashboardSubForm(), (Button)sender);
        }

        public Form GetCurrentForm()
        {
            return currentChildForm;
        }

    }
}
