using CoronaTracker.Instances;
using CoronaTracker.SubForms;
using CoronaTracker.Timers;
using CoronaTracker.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CoronaTracker.Enums.EmployeePoseEnum;

namespace CoronaTracker
{
    public partial class UI : Form
    {

        private Form currentChildForm = null;
        private Button currentButton = null;

        public UI()
        {
            LogClass.Log($"Start initialize sub sub form");
            InitializeComponent();

            logLabel.Hide();
            label2.Text = "version: " + ProgramVariables.Version;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Region = new Region(path);

            OpenChildForm(new HomeSubForm(), button6);
            LogClass.Log($"Sub sub form successfully initialized");

        }
        
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);
        private void UpperPanel_MouseDown(object sender, MouseEventArgs e)
        {
            LogClass.Log($"UpperPanel mouse down event handler start");
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
            LogClass.Log($"UpperPanel mouse down event handler end");
        }

        public void UpdateProfilePicture()
        {
            pictureBox2.ImageLocation = ProgramVariables.ProfileURL;
        }

        public void OpenChildForm(Form childForm, Button button)
        {
            LogClass.Log($"Start open child form");
            // Setup child form
            if (currentChildForm != null)
            {
                currentChildForm.Hide();
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
            LogClass.Log($"Successfully opened child form");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button7 click event handler start");
            try
            {
                File.Delete("temp.jpg");
                File.Delete("temp.pdf");
            }
            catch
            {

            }
            LogClass.Log($"Ending program");
            LogClass.Save();
            ProgramVariables.LoginUI.EndApp();
            Application.Exit();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void UI_VisibleChanged(object sender, EventArgs e)
        {
            LogClass.Log($"UI visible changed event handler start");
            if (ProgramVariables.Fullname != null)
            {
                label4.Text = ProgramVariables.Fullname;
                pictureBox2.ImageLocation = ProgramVariables.ProfileURL;
            }
            LogClass.Log($"UI visible changed event handler end");
        }

        private void label3_Click(object sender, EventArgs e)
        {
            LogClass.Log($"label3 click event handler start");
            ProgramVariables.Fullname = null;
            ProgramVariables.ID = 0;
            ProgramVariables.ProfileURL = null;
            ProgramVariables.LoginUI.Show();
            this.Hide();
            LogClass.Log($"label3 click event handler end");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LogClass.Log($"button6 click event handler start");

            /*if (((Button)sender).Text.Equals("Home"))
                OpenChildForm(new HomeSubForm(), (Button)sender);
            else if (((Button)sender).Text.Equals("Countries"))
                OpenChildForm(new CountriesSubForm(), (Button)sender);
            else if (((Button)sender).Text.Equals("Vaccine") && HasAccess())
                OpenChildForm(new VaccineTypeSubForm(), (Button)sender);
            else if (((Button)sender).Text.Equals("Patient") && HasAccess())
                OpenChildForm(new PatientSubForm(), (Button)sender);
            else if (((Button)sender).Text.Equals("Dashboard"))
                OpenChildForm(new DashboardSubForm(), (Button)sender);
            else if (((Button)sender).Text.Equals("Settings") && HasAccess())
                OpenChildForm(new SettingsSubForm(), (Button)sender);*/

            if (((Button)sender).Text.Equals(currentButton.Text))
            {
                LogClass.Log("User tried to open same form");
                return;
            }

            if (((Button)sender).Text.Equals("Vaccine") ||
               ((Button)sender).Text.Equals("Patient") ||
               ((Button)sender).Text.Equals("Settings"))
            {
                if (HasAccess())
                {
                    OpenChildForm(ProgramVariables.FormCache[((Button)sender).Text], (Button)sender);
                }
                else
                {
                    MessageBox.Show("Unfortunately, you do not have access to this section.\n\nTip: Ask your leader to change you user status");
                }
            }
            else
                OpenChildForm(ProgramVariables.FormCache[((Button)sender).Text], (Button)sender);

            LogClass.Log($"button6 click event handler end");
        }

        private bool HasAccess()
        {
            if (ProgramVariables.Pose == EmployeePose.User)
                return false;
            else
                return true;
        }

        public Form GetCurrentForm()
        {
            return currentChildForm;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!ProgramVariables.FormCache["REPORT"].Visible)
                ProgramVariables.FormCache["REPORT"].Show();
        }
    }
}
